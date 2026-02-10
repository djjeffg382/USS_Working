using MOO.DAL.Core.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Core.Services
{
    public static class Default_UsedSvc
    {
        static Default_UsedSvc()
        {
            Util.RegisterOracle();
        }


        public static Default_Used Get(decimal default_group_id, DateTime StartDateNoDST)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE default_group_id = :default_group_id");
            sql.AppendLine($"and start_date_no_dst = :start_date_no_dst");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("default_group_id", default_group_id);
            da.SelectCommand.Parameters.Add("start_date_no_dst", StartDateNoDST);
            da.SelectCommand.BindByName = true;

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }



        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}default_group_id {ColPrefix}default_group_id, ");
            cols.AppendLine($"{ta}start_date_no_dst {ColPrefix}start_date_no_dst, {ta}shift_date {ColPrefix}shift_date, ");
            cols.AppendLine($"{ta}shift {ColPrefix}shift, {ta}half {ColPrefix}half, {ta}hour {ColPrefix}hour");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM core.default_used");
            return sql.ToString();
        }


        public static int Insert(Default_Used obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Default_Used obj, OracleConnection conn)
        {
           

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO core.Default_Used(");
            sql.AppendLine("default_group_id, start_date_no_dst, shift_date, shift, half, hour)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":default_group_id, :start_date_no_dst, :shift_date, :shift, :half, :hour)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("default_group_id", obj.Default_Group_Id);
            ins.Parameters.Add("start_date_no_dst", obj.Start_Date_No_Dst);
            ins.Parameters.Add("shift_date", obj.Shift_Date);
            ins.Parameters.Add("shift", obj.Shift);
            ins.Parameters.Add("half", obj.Half);
            ins.Parameters.Add("hour", obj.Hour);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Delete(Default_Used obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Default_Used obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM core.Default_Used");
            sql.AppendLine("WHERE default_group_id = :default_group_id");
            sql.AppendLine("AND start_date_no_dst = :start_date_no_dst");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("default_group_id", obj.Default_Group_Id);
            del.Parameters.Add("start_date_no_dst", obj.Start_Date_No_Dst);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Default_Used DataRowToObject(DataRow row, string ColPrefix = "")
        {
            Default_Used RetVal = new();
            RetVal.Default_Group_Id = row.Field<decimal>($"{ColPrefix}default_group_id");
            RetVal.Start_Date_No_Dst = row.Field<DateTime>($"{ColPrefix}start_date_no_dst");
            RetVal.Shift_Date = row.Field<DateTime?>($"{ColPrefix}shift_date");
            RetVal.Shift = row.Field<decimal?>($"{ColPrefix}shift");
            RetVal.Half = row.Field<decimal?>($"{ColPrefix}half");
            RetVal.Hour = row.Field<decimal?>($"{ColPrefix}hour");
            return RetVal;
        }

    }
}
