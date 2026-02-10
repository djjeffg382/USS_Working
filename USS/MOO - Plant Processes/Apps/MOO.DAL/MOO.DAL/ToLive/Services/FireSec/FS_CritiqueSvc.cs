using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Services
{
    public static class FS_CritiqueSvc
    {
        static FS_CritiqueSvc()
        {
            Util.RegisterOracle();
        }
        /// <summary>
        /// Get only record based on id
        /// </summary>
        /// <param name="postID"></param>
        /// <returns></returns>
        public static FS_Critique Get(int postID)
        {
            StringBuilder sql = new();
            sql.AppendLine(GetSelect());
            sql.AppendLine($"WHERE postId = {postID}");

            DataSet ds = Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count == 1)
            {
                return DataRowToObject(ds.Tables[0].Rows[0]);
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// Return all data or search for postid
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        public static List<FS_Critique> GetAll(int postId = 0)
        {
            StringBuilder sql = new();
            sql.AppendLine(GetSelect());

            if (postId > 0)
                sql.AppendLine($"Where postId = {postId}");

            DataSet ds = Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            List<FS_Critique> Elements = new();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                FS_Critique fs = new();
                Elements.Add(DataRowToObject(dr));
            }
            return Elements;
        }
        private static string GetSelect(string addField = "")
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT fs.*,");
            sql.AppendLine("person_id, last_name, first_name, middle_name, employee_number, work_location_code, ");
            sql.AppendLine("supervisor_person_id, low_level_group, base_level_group, cost_center, status_ind, ");
            sql.AppendLine("windows_ad_account, home_number, mobile_number, office_ext");

            if (!string.IsNullOrEmpty(addField))
            {
                sql.Append(", ");
                sql.AppendLine(addField);
            }

            sql.AppendLine("FROM tolive.fs_critique fs");
            sql.AppendLine("INNER JOIN tolive.people p");
            sql.AppendLine("ON p.person_id = fs.preparedby");
            return sql.ToString();
        }

        public static int Update(FS_Critique obj)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                int recsAffected = Update(obj, conn);
                return recsAffected;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
        }


        public static int Update(FS_Critique obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.fs_critique SET");
            sql.AppendLine("plant = :plant, ");
            sql.AppendLine("location = :location, ");
            sql.AppendLine("dateentered = :dateentered, ");
            sql.AppendLine("critiquetype = :critiquetype, ");
            sql.AppendLine("enteredby = :enteredby, ");
            sql.AppendLine("preparedby = :preparedby, ");
            sql.AppendLine("specificlocation = :specificlocation, ");
            sql.AppendLine("scenariodesc = :scenariodesc, ");
            sql.AppendLine("alarmsreport = :alarmsreport, ");
            sql.AppendLine("alarmsreportdesc = :alarmsreportdesc, ");
            sql.AppendLine("accountability = :accountability, ");
            sql.AppendLine("accountabilitydesc = :accountabilitydesc, ");
            sql.AppendLine("response = :response, ");
            sql.AppendLine("responsedesc = :responsedesc, ");
            sql.AppendLine("utilitiesupdown = :utilitiesupdown, ");
            sql.AppendLine("utilitiesupdowndesc = :utilitiesupdowndesc, ");
            sql.AppendLine("sopsused = :sopsused, ");
            sql.AppendLine("sopsdesc = :sopsdesc, ");
            sql.AppendLine("operrecommendation = :operrecommendation, ");
            sql.AppendLine("safetyrecommendation = :safetyrecommendation, ");
            sql.AppendLine("trainingrecommendation = :trainingrecommendation,");

            sql.AppendLine("ActionRequired = :ActionRequired, ");
            sql.AppendLine("ActionRequiredDesc = :ActionRequiredDesc, ");
            sql.AppendLine("Resolved = :Resolved, ");
            sql.AppendLine("ResolvedDesc = :ResolvedDesc, ");
            sql.AppendLine("LastEmailSent = :LastEmailSent");

            sql.AppendLine("WHERE postid = :postid");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.Parameters.Add("plant", obj.Plant.ToString());
            upd.Parameters.Add("location", obj.Location.Name);
            upd.Parameters.Add("dateentered", obj.DateEntered);
            upd.Parameters.Add("critiquetype", obj.Critique_Type);
            upd.Parameters.Add("enteredby", obj.Entered_By);
            upd.Parameters.Add("preparedby", obj.Prepared_By.Person_Id);
            upd.Parameters.Add("specificlocation", obj.Specific_Location);
            upd.Parameters.Add("scenariodesc", obj.Scenario_Desc);
            upd.Parameters.Add("alarmsreport", obj.Alarms_Report);
            upd.Parameters.Add("alarmsreportdesc", obj.AlarmsReport_Desc);
            upd.Parameters.Add("accountability", obj.Alarms_Report);
            upd.Parameters.Add("accountabilitydesc", obj.AlarmsReport_Desc);
            upd.Parameters.Add("response", obj.Response);
            upd.Parameters.Add("responsedesc", obj.Response_Desc);
            upd.Parameters.Add("utilitiesupdown", obj.UtilitiesUpDown);
            upd.Parameters.Add("utilitiesupdowndesc", obj.UtilitiesUpDown_Desc);
            upd.Parameters.Add("sopsused", obj.SOPsUsed);
            upd.Parameters.Add("sopsdesc", obj.SOPs_Desc);
            upd.Parameters.Add("operrecommendation", obj.Oper_Recommandation);
            upd.Parameters.Add("safetyrecommendation", obj.Safety_Recommandation);
            upd.Parameters.Add("trainingrecommendation", obj.Training_Recommedation);

            upd.Parameters.Add("ActionRequired", obj.Action_Required);
            upd.Parameters.Add("ActionRequiredDesc", obj.Action_Required_Desc);
            upd.Parameters.Add("Resolved", obj.Resolved);
            upd.Parameters.Add("ResolvedDesc", obj.Resolved_Desc);
            upd.Parameters.Add("LastEmailSent", obj.Last_Email_Sent);

            upd.Parameters.Add("postid", obj.Post_ID);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static void Insert(FS_Critique crit, OracleConnection conn)
        {
            //get the next id for the nrole
            if (crit.Post_ID <= 0)
                crit.Post_ID = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.seq_firesec"));
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.fs_critique (postID, plant, location, DateEntered,");
            sql.AppendLine("critiqueType, EnteredBy, PreparedBy, SpecificLocation, ScenarioDesc,");
            sql.AppendLine("AlarmsReport, AlarmsReportDesc, Accountability, AccountabilityDesc,");
            sql.AppendLine("Response, ResponseDesc, UtilitiesUpDown, UtilitiesUpDownDesc,");
            sql.AppendLine("SOPsUsed, SOPsDesc, OperRecommendation, ");
            sql.AppendLine("SafetyRecommendation, TrainingRecommendation,ActionRequired,ActionRequiredDesc,Resolved,ResolvedDesc,LastEmailSent)");
            sql.AppendLine("VALUES(:postID, :plant, :location, :DateEntered,");
            sql.AppendLine(":critiqueType, :EnteredBy, :PreparedBy, :SpecificLocation, :ScenarioDesc,");
            sql.AppendLine(":AlarmsReport, :AlarmsReportDesc, :Accountability, :AccountabilityDesc,");
            sql.AppendLine(":Response, :ResponseDesc, :UtilitiesUpDown, :UtilitiesUpDownDesc,");
            sql.AppendLine(":SOPsUsed, :SOPsDesc, :OperRecommendation, ");
            sql.AppendLine(":SafetyRecommendation, :TrainingRecommendation,:ActionRequired,:ActionRequiredDesc,:Resolved,:ResolvedDesc,:LastEmailSent)");

            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("postID", crit.Post_ID);
            ins.Parameters.Add("plant", crit.Plant.ToString());
            ins.Parameters.Add("location", crit.Location.Name);
            ins.Parameters.Add("DateEntered", crit.DateEntered);
            ins.Parameters.Add("critiqueType", crit.Critique_Type);
            ins.Parameters.Add("EnteredBy", crit.Entered_By);
            ins.Parameters.Add("PreparedBy", crit.Prepared_By.Person_Id);
            ins.Parameters.Add("SpecificLocation", crit.Specific_Location);
            ins.Parameters.Add("ScenarioDesc", crit.Scenario_Desc);
            ins.Parameters.Add("AlarmsReport", crit.Alarms_Report);
            ins.Parameters.Add("AlarmsReportDesc", crit.AlarmsReport_Desc);
            ins.Parameters.Add("Accountability", crit.Alarms_Report);
            ins.Parameters.Add("AccountabilityDesc", crit.AlarmsReport_Desc);
            ins.Parameters.Add("Response", crit.Response);
            ins.Parameters.Add("ResponseDesc", crit.Response_Desc);
            ins.Parameters.Add("UtilitiesUpDown", crit.UtilitiesUpDown);
            ins.Parameters.Add("UtilitiesUpDownDesc", crit.UtilitiesUpDown_Desc);
            ins.Parameters.Add("SOPsUsed", crit.SOPsUsed);
            ins.Parameters.Add("SOPsDesc", crit.SOPs_Desc);
            ins.Parameters.Add("OperRecommendation", crit.Oper_Recommandation);
            ins.Parameters.Add("SafetyRecommendation", crit.Safety_Recommandation);
            ins.Parameters.Add("TrainingRecommendation", crit.Training_Recommedation);

            ins.Parameters.Add("ActionRequired", crit.Action_Required);
            ins.Parameters.Add("ActionRequiredDesc", crit.Action_Required_Desc);
            ins.Parameters.Add("Resolved", crit.Resolved);
            ins.Parameters.Add("ResolvedDesc", crit.Resolved_Desc);
            ins.Parameters.Add("LastEmailSent", crit.Last_Email_Sent);

            MOO.Data.ExecuteNonQuery(ins);
        }
        public static void Insert(FS_Critique fs)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                Insert(fs, conn);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        //public static PagedData<List<FS_Critique>> GetPagedData(string pName, int offset, int totalItems, string orderBy = "DateEntered", string orderDirection = "ASC")
        //{
        //    PagedData<List<Error>> retObj = new();
        //    StringBuilder sql = new();
        //    OracleDataAdapter da = new("", MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
        //    string filter = AddFilters(pName, da.SelectCommand);

        //    sql.AppendLine("SELECT * FROM (");
        //    sql.AppendLine("SELECT error_date, error_num, pname, error_sql, error_type,");
        //    sql.AppendLine("    error_desc, error_stack,");
        //    sql.AppendLine($"    ROW_NUMBER() OVER(ORDER BY {orderBy} {orderDirection}) rn");
        //    sql.AppendLine("FROM tolive.error");
        //    sql.AppendLine(filter);
        //    sql.AppendLine(") tbl");
        //    sql.AppendLine($"WHERE rn BETWEEN {offset} AND {offset + totalItems}");
        //    da.SelectCommand.CommandText = sql.ToString();
        //    DataSet ds = MOO.Data.ExecuteQuery(da);

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        retObj.Data.Add(DataRowToObject(row));
        //    }
        //    retObj.TotalRows = GetOraErrLogsCountAsync(startDate, endDate, error_Type, error_Desc, pName, error_Sql);

        //    return retObj;

        //}

        //private static string AddFilters(string pName, OracleCommand cmd)
        //{
        //    StringBuilder filter = new();

        //    /*****************Start/End Date Parameter*************/
        //    if (startDate.HasValue && endDate.HasValue)
        //    {
        //        filter.AppendLine("WHERE error_date BETWEEN :startDate AND :endDate");
        //        cmd.Parameters.Add("startDate", startDate.Value);
        //        cmd.Parameters.Add("endDate", endDate.Value);
        //    }
        //    /*****************Error Desc Parameter*************/
        //    if (!string.IsNullOrEmpty(error_Desc))
        //    {
        //        if (filter.Length == 0)
        //            filter.Append("WHERE ");
        //        else
        //            filter.Append("AND ");

        //        filter.AppendLine("error_desc LIKE :error_dec");
        //        cmd.Parameters.Add("error_desc", $"%{error_Desc}%");
        //    }
        //    /*************PName Parameter*******************/
        //    if (!string.IsNullOrEmpty(pName))
        //    {
        //        if (filter.Length == 0)
        //            filter.Append("WHERE ");
        //        else
        //            filter.Append("AND ");

        //        filter.AppendLine("pName LIKE :pName");
        //        cmd.Parameters.Add("pName", $"%{pName}%");
        //    }
        //    /*************error_Sql Parameter*******************/
        //    if (!string.IsNullOrEmpty(error_Sql))
        //    {
        //        if (filter.Length == 0)
        //            filter.Append("WHERE ");
        //        else
        //            filter.Append("AND ");

        //        filter.AppendLine("error_Sql LIKE :error_Sql");
        //        cmd.Parameters.Add("error_Sql", $"%{error_Sql}%");
        //    }
        //    if (error_Type != null && error_Type.Length > 0)
        //    {
        //        string errTypeList = string.Join(", ", error_Type.Select(e => $"{(int)e}"));

        //        if (filter.Length == 0)
        //            filter.Append("WHERE ");
        //        else
        //            filter.Append("AND ");

        //        filter.AppendLine($"error_type IN ({errTypeList})");
        //    }

        //    return filter.ToString();
        //}

        private static FS_Critique DataRowToObject(DataRow row)
        {
            FS_Critique RetVal = new();
            RetVal.Post_ID = Convert.ToInt32(row["postID"]);
            RetVal.Plant = Enum.Parse<MOO.Plant>(row["plant"].ToString());
            RetVal.Location = MOO.PlantAreaList.GetByName(row["location"].ToString(), RetVal.Plant);
            RetVal.DateEntered = Convert.ToDateTime(row["DateEntered"]);
            RetVal.Critique_Type = Convert.ToInt32(row["critiqueType"]);
            RetVal.Entered_By = row["EnteredBy"].ToString();
            RetVal.Prepared_By = PeopleSvc.DataRowToObject(row);
            RetVal.Specific_Location = row["SpecificLocation"].ToString();
            RetVal.Scenario_Desc = row["ScenarioDesc"].ToString();
            RetVal.Alarms_Report = Convert.ToInt32(row["Response"]);
            RetVal.AlarmsReport_Desc = row["ResponseDesc"].ToString();
            RetVal.Accountability = Convert.ToInt32(row["Accountability"]);
            RetVal.Accountability_Desc = row["AccountabilityDesc"].ToString();
            RetVal.Response = Convert.ToInt32(row["Response"]);
            RetVal.Response_Desc = row["ResponseDesc"].ToString();
            RetVal.UtilitiesUpDown = Convert.ToInt32(row["UtilitiesUpDown"]);
            RetVal.UtilitiesUpDown_Desc = row["UtilitiesUpDownDesc"].ToString();
            RetVal.SOPsUsed = Convert.ToInt32(row["SOPsUsed"]);
            RetVal.SOPs_Desc = row.IsNull("SOPsDesc") ? "" : row["SOPsDesc"].ToString();
            RetVal.Oper_Recommandation = row["OperRecommendation"].ToString();
            RetVal.Safety_Recommandation = row["SafetyRecommendation"].ToString();
            RetVal.Training_Recommedation = row["TrainingRecommendation"].ToString();

            RetVal.Action_Required = row.IsNull("ActionRequired") ?0:Convert.ToInt32(row["ActionRequired"]);
            RetVal.Action_Required_Desc = row.IsNull("ActionRequiredDesc") ? "" : row["ActionRequiredDesc"].ToString();
            RetVal.Resolved = row.IsNull("Resolved") ? 0 : Convert.ToInt32(row["Resolved"]);
            RetVal.Resolved_Desc = row.IsNull("ResolvedDesc") ? "" : row["ResolvedDesc"].ToString();
            RetVal.Last_Email_Sent = row.IsNull("LastEmailSent") ? null : Convert.ToDateTime(row["LastEmailSent"]);

            return RetVal;
        }
    }
}
