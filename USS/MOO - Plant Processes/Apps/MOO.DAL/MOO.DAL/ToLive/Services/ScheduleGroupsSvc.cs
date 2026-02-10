using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;

namespace MOO.DAL.ToLive.Services
{
    public class ScheduleGroupsSvc
    {
        static ScheduleGroupsSvc()
        {
            Util.RegisterOracle();
        }


        public static ScheduleGroups Get(int group_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE group_id = {group_id}");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }

        public static List<ScheduleGroups> GetAll(string calendar_name = "")
        {
            List<ScheduleGroups> groupListing = new();
           StringBuilder sql = new();
            sql.Append(GetSelect());
            if(!string.IsNullOrEmpty(calendar_name))
                sql.AppendLine($"WHERE lower(calendar_name) = lower('{calendar_name}')");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count >= 1)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    ScheduleGroups group = new ScheduleGroups();
                    group = DataRowToObject(row);
                    groupListing.Add(group);
                }
                return groupListing;
            }
            else
                return null;
        }


        private static string GetSelect(string addField = "")
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine("group_id, group_name, calendar_name, color, active");
            if (!string.IsNullOrEmpty(addField))
            {
                sql.Append(", ");
                sql.AppendLine(addField);
            }
            sql.AppendLine("FROM tolive.sched_groups");
            return sql.ToString();
        }


        public static int Insert(ScheduleGroups obj)
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


        public static int Insert(ScheduleGroups obj, OracleConnection conn)
        {
            if (obj.Group_Id <= 0)
                obj.Group_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.seq_generic"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.sched_groups(");
            sql.AppendLine("group_id, group_name, calendar_name, color, active)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":group_id, :group_name, :calendar_name, :color, :active)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("group_id", obj.Group_Id);
            ins.Parameters.Add("group_name", obj.Group_Name);
            ins.Parameters.Add("calendar_name", obj.Calendar_Name);
            ins.Parameters.Add("color", obj.Color);
            ins.Parameters.Add("active", obj.Active ? 1 : 0);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(ScheduleGroups obj)
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


        public static int Update(ScheduleGroups obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.sched_groups SET");
            sql.AppendLine("group_name = :group_name, ");
            sql.AppendLine("calendar_name = :calendar_name, ");
            sql.AppendLine("color = :color, active = :active");
            sql.AppendLine("WHERE group_id = :group_id"); OracleCommand upd = new(sql.ToString(), conn);
            upd.Parameters.Add("group_name", obj.Group_Name);
            upd.Parameters.Add("calendar_name", obj.Calendar_Name);
            upd.Parameters.Add("color", obj.Color);
            upd.Parameters.Add("group_id", obj.Group_Id);
            upd.Parameters.Add("active", obj.Active ? 1 : 0);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(ScheduleGroups obj)
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


        public static int Delete(ScheduleGroups obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.sched_groups");
            sql.AppendLine("WHERE group_id = :group_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.Parameters.Add("group_id", obj.Group_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        private static ScheduleGroups DataRowToObject(DataRow row)
        {
            ScheduleGroups RetVal = new();
            RetVal.Group_Id = row.Field<decimal>("group_id");
            RetVal.Group_Name = row.Field<string>("group_name");
            RetVal.Calendar_Name = row.Field<string>("calendar_name");
            RetVal.Color = row.Field<string>("color");
            RetVal.Active = row.Field<short>("active") == 1;
            return RetVal;
        }

    }
}
