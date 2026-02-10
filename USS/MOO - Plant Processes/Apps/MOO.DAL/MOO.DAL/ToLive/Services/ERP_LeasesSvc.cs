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
    /// <summary>
    /// Service for Inserting and Deleting ERP_Leases records
    /// </summary>
    /// <remarks>There will be no update function, we will rely on doing a delete all for a month and then re-insert</remarks>
    public static class ERP_LeasesSvc
    {
        static ERP_LeasesSvc()
        {
            Util.RegisterOracle();
        }


        public static ERP_Leases Get(MOO.Plant Plant, DateTime MonthDate,  string Lease)
        {

            DateTime startOfMonth = MOO.Dates.FirstDayOfMonth(MonthDate);
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE site = :Plant");
            sql.AppendLine($"AND Month_Date = :MonthDate");
            sql.AppendLine($"AND Lease = :Lease");


            ERP_Leases retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("Plant", Plant == Plant.Minntac ? "mtc" : "ktc");
            cmd.Parameters.Add("MonthDate", startOfMonth);
            cmd.Parameters.Add("Lease", Lease);
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
        /// Gets all Leases for a given month
        /// </summary>
        /// <param name="Plant"></param>
        /// <param name="MonthDate">Any Date within the month</param>
        /// <returns></returns>
        public static List<ERP_Leases> GetAllByMonth(MOO.Plant Plant, DateTime MonthDate)
        {
            DateTime startOfMonth = MOO.Dates.FirstDayOfMonth(MonthDate);
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE site = :Plant");
            sql.AppendLine($"AND Month_Date = :MonthDate");

            List<ERP_Leases> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("Plant", Plant == Plant.Minntac ? "mtc" : "ktc");
            cmd.Parameters.Add("MonthDate", startOfMonth);
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
            cols.AppendLine($"{ta}lease {ColPrefix}lease, {ta}month_date {ColPrefix}month_date, {ta}weight {ColPrefix}weight, ");
            cols.AppendLine($"{ta}site {ColPrefix}site");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.erp_leases");
            return sql.ToString();
        }


        public static int Insert(ERP_Leases obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(ERP_Leases obj, OracleConnection conn)
        {
            
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.ERP_Leases(");
            sql.AppendLine("lease, month_date, weight, site)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":lease, :month_date, :weight, :site)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("lease", obj.Lease);
            ins.Parameters.Add("month_date", obj.Month_Date);
            ins.Parameters.Add("weight", obj.Weight);
            ins.Parameters.Add("site", obj.Site == Plant.Minntac ? "mtc" : "ktc");
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }

        /// <summary>
        /// Deletes all leases for specified plant and month
        /// </summary>
        /// <param name="Plant"></param>
        /// <param name="MonthDate">Any date within the month</param>
        /// <returns></returns>
        public static int DeleteMonth(MOO.Plant Plant, DateTime MonthDate)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(Plant, MonthDate, conn);
            conn.Close();
            return recsAffected;
        }

        /// <summary>
        /// Deletes all leases for specified plant and month
        /// </summary>
        /// <param name="Plant"></param>
        /// <param name="MonthDate">Any date within the month</param>
        /// <returns></returns>
        public static int Delete(MOO.Plant Plant, DateTime MonthDate, OracleConnection conn)
        {
            DateTime startOfMonth = MOO.Dates.FirstDayOfMonth(MonthDate);
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.ERP_Leases");
            sql.AppendLine("WHERE month_date = :month_date ");
            sql.AppendLine("AND site = :site");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("month_date", startOfMonth);
            del.Parameters.Add("site", Plant == Plant.Minntac ? "mtc" : "ktc");
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static ERP_Leases DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            ERP_Leases RetVal = new();
            RetVal.Lease = (string)Util.GetRowVal(row, $"{ColPrefix}lease");
            RetVal.Month_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}month_date");
            RetVal.Weight = (decimal?)Util.GetRowVal(row, $"{ColPrefix}weight");
            RetVal.Site = (string)Util.GetRowVal(row, $"{ColPrefix}site") == "mtc" ? Plant.Minntac : Plant.Keetac;
            return RetVal;
        }

    }
}
