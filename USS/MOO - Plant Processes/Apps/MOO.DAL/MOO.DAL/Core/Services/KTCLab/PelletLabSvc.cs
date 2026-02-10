using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.DAL.Core.Models;
using MOO.DAL.Core.Services;
using MOO.DAL.Core.Models.KTCLab;
using Oracle.ManagedDataAccess.Client;

namespace MOO.DAL.Core.Services.KTCLab
{
    internal static class PelletLabSvc
    {

        #region "Metric ID Constants"
        //Metric ID's

        //Before tumbles
        public const int ID_BT_START_WEIGHT = 321;
        public const int ID_BT_12INCH = 322;
        public const int ID_BT_716INCH = 968;
        public const int ID_BT_38INCH = 969;
        public const int ID_BT_14INCH = 323;

        //After Tumbles
        public const int ID_AT_START_WEIGHT = 324;
        public const int ID_AT_12INCH = 970;
        public const int ID_AT_716INCH = 971;
        public const int ID_AT_38INCH = 972;
        public const int ID_AT_14INCH = 325;
        public const int ID_AT_30MESH = 326;

        //pellet Compression
        public const int ID_COMPRESS_LBS = 341;
        public const int ID_COMPRESS_300 = 342;
        public const int ID_COMPRESS_200 = 343;
        public const int ID_PCT_MAG = 344;

        //pellet XRF analysis
        public const int ID_PELL_FE = 361;
        public const int ID_PELL_SIO2 = 362;
        public const int ID_PELL_CAO = 363;
        public const int ID_PELL_MOIST = 364;
        public const int ID_PELL_AL2O3 = 1134;
        public const int ID_PELL_MGO = 1135;
        public const int ID_PELL_MN = 1136;

        public const int ID_PELL_FERROUS = 1119;
        #endregion



        /// <summary>
        /// Fills the Pellet Lab data for the specified shift
        /// </summary>
        /// <param name="ShiftDate"></param>
        /// <param name="PellLabList"></param>
        public static void FillPelletLab(DateTime ShiftDate, List<PelletLab> PellLabList)
        {

            PellLabList.Clear();
            //need to create a record for 23,3,7,11,15,19 hours
            //start with 23:00 and go from there
            DateTime startDate = MOO.Shifts.Shift8.ShiftStartEndTime(ShiftDate, 1, Plant.Keetac)[0];
            for (int i = 0; i < 6; i++)
            {                
                PelletLab pl = new()
                {
                    Shift_Date = ShiftDate,
                    Start_Date = startDate,
                    Shift = MOO.Shifts.Shift8.ShiftNumber(startDate, Plant.Keetac),
                    Half = MOO.Shifts.Shift8.HalfShift(startDate, Plant.Keetac),
                    Hour = MOO.Shifts.Shift8.ShiftHour(startDate, Plant.Keetac)
                };

                pl.Start_Date_No_DST = pl.Start_Date.IsDaylightSavingTime() ? pl.Start_Date.AddHours(-1) : pl.Start_Date;
                //check if defaults were used for this half shift
                var defUsed = Default_UsedSvc.Get(1,pl.Start_Date_No_DST);
                if (defUsed != null)
                    pl.DefaultsUsed = true;
                PellLabList.Add(pl);
                startDate = startDate.AddHours(4);
            }
            //fill the Before tumbles start weight field
            List<Metric_Value> mvVals = Metric_ValueSvc.GetByShiftDate(ID_BT_START_WEIGHT,ShiftDate,ShiftDate);
            foreach(var mv in mvVals)
            {
                PellLabList.FirstOrDefault(a => a.Start_Date_No_DST == mv.Start_Date_No_DST).Bt_StartingWeight = mv.Value;
                PellLabList.FirstOrDefault(a => a.Start_Date_No_DST == mv.Start_Date_No_DST).Approval = mv.Approval;
            }


            //fill the Before tumbles 1/2 inch field
            mvVals = Metric_ValueSvc.GetByShiftDate(ID_BT_12INCH, ShiftDate, ShiftDate);
            foreach (var mv in mvVals)
            {
                PellLabList.FirstOrDefault(a => a.Start_Date_No_DST == mv.Start_Date_No_DST).Bt_12 = mv.Value;
            }


            //fill the Before tumbles 7/16 inch field
            mvVals = Metric_ValueSvc.GetByShiftDate(ID_BT_716INCH, ShiftDate, ShiftDate);
            foreach (var mv in mvVals)
            {
                PellLabList.FirstOrDefault(a => a.Start_Date_No_DST == mv.Start_Date_No_DST).Bt_716 = mv.Value;
            }


            //fill the Before tumbles 3/8 inch field
            mvVals = Metric_ValueSvc.GetByShiftDate(ID_BT_38INCH, ShiftDate, ShiftDate);
            foreach (var mv in mvVals)
            {
                PellLabList.FirstOrDefault(a => a.Start_Date_No_DST == mv.Start_Date_No_DST).Bt_38 = mv.Value;
            }


            //fill the Before tumbles 1/4 inch field
            mvVals = Metric_ValueSvc.GetByShiftDate(ID_BT_14INCH, ShiftDate, ShiftDate);
            foreach (var mv in mvVals)
            {
                PellLabList.FirstOrDefault(a => a.Start_Date_No_DST == mv.Start_Date_No_DST).Bt_14 = mv.Value;
            }


            //fill the After tumbles Starting Weight field
            mvVals = Metric_ValueSvc.GetByShiftDate(ID_AT_START_WEIGHT, ShiftDate, ShiftDate);
            foreach (var mv in mvVals)
            {
                PellLabList.FirstOrDefault(a => a.Start_Date_No_DST == mv.Start_Date_No_DST).At_StartingWeight = mv.Value;
            }

            //fill the After tumbles 1/2 Inch field
            mvVals = Metric_ValueSvc.GetByShiftDate(ID_AT_12INCH, ShiftDate, ShiftDate);
            foreach (var mv in mvVals)
            {
                PellLabList.FirstOrDefault(a => a.Start_Date_No_DST == mv.Start_Date_No_DST).At_12 = mv.Value;
            }
            //fill the After tumbles 7/16 Inch field
            mvVals = Metric_ValueSvc.GetByShiftDate(ID_AT_716INCH, ShiftDate, ShiftDate);
            foreach (var mv in mvVals)
            {
                PellLabList.FirstOrDefault(a => a.Start_Date_No_DST == mv.Start_Date_No_DST).At_716 = mv.Value;
            }
            //fill the After tumbles 3/8 Inch field
            mvVals = Metric_ValueSvc.GetByShiftDate(ID_AT_38INCH, ShiftDate, ShiftDate);
            foreach (var mv in mvVals)
            {
                PellLabList.FirstOrDefault(a => a.Start_Date_No_DST == mv.Start_Date_No_DST).At_38 = mv.Value;
            }
            //fill the After tumbles 1/4 Inch field
            mvVals = Metric_ValueSvc.GetByShiftDate(ID_AT_14INCH, ShiftDate, ShiftDate);
            foreach (var mv in mvVals)
            {
                PellLabList.FirstOrDefault(a => a.Start_Date_No_DST == mv.Start_Date_No_DST).At_14 = mv.Value;
            }
            //fill the After tumbles 30 mesh field
            mvVals = Metric_ValueSvc.GetByShiftDate(ID_AT_30MESH, ShiftDate, ShiftDate);
            foreach (var mv in mvVals)
            {
                PellLabList.FirstOrDefault(a => a.Start_Date_No_DST == mv.Start_Date_No_DST).At_30Mesh = mv.Value;
            }


            //fill the Compression lbs field
            mvVals = Metric_ValueSvc.GetByShiftDate(ID_COMPRESS_LBS, ShiftDate, ShiftDate);
            foreach (var mv in mvVals)
            {
                PellLabList.FirstOrDefault(a => a.Start_Date_No_DST == mv.Start_Date_No_DST).CompressionLbs = mv.Value;
            }
            //fill the Compression -300 field
            mvVals = Metric_ValueSvc.GetByShiftDate(ID_COMPRESS_300, ShiftDate, ShiftDate);
            foreach (var mv in mvVals)
            {
                PellLabList.FirstOrDefault(a => a.Start_Date_No_DST == mv.Start_Date_No_DST).CompLess300 = mv.Value;
            }
            //fill the Compression -200 field
            mvVals = Metric_ValueSvc.GetByShiftDate(ID_COMPRESS_200, ShiftDate, ShiftDate);
            foreach (var mv in mvVals)
            {
                PellLabList.FirstOrDefault(a => a.Start_Date_No_DST == mv.Start_Date_No_DST).CompLess200 = mv.Value;
            }


            //fill the Pell MagFe field
            mvVals = Metric_ValueSvc.GetByShiftDate(ID_PCT_MAG, ShiftDate, ShiftDate);
            foreach (var mv in mvVals)
            {
                PellLabList.FirstOrDefault(a => a.Start_Date_No_DST == mv.Start_Date_No_DST).PctMagPell = mv.Value;
            }



            //fill the XRF Pell Fe field
            mvVals = Metric_ValueSvc.GetByShiftDate(ID_PELL_FE, ShiftDate, ShiftDate);
            foreach (var mv in mvVals)
            {
                PellLabList.FirstOrDefault(a => a.Start_Date_No_DST == mv.Start_Date_No_DST).PellFe = mv.Value;
            }
            //fill the XRF Pell sio2 field
            mvVals = Metric_ValueSvc.GetByShiftDate(ID_PELL_SIO2, ShiftDate, ShiftDate);
            foreach (var mv in mvVals)
            {
                PellLabList.FirstOrDefault(a => a.Start_Date_No_DST == mv.Start_Date_No_DST).PellSiO2 = mv.Value;
            }
            //fill the XRF Pell cao field
            mvVals = Metric_ValueSvc.GetByShiftDate(ID_PELL_CAO, ShiftDate, ShiftDate);
            foreach (var mv in mvVals)
            {
                PellLabList.FirstOrDefault(a => a.Start_Date_No_DST == mv.Start_Date_No_DST).PellCao = mv.Value;
            }


            //fill the XRF Pell moisture field
            mvVals = Metric_ValueSvc.GetByShiftDate(ID_PELL_MOIST, ShiftDate, ShiftDate);
            foreach (var mv in mvVals)
            {
                PellLabList.FirstOrDefault(a => a.Start_Date_No_DST == mv.Start_Date_No_DST).PellMoisture = mv.Value;
            }

            //fill the XRF Pell AL2O3 field
            mvVals = Metric_ValueSvc.GetByShiftDate(ID_PELL_AL2O3, ShiftDate, ShiftDate);
            foreach (var mv in mvVals)
            {
                PellLabList.FirstOrDefault(a => a.Start_Date_No_DST == mv.Start_Date_No_DST).PellAl2O3 = mv.Value;
            }
            //fill the XRF Pell MgO field
            mvVals = Metric_ValueSvc.GetByShiftDate(ID_PELL_MGO, ShiftDate, ShiftDate);
            foreach (var mv in mvVals)
            {
                PellLabList.FirstOrDefault(a => a.Start_Date_No_DST == mv.Start_Date_No_DST).PellMgO = mv.Value;
            }
            //fill the XRF Pell MN field
            mvVals = Metric_ValueSvc.GetByShiftDate(ID_PELL_MN, ShiftDate, ShiftDate);
            foreach (var mv in mvVals)
            {
                PellLabList.FirstOrDefault(a => a.Start_Date_No_DST == mv.Start_Date_No_DST).PellMn = mv.Value;
            }

            //fill the Pell Ferrous field
            mvVals = Metric_ValueSvc.GetByShiftDate(ID_PELL_FERROUS, ShiftDate, ShiftDate);
            foreach (var mv in mvVals)
            {
                PellLabList.FirstOrDefault(a => a.Start_Date_No_DST == mv.Start_Date_No_DST).PellFerrous = mv.Value;
            }

        }

        public static void SavePelletLab(List<PelletLab> pelletLabList, string changedBy, OracleConnection conn)
        {
            //need to loop through each of the values and save them separately
            foreach(var pl in pelletLabList)
            {
                //before tumbles
                SaveMetric(ID_BT_START_WEIGHT, pl, pl.Bt_StartingWeight,changedBy, conn);
                SaveMetric(ID_BT_12INCH, pl, pl.Bt_12, changedBy, conn);
                SaveMetric(ID_BT_716INCH, pl, pl.Bt_716, changedBy, conn);
                SaveMetric(ID_BT_38INCH, pl, pl.Bt_38, changedBy, conn);
                SaveMetric(ID_BT_14INCH, pl, pl.Bt_14, changedBy, conn);
                //after tumbles
                SaveMetric(ID_AT_START_WEIGHT, pl, pl.At_StartingWeight, changedBy, conn);
                SaveMetric(ID_AT_12INCH, pl, pl.At_12, changedBy, conn);
                SaveMetric(ID_AT_716INCH, pl, pl.At_716, changedBy, conn);
                SaveMetric(ID_AT_38INCH, pl, pl.At_38, changedBy, conn);
                SaveMetric(ID_AT_14INCH, pl, pl.At_14, changedBy, conn);
                SaveMetric(ID_AT_30MESH, pl, pl.At_30Mesh, changedBy, conn);
                //compression
                SaveMetric(ID_COMPRESS_LBS, pl, pl.CompressionLbs, changedBy, conn);
                SaveMetric(ID_COMPRESS_200, pl, pl.CompLess200, changedBy, conn);
                SaveMetric(ID_COMPRESS_300, pl, pl.CompLess300, changedBy, conn);
                SaveMetric(ID_PCT_MAG, pl, pl.PctMagPell, changedBy, conn);
                //xrf
                SaveMetric(ID_PELL_FE, pl, pl.PellFe, changedBy, conn);
                SaveMetric(ID_PELL_SIO2, pl, pl.PellSiO2, changedBy, conn);
                SaveMetric(ID_PELL_CAO, pl, pl.PellCao, changedBy, conn);
                SaveMetric(ID_PELL_MOIST, pl, pl.PellMoisture, changedBy, conn);
                SaveMetric(ID_PELL_AL2O3, pl, pl.PellAl2O3, changedBy, conn);
                SaveMetric(ID_PELL_MGO, pl, pl.PellMgO, changedBy, conn);
                SaveMetric(ID_PELL_MN, pl, pl.PellMn, changedBy, conn);
                SaveMetric(ID_PELL_FERROUS, pl, pl.PellFerrous, changedBy, conn);

                //check if defaults were used for this half shift
                Default_Used defUsed = Default_UsedSvc.Get(1, pl.Start_Date_No_DST);
                if (defUsed == null && pl.DefaultsUsed)
                {
                    //need to create a defaults used record
                    defUsed = new()
                    {
                        Default_Group_Id = 1,
                        Shift_Date = pl.Shift_Date,
                        Half = pl.Half,
                        Hour = pl.Hour,
                        Start_Date_No_Dst = pl.Start_Date_No_DST,
                        Shift = pl.Shift
                    };
                    Default_UsedSvc.Insert(defUsed);
                }
                else if (defUsed != null && !pl.DefaultsUsed)
                    Default_UsedSvc.Delete(defUsed);


            }
        }

        private static void SaveMetric(long MetricID, PelletLab pl, decimal? Value, string changedBy, OracleConnection conn)
        {
            Metric_Value mv = new()
            {
                Metric_Id = MetricID,
                Value = Value,
                Half = pl.Half,
                Hour = pl.Hour,
                Inserted_Date = DateTime.Now,
                Shift_Date = pl.Shift_Date,
                Shift = pl.Shift,
                Update_Date = DateTime.Now,
                Start_Date = pl.Start_Date,
                Start_Date_No_DST = pl.Start_Date_No_DST
            };
            if (Metric_ValueSvc.UpdateWithAudit(mv, changedBy, conn) == 0)
                Metric_ValueSvc.InsertWithAudit(mv,changedBy, conn);
        }


        public static void ApprovePelletLabItems(Approval Appvl, DateTime StartDateNoDST, OracleConnection conn)
        {
            //Before tumbles
            ApprovalSvc.ApproveMetricValue(ID_BT_START_WEIGHT, StartDateNoDST, Appvl, conn);
            ApprovalSvc.ApproveMetricValue(ID_BT_12INCH, StartDateNoDST, Appvl, conn);
            ApprovalSvc.ApproveMetricValue(ID_BT_716INCH, StartDateNoDST, Appvl, conn);
            ApprovalSvc.ApproveMetricValue(ID_BT_38INCH, StartDateNoDST, Appvl, conn);
            ApprovalSvc.ApproveMetricValue(ID_BT_14INCH, StartDateNoDST, Appvl, conn);


            //After Tumbles
            ApprovalSvc.ApproveMetricValue(ID_AT_START_WEIGHT, StartDateNoDST, Appvl, conn);
            ApprovalSvc.ApproveMetricValue(ID_AT_12INCH, StartDateNoDST, Appvl, conn);
            ApprovalSvc.ApproveMetricValue(ID_AT_716INCH, StartDateNoDST, Appvl, conn);
            ApprovalSvc.ApproveMetricValue(ID_AT_38INCH, StartDateNoDST, Appvl, conn);
            ApprovalSvc.ApproveMetricValue(ID_AT_14INCH, StartDateNoDST, Appvl, conn);
            ApprovalSvc.ApproveMetricValue(ID_AT_30MESH, StartDateNoDST, Appvl, conn);

        //pellet Compression
            ApprovalSvc.ApproveMetricValue(ID_COMPRESS_LBS, StartDateNoDST, Appvl, conn);
            ApprovalSvc.ApproveMetricValue(ID_COMPRESS_300, StartDateNoDST, Appvl, conn);
            ApprovalSvc.ApproveMetricValue(ID_COMPRESS_200, StartDateNoDST, Appvl, conn);
            ApprovalSvc.ApproveMetricValue(ID_PCT_MAG, StartDateNoDST, Appvl, conn);

        //pellet XRF analysis
            ApprovalSvc.ApproveMetricValue(ID_PELL_FE, StartDateNoDST, Appvl, conn);
            ApprovalSvc.ApproveMetricValue(ID_PELL_SIO2, StartDateNoDST, Appvl, conn);
            ApprovalSvc.ApproveMetricValue(ID_PELL_CAO, StartDateNoDST, Appvl, conn);
            ApprovalSvc.ApproveMetricValue(ID_PELL_MOIST, StartDateNoDST, Appvl, conn);
            ApprovalSvc.ApproveMetricValue(ID_PELL_AL2O3, StartDateNoDST, Appvl, conn);
            ApprovalSvc.ApproveMetricValue(ID_PELL_MGO, StartDateNoDST, Appvl, conn);
            ApprovalSvc.ApproveMetricValue(ID_PELL_MN, StartDateNoDST, Appvl, conn);
            ApprovalSvc.ApproveMetricValue(ID_PELL_FERROUS, StartDateNoDST, Appvl, conn);


        }

        /// <summary>
        /// sets the values to defaults for the specified pellet lab data
        /// </summary>
        /// <param name="PelletLabVals"></param>
        public static void SetDefaults(PelletLab PelletLabVals)
        {
            PelletLabVals.DefaultsUsed = true;
            PelletLabVals.Bt_StartingWeight = 11300;
            PelletLabVals.Bt_12 = 820;
            PelletLabVals.Bt_716 = 3690;
            PelletLabVals.Bt_38 = 9850;
            PelletLabVals.Bt_14 = 11218;

            PelletLabVals.At_StartingWeight = 11218;
            PelletLabVals.At_12 = 630;
            PelletLabVals.At_716 = 3180;
            PelletLabVals.At_38 = 9310;
            PelletLabVals.At_14 = 10825;
            PelletLabVals.At_30Mesh = 10953;

            PelletLabVals.CompressionLbs = 582;
            PelletLabVals.CompLess200 = 2;
            PelletLabVals.CompLess300 = 7;
            PelletLabVals.PctMagPell = 5;
            PelletLabVals.PellFe = 65.87M;
            PelletLabVals.PellSiO2 = 4.53M;
            PelletLabVals.PellCao = .68M;
            PelletLabVals.PellMoisture = 1.4M;
            PelletLabVals.PellAl2O3 = 0.18M;
            PelletLabVals.PellMgO = 0.4M;
            PelletLabVals.PellMn = 0.13M;
            PelletLabVals.PellFerrous = 0M;


        }
    }
}
