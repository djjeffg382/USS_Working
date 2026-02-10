using MOO.DAL.ERP.Enums;
using MOO.DAL.ERP.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ERP.Services
{
    public static class KeetacAggSvc
    {
        /// <summary>
        /// Gets Values for Balling Line Data
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPValues> GetBallingValues(DateTime StartDate, DateTime EndDate)
        {
            List<ERPValues> retVal = new();
            string sql = @"SELECT shift_date, green_ball_tons_gt, line
                                FROM warehouse.k_pell_ball_line_day
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
        /// Gets the ERP Messages for the Agglomerator Balling lines
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPMessage> GetBallingMessages(DateTime StartDate, DateTime EndDate)
        {
            var data = GetBallingValues(StartDate, EndDate);
            return ERPMessage.DataValuesToERPMessages("Balling Line Production",data, Plant.Keetac, TimePeriod.Daily, ProcessCode.XAG,
                                    Material.K1, Activity.AGGLOMERATOR, 0, "LT", "ACT", x => $"BAL{x}");
        }


        /// <summary>
        /// Gets Data for Pellet Forecast Data
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPValues> GetPelletForecastValues(DateTime StartDate, DateTime EndDate)
        {
            List<ERPValues> retVal = new();
            string sql = @"SELECT shift_date, pell_prod_forecast_gt
                                FROM warehouse.k_pell_day
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
        /// Gets the ERP Messages for the Pellet Forecast
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPMessage> GetPelletForecastMessages(DateTime StartDate, DateTime EndDate)
        {
            var data = GetPelletForecastValues(StartDate, EndDate);
            return ERPMessage.DataValuesToERPMessages("Pellet Forecast",data, Plant.Keetac, TimePeriod.Daily, ProcessCode.XAG,
                                    Material.K1, Activity.AGGLOMERATOR, 0, "LT", "ACT", "AGGAE");
        }





        /// <summary>
        /// Gets Data for Pellet Tons
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPValues> GetPelletProductionValues(DateTime StartDate, DateTime EndDate, Core.Enums.KTCPelletType PelletType)
        {
            return GetPellToDateValues(StartDate, EndDate, "pellet_tons_gt", PelletType);            
        }

        /// <summary>
        /// Gets the ERP Messages for the Pellet Production
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPMessage> GetPelletProductionMessages(DateTime StartDate, DateTime EndDate, Core.Enums.KTCPelletType PelletType)
        {
            var data = GetPelletProductionValues(StartDate, EndDate, PelletType);
            string productionUnit = PelletType== Core.Enums.KTCPelletType.BlastFurnace ? "AGGAD" : "DRPEL";
            Material pellMaterial;
            switch (PelletType)
            {
                case Core.Enums.KTCPelletType.BlastFurnace:
                    pellMaterial = Material.K1; break;
                case Core.Enums.KTCPelletType.DRPellet:
                    pellMaterial = Material.DRPel; break;
                default:
                    throw new Exception($"Invalid material for KTC Pellet Production {PelletType}");
            }
            return ERPMessage.DataValuesToERPMessages("Pellet Production", data, Plant.Keetac, TimePeriod.Daily, ProcessCode.XAG,
                                    pellMaterial, Activity.AGGLOMERATOR, 0, "LT", "ACT", productionUnit);
        }






        /// <summary>
        /// Gets Values for Keetac Field in the k_pell_td table
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        private static List<ERPValues> GetPellToDateValues(DateTime StartDate, DateTime EndDate, string FieldName, Core.Enums.KTCPelletType PelletType)
        {
            List<ERPValues> retVal = new();
            StringBuilder sql = new();
            sql.AppendLine($"SELECT shift_date, NVL({FieldName},0)");
            sql.AppendLine("FROM warehouse.k_pell_td");
            sql.AppendLine("WHERE shift_date between " + MOO.Dates.OraDate(StartDate.AddDays(-1)) + " AND " + MOO.Dates.OraDate(EndDate));
            sql.AppendLine("AND td_type = 1");
            sql.AppendLine("AND pellet_type = " + ((int)PelletType).ToString() );
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



        /// <summary>
        /// Gets the total consumption of Concentrate for the specified month
        /// </summary>
        /// <param name="MonthDate">Can be any day in the month where we are getting the concumption month total</param>
        /// <returns></returns>
        public static decimal GetMonthK1PellConsumption(DateTime MonthDate)
        {
            DateTime endOfMonth = MOO.Dates.LastDayOfMonth(MonthDate);
            decimal retVal = 0;
            var ErpProdCons = MOO.DAL.ToLive.Services.ERP_Production_ConsumptionSvc.Get(Plant.Keetac, MonthDate);
            if (ErpProdCons != null)
            {
                retVal = ErpProdCons.Aggk1_Cons.GetValueOrDefault(0);
            }

            return retVal;
        }


        /// <summary>
        /// Gets the total Production of BF Pellets for the specified month
        /// </summary>
        /// <param name="MonthDate">Can be any day in the month where we are getting the concumption month total</param>
        /// <returns></returns>
        public static decimal GetMonthPellProduction(DateTime MonthDate, Core.Enums.KTCPelletType PelletType)
        {
            DateTime endOfMonth = MOO.Dates.LastDayOfMonth(MonthDate);
            StringBuilder sql = new();
            //not sure why we divide by .91.  This was in the old code.  This may be for wet/dry conversion but not sure
            sql.AppendLine($"SELECT shift_date, ROUND(NVL(pellet_tons_gt,0 ),0)");
            sql.AppendLine("FROM warehouse.K_PELL_TD");
            sql.AppendLine("WHERE shift_date = " + MOO.Dates.OraDate(endOfMonth));
            sql.AppendLine("AND td_type = 1");
            sql.AppendLine($"AND pellet_type = {(int)PelletType}");

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
                    throw new Exception($"K_PELL_TD had extra line for date {endOfMonth:MM/dd/yyyy} and only one record was expected for ERP Pellet Month Production");
            }
            conn.Close();
            return retVal;
        }


        /// <summary>
        /// Gets the ERP Messages for the Agglomerator Month End Production
        /// </summary>
        /// <param name="MonthDate">Can be any day in the month where we are getting the production month total</param>
        /// <returns></returns>
        /// <remarks>This message will include a month total for each production as well as consumption for Concentrate.  Consumption records are in the RAW records</remarks>
        public static ERPMessage GetK1PellProdMonthMessages(DateTime MonthDate)
        {
            DateTime endOfMonth = MOO.Dates.LastDayOfMonth(MonthDate);

            decimal prod = GetMonthPellProduction(MonthDate,Core.Enums.KTCPelletType.BlastFurnace);
            decimal concConsumption = GetMonthK1PellConsumption(MonthDate);
            MSTRec mst = new(MOO.Plant.Keetac, endOfMonth, endOfMonth, TimePeriod.Monthly);
            MPARec mpa = new("AGGAD", ProcessCode.XAG, Material.K1, Activity.AGGLOMERATOR,
                            Math.Round(prod, 2, MidpointRounding.AwayFromZero).ToString(), "LT", "ACT");
            List<RAWRec> rawRecs = new();
            rawRecs.Add(new RAWRec(Material.Concentrate, concConsumption.ToString(), "LT", "ACT"));

            var retVal = new ERPMessage($"K1 Pellet Month End Production/Consumption", mst, mpa, rawRecs);

            return retVal;
        }




        /// <summary>
        /// Gets the total consumption of DR Concentrate for the specified month to proudce DR Pellets
        /// </summary>
        /// <param name="MonthDate">Can be any day in the month where we are getting the concumption month total</param>
        /// <returns></returns>
        public static decimal GetMonthDRPellConsumption(DateTime MonthDate)
        {
            DateTime endOfMonth = MOO.Dates.LastDayOfMonth(MonthDate);
            decimal retVal = 0;
            var ErpProdCons = MOO.DAL.ToLive.Services.ERP_Production_ConsumptionSvc.Get(Plant.Keetac, MonthDate);
            if (ErpProdCons != null)
            {
                retVal = ErpProdCons.DRPell_Cons.GetValueOrDefault(0);
            }

            return retVal;
        }




        /// <summary>
        /// Gets the ERP Messages for the Agglomerator Month End Production
        /// </summary>
        /// <param name="MonthDate">Can be any day in the month where we are getting the production month total</param>
        /// <returns></returns>
        /// <remarks>This message will include a month total for each production as well as consumption for Concentrate.  Consumption records are in the RAW records</remarks>
        public static ERPMessage GetDRPellProdMonthMessages(DateTime MonthDate)
        {
            DateTime endOfMonth = MOO.Dates.LastDayOfMonth(MonthDate);

            decimal prod = GetMonthPellProduction(MonthDate, Core.Enums.KTCPelletType.DRPellet);
            decimal concConsumption = GetMonthDRPellConsumption(MonthDate);
            MSTRec mst = new(MOO.Plant.Keetac, endOfMonth, endOfMonth, TimePeriod.Monthly);
            MPARec mpa = new("DRPEL", ProcessCode.XAG, Material.DRPel, Activity.AGGLOMERATOR,
                            Math.Round(prod, 2, MidpointRounding.AwayFromZero).ToString(), "LT", "ACT");
            List<RAWRec> rawRecs = new();
            rawRecs.Add(new RAWRec(Material.DRCon, concConsumption.ToString(), "LT", "ACT"));

            var retVal = new ERPMessage($"DR Pellet Month End Production/Consumption", mst, mpa, rawRecs);

            return retVal;
        }

    }
}
