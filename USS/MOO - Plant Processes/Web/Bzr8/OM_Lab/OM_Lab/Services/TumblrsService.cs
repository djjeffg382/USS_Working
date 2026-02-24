using MOO.DAL.ToLive.Enums;
using MOO.DAL.ToLive.Services;
using OM_Lab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OM_Lab.Services
{
    public class TumblrsService : ITumblrsService
    {
        // Lab_Phys_Type_Id 14 = Before Tumbles; used as the reference record type
        // for determining the last entered and latest unauthorized shift/half.
        private const int LAB_PHYS_TYPE_BT = 14;

        // Lab_Phys_Type_Id 15 = After Tumbles
        private const int LAB_PHYS_TYPE_AT = 15;

        // How far back to search for the most recently entered record.
        private const int LAST_ENTRY_LOOKBACK_YEARS = 1;

        // How far back to search for the latest unauthorized record.
        private const int UNAUTHORIZED_LOOKBACK_MONTHS = 1;

        /// <inheritdoc />
        public async Task<(DateTime date, int shift, int half)> GetNextShiftHalfAfterLastEntryAsync()
        {
            return await Task.Run(() =>
            {
                var records = Lab_Phys_AnalysisSvc.GetByShiftDate(
                    LAB_PHYS_TYPE_BT,
                    DateTime.Today.AddYears(-LAST_ENTRY_LOOKBACK_YEARS),
                    DateTime.Today,
                    ShiftType.ShiftType8Hour);

                if (!records.Any())
                    return (DateTime.Today, 1, 1);

                var last = records.OrderByDescending(r => r.Analysis_Date).First();

                // Advance one half-shift (4 hours) past the last entry to get the next period.
                DateTime nextTime = last.Analysis_Date.AddHours(4);
                DateTime nextShiftDate = MOO.Shifts.Shift8.ShiftDate(nextTime, MOO.Plant.Keetac);
                int nextShift = MOO.Shifts.Shift8.ShiftNumber(nextTime, MOO.Plant.Keetac);
                int nextHalf = ToWithinShiftHalf(MOO.Shifts.Shift8.HalfShift(nextTime, MOO.Plant.Keetac));

                return (nextShiftDate, nextShift, nextHalf);
            });
        }

        /// <inheritdoc />
        public async Task<(DateTime date, int shift, int half)> GetLatestUnauthorizedShiftHalfAsync()
        {
            return await Task.Run(() =>
            {
                // Query recent records; records without an Authorized_By value (to be
                // populated by issue #7) are candidates for coordinator authorization.
                // Until Authorized_By is added to the model, all recent records are
                // returned and the most recent is used as the default.
                var records = Lab_Phys_AnalysisSvc.GetByShiftDate(
                    LAB_PHYS_TYPE_BT,
                    DateTime.Today.AddMonths(-UNAUTHORIZED_LOOKBACK_MONTHS),
                    DateTime.Today,
                    ShiftType.ShiftType8Hour);

                if (!records.Any())
                    return (DateTime.Today, 1, 1);

                // Filter to records that have not yet been authorized.
                var unauthorized = records.Where(r => string.IsNullOrEmpty(r.Authorized_By)).ToList();
                var candidates = unauthorized.Any() ? unauthorized : records;

                var latest = candidates.OrderByDescending(r => r.Analysis_Date).First();
                int half = ToWithinShiftHalf(MOO.Shifts.Shift8.HalfShift(latest.Analysis_Date, MOO.Plant.Keetac));

                return (latest.Shift_Date8, (int)latest.Shift_Nbr8, half);
            });
        }

        /// <inheritdoc />
        public async Task<Dictionary<int, TumblrLineData>> GetTumblrLinesAsync(DateTime shiftDate, int shiftNumber, int half)
        {
            return await Task.Run(() =>
            {
                // Build an empty result for each pellet line.
                var result = new Dictionary<int, TumblrLineData>();
                for (int line = 3; line <= 7; line++)
                    result[line] = new TumblrLineData { LineNumber = line };

                // Convert the within-shift half (1-2) to the global half of the day (1-6).
                byte globalHalf = (byte)((shiftNumber - 1) * 2 + half);

                // Determine the analysis-date window for this half-shift.
                DateTime[] halfTimes = MOO.Shifts.Shift8.HalfShiftStartEndTime(
                    shiftDate, globalHalf, MOO.Plant.Keetac);
                DateTime halfStart = halfTimes[0];
                DateTime halfEnd   = halfTimes[1];

                // ── Before Tumbles ────────────────────────────────────────────
                var btRecords = Lab_Phys_AnalysisSvc.GetByAnalysisDate(
                    LAB_PHYS_TYPE_BT, halfStart, halfEnd);

                foreach (var rec in btRecords)
                {
                    if (!rec.Line_Nbr.HasValue || !result.ContainsKey(rec.Line_Nbr.Value))
                        continue;

                    var ld = result[rec.Line_Nbr.Value];
                    ld.BtAnalysisId   = rec.Lab_Phys_Analysis_Id;
                    ld.BtAnalysisDate = rec.Analysis_Date;
                    ld.TotWt      = ToDecimal(rec.Start_Wgt);
                    ld.Mesh916_BT = ToDecimal(rec.Inch_9_16_Wgt);
                    ld.Mesh12_BT  = ToDecimal(rec.Inch_1_2_Wgt);
                    ld.Mesh716_BT = ToDecimal(rec.Inch_7_16_Wgt);
                    ld.Mesh38_BT  = ToDecimal(rec.Inch_3_8_Wgt);
                    ld.Mesh14_BT  = ToDecimal(rec.Inch_1_4_Wgt);
                    ld.Mesh28M_BT = ToDecimal(rec.Mesh_28_30_Wgt);
                }

                // ── After Tumbles ─────────────────────────────────────────────
                var atRecords = Lab_Phys_AnalysisSvc.GetByAnalysisDate(
                    LAB_PHYS_TYPE_AT, halfStart, halfEnd);

                foreach (var rec in atRecords)
                {
                    if (!rec.Line_Nbr.HasValue || !result.ContainsKey(rec.Line_Nbr.Value))
                        continue;

                    var ld = result[rec.Line_Nbr.Value];
                    ld.AtAnalysisId   = rec.Lab_Phys_Analysis_Id;
                    ld.AtAnalysisDate = rec.Analysis_Date;
                    ld.TotWt_AT   = ToDecimal(rec.Start_Wgt);
                    // Keep backward-compatibility: when no BT record supplied TotWt, use the AT start weight.
                    if (!ld.TotWt.HasValue)
                        ld.TotWt = ld.TotWt_AT;
                    ld.Mesh916_AT = ToDecimal(rec.Inch_9_16_Wgt);
                    ld.Mesh12_AT  = ToDecimal(rec.Inch_1_2_Wgt);
                    ld.Mesh716_AT = ToDecimal(rec.Inch_7_16_Wgt);
                    ld.Mesh38_AT  = ToDecimal(rec.Inch_3_8_Wgt);
                    ld.Mesh14_AT  = ToDecimal(rec.Inch_1_4_Wgt);
                    ld.Mesh28M_AT = ToDecimal(rec.Mesh_28_30_Wgt);
                }

                return result;
            });
        }

        /// <summary>Converts a nullable <see cref="double"/> from a DAL model to a nullable
        /// <see cref="decimal"/> suitable for UI binding.</summary>
        private static decimal? ToDecimal(double? value) =>
            value.HasValue ? (decimal?)Convert.ToDecimal(value.Value) : null;

        /// <summary>
        /// Converts a global half-shift number (1–6, representing all half-shifts in a day)
        /// to a within-shift half number (1 or 2).
        /// </summary>
        private static int ToWithinShiftHalf(byte globalHalf) => (globalHalf - 1) % 2 + 1;

        /// <inheritdoc />
        public async Task SaveTumblrLinesAsync(DateTime shiftDate, int shiftNumber, int half,
            Dictionary<int, TumblrLineData> lines, string username, string? authorizedBy = null)
        {
            await Task.Run(() =>
            {
                // Compute the analysis-date window for this half-shift.
                byte globalHalf = (byte)((shiftNumber - 1) * 2 + half);
                DateTime[] halfTimes = MOO.Shifts.Shift8.HalfShiftStartEndTime(
                    shiftDate, globalHalf, MOO.Plant.Keetac);
                DateTime halfStart = halfTimes[0];

                var btType = Lab_Phys_TypeSvc.Get(LAB_PHYS_TYPE_BT);
                var atType = Lab_Phys_TypeSvc.Get(LAB_PHYS_TYPE_AT);

                foreach (var kvp in lines)
                {
                    int lineNumber = kvp.Key;
                    var ld = kvp.Value;

                    SaveRecord(ld.BtAnalysisId, btType, lineNumber,
                        shiftDate, shiftNumber, half, halfStart, username, authorizedBy, ld.DefaultsUsed,
                        ld.TotWt, ld.Mesh916_BT, ld.Mesh12_BT, ld.Mesh716_BT,
                        ld.Mesh38_BT, ld.Mesh14_BT, ld.Mesh28M_BT);

                    SaveRecord(ld.AtAnalysisId, atType, lineNumber,
                        shiftDate, shiftNumber, half, halfStart, username, authorizedBy, ld.DefaultsUsed,
                        ld.TotWt_AT, ld.Mesh916_AT, ld.Mesh12_AT, ld.Mesh716_AT,
                        ld.Mesh38_AT, ld.Mesh14_AT, ld.Mesh28M_AT);
                }
            });
        }

        /// <summary>
        /// Inserts or updates a single <c>Lab_Phys_Analysis</c> record.
        /// </summary>
        private static void SaveRecord(
            int analysisId,
            MOO.DAL.ToLive.Models.Lab_Phys_Type physType, int lineNumber,
            DateTime shiftDate, int shiftNumber, int half, DateTime halfStart,
            string username, string? authorizedBy, bool defaultsUsed,
            decimal? startWgt, decimal? mesh916, decimal? mesh12, decimal? mesh716,
            decimal? mesh38, decimal? mesh14, decimal? mesh28)
        {
            if (analysisId == 0)
            {
                // INSERT
                var rec = new MOO.DAL.ToLive.Models.Lab_Phys_Analysis
                {
                    Lab_Phys_Type  = physType,
                    Line_Nbr       = lineNumber,
                    Analysis_Date  = halfStart,
                    Shift_Date8    = shiftDate,
                    Shift_Nbr8     = (short)shiftNumber,
                    Shift_Half8    = (byte)half,
                    Last_Update_By = username,
                    Authorized_By  = authorizedBy,
                    Defaults_Used  = defaultsUsed,
                    Start_Wgt      = ToDouble(startWgt),
                    Inch_9_16_Wgt  = ToDouble(mesh916),
                    Inch_1_2_Wgt   = ToDouble(mesh12),
                    Inch_7_16_Wgt  = ToDouble(mesh716),
                    Inch_3_8_Wgt   = ToDouble(mesh38),
                    Inch_1_4_Wgt   = ToDouble(mesh14),
                    Mesh_28_30_Wgt = ToDouble(mesh28),
                };
                rec.CalcShift12(MOO.Plant.Keetac, MOO.Area.Concentrator);

                Lab_Phys_AnalysisSvc.Insert(rec);
            }
            else
            {
                // UPDATE – load the existing record first to preserve all unchanged fields.
                var rec = Lab_Phys_AnalysisSvc.Get(analysisId);
                rec.Last_Update_By = username;
                if (authorizedBy != null)
                    rec.Authorized_By = authorizedBy;
                rec.Defaults_Used  = defaultsUsed;
                rec.Shift_Half8    = (byte)half;
                rec.Start_Wgt      = ToDouble(startWgt);
                rec.Inch_9_16_Wgt  = ToDouble(mesh916);
                rec.Inch_1_2_Wgt   = ToDouble(mesh12);
                rec.Inch_7_16_Wgt  = ToDouble(mesh716);
                rec.Inch_3_8_Wgt   = ToDouble(mesh38);
                rec.Inch_1_4_Wgt   = ToDouble(mesh14);
                rec.Mesh_28_30_Wgt = ToDouble(mesh28);

                Lab_Phys_AnalysisSvc.Update(rec);
            }
        }

        /// <summary>Converts a nullable <see cref="decimal"/> from UI binding to a nullable
        /// <see cref="double"/> for the DAL model.</summary>
        private static double? ToDouble(decimal? value) =>
            value.HasValue ? (double?)Convert.ToDouble(value.Value) : null;
    }
}
