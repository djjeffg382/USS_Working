using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Services
{
    public static class Auto_Rpt_ItemSvc
    {
        static Auto_Rpt_ItemSvc()
        {
            Util.RegisterOracle();
        }


        public static Auto_Rpt_Item Get(int item_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE item_id = :item_id");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("item_id", item_id);
            da.SelectCommand.BindByName = true;

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }

        public static List<Auto_Rpt_Item> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.BindByName = true;

            DataSet ds = MOO.Data.ExecuteQuery(da);
            List<Auto_Rpt_Item> elements = new();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                elements.Add(DataRowToObject(dr));
            }
            return elements;
        }

        /// <summary>
        /// Gets the list of reports the user is subscribed to
        /// </summary>
        /// <param name="ADUser">the user account (example: mno\abc1234)</param>
        /// <returns></returns>
        public static List<Auto_Rpt_Item> GetSubscribed(string ADUser)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("JOIN tolive.auto_rpt_rcpt arr ON ari.item_id = arr.item_id");
            sql.AppendLine("WHERE lower(arr.recipient) = lower(:ADUser)");
            sql.AppendLine("AND arr.is_printer = 0");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.BindByName = true;
            da.SelectCommand.Parameters.Add("ADUser", ADUser);

            DataSet ds = MOO.Data.ExecuteQuery(da);
            List<Auto_Rpt_Item> elements = new();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                elements.Add(DataRowToObject(dr));
            }
            return elements;
        }


        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}item_id {ColPrefix}item_id, {ta}item_name {ColPrefix}item_name, ");
            cols.AppendLine($"{ta}item_description {ColPrefix}item_description, {ta}item_value {ColPrefix}item_value, ");
            cols.AppendLine($"{ta}type_id {ColPrefix}type_id, {ta}copy_to_exec_folder {ColPrefix}copy_to_exec_folder");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns("ari") + ",");
            sql.AppendLine(Auto_Rpt_TypeSvc.GetColumns("rt", "rt_"));
            sql.AppendLine("FROM tolive.auto_rpt_item ari");
            sql.AppendLine("JOIN tolive.auto_rpt_type rt ON ari.type_id = rt.type_id");
            return sql.ToString();
        }


        public static int Insert(Auto_Rpt_Item obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Auto_Rpt_Item obj, OracleConnection conn)
        {
            //new id is the max id + 1 we don't have a sequence set up for this
            obj.Item_Id = (long)MOO.Data.ExecuteScalar("SELECT CAST(NVL(MAX(item_id),0) as NUMBER(10,0)) FROM tolive.auto_rpt_item", Data.MNODatabase.DMART, 0) + 1;

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.AUTO_RPT_ITEM(");
            sql.AppendLine("item_id, item_name, item_description, item_value, type_id, copy_to_exec_folder)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":item_id, :item_name, :item_description, :item_value, :type_id, :copy_to_exec_folder)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("item_id", obj.Item_Id);
            ins.Parameters.Add("item_name", obj.Item_Name);
            ins.Parameters.Add("item_description", obj.Item_Description);
            ins.Parameters.Add("item_value", obj.Item_Value);
            ins.Parameters.Add("type_id", obj.Report_Type.Type_Id);
            ins.Parameters.Add("copy_to_exec_folder", obj.Copy_To_Exec_Folder ? 1 : 0);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Auto_Rpt_Item obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Auto_Rpt_Item obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.AUTO_RPT_ITEM SET");
            sql.AppendLine("item_name = :item_name, ");
            sql.AppendLine("item_description = :item_description, ");
            sql.AppendLine("item_value = :item_value, ");
            sql.AppendLine("type_id = :type_id, ");
            sql.AppendLine("copy_to_exec_folder = :copy_to_exec_folder");
            sql.AppendLine("WHERE item_id = :item_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("item_name", obj.Item_Name);
            upd.Parameters.Add("item_description", obj.Item_Description);
            upd.Parameters.Add("item_value", obj.Item_Value);
            upd.Parameters.Add("type_id", obj.Report_Type.Type_Id);
            upd.Parameters.Add("copy_to_exec_folder", obj.Copy_To_Exec_Folder ? 1 : 0);
            upd.Parameters.Add("item_id", obj.Item_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Auto_Rpt_Item obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Auto_Rpt_Item obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.AUTO_RPT_ITEM");
            sql.AppendLine("WHERE item_id = :item_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("item_id", obj.Item_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Auto_Rpt_Item DataRowToObject(DataRow row, string ColPrefix = "")
        {
            Auto_Rpt_Item RetVal = new();
            RetVal.Item_Id = row.Field<long>($"{ColPrefix}item_id");
            RetVal.Item_Name = row.Field<string>($"{ColPrefix}item_name");
            RetVal.Item_Description = row.Field<string>($"{ColPrefix}item_description");
            RetVal.Item_Value = row.Field<string>($"{ColPrefix}item_value");
            RetVal.Report_Type = Auto_Rpt_TypeSvc.DataRowToObject(row, "rt_");
            RetVal.Copy_To_Exec_Folder = row.Field<short>($"{ColPrefix}copy_to_exec_folder") == 1;
            
            return RetVal;
        }

    }
}
