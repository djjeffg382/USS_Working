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
    public static class Auto_Rpt_Run_Item_ParamSvc
    {
        static Auto_Rpt_Run_Item_ParamSvc()
        {
            Util.RegisterOracle();
        }


        public static Auto_Rpt_Run_Item_Param Get(int param_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE param_id = :param_id");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("param_id", param_id);
            da.SelectCommand.BindByName = true;

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }

        public static List<Auto_Rpt_Run_Item_Param> GetAll(int ReportItemId)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Item_id = :Item_id");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("Item_id", ReportItemId);
            da.SelectCommand.BindByName = true;

            DataSet ds = MOO.Data.ExecuteQuery(da);
            List<Auto_Rpt_Run_Item_Param> elements = new();

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
            cols.AppendLine($"{ta}param_id {ColPrefix}param_id, {ta}param_name {ColPrefix}param_name, ");
            cols.AppendLine($"{ta}param_value {ColPrefix}param_value, {ta}item_id {ColPrefix}item_id, ");
            cols.AppendLine($"{ta}dateformat {ColPrefix}dateformat");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.auto_rpt_run_item_param");
            return sql.ToString();
        }


        public static int Insert(Auto_Rpt_Run_Item_Param obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Auto_Rpt_Run_Item_Param obj, OracleConnection conn)
        {
            //new id is the max id + 1 we don't have a sequence set up for this
            if(obj.Param_Id <= 0)
                obj.Param_Id = (long)MOO.Data.ExecuteScalar("SELECT CAST(NVL(MAX(param_id),0) as NUMBER(10,0)) FROM tolive.AUTO_RPT_Run_Item_Param", Data.MNODatabase.DMART, 0) + 1;

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.AUTO_RPT_Run_Item_Param(");
            sql.AppendLine("param_id, param_name, param_value, item_id, dateformat)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":param_id, :param_name, :param_value, :item_id, :dateformat)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("param_id", obj.Param_Id);
            ins.Parameters.Add("param_name", obj.Param_Name);
            ins.Parameters.Add("param_value", obj.Param_Value);
            ins.Parameters.Add("item_id", obj.Item_Id);
            ins.Parameters.Add("dateformat", obj.Dateformat);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Auto_Rpt_Run_Item_Param obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Auto_Rpt_Run_Item_Param obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.AUTO_RPT_Run_Item_Param SET");
            sql.AppendLine("param_name = :param_name, ");
            sql.AppendLine("param_value = :param_value, ");
            sql.AppendLine("item_id = :item_id, ");
            sql.AppendLine("dateformat = :dateformat");
            sql.AppendLine("WHERE param_id = :param_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("param_name", obj.Param_Name);
            upd.Parameters.Add("param_value", obj.Param_Value);
            upd.Parameters.Add("item_id", obj.Item_Id);
            upd.Parameters.Add("dateformat", obj.Dateformat);
            upd.Parameters.Add("param_id", obj.Param_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Auto_Rpt_Run_Item_Param obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Auto_Rpt_Run_Item_Param obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.AUTO_RPT_Run_Item_Param");
            sql.AppendLine("WHERE param_id = :param_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("param_id", obj.Param_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Auto_Rpt_Run_Item_Param DataRowToObject(DataRow row, string ColPrefix = "")
        {
            Auto_Rpt_Run_Item_Param RetVal = new();
            RetVal.Param_Id = row.Field<long>($"{ColPrefix}param_id");
            RetVal.Param_Name = row.Field<string>($"{ColPrefix}param_name");
            RetVal.Param_Value = row.Field<string>($"{ColPrefix}param_value");
            RetVal.Item_Id = row.Field<long>($"{ColPrefix}item_id");
            RetVal.Dateformat = row.Field<string>($"{ColPrefix}dateformat");
            return RetVal;
        }

    }
}
