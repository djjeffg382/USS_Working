using System;
using System.Collections.Generic;
using System.Text;
using MOO.DAL.Pi.Models;
using System.Data.OleDb;
using System.Data;
using MOO.DAL.Pi.Enums;
using MOO.Shifts;
using Azure;

namespace MOO.DAL.Pi.Services
{

    /// <summary>
    /// Class used for handling all of the PI Aggregation items (sum, avg, min, max, calc etc.)
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    public class PiAggregateSvc
    {
        static PiAggregateSvc()
        {
            Util.RegisterOLEDB();
        }

        /// <summary>
        /// Gets a PI Total listing
        /// </summary>
        /// <param name="StartTime">Start Time of the PI Total</param>
        /// <param name="EndTime">End Time of the PI Total</param>
        /// <param name="Tag">Pi Tag</param>
        /// <param name="Expr">Performance equation expresion</param>
        /// <param name="FilterExpr">Filter Expression</param>
        /// <param name="FilterSampleType">Filter Sample Type</param>
        /// <param name="FilterSampleInterval">Filter Sample Interval</param>
        /// <param name="TimeStep">Timestep</param>
        /// <param name="UseUtcTime">If true, time values will be returned in UTC and filter will be applied as UTC</param>
        /// <param name="CalcBasis">Calculation Basis</param>
        /// <returns></returns>
        public static List<PiTotal> GetPiTotal(DateTime StartTime, DateTime EndTime,
                                        string Tag = "", string Expr = "", string FilterExpr = "", string FilterSampleType = "",
                                            string FilterSampleInterval = "", string TimeStep = "", bool UseUtcTime = false,
                                            PiCalcBasis CalcBasis = PiCalcBasis.TimeWeighted)
        {
            return GetPiTotal(StartTime, EndTime, out _, Tag: Tag, Expr: Expr, FilterExpr: FilterExpr, FilterSampleType: FilterSampleType,
                                FilterSampleInterval: FilterSampleInterval, TimeStep: TimeStep, UseUtcTime: UseUtcTime, CalcBasis: CalcBasis);
        }


        /// <summary>
        /// Gets a PI Average listing
        /// </summary>
        /// <param name="StartTime">Start Time of the PI Total</param>
        /// <param name="EndTime">End Time of the PI Total</param>
        /// <param name="Tag">Pi Tag</param>
        /// <param name="Expr">Performance equation expresion</param>
        /// <param name="FilterExpr">Filter Expression</param>
        /// <param name="FilterSampleType">Filter Sample Type</param>
        /// <param name="FilterSampleInterval">Filter Sample Interval</param>
        /// <param name="TimeStep">Timestep</param>
        /// <param name="UseUtcTime">If true, time values will be returned in UTC and filter will be applied as UTC</param>
        /// <param name="CalcBasis">Calculation Basis</param>
        /// <returns></returns>
        public static List<PiAvg> GetPiAvg(DateTime StartTime, DateTime EndTime,
                                        string Tag = "", string Expr = "", string FilterExpr = "", string FilterSampleType = "",
                                            string FilterSampleInterval = "", string TimeStep = "", bool UseUtcTime = false,
                                            PiCalcBasis CalcBasis = PiCalcBasis.TimeWeighted)
        {
            return GetPiAvg(StartTime, EndTime, out _, Tag: Tag, Expr: Expr, FilterExpr: FilterExpr, FilterSampleType: FilterSampleType,
                                FilterSampleInterval: FilterSampleInterval, TimeStep: TimeStep, UseUtcTime: UseUtcTime, CalcBasis: CalcBasis);
        }

        /// <summary>
        /// Gets a PI Total listing, contains out parameter to obtain the PISQL used
        /// </summary>
        /// <param name="StartTime">Start Time of the PI Total</param>
        /// <param name="EndTime">End Time of the PI Total</param>
        /// <param name="piSql">Out variable that will contain the SQL used to query (used for troubleshooting)</param>
        /// <param name="Tag">Pi Tag</param>
        /// <param name="Expr">Performance equation expresion</param>
        /// <param name="FilterExpr">Filter Expression</param>
        /// <param name="FilterSampleType">Filter Sample Type</param>
        /// <param name="FilterSampleInterval">Filter Sample Interval</param>
        /// <param name="TimeStep">Timestep</param>
        /// <param name="UseUtcTime">If true, time values will be returned in UTC and filter will be applied as UTC</param>
        /// <param name="CalcBasis">Calculation Basis</param>
        /// <returns></returns>
        public static List<PiTotal> GetPiTotal(DateTime StartTime, DateTime EndTime, out string piSql,
                                        string Tag = "", string Expr = "", string FilterExpr = "", string FilterSampleType = "",
                                            string FilterSampleInterval = "", string TimeStep = "", bool UseUtcTime = false,
                                            PiCalcBasis CalcBasis = PiCalcBasis.TimeWeighted)
        {
            List<PiTotal> retVal = new();
            OleDbCommand cmd = GetCommand("", "pitotal", StartTime, EndTime, out piSql, Tag, Expr, FilterExpr, FilterSampleType,
                                        FilterSampleInterval, TimeStep, CalcBasis);
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
                while (rdr.Read())
                {
                    PiTotal pt = new();
                    FillPiAggregate(pt, rdr);
                    pt.CalcBasis = Enum.Parse<PiCalcBasis>(rdr.GetFieldValue<string>(rdr.GetOrdinal("calcbasis")));
                    retVal.Add(pt);
                }
            }
            conn.Close();
            conn.Dispose();
            return retVal;
        }


        private static OleDbCommand GetCommand(string AddColumns, string PiTable, DateTime StartTime, DateTime EndTime, out string piSql,
                                        string Tag, string Expr , string FilterExpr , string FilterSampleType,
                                            string FilterSampleInterval, string TimeStep, 
                                            PiCalcBasis CalcBasis)
        {
            OleDbCommand cmd = new();

            StringBuilder sql = new();
            if (string.IsNullOrEmpty(Tag) && string.IsNullOrEmpty(Expr))
                throw new Exception("PI Aggregate comman requires either the Tag or the Expr parameter");

            sql.AppendLine("SELECT tag, expr, filterexpr, filtersampletype, filtersampleinterval, timestep, time, calcbasis,");
            sql.AppendLine("    value, pctgood");
            if (!string.IsNullOrEmpty(AddColumns))
                sql.AppendLine("," + AddColumns);
            sql.AppendLine($"FROM [piarchive]..[{PiTable}]");
            //add in the required parameters first
            sql.AppendLine($"WHERE calcbasis = '{CalcBasis}'");

            //we must have either a tag or an expr
            //Had an issue with making the time as a parameter, it would work one time and then not the next CRAZY!!!!
            //found that if I just put the data as a string, then it seems to always work
            sql.AppendLine($"AND time BETWEEN '{StartTime:dd-MMM-yyyy HH:mm:ss}' AND '{EndTime:dd-MMM-yyyy HH:mm:ss}' ");


            if (!string.IsNullOrEmpty(Tag))
            {
                sql.AppendLine($"AND tag = ?");
                cmd.Parameters.AddWithValue("", Tag);
            }

            if (!string.IsNullOrEmpty(Expr))
            {
                sql.AppendLine($"AND expr = ?");
                cmd.Parameters.AddWithValue("", Expr);
            }



            //Optional Parameters
            if (!string.IsNullOrEmpty(TimeStep))
            {
                sql.AppendLine($"AND timestep = '{TimeStep}'");
            }
                
            if (!string.IsNullOrEmpty(FilterExpr))
            {
                sql.AppendLine($"AND filterexpr = ?");
                cmd.Parameters.AddWithValue("", FilterExpr);
            }
                
            if (!string.IsNullOrEmpty(FilterSampleType))
            {
                sql.AppendLine($"AND filtersampletype = ?");
                cmd.Parameters.AddWithValue("", FilterSampleType);
            }
                
            if (!string.IsNullOrEmpty(FilterSampleInterval))
            {
                sql.AppendLine($"AND filtersampleinterval = ?");
                cmd.Parameters.AddWithValue("", FilterSampleInterval);
            }
                
            piSql = sql.ToString();
            cmd.CommandText = sql.ToString();
            return cmd;
        }


        /// <summary>
        /// Gets a PI Average listing, contains out parameter to obtain the PISQL used
        /// </summary>
        /// <param name="StartTime">Start Time of the PI Total</param>
        /// <param name="EndTime">End Time of the PI Total</param>
        /// <param name="piSql">Out variable that will contain the SQL used to query (used for troubleshooting)</param>
        /// <param name="Tag">Pi Tag</param>
        /// <param name="Expr">Performance equation expresion</param>
        /// <param name="FilterExpr">Filter Expression</param>
        /// <param name="FilterSampleType">Filter Sample Type</param>
        /// <param name="FilterSampleInterval">Filter Sample Interval</param>
        /// <param name="TimeStep">Timestep</param>
        /// <param name="UseUtcTime">If true, time values will be returned in UTC and filter will be applied as UTC</param>
        /// <param name="CalcBasis">Calculation Basis</param>
        /// <returns></returns>
        public static List<PiAvg> GetPiAvg(DateTime StartTime, DateTime EndTime, out string piSql,
                                        string Tag = "", string Expr = "", string FilterExpr = "", string FilterSampleType = "",
                                            string FilterSampleInterval = "", string TimeStep = "", bool UseUtcTime = false,
                                            PiCalcBasis CalcBasis = PiCalcBasis.TimeWeighted)
        {
            List<PiAvg> retVal = new();
            OleDbCommand cmd = GetCommand("", "piavg", StartTime, EndTime, out piSql, Tag, Expr, FilterExpr, FilterSampleType,
                                        FilterSampleInterval, TimeStep, CalcBasis);
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
                while (rdr.Read())
                {
                    PiAvg pt = new();
                    FillPiAggregate(pt, rdr);
                    pt.CalcBasis = Enum.Parse<PiCalcBasis>(rdr.GetFieldValue<string>(rdr.GetOrdinal("calcbasis")));
                    retVal.Add(pt);
                }
            }
            conn.Close();
            return retVal;
        }

        /// <summary>
        /// used to fill the common aggregate fields for PiAggregate objects
        /// </summary>
        /// <param name="piObj"></param>
        /// <param name="dr"></param>
        private static void FillPiAggregate(PiAggregate piObj, OleDbDataReader dr)
        {
            piObj.Tag = (string)Util.GetRowVal(dr, "tag");
            piObj.Expr = (string)Util.GetRowVal(dr, "expr"); 
            piObj.FilterExpr = (string)Util.GetRowVal(dr, "filterexpr"); 
            piObj.FilterSampleType = (string)Util.GetRowVal(dr, "filtersampletype"); 
            piObj.FilterSampleInterval = (string)Util.GetRowVal(dr, "filtersampleinterval");                   
            piObj.Time = (DateTime)Util.GetRowVal(dr, "time");
            piObj.TimeStep = (TimeSpan)Util.GetRowVal(dr, "timestep");
            piObj.Value = (double?)Util.GetRowVal(dr, "value");
            piObj.PctGood = (double?)Util.GetRowVal(dr, "pctgood");

        }

    }
}
