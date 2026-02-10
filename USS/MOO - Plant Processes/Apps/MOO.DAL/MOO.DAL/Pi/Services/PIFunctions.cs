using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Pi.Services
{
    /// <summary>
    /// Class for handling misc. PI Functions
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    public static class PIFunctions
    {


        /// <summary>
        /// Runs a raw Performance equation function on PI
        /// </summary>
        /// <param name="PiFunction"></param>
        /// <param name="UseUtcTime"></param>
        /// <returns></returns>
        public static object RunPiFunction(string PiFunction, bool UseUtcTime = false)
        {
            string command = $"SELECT {PiFunction}";
            OleDbCommand cmd = new(command);


            string ConnString;

            if (UseUtcTime)
                ConnString = MOO.Data.GetConnectionString(Data.MNODatabase.MTC_PI_UTC);
            else
                ConnString = MOO.Data.GetConnectionString(Data.MNODatabase.MTC_Pi);

            using OleDbConnection conn = new(ConnString);
            conn.Open();
            cmd.Connection = conn;
            OleDbDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                //should only have one row
                return rdr[0];
            }
            else
                return null;

        }

        /// <summary>
        /// Gets the timespan for a given digital tag value
        /// </summary>
        /// <param name="piTag">The Pi Tag</param>
        /// <param name="startDate">Start Date </param>
        /// <param name="endDate">End Date</param>
        /// <param name="digitalValue">Digital Value of the PI Tag</param>
        /// <returns></returns>
        public static TimeSpan GetTimeEquals(string piTag, DateTime startDate, DateTime endDate, string digitalValue)
        {
            var result = (TimeSpan)RunPiFunction($"TimeEq ('{piTag}','{startDate:MM/dd/yyyy HH:mm:ss}','{endDate:MM/dd/yyyy HH:mm:ss}','{digitalValue}') ");
            return result;
        }

        /// <summary>
        /// Gets the timespan a tag was equal to a given tag value
        /// </summary>
        /// <param name="piTag">The Pi Tag</param>
        /// <param name="startDate">Start Date </param>
        /// <param name="endDate">End Date</param>
        /// <param name="tagValue">Value to compare tag against</param>
        /// <returns></returns>
        public static TimeSpan GetTimeEquals(string piTag, DateTime startDate, DateTime endDate, double tagValue)
        {
            var result = (TimeSpan)RunPiFunction($"TimeEq ('{piTag}','{startDate:MM/dd/yyyy HH:mm:ss}','{endDate:MM/dd/yyyy HH:mm:ss}',{tagValue}) ");
            return result;
        }


        /// <summary>
        /// Gets the timespan a tag was greater than or equal to a given tag value
        /// </summary>
        /// <param name="piTag">The Pi Tag</param>
        /// <param name="startDate">Start Date </param>
        /// <param name="endDate">End Date</param>
        /// <param name="tagValue">Value to compare tag against</param>
        /// <returns></returns>
        public static TimeSpan GetTimeGreaterOrEquals(string piTag, DateTime startDate, DateTime endDate, double tagValue)
        {
            var result = (TimeSpan)RunPiFunction($"TimeGE ('{piTag}','{startDate:MM/dd/yyyy HH:mm:ss}','{endDate:MM/dd/yyyy HH:mm:ss}',{tagValue}) ");
            return result;
        }


        /// <summary>
        /// Gets the timespan a tag was greater than a given tag value
        /// </summary>
        /// <param name="piTag">The Pi Tag</param>
        /// <param name="startDate">Start Date </param>
        /// <param name="endDate">End Date</param>
        /// <param name="tagValue">Value to compare tag against</param>
        /// <returns></returns>
        public static TimeSpan GetTimeGreaterThan(string piTag, DateTime startDate, DateTime endDate, double tagValue)
        {
            var result = (TimeSpan)RunPiFunction($"TimeGT ('{piTag}','{startDate:MM/dd/yyyy HH:mm:ss}','{endDate:MM/dd/yyyy HH:mm:ss}',{tagValue}) ");
            return result;
        }


        /// <summary>
        /// Gets the timespan a tag was less than or equal to a given tag value
        /// </summary>
        /// <param name="piTag">The Pi Tag</param>
        /// <param name="startDate">Start Date </param>
        /// <param name="endDate">End Date</param>
        /// <param name="tagValue">Value to compare tag against</param>
        /// <returns></returns>
        public static TimeSpan GetTimeLessOrEquals(string piTag, DateTime startDate, DateTime endDate, double tagValue)
        {
            var result = (TimeSpan)RunPiFunction($"TimeLE ('{piTag}','{startDate:MM/dd/yyyy HH:mm:ss}','{endDate:MM/dd/yyyy HH:mm:ss}',{tagValue}) ");
            return result;
        }


        /// <summary>
        /// Gets the timespan a tag was less than a given tag value
        /// </summary>
        /// <param name="piTag">The Pi Tag</param>
        /// <param name="startDate">Start Date </param>
        /// <param name="endDate">End Date</param>
        /// <param name="tagValue">Value to compare tag against</param>
        /// <returns></returns>
        public static TimeSpan GetTimeLessThan(string piTag, DateTime startDate, DateTime endDate, double tagValue)
        {
            var result = (TimeSpan)RunPiFunction($"TimeLT ('{piTag}','{startDate:MM/dd/yyyy HH:mm:ss}','{endDate:MM/dd/yyyy HH:mm:ss}',{tagValue}) ");
            return result;
        }


        /// <summary>
        /// Gets time equal based on a PI Expression
        /// </summary>
        /// <param name="StartDate">Start Time</param>
        /// <param name="EndDate">End Time</param>
        /// <param name="CalcExpression">PI Calculation</param>
        /// <param name="TagName">Primary tag used in the calculation</param>
        /// <param name="UseUtcTime">Whether time is given in UTC</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static int GetTimeTrueFromExprSeconds(DateTime StartDate, DateTime EndDate, string CalcExpression, string TagName, bool UseUtcTime = false)
        {
            

            StringBuilder sql = new();
            if (string.IsNullOrEmpty(CalcExpression))
                throw new Exception("picalc requires Expr value");

            sql.AppendLine("SELECT value");
            sql.AppendLine("FROM [piarchive]..[picount]");
            sql.AppendLine("    WHERE filterexpr = ?");
            sql.AppendLine("    AND time BETWEEN ? AND ?");
            sql.AppendLine("    AND tag = ?");
            sql.AppendLine("    and filtersampleinterval = '1s'");
            sql.AppendLine("");
            sql.AppendLine("");
            OleDbCommand cmd = new(sql.ToString());
            cmd.Parameters.AddWithValue("", CalcExpression);
            cmd.Parameters.AddWithValue("", StartDate);
            cmd.Parameters.AddWithValue("", EndDate);
            cmd.Parameters.AddWithValue("", TagName);

            string ConnString;

            if (UseUtcTime)
                ConnString = MOO.Data.GetConnectionString(Data.MNODatabase.MTC_PI_UTC);
            else
                ConnString = MOO.Data.GetConnectionString(Data.MNODatabase.MTC_Pi);

            using OleDbConnection conn = new(ConnString);
            conn.Open();
            cmd.Connection = conn;
            OleDbDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                //should only have one row
                return int.Parse(rdr[0].ToString());
            }
            else
                return 0;

        }
    }
}
