using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.DAL.Core.Models.KTCLab;
using MOO.DAL.Core.Models;
using Oracle.ManagedDataAccess.Client;

namespace MOO.DAL.Core.Services.KTCLab
{
    public static class KTC_LabSvc
    {

        static KTC_LabSvc()
        {
            Util.RegisterOracle();
        }


        public const int ID_CONC_SCN_START_WEIGHT = 401;
        public const int ID_CONC_SCN_100_MESH = 241;
        public const int ID_CONC_SCN_200_MESH = 261;
        public const int ID_CONC_SCN_325_MESH = 262;



        public static KTC_Lab Get(DateTime ShiftDate)
        {
            KTC_Lab retVal = new()
            {
                Shift_Date = ShiftDate
            };
            PelletLabSvc.FillPelletLab(ShiftDate,retVal.PelletLab);
            ConcShiftSvc.FillConcShift(ShiftDate,retVal.ConcShift);


            Metric_Value mv = Metric_ValueSvc.Get(ID_CONC_SCN_START_WEIGHT, retVal.Start_Date_No_DST);
            retVal.ConcScreenStartWeight = mv?.Value;


            mv = Metric_ValueSvc.Get(ID_CONC_SCN_100_MESH, retVal.Start_Date_No_DST);
            if(mv != null)
            {
                retVal.Approval = mv.Approval;
                retVal.ConcScreen100Mesh = mv.Value;
            }

            mv = Metric_ValueSvc.Get(ID_CONC_SCN_200_MESH, retVal.Start_Date_No_DST);
            retVal.ConcScreen200Mesh = mv?.Value;

            mv = Metric_ValueSvc.Get(ID_CONC_SCN_325_MESH, retVal.Start_Date_No_DST);
            retVal.ConcScreen325Mesh = mv?.Value;

            return retVal; 
        }

        public static void Save(KTC_Lab labData, string changedBy)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            OracleTransaction trans = conn.BeginTransaction();
            try
            {

                SaveMetric(ID_CONC_SCN_START_WEIGHT, labData, labData.ConcScreenStartWeight, changedBy, conn);
                SaveMetric(ID_CONC_SCN_100_MESH, labData, labData.ConcScreen100Mesh, changedBy, conn);
                SaveMetric(ID_CONC_SCN_200_MESH, labData, labData.ConcScreen200Mesh, changedBy, conn);
                SaveMetric(ID_CONC_SCN_325_MESH, labData, labData.ConcScreen325Mesh, changedBy, conn);

                PelletLabSvc.SavePelletLab(labData.PelletLab, changedBy, conn);
                ConcShiftSvc.SaveConcShiftLab(labData.ConcShift,changedBy, conn);



                trans.Commit();
            }
            catch (Exception)
            {
                trans?.Rollback();
                throw;
            }
            finally
            {
                conn.Close();
            }
           
        }



        private static void SaveMetric(long MetricID, KTC_Lab LabData, decimal? Value, string changedBy, OracleConnection conn)
        {
            Metric_Value mv = new()
            {
                Metric_Id = MetricID,
                Value = Value,
                Half = 1,
                Hour =1,
                Inserted_Date = DateTime.Now,
                Shift_Date = LabData.Shift_Date,
                Shift =1,
                Update_Date = DateTime.Now,
                Start_Date = LabData.Start_Date,
                Start_Date_No_DST = LabData.Start_Date_No_DST
            };
            if (Metric_ValueSvc.UpdateWithAudit(mv, changedBy, conn) == 0)
                Metric_ValueSvc.InsertWithAudit(mv,changedBy, conn);
        }



        public static void Approve(KTC_Lab LabData, string ApprovalUser, DateTime ShiftDate, byte ShiftNumber, byte ShiftHalf)
        {
            //create an approval record and approve all metrics
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            OracleTransaction trans = conn.BeginTransaction();
            try
            {
                Approval appvl = new()
                {
                    Approved_By = ApprovalUser,
                    Approved_Date = DateTime.Now
                };
                ApprovalSvc.Insert(appvl, conn);

                //appvl object should now have a approval id.

                if (ShiftHalf == 6)
                {
                    //only approve conc screen on last approval of shift date                    
                    ApprovalSvc.ApproveMetricValue(ID_CONC_SCN_START_WEIGHT, LabData.Start_Date_No_DST, appvl, conn);
                    ApprovalSvc.ApproveMetricValue(ID_CONC_SCN_100_MESH, LabData.Start_Date_No_DST, appvl, conn);
                    ApprovalSvc.ApproveMetricValue(ID_CONC_SCN_200_MESH, LabData.Start_Date_No_DST, appvl, conn);
                    ApprovalSvc.ApproveMetricValue(ID_CONC_SCN_325_MESH, LabData.Start_Date_No_DST, appvl, conn);
                }
                
                //only approve conc shift on last half of each shift
                if(ShiftHalf %2 == 0)
                {
                    //get the concshift we are approving
                    var cs = LabData.ConcShift.FirstOrDefault(a => a.Shift == ShiftNumber);
                    ConcShiftSvc.ApproveConcShiftLabItems(appvl, cs.Start_Date_No_DST, conn);
                }

                var ps = LabData.PelletLab.FirstOrDefault(a => a.Half == ShiftHalf);
                PelletLabSvc.ApprovePelletLabItems(appvl, ps.Start_Date_No_DST, conn);
                

                trans.Commit();
            }
            catch (Exception)
            {
                trans?.Rollback();
                throw;
            }
            finally
            {
                conn.Close();
            }
        }





        public static void SetDefaults(PelletLab PelletLabVals)
        {
            PelletLabSvc.SetDefaults(PelletLabVals);
        }


    }
}
