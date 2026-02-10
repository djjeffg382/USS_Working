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
    public static class FS_HotworksSvc
    {
        static FS_HotworksSvc()
        {
            Util.RegisterOracle();
        }


        public static FS_Hotworks Get(decimal hotworks_id, string permit_nbr = "")
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE hotworks_id = {hotworks_id}");
            if (!string.IsNullOrEmpty(permit_nbr))
                sql.AppendLine($"AND permit_nbr = {permit_nbr}");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hotworks_id"></param>
        /// <param name="onlyActive">Return only active permits</param>
        /// <returns></returns>
        public static List<FS_Hotworks> GetAll(bool onlyActive = true, MOO.Plant? plant = null)
        {
            StringBuilder sql = new();
            sql.AppendLine(GetSelect());
            if (onlyActive)
                sql.AppendLine($"WHERE date_close IS NULL");
            if (plant != null && onlyActive)
                sql.AppendLine($"AND Plant = '{plant.ToString()[..1]}'");
            else if (plant != null && onlyActive == false)
                sql.AppendLine($"WHERE Plant = '{plant.ToString()[..1]}'");

            DataSet ds = Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            List<FS_Hotworks> Elements = new();
            if (ds.Tables[0].Rows.Count >= 1)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Elements.Add(DataRowToObject(dr));
                }
            }
               
            return Elements;
        }


        private static string GetSelect(string addField = "")
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine("hotworks_id, permit_nbr, supervisor, employee_nbr, phone_nbr, location, date_open, ");
            sql.AppendLine("badge_nbr_open, date_close, badge_nbr_close, comments, plant, audited, audited_by");
            if (!string.IsNullOrEmpty(addField))
            {
                sql.Append(", ");
                sql.AppendLine(addField);
            }
            sql.AppendLine("FROM tolive.fs_hotworks");
            return sql.ToString();
        }


        public static int Insert(FS_Hotworks obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            return recsAffected;
        }


        public static int Insert(FS_Hotworks obj, OracleConnection conn)
        {
            if (obj.Hotworks_Id <= 0)
                obj.Hotworks_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.seq_firesec"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.fs_hotworks(");
            sql.AppendLine("hotworks_id, permit_nbr, supervisor, employee_nbr, phone_nbr, location, date_open, ");
            sql.AppendLine("badge_nbr_open, date_close, badge_nbr_close, comments, plant, audited, audited_by)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":hotworks_id, :permit_nbr, :supervisor, :employee_nbr, :phone_nbr, :location, ");
            sql.AppendLine(":date_open, :badge_nbr_open, :date_close, :badge_nbr_close, :comments, :plant, ");
            sql.AppendLine(":audited, :audited_by)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("hotworks_id", obj.Hotworks_Id);
            ins.Parameters.Add("permit_nbr", obj.Permit_Nbr);
            ins.Parameters.Add("supervisor", obj.Supervisor);
            ins.Parameters.Add("employee_nbr", obj.Employee_Nbr);
            ins.Parameters.Add("phone_nbr", obj.Phone_Nbr);
            ins.Parameters.Add("location", obj.Location);
            ins.Parameters.Add("date_open", obj.Date_Open);
            ins.Parameters.Add("badge_nbr_open", obj.Badge_Nbr_Open);
            ins.Parameters.Add("date_close", obj.Date_Close);
            ins.Parameters.Add("badge_nbr_close", obj.Badge_Nbr_Close);
            ins.Parameters.Add("comments", obj.Comments);
            ins.Parameters.Add("plant", obj.Plant.ToString()[..1].ToUpper());
            ins.Parameters.Add("audited", obj.Audited.HasValue ? (obj.Audited.GetValueOrDefault(true) ? 1 : 0) : 0);
            ins.Parameters.Add("audited_by", obj.Audited_By);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(FS_Hotworks obj)
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


        public static int Update(FS_Hotworks obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.fs_hotworks SET");
            sql.AppendLine("permit_nbr = :permit_nbr, ");
            sql.AppendLine("supervisor = :supervisor, ");
            sql.AppendLine("employee_nbr = :employee_nbr, ");
            sql.AppendLine("phone_nbr = :phone_nbr, ");
            sql.AppendLine("location = :location, ");
            sql.AppendLine("date_open = :date_open, ");
            sql.AppendLine("badge_nbr_open = :badge_nbr_open, ");
            sql.AppendLine("date_close = :date_close, ");
            sql.AppendLine("badge_nbr_close = :badge_nbr_close, ");
            sql.AppendLine("comments = :comments, ");
            sql.AppendLine("plant = :plant, ");
            sql.AppendLine("audited = :audited, ");
            sql.AppendLine("audited_by = :audited_by");
            sql.AppendLine("WHERE hotworks_id = :hotworks_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.Parameters.Add("permit_nbr", obj.Permit_Nbr);
            upd.Parameters.Add("supervisor", obj.Supervisor);
            upd.Parameters.Add("employee_nbr", obj.Employee_Nbr);
            upd.Parameters.Add("phone_nbr", obj.Phone_Nbr);
            upd.Parameters.Add("location", obj.Location);
            upd.Parameters.Add("date_open", obj.Date_Open);
            upd.Parameters.Add("badge_nbr_open", obj.Badge_Nbr_Open);
            upd.Parameters.Add("date_close", obj.Date_Close);
            upd.Parameters.Add("badge_nbr_close", obj.Badge_Nbr_Close);
            upd.Parameters.Add("comments", obj.Comments);
            upd.Parameters.Add("plant", obj.Plant.ToString()[..1].ToUpper());
            upd.Parameters.Add("audited", obj.Audited.HasValue ? (obj.Audited.GetValueOrDefault(true) ? 1 : 0) : 0);
            upd.Parameters.Add("audited_by", obj.Audited_By);
            upd.Parameters.Add("hotworks_id", obj.Hotworks_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(FS_Hotworks obj)
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


        public static int Delete(FS_Hotworks obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.fs_hotworks");
            sql.AppendLine("WHERE hotworks_id = :hotworks_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.Parameters.Add("hotworks_id", obj.Hotworks_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        private static FS_Hotworks DataRowToObject(DataRow row)
        {
            FS_Hotworks RetVal = new();
            RetVal.Hotworks_Id = row.Field<decimal>("hotworks_id");
            RetVal.Permit_Nbr = row.Field<string>("permit_nbr");
            RetVal.Supervisor = row.Field<string>("supervisor");
            RetVal.Employee_Nbr = row.Field<string>("employee_nbr");
            RetVal.Phone_Nbr = row.Field<string>("phone_nbr");
            RetVal.Location = row.Field<string>("location");
            RetVal.Date_Open = row.Field<DateTime>("date_open");
            RetVal.Badge_Nbr_Open = row.Field<string>("badge_nbr_open");
            RetVal.Date_Close = row.Field<DateTime?>("date_close");
            RetVal.Badge_Nbr_Close = row.Field<string>("badge_nbr_close");
            RetVal.Comments = row.Field<string>("comments");

            if(row["plant"].ToString().ToLower() == "m")
                RetVal.Plant = MOO.Plant.Minntac;
            else
                RetVal.Plant = MOO.Plant.Keetac;

            RetVal.Audited = Convert.ToBoolean(row.Field<decimal>("audited") == 1);
            RetVal.Audited_By = row.Field<string>("audited_by");
            return RetVal;

        }
    }
}
