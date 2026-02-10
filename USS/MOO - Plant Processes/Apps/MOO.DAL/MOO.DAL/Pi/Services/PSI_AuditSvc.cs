using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.DAL.Pi.Models;

namespace MOO.DAL.Pi.Services
{

    /// <summary>
    /// Class for retriving and inserting PSI Audit data
    /// </summary>
    /// <remarks>
    ///     Note that we are using Unbuffered Writes.  This is because this will be done from the web server.  If we buffer the writes, then deletes will not work on the current snapshot.
    /// </remarks>
    public static class PSI_AuditSvc
    {

        public static List<PSI_Audit> GetPSIData(byte LineNumber, DateTime StartDate, DateTime EndDate)
        {
            string[] PSI_Tags = GetPSITags(LineNumber);
            //start with the lab tag and fill a list of NOLA_Audit models
            List<MOO.DAL.Pi.Models.PiComp2> LabDataList = MOO.DAL.Pi.Services.PiComp2Svc.Get(StartDate, EndDate, PSI_Tags[0]);
            List<PSI_Audit> retVal = new();
            foreach (MOO.DAL.Pi.Models.PiComp2 cpi in LabDataList)
            {
                retVal.Add(new PSI_Audit() { Lab = cpi.Value.GetValueOrDefault(0), Time = cpi.Time, Line = LineNumber });
            }
            AddPSIVal(PSI_Tags[1], StartDate, EndDate, retVal);
            AddSolidsVal(PSI_Tags[2], StartDate, EndDate, retVal);
            return retVal;
        }


        public static void InsertPsiAudit(byte LineNumber, DateTime PauditDate, double PauditValue)
        {
            string[] PSI_Tags = GetPSITags(LineNumber);
            PiComp2Svc.PutPIDataUnBuffered(PSI_Tags[0], PauditDate, PauditValue);
            //After inserting we need to also update the Lab Delta point
            InsertPSILabDelta(LineNumber, PauditDate);

            
        }


        public static void DeletePsiAudit(byte LineNumber, DateTime PauditDate)
        {
            string[] PSI_Tags = GetPSITags(LineNumber);
            PiComp2Svc.DeletePIDataUnbuffered(PSI_Tags[0], PauditDate);
            //After deleting, we need to also delete the lab delta point for that date
            string tag = GetLabDeltaPoint(LineNumber);
            PiComp2Svc.DeletePIDataUnbuffered(tag, PauditDate);

        }


        /// <summary>
        /// Inserts an average of the last 10 values PSI Delta.  This is a point specifically used to push back to Ovation
        /// </summary>
        /// <param name="PSIDate"></param>
        private static void InsertPSILabDelta(byte LineNumber, DateTime PSIDate)
        {
            var psiData = GetPSIData(LineNumber, PSIDate.AddDays(-90), PSIDate);
            //get the rolling average of the last 10
            byte nCount = 0;
            double nSum = 0;
            for (int nRec = psiData.Count - 1; nRec >= 0; nRec--)
            {
                nCount++;
                nSum += psiData[nRec].Lab - psiData[nRec].Psi;
                if (nCount >= 10)
                    break;
            }
            double labDelta = Math.Round( nSum / nCount, 3, MidpointRounding.AwayFromZero);

            string tag = GetLabDeltaPoint(LineNumber);
            int a = 0;
            PiComp2Svc.PutPIDataUnBuffered(tag,PSIDate, labDelta);

        }


        /// <summary>
        /// Gets the lab delta point for the given line number 
        /// </summary>
        /// <param name="LineNumber"></param>
        /// <returns></returns>
        private static string GetLabDeltaPoint(byte LineNumber)
        {
            //The point number we need to update starts with A4241107 for line 3 and increments by 1 for each line.
            //So line 4 ia A4241108, Line 5 is A4241109, Line 6 is A4241110 etc.
            int pointNum = 107 + LineNumber - 3;
            string tag = $"A4241{pointNum}";
            return tag;
        }



        private static void AddPSIVal(string NOLATag, DateTime StartDate, DateTime EndDate,
                                        List<PSI_Audit> PSI_AuditList)
        {
            List<MOO.DAL.Pi.Models.PiComp2> NOLADataList = MOO.DAL.Pi.Services.PiComp2Svc.Get(StartDate, EndDate.AddMinutes(60), NOLATag);

            foreach (PSI_Audit PSI_Item in PSI_AuditList)
            {
                //find the corresponding NOLA data that occured right after the date
                var a = NOLADataList.Where(a => a.Time >= PSI_Item.Time).OrderBy(a => a.Time).FirstOrDefault();
                if (a != null)
                {
                    PSI_Item.Psi = Math.Round(a.Value.GetValueOrDefault(0), 2, MidpointRounding.AwayFromZero);
                    PSI_Item.PsiDate = a.Time;
                }
                    
            }
        }


        private static void AddSolidsVal(string SolidsTag, DateTime StartDate, DateTime EndDate,
                                       List<PSI_Audit> PSI_AuditList)
        {
            List<MOO.DAL.Pi.Models.PiComp2> NOLADataList = MOO.DAL.Pi.Services.PiComp2Svc.Get(StartDate, EndDate.AddMinutes(60), SolidsTag);

            foreach (PSI_Audit PSI_Item in PSI_AuditList)
            {
                //find the corresponding NOLA data that occured right after the date
                var a = NOLADataList.Where(a => a.Time <= PSI_Item.PsiDate.AddSeconds(60)).OrderByDescending(a => a.Time).FirstOrDefault(); //add 60 seconds in case the solids tag doesn't get scanned at the same time
                if (a != null)
                    PSI_Item.Solids = a.Value.GetValueOrDefault(0);
            }
        }

        /// <summary>
        /// returns a string array of the selected nola tags 1-Lab Tag, 2-PSITag, 3-Solids Tag
        /// </summary>
        /// <param name="SelectedNola"></param>
        /// <returns></returns>
        private static string[] GetPSITags(byte LineNumber)
        {
            string LabTag = $"LAB:M_L{LineNumber}_PSI";

            switch (LineNumber)
            {
                case 3:
                    return [LabTag, "AL423C01", "AL423C00"];
                case 4:
                    return [LabTag, "AL423D01", "AL423D00"];
                case 5:
                    return [LabTag, "AL423E01", "AL423E00"];
                case 6:
                    return [LabTag, "AL423F01", "AL423F00"];
                case 7:
                    return [LabTag, "AI409032", "AL425A37"];
                case 8:
                    return [LabTag, "AI409036", "AL425B37"];
                case 9:
                    return [LabTag, "AI413040", "AL425C37"];
                case 10:
                    return [LabTag, "AI413044", "AL425D37"];
                case 11:
                    return [LabTag, "AI414064", "AL425E37"];
                case 12:
                    return [LabTag, "AI414068", "AL425F37"];
                case 13:
                    return [LabTag, "AI416040", "AI416042"];
                case 14:
                    return [LabTag, "AI416033", "AI416035"];
                case 15:
                    return [LabTag, "AI418040", "AI418042"];
                case 16:
                    return [LabTag, "AI418033", "AI418035"];
                case 17:
                    return [LabTag, "AI420040", "AI420042"];
                case 18:
                    return [LabTag, "AI420033", "AI420035"];
                default: return ["", "", ""];
            }
        }

    }
}
