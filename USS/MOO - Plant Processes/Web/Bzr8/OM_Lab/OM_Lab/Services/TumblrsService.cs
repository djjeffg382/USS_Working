using MOO.DAL.ToLive.Enums;
using MOO.DAL.ToLive.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OM_Lab.Services
{
    public class TumblrsService : ITumblrsService
    {
        // Lab_Phys_Type_Id 14 = Before Tumbles; used as the reference record type
        // for determining the last entered and latest unauthorized shift/half.
        private const int LAB_PHYS_TYPE_BT = 14;

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

                var latest = records.OrderByDescending(r => r.Analysis_Date).First();
                int half = ToWithinShiftHalf(MOO.Shifts.Shift8.HalfShift(latest.Analysis_Date, MOO.Plant.Keetac));

                return (latest.Shift_Date8, (int)latest.Shift_Nbr8, half);
            });
        }

        /// <summary>
        /// Converts a global half-shift number (1–6, representing all half-shifts in a day)
        /// to a within-shift half number (1 or 2).
        /// </summary>
        private static int ToWithinShiftHalf(byte globalHalf) => (globalHalf - 1) % 2 + 1;
    }
}
