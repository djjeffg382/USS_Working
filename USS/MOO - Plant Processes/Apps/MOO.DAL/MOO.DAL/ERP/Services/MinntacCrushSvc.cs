using Microsoft.Data.SqlClient;
using MOO.DAL.ERP.Enums;
using MOO.DAL.ERP.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ERP.Services
{
    public static class MinntacCrushSvc
    {
        /// <summary>
        /// Gets the Values for the Secondary crushing lines per day
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPValues> GetSecondaryCrushLineValues(DateTime StartDate, DateTime EndDate)
        {
            List<ERPValues> retVal = new();
            StringBuilder sql = new();

            string shiftType = StartDate > DateTime.Parse("11/1/2023") ? "12" : "8";  //before 11/1/2023 we  were 3 shifts per day in the database

            sql.AppendLine("select cls.shift_date as prodDate, round(sum(cls.crusher_feed_Ltons), 2) as weight, cls.prod_line as line");
            sql.AppendLine("from prod_rpt.crush_sec_line_summary cls");
            sql.AppendLine($"WHERE cls.shift_date between {MOO.Dates.OraDate(StartDate)} AND {MOO.Dates.OraDate(EndDate)}");
            sql.AppendLine($"AND cls.shift_type = {shiftType} AND cls.time_type = 1");
            sql.AppendLine("group by cls.prod_line, cls.shift_date order by cls.prod_line");

            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    retVal.Add(new ERPValues() { Shift_Date = rdr.GetDateTime(0), Value = rdr.GetDecimal(1), Line = Convert.ToInt32(rdr.GetDecimal(2)) });
                }
            }
            conn.Close();
            return retVal;
        }

        /// <summary>
        /// Gets the ERP Messages for the secondary crushing lines per day
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPMessage> GetSecondaryCrushLineMessages(DateTime StartDate, DateTime EndDate)
        {
            var data = GetSecondaryCrushLineValues(StartDate, EndDate);

            return ERPMessage.DataValuesToERPMessages("Secondary Crusher Production",data, Plant.Minntac, TimePeriod.Daily, ProcessCode.XCR,
                                    Material.CrushRock, Activity.CRUSHING, 0, "LT", "CALC", x => $"SCL{x}");
        }


        /// <summary>
        /// Gets the Values for the Tertiary crushing lines per day
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPValues> GetTertiaryCrushLineValues(DateTime StartDate, DateTime EndDate)
        {
            List<ERPValues> retVal = new();
            StringBuilder sql = new();
            string shiftType = StartDate > DateTime.Parse("11/1/2023") ? "12" : "8";  //before 11/1/2023 we  were 3 shifts per day in the database

            sql.AppendLine("select cls.shift_date as prodDate, round(sum(cls.crusher_feed_Ltons), 2) as weight, cls.prod_line as line");
            sql.AppendLine("from prod_rpt.crush_ter_line_summary cls");
            sql.AppendLine($"WHERE cls.shift_date between {MOO.Dates.OraDate(StartDate)} AND {MOO.Dates.OraDate(EndDate)}");
            sql.AppendLine($"AND cls.shift_type = {shiftType} AND cls.time_type = 1");
            sql.AppendLine("group by cls.prod_line, cls.shift_date order by cls.prod_line");

            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    retVal.Add(new ERPValues() { Shift_Date = rdr.GetDateTime(0), Value = rdr.GetDecimal(1), Line = Convert.ToInt32(rdr.GetDecimal(2)) });
                }
            }
            conn.Close();
            return retVal;
        }

        /// <summary>
        /// Gets the ERP Messages for the Tertiary crushing lines per day
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPMessage> GetTertiaryCrushLineMessages(DateTime StartDate, DateTime EndDate)
        {
            var data = GetTertiaryCrushLineValues(StartDate, EndDate);

            return ERPMessage.DataValuesToERPMessages("Tertiary Crusher Production",data, Plant.Minntac, TimePeriod.Daily, ProcessCode.XCR,
                                    Material.CrushRock, Activity.CRUSHING, 0, "LT", "CALC", x => $"TCL{x}");
        }






        /// <summary>
        /// Gets Rod Mill Feed values from Met table
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPValues> GetRMFValues(DateTime StartDate, DateTime EndDate)
        {
            var vals = ToLive.Services.Met_Conc_LineSvc.Get(StartDate, EndDate, 2, 18);

            var rmf = (from rec in vals
                       group rec by rec.Datex into grp

                        select new ERPValues
                        {
                            Shift_Date = grp.Key,
                            Value = grp.Sum(x => x.Rmf_Tons).GetValueOrDefault(0)
                        }
                        ).OrderBy(x => x.Shift_Date).ToList();
            return rmf;
        }

        /// <summary>
        /// Gets the ERP Messages for the Crusher Production Forecast per day
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPMessage> GetRMFMessages(DateTime StartDate, DateTime EndDate)
        {
            var data = GetRMFValues(StartDate, EndDate);

            return ERPMessage.DataValuesToERPMessages("RMF Tons",data, Plant.Minntac, TimePeriod.Daily, ProcessCode.XCR,
                                    Material.CrushRock, Activity.CRUSHING, 2, "LT", "CALC", "RMF");
        }



    }
}
