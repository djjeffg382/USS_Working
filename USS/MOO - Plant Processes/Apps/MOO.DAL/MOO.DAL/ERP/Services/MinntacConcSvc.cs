using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.DAL.ERP.Enums;
using MOO.DAL.ERP.Models;

namespace MOO.DAL.ERP.Services
{
    public static class MinntacConcSvc
    {
        public const int METRIC_ID_CRUSH_FORECAST_OLD = 1018;
        public const int METRIC_ID_CRUSH_FORECAST = 1116;  //Was 1018 when this was a 3 shift schedule.  1116 is the 2 shift schedule
        public const int METRIC_ID_CON_FORECAST = 1030;


        /// <summary>
        /// Gets the ERP Messages for the Concentrator Production line days
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPMessage> GetConcProdLineDayMessages(DateTime StartDate, DateTime EndDate)
        {
            List<ERPMessage> retVal = new();
            var data = GetConcProdLineDayValues(StartDate, EndDate);

            return ERPMessage.DataValuesToERPMessages("Conc Line Production",data, Plant.Minntac, TimePeriod.Daily, ProcessCode.XCN, 
                                    Material.Concentrate, Activity.CONCENTRATOR, 2, "LT", "ACT", x => $"CCL{x}");
        }

        /// <summary>
        /// Gets ERP daily data for Concentrator Lines
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPValues> GetConcProdLineDayValues(DateTime StartDate, DateTime EndDate)
        {
            List<ERPValues> retVal = new();
            var concLine = ToLive.Services.Met_Conc_LineSvc.Get(StartDate, EndDate, 2, 18);
            foreach (var cnc in concLine)
            {
                retVal.Add(new ERPValues() { Shift_Date = cnc.Datex, Line = cnc.Line, Value = cnc.Conc_Tons.GetValueOrDefault(0) });
            }
            return retVal;
        }



        /// <summary>
        /// Gets the Conc Forecast day Values
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPValues> GetConcForecastDayValues(DateTime StartDate, DateTime EndDate)
        {
            return ERPValues.GetMetricValuesByShiftDate(METRIC_ID_CON_FORECAST, StartDate, EndDate);
        }

        /// <summary>
        /// Gets the ERP Messages for the Concentrator Forecast day Values
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPMessage> GetConcForecastDayMessages(DateTime StartDate, DateTime EndDate)
        {
            var data = GetConcForecastDayValues(StartDate, EndDate);

            return ERPMessage.DataValuesToERPMessages("Concentrate Forecast", data, Plant.Minntac, TimePeriod.Daily, ProcessCode.XCN,
                                    Material.Concentrate, Activity.CONCENTRATOR, 2, "LT", "ACT", "CONOE");
        }


        /// <summary>
        /// Gets the Crusher Forecast Values (001 +/- 010 and 011 belts).  Forecast of what goes into the 100 bins
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPValues> GetCrusherForecastValues(DateTime StartDate, DateTime EndDate)
        {
            int metricId;
            if (StartDate < DateTime.Parse("11/1/2023"))
                metricId = METRIC_ID_CRUSH_FORECAST_OLD;  //this was the old 3 shift target
            else
                metricId = METRIC_ID_CRUSH_FORECAST;  //New 2 shift target, switched to this on 11/1/2023

            return ERPValues.GetMetricValuesByShiftDate(metricId, StartDate, EndDate);
        }


        /// <summary>
        /// Gets the ERP Messages for the Concentrator forecast Values (001 +/- 010 and 011 belts).  Forecast of what goes into the 100 bins
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPMessage> GetCrusherForecastMessages(DateTime StartDate, DateTime EndDate)
        {

            /*********************NOTE, Not sure if this is used.  Pulled this from old program but didn't see it being sent in old program************/
            var data = GetCrusherForecastValues(StartDate, EndDate);

            return ERPMessage.DataValuesToERPMessages("Crusher Forecast (Crushed Rock Sent to Conc)",data, Plant.Minntac, TimePeriod.Daily, ProcessCode.XCN,
                                    Material.Concentrate, Activity.CONCENTRATOR, 2, "LT", "ACT", "MILLE");
        }


        /// <summary>
        /// Gets the Concentrator Total production for each day
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPValues> GetTotalConcProdDayValues(DateTime StartDate, DateTime EndDate)
        {
            List<ERPValues> retVal = new();
            var concLine = ToLive.Services.Met_Conc_Total_PlantSvc.GetByDateRange(StartDate, EndDate);
            foreach (var cnc in concLine)
            {
                retVal.Add(new ERPValues() { Shift_Date = cnc.Datex, Value = cnc.Conc_Tons_Nat.GetValueOrDefault(0) });
            }
            return retVal;
        }

        /// <summary>
        /// Gets the ERP Messages for the Concentrator Total production for each day
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPMessage> GetTotalConcProdDayMessages(DateTime StartDate, DateTime EndDate)
        {
            var data = GetTotalConcProdDayValues(StartDate, EndDate);

            return ERPMessage.DataValuesToERPMessages("Total Concentrate Production",data, Plant.Minntac, TimePeriod.Daily, ProcessCode.XCN,
                                    Material.Concentrate, Activity.CONCENTRATOR, 0, "LT", "ACT", "CONOD");
        }


        /// <summary>
        /// Gets total Rod Mill Feed for each day at the concentrator
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPValues> GetConcRMFDayValues(DateTime StartDate, DateTime EndDate)
        {
            List<ERPValues> retVal = new();
            string sql = @"SELECT to_date(ymd,'yyyymmdd') as prodDate, ROUND(sum(day_total),4) weight
                                FROM west_main.west_daily_conc
                                WHERE ymd BETWEEN " + StartDate.ToString("yyyyMMdd") + " AND " + EndDate.ToString("yyyyMMdd") +
                                @" AND id IN (4154, 4155, 4156)
                                GROUP BY ymd
                                ORDER BY ymd";


            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    retVal.Add(new ERPValues() { Shift_Date = rdr.GetDateTime(0), Value = rdr.GetDecimal(1) });
                }
            }
            conn.Close();
            return retVal;

        }


        /// <summary>
        /// Gets the ERP Messages for the Concentrator Total production for each day
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPMessage> GetConcRMFDayMessages(DateTime StartDate, DateTime EndDate)
        {
            var data = GetConcRMFDayValues(StartDate, EndDate);

            return ERPMessage.DataValuesToERPMessages("RMF Production", data, Plant.Minntac, TimePeriod.Daily, ProcessCode.XCN,
                                    Material.Concentrate, Activity.CONCENTRATOR, 2, "LT", "ACT", "MILLD");
        }




        /// <summary>
        /// Gets the Consumption values used for the month end Conc Consumption Totals
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPValues> GetConcConsumptionValues(DateTime StartDate, DateTime EndDate)
        {
            List<ERPValues> retVal = new();
            var concLine = ToLive.Services.Met_Conc_LineSvc.Get(StartDate, EndDate,2,18);
            foreach (var cnc in concLine)
            {
                retVal.Add(new ERPValues() { Shift_Date = cnc.Datex, Value = cnc.Rmf_Tons.GetValueOrDefault(0), Line=cnc.Line });
            }
            return retVal;
        }

        /// <summary>
        /// Gets the total Concentrator Consumption for the month
        /// </summary>
        /// <param name="MonthDate">Can be any day in the month where we are getting the production month total</param>
        /// <returns></returns>
        public static decimal GetConcConsumptionMonthTotal(DateTime MonthDate)
        {
            DateTime startDate = MOO.Dates.FirstDayOfMonth(MonthDate);
            DateTime endDate = MOO.Dates.LastDayOfMonth(MonthDate);
            var vals = GetConcConsumptionValues(startDate, endDate);
            decimal retVal = vals.Sum(x => x.Value);
            return retVal;
        }

        /// <summary>
        /// Gets the total Concentrator Production for the month
        /// </summary>
        /// <param name="MonthDate"></param>
        /// <returns></returns>
        public static decimal GetConcProductionMonthTotal(DateTime MonthDate)
        {
            DateTime endOfMonth = MOO.Dates.LastDayOfMonth(MonthDate);
            var ftg = MOO.DAL.ToLive.Services.Met_Feed_To_GrateSvc.GetAll(endOfMonth, endOfMonth);
            decimal concProdTotal = Math.Round(ftg.Sum(x => x.Conc_Tons).GetValueOrDefault(0),2,MidpointRounding.AwayFromZero) ;
            return concProdTotal;
        }


        /// <summary>
        /// Gets the ERP Messages for the Concentrator Month End Production
        /// </summary>
        /// <param name="MonthDate">Can be any day in the month where we are getting the production month total</param>
        /// <returns></returns>
        /// <remarks>This message will include a month total for concentrator and will include a consumption RAW record for the RMF Tons</remarks>
        public static ERPMessage GetConcProductionMonthMessage(DateTime MonthDate)
        {
            DateTime endOfMonth = MOO.Dates.LastDayOfMonth(MonthDate);
            
            decimal concProduction = GetConcProductionMonthTotal(endOfMonth);
            decimal concConsumption = GetConcConsumptionMonthTotal(endOfMonth);

            MSTRec mst = new(MOO.Plant.Minntac, endOfMonth, endOfMonth, TimePeriod.Monthly);
            MPARec mpa = new("CONOD", ProcessCode.XCN, Material.Concentrate, Activity.CONCENTRATOR,
                            Math.Round(concProduction,2).ToString(), "LT", "ACT");
            RAWRec raw = new RAWRec(Material.CrushRock, Math.Round(concConsumption, 2).ToString(), "LT", "ACT");
            List<RAWRec> rawRecs = new List<RAWRec>();
            rawRecs.Add(raw);

            ERPMessage retVal = new("Concentrator Month End Production/Consumption",mst,mpa, rawRecs);

            return retVal;
        }
    }
}
