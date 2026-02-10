using System;
using System.Collections.Generic;
using System.Text;
using MOO.DAL.Pi.Models;
using System.Data.OleDb;
using System.Data;
using MOO.DAL.Pi.Enums;
using MOO.Shifts;
using Azure;
using System.Xml;

namespace MOO.DAL.Pi.Services
{

    /// <summary>
    /// Obtains Digital Values from the PI System for a given tag
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    public static class PiDigSvc
    {
        static PiDigSvc()
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
        public static List<PiDig> Get(DateTime StartTime, DateTime EndTime, string Tag, string TimeStep = "", bool UseUtcTime = false)
        {
            List<PiDig> retVal = [];
            OleDbCommand cmd = GetCommandDigital(StartTime, EndTime, Tag,  TimeStep);
            string ConnString;

            if (UseUtcTime)
                ConnString = MOO.Data.GetConnectionString(Data.MNODatabase.MTC_PI_UTC);
            else
                ConnString = MOO.Data.GetConnectionString(Data.MNODatabase.MTC_Pi);

            using OleDbConnection conn = new(ConnString);
            conn.Open();
            cmd.Connection = conn;
            OleDbDataReader rdr = null;
            try
            {
                rdr = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("First argument for function is not digital"))
                {
                    //this tag is set up as an analog and not a digital, get the analog value
                    cmd = GetCommandAnalog(StartTime, EndTime, Tag, TimeStep);
                    cmd.Connection = conn;
                    rdr = cmd.ExecuteReader();

                }
                else
                    throw;
            }
            
            if (rdr != null && rdr.HasRows)
            {
                while (rdr.Read())
                {
                    PiDig pt = new();
                    FillPiObj(pt, rdr);
                    retVal.Add(pt);
                }
            }
            conn.Close();
            return retVal;
        }


        /// <summary>
        /// gets the command if this is a digital point
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="Expr"></param>
        /// <param name="TimeStep"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static OleDbCommand GetCommandDigital(DateTime StartTime, DateTime EndTime,
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
            cmd.Parameters.AddWithValue("", $"StateNo('{Expr}')");

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
        /// Gets the digital values if the PI tag is not a digital point
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="Expr"></param>
        /// <param name="TimeStep"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <remarks>Sometimes digital tags were set up as an analog, this will handle that situation by getting data from the interp table</remarks>
        private static OleDbCommand GetCommandAnalog(DateTime StartTime, DateTime EndTime,
                                       string Expr, string TimeStep)
        {
            OleDbCommand cmd = new();

            StringBuilder sql = new();
            if (string.IsNullOrEmpty(Expr))
                throw new Exception("picalc requires Expr value");

            sql.AppendLine("SELECT tag expr, time, value, timestep");
            sql.AppendLine($"FROM [piarchive]..[piinterp2]");
            //add in the required parameters first
            sql.AppendLine($"WHERE tag = ?");
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
        private static void FillPiObj(PiDig piObj, OleDbDataReader dr)
        {
            piObj.Expr = (string)Util.GetRowVal(dr, "expr");                  
            piObj.Time = (DateTime)Util.GetRowVal(dr, "time");
            piObj.TimeStep = (TimeSpan)Util.GetRowVal(dr, "timestep");
            piObj.Value = int.Parse(Util.GetRowVal(dr, "value").ToString());
            

        }

    }
}
