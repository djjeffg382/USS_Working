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
    /// Obtains Calculated values from the PI System
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    public static class PiCalcSvc
    {
        static PiCalcSvc()
        {
            Util.RegisterOLEDB();
        }


        /// <summary>
        /// Gets a PI Total listing
        /// </summary>
        /// <param name="StartTime">Start Time of the PI Total</param>
        /// <param name="EndTime">End Time of the PI Total</param>
        /// <param name="Expr">Performance equation expresion</param>
        /// <param name="TimeStep">Timestep</param>
        /// <returns></returns>
        public static List<PiCalc> Get(DateTime StartTime, DateTime EndTime, string Expr, string TimeStep = "", bool UseUtcTime = false)
        {
            List<PiCalc> retVal = new();
            OleDbCommand cmd = GetCommand(StartTime, EndTime, Expr,  TimeStep);
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
                    PiCalc pt = new();
                    FillPiAggregate(pt, rdr);
                    retVal.Add(pt);
                }
            }
            conn.Close();
            return retVal;
        }



        private static OleDbCommand GetCommand(DateTime StartTime, DateTime EndTime,
                                        string Expr ,  string TimeStep)
        {
            OleDbCommand cmd = new();

            StringBuilder sql = new();
            if (string.IsNullOrEmpty(Expr))
                throw new Exception("picalc requires Expr value");

            sql.AppendLine("SELECT expr, time, value, timestep");
            sql.AppendLine($"FROM [piarchive]..[picalc]");
            //add in the required parameters first
            sql.AppendLine($"WHERE expr = ?");
            cmd.Parameters.AddWithValue("", Expr);

            //we must have either a tag or an expr
            sql.AppendLine($"AND time BETWEEN ? AND ? ");
            cmd.Parameters.AddWithValue("", StartTime);
            cmd.Parameters.AddWithValue("", EndTime);




            //Optional Parameters
            if (!string.IsNullOrEmpty(TimeStep))
            {
                sql.AppendLine($"AND timestep = '{TimeStep}'");
            }
                
            cmd.CommandText = sql.ToString();
            return cmd;
        }



        /// <summary>
        /// used to fill the common aggregate fields for PiAggregate objects
        /// </summary>
        /// <param name="piObj"></param>
        /// <param name="dr"></param>
        private static void FillPiAggregate(PiCalc piObj, OleDbDataReader dr)
        {
            piObj.Expr = (string)Util.GetRowVal(dr, "expr");                  
            piObj.Time = (DateTime)Util.GetRowVal(dr, "time");
            piObj.TimeStep = (TimeSpan)Util.GetRowVal(dr, "timestep");
            object val = Util.GetRowVal(dr, "value");
            if (val != null)
            {
                if(double.TryParse(val.ToString(),out double result))
                    piObj.Value = result;  //this data is set as variant and is sometimes a float instead of double
            }
                

        }

    }
}
