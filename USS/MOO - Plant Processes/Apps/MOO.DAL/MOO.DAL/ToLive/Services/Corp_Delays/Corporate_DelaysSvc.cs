using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.DAL.ToLive.Models;

namespace MOO.DAL.ToLive.Services
{
    public static class Corporate_DelaysSvc
    {
        static Corporate_DelaysSvc()
        {
            Util.RegisterOracle();
        }


        public static Corporate_Delays Get(int Delay_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE cd.delay_id = :delay_id");


            Corporate_Delays retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("delay_id", Delay_Id);
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
        /// Gets the delay for the given Delay Type where end of delay is null.  Returns null if no open delay
        /// </summary>
        /// <param name="DelayType"></param>
        /// <returns></returns>
        public static Corporate_Delays GetOpenDelay(string DelayType)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE cd.delay_type = :DelayType");
            sql.AppendLine("AND cd.End_Of_Delay IS NULL");


            Corporate_Delays retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("DelayType", DelayType);
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
        /// Gets the delay for the given Delay Type where end of delay is null.  Returns null if no open delay
        /// </summary>
        /// <param name="DelayType"></param>
        /// <returns></returns>
        public static List<Corporate_Delays> GetOpenDelayLike(string DelayType)
        {
            List<Corporate_Delays> elements = new();

            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE cd.delay_type like :DelayType");
            sql.AppendLine("AND cd.End_Of_Delay IS NULL");


            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("DelayType", $"{DelayType}%");
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
        /// Gets delays by a given date range with optional parameter of Delay_Type
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="DelayType"></param>
        /// <returns></returns>
        public static List<Corporate_Delays> GetByDateRange(DateTime StartDate, DateTime EndDate, string DelayType = "")
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE (cd.Start_Of_Delay BETWEEN :StartDate AND :EndDate");
            sql.AppendLine($"OR cd.End_Of_Delay BETWEEN :StartDate AND :EndDate)");
            if (!string.IsNullOrEmpty(DelayType))
                sql.AppendLine($"AND cd.Delay_Type = :DelayType");


            List<Corporate_Delays> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("StartDate", StartDate);
            cmd.Parameters.Add("EndDate", EndDate);
            if (!string.IsNullOrEmpty(DelayType))
                cmd.Parameters.Add("DelayType", DelayType);
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


        internal static string GetColumns(string TableAlias = "cd", string ColPrefix = "cd_")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}delay_type {ColPrefix}delay_type, {ta}start_of_delay {ColPrefix}start_of_delay, ");
            cols.AppendLine($"{ta}end_of_delay {ColPrefix}end_of_delay, {ta}delay_id {ColPrefix}delay_id");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns() + ",");
            sql.AppendLine(Corporate_Delay_TypesSvc.GetColumns("cdt", "cdt_"));
            sql.AppendLine("FROM tolive.corporate_delays cd");
            sql.AppendLine("JOIN tolive.corporate_delay_types cdt ");
            sql.AppendLine("    ON cdt.delay_type_id = cd.delay_type");
            return sql.ToString();
        }


        /// <summary>
        /// Cahnges the delay type by ending old delay and creating a new one in the corporate delay table
        /// </summary>
        /// <param name="DelayTypeFrom">The old delay Type Id</param>
        /// <param name="DelayTypeTo">The mew delay Type Id</param>
        /// <param name="DelayTime">The time of the start of the delay</param>
        public static void ChangeDelay(string DelayTypeFrom,string DelayTypeTo, DateTime DelayTime)
        {
            using OracleConnection oracleConnection = new OracleConnection(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            oracleConnection.Open();
            OracleCommand oracleCommand = new OracleCommand("Delay.SwitchDelay", oracleConnection);
            oracleCommand.Parameters.Add("p_date", DelayTime);
            oracleCommand.Parameters.Add("p_delayTypeFrom", DelayTypeFrom);
            oracleCommand.Parameters.Add("p_delayTypeTo", DelayTypeTo);
            oracleCommand.CommandType = System.Data.CommandType.StoredProcedure;
            oracleCommand.BindByName = true;
            oracleCommand.ExecuteNonQuery();
        }


            /// <summary>
            /// Records the start or end of a corporate delay by calling the Delay.M function
            /// </summary>
            /// <param name="DelayType">The delay Type Id</param>
            /// <param name="DelayTime">The time of the start or end delay</param>
            /// <param name="RunIndicator">False = Start Delay, True = End Delay</param>
            public static void RecordMtcDelay(string DelayType, DateTime DelayTime, bool RunIndicator)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new("Delay.M", conn);
            cmd.Parameters.Add("p_date", DelayTime);
            cmd.Parameters.Add("p_delay_type", DelayType);
            cmd.Parameters.Add("p_run_ind", RunIndicator ? 1 : 0);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.ExecuteNonQuery();
            conn.Close();
        }


        internal static Corporate_Delays DataRowToObject(DbDataReader row, string ColPrefix = "cd_")
        {
            Corporate_Delays RetVal = new();
            RetVal.Delay_Type = Corporate_Delay_TypesSvc.DataRowToObject(row, "cdt_");
            RetVal.Start_Of_Delay = (DateTime)Util.GetRowVal(row, $"{ColPrefix}start_of_delay");
            RetVal.End_Of_Delay = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}end_of_delay");
            RetVal.Delay_Id = (decimal)Util.GetRowVal(row, $"{ColPrefix}delay_id");
            return RetVal;
        }

    }
}
