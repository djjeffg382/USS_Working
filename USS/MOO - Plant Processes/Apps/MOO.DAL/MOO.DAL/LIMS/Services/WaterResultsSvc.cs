using Microsoft.Data.SqlClient;
using MOO.DAL.LIMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.LIMS.Services
{
    public static class WaterResultsSvc
    {
        static WaterResultsSvc()
        {
            Util.RegisterSqlServer();
        }



        public static List<WaterResults> GetAll(DateTime StartDate, DateTime EndDate, string SamplePoint = "", bool ShowOnlyNonValidate = false,
                                        int LimsBatchId = -1)
        {
            StringBuilder sql = new();

            List<WaterResults> retVal = new();
            using SqlConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.LIMS_Read));
            conn.Open();
            SqlCommand cmd = new(sql.ToString(), conn);
            cmd.CommandText = GetSelect(cmd, StartDate, EndDate, SamplePoint, ShowOnlyNonValidate,
                                        LimsBatchId);
            SqlDataReader rdr = cmd.ExecuteReader();
            //we will have to now do a pivot as the results are multiple records per sample
            long currentSampleID = -1;
            WaterResults element = new();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    element = DataRowToObject(rdr);
                    retVal.Add(element);
                    currentSampleID = element.Sample_Nbr;
                }
            }
            conn.Close();
            return retVal;
        }



        private static string GetSelect(SqlCommand Cmd, DateTime StartDate, DateTime EndDate, string SamplePoint = "",
                                    bool ShowOnlyNonValidate = false, int LimsBatchId = -1)
        {
            string db = MOO.Settings.LIMS_DB_Name;

            //we will assume 3 levels deep for parent location
            StringBuilder sql = new();
            sql.AppendLine("SELECT PivotTable.*, ROW_NUMBER() OVER(ORDER BY id_numeric) rn");
            sql.AppendLine("FROM (");
            sql.AppendLine("    SELECT str.id_numeric, sp.name sample_point, str.component_name, str.result_value,");
            sql.AppendLine("        str.login_date, s.recd_date");
            sql.AppendLine(",str.sampled_date");
            sql.AppendLine($"    FROM {db}.dbo.SAMP_TEST_RESULT str");
            sql.AppendLine($"    INNER JOIN {db}.dbo.sample s");
            sql.AppendLine("    ON s.id_numeric = str.id_numeric");
            sql.AppendLine($"	LEFT OUTER JOIN {db}.dbo.SAMPLE_POINT sp");
            sql.AppendLine("	ON sp.[IDENTITY] = str.SAMPLING_POINT");
            sql.AppendLine("    WHERE s.sample_type = 'WATER' AND str.sampled_date >= @StartDate");
            sql.AppendLine("AND str.sampled_date <= @EndDate");
            if (ShowOnlyNonValidate)
                sql.AppendLine("                AND s.status IN ('C', 'V')");
            else
                sql.AppendLine("                AND s.status IN ('A')");

            if (!string.IsNullOrEmpty(SamplePoint))
            {
                sql.AppendLine("AND sp.[IDENTITY] = @SamplePoint");
                Cmd.Parameters.AddWithValue("SamplePoint", SamplePoint);
            }


            if (LimsBatchId > 0)
            {
                //get all samples for this batch then just include these samples
                var bList = MOO.DAL.ToLive.Services.LIMS_Batch_SamplesSvc.Get(LimsBatchId);
                if (bList.Count > 0)
                {
                    StringBuilder samples = new();
                    foreach (var b in bList)
                    {
                        if (samples.Length > 0)
                            samples.Append(',');
                        samples.Append($"{b.Sample_Number}");
                    }
                    sql.AppendLine($"AND str.id_numeric IN ({samples.ToString()})");
                }
            }


            sql.AppendLine(") srcTable");
            sql.AppendLine("PIVOT (");
            sql.AppendLine("        MAX(result_value)");
            sql.AppendLine("        FOR component_name IN ([Ca],[Fe],[K],[Mg],[Na],[Bromide],[Chloride],[Fluoride],[Nitrate],[Nitrite],[Phosphate],[Sulfate],[pH],[Alkalinity],[Conductivity])");
            sql.AppendLine(") AS PivotTable");

            Cmd.Parameters.AddWithValue("StartDate", StartDate);
            Cmd.Parameters.AddWithValue("EndDate", EndDate);

            return sql.ToString();
        }


        private static WaterResults DataRowToObject(DbDataReader row)
        {
#pragma warning disable IDE0017 // Simplify object initialization
            WaterResults RetVal = new();
#pragma warning restore IDE0017 // Simplify object initialization

            RetVal.Sample_Nbr = int.Parse((string)Util.GetRowVal(row, $"Id_Numeric"));
            RetVal.Sample_Date = (DateTime)Util.GetRowVal(row, $"sampled_date");
            RetVal.Login_Date = (DateTime)Util.GetRowVal(row, $"login_date");
            RetVal.Recd_Date = (DateTime?)Util.GetRowVal(row, $"recd_date");
            RetVal.SamplePoint = (string)Util.GetRowVal(row, $"sample_point");

            RetVal.Ca = (double?)Util.GetRowVal(row, $"ca");
            RetVal.Fe = (double?)Util.GetRowVal(row, $"Fe");
            RetVal.K = (double?)Util.GetRowVal(row, $"K");
            RetVal.Mg = (double?)Util.GetRowVal(row, $"Mg");
            RetVal.Na = (double?)Util.GetRowVal(row, $"Na");
            RetVal.Bromide = (double?)Util.GetRowVal(row, $"Bromide");
            RetVal.Chloride = (double?)Util.GetRowVal(row, $"Chloride");
            RetVal.Fluoride = (double?)Util.GetRowVal(row, $"Fluoride");
            RetVal.Nitrate = (double?)Util.GetRowVal(row, $"Nitrate");
            RetVal.Nitrite = (double?)Util.GetRowVal(row, $"Nitrite");
            RetVal.Phosphate = (double?)Util.GetRowVal(row, $"Phosphate");
            RetVal.Sulfate = (double?)Util.GetRowVal(row, $"Sulfate");
            RetVal.PH = (double?)Util.GetRowVal(row, $"PH");
            RetVal.Alkalinity = (double?)Util.GetRowVal(row, $"Alkalinity");
            RetVal.Conductivity = (double?)Util.GetRowVal(row, $"Conductivity");

            return RetVal;
        }
    }
}
