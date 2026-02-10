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
    public static class Audit_TableSvc
    {
        static Audit_TableSvc()
        {
            Util.RegisterOracle();
        }


        //------REPLACE "PLACEHOLDER" WITH YOUR NEEDS------
        public static List<Audit_Table> GetByTable(string TableName, DateTime StartDate, DateTime EndDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE table_name = :TableName");
            sql.AppendLine("AND thedate BETWEEN :StartDate AND :EndDate");

            List<Audit_Table> elements = [];
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("TableName", TableName);
            cmd.Parameters.Add("StartDate", StartDate);
            cmd.Parameters.Add("EndDate", EndDate);
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
            cols.AppendLine($"{ta}thedate {ColPrefix}thedate, {ta}modified_by {ColPrefix}modified_by, ");
            cols.AppendLine($"{ta}table_name {ColPrefix}table_name, {ta}column_name {ColPrefix}column_name, ");
            cols.AppendLine($"{ta}key_value {ColPrefix}key_value, {ta}old_value {ColPrefix}old_value, ");
            cols.AppendLine($"{ta}new_value {ColPrefix}new_value");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.audit_table");
            return sql.ToString();
        }


        public static int Insert(Audit_Table obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Insert(Audit_Table obj, OracleConnection conn)
        {           
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Audit_Table(");
            sql.AppendLine("thedate, modified_by, table_name, column_name, key_value, old_value, new_value)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":thedate, :modified_by, :table_name, :column_name, :key_value, :old_value, :new_value)");
            OracleCommand ins = new(sql.ToString(), conn)
            {
                BindByName = true
            };
            ins.Parameters.Add("thedate", obj.Thedate);
            ins.Parameters.Add("modified_by", obj.Modified_By);
            ins.Parameters.Add("table_name", obj.Table_Name);
            ins.Parameters.Add("column_name", obj.Column_Name);
            ins.Parameters.Add("key_value", obj.Key_Value);
            ins.Parameters.Add("old_value", obj.Old_Value);
            ins.Parameters.Add("new_value", obj.New_Value);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }



        internal static Audit_Table DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Audit_Table RetVal = new();
            RetVal.Thedate = (DateTime)Util.GetRowVal(row, $"{ColPrefix}thedate");
            RetVal.Modified_By = (string)Util.GetRowVal(row, $"{ColPrefix}modified_by");
            RetVal.Table_Name = (string)Util.GetRowVal(row, $"{ColPrefix}table_name");
            RetVal.Column_Name = (string)Util.GetRowVal(row, $"{ColPrefix}column_name");
            RetVal.Key_Value = (string)Util.GetRowVal(row, $"{ColPrefix}key_value");
            RetVal.Old_Value = (string)Util.GetRowVal(row, $"{ColPrefix}old_value");
            RetVal.New_Value = (string)Util.GetRowVal(row, $"{ColPrefix}new_value");
            return RetVal;
        }

    }
}
