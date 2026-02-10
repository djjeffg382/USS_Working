using Microsoft.Data.SqlClient;
using MOO.DAL.ERP.Enums;
using MOO.DAL.ERP.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ERP.Services
{
    public static class KeetacMineSvc
    {
        /// <summary>
        /// Gets Values for tons dumped into Crusher 1 and Crusher 2 
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPValues> GetCrusherValues(DateTime StartDate, DateTime EndDate)
        {
            List<ERPValues> retVal = new();
            StringBuilder sql = new();
            sql.AppendLine("select Convert(Datetime,cast(m.shiftyyyymmdd as char(8)),112) as prodDate, ");
            sql.AppendLine("ISNULL(SUM(m.EQSurveyAdjustedWeight),0) weight, dumploc");
            sql.AppendLine("FROM ODSKTCProd.dbo.Material_Movement_Loads m with(NOLOCK)");
            sql.AppendLine("where m.RECORD_EXISTS='Y'");
            sql.AppendLine("and m.load_enum in (1,2)");
            sql.AppendLine("and m.loadloc not in ( 'CRSTK')");
            sql.AppendLine("and m.dumploc in ('CR1', 'CR2') ");
            sql.AppendLine("and m.loadloc not like '%-NP'");
            sql.AppendLine("and m.dumploc not like '%-NP'");
            sql.AppendLine($"and m.shiftindex BETWEEN {StartDate:yyyyMMdd}1 AND {EndDate:yyyyMMdd}2");
            sql.AppendLine("group by m.dumploc, m.shiftyyyymmdd");
            sql.AppendLine("ORDER BY shiftyyyymmdd, dumploc");


            using SqlConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.KTC_Minvu));
            conn.Open();
            SqlCommand cmd = new(sql.ToString(), conn);
            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    int line = rdr.GetString(2) == "CR1" ? 1 : 2;
                    retVal.Add(new ERPValues() { Shift_Date = rdr.GetDateTime(0), Value = (decimal)rdr.GetDouble(1), Line = line });
                }
            }
            conn.Close();
            return retVal;
        }


        /// <summary>
        /// Gets the ERP Messages for the Keetac dumped into Crusher 1 and Crusher 2 
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPMessage> GetCrusherMessages(DateTime StartDate, DateTime EndDate)
        {
            List<ERPMessage> retVal = new();
            var data = GetCrusherValues(StartDate, EndDate);
            return ERPMessage.DataValuesToERPMessages("Crusher Production",data, Plant.Keetac, TimePeriod.Daily, ProcessCode.XCR,
                                    Material.CrushRock, Activity.CRUSHING, 2, "LT", "CALC", x => $"CRUS{x}");
        }


        /// <summary>
        /// Gets Values for Keetac crusher stockpiled
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPValues> GetCrushStockedValues(DateTime StartDate, DateTime EndDate)
        {
            List<ERPValues> retVal = new();
            StringBuilder sql = new();
            sql.AppendLine("select Convert(Datetime,cast(m.shiftyyyymmdd as char(8)),112) as prodDate, ");
            sql.AppendLine("ISNULL(SUM(m.EQSurveyAdjustedWeight),0) weight");
            sql.AppendLine("FROM ODSKTCProd.dbo.Material_Movement_Loads m with(NOLOCK)");
            sql.AppendLine("where m.RECORD_EXISTS='Y'");
            sql.AppendLine("and m.load_enum in (1,2)");
            sql.AppendLine("and m.loadloc not in ( 'CRSTK')");
            sql.AppendLine("and m.dumploc in ('CRSTK') ");
            sql.AppendLine("and m.loadloc not like '%-NP'");
            sql.AppendLine("and m.dumploc not like '%-NP'");
            sql.AppendLine($"and m.shiftindex BETWEEN {StartDate:yyyyMMdd}1 AND {EndDate:yyyyMMdd}2");
            sql.AppendLine("group by m.shiftyyyymmdd");
            sql.AppendLine("ORDER BY shiftyyyymmdd");


            using SqlConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.KTC_Minvu));
            conn.Open();
            SqlCommand cmd = new(sql.ToString(), conn);
            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {                    
                    retVal.Add(new ERPValues() { Shift_Date = rdr.GetDateTime(0), Value = (decimal)rdr.GetDouble(1)});
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
        public static List<ERPMessage> GetCrushStockedMessages(DateTime StartDate, DateTime EndDate)
        {
            var data = GetCrushStockedValues(StartDate, EndDate);
            return ERPMessage.DataValuesToERPMessages("Crush Stocked",data, Plant.Keetac, TimePeriod.Daily, ProcessCode.XCR,
                                    Material.CrushRock, Activity.CRUSHING, 2, "LT", "CALC", "CRUSSTK");
        }



        /// <summary>
        /// Gets Crusher Production Forecast per day
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPValues> GetCrushedProductionForecastValues(DateTime StartDate, DateTime EndDate)
        {
            return ERPValues.GetMinVuPlansByShiftDate(Plant.Keetac, StartDate, EndDate, new int[] { 1 });
        }

        /// <summary>
        /// Gets the ERP Messages for the Crusher Production Forecast per day
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPMessage> GetCrushedProductionForecastMessages(DateTime StartDate, DateTime EndDate)
        {
            var data = GetCrushedProductionForecastValues(StartDate, EndDate);

            return ERPMessage.DataValuesToERPMessages("Mine Crude Forecast",data, Plant.Keetac, TimePeriod.Daily, ProcessCode.XCR,
                                    Material.CrushRock, Activity.CRUSHING, 2, "LT", "CALC", "CRUSE");
        }


        /// <summary>
        /// Gets Crusher Production Forecast per day
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPValues> GetWasteForecastValues(DateTime StartDate, DateTime EndDate)
        {
            return ERPValues.GetMinVuPlansByShiftDate(Plant.Keetac, StartDate, EndDate, new int[] { 8 });
        }

        /// <summary>
        /// Gets the ERP Messages for the Crusher Production Forecast per day
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPMessage> GetWasteForecastMessages(DateTime StartDate, DateTime EndDate)
        {
            var data = GetWasteForecastValues(StartDate, EndDate);

            return ERPMessage.DataValuesToERPMessages("Mine Waste Forecast", data, Plant.Keetac, TimePeriod.Daily, ProcessCode.XCR,
                                    Material.CrushRock, Activity.CRUSHING, 2, "LT", "CALC", "WSTEE");
        }





        /// <summary>
        /// Gets Values for Keetac crusher stockpiled
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPValues> GetWasteValues(DateTime StartDate, DateTime EndDate)
        {
            List<ERPValues> retVal = new();
            StringBuilder sql = new();
            sql.AppendLine("select Convert(Datetime,cast(m.shiftyyyymmdd as char(8)),112) as prodDate, ");
            sql.AppendLine("ISNULL(SUM(m.EQSurveyAdjustedWeight),0) weight");
            sql.AppendLine("FROM ODSKTCProd.dbo.Material_Movement_By_Load m with(NOLOCK)");
            sql.AppendLine("where m.RECORD_EXISTS='Y'");
            sql.AppendLine("and m.load_enum in (3,4,5,6)");
            sql.AppendLine("and m.loadloc not like '%-NP'");
            sql.AppendLine("and m.dumploc not like '%-NP'");
            sql.AppendLine($"and m.shiftindex BETWEEN {StartDate:yyyyMMdd}1 AND {EndDate:yyyyMMdd}2");
            sql.AppendLine("group by m.shiftyyyymmdd");
            sql.AppendLine("ORDER BY shiftyyyymmdd");


            using SqlConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.KTC_Minvu));
            conn.Open();
            SqlCommand cmd = new(sql.ToString(), conn);
            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    retVal.Add(new ERPValues() { Shift_Date = rdr.GetDateTime(0), Value = (decimal)rdr.GetDouble(1) });
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
        public static List<ERPMessage> GetWasteMessages(DateTime StartDate, DateTime EndDate)
        {
            var data = GetWasteValues(StartDate, EndDate);
            return ERPMessage.DataValuesToERPMessages("Mine Waste Production", data, Plant.Keetac, TimePeriod.Daily, ProcessCode.XCR,
                                    Material.CrushRock, Activity.CRUSHING,0, "LT", "CALC", "WSTED");
        }

        /// <summary>
        /// Gets Values for total crude tons (CR1, CR2, CRSTK)
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPValues> GetTotalCrudeMinedValues(DateTime StartDate, DateTime EndDate)
        {
            List<ERPValues> retVal = new();
            StringBuilder sql = new();
            sql.AppendLine("select Convert(Datetime,cast(m.shiftyyyymmdd as char(8)),112) as prodDate, ");
            sql.AppendLine("ISNULL(SUM(m.EQSurveyAdjustedWeight),0) weight");
            sql.AppendLine("FROM ODSKTCProd.dbo.Material_Movement_By_Load m with(NOLOCK)");
            sql.AppendLine("where m.RECORD_EXISTS='Y'");
            sql.AppendLine("and m.load_enum in (1,2)");
            sql.AppendLine("and m.loadloc not in ( 'CRSTK')");
            sql.AppendLine("and m.dumploc in ('CR1', 'CR2', 'CRSTK') ");
            sql.AppendLine("and m.loadloc not like '%-NP'");
            sql.AppendLine("and m.dumploc not like '%-NP'");
            sql.AppendLine($"and m.shiftindex BETWEEN {StartDate:yyyyMMdd}1 AND {EndDate:yyyyMMdd}2");
            sql.AppendLine("group by m.shiftyyyymmdd");
            sql.AppendLine("ORDER BY shiftyyyymmdd");


            using SqlConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.KTC_Minvu));
            conn.Open();
            SqlCommand cmd = new(sql.ToString(), conn);
            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {                    
                    retVal.Add(new ERPValues() { Shift_Date = rdr.GetDateTime(0), Value = (decimal)rdr.GetDouble(1) });
                }
            }
            conn.Close();
            return retVal;
        }


        /// <summary>
        /// Gets the ERP Messages for the Keetac dumped into Crusher 1 and Crusher 2 
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPMessage> GetTotalCrudeMinedMessages(DateTime StartDate, DateTime EndDate)
        {
            var data = GetTotalCrudeMinedValues(StartDate, EndDate);
            return ERPMessage.DataValuesToERPMessages("Total Crude Mined",data, Plant.Keetac, TimePeriod.Daily, ProcessCode.XCR,
                                    Material.CrushRock, Activity.CRUSHING, 2, "LT", "CALC", "CRUSTOT");
        }




        /// <summary>
        /// Gets the total Consumption for the mine
        /// </summary>
        /// <param name="MonthDate">Can be any day in the month where we are getting the concumption month total</param>
        /// <returns></returns>
        /// <remarks>ERP does not do anything with this number so we will just get this a hand entered value.  Eventually I want to try to not send this value at all</remarks>
        public static decimal GetMonthCrushConsumption(DateTime MonthDate)
        {
            DateTime startOfMonth = MOO.Dates.LastDayOfMonth(MonthDate);
            DateTime endOfMonth = MOO.Dates.LastDayOfMonth(MonthDate);
            var consume = MOO.DAL.ToLive.Services.ERP_Production_ConsumptionSvc.Get(Plant.Keetac, startOfMonth);
            if (consume == null)
                return 0;
            else
                return consume.Crush_Cons.GetValueOrDefault(0);

        }
        /// <summary>
        /// Gets the total Crusher production
        /// </summary>
        /// <param name="MonthDate">Can be any day in the month where we are getting the concumption month total</param>
        /// <returns></returns>
        /// <remarks>This value is set to equal the consumption for the concentratory</remarks>
        public static decimal GetMonthCrushProduction(DateTime MonthDate)
        {            
            return KeetacConcSvc.GetMonthConcConsumption(MonthDate);
        }


        /// <summary>
        /// Gets the ERP Messages for the Concentrator Month End Production
        /// </summary>
        /// <param name="MonthDate">Can be any day in the month where we are getting the production month total</param>
        /// <returns></returns>
        /// <remarks>This message will include a month total for concentrator and will include a consumption RAW record for the Crush Rock</remarks>
        public static ERPMessage GetMineProductionMonthMessage(DateTime MonthDate)
        {
            DateTime endOfMonth = MOO.Dates.LastDayOfMonth(MonthDate);

            decimal crushProduction = GetMonthCrushConsumption(endOfMonth);
            decimal crushConsumption = GetMonthCrushProduction(endOfMonth);

            MSTRec mst = new(MOO.Plant.Keetac, endOfMonth, endOfMonth, TimePeriod.Monthly);
            MPARec mpa = new("CRUSTOT", ProcessCode.XCR, Material.CrushRock, Activity.CRUSHING,
                            Math.Round(crushProduction, 2).ToString(), "LT", "ACT");
            RAWRec raw = new RAWRec(Material.Taconite, Math.Round(crushConsumption, 2).ToString(), "LT", "ACT", "USS", "USS", "USS", "USS", 1,"USS");
            List<RAWRec> rawRecs = new List<RAWRec>();
            rawRecs.Add(raw);
            ERPMessage retVal = new("Crusher Month End Production/Consumption", mst, mpa, rawRecs);
            return retVal;
        }

    }
}
