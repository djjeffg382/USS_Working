using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.DAL.Pi.Models;

namespace MOO.DAL.Pi.Services
{
    //class for pulling in the 3 points used in the NOLA Auditing data
    public static class NOLA_AuditSvc
    {
        public static List<NOLA_Audit> GetNolaData(NOLA_Audit.NOLA_Type SelectedNola, DateTime StartDate, DateTime EndDate)
        {
            string[] NOLA_Tags = GetNOLATags(SelectedNola);
            //start with the lab tag and fill a list of NOLA_Audit models
            List<MOO.DAL.Pi.Models.PiComp2> LabDataList = MOO.DAL.Pi.Services.PiComp2Svc.Get(StartDate, EndDate, NOLA_Tags[0]);
            List<NOLA_Audit> retVal = new();
            foreach (MOO.DAL.Pi.Models.PiComp2 cpi in LabDataList)
            {
                retVal.Add(new NOLA_Audit() { Lab = cpi.Value.GetValueOrDefault(0), Time = cpi.Time, Type = SelectedNola });
            }
            AddNOLAVal(NOLA_Tags[1], StartDate, EndDate, retVal);
            AddSolidsVal(NOLA_Tags[2], StartDate, EndDate, retVal);
            return retVal;
        }

        public static void InsertNolaAudit(NOLA_Audit.NOLA_Type SelectedNola, DateTime NauditDate, double NauditValue)
        {
            string[] NOLA_Tags = GetNOLATags(SelectedNola);
            PiComp2Svc.PutPIDataUnBuffered(NOLA_Tags[0], NauditDate, NauditValue);
        }


        public static void DeleteNolaAudit(NOLA_Audit.NOLA_Type SelectedNola, DateTime NauditDate)
        {
            string[] NOLA_Tags = GetNOLATags(SelectedNola);
            PiComp2Svc.DeletePIDataUnbuffered(NOLA_Tags[0], NauditDate);
        }


        private static void AddNOLAVal(string NOLATag, DateTime StartDate, DateTime EndDate, 
                                        List<NOLA_Audit> NOLA_AuditList)
        {
            List<MOO.DAL.Pi.Models.PiComp2> NOLADataList = MOO.DAL.Pi.Services.PiComp2Svc.Get(StartDate, EndDate.AddMinutes(2), NOLATag);

            foreach (NOLA_Audit NA_Item in NOLA_AuditList)
            {
                //find the corresponding NOLA data that occured right after the date
                var a = NOLADataList.Where(a => a.Time >= NA_Item.Time).OrderBy(a => a.Time).FirstOrDefault();
                if (a !=null)
                    NA_Item.NOLA = a.Value.GetValueOrDefault(0);
            }
        }


        private static void AddSolidsVal(string SolidsTag, DateTime StartDate, DateTime EndDate,
                                       List<NOLA_Audit> NOLA_AuditList)
        {
            List<MOO.DAL.Pi.Models.PiComp2> NOLADataList = MOO.DAL.Pi.Services.PiComp2Svc.Get(StartDate, EndDate.AddMinutes(2), SolidsTag);

            foreach (NOLA_Audit NA_Item in NOLA_AuditList)
            {
                //find the corresponding NOLA data that occured right after the date
                var a = NOLADataList.Where(a => a.Time >= NA_Item.Time).OrderBy(a => a.Time).FirstOrDefault();
                if (a != null)
                    NA_Item.Solids = a.Value.GetValueOrDefault(0);
            }
        }

        /// <summary>
        /// returns a string array of the selected nola tags 1-Lab Tag, 2-NOLA Tag, 3-Solids Tag
        /// </summary>
        /// <param name="SelectedNola"></param>
        /// <returns></returns>
        private static string[] GetNOLATags(NOLA_Audit.NOLA_Type SelectedNola)
        {
            switch (SelectedNola)
            {
                case NOLA_Audit.NOLA_Type.NOLA2_Step2:
                    return new string[] { "LAB:M_NOLA2_S2_SILICA", "SI429S2A", "SL429S2A" };
                case NOLA_Audit.NOLA_Type.NOLA2_Step3:
                    return new string[] { "LAB:M_NOLA2_S3_SILICA", "SI429S3A", "SL429S3A" };
                case NOLA_Audit.NOLA_Type.NOLA3_Step2:
                    return new string[] { "LAB:M_NOLA3_S2_SILICA", "SI429S2B", "SL429S2B" };
                case NOLA_Audit.NOLA_Type.NOLA3_Step3:
                    return new string[] { "LAB:M_NOLA3_S3_SILICA", "SI429S3B", "SL429S3B" };
                case NOLA_Audit.NOLA_Type.NOLA3_FF:
                    return new string[] { "LAB:M_NOLA3_FF_SILICA", "SI429FFB", "SL429FFB" };
                default: return new string[] { "", "", "" };
            }
        }
    }
}
