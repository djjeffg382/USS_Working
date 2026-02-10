using Microsoft.Data.SqlClient;
using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Text;
using MOO.Enums.Extension;


namespace MOO.DAL.ToLive.Services
{

    /// <summary>
    /// Conc Analytics Uplift data obtained from Data Science group
    /// </summary>
    /// <remarks>No Insert, Update, Delete needed.  Data science will be modifying this table.  Only reads needed</remarks>
    public static class Conc_Analytics_UpliftSvc
    {
        static Conc_Analytics_UpliftSvc()
        {
            Util.RegisterOracle();
        }

        public static List<Conc_Analytics_Uplift> GetLatest()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT * FROM (");
            sql.Append(GetSelect());
            sql.AppendLine(")");
            sql.AppendLine("WHERE rn = 1");

            List<Conc_Analytics_Uplift> elements = [];
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

        /// <summary>
        /// Gets a list of the uplift by date
        /// </summary>
        /// <param name="StartDate">Start Date of range</param>
        /// <param name="EndDate">End date of range</param>
        /// <param name="Level">Level (Plant or Line) exclude for all</param>
        /// <param name="Frequency">Calculation frequency (exclude for all)</param>
        /// <returns></returns>
        public static List<Conc_Analytics_Uplift> GetAll(DateTime StartDate, DateTime EndDate, 
                                                        Conc_Analytics_Uplift.Level? Level = null, Conc_Analytics_Uplift.UpliftFrequency? Frequency = null )
        {
            StringBuilder sql = new();
            sql.AppendLine(GetSelect());
            sql.AppendLine("WHERE TheDate BETWEEN :StartDate AND :EndDate");
            if(Level.HasValue)
                sql.AppendLine("AND PlantLevel = :Lvl");
            if(Frequency.HasValue)
                sql.AppendLine("AND frequency = :Frequency");

            List<Conc_Analytics_Uplift> elements = [];
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("StartDate", StartDate);
            cmd.Parameters.Add("EndDate", EndDate);
            if (Level.HasValue)
                cmd.Parameters.Add("Lvl", Level.ToString());
            if (Frequency.HasValue)
                cmd.Parameters.Add("Frequency", Frequency.GetDisplay().Prompt);



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
            cols.AppendLine($"{ta}thedate {ColPrefix}thedate, {ta}plantlevel {ColPrefix}plantlevel, {ta}line {ColPrefix}line, ");
            cols.AppendLine($"{ta}uplift_long_tons {ColPrefix}uplift_long_tons, {ta}uplift_pct {ColPrefix}uplift_pct, ");
            cols.AppendLine($"{ta}frequency {ColPrefix}frequency");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns() + ",");
            sql.AppendLine("ROW_NUMBER() OVER(PARTITION BY PlantLevel, Line, frequency ORDER BY thedate desc ) rn");  //this will be used to get latest for each Plant,Line
            sql.AppendLine("FROM tolive.conc_analytics_uplift");
            return sql.ToString();
        }

        


        internal static Conc_Analytics_Uplift DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Conc_Analytics_Uplift RetVal = new();
            RetVal.TheDate = (DateTime)Util.GetRowVal(row, $"{ColPrefix}thedate");
            RetVal.PlantLevel = Enum.Parse<Conc_Analytics_Uplift.Level>( (string)Util.GetRowVal(row, $"{ColPrefix}plantlevel"));
            RetVal.Line = Convert.ToByte( (decimal)Util.GetRowVal(row, $"{ColPrefix}line"));
            RetVal.Uplift_Long_Tons = (double)Util.GetRowVal(row, $"{ColPrefix}uplift_long_tons");
            RetVal.Uplift_Pct = Math.Round((float)Util.GetRowVal(row, $"{ColPrefix}uplift_pct"),2, MidpointRounding.AwayFromZero);
            string freq = (string)Util.GetRowVal(row, $"{ColPrefix}frequency");            
            switch (freq)
            {
                case "Current":
                    RetVal.Frequency = Conc_Analytics_Uplift.UpliftFrequency.Current; break;
                case "Shift":
                    RetVal.Frequency = Conc_Analytics_Uplift.UpliftFrequency.Shift; break;
                case "Today":
                    RetVal.Frequency = Conc_Analytics_Uplift.UpliftFrequency.Today; break;
                case "7_Days":
                    RetVal.Frequency = Conc_Analytics_Uplift.UpliftFrequency.Days_7; break;
                case "30_Days":
                    RetVal.Frequency = Conc_Analytics_Uplift.UpliftFrequency.Days_30; break;
            }

            return RetVal;

        }

    }
}
