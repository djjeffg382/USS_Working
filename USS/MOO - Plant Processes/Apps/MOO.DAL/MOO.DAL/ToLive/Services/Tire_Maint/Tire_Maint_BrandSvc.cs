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
    public static class Tire_Maint_BrandSvc
    {
        static Tire_Maint_BrandSvc()
        {
            Util.RegisterOracle();
        }

        public static Tire_Maint_Brand Get(int tire_maint_brand_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE tire_maint_brand_id = :tire_maint_brand_id");


            Tire_Maint_Brand retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("tire_maint_brand_id", tire_maint_brand_id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }

        public static List<Tire_Maint_Brand> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            List<Tire_Maint_Brand> elements = [];
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
            cols.AppendLine($"{ta}tire_maint_brand_id {ColPrefix}tire_maint_brand_id, {ta}tire_brand {ColPrefix}tire_brand, ");
            cols.AppendLine($"{ta}tire_size {ColPrefix}tire_size");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.tire_maint_brand");
            return sql.ToString();
        }

        internal static Tire_Maint_Brand DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Tire_Maint_Brand RetVal = new();
            RetVal.Tire_Maint_Brand_Id = (long)Util.GetRowVal(row, $"{ColPrefix}tire_maint_brand_id");
            RetVal.Tire_Brand = (string)Util.GetRowVal(row, $"{ColPrefix}tire_brand");
            RetVal.Tire_Size = (string)Util.GetRowVal(row, $"{ColPrefix}tire_size");
            return RetVal;
        }
    }
}
