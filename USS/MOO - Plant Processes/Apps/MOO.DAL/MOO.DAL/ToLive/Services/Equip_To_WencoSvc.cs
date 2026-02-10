using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;

namespace MOO.DAL.ToLive.Services
{
    public class Equip_To_WencoSvc
    {
        static Equip_To_WencoSvc()
        {
            Util.RegisterOracle();
        }


        public static Equip_To_Wenco Get(string plant, string WencoEquipIdent)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE plant = '{plant}'");
            sql.AppendLine($"AND Wenco_Equip_Ident = '{WencoEquipIdent}'");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }


        public static List<Equip_To_Wenco> GetAll(bool IncludeInactive = false)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            if (!IncludeInactive)
            {
                sql.AppendLine("WHERE active = 1");
            }

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            List<Equip_To_Wenco> retVal = new();
            foreach(DataRow dr in ds.Tables[0].Rows)
            {
                retVal.Add(DataRowToObject(dr));
            }
            return retVal;
        }


        private static string GetSelect(string addField = "")
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine("plant, wenco_equip_ident, foreign_id, previous_status, previous_status_date, ");
            sql.AppendLine("previous_position_date, previous_badge_date, active, system_name, last_sys_access_date,");
            sql.AppendLine("wenco_gen_production_code");
            if (!string.IsNullOrEmpty(addField))
            {
                sql.Append(", ");
                sql.AppendLine(addField);
            }
            sql.AppendLine("FROM tolive.equip_to_wenco");
            return sql.ToString();
        }


        public static int Insert(Equip_To_Wenco obj)
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


        public static int Insert(Equip_To_Wenco obj, OracleConnection conn)
        {
            

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Equip_To_Wenco(");
            sql.AppendLine("plant, wenco_equip_ident, foreign_id, previous_status, previous_status_date, ");
            sql.AppendLine("previous_position_date, previous_badge_date, active, system_name, wenco_gen_production_code)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":plant, :wenco_equip_ident, :foreign_id, :previous_status, :previous_status_date, ");
            sql.AppendLine(":previous_position_date, :previous_badge_date, :active, :system_name, :wenco_gen_production_code)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("plant", obj.Plant.ToString());
            ins.Parameters.Add("wenco_equip_ident", obj.Wenco_Equip_Ident);
            ins.Parameters.Add("foreign_id", obj.Foreign_Id);
            ins.Parameters.Add("previous_status", obj.Previous_Status);
            ins.Parameters.Add("previous_status_date", obj.Previous_Status_Date);
            ins.Parameters.Add("previous_position_date", obj.Previous_Position_Date);
            ins.Parameters.Add("previous_badge_date", obj.Previous_Badge_Date);
            ins.Parameters.Add("active", obj.Active ? 1 : 0);
            ins.Parameters.Add("system_name", obj.System_Name.ToString());
            ins.Parameters.Add("wenco_gen_production_code", obj.Wenco_Gen_Production_Code);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        /// <summary>
        /// Updates only the settings data and not the data used in the interface
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int UpdateSettingData(Equip_To_Wenco obj)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                int recsAffected = UpdateSettingData(obj, conn);
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

        /// <summary>
        /// Updates only the interfaceData
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int UpdateInterfaceData(Equip_To_Wenco obj)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                int recsAffected = UpdateInterfaceData(obj, conn);
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


        /// <summary>
        /// Updates only the settings data and not the data used in the interface
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int UpdateSettingData(Equip_To_Wenco obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Equip_To_Wenco SET");
            sql.AppendLine("foreign_id = :foreign_id, ");
            sql.AppendLine("system_name = :system_name, ");
            sql.AppendLine("active = :active,");
            sql.AppendLine("wenco_gen_production_code = :wenco_gen_production_code");
            sql.AppendLine("WHERE plant = :plant AND wenco_equip_ident = :wenco_equip_ident");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("foreign_id", obj.Foreign_Id);
            upd.Parameters.Add("system_name", obj.System_Name.ToString());
            upd.Parameters.Add("active", obj.Active ? 1:0);
            upd.Parameters.Add("wenco_gen_production_code", obj.Wenco_Gen_Production_Code);
            upd.Parameters.Add("plant", obj.Plant.ToString());
            upd.Parameters.Add("wenco_equip_ident", obj.Wenco_Equip_Ident);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        /// <summary>
        /// Updates only the interfaceData
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int UpdateInterfaceData(Equip_To_Wenco obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Equip_To_Wenco SET");
            sql.AppendLine("previous_status = :previous_status, ");
            sql.AppendLine("previous_status_date = :previous_status_date, ");
            sql.AppendLine("previous_position_date = :previous_position_date, ");
            sql.AppendLine("previous_badge_date = :previous_badge_date,");
            sql.AppendLine("last_sys_access_date = :last_sys_access_date");
            sql.AppendLine("WHERE plant = :plant AND wenco_equip_ident = :wenco_equip_ident");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("previous_status", obj.Previous_Status);
            upd.Parameters.Add("previous_status_date", obj.Previous_Status_Date);
            upd.Parameters.Add("previous_position_date", obj.Previous_Position_Date);
            upd.Parameters.Add("previous_badge_date", obj.Previous_Badge_Date);
            upd.Parameters.Add("last_sys_access_date", obj.Last_Sys_Access_Date);
            upd.Parameters.Add("plant", obj.Plant.ToString());
            upd.Parameters.Add("wenco_equip_ident", obj.Wenco_Equip_Ident);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Equip_To_Wenco obj)
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


        public static int Delete(Equip_To_Wenco obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.Equip_To_Wenco");
            sql.AppendLine("WHERE plant = :plant");
            sql.AppendLine("AND WENCO_EQUIP_IDENT = :WENCO_EQUIP_IDENT");
            OracleCommand del = new(sql.ToString(), conn);
            del.Parameters.Add("plant", obj.Plant.ToString());
            del.Parameters.Add("WENCO_EQUIP_IDENT", obj.Wenco_Equip_Ident.ToString());
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        private static Equip_To_Wenco DataRowToObject(DataRow row)
        {
            Equip_To_Wenco RetVal = new();
            RetVal.Plant = Enum.Parse<MOO.Plant>( row.Field<string>("plant"));
            RetVal.Wenco_Equip_Ident = row.Field<string>("wenco_equip_ident");
            RetVal.Foreign_Id = row.Field<string>("foreign_id");
            RetVal.Previous_Status = row.Field<string>("previous_status");
            RetVal.Previous_Status_Date = row.Field<DateTime>("previous_status_date");
            RetVal.Previous_Position_Date = row.Field<DateTime>("previous_position_date");
            RetVal.Previous_Badge_Date = row.Field<DateTime>("previous_badge_date");
            RetVal.System_Name = Enum.Parse<Equip_To_Wenco.SystemType>(row.Field<string>("system_name"));
            RetVal.Active = row.Field<short>("active") == 1;
            RetVal.Last_Sys_Access_Date = row.Field<DateTime>("Last_Sys_Access_Date");
            RetVal.Wenco_Gen_Production_Code = row.Field<string>("Wenco_Gen_Production_Code");
            return RetVal;
        }

    }
}
