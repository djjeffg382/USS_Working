using MOO.DAL.ToLive.Models;
using OM_Lab.Data.Models;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using DAL = MOO.DAL.ToLive;

namespace OM_Lab.Data.Services
{
    public static class LabChemPhysSvc
    {
        public const int MESH_START_WGT = 10000;        
        /// <summary>
        /// Gets a joined Lab_Chem_Analysis with Lab_Phys_Analysis list for specified shift date and shift and creates records if not found
        /// </summary>
        /// <param name="ShiftDate">Shift Date</param>
        /// <param name="LabChemTypeId">Lab Chem Type ID</param>
        /// <param name="LabPhysTypeId">Lab Phys Type Id</param>
        /// <param name="FirstSampleHrsAfterStart">Hours offset from Midnight for the first sample</param>
        /// <param name="SampleIntervalHrs">Hours interval between each sample</param>
        /// <returns></returns>
        public static List<LabChemPhys> GetByShiftWithCreate(DateTime ShiftDate, int ShiftNumber, int LabChemTypeId, int LabPhysTypeId, int FirstSampleHrsAfterStart, int SampleIntervalHrs)
        {
            List<LabChemPhys> retVal = [];
            var lcType = DAL.Services.Lab_Chem_TypeSvc.Get(LabChemTypeId);
            var lpType = DAL.Services.Lab_Phys_TypeSvc.Get(LabPhysTypeId);
            DateTime[] startShiftEnd = MOO.Shifts.Shift.ShiftStartEndTime(MOO.Plant.Keetac, MOO.Area.Concentrator, ShiftDate, (byte)ShiftNumber);
            var phys = DAL.Services.Lab_Phys_AnalysisSvc.GetByAnalysisDate(LabPhysTypeId, startShiftEnd[0], startShiftEnd[1]);
            if (phys.Count == 0)
            {

                DateTime loopDate = startShiftEnd[0].AddHours(FirstSampleHrsAfterStart);
                while (loopDate < startShiftEnd[1])
                {
                    LabChemPhys lcpRec = new() { SampleDate = loopDate };
                    Lab_Phys_Analysis lpRec = new()
                    {
                        Analysis_Date = loopDate,
                        Lab_Phys_Type = lpType,
                        Shift_Date12 = ShiftDate,
                        Shift_Nbr12 = MOO.Shifts.Shift.ShiftNumber(MOO.Plant.Keetac, MOO.Area.Concentrator, loopDate),
                        Shift_Date8 = ShiftDate,
                        Shift_Nbr8 = MOO.Shifts.Shift8.ShiftNumber(loopDate.AddHours(-2), MOO.Plant.Keetac),
                        Start_Wgt = MESH_START_WGT
                    };

                    lcpRec.Phys_Analysis = lpRec;
                    retVal.Add(lcpRec);
                    loopDate = loopDate.AddHours(SampleIntervalHrs);
                }
            }
            else
            {
                foreach (var physRecord in phys)
                {
                    LabChemPhys lcpRec = new()
                    {
                        SampleDate = physRecord.Analysis_Date,
                        Phys_Analysis = physRecord
                    };
                    retVal.Add(lcpRec);
                }
            }

            var chem = DAL.Services.Lab_Chem_AnalysisSvc.GetByAnalysisDate(LabChemTypeId, startShiftEnd[0], startShiftEnd[1]);

            if (chem.Count == 0)
            {
                DateTime loopDate = startShiftEnd[0].AddHours(FirstSampleHrsAfterStart);
                while (loopDate < startShiftEnd[1])
                {
                    LabChemPhys lcpRec = retVal.FirstOrDefault(x => x.SampleDate == loopDate)!;
                    Lab_Chem_Analysis lcRec = new()
                    {
                        Analysis_Date = loopDate,
                        Lab_Chem_Type = lcType,
                        Shift_Date12 = ShiftDate,
                        Shift_Nbr12 = MOO.Shifts.Shift.ShiftNumber(MOO.Plant.Keetac, MOO.Area.Concentrator, loopDate),
                        Shift_Date8 = ShiftDate,
                        Shift_Nbr8 = MOO.Shifts.Shift8.ShiftNumber(loopDate.AddHours(-2), MOO.Plant.Keetac)
                    };

                    if (lcpRec != null)
                        lcpRec.Chem_Analysis = lcRec;
                    loopDate = loopDate.AddHours(SampleIntervalHrs);
                }
            }
            else
            {
                foreach (var chemRecord in chem)
                {
                    LabChemPhys lcpRec = retVal.FirstOrDefault(x => x.SampleDate == chemRecord.Analysis_Date)!;
                    if (lcpRec != null)
                        lcpRec.Chem_Analysis = chemRecord;
                }
            }

            return retVal;
        }


        /// <summary>
        /// Gets a joined Lab_Chem_Analysis with Lab_Phys_Analysis list for specified shift date and shift and creates records if not found
        /// </summary>
        /// <param name="ShiftDate">Shift Date</param>
        /// <param name="LabChemTypeId">Lab Chem Type ID</param>
        /// <param name="LabPhysTypeId">Lab Phys Type Id</param>
        /// <returns></returns>
        public static List<LabChemPhys> GetByShift(DateTime ShiftDate, int ShiftNumber, int LabChemTypeId, int LabPhysTypeId)
        {
            List<LabChemPhys> retVal = [];
            DateTime[] startShiftEnd = MOO.Shifts.Shift.ShiftStartEndTime(MOO.Plant.Keetac, MOO.Area.Concentrator, ShiftDate, (byte)ShiftNumber);
            var physList = DAL.Services.Lab_Phys_AnalysisSvc.GetByAnalysisDate(LabPhysTypeId, startShiftEnd[0], startShiftEnd[1]);
            var chemList = DAL.Services.Lab_Chem_AnalysisSvc.GetByAnalysisDate(LabChemTypeId, startShiftEnd[0], startShiftEnd[1]);

            //now we need to match up each phys with a chem record
            foreach (var phys in physList)
            {
                var chem = chemList.FirstOrDefault(x => x.Analysis_Date == phys.Analysis_Date && x.Line_Nbr == phys.Line_Nbr);
                if (chem == null)
                    throw new Exception($"Missing Lab_Chem_Analysis record for the Phys date of {phys.Analysis_Date}");

                LabChemPhys newRec = new LabChemPhys()
                {
                    Chem_Analysis = chem,
                    Phys_Analysis = phys
                };
                retVal.Add(newRec);
            }
            return retVal;

        }



        public static void SaveChemPhys(List<LabChemPhys> lcpList, OracleConnection conn, string updatedBy)
        {
            //loop through each chem and phys record and save
            foreach (var rec in lcpList)
            {
                rec.Chem_Analysis.Last_Update_By = updatedBy;
                rec.Chem_Analysis.Update_Date = DateTime.Now;

                rec.Phys_Analysis.Last_Update_By = updatedBy;
                rec.Phys_Analysis.Update_Date = DateTime.Now;

                if (rec.Chem_Analysis.Lab_Chem_Analysis_Id == 0)
                    DAL.Services.Lab_Chem_AnalysisSvc.Insert(rec.Chem_Analysis, conn);
                else
                    DAL.Services.Lab_Chem_AnalysisSvc.Update(rec.Chem_Analysis, conn);

                if (rec.Phys_Analysis.Lab_Phys_Analysis_Id == 0)
                    DAL.Services.Lab_Phys_AnalysisSvc.Insert(rec.Phys_Analysis, conn);
                else
                    DAL.Services.Lab_Phys_AnalysisSvc.Update(rec.Phys_Analysis, conn);
            }
        }
    }



}
