using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Services
{
    public class TenantSvc
    {
        static TenantSvc()
        {
            Util.RegisterOracle();
        }


        public static Tenant Get(int tenant_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE tenant_id = :tenant_id");


            Tenant retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("tenant_id", tenant_id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }


        public static List<Tenant> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            List<Tenant> elements = [];
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
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
            cols.AppendLine($"{ta}tenant_id {ColPrefix}tenant_id, {ta}complaint_date {ColPrefix}complaint_date, ");
            cols.AppendLine($"{ta}plant {ColPrefix}plant, {ta}entered_by {ColPrefix}entered_by, ");
            cols.AppendLine($"{ta}tenant_name {ColPrefix}tenant_name, {ta}tenant_callback_nbr {ColPrefix}tenant_callback_nbr, ");
            cols.AppendLine($"{ta}tenant_address {ColPrefix}tenant_address, {ta}tenant_issue {ColPrefix}tenant_issue");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.tenant_form");
            return sql.ToString();
        }


        public static int Insert(Tenant obj)
        {
            using OracleConnection conn = new(Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Tenant obj, OracleConnection conn)
        {
            if (obj.Tenant_Id <= 0)
                obj.Tenant_Id = Convert.ToInt32(Data.GetNextSequence("tolive.seq_firesec"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.tenant_form(");
            sql.AppendLine("tenant_id, complaint_date, plant, entered_by, tenant_name, tenant_callback_nbr, ");
            sql.AppendLine("tenant_address, tenant_issue)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":tenant_id, :complaint_date, :plant, :entered_by, :tenant_name, :tenant_callback_nbr, ");
            sql.AppendLine(":tenant_address, :tenant_issue)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("tenant_id", obj.Tenant_Id);
            ins.Parameters.Add("complaint_date", obj.Complaint_Date);
            ins.Parameters.Add("plant", obj.Plant);
            ins.Parameters.Add("entered_by", obj.Entered_By);
            ins.Parameters.Add("tenant_name", obj.Tenant_Name);
            ins.Parameters.Add("tenant_callback_nbr", obj.Tenant_Callback_Nbr);
            ins.Parameters.Add("tenant_address", obj.Tenant_Address);
            ins.Parameters.Add("tenant_issue", obj.Tenant_Issue);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Tenant obj)
        {
            using OracleConnection conn = new(Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Tenant obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.tenant_form SET");
            sql.AppendLine("complaint_date = :complaint_date, ");
            sql.AppendLine("plant = :plant, ");
            sql.AppendLine("entered_by = :entered_by, ");
            sql.AppendLine("tenant_name = :tenant_name, ");
            sql.AppendLine("tenant_callback_nbr = :tenant_callback_nbr, ");
            sql.AppendLine("tenant_address = :tenant_address, ");
            sql.AppendLine("tenant_issue = :tenant_issue");
            sql.AppendLine("WHERE tenant_id = :tenant_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("complaint_date", obj.Complaint_Date);
            upd.Parameters.Add("plant", obj.Plant);
            upd.Parameters.Add("entered_by", obj.Entered_By);
            upd.Parameters.Add("tenant_name", obj.Tenant_Name);
            upd.Parameters.Add("tenant_callback_nbr", obj.Tenant_Callback_Nbr);
            upd.Parameters.Add("tenant_address", obj.Tenant_Address);
            upd.Parameters.Add("tenant_issue", obj.Tenant_Issue);
            upd.Parameters.Add("tenant_id", obj.Tenant_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Tenant obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Tenant obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.tenant_form");
            sql.AppendLine("WHERE tenant_id = :tenant_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("tenant_id", obj.Tenant_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Tenant DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Tenant RetVal = new();
            RetVal.Tenant_Id = (int)Util.GetRowVal(row, $"{ColPrefix}tenant_id");
            RetVal.Complaint_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}complaint_date");
            RetVal.Plant = (string)Util.GetRowVal(row, $"{ColPrefix}plant");
            RetVal.Entered_By = (string)Util.GetRowVal(row, $"{ColPrefix}entered_by");
            RetVal.Tenant_Name = (string)Util.GetRowVal(row, $"{ColPrefix}tenant_name");
            RetVal.Tenant_Callback_Nbr = (string)Util.GetRowVal(row, $"{ColPrefix}tenant_callback_nbr");
            RetVal.Tenant_Address = (string)Util.GetRowVal(row, $"{ColPrefix}tenant_address");
            RetVal.Tenant_Issue = (string)Util.GetRowVal(row, $"{ColPrefix}tenant_issue");
            return RetVal;
        }


    }
}
