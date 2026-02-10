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
    /// Class for reading Analytics Anomalies from the tolive database.
    /// </summary>
    public static class AnomaliesSvc
    {
        static AnomaliesSvc()
        {
            Util.RegisterOracle();
        }


        /// <summary>
        /// converts the MOO.Plant variable to MTC or KTC
        /// </summary>
        /// <param name="Plant"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static string ConvertPlantToAbbreviatedPlant(MOO.Plant Plant)
        {
            switch (Plant)
            {
                case Plant.Minntac:
                    return "MTC";
                case Plant.Keetac:
                    return "KTC";
                default:
                    throw new Exception($"Invalid plant {Plant}");
            }
        }



        /// <summary>
        /// Converts MTC or KTC to MOO.Plant variable
        /// </summary>
        /// <param name="Plant"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static MOO.Plant ConvertAbbreviatedPlantName(string Plant)
        {
            switch (Plant.ToUpper())
            {
                case "MTC":
                    return MOO.Plant.Minntac;
                case "KTC":
                    return MOO.Plant.Keetac;
                default:
                    throw new Exception($"Invalid plant {Plant}");
            }
        }

        /// <summary>
        /// Returns the latest N records for a given Plant, Location, Line, Sensor
        /// </summary>
        /// <param name="RecordCount"></param>
        /// <param name="Plant"></param>
        /// <param name="Location"></param>
        /// <param name="Line"></param>
        /// <param name="Sensor"></param>
        /// <returns></returns>
        public static List<Anomalies> GetLatestValues(int RecordCount, MOO.Plant Plant, string Location, short? Line = null, string Sensor = "")
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE rn <= :RecordCount");
            sql.AppendLine("AND plant = :Plant");
            sql.AppendLine("AND location = :Location");
            if (Line.HasValue)
                sql.AppendLine("AND line = :Line");
            if (!string.IsNullOrEmpty(Sensor))
                sql.AppendLine("AND sensor = :Sensor");



            List<Anomalies> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("RecordCount", RecordCount);
            cmd.Parameters.Add("Plant", ConvertPlantToAbbreviatedPlant(Plant));
            cmd.Parameters.Add("Location", Location);

            if (Line.HasValue)
                cmd.Parameters.Add("Line", Line);
            if (!string.IsNullOrEmpty(Sensor))
                cmd.Parameters.Add("Sensor", Sensor);

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
        /// Gets the anomalies for a given date range
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="Plant"></param>
        /// <param name="Location"></param>
        /// <param name="Line"></param>
        /// <param name="Sensor"></param>
        /// <returns></returns>
        public static List<Anomalies> Get(DateTime StartDate, DateTime EndDate, MOO.Plant Plant, string Location, short? Line = null, string Sensor = "")
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE tag_time BETWEEN :StartDate AND :EndDate");
            sql.AppendLine("AND plant = :Plant");
            sql.AppendLine("AND location = :Location");
            if (Line.HasValue)
                sql.AppendLine("AND line = :Line");
            if (!string.IsNullOrEmpty(Sensor))
                sql.AppendLine("AND sesnor = :Sensor");



            List<Anomalies> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("StartDate", StartDate);
            cmd.Parameters.Add("EndDate", EndDate);
            cmd.Parameters.Add("Plant", ConvertPlantToAbbreviatedPlant(Plant));
            cmd.Parameters.Add("Location", Location);

            if (Line.HasValue)
                cmd.Parameters.Add("Line", Line);
            if (!string.IsNullOrEmpty(Sensor))
                cmd.Parameters.Add("Sensor", Sensor);

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
            cols.AppendLine($"{ta}plant {ColPrefix}plant, {ta}location {ColPrefix}location, {ta}line {ColPrefix}line, ");
            cols.AppendLine($"{ta}sensor {ColPrefix}sensor, {ta}tag_time {ColPrefix}tag_time, {ta}flag {ColPrefix}flag");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT * FROM (");

            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns() + ",");
            //including a row number field so we can use this to get latest values
            sql.AppendLine("ROW_NUMBER() OVER(PARTITION BY Plant, Location, Sensor, line Order By tag_time desc) rn");
            sql.AppendLine("FROM tolive.anomalies");

            sql.AppendLine(")");
            return sql.ToString();
        }

        //Insert,Update,Delete not included in this as this table should be filled from Conc Analytic.  We should only need read access on this table for our applications

        internal static Anomalies DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Anomalies RetVal = new();
            RetVal.Plant = ConvertAbbreviatedPlantName((string)Util.GetRowVal(row, $"{ColPrefix}plant"));
            RetVal.Location = (string)Util.GetRowVal(row, $"{ColPrefix}location");
            RetVal.Line = (short)Util.GetRowVal(row, $"{ColPrefix}line");
            RetVal.Sensor = (string)Util.GetRowVal(row, $"{ColPrefix}sensor");
            RetVal.Tag_Time = (DateTime)Util.GetRowVal(row, $"{ColPrefix}tag_time");
            RetVal.Flag = (Enums.AnomalyFlag)((short)Util.GetRowVal(row, $"{ColPrefix}flag"));
            return RetVal;
        }
    }
}
