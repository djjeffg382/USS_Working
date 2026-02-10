using System;
using System.Data;
using System.Text;
using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MOO.DAL.ToLive.Services
{
    public class ScheduleAssignSvc
    {
        static ScheduleAssignSvc()
        {
            Util.RegisterOracle();
        }


        public static ScheduleAssign Get(decimal assign_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE sa.assign_id = {assign_id}");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }

        public static ScheduleAssign Get(decimal group_id, DateTime startDate, DateTime endDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE sa.group_id = {group_id}");
            sql.AppendLine($"AND sa.start_date = {MOO.Dates.OraDate(startDate)}");
            sql.AppendLine($"AND sa.end_date = {MOO.Dates.OraDate(endDate)}");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="GroupID">Pass if only a selected group wants to show.</param>
        /// <param name="sched_name">Set what report its for I.E. WeekEnd Duty/Mine Engineers</param>
        /// <param name="startDT">Get all records after start date</param>
        /// <param name="endDt">Get all records before end date</param>
        /// <returns></returns>
        public static List<ScheduleAssign> GetAll(int GroupID = 0, string sched_name = "", DateTime? startDT = null, DateTime? endDt = null)
        {
            List<ScheduleAssign> list = new();
            StringBuilder sql = new();
            sql.Append(GetSelect());

            if(GroupID > 0)
                sql.AppendLine($"WHERE group_id = {GroupID}");
            if (sched_name != "")
                if (GroupID != 0)
                    sql.AppendLine($"AND sched_name = '{sched_name}'");
                else
                    sql.AppendLine($"WHERE sched_name = '{sched_name}'");

            //Scheduler has OnLoadData, ultimately just load what is currently available and NOT everything.
            if(startDT != null && endDt != null)
            {
                if(GroupID == 0 && sched_name == "")
                    sql.AppendLine($"WHERE start_date Between {MOO.Dates.OraDate(startDT ?? DateTime.Now)} AND {MOO.Dates.OraDate(endDt ?? DateTime.Now.AddDays(3))}");
                else
                    sql.AppendLine($"AND start_date Between {MOO.Dates.OraDate(startDT ?? DateTime.Now)} AND {MOO.Dates.OraDate(endDt ?? DateTime.Now.AddDays(3))}");
            }
                    

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count >= 1)
            {
                foreach(DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(ScheduleAssignSvc.DataRowToObject(row));
                }
                
            }
            else
                return null;
            return list;
        }


        private static string GetSelect(string addField = "")
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine("assign_id, short_view, group_id, sched_name, start_date, end_date, long_view, people_assign");
            if (!string.IsNullOrEmpty(addField))
            {
                sql.Append(", ");
                sql.AppendLine(addField);
            }
            sql.AppendLine("FROM tolive.sched_assign sa");
            return sql.ToString();
        }


        public static int Insert(ScheduleAssign obj)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                int recsAffected = Insert(obj, conn);
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


        public static int Insert(ScheduleAssign obj, OracleConnection conn)
        {
            if (obj.Assign_Id <= 0)
                obj.Assign_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.seq_generic"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.sched_assign(");
            sql.AppendLine("assign_id, short_view, group_id, sched_name, start_date, end_date, long_view, people_assign)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":assign_id, :short_view, :group_id, :sched_name, :start_date, :end_date, :long_view, :people_assign)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("assign_id", obj.Assign_Id);
            ins.Parameters.Add("short_view", obj.Short_View);
            ins.Parameters.Add("group_id", obj.Group_Id);
            ins.Parameters.Add("sched_name", obj.Sched_Name);
            ins.Parameters.Add("start_date", obj.Start_Date);
            ins.Parameters.Add("end_date", obj.End_Date);
            ins.Parameters.Add("long_view", obj.Long_View);
            ins.Parameters.Add("people_assign", string.Join(" ;", obj.People_Assign));
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(ScheduleAssign obj)
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


        public static int Update(ScheduleAssign obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.sched_assign SET");
            sql.AppendLine("short_view = :short_view, ");
            sql.AppendLine("group_id = :group_id, ");
            sql.AppendLine("sched_name = :sched_name, ");
            sql.AppendLine("start_date = :start_date, ");
            sql.AppendLine("end_date = :end_date, ");
            sql.AppendLine("long_view = :long_view, ");
            sql.AppendLine("people_assign = :people_assign");
            sql.AppendLine("WHERE assign_id = :assign_id"); 
            OracleCommand upd = new(sql.ToString(), conn);
            upd.Parameters.Add("short_view", obj.Short_View);
            upd.Parameters.Add("group_id", obj.Group_Id);
            upd.Parameters.Add("sched_name", obj.Sched_Name);
            upd.Parameters.Add("start_date", obj.Start_Date);
            upd.Parameters.Add("end_date", obj.End_Date);
            upd.Parameters.Add("long_view", obj.Long_View);
            upd.Parameters.Add("people_assign", obj.People_Assign);
            upd.Parameters.Add("assign_id", obj.Assign_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(ScheduleAssign obj)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                int recsAffected = Delete(obj, conn);
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


        public static int Delete(ScheduleAssign obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.sched_assign");
            sql.AppendLine("WHERE assign_id = :assign_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.Parameters.Add("assign_id", obj.Assign_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        private static ScheduleAssign DataRowToObject(DataRow row)
        {
            ScheduleAssign RetVal = new();
            RetVal.Assign_Id = row.Field<decimal>("assign_id");
            RetVal.Short_View = row.Field<string>("short_view");
            RetVal.Group_Id = row.Field<decimal>("group_id");
            RetVal.Sched_Name = row.Field<string>("sched_name");
            RetVal.Start_Date = row.Field<DateTime>("start_date");
            RetVal.End_Date = row.Field<DateTime>("end_date");
            RetVal.Long_View = row.Field<string>("long_view");
            RetVal.People_Assign = row.Field<string>("people_assign");
            return RetVal;
        }

    }
}
