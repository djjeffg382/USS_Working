using Microsoft.Data.SqlClient;
using MOO.DAL.LIMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.LIMS.Services
{
    public static class SampleSvc
    {
        static SampleSvc()
        {
            Util.RegisterSqlServer();
        }


        public static Sample Get(int SampleId)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE s.id_numeric = @SampleId");


            Sample retVal = null;
            using SqlConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.LIMS_Read));
            conn.Open();
            SqlCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.AddWithValue("SampleId", SampleId);
            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }



        internal static string GetColumns(string TableAlias = "s", string ColPrefix = "s_")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}id_numeric {ColPrefix}id_numeric, {ta}Id_Text {ColPrefix}Id_Text, {ta}Compared {ColPrefix}Compared,");
            cols.AppendLine($"{ta}On_Spec {ColPrefix}On_Spec, {ta}Login_Date {ColPrefix}Login_Date, {ta}Login_By {ColPrefix}Login_By,");
            cols.AppendLine($"{ta}Sampled_Date {ColPrefix}Sampled_Date, {ta}Recd_Date {ColPrefix}Recd_Date, {ta}Date_Started {ColPrefix}DateStarted,");
            cols.AppendLine($"{ta}Date_Completed {ColPrefix}Date_Completed, {ta}Date_Authorised {ColPrefix}Date_Authorised, {ta}Authoriser {ColPrefix}Authoriser,");
            cols.AppendLine($"{ta}Description {ColPrefix}Description, {ta}USS_Waybill {ColPrefix}Waybill");
            cols.AppendLine($", {ta}USS_Transfer_Date {ColPrefix}Transfer_Date, {ta}USS_Collected_Date {ColPrefix}Collected_Date");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            string db = MOO.Settings.LIMS_DB_Name;

            //we will assume 3 levels deep for parent location
            StringBuilder sql = new();
            //the with part will give us a comma seperated list of tests that the sample will have run
            // this may eventually be simplified to use STRING_AGG function but our current version of SQL Server does not have this yet
            sql.AppendLine("WITH sampTest AS (");
            sql.AppendLine("select");
            sql.AppendLine("    sample,");
            sql.AppendLine("    stuff((");
            sql.AppendLine("        select ',' + t.analysis");
            sql.AppendLine($"        from {db}.[dbo].[TEST] t");
            sql.AppendLine("        where t.sample = t1.sample");
            sql.AppendLine("        order by t.analysis");
            sql.AppendLine("        for xml path('')");
            sql.AppendLine("    ),1,1,'') as tests_csv");
            sql.AppendLine($"from {db}.[dbo].[TEST] t1");
            sql.AppendLine("group by sample");
            sql.AppendLine(")");







            sql.AppendLine("SELECT st.tests_csv,");
            sql.AppendLine(GetColumns() + ",");
            sql.AppendLine(PhraseSvc.GetColumns("stat", "stat_") + ",");
            sql.AppendLine(PhraseSvc.GetColumns("stype", "stype_") + ",");
            sql.AppendLine(Sample_PointSvc.GetColumns("sp", "sp_") + ",");
            sql.AppendLine(PhraseSvc.GetColumns("oil", "oil_") + ",");
            sql.AppendLine(PhraseSvc.GetColumns("lube", "lube_") + ",");
            sql.AppendLine(LocationSvc.GetColumns("l", "l_") + ",");
            sql.AppendLine(LocationSvc.GetColumns("l2", "l2_") + ",");
            sql.AppendLine(LocationSvc.GetColumns("l3", "l3_"));
            sql.AppendLine($"FROM {db}.dbo.Sample s");
            sql.AppendLine($"LEFT JOIN {db}.dbo.phrase stat ON stat.phrase_type = 'SAMP_STAT' AND stat.phrase_id = s.status");
            sql.AppendLine($"LEFT JOIN {db}.dbo.phrase stype ON stype.phrase_type = 'SAMP_TYPE' AND stype.phrase_id = s.sample_type");
            sql.AppendLine($"LEFT JOIN {db}.dbo.sample_point sp ON sp.[IDENTITY] = s.sampling_point");
            sql.AppendLine($"LEFT JOIN {db}.dbo.phrase oil ON oil.phrase_type = 'OIL_TYPE' AND oil.phrase_id = sp.USS_Oil_Type");
            sql.AppendLine($"LEFT JOIN {db}.dbo.phrase lube ON lube.phrase_type = 'LBP_TYPE' AND lube.phrase_id = sp.USS_Lube_Point_Type");
            sql.AppendLine($"LEFT JOIN {db}.dbo.location l ON sp.point_location = l.[IDENTITY]");
            sql.AppendLine($"LEFT JOIN {db}.dbo.location l2 ON l.parent_location = l2.[IDENTITY]");
            sql.AppendLine($"LEFT JOIN {db}.dbo.location l3 ON l2.parent_location = l3.[IDENTITY]");
            sql.AppendLine("LEFT JOIN sampTest st ON s.id_numeric = st.sample");
            return sql.ToString();
        }


        internal static Sample DataRowToObject(DbDataReader row, string ColPrefix = "s_", string StatusPrefix = "stat_",
                                                        string SampTypePrfix="stype_", string SampPointPrefix = "sp_",
                                                       string OilPrefix = "oil_", string LubePrefix = "lube_",
                                                       string LocPrefix = "l_", string LocParentPrefix = "l2_",
                                                       string LocGrandParentPrefix = "l3_")
        {
            Sample RetVal = new();

            RetVal.Id_Numeric = int.Parse((string)Util.GetRowVal(row, $"{ColPrefix}Id_Numeric"));
            RetVal.Id_Text = (string)Util.GetRowVal(row, $"{ColPrefix}Id_Text");
            RetVal.Compared = ((string)Util.GetRowVal(row, $"{ColPrefix}Compared")) == "T";
            RetVal.On_Spec = ((string)Util.GetRowVal(row, $"{ColPrefix}On_Spec")) == "T";
            RetVal.Login_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}Login_Date");
            RetVal.Login_By = (string)Util.GetRowVal(row, $"{ColPrefix}Login_By");
            RetVal.Sampled_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}Sampled_Date");
            RetVal.Recd_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}Recd_Date");
            RetVal.DateStarted = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}DateStarted");
            RetVal.Date_Completed = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}Date_Completed");
            RetVal.Date_Authorised = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}Date_Authorised");
            RetVal.Authoriser = (string)Util.GetRowVal(row, $"{ColPrefix}Authoriser");
            RetVal.Description = (string)Util.GetRowVal(row, $"{ColPrefix}Description");
            RetVal.Waybill = (string)Util.GetRowVal(row, $"{ColPrefix}Waybill");
            RetVal.Transfer_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}Transfer_Date");
            RetVal.Collected_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}Collected_Date");

            string tests = (string)Util.GetRowVal(row, "tests_csv");
            if(!string.IsNullOrEmpty( tests ))
                RetVal.Tests = ((string)Util.GetRowVal(row, "tests_csv")).Split(',');
            else
                RetVal.Tests = [];

            if (!row.IsDBNull(row.GetOrdinal($"{SampPointPrefix}identity")))
            {
                RetVal.Sampling_Point = Sample_PointSvc.DataRowToObject(row, SampPointPrefix, OilPrefix, LubePrefix, LocPrefix, LocParentPrefix, LocGrandParentPrefix);
            }

            if (!row.IsDBNull(row.GetOrdinal($"{StatusPrefix}phrase_id")))
            {
                RetVal.Status = PhraseSvc.DataRowToObject(row, StatusPrefix);
            }

            if (!row.IsDBNull(row.GetOrdinal($"{SampTypePrfix}phrase_id")))
            {
                RetVal.Sample_Type = PhraseSvc.DataRowToObject(row, SampTypePrfix);
            }


            if (!row.IsDBNull(row.GetOrdinal($"{OilPrefix}phrase_id")))
            {
                RetVal.Oil_Type = PhraseSvc.DataRowToObject(row, OilPrefix);
            }

            return RetVal;
        }
    }
}
