using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;

namespace MOO.DAL.ToLive.Services
{
    public static class Pulse_Analog_CheckSvc
    {


        static Pulse_Analog_CheckSvc()
        {
            Util.RegisterOracle();
        }

        /// <summary>
        /// Gets the pulse analog data by area
        /// </summary>
        /// <param name="area">The plant area</param>
        /// <returns></returns>
        public static List<Pulse_Analog_Check> GetByArea(Pulse_Analog_Check.PACheckArea area)
        {
            List<Pulse_Analog_Check> retVal = new();
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE area = '{area}'");
            sql.AppendLine("ORDER BY description");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            foreach(DataRow dr in ds.Tables[0].Rows)
            {
                retVal.Add(DataRowToObject(dr));
            }
            return retVal;
        }


        /// <summary>
        /// Gets the pulse analog data by area with the pi values
        /// </summary>
        /// <param name="area">The plant area</param>
        /// <returns></returns>
        public static List<Pulse_Analog_Check> GetByAreaWithPiValues(Pulse_Analog_Check.PACheckArea area, DateTime startTime, DateTime endTime)
        {
            List<Pulse_Analog_Check> retVal = GetByArea(area);
            
            foreach (Pulse_Analog_Check pac in retVal)
            {
                List<Pi.Models.PiTotal> pt;
                pt = Pi.Services.PiAggregateSvc.GetPiTotal(startTime, endTime, Tag: pac.Analog_Point);
                if (pt.Count > 0)
                    pac.PiAnalogValue = pt[0];

                pt = Pi.Services.PiAggregateSvc.GetPiTotal(startTime, endTime, Tag: pac.Pulse_Point);
                if (pt.Count > 0)
                    pac.PiPulseValue = pt[0];
            }
            return retVal;
        }


        private static string GetSelect(string addField = "")
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine("active, pulse_point, pulse_multiplier, analog_point, analog_multiplier, email_list, ");
            sql.AppendLine("area, percent_diff, minimum_value, description, notify");
            if (!string.IsNullOrEmpty(addField))
            {
                sql.Append(", ");
                sql.AppendLine(addField);
            }
            sql.AppendLine("FROM tolive.pulse_analog_check");
            return sql.ToString();
        }



        private static Pulse_Analog_Check DataRowToObject(DataRow row)
        {
            Pulse_Analog_Check RetVal = new();
            RetVal.Active = row.Field<decimal>("active") == 1;
            RetVal.Pulse_Point = row.Field<string>("pulse_point");
            RetVal.Pulse_Multiplier = row.Field<decimal>("pulse_multiplier");
            RetVal.Analog_Point = row.Field<string>("analog_point");
            RetVal.Analog_Multiplier = row.Field<decimal>("analog_multiplier");
            RetVal.Email_List = row.Field<string>("email_list");
            RetVal.Area = Enum.Parse<Pulse_Analog_Check.PACheckArea>(row.Field<string>("area"));
            RetVal.Percent_Diff = row.Field<decimal>("percent_diff");
            RetVal.Minimum_Value = row.Field<decimal>("minimum_value");
            RetVal.Description = row.Field<string>("description");
            RetVal.Notify = row.Field<decimal>("notify") == 1;
            return RetVal;
        }
    }
}
