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
    public static class Property_Removal_VendorsSvc
    {
        public const string TABLE_NAME = "TOLIVE.Property_Removal_Vendors";
        static Property_Removal_VendorsSvc()
        {
            Util.RegisterOracle();
        }


        public static Property_Removal_Vendors Get(int companyId)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE company_id = :company_id");


            Property_Removal_Vendors retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("company_id", companyId);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }

        /// <summary>
        /// Retrieves the full list of vendors
        /// </summary>
        /// <param name="InculdeInactive">Whether to include inactive vendors</param>
        /// <returns></returns>
        public static List<Property_Removal_Vendors> GetAll(bool InculdeInactive = false)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            if (!InculdeInactive)
                sql.AppendLine("WHERE Active = 'True'");

            List<Property_Removal_Vendors> elements = [];
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
            cols.AppendLine($"{ta}company_name {ColPrefix}company_name, {ta}active {ColPrefix}active, ");
            cols.AppendLine($"{ta}company_id {ColPrefix}company_id");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.property_removal_vendors");
            return sql.ToString();
        }


        public static int Insert(Property_Removal_Vendors obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Property_Removal_Vendors obj, OracleConnection conn)
        {
            if (obj.Company_Id <= 0)
            {
                string sqlId = "SELECT NVL(MAX(company_id),0) FROM tolive.property_removal_vendors";
                int nextId = Convert.ToInt32((decimal)MOO.Data.ExecuteScalar(sqlId, Data.MNODatabase.DMART)) + 1;
                obj.Company_Id = nextId;
            }

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.property_removal_vendors(");
            sql.AppendLine("company_name, active, company_id)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":company_name, :active, :company_id)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("company_name", obj.Company_Name);
            ins.Parameters.Add("active", obj.Active.ToString());
            ins.Parameters.Add("company_id", obj.Company_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Property_Removal_Vendors obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Property_Removal_Vendors obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.property_removal_vendors SET");
            sql.AppendLine("active = :active, ");
            sql.AppendLine("company_name = :company_name");
            sql.AppendLine("WHERE company_id = :company_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("active", obj.Active.ToString());
            upd.Parameters.Add("company_name", obj.Company_Name);
            upd.Parameters.Add("company_id", obj.Company_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Property_Removal_Vendors obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Property_Removal_Vendors obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.property_removal_vendors");
            sql.AppendLine("WHERE company_id = :company_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("company_id", obj.Company_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Property_Removal_Vendors DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Property_Removal_Vendors RetVal = new();
            RetVal.Company_Name = (string)Util.GetRowVal(row, $"{ColPrefix}company_name");
            RetVal.Active = bool.Parse((string)Util.GetRowVal(row, $"{ColPrefix}active"));
            RetVal.Company_Id = Convert.ToInt32((decimal)Util.GetRowVal(row, $"{ColPrefix}company_id"));
            return RetVal;
        }

    }
}
