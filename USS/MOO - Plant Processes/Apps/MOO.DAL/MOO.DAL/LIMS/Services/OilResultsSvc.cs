using Microsoft.Data.SqlClient;
using MOO.DAL.LIMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.LIMS.Services
{
    public static class OilResultsSvc
    {
        static OilResultsSvc()
        {
            Util.RegisterSqlServer();
        }



        public static List<OilResults> GetAll(DateTime StartDate, DateTime EndDate, string SamplePoint = "", string Location = "",
                                        string Equipment = "", bool ShowOnlyOutOfSpec = false, bool ShowOnlyNonValidate = false,
                                        int LimsBatchId = -1)
        {
            StringBuilder sql = new();

            List<OilResults> retVal = new();
            using SqlConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.LIMS_Read));
            conn.Open();
            SqlCommand cmd = new(sql.ToString(), conn);
            cmd.CommandText = GetSelect(cmd, StartDate, EndDate, SamplePoint, Location,
                                        Equipment, ShowOnlyOutOfSpec, ShowOnlyNonValidate,
                                        LimsBatchId);
            SqlDataReader rdr = cmd.ExecuteReader();
            //we will have to now do a pivot as the results are multiple records per sample
            long currentSampleID = -1;
            OilResults element = new();
            if (rdr.HasRows)
            {
                while(rdr.Read())
                {
                    if (long.Parse((string)Util.GetRowVal(rdr, $"Id_Numeric")) != currentSampleID)
                    {
                        element = DataRowToObject(rdr);
                        retVal.Add(element);
                        currentSampleID = element.Sample_Nbr;
                    }
                    AddResultValues(rdr, element);
                }
            }
            conn.Close();
            return retVal;
        }



        private static string GetSelect(SqlCommand Cmd, DateTime StartDate, DateTime EndDate, string SamplePoint = "", string Location = "",
                                        string Equipment = "", bool ShowOnlyOutOfSpec = false, bool ShowOnlyNonValidate = false,
                                        int LimsBatchId = -1)
        {
            string db = MOO.Settings.LIMS_DB_Name;

            //we will assume 3 levels deep for parent location
            StringBuilder sql = new();
            sql.AppendLine("WITH smp AS (SELECT str.id_numeric, str.sampled_date, str.login_date, sp.NAME sample_point, s.recd_date");
            sql.AppendLine($"				FROM {db}.dbo.samp_test_result str");
            sql.AppendLine($"				INNER JOIN {db}.dbo.SAMPLE s");
            sql.AppendLine("				ON str.ID_NUMERIC = s.ID_NUMERIC");
            sql.AppendLine($"				INNER JOIN {db}.dbo.SAMPLE_POINT sp");
            sql.AppendLine("				ON str.SAMPLING_POINT = sp.[IDENTITY]");
            sql.AppendLine($"				INNER JOIN {db}.dbo.result r");
            sql.AppendLine("				ON str.TEST_NUMBER = r.TEST_NUMBER AND str.component_name = r.name");
            sql.AppendLine($"                INNER JOIN {db}.dbo.location l");
            sql.AppendLine("                ON sp.point_location = l.[IDENTITY]");
            sql.AppendLine("                WHERE s.SAMPLE_TYPE = 'LUBE_OIL'");
            if (ShowOnlyNonValidate)
                sql.AppendLine("                AND s.status IN ('C', 'V')");
            else
                sql.AppendLine("                AND s.status IN ('A')");


            if (!string.IsNullOrEmpty(Location))
            {
                sql.AppendLine($"AND l.parent_location = @Location");
                Cmd.Parameters.AddWithValue("Location", Location);
            }
            if (!string.IsNullOrEmpty(Equipment))
            {
                sql.AppendLine($"AND l.[IDENTITY] = @Equipment");
                Cmd.Parameters.AddWithValue("Equipment", Equipment);
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
            if (!string.IsNullOrEmpty(SamplePoint))
            {
                sql.AppendLine($"AND str.sampling_point = @SamplePoint");
                Cmd.Parameters.AddWithValue("SamplePoint", SamplePoint);
            }



            sql.AppendLine("            AND s.recd_date >= @StartDate");
            sql.AppendLine("AND s.recd_date <= @EndDate");
            sql.AppendLine("                GROUP BY str.id_numeric, str.sampled_date, sp.NAME, s.recd_date, str.login_date)");
            sql.AppendLine("SELECT smp.*, str.status,str.component_name,str.ANALYSIS, str.result_value,str.out_of_range oos,");
            sql.AppendLine("            mlpv.MIN_LIMIT, mlpv.MAX_LIMIT");
            sql.AppendLine("FROM smp");
            sql.AppendLine($"INNER JOIN {db}.dbo.samp_test_result str");
            sql.AppendLine("ON smp.ID_NUMERIC = str.ID_NUMERIC");
            sql.AppendLine($"INNER JOIN {db}.dbo.result r");
            sql.AppendLine("ON str.TEST_NUMBER = r.TEST_NUMBER AND str.component_name = r.name");
            sql.AppendLine($"LEFT OUTER JOIN {db}.dbo.mlp_header mlp ON mlp.[IDENTITY] = str.PRODUCT and mlp.PRODUCT_VERSION = str.PRODUCT_VERSION");
            sql.AppendLine($"LEFT OUTER JOIN {db}.dbo.mlp_components mlpc ON mlp.[IDENTITY] = mlpc.PRODUCT_ID ");
            sql.AppendLine("						AND str.ANALYSIS = mlpc.ANALYSIS_ID");
            sql.AppendLine("						AND str.COMPONENT_NAME = mlpc.COMPONENT_NAME");
            sql.AppendLine($"LEFT OUTER JOIN {db}.dbo.MLP_VALUES mlpv on mlpc.ENTRY_CODE = mlpv.ENTRY_CODE AND mlpv.level_id = 'FAIL'");
            //put a dummy where clause in so we don't need to check if we need a "WHERE" or "AND" on subsequent filters
            //this is me being lazy
            sql.AppendLine("WHERE 1=1");


            Cmd.Parameters.AddWithValue("StartDate", StartDate);
            Cmd.Parameters.AddWithValue("EndDate", EndDate);


            if (ShowOnlyOutOfSpec)
            {
                sql.AppendLine("AND str.out_of_range = 'T'");
            }

            sql.AppendLine("order by ID_NUMERIC");
            return sql.ToString();
        }


        private static OilResults DataRowToObject(DbDataReader row)
        {
#pragma warning disable IDE0017 // Simplify object initialization
            OilResults RetVal = new();
#pragma warning restore IDE0017 // Simplify object initialization

            RetVal.Sample_Nbr = int.Parse((string)Util.GetRowVal(row, $"Id_Numeric"));
            RetVal.Sample_Date = (DateTime)Util.GetRowVal(row, $"sampled_date");
            RetVal.Login_Date = (DateTime)Util.GetRowVal(row, $"login_date");
            RetVal.Recd_Date = (DateTime?)Util.GetRowVal(row, $"recd_date");
            RetVal.SamplePoint = (string)Util.GetRowVal(row, $"sample_point");
            RetVal.Status = (string)Util.GetRowVal(row, $"status");

            return RetVal;
        }

        private static void AddResultValues(DbDataReader row, OilResults Oil)
        {
            Type myType = typeof(OilResults);
            string component = TranslateComponent((string)Util.GetRowVal(row, $"Analysis"), (string)Util.GetRowVal(row, "component_name"));

            //set the result value
            var prop = myType.GetProperty(component, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (prop != null)
                prop.SetValue(Oil, (double?)Util.GetRowVal(row, "result_value"));

            //Set Out Of Spec
            prop = myType.GetProperty($"{component}_OOS", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (prop != null)
                prop.SetValue(Oil, ((string)Util.GetRowVal(row, "oos")) == "T");
            //Set Min/Max values

            var minValProp = myType.GetProperty($"{component}_MIN", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            var maxValProp = myType.GetProperty($"{component}_MAX", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            double oosVal;
            if (!row.IsDBNull(row.GetOrdinal("min_limit")))
            {



                if (!row.IsDBNull(row.GetOrdinal("min_limit")) && ((string)Util.GetRowVal(row, "min_limit")).Contains('>'))
                {
                    //if the min value contains a ">" then this is the max value and there is no min and this is the max instead
                    if (double.TryParse(((string)Util.GetRowVal(row, "min_limit")).Replace(">", ""), out oosVal))
                        minValProp.SetValue(Oil, oosVal);
                }
                else if (!row.IsDBNull(row.GetOrdinal("min_limit")))
                {
                    //handle for any other value in Min_Limit field
                    //min_limit field may contain a "<" this just means there is no max limit
                    if (double.TryParse(((string)Util.GetRowVal(row, "min_limit")).Replace("<", ""), out oosVal))
                        minValProp.SetValue(Oil, oosVal);
                }
            }
            if (!row.IsDBNull(row.GetOrdinal("max_limit")) && !string.IsNullOrEmpty((string)Util.GetRowVal(row, "max_limit")))
            {
                //handle for any other value in Min_Limit field
                //min_limit field may contain a "<" this just means there is no max limit
                if (double.TryParse(((string)Util.GetRowVal(row, "max_limit")).Replace("<", ""), out oosVal))
                    maxValProp.SetValue(Oil, oosVal);
            }

        }

        private static string TranslateComponent(string AnalysisName, string ComponentName)
        {

            if (ComponentName == "Oxidation (A/cm)")
                return "Oxidation";
            else if (ComponentName == "Soot 1980 (A/cm)")
                return "Soot_1980";
            else if (ComponentName == "Soot 3800 (A/cm)")
                return "Soot_3800";
            else if (ComponentName == "Water (%)")
                return "Water";
            else if (ComponentName == "PQ Value")
                return "PQ";
            else if (ComponentName == "Glycol (%)")
                return "Glycol";
            else if (AnalysisName == "GC" && ComponentName == "Glycol")
                return "Glycol_GC";
            else
                return ComponentName.ToUpper();

        }
    }
}
