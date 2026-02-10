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
    public static class Sec_Request_ItemSvc
    {
        static Sec_Request_ItemSvc()
        {
            Util.RegisterOracle();
        }


        public static Sec_Request_Item Get(long sec_request_item_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE sec_request_item_id = :sec_request_item_id");


            Sec_Request_Item retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("sec_request_item_id", sec_request_item_id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }


        public static List<Sec_Request_Item> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            List<Sec_Request_Item> elements = new();
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
            cols.AppendLine($"{ta}sec_request_item_id {ColPrefix}sec_request_item_id, {ta}item_name {ColPrefix}item_name, ");
            cols.AppendLine($"{ta}additional_info_role {ColPrefix}additional_info_role, ");
            cols.AppendLine($"{ta}approver_role {ColPrefix}approver_role, {ta}action_role {ColPrefix}action_role, ");
            cols.AppendLine($"{ta}active {ColPrefix}active, {ta}request_comment_header {ColPrefix}request_comment_header");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns("sri_", "sri_") + ",");
            sql.AppendLine(Sec_RoleSvc.GetColumns("ad_", "ad_") + ",");
            sql.AppendLine(Sec_RoleSvc.GetColumns("app_", "app_") + ",");
            sql.AppendLine(Sec_RoleSvc.GetColumns("act_", "act_"));
            sql.AppendLine("FROM tolive.sec_request_item sri_");
            sql.AppendLine("LEFT JOIN tolive.sec_role ad_");
            sql.AppendLine("    ON sri_.additional_info_role = ad_.role_id");
            sql.AppendLine("LEFT JOIN tolive.sec_role app_");
            sql.AppendLine("    ON sri_.approver_role = app_.role_id");
            sql.AppendLine("JOIN tolive.sec_role act_");
            sql.AppendLine("    ON sri_.action_role = act_.role_id");
            return sql.ToString();
        }


        public static int Insert(Sec_Request_Item obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Sec_Request_Item obj, OracleConnection conn)
        {
            if (obj.Sec_Request_Item_Id <= 0)
            {
                string sqlId = "SELECT NVL(MAX(sec_request_item_id),0) FROM tolive.sec_request_item";
                int nextId = Convert.ToInt32((decimal)MOO.Data.ExecuteScalar(sqlId, Data.MNODatabase.DMART)) + 1;
                obj.Sec_Request_Item_Id = nextId;
            }

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Sec_Request_Item(");
            sql.AppendLine("sec_request_item_id, item_name, additional_info_role, approver_role, action_role, ");
            sql.AppendLine("active, request_comment_header)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":sec_request_item_id, :item_name, :additional_info_role, :approver_role, :action_role, ");
            sql.AppendLine(":active, :request_comment_header)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("sec_request_item_id", obj.Sec_Request_Item_Id);
            ins.Parameters.Add("item_name", obj.Item_Name);
            ins.Parameters.Add("additional_info_role", obj.Additional_Info_Role?.Role_Id);
            ins.Parameters.Add("approver_role", obj.Approver_Role?.Role_Id);
            ins.Parameters.Add("action_role", obj.Action_Role.Role_Id);
            ins.Parameters.Add("active", obj.Active ? 1 : 0);
            ins.Parameters.Add("request_comment_header", obj.Request_Comment_Header);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Sec_Request_Item obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Sec_Request_Item obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Sec_Request_Item SET");
            sql.AppendLine("item_name = :item_name, ");
            sql.AppendLine("additional_info_role = :additional_info_role, ");
            sql.AppendLine("approver_role = :approver_role, ");
            sql.AppendLine("action_role = :action_role, ");
            sql.AppendLine("active = :active, ");
            sql.AppendLine("request_comment_header = :request_comment_header");
            sql.AppendLine("WHERE sec_request_item_id = :sec_request_item_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("sec_request_item_id", obj.Sec_Request_Item_Id);
            upd.Parameters.Add("item_name", obj.Item_Name);
            upd.Parameters.Add("additional_info_role", obj.Additional_Info_Role?.Role_Id);
            upd.Parameters.Add("approver_role", obj.Approver_Role?.Role_Id);
            upd.Parameters.Add("action_role", obj.Action_Role.Role_Id);
            upd.Parameters.Add("active", obj.Active ? 1 : 0);
            upd.Parameters.Add("request_comment_header", obj.Request_Comment_Header);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }

        public static int Delete(Sec_Request_Item obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Sec_Request_Item obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.Sec_Request_Item");
            sql.AppendLine("WHERE sec_request_item_id = :sec_request_item_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("sec_request_item_id", obj.Sec_Request_Item_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Sec_Request_Item DataRowToObject(DbDataReader row, string ColPrefix = "sri_")
        {
            Sec_Request_Item RetVal = new();
            RetVal.Sec_Request_Item_Id = (long)(decimal)Util.GetRowVal(row, $"{ColPrefix}sec_request_item_id");
            RetVal.Item_Name = (string)Util.GetRowVal(row, $"{ColPrefix}item_name");

            if (row.IsDBNull(row.GetOrdinal($"{ColPrefix}additional_info_role")))
            {
                RetVal.Additional_Info_Role = null;
            }
            else
            {
                RetVal.Additional_Info_Role = Sec_RoleSvc.DataRowToObject(row,"ad_");
            }

            if (row.IsDBNull(row.GetOrdinal($"{ColPrefix}approver_role")))
            {
                RetVal.Approver_Role = null;
            }
            else
            {
                RetVal.Approver_Role = Sec_RoleSvc.DataRowToObject(row,"app_");
            }

            RetVal.Action_Role = Sec_RoleSvc.DataRowToObject(row,"act_");
            RetVal.Active = (decimal)Util.GetRowVal(row, $"{ColPrefix}active")==1;
            RetVal.Request_Comment_Header = (string)Util.GetRowVal(row, $"{ColPrefix}request_comment_header");
            return RetVal;
        }

    }
}
