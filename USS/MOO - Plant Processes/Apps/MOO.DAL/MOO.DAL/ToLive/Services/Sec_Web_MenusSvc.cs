using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Services
{
    public static class Sec_Web_MenusSvc
    {
        static Sec_Web_MenusSvc()
        {
            Util.RegisterOracle();
        }


        public static Sec_Web_Menus Get(int Web_Menu_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Web_Menu_Id = :Web_Menu_Id");


            Sec_Web_Menus retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("Web_Menu_Id", Web_Menu_Id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }


        public static List<Sec_Web_Menus> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            List<Sec_Web_Menus> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    elements.Add(DataRowToObject(rdr));
                }
            }
            conn.Close();
            return elements;
        }


        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}description {ColPrefix}description, {ta}modified_by {ColPrefix}modified_by, ");
            cols.AppendLine($"{ta}where_used {ColPrefix}where_used, {ta}web_menu_id {ColPrefix}web_menu_id");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.sec_web_menus");
            return sql.ToString();
        }


        public static int Insert(Sec_Web_Menus obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Sec_Web_Menus obj, OracleConnection conn)
        {
            if (obj.Web_Menu_Id <= 0)
                obj.Web_Menu_Id = Convert.ToInt32(MOO.Data.GetNextSequence("TOLIVE.Seq_Security"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO TOLIVE.SEC_WEB_MENUS(");
            sql.AppendLine("web_menu_id, description, modified_by, where_used)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":web_menu_id, :description, :modified_by, :where_used)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("web_menu_id", obj.Web_Menu_Id);
            ins.Parameters.Add("description", obj.Description);
            ins.Parameters.Add("modified_by", obj.Modified_By);
            ins.Parameters.Add("where_used", obj.Where_Used);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Sec_Web_Menus obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Sec_Web_Menus obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE TOLIVE.SEC_WEB_MENUS SET");
            sql.AppendLine("modified_by = :modified_by, ");
            sql.AppendLine("where_used = :where_used, ");
            sql.AppendLine("description = :description");
            sql.AppendLine("WHERE web_menu_id = :web_menu_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("modified_by", obj.Modified_By);
            upd.Parameters.Add("where_used", obj.Where_Used);
            upd.Parameters.Add("description", obj.Description);
            upd.Parameters.Add("web_menu_id", obj.Web_Menu_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Sec_Web_Menus obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Sec_Web_Menus obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM TOLIVE.SEC_WEB_MENUS");
            sql.AppendLine("WHERE web_menu_id = :web_menu_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("web_menu_id", obj.Web_Menu_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Sec_Web_Menus DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Sec_Web_Menus RetVal = new();
            RetVal.Description = (string)Util.GetRowVal(row, $"{ColPrefix}description");
            RetVal.Modified_By = (string)Util.GetRowVal(row, $"{ColPrefix}modified_by");
            RetVal.Where_Used = (string)Util.GetRowVal(row, $"{ColPrefix}where_used");
            RetVal.Web_Menu_Id = (int)(decimal)Util.GetRowVal(row, $"{ColPrefix}web_menu_id");
            return RetVal;
        }

    }
}
