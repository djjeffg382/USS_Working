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
    public class FS_ConfSpaceSvc
    {
        static FS_ConfSpaceSvc()
        {
            Util.RegisterOracle();
        }


        public static FS_ConfSpace Get(decimal confinedspace_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE confinedspace_id = :confinedspace_id");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("confinedspace_id", confinedspace_id);

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }


        //------REPLACE "PLACEHOLDER" WITH YOUR NEEDS------
        /// <summary>
        /// Get All results for FireSecurity Dashboards
        /// </summary>
        /// <param name="onlyActive">Show only active results. If false it will return all records</param>
        /// <returns></returns>
        public static List<FS_ConfSpace> GetAll(bool onlyActive = true, MOO.Plant? plant = null)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            if (onlyActive)
                sql.AppendLine($"WHERE CLEAR_TIME IS NULL");
            if (plant != null && onlyActive)
                sql.AppendLine($"AND Plant = '{plant.ToString().Substring(0, 1)}'");
            else if (plant != null && onlyActive == false)
                sql.AppendLine($"WHERE Plant = '{plant.ToString().Substring(0, 1)}'");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));

            DataSet ds = MOO.Data.ExecuteQuery(da);
            List<FS_ConfSpace> elements = new();
            if (ds.Tables[0].Rows.Count >= 1)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    elements.Add(DataRowToObject(dr));
                }
            }
            return elements;
        }


        private static string GetSelect(string addField = "")
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine("confinedspace_id, date_open, created_date, call_back_nbr, location, clear_time, ");
            sql.AppendLine("supervisor, plant, audited, audited_by, permitted");
            if (!string.IsNullOrEmpty(addField))
            {
                sql.Append(", ");
                sql.AppendLine(addField);
            }
            sql.AppendLine("FROM tolive.fs_confspace");
            return sql.ToString();
        }


        public static int Insert(FS_ConfSpace obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(FS_ConfSpace obj, OracleConnection conn)
        {
            if (obj.Confinedspace_Id <= 0)
                obj.Confinedspace_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.seq_firesec"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.fs_confspace(");
            sql.AppendLine("confinedspace_id, date_open, created_date, call_back_nbr, location, clear_time, ");
            sql.AppendLine("supervisor, plant, audited, audited_by, permitted)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":confinedspace_id, :date_open, :created_date, :call_back_nbr, :location, :clear_time, ");
            sql.AppendLine(":supervisor, :plant, :audited, :audited_by, :permitted)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("confinedspace_id", obj.Confinedspace_Id);
            ins.Parameters.Add("date_open", obj.Date_Open);
            ins.Parameters.Add("created_date", obj.Created_Date);
            ins.Parameters.Add("call_back_nbr", obj.Call_Back_Nbr);
            ins.Parameters.Add("location", obj.Location);
            ins.Parameters.Add("clear_time", obj.Clear_Time);
            ins.Parameters.Add("supervisor", obj.Supervisor);
            ins.Parameters.Add("plant", obj.Plant.ToString()[..1]);
            ins.Parameters.Add("audited", obj.Audited.HasValue ? (obj.Audited.GetValueOrDefault(true) ? 1 : 0) : 0);
            ins.Parameters.Add("audited_by", obj.Audited_By);
            ins.Parameters.Add("permitted", obj.Permitted.HasValue ? (obj.Permitted.GetValueOrDefault(true) ? 1 : 0) : 0);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(FS_ConfSpace obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(FS_ConfSpace obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.fs_confspace SET");
            sql.AppendLine("date_open = :date_open, ");
            sql.AppendLine("created_date = :created_date, ");
            sql.AppendLine("call_back_nbr = :call_back_nbr, ");
            sql.AppendLine("location = :location, ");
            sql.AppendLine("clear_time = :clear_time, ");
            sql.AppendLine("supervisor = :supervisor, ");
            sql.AppendLine("plant = :plant, ");
            sql.AppendLine("audited = :audited, ");
            sql.AppendLine("audited_by = :audited_by, ");
            sql.AppendLine("permitted = :permitted");
            sql.AppendLine("WHERE confinedspace_id = :confinedspace_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.Parameters.Add("date_open", obj.Date_Open);
            upd.Parameters.Add("created_date", obj.Created_Date);
            upd.Parameters.Add("call_back_nbr", obj.Call_Back_Nbr);
            upd.Parameters.Add("location", obj.Location);
            upd.Parameters.Add("clear_time", obj.Clear_Time);
            upd.Parameters.Add("supervisor", obj.Supervisor);
            upd.Parameters.Add("plant", obj.Plant.ToString()[..1]);
            upd.Parameters.Add("audited", obj.Audited.HasValue ? (obj.Audited.GetValueOrDefault(true) ? 1 : 0) : 0);
            upd.Parameters.Add("audited_by", obj.Audited_By);
            upd.Parameters.Add("permitted", obj.Permitted.HasValue ? (obj.Permitted.GetValueOrDefault(true) ? 1 : 0) : 0);
            upd.Parameters.Add("confinedspace_id", obj.Confinedspace_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(FS_ConfSpace obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(FS_ConfSpace obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.fs_confspace");
            sql.AppendLine("WHERE confinedspace_id = :confinedspace_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.Parameters.Add("confinedspace_id", obj.Confinedspace_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        private static FS_ConfSpace DataRowToObject(DataRow row)
        {
            FS_ConfSpace RetVal = new();
            RetVal.Confinedspace_Id = row.Field<decimal>("confinedspace_id");
            RetVal.Date_Open = row.Field<DateTime>("date_open");
            RetVal.Created_Date = row.Field<DateTime>("created_date");
            RetVal.Call_Back_Nbr = row.Field<string>("call_back_nbr");
            RetVal.Location = row.Field<string>("location");
            RetVal.Clear_Time = row.Field<DateTime?>("clear_time");
            RetVal.Supervisor = row.Field<string>("supervisor");
            if (row["plant"].ToString().ToLower() == "m")
            {
                RetVal.Plant = MOO.Plant.Minntac;
            }
            else
            {
                RetVal.Plant = MOO.Plant.Keetac;
            }
            RetVal.Audited = Convert.ToBoolean(row.Field<decimal>("Audited") == 1);
            RetVal.Audited_By = row.Field<string>("audited_by");
            RetVal.Permitted = Convert.ToBoolean(row.Field<decimal>("Permitted") == 1);

            return RetVal;
        }

    }
}
