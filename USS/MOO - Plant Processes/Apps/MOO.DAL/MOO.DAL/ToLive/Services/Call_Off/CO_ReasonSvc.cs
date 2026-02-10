using Oracle.ManagedDataAccess.Client;
using System.Data.Common;
using System.Text;
using MOO.DAL;
using MOO.DAL.ToLive.Models;
using System.Collections.Generic;
using System;
using System.Net;

namespace MOO.DAL.ToLive.Services
{
    public class CO_ReasonSvc
    {
        static CO_ReasonSvc()
        {
            Util.RegisterOracle();
        }


        public static CO_Reason Get(int reason_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE reason_id = :reason_id");


            CO_Reason retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("reason_id", reason_id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }


        public static List<CO_Reason> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            List<CO_Reason> elements = new();
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
            cols.AppendLine($"{ta}reason_id {ColPrefix}reason_id, {ta}reason_name {ColPrefix}reason_name, ");
            cols.AppendLine($"{ta}enabled {ColPrefix}enabled, {ta}req_comment {ColPrefix}req_comment, {ta}additional_info {ColPrefix}additional_info");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.co_reason");
            return sql.ToString();
        }


        public static int Insert(CO_Reason obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(CO_Reason obj, OracleConnection conn)
        {
            if (obj.Reason_Id <= 0)
                obj.Reason_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.SEQ_CALL_OFF"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.co_reason(");
            sql.AppendLine("reason_id, reason_name, enabled, req_comment, additional_info)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":reason_id, :reason_name, :enabled, :req_comment, :additional_info)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("reason_id", obj.Reason_Id);
            ins.Parameters.Add("reason_name", obj.Reason_Name);
            ins.Parameters.Add("enabled", obj.Enabled ? 1 : 0);
            ins.Parameters.Add("req_comment", obj.Req_Comment ? 1 : 0);
            ins.Parameters.Add("additional_info", obj.Additional_Info);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(CO_Reason obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(CO_Reason obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.co_reason SET");
            sql.AppendLine("reason_name = :reason_name, ");
            sql.AppendLine("enabled = :enabled, ");
            sql.AppendLine("req_comment = :req_comment,");
            sql.AppendLine("additional_info = :additional_info");
            sql.AppendLine("WHERE reason_id = :reason_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("reason_name", obj.Reason_Name);
            upd.Parameters.Add("enabled", obj.Enabled ? 1 : 0);
            upd.Parameters.Add("req_comment", obj.Req_Comment ? 1 : 0);
            upd.Parameters.Add("additional_info", obj.Additional_Info);
            upd.Parameters.Add("reason_id", obj.Reason_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(CO_Reason obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(CO_Reason obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.co_reason");
            sql.AppendLine("WHERE reason_id = :reason_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("reason_id", obj.Reason_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static CO_Reason DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            CO_Reason RetVal = new();
            RetVal.Reason_Id = Convert.ToInt32(GetRowVal(row, $"{ColPrefix}reason_id"));
            RetVal.Reason_Name = (string)GetRowVal(row, $"{ColPrefix}reason_name");
            RetVal.Enabled = (short)GetRowVal(row, $"{ColPrefix}enabled") == 1;
            RetVal.Req_Comment = (short)GetRowVal(row, $"{ColPrefix}req_comment") == 1;
            RetVal.Additional_Info = (string)GetRowVal(row, $"{ColPrefix}additional_info");
            return RetVal;
        }
        internal static object GetRowVal(DbDataReader rdr, string ColName)
        {
            int ordinal = rdr.GetOrdinal(ColName);
            if (rdr.IsDBNull(ordinal))
            {
                return null;
            }

            return rdr.GetValue(ordinal);
        }

    }
}
