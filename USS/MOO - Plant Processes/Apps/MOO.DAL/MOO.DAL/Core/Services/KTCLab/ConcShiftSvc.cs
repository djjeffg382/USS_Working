using MOO.DAL.Core.Models;
using MOO.DAL.Core.Models.KTCLab;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Core.Services.KTCLab
{
    internal static class ConcShiftSvc
    {
        #region "Metric ID Constants"
        //Metric ID's

        public const int ID_HYDRO = 281;
        public const int ID_THICKENER = 282;
        public const int ID_NOLA_SIO2 = 301;
        public const int ID_CONTROL_SIO2 = 302;
        public const int ID_CONC_FE = 621;

        #endregion



        /// <summary>
        /// Fills the Pellet Lab data for the specified shift
        /// </summary>
        /// <param name="ShiftDate"></param>
        /// <param name="ConcShiftList"></param>
        public static void FillConcShift(DateTime ShiftDate, List<ConcShift> ConcShiftList)
        {

            ConcShiftList.Clear();
            //need to create a record for 23,3,7,11,15,19 hours
            //start with 23:00 and go from there
            DateTime startDate = MOO.Shifts.Shift8.ShiftStartEndTime(ShiftDate, 1, Plant.Keetac)[0];
            for (int i = 0; i < 3; i++)
            {
                ConcShift cs = new()
                {
                    Shift_Date = ShiftDate,
                    Start_Date = startDate,
                    Shift = MOO.Shifts.Shift8.ShiftNumber(startDate, Plant.Keetac),
                    Half = MOO.Shifts.Shift8.HalfShift(startDate, Plant.Keetac),
                    Hour = MOO.Shifts.Shift8.ShiftHour(startDate, Plant.Keetac)
                };

                cs.Start_Date_No_DST = cs.Start_Date.IsDaylightSavingTime() ? cs.Start_Date.AddHours(-1) : cs.Start_Date;
                ConcShiftList.Add(cs);
                startDate = startDate.AddHours(8);
            }
            //fill the hydro field
            List<Metric_Value> mvVals = Metric_ValueSvc.GetByShiftDate(ID_HYDRO, ShiftDate, ShiftDate);
            foreach (var mv in mvVals)
            {
                ConcShiftList.FirstOrDefault(a => a.Start_Date_No_DST == mv.Start_Date_No_DST).TailsHydro = mv.Value;
                ConcShiftList.FirstOrDefault(a => a.Start_Date_No_DST == mv.Start_Date_No_DST).Approval = mv.Approval;
            }
            //fill the Thickener field
            mvVals = Metric_ValueSvc.GetByShiftDate(ID_THICKENER, ShiftDate, ShiftDate);
            foreach (var mv in mvVals)
            {
                ConcShiftList.FirstOrDefault(a => a.Start_Date_No_DST == mv.Start_Date_No_DST).TailsThickener = mv.Value;
            }


            //fill the NOLA SIO2 field
            mvVals = Metric_ValueSvc.GetByShiftDate(ID_NOLA_SIO2, ShiftDate, ShiftDate);
            foreach (var mv in mvVals)
            {
                ConcShiftList.FirstOrDefault(a => a.Start_Date_No_DST == mv.Start_Date_No_DST).NOLA_SiO2 = mv.Value;
            }
            //fill the Control SIO2 field
            mvVals = Metric_ValueSvc.GetByShiftDate(ID_CONTROL_SIO2, ShiftDate, ShiftDate);
            foreach (var mv in mvVals)
            {
                //Control SiO2 used to be one value per shift.  Nov 2024 was switched to every 4 hour
                //to handle this we made the Control_SiO2 value an array where item 0 is half 1 and item 1 is half 2
                int half =  (mv.Half.Value - 1) % 2;

                ConcShiftList.FirstOrDefault(a => a.Shift == mv.Shift).Control_SiO2[half] = mv.Value;
            }


            //fill the Conc Fe field
            mvVals = Metric_ValueSvc.GetByShiftDate(ID_CONC_FE, ShiftDate, ShiftDate);
            foreach (var mv in mvVals)
            {
                //Conc Fe used to be one value per shift.  Nov 2024 was switched to every 4 hour
                //to handle this we made the Control_SiO2 value an array where item 0 is half 1 and item 1 is half 2
                int half = (mv.Half.Value - 1) % 2;
                ConcShiftList.FirstOrDefault(a => a.Shift == mv.Shift).ConcFe[half] = mv.Value;
            }


        }

        public static void SaveConcShiftLab(List<ConcShift> ConcShiftList, string changedBy, OracleConnection conn)
        {
            //need to loop through each of the values and save them separately
            foreach (var cs in ConcShiftList)
            {
                SaveMetric(ID_HYDRO, cs, cs.TailsHydro, changedBy, conn);
                SaveMetric(ID_THICKENER, cs, cs.TailsThickener, changedBy, conn);
                SaveMetric(ID_NOLA_SIO2, cs, cs.NOLA_SiO2, changedBy, conn);
                SaveMetric(ID_CONTROL_SIO2, cs, cs.Control_SiO2[0], changedBy, conn);
                SaveMetric(ID_CONTROL_SIO2, cs, cs.Control_SiO2[1], changedBy, conn, 1);
                SaveMetric(ID_CONC_FE, cs, cs.ConcFe[0], changedBy, conn);
                SaveMetric(ID_CONC_FE, cs, cs.ConcFe[1], changedBy, conn, 1);


            }
        }

        private static void SaveMetric(long MetricID, ConcShift cs, decimal? Value, string changedBy, OracleConnection conn, byte HalfAdd = 0)
        {
            //the half add parameter was added as this is set up for one record per shift.  This was fine until Nov 2024 when NOLA Control SiO2 needed to be recorded every 4 hours
            //To handle this we changed this to an array but we will need to store  per half
            Metric_Value mv = new()
            {
                Metric_Id = MetricID,
                Value = Value,
                Half = (byte)(cs.Half + HalfAdd),
                Hour = (byte)(cs.Hour + (4 * HalfAdd)),
                Inserted_Date = DateTime.Now,
                Shift_Date = cs.Shift_Date,
                Shift = cs.Shift,
                Update_Date = DateTime.Now,
                Start_Date = cs.Start_Date.AddHours((4 * HalfAdd)),
                Start_Date_No_DST = cs.Start_Date_No_DST.AddHours((4 * HalfAdd))
            };
            if (Metric_ValueSvc.UpdateWithAudit(mv, changedBy, conn) == 0)
                Metric_ValueSvc.InsertWithAudit(mv, changedBy, conn);
        }


        public static void ApproveConcShiftLabItems(Approval Appvl, DateTime StartDateNoDST, OracleConnection conn)
        {
            ApprovalSvc.ApproveMetricValue(ID_HYDRO, StartDateNoDST, Appvl,conn);
            ApprovalSvc.ApproveMetricValue(ID_THICKENER, StartDateNoDST, Appvl, conn);
            ApprovalSvc.ApproveMetricValue(ID_NOLA_SIO2, StartDateNoDST, Appvl, conn);
            ApprovalSvc.ApproveMetricValue(ID_CONTROL_SIO2, StartDateNoDST, Appvl, conn);
            ApprovalSvc.ApproveMetricValue(ID_CONTROL_SIO2, StartDateNoDST.AddHours(4), Appvl, conn);
            ApprovalSvc.ApproveMetricValue(ID_CONC_FE, StartDateNoDST, Appvl, conn);
            ApprovalSvc.ApproveMetricValue(ID_CONC_FE, StartDateNoDST.AddHours(4), Appvl, conn);


        }
    }
}
