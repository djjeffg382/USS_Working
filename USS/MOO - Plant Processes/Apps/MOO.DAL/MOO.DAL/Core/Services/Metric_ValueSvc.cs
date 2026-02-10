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
    public class Metric_ValueSvc
    {
        /// <summary>
        /// Do not allow an insert before this date
        /// </summary>
        private static readonly DateTime CHECK_DATE = DateTime.Parse("1/1/1980");

        static Metric_ValueSvc()
        {
            Util.RegisterOracle();
        }

        /// <summary>
        /// gets the values from the Metric Value table by date range
        /// </summary>
        /// <param name="metric_Id"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static List<Metric_Value> GetByDateRange(long metric_Id, DateTime startDate, DateTime endDate)
        {
            List<Metric_Value> retVal = [];
            StringBuilder sql = new();
            sql.AppendLine(GetSelect());
            sql.AppendLine($"WHERE metric_id = {metric_Id}");
            sql.AppendLine($"AND start_date BETWEEN {MOO.Dates.OraDate(startDate, false)} AND {MOO.Dates.OraDate(endDate, false)}");
            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                retVal.Add(DataRowToObj(row));
            }

            return retVal;
        }

        /// <summary>
        /// gets the value by metric ID and start_date_No_DST
        /// </summary>
        /// <param name="metric_Id"></param>
        /// <param name="startDateNoDST"></param>
        /// <returns></returns>
        public static Metric_Value Get(long metric_Id, DateTime startDateNoDST)
        {
            //strip milliseconds from the date
            DateTime sd = new(startDateNoDST.Year, startDateNoDST.Month, startDateNoDST.Day,
                                            startDateNoDST.Hour, startDateNoDST.Minute, startDateNoDST.Second);

            StringBuilder sql = new();
            sql.AppendLine(GetSelect());
            sql.AppendLine($"WHERE metric_id = {metric_Id}");
            sql.AppendLine($"AND start_date_no_dst = {MOO.Dates.OraDate(sd, false)}");
            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObj(ds.Tables[0].Rows[0]);
            else
                return null;
            
        }




        /// <summary>
        /// Gets the values from metric value table filtered by range of shift dates
        /// </summary>
        /// <param name="metric_Id"></param>
        /// <param name="shiftDate"></param>
        /// <returns></returns>
        public static List<Metric_Value> GetByShiftDate(long metric_Id, DateTime startDate, DateTime endDate)
        {
            List<Metric_Value> retVal = [];
            StringBuilder sql = new();
            sql.AppendLine(GetSelect());
            sql.AppendLine($"WHERE metric_id = {metric_Id}");
            sql.AppendLine($"AND shift_date BETWEEN {MOO.Dates.OraDate(startDate, false)} AND {MOO.Dates.OraDate(endDate, false)}");
            sql.AppendLine($"ORDER BY start_date_no_dst");
            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                retVal.Add(DataRowToObj(row));
            }

            return retVal;
        }


        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}metric_id {ColPrefix}metric_id, {ta}start_date_no_dst {ColPrefix}start_date_no_dst, ");
            cols.AppendLine($"{ta}start_date {ColPrefix}start_date, {ta}shift {ColPrefix}shift, {ta}half {ColPrefix}half, ");
            cols.AppendLine($"{ta}hour {ColPrefix}hour, {ta}shift_date {ColPrefix}shift_date, ");
            cols.AppendLine($"{ta}inserted_date {ColPrefix}inserted_date, {ta}approval_id {ColPrefix}approval_id, ");
            cols.AppendLine($"{ta}value {ColPrefix}value, {ta}quality {ColPrefix}quality, {ta}update_date {ColPrefix}update_date, ");
            cols.AppendLine($"{ta}value_str {ColPrefix}value_str");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns("mv") + ",");
            sql.AppendLine(ApprovalSvc.GetColumns("apvl", "apvl_"));
            sql.AppendLine("FROM core.metric_value mv");
            sql.AppendLine("LEFT JOIN core.approval apvl");
            sql.AppendLine("ON mv.approval_id = apvl.approval_id");
            return sql.ToString();
        }



        #region "Delete Code"
        /// <summary>
        /// Deletes the metric value record and record it to the audit table
        /// </summary>
        /// <param name="mv"></param>
        /// <param name="changedBy"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int DeleteWithAudit(Metric_Value mv, string changedBy, OracleConnection conn)
        {
            //get the record currently in DB
            Metric_Value mvOld = Get(mv.Metric_Id, mv.Start_Date_No_DST);

            int recsAffected;
            recsAffected = Delete(mv, conn);
            if (recsAffected > 0)
            {
                //if recsaffected is zero, nothing was deleted so no need to audit
                Metric_Value_Audit mva = new()
                {
                    Metric_Id = mv.Metric_Id,
                    Start_Date_No_DST = mv.Start_Date_No_DST,
                    Changed_By = changedBy,
                    Date_Changed = DateTime.Now,
                    Old_Value = mvOld.Value

                };
                Metric_Value_AuditSvc.Insert(mva, conn);
                return recsAffected;
            }
            return recsAffected;
            
        }

        /// <summary>
        /// Deletes the metric value record and record it to the audit table
        /// </summary>
        /// <param name="mv"></param>
        /// <param name="changedBy"></param>
        /// <returns></returns>
        public static int DeleteWithAudit(Metric_Value mv, string changedBy)
        {

            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));            
            conn.Open();
            OracleTransaction trans = conn.BeginTransaction();
            int recsAffected;
            try
            {
                recsAffected = DeleteWithAudit(mv, changedBy, conn);
                if (recsAffected > 0)
                {
                    trans.Commit();
                    return recsAffected;
                }
                else
                {
                    trans.Rollback();  //no records affected, just run rollback
                    return 0;
                }
                    
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


        /// <summary>
        /// Deletes the specified metric_value record
        /// </summary>
        /// <param name="mv"></param>
        /// <param name="conn"></param>
        /// <returns>number of records deleted (should be 1 if successful)</returns>
        public static int Delete(Metric_Value mv)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                return Delete(mv, conn);
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

        /// <summary>
        /// Deletes the specified metric_value record
        /// </summary>
        /// <param name="mv"></param>
        /// <param name="conn"></param>
        /// <returns>number of records deleted (should be 1 if successful)</returns>
        public static int Delete(Metric_Value mv, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM core.metric_value");
            sql.AppendLine("WHERE metric_id = :metric_id");
            sql.AppendLine("    AND start_date_no_dst = :start_date_no_dst");

            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("metric_id", mv.Metric_Id);
            cmd.Parameters.Add("start_date_no_dst", mv.Start_Date_No_DST);

            return cmd.ExecuteNonQuery();
        }

        #endregion

        #region "Update Code"

        /// <summary>
        /// Updates the metric value record and record it to the audit table
        /// </summary>
        /// <param name="mv"></param>
        /// <param name="changedBy"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int UpdateWithAudit(Metric_Value mv, string changedBy, OracleConnection conn)
        {
            //get the record currently in DB
            Metric_Value mvOld = Get(mv.Metric_Id, mv.Start_Date_No_DST);

            int recsAffected;
            recsAffected = Update(mv, conn);
            if (recsAffected > 0 && mvOld.Value != mv.Value)
            {
                //if recsaffected is zero, nothing was deleted so no need to audit
                Metric_Value_Audit mva = new()
                {
                    Metric_Id = mv.Metric_Id,
                    Start_Date_No_DST = mv.Start_Date_No_DST,
                    Changed_By = changedBy,
                    Date_Changed = DateTime.Now,
                    Old_Value = mvOld.Value,
                    New_Value = mv.Value

                };
                Metric_Value_AuditSvc.Insert(mva, conn);
                return recsAffected;
            }
            return recsAffected;

        }

        /// <summary>
        /// Updates the metric value record and record it to the audit table
        /// </summary>
        /// <param name="mv"></param>
        /// <param name="changedBy"></param>
        /// <returns></returns>
        public static int UpdateWithAudit(Metric_Value mv, string changedBy)
        {

            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            OracleTransaction trans = conn.BeginTransaction();
            int recsAffected;
            try
            {
                recsAffected = UpdateWithAudit(mv, changedBy, conn);
                if (recsAffected > 0)
                {
                    trans.Commit();
                    return recsAffected;
                }
                else
                {
                    trans.Rollback();  //no records affected, just run rollback
                    return 0;
                }

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




        /// <summary>
        /// updates the metric value record (will not update start_date, shift, half, hour)
        /// </summary>
        /// <param name="mv"></param>
        /// <param name="conn"></param>
        /// <returns>number of records updated (should be 1 if successful)</returns>
        /// <remarks>We will not update the start_date, shift, half, hour, inserted_date
        /// these should not be updated. If the date is wrong, the record should be deleted and create a new.</remarks>
        public static int Update(Metric_Value mv)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                return Update(mv, conn);
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

        /// <summary>
        /// updates the metric value record (will not update start_date, shift, half, hour)
        /// </summary>
        /// <param name="mv"></param>
        /// <param name="conn"></param>
        /// <returns>number of records updated (should be 1 if successful)</returns>
        /// <remarks>We will not update the start_date, shift, half, hour, inserted_date
        /// these should not be updated. If the date is wrong, the record should be deleted and create a new.
        /// approval id will not be handled here either, use the approvalsvc instead</remarks>
        public static int Update(Metric_Value mv, OracleConnection conn)
        {
            StringBuilder sql = new();

            sql.AppendLine("UPDATE core.metric_value");
            sql.AppendLine("SET value = :value, quality = :quality,");
            sql.AppendLine("    update_date = :update_date,");
            sql.AppendLine("    value_str = :value_str");
            sql.AppendLine("WHERE metric_id = :metric_id");
            sql.AppendLine("    AND start_date_no_dst = :start_date_no_dst");

            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("value", mv.Value);
            cmd.Parameters.Add("quality", mv.Quality);
            cmd.Parameters.Add("update_date", mv.Update_Date);
            cmd.Parameters.Add("value_str", mv.Value_Str);
            cmd.Parameters.Add("metric_id", mv.Metric_Id);
            cmd.Parameters.Add("start_date_no_dst", mv.Start_Date_No_DST);

            return cmd.ExecuteNonQuery();
        }

        #endregion


        #region "Upsert Code"

        /// <summary>
        /// Updates/Inserts the metric value record and record it to the audit table
        /// </summary>
        /// <param name="Mv"></param>
        /// <param name="ChangedBy"></param>
        /// <param name="Conn"></param>
        /// <returns></returns>
        public static int UpsertWithAudit(Metric_Value Mv, string ChangedBy, OracleConnection Conn)
        {
            
            int recsAffected;
            recsAffected = UpdateWithAudit(Mv, ChangedBy, Conn);
            if (recsAffected == 0)
            {
                InsertWithAudit(Mv, ChangedBy, Conn);
                recsAffected = 1;
            }
            return recsAffected;

        }

        /// <summary>
        /// Updates/Inserts the metric value record and record it to the audit table
        /// </summary>
        /// <param name="mv"></param>
        /// <param name="changedBy"></param>
        /// <returns></returns>
        public static int UpsertWithAudit(Metric_Value mv, string changedBy)
        {

            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            OracleTransaction trans = conn.BeginTransaction();
            int recsAffected;
            try
            {
                recsAffected = UpsertWithAudit(mv, changedBy, conn);
                if (recsAffected == 0)
                {
                    trans.Commit();
                    return recsAffected;
                }
                else
                {
                    trans.Rollback();  //no records affected, just run rollback
                    return 0;
                }

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




        /// <summary>
        /// Updates/Inserts the metric value record (will not update start_date, shift, half, hour if update)
        /// </summary>
        /// <param name="mv"></param>
        /// <param name="conn"></param>
        /// <returns>number of records updated (should be 1 if successful)</returns>
        public static int Upsert(Metric_Value mv)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                return Upsert(mv, conn);
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

        /// <summary>
        ///  Updates/Inserts the metric value record (will not update start_date, shift, half, hour if update)
        /// </summary>
        /// <param name="mv"></param>
        /// <param name="conn"></param>
        /// <returns>number of records updated (should be 1 if successful)</returns>
        public static int Upsert(Metric_Value mv, OracleConnection conn)
        {
            int recsAffected = Update(mv, conn);
            if(recsAffected == 0)
            {
                Insert(mv, conn);
                recsAffected = 1;
            }
            return recsAffected;
        }

        #endregion



        /// <summary>
        /// Deletes the metric value record and record it to the audit table
        /// </summary>
        /// <param name="mv"></param>
        /// <param name="changedBy"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static void InsertWithAudit(Metric_Value mv, string changedBy, OracleConnection conn)
        {
                       
            Insert(mv, conn);
            
            //if recsaffected is zero, nothing was deleted so no need to audit
            Metric_Value_Audit mva = new()
            {
                Metric_Id = mv.Metric_Id,
                Start_Date_No_DST = mv.Start_Date_No_DST,
                Changed_By = changedBy,
                Date_Changed = DateTime.Now,
                New_Value = mv.Value

            };
            Metric_Value_AuditSvc.Insert(mva, conn);
            

        }

        /// <summary>
        /// Deletes the metric value record and record it to the audit table
        /// </summary>
        /// <param name="mv"></param>
        /// <param name="changedBy"></param>
        /// <returns></returns>
        public static void InsertWithAudit(Metric_Value mv, string changedBy)
        {

            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            OracleTransaction trans = conn.BeginTransaction();
            try
            {
                InsertWithAudit(mv, changedBy, conn);                
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

        /// <summary>
        /// Inserts the metric Value record into the database
        /// </summary>
        /// <param name="mv"></param>
        public static void Insert(Metric_Value mv)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                Insert(mv, conn);
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

        /// <summary>
        /// Inserts the metric Value record into the database
        /// </summary>
        /// <param name="mv"></param>
        /// <param name="conn"></param>
        public static void Insert(Metric_Value mv, OracleConnection conn)
        {

            //validate the dates before inserting
            if (mv.Start_Date < CHECK_DATE || mv.Start_Date_No_DST < CHECK_DATE)
                throw new Exception($"Invalid start date.  Cannot insert date before {CHECK_DATE}");
            //the difference between start_date and start_date_dst should never be greater than one hour
            if (Math.Round(Math.Abs(mv.Start_Date.Subtract(mv.Start_Date_No_DST).TotalSeconds),0) > 3600)
                throw new Exception("Invalid start_date_dst, Date should not be different than start date by more than one hour");

            StringBuilder sql = new();

            sql.AppendLine("INSERT INTO core.metric_value");
            sql.AppendLine("(metric_id, start_date_no_dst, start_date, shift, half,");
            sql.AppendLine("    hour, shift_date, inserted_date, value,");
            sql.AppendLine("    quality, update_date, value_str)");
            sql.AppendLine("VALUES(:metric_id, :start_date_no_dst, :start_date, :shift, :half,");
            sql.AppendLine("    :hour, :shift_date, :inserted_date, :value,");
            sql.AppendLine("    :quality, :update_date, :value_str)");

            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("metric_id", mv.Metric_Id);
            cmd.Parameters.Add("start_date_no_dst", mv.Start_Date_No_DST);
            cmd.Parameters.Add("start_date", mv.Start_Date);
            cmd.Parameters.Add("shift", mv.Shift);
            cmd.Parameters.Add("half", mv.Half);
            cmd.Parameters.Add("hour", mv.Hour);
            cmd.Parameters.Add("shift_date", mv.Shift_Date);
            cmd.Parameters.Add("inserted_date", mv.Inserted_Date);
            cmd.Parameters.Add("value", mv.Value);
            cmd.Parameters.Add("quality", mv.Quality);
            cmd.Parameters.Add("update_date", mv.Update_Date);
            cmd.Parameters.Add("value_str", mv.Value_Str);

            cmd.ExecuteNonQuery();
        }


        /// <summary>
        /// converts the datarow to an object
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private static Metric_Value DataRowToObj(DataRow row)
        {
            Metric_Value retObj = new();
            if (row.IsNull("apvl_approval_id"))
                retObj.Approval = null;
            else
                retObj.Approval = ApprovalSvc.DataRowToObject(row, "apvl_");

            retObj.Metric_Id = Convert.ToInt64(row["metric_id"]);
            retObj.Start_Date_No_DST = row.Field<DateTime>("start_date_no_dst");
            retObj.Start_Date = row.Field<DateTime>("start_date");
            retObj.Shift = row.IsNull("shift") ? null : Convert.ToByte(row["shift"]);
            retObj.Half = row.IsNull("half") ? null : Convert.ToByte(row["half"]);
            retObj.Hour = row.IsNull("hour") ? null : Convert.ToByte(row["hour"]);
            retObj.Shift_Date = row.Field<DateTime?>("shift_date");
            retObj.Inserted_Date = row.Field<DateTime?>("inserted_date");
            

            retObj.Value = row.Field<decimal?>("value");
            retObj.Quality = row.Field<decimal?>("quality");
            retObj.Update_Date = row.Field<DateTime?>("update_date");
            retObj.Value_Str = row.IsNull("value_str") ? "" : row["value_str"].ToString();


            return retObj;
        }
    }
}
