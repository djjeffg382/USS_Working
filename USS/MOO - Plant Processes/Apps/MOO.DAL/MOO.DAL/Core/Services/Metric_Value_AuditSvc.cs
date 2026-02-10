using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.DAL.Core.Models;
using Oracle.ManagedDataAccess.Client;

namespace MOO.DAL.Core.Services
{
    public class Metric_Value_AuditSvc
    {

        static Metric_Value_AuditSvc()
        {
            Util.RegisterOracle();
        }
        /// <summary>
        /// Get the audit records for a specified Metric Value record
        /// </summary>
        /// <param name="metric_Id"></param>
        /// <param name="start_Date_No_DST"></param>
        /// <returns></returns>
        public static List<Metric_Value_Audit> GetAuditsForMetricValue(long metric_Id, DateTime start_Date_No_DST)
        {
            List<Metric_Value_Audit> retObj = new();
            StringBuilder sql = new();
            sql.AppendLine("SELECT metric_id, start_date_no_dst, old_value,");
            sql.AppendLine("    new_value, date_changed, changed_by");
            sql.AppendLine("FROM core.metric_value_audit");
            sql.AppendLine($"WHERE metric_id = {metric_Id}");
            sql.AppendLine($"AND start_date_no_dst = {MOO.Dates.OraDate(start_Date_No_DST)}");
            sql.AppendLine("ORDER BY date_changed");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            foreach(DataRow row in ds.Tables[0].Rows)
            {
                retObj.Add(DataRowToObj(row));
            }
            return retObj;
        }


        /// <summary>
        /// Inserts the audit record into the Metric Value Audit table
        /// </summary>
        /// <param name="mva"></param>
        /// <param name="conn"></param>
        public static void Insert(Metric_Value_Audit mva, OracleConnection conn)
        {
            StringBuilder sql = new();

            sql.AppendLine("INSERT INTO core.metric_value_audit");
            sql.AppendLine("(metric_id, start_date_no_dst, old_value, new_value,");
            sql.AppendLine("    date_changed,changed_by)");
            sql.AppendLine("VALUES(:metric_id, :start_date_no_dst, :old_value, :new_value,");
            sql.AppendLine("    :date_changed, :changed_by)");

            OracleCommand cmd = new(sql.ToString(), conn);

            cmd.Parameters.Add("metric_id", mva.Metric_Id);
            cmd.Parameters.Add("start_date_no_dst", mva.Start_Date_No_DST);

            cmd.Parameters.Add("old_value", mva.Old_Value);
            cmd.Parameters.Add("new_value", mva.New_Value);
            cmd.Parameters.Add("date_changed", mva.Date_Changed);
            cmd.Parameters.Add("changed_by", mva.Changed_By);

            cmd.ExecuteNonQuery();


        }


        /// <summary>
        /// converts the datarow to an object
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private static Metric_Value_Audit DataRowToObj(DataRow row)
        {


            Metric_Value_Audit retObj = new();
            retObj.Metric_Id = Convert.ToInt64(row["metric_id"]);
            retObj.Start_Date_No_DST = row.Field<DateTime>("start_date_no_dst");
            retObj.Old_Value = row.Field<decimal?>("old_value");
            retObj.New_Value = row.Field<decimal?>("new_value");
            retObj.Date_Changed = row.Field<DateTime>("date_changed");
            retObj.Changed_By = row.IsNull("changed_by") ? "" : row["changed_by"].ToString();


            return retObj;
        }
    }
}
