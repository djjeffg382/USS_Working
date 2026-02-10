using MOO.DAL.Core.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Core.Services
{
    public static class ApprovalSvc
    {
        static ApprovalSvc()
        {
            Util.RegisterOracle();
        }


        public static Approval Get(decimal approval_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE approval_id = :approval_id");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("approval_id", approval_id);
            da.SelectCommand.BindByName = true;

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }



        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}approval_id {ColPrefix}approval_id, {ta}approved_by {ColPrefix}approved_by, ");
            cols.AppendLine($"{ta}approved_date {ColPrefix}approved_date");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM core.approval");
            return sql.ToString();
        }


        public static int Insert(Approval obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Approval obj, OracleConnection conn)
        {
            if (obj.Approval_Id <= 0)
                obj.Approval_Id = Convert.ToInt32(MOO.Data.GetNextSequence("core.seq_approval"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO core.Approval(");
            sql.AppendLine("approval_id, approved_by, approved_date)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":approval_id, :approved_by, :approved_date)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("approval_id", obj.Approval_Id);
            ins.Parameters.Add("approved_by", obj.Approved_By);
            ins.Parameters.Add("approved_date", obj.Approved_Date);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        /// <summary>
        /// Adds approval to the specified metric Id
        /// </summary>
        /// <param name="MetricId"></param>
        /// <param name="StartDateNoDST"></param>
        /// <param name="Apprvl"></param>
        public static int ApproveMetricValue(long MetricId, DateTime StartDateNoDST, Approval Apprvl, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("update core.metric_value");
            sql.AppendLine("set approval_id = :approval_id");
            sql.AppendLine("WHERE metric_id = :metric_id AND start_date_no_dst = :sdate");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("approval_id", Apprvl.Approval_Id);
            upd.Parameters.Add("metric_id", MetricId);
            upd.Parameters.Add("sdate", StartDateNoDST);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        /// <summary>
        /// Adds approval to the specified metric Id
        /// </summary>
        /// <param name="MetricId"></param>
        /// <param name="StartDateNoDST"></param>
        /// <param name="Apprvl"></param>
        public static int ApproveMetricValue(long MetricId, DateTime StartDateNoDST, Approval Apprvl)
        {
            int recsAffected;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            recsAffected = ApproveMetricValue(MetricId, StartDateNoDST, Apprvl, conn);
            conn.Close();
            return recsAffected;
        }




        /// <summary>
        /// removes the approval from all metrics
        /// </summary>
        /// <param name="Approval_Id"></param>
        public static int RemoveApproval(int Approval_Id)
        {
            int recsAffected = 0;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            OracleTransaction trans = conn.BeginTransaction();
            try
            {
                recsAffected = RemoveApproval(Approval_Id, conn);
                trans.Commit();
            }
            catch (Exception)
            {
                if (trans != null)
                    trans.Rollback();
                throw;
            }
            finally
            {
                conn.Close();
            }
            return recsAffected;
        }


        /// <summary>
        /// removes the approval from all metrics
        /// </summary>
        /// <param name="Approval_Id"></param>
        public static int RemoveApproval(int Approval_Id, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("update core.metric_value");
            sql.AppendLine("set approval_id = null");
            sql.AppendLine("WHERE approval_id = :approval_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("approval_id", Approval_Id);
            MOO.Data.ExecuteNonQuery(upd);
            //now delete the approval
            sql.Clear();
            sql.AppendLine("DELETE FROM core.Approval");
            sql.AppendLine("WHERE approval_id = :approval_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("approval_id", Approval_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }



        internal static Approval DataRowToObject(DataRow row, string ColPrefix = "")
        {
            Approval RetVal = new();
            RetVal.Approval_Id = (int)row.Field<decimal>($"{ColPrefix}approval_id");
            RetVal.Approved_By = row.Field<string>($"{ColPrefix}approved_by");
            RetVal.Approved_Date = row.Field<DateTime>($"{ColPrefix}approved_date");
            return RetVal;
        }

    }
}
