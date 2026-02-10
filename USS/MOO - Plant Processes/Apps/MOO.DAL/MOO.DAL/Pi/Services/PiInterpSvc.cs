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
    /// Obtains Interpreted values from the PI System
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    public static class PiInterpSvc
    {
        static PiInterpSvc()
        {
            Util.RegisterOLEDB();
        }


        /// <summary>
        /// Gets a PI Total listing
        /// </summary>
        /// <param name="StartTime">Start Time of the PI Total</param>
        /// <param name="EndTime">End Time of the PI Total</param>
        /// <param name="Tag">Pi Tag</param>
        /// <param name="TimeStep">Timestep</param>
        /// <returns></returns>
        public static List<PiInterp> Get(DateTime StartTime, DateTime EndTime, string Tag, string TimeStep = "", bool UseUtcTime = false)
        {
            List<PiInterp> retVal = new();
            OleDbCommand cmd = GetCommand(StartTime, EndTime, Tag,  TimeStep);
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
                    PiInterp pt = new();
                    FillPiObj(pt, rdr);
                    retVal.Add(pt);
                }
            }
            conn.Close();
            return retVal;
        }



        private static OleDbCommand GetCommand(DateTime StartTime, DateTime EndTime,
                                        string Tag ,  string TimeStep)
        {
            OleDbCommand cmd = new();

            StringBuilder sql = new();
            if (string.IsNullOrEmpty(Tag))
                throw new Exception("PI Interp requires Tag value");

            sql.AppendLine("SELECT tag, time, value, status, timestep");
            sql.AppendLine($"FROM [piarchive]..[piinterp2]");
            //add in the required parameters first
            sql.AppendLine($"WHERE tag = ?");
            cmd.Parameters.AddWithValue("", Tag);

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
        private static void FillPiObj(PiInterp piObj, OleDbDataReader dr)
        {
            piObj.Tag = (string)Util.GetRowVal(dr, "tag");                  
            piObj.Time = (DateTime)Util.GetRowVal(dr, "time");
            piObj.TimeStep = (TimeSpan)Util.GetRowVal(dr, "timestep");
            object val = Util.GetRowVal(dr, "value");
            if(MOO.General.IsNumber(val))
                piObj.Value = Convert.ToDouble(val);
            else
            {
                if(val != null) 
                    piObj.SValue =  val.ToString();
            }
               
            piObj.Status = (int)Util.GetRowVal(dr, "status");


        }

    }
}
