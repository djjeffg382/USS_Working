using MOO.DAL.ERP.Enums;
using MOO.DAL.ERP.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ERP.Services
{
    public static class MinntacAggSvc
    {
        private const int METRIC_ID_AGG2_ACID_FORECAST = 1044;
        private const int METRIC_ID_AGG3_ACID_FORECAST = 1048;

        private const int METRIC_ID_AGG2_FLUX_FORECAST = 1052;
        private const int METRIC_ID_AGG3_FLUX_FORECAST = 1053;


        /// <summary>
        /// Gets ERP Messages for daily totals for Agglomerator Lines
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="AggMaterial"></param>
        /// <returns></returns>
        public static List<ERPValues> GetAggProdLineDayValues(DateTime StartDate, DateTime EndDate, ToLive.Models.Met_Material AggMaterial)
        {
            List<ERPValues> retVal = new();
            var aggLine = ToLive.Services.Met_Agg_LineSvc.Get(StartDate, EndDate, 3, 7, AggMaterial, ToLive.Models.Met_DMY.Day);
            foreach (var agg in aggLine)
            {
                retVal.Add(new ERPValues() { Shift_Date = agg.Datex, Line = agg.Line, Value = agg.PelTons.GetValueOrDefault(0) });
            }
            return retVal;
        }


        /// <summary>
        /// Gets the ERP Messages for the Agglomerator lines per day
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPMessage> GetAggProdLineDayMessages(DateTime StartDate, DateTime EndDate, ToLive.Models.Met_Material AggMaterial)
        {
            List<ERPMessage> retVal = new();
            var data = GetAggProdLineDayValues(StartDate, EndDate, AggMaterial);
            Material erpMat = AggMaterial == ToLive.Models.Met_Material.Flux ? Material.FPellet : Material.APellet;
            return ERPMessage.DataValuesToERPMessages($"Agglomerator {AggMaterial} Production", data, Plant.Minntac, TimePeriod.Daily, ProcessCode.XAG,
                                    erpMat, Activity.AGGLOMERATOR, 2, "LT", "ACT", x => MinntacAggSvc.GetProductionUnitAggLine(x, AggMaterial));
        }



        /// <summary>
        /// Gets ERP Messages for Monthly totals for Agglomerator Lines
        /// </summary>
        /// <param name="MonthDate">Can be any day in the month where we are getting the production month total</param>
        /// <param name="AggMaterial">Agglomerator material (Flux or Acid)</param>
        /// <returns></returns>
        public static List<ERPValues> GetAggProdLineMonthValues(DateTime MonthDate, ToLive.Models.Met_Material AggMaterial)
        {
            DateTime endOfMonth = MOO.Dates.LastDayOfMonth(MonthDate);
            List<ERPValues> retVal = new();
            var aggLine = ToLive.Services.Met_Agg_LineSvc.Get(endOfMonth, endOfMonth, 3, 7, AggMaterial, ToLive.Models.Met_DMY.Month);
            for (int lineNbr = 3; lineNbr <= 7; lineNbr++)
            {
                var agg = aggLine.FirstOrDefault(x => x.Line == lineNbr);
                //if we didn't make the specified product that month, there will not be any records, still need a value for these so we can send zero
                if (agg == null)
                    retVal.Add(new ERPValues() { Shift_Date = MonthDate, Line = lineNbr, Value = 0 });  //create a record with zero tons
                else
                    retVal.Add(new ERPValues() { Shift_Date = agg.Datex, Line = agg.Line, Value = agg.PelTons_Adj.GetValueOrDefault(0) });
            }

            return retVal;
        }

        /// <summary>
        /// Gets the limestone consumption for the specified month
        /// </summary>
        /// <param name="MonthDate">Can be any day in the month where we are getting the production month total</param>
        /// <returns></returns>
        public static decimal GetLimestoneMonthConsumption(DateTime MonthDate)
        {
            decimal limeAdjusted = 0;
            DateTime endOfMonth = MOO.Dates.LastDayOfMonth(MonthDate);
            var crushPlant = MOO.DAL.ToLive.Services.Met_Crush_PlantSvc.Get(endOfMonth, ToLive.Models.Met_DMY.Month);
            if(crushPlant != null)
            {
                decimal limeAdjustedPct = decimal.Parse(MOO.Data.ReadDBKey("ERP_LIME_ADJ_PCT", "0"));
                limeAdjusted = crushPlant.Limestone_Tons.GetValueOrDefault(0) * (1 + (limeAdjustedPct / 100));
            }
            return limeAdjusted;
        }


        /// <summary>
        /// Gets the total Concentrate consumption for the specified month
        /// </summary>
        /// <param name="MonthDate">Can be any day in the month where we are getting the concumption month total</param>
        /// <param name="AggMaterial">Agglomerator material (Flux or Acid)</param>
        /// <returns></returns>
        /// <remarks>This message will include a month totatl for each line production as well as consumption for Concentrate and Limestone (If Flux).  Consumption records are in the RAW records</remarks>
        public static decimal GetAggMonthConcConsumption(DateTime MonthDate, ToLive.Models.Met_Material AggMaterial)
        {
            DateTime endOfMonth = MOO.Dates.LastDayOfMonth(MonthDate);
            var ftgStep2 = MOO.DAL.ToLive.Services.Met_Feed_To_GrateSvc.Get(endOfMonth, 2, AggMaterial);
            var ftgStep3 = MOO.DAL.ToLive.Services.Met_Feed_To_GrateSvc.Get(endOfMonth, 3, AggMaterial);
            decimal ftgTotal = (ftgStep2 == null ? 0 : ftgStep2.F_T_G.GetValueOrDefault(0)) +
                                (ftgStep3 == null ? 0 : ftgStep3.F_T_G.GetValueOrDefault(0));  //Total feed to grate for plant
            return ftgTotal;
        }

        /// <summary>
        /// Gets the ERP Messages for the Agglomerator Month End Production
        /// </summary>
        /// <param name="MonthDate">Can be any day in the month where we are getting the production month total</param>
        /// <param name="AggMaterial">Agglomerator material (Flux or Acid)</param>
        /// <returns></returns>
        /// <remarks>This message will include a month total for each line production as well as consumption for Concentrate and Limestone (If Flux).  Consumption records are in the RAW records</remarks>
        public static List<ERPMessage> GetAggProdLineMonthMessages(DateTime MonthDate, ToLive.Models.Met_Material AggMaterial)
        {
            DateTime endOfMonth = MOO.Dates.LastDayOfMonth(MonthDate);
            List<ERPMessage> retVal = new();
            var data = GetAggProdLineMonthValues(endOfMonth, AggMaterial);
            Material erpMat = AggMaterial == ToLive.Models.Met_Material.Flux ? Material.FPellet : Material.APellet;

            decimal stepConcTotals = data.Sum(x => x.Value);  //total tons for each step, this will be used to distribute the conc consumption
            decimal ftgTotal = GetAggMonthConcConsumption(MonthDate,AggMaterial);  //Total feed to grate for plant

            //need to also add Limestone consumption to the records, if material is acid, then just set it to zero as we will not send lime consumption
            decimal limeAdjustedTotal = AggMaterial == ToLive.Models.Met_Material.Flux ?  GetLimestoneMonthConsumption(endOfMonth) : 0;


            foreach (var lne in data)
            {
                List<RAWRec> rawRecs = new();
                string prodUnit = GetProductionUnitAggLine(lne.Line.Value, AggMaterial);
                MSTRec mst = new(MOO.Plant.Minntac, lne.Shift_Date, lne.Shift_Date, TimePeriod.Monthly);
                MPARec mpa = new(prodUnit, ProcessCode.XAG, erpMat, Activity.AGGLOMERATOR,
                                Math.Round(lne.Value, 2, MidpointRounding.AwayFromZero).ToString(), "LT", "ACT");
                //equally distribute conc consumption across alll lines per step
                decimal concConsumption = stepConcTotals > 0 ?
                                            Math.Round((lne.Value / stepConcTotals) * ftgTotal, 2, MidpointRounding.AwayFromZero) : 0;
                rawRecs.Add(new RAWRec(Material.Concentrate, concConsumption.ToString(), "LT", "ACT"));
                if (AggMaterial == ToLive.Models.Met_Material.Flux)
                {

                    //lime will be adjusted over all lines
                    decimal limeConsumption = stepConcTotals > 0 ?
                                                    Math.Round((lne.Value / stepConcTotals) * limeAdjustedTotal, 2, MidpointRounding.AwayFromZero) : 0;

                    rawRecs.Add(new RAWRec(Material.Limestone, limeConsumption.ToString(), "LT", "ACT"));
                }

                retVal.Add(new ERPMessage($"Agglomerator Month End Production/Consumption - {AggMaterial}",mst, mpa, rawRecs));
            }

            return retVal;
        }




        /// <summary>
        /// function used in lambda expression of GetAggProdLineDayMessage to convert the production unit
        /// </summary>
        /// <param name="Line"></param>
        /// <param name="AggMaterial"></param>
        /// <returns></returns>
        internal static string GetProductionUnitAggLine(int Line, ToLive.Models.Met_Material AggMaterial)
        {
            //example for line 3 is A12F3, Line 6 is AG3F6
            string step = Line <= 5 ? "12" : "G3";
            string mat = AggMaterial == ToLive.Models.Met_Material.Flux ? "F" : "A";
            return $"A{step}{mat}{Line}";
        }





        /// <summary>
        /// gets the Agglomerator Forecast Values for each step/day and material
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="AggMaterial"></param>
        /// <returns></returns>
        public static List<ERPValues> GetAggForecastDayValues(DateTime StartDate, DateTime EndDate, byte StepNbr, ToLive.Models.Met_Material AggMaterial)
        {
            int metricId;
            //get the metric id based on the Agg Step and the Material
            if (StepNbr < 3)
            {
                if (AggMaterial == ToLive.Models.Met_Material.Acid)
                    metricId = METRIC_ID_AGG2_ACID_FORECAST;
                else
                    metricId = METRIC_ID_AGG2_FLUX_FORECAST;
            }
            else
            {
                if (AggMaterial == ToLive.Models.Met_Material.Acid)
                    metricId = METRIC_ID_AGG3_ACID_FORECAST;
                else
                    metricId = METRIC_ID_AGG3_FLUX_FORECAST;
            }
            return ERPValues.GetMetricValuesByShiftDate(metricId, StartDate, EndDate);

        }

        /// <summary>
        /// Gets the ERP Messages for the Agglomerator Step Forecast per day
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPMessage> GetAggForecastDayMessages(DateTime StartDate, DateTime EndDate, byte StepNbr, ToLive.Models.Met_Material AggMaterial)
        {
            var data = GetAggForecastDayValues(StartDate, EndDate, StepNbr, AggMaterial);
            string prodUnit;
            //get the metric id based on the Agg Step and the Material
            if (StepNbr < 3)
            {
                if (AggMaterial == ToLive.Models.Met_Material.Acid)
                    prodUnit = "A12AE";
                else
                    prodUnit = "A12FE";
            }
            else
            {
                if (AggMaterial == ToLive.Models.Met_Material.Acid)
                    prodUnit = "AG3AE";
                else
                    prodUnit = "AG3FE";
            }


            Material erpMat = AggMaterial == ToLive.Models.Met_Material.Flux ? Material.FPellet : Material.APellet;
            return ERPMessage.DataValuesToERPMessages($"Agglomerator {AggMaterial} Forecast",data, Plant.Minntac, TimePeriod.Daily, ProcessCode.XAG,
                                    erpMat, Activity.AGGLOMERATOR, 2, "LT", "ACT", prodUnit);
        }



    }





}
