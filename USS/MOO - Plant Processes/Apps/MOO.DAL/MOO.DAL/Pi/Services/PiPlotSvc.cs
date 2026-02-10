using System;
using System.Collections.Generic;
using System.Text;
using MOO.DAL.Pi.Models;
using System.Data.OleDb;
using System.Data;
using MOO.DAL.Pi.Enums;
using MOO.Shifts;
using Azure;
using Newtonsoft.Json.Linq;
using System.Security.Policy;

namespace MOO.DAL.Pi.Services
{

    /// <summary>
    /// Obtains Interpreted values from the PI System
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    public static class PiPlotSvc
    {
        static PiPlotSvc()
        {
            Util.RegisterOLEDB();
        }


        /// <summary>
        /// Gets a PI Total listing
        /// </summary>
        /// <param name="StartTime">Start Time of the PI Total</param>
        /// <param name="EndTime">End Time of the PI Total</param>
        /// <param name="Tag">Pi Tag</param>
        /// <param name="IntervalCount">Tells how many intervals will be included across the whole time span.  Each interval will produce up to 5 values if they are
        ///unique, the value at the beginning of the interval, the value at the end of the interval, the
        ///highest value, the lowest value and at most one exceptional point
        ///</param>
        ///<param name="IncludeOnlyGoodStatus">If true, only records where status = 0 will be included</param>
        /// <returns></returns>
        public static List<PiPlot> Get(DateTime StartTime, DateTime EndTime, string Tag, int IntervalCount = 20, bool UseUtcTime = false, bool IncludeOnlyGoodStatus = true)
        {
            List<PiPlot> retVal = new();
            OleDbCommand cmd = GetCommand(StartTime, EndTime, Tag, IntervalCount);
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
                    if((int)Util.GetRowVal(rdr, "status") == 0)
                    {
                        PiPlot pt = new();
                        FillPiObj(pt, rdr);
                        retVal.Add(pt);
                    }
                    
                }
            }
            conn.Close();
            return retVal;
        }



        private static OleDbCommand GetCommand(DateTime StartTime, DateTime EndTime,
                                        string Tag ,  int IntervalCount)
        {
            OleDbCommand cmd = new();

            StringBuilder sql = new();
            if (string.IsNullOrEmpty(Tag))
                throw new Exception("PI Plot requires Tag value");

            sql.AppendLine("SELECT tag, time, value, status, questionable, substituted, annotated, annotations, intervalcount");
            sql.AppendLine($"FROM [piarchive]..[piplot]");
            //add in the required parameters first
            sql.AppendLine($"WHERE tag = ?");
            cmd.Parameters.AddWithValue("", Tag);

            //we must have either a tag or an expr
            sql.AppendLine($"AND time BETWEEN ? AND ? ");
            cmd.Parameters.AddWithValue("", StartTime);
            cmd.Parameters.AddWithValue("", EndTime);
            sql.AppendLine($"AND intervalcount = {IntervalCount}");
            cmd.CommandText = sql.ToString();
            return cmd;
        }



        /// <summary>
        /// used to fill the common aggregate fields for PiAggregate objects
        /// </summary>
        /// <param name="piObj"></param>
        /// <param name="dr"></param>
        private static void FillPiObj(PiPlot piObj, OleDbDataReader dr)
        {
            piObj.Tag = (string)Util.GetRowVal(dr, "tag");                  
            piObj.Time = (DateTime)Util.GetRowVal(dr, "time");
            object val = Util.GetRowVal(dr, "value");
            if (MOO.General.IsNumber(val))
                piObj.Value = Convert.ToDecimal(val);
            else
                piObj.Value = null;
            piObj.Status = (int)Util.GetRowVal(dr, "status");
            piObj.Questionable = (bool)Util.GetRowVal(dr, "questionable");
            piObj.Substituted = (bool)Util.GetRowVal(dr, "substituted");
            piObj.Annotated = (bool)Util.GetRowVal(dr, "annotated");
            piObj.Annotations = Util.GetRowVal(dr, "annotations")?.ToString();

            piObj.IntervalCount = (int)Util.GetRowVal(dr, "intervalcount");


        }

    }
}
