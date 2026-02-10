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
    public static class Sec_Request_StatusSvc
    {
        static Sec_Request_StatusSvc()
        {
            Util.RegisterOracle();
        }


        public static Sec_Request_Status Get(long status_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE status_id = :status_id");


            Sec_Request_Status retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("status_id", status_id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }


        public static List<Sec_Request_Status> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            List<Sec_Request_Status> elements = new();
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
            cols.AppendLine($"{ta}status_id {ColPrefix}status_id, {ta}status_name {ColPrefix}status_name, ");
            cols.AppendLine($"{ta}code {ColPrefix}code");
            return cols.ToString();
        }

        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.sec_request_status");
            return sql.ToString();
        }


        public static int Insert(Sec_Request_Status obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Sec_Request_Status obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Sec_Request_Status(");
            sql.AppendLine("status_id, status_name, code)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":status_id, :status_name, :code)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("status_id", obj.Status_Id);
            ins.Parameters.Add("status_name", obj.Status_Name);
            ins.Parameters.Add("code", obj.Code);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Sec_Request_Status obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Sec_Request_Status obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Sec_Request_Status SET");
            sql.AppendLine("status_name = :status_name, ");
            sql.AppendLine("code = :code");
            sql.AppendLine("WHERE status_id = :status_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("status_name", obj.Status_Name);
            upd.Parameters.Add("code", obj.Code);
            upd.Parameters.Add("status_id", obj.Status_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Sec_Request_Status obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Sec_Request_Status obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.Sec_Request_Status");
            sql.AppendLine("WHERE status_id = :status_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("status_id", obj.Status_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Sec_Request_Status DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Sec_Request_Status RetVal = new();
            RetVal.Status_Id = (long)(decimal)Util.GetRowVal(row, $"{ColPrefix}status_id");
            RetVal.Status_Name = (string)Util.GetRowVal(row, $"{ColPrefix}status_name");
            RetVal.Code = (string)Util.GetRowVal(row, $"{ColPrefix}code");
            return RetVal;
        }
    }
}
