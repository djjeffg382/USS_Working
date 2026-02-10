using MOO.DAL.ERP.Enums;
using MOO.DAL.ERP.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ERP.Services
{
    public static class KeetacConcSvc
    {

        /// <summary>
        /// Gets Values for Keetac Concentrator Primary Mill Lines Feed
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPValues> GetConcPrimaryLineValues(DateTime StartDate, DateTime EndDate)
        {
            List<ERPValues> retVal = new();
            string sql = @"Select shift_date, conc_feed_gt, line
                                FROM warehouse.K_CONC_PRI_LINE_DAY
                                WHERE shift_date between " + MOO.Dates.OraDate(StartDate) + " AND " + MOO.Dates.OraDate(EndDate) +
                                " ORDER BY shift_date,line";


            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    retVal.Add(new ERPValues() { Shift_Date = rdr.GetDateTime(0), Value = rdr.GetDecimal(1), Line = (int)rdr.GetDecimal(2) });
                }
            }
            conn.Close();
            return retVal;
        }

        /// <summary>
        /// Gets the ERP Messages for the Keetac Concentrator Primary Mill Lines Feed
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPMessage> GetConcPrimaryLineMessages(DateTime StartDate, DateTime EndDate)
        {
            List<ERPMessage> retVal = new();
            var data = GetConcPrimaryLineValues(StartDate, EndDate);
            return ERPMessage.DataValuesToERPMessages("Conc Primary Line Production",data, Plant.Keetac, TimePeriod.Daily, ProcessCode.XCN,
                                    Material.Concentrate, Activity.CONCENTRATOR, 0, "LT", "ACT", x => $"PRL{x}");
        }



        /// <summary>
        /// Gets Values for Keetac Concentrate Forecast Values
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPValues> GetConcentrateForecastValues(DateTime StartDate, DateTime EndDate)
        {
            List<ERPValues> retVal = new();
            string sql = @"SELECT shift_date, NVL(conc_forecast_gt,0) as weight
                                FROM warehouse.K_CONC_DAY
                                WHERE shift_date between " + MOO.Dates.OraDate(StartDate) + " AND " + MOO.Dates.OraDate(EndDate) +
                                " ORDER BY shift_date";


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
        /// Gets the ERP Messages for the Keetac Concentrate Forecast
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPMessage> GetConcentrateForecastMessages(DateTime StartDate, DateTime EndDate)
        {
            var data = GetConcentrateForecastValues(StartDate, EndDate);
            return ERPMessage.DataValuesToERPMessages("Concentrate Forecast", data, Plant.Keetac, TimePeriod.Daily, ProcessCode.XCN,
                                    Material.Concentrate, Activity.CONCENTRATOR, 0, "LT", "ACT", "CONOE");
        }

        /// <summary>
        /// Gets Values for Keetac Mill Feed Forecast Values
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPValues> GetMillFeedForecastValues(DateTime StartDate, DateTime EndDate)
        {
            List<ERPValues> retVal = new();
            string sql = @"SELECT shift_date, NVL(mill_feed_forecast_gt,0)
                                FROM warehouse.K_CONC_DAY
                                WHERE shift_date between " + MOO.Dates.OraDate(StartDate) + " AND " + MOO.Dates.OraDate(EndDate) +
                                " ORDER BY shift_date";


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
        /// Gets the ERP Messages for the Keetac Mill Feed Forecast
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPMessage> GetMillFeedForecastMessages(DateTime StartDate, DateTime EndDate)
        {
            var data = GetMillFeedForecastValues(StartDate, EndDate);
            return ERPMessage.DataValuesToERPMessages("Primary Mill Feed Forecast",data, Plant.Keetac, TimePeriod.Daily, ProcessCode.XCN,
                                    Material.Concentrate, Activity.CONCENTRATOR, 0, "LT", "ACT", "MILLE");
        }


        /// <summary>
        /// Gets Values for Keetac Contrator Production
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPValues> GetConcProductionValues(DateTime StartDate, DateTime EndDate)
        {
            return GetConcToDateValues(StartDate, EndDate, "concentrate_slurry_gt");
        }

        /// <summary>
        /// Gets the ERP Messages for the Keetac Contrator Production
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPMessage> GetConcProductionMessages(DateTime StartDate, DateTime EndDate)
        {
            var data = GetConcProductionValues(StartDate, EndDate);
            return ERPMessage.DataValuesToERPMessages("Conc Production",data, Plant.Keetac, TimePeriod.Daily, ProcessCode.XCN,
                                    Material.Concentrate, Activity.CONCENTRATOR, 0, "LT", "ACT", "CONOD");
        }





        /// <summary>
        /// Gets Values for Keetac DR Contrator Production
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPValues> GetDRConcProductionValues(DateTime StartDate, DateTime EndDate)
        {
            return GetConcToDateValues(StartDate, EndDate, "dr_conc_gt");
        }

        /// <summary>
        /// Gets the ERP Messages for the Keetac DR Contrator Production
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPMessage> GetDRConcProductionMessages(DateTime StartDate, DateTime EndDate)
        {
            var data = GetDRConcProductionValues(StartDate, EndDate);
            return ERPMessage.DataValuesToERPMessages("DR Conc Production", data, Plant.Keetac, TimePeriod.Daily, ProcessCode.XCN,
                                    Material.DRCon, Activity.CONCENTRATOR, 0, "LT", "ACT", "DRCON");
        }





        /// <summary>
        /// Gets Values for Keetac Contrator Mill Feed Totals
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPValues> GetConcMillFeedValues(DateTime StartDate, DateTime EndDate)
        {
            return GetConcToDateValues(StartDate, EndDate, "pri_conc_feed_gt");
        }
        /// <summary>
        /// Gets the ERP Messages for the Keetac Contrator Mill Feed Totals
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPMessage> GetConcMillFeedMessages(DateTime StartDate, DateTime EndDate)
        {
            var data = GetConcMillFeedValues(StartDate, EndDate);
            return ERPMessage.DataValuesToERPMessages("Mill Feed Forecast", data, Plant.Keetac, TimePeriod.Daily, ProcessCode.XCN,
                                    Material.Concentrate, Activity.CONCENTRATOR, 0, "LT", "ACT", "MILLD");
        }



        /// <summary>
        /// Gets Values for Keetac Field in the K_CONC_TD table
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        private static List<ERPValues> GetConcToDateValues(DateTime StartDate, DateTime EndDate, string FieldName)
        {
            List<ERPValues> retVal = new();
            StringBuilder sql = new();
            sql.AppendLine($"SELECT shift_date, NVL({FieldName},0)");
            sql.AppendLine("FROM warehouse.K_CONC_TD");
            sql.AppendLine("WHERE shift_date between " + MOO.Dates.OraDate(StartDate.AddDays(-1)) + " AND " + MOO.Dates.OraDate(EndDate));
            sql.AppendLine("AND td_type = 1");
            sql.AppendLine("ORDER BY shift_date");



            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            OracleDataReader rdr = cmd.ExecuteReader();
            decimal prevDayVal = 0;
            bool firstRead = true;
            DateTime prevDate = DateTime.MinValue;
            //we are getting data from the "TO_DATE" table therefore each day totals are evaluated from the difference from the previous day.
            //we are using the "TO_DATE" table as this table will include month adjustments
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    if (firstRead)
                    {
                        firstRead = false;
                    }
                    else
                    {
                        DateTime shiftDate = rdr.GetDateTime(0);
                        if (shiftDate.Month != prevDate.Month)
                            prevDayVal = 0;  //new month, zero the previous day value

                        retVal.Add(new ERPValues() { Shift_Date = shiftDate, Value = rdr.GetDecimal(1) - prevDayVal });
                    }
                    prevDayVal = rdr.GetDecimal(1);
                    prevDate = rdr.GetDateTime(0);
                }
            }
            conn.Close();
            return retVal;
        }

        #region "Monthe End Functions"

        /// <summary>
        /// Gets the total consumption of CrushRock for the specified month
        /// </summary>
        /// <param name="MonthDate">Can be any day in the month where we are getting the concumption month total</param>
        /// <returns></returns>
        public static decimal GetMonthConcConsumption(DateTime MonthDate)
        {
            DateTime endOfMonth = MOO.Dates.LastDayOfMonth(MonthDate);
            StringBuilder sql = new();
            //not sure why we are dividing by .97 but this was in the old code
            sql.AppendLine($"SELECT shift_date, ROUND(NVL(pri_conc_feed_gt/0.97,0),0)");
            sql.AppendLine("FROM warehouse.K_CONC_TD");
            sql.AppendLine("WHERE shift_date = " + MOO.Dates.OraDate(endOfMonth));
            sql.AppendLine("AND td_type = 1");

            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            OracleDataReader rdr = cmd.ExecuteReader();
            decimal retVal = 0;
            
            if (rdr.HasRows)
            {
                //there should only be one row for this shift date
                rdr.Read();
                retVal = rdr.GetDecimal(1);
                if (rdr.Read())
                    throw new Exception($"K_CONC_TD had extra line for date {endOfMonth:MM/dd/yyyy} and only one record was expected for ERP Conc Month Consumption");
            }
            conn.Close();
            return retVal;
        }

        /// <summary>
        /// Gets the total Production of concentrate for the specified month
        /// </summary>
        /// <param name="MonthDate">Can be any day in the month where we are getting the concumption month total</param>
        /// <returns></returns>
        public static decimal GetMonthConcProduction(DateTime MonthDate)
        {
            DateTime endOfMonth = MOO.Dates.LastDayOfMonth(MonthDate);
            StringBuilder sql = new();
            //This value is in dry tons, we need to send natural tons to ERP so divide by .91 (assume 9% moisture)
            sql.AppendLine($"SELECT shift_date, ROUND(NVL(concentrate_slurry_gt / .91,0),0)");
            sql.AppendLine("FROM warehouse.K_CONC_TD");
            sql.AppendLine("WHERE shift_date = " + MOO.Dates.OraDate(endOfMonth));
            sql.AppendLine("AND td_type = 1");

            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            OracleDataReader rdr = cmd.ExecuteReader();
            decimal retVal = 0;

            if (rdr.HasRows)
            {
                //there should only be one row for this shift date
                rdr.Read();
                retVal = rdr.GetDecimal(1);
                if (rdr.Read())
                    throw new Exception($"K_CONC_TD had extra line for date {endOfMonth:MM/dd/yyyy} and only one record was expected for ERP Conc Month Production");
            }
            conn.Close();
            return retVal;
        }


        /// <summary>
        /// Gets the ERP Messages for the Concentrator Month End Production
        /// </summary>
        /// <param name="MonthDate">Can be any day in the month where we are getting the production month total</param>
        /// <returns></returns>
        /// <remarks>This message will include a month total for concentrator and will include a consumption RAW record for the Crush Rock</remarks>
        public static ERPMessage GetConcProductionMonthMessage(DateTime MonthDate)
        {
            DateTime endOfMonth = MOO.Dates.LastDayOfMonth(MonthDate);

            decimal concProduction = GetMonthConcProduction(endOfMonth);
            decimal concConsumption = GetMonthConcConsumption(endOfMonth);

            MSTRec mst = new(MOO.Plant.Keetac, endOfMonth, endOfMonth, TimePeriod.Monthly);
            MPARec mpa = new("CONOD", ProcessCode.XCN, Material.Concentrate, Activity.CONCENTRATOR,
                            Math.Round(concProduction, 2).ToString(), "LT", "ACT");
            RAWRec raw = new RAWRec(Material.CrushRock, Math.Round(concConsumption, 2).ToString(), "LT", "ACT");
            List<RAWRec> rawRecs = new List<RAWRec>();
            rawRecs.Add(raw);

            ERPMessage retVal = new("Concentrator Month End Production/Consumption", mst, mpa, rawRecs);

            return retVal;
        }

        /// <summary>
        /// Gets the total consumption of Concentrate for the specified month to produce DR Conentrate
        /// </summary>
        /// <param name="MonthDate">Can be any day in the month where we are getting the concumption month total</param>
        /// <returns></returns>
        public static decimal GetMonthDRConcConsumption(DateTime MonthDate)
        {
            DateTime endOfMonth = MOO.Dates.LastDayOfMonth(MonthDate);
            decimal retVal = 0;
            var ErpProdCons = MOO.DAL.ToLive.Services.ERP_Production_ConsumptionSvc.Get(Plant.Keetac, MonthDate);
            if (ErpProdCons != null)
            {
                retVal = ErpProdCons.DRCon_Cons.GetValueOrDefault(0);
            }

            return retVal;
        }


        /// <summary>
        /// Gets the total Production of DR Concentrate for the specified month
        /// </summary>
        /// <param name="MonthDate">Can be any day in the month where we are getting the concumption month total</param>
        /// <returns></returns>
        public static decimal GetMonthDRConcProduction(DateTime MonthDate)
        {
            DateTime endOfMonth = MOO.Dates.LastDayOfMonth(MonthDate);
            StringBuilder sql = new();
            //This value is in dry tons, we need to send natural tons to ERP so divide by .91 (assume 9% moisture)
            sql.AppendLine($"SELECT shift_date, ROUND(NVL(dr_conc_gt / .91,0),0)");
            sql.AppendLine("FROM warehouse.K_CONC_TD");
            sql.AppendLine("WHERE shift_date = " + MOO.Dates.OraDate(endOfMonth));
            sql.AppendLine("AND td_type = 1");

            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            OracleDataReader rdr = cmd.ExecuteReader();
            decimal retVal = 0;

            if (rdr.HasRows)
            {
                //there should only be one row for this shift date
                rdr.Read();
                retVal = rdr.GetDecimal(1);
                if (rdr.Read())
                    throw new Exception($"K_CONC_TD had extra line for date {endOfMonth:MM/dd/yyyy} and only one record was expected for ERP DR Conc Month Production");
            }
            conn.Close();
            return retVal;
        }



        /// <summary>
        /// Gets the ERP Messages for the DR Contrate Month End Production
        /// </summary>
        /// <param name="MonthDate">Can be any day in the month where we are getting the production month total</param>
        /// <returns></returns>
        /// <remarks>This message will include a month total for DR Concentrate and will include a consumption RAW record for the Concentrate consumed</remarks>
        public static ERPMessage GetDRConcProductionMonthMessage(DateTime MonthDate)
        {
            DateTime endOfMonth = MOO.Dates.LastDayOfMonth(MonthDate);

            decimal drConcProduction = GetMonthDRConcProduction(endOfMonth);
            decimal drConcConsumption = GetMonthDRConcConsumption(endOfMonth);

            MSTRec mst = new(MOO.Plant.Keetac, endOfMonth, endOfMonth, TimePeriod.Monthly);
            MPARec mpa = new("DRCON", ProcessCode.XCN, Material.DRCon, Activity.CONCENTRATOR,
                            Math.Round(drConcProduction, 2).ToString(), "LT", "ACT");
            RAWRec raw = new RAWRec(Material.Concentrate, Math.Round(drConcConsumption, 2).ToString(), "LT", "ACT");
            List<RAWRec> rawRecs = new List<RAWRec>();
            rawRecs.Add(raw);

            ERPMessage retVal = new("DR Concentrate Month End Production/Consumption", mst, mpa, rawRecs);

            return retVal;
        }

        #endregion

    }
}
