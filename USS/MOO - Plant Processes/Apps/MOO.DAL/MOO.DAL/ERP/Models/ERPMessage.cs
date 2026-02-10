using MOO.DAL.ERP.Enums;
using MOO.DAL.ERP.Services;
using System;
using System.Collections.Generic;
using System.Text;
using MOO.Enums.Extension;

namespace MOO.DAL.ERP.Models
{
    public sealed class ERPMessage
    {
        public MPARec MPARecord { get; set; }
        public MSTRec MSTRecord { get; set; }
        public List<RAWRec> RawRecords { get; set; } = new();
        /// <summary>
        /// This will be used for troubleshooting as a description label for what this is
        /// </summary>
        public string Label { get; set; }



     
        public ERPMessage(string Label, MSTRec MSTRecord, MPARec MPARecord, List<RAWRec> RawRec)
        {
            this.Label = Label;
            this.MPARecord = MPARecord;
            this.MSTRecord = MSTRecord;
            this.RawRecords = RawRec;
        }

        /// <summary>
        /// Returns the ERP Message as a string
        /// </summary>
        /// <returns></returns>
        public string GetERPMessageString()
        {
            StringBuilder sb = new();
            sb.Append(MSTRecord.StartOfRecord.PadRight(4));
            sb.Append(MSTRecord.InterfaceType.PadRight(30));
            sb.Append(MSTRecord.InterfaceLayout.PadRight(30));
            sb.Append(MSTRecord.InterfaceVersion.PadRight(4));
            sb.Append(MSTRecord.InterfaceTimestamp.ToString("yyyy-MM-ddTHH:mm:ss.ffffff-05:00"));
            sb.Append(MSTRecord.InterfaceEnvironment.PadRight(4));
            sb.Append(MSTRecord.ActionCode.PadRight(1));
            sb.Append(MSTRecord.DataSource.PadRight(30));
            sb.Append(MSTRecord.ProgramId.PadRight(128));
            sb.Append(MSTRecord.UserId.PadRight(30));
            sb.Append(MSTRecord.TransactionId.PadRight(40));
            sb.Append(MSTRecord.TransactionQualifier.PadRight(30));
            sb.Append(MSTRecord.PlantId.PadRight(3));
            sb.Append(MSTRecord.PeriodStartDate.ToString("yyyy-MM-dd").PadRight(32));
            sb.Append(MSTRecord.PeriodStopDate.ToString("yyyy-MM-dd").PadRight(32));
            sb.Append(
                    (MSTRecord.PeriodUnitOfMeasure == TimePeriod.Daily ? "D" : "M")
                    .PadRight(3));
            sb.Append(MSTRecord.Turn.PadRight(1));
            //add a carrot for end of MST Record
            sb.Append('^');
            sb.Append(MPARecord.StartOfRecord.PadRight(4));
            sb.Append(MPARecord.ProductionUnit.PadRight(8));
            sb.Append(MPARecord.ProcessCode.ToString().PadRight(3));
            sb.Append(MPARecord.MaterialLotIdentity.PadRight(40));
            sb.Append(MPARecord.MaterialName.GetDisplay().Name.PadRight(30));
            sb.Append(MPARecord.ActivityName.ToString().PadRight(20));
            sb.Append(MPARecord.Weight.PadLeft(12, '0'));
            sb.Append(MPARecord.WeightUOM.PadRight(3));
            sb.Append(MPARecord.WeightMeasurementType.PadRight(4));
            //add a carrot for end of MPA Record
            sb.Append('^');
            if(RawRecords != null)
            {
                foreach (var raw in RawRecords)
                {
                    sb.Append(raw.StartOfRecord.PadRight(4));
                    sb.Append(raw.RawMaterialName.GetDisplay().Name.PadRight(30));
                    sb.Append(raw.RawMaterialWeight.PadLeft(12, '0'));
                    sb.Append(raw.RawMaterialWeightUOM.PadRight(3));
                    sb.Append(raw.RawMaterialWeightMeasurementType.PadRight(4));
                    sb.Append(raw.PropertyCode.PadRight(10));
                    sb.Append(raw.PropertyGroupCode.PadRight(24));
                    sb.Append(raw.LeaseCode.PadRight(25));
                    sb.Append(raw.LeaseCodeName.PadRight(50));
                    sb.Append(raw.LessorCode.ToString().PadRight(25));
                    sb.Append(raw.LessorGroupCodeName.PadRight(50));

                    //add a carrot for end of RAW Record
                    sb.Append('^');
                }
            }
            //add a carrot for end of whole Record
            sb.Append('^');

            return sb.ToString();
        }




        /// <summary>
        /// Creates an ERP Message list from a list of ERP Values
        /// </summary>
        /// <param name="Label">A description that will be added to the ERP Message for troubleshooting</param>
        /// <param name="ERPVals">List of ERP Values to convert into messages</param>
        /// <param name="Plant">Minntac/Keetac</param>
        /// <param name="Period">Time Period of the message</param>
        /// <param name="ProcCode">Process Code</param>
        /// <param name="ERPMaterial">Message Material</param>
        /// <param name="ERPActivity">ERP Activity type</param>
        /// <param name="DecimalPlaces">Number of decimal places to show in the value</param>
        /// <param name="Units">Units of Measure</param>
        /// <param name="Measurement">ACT or CALC</param>
        /// <param name="LineToProdUnit">Lambda expression on how to calculate the ProductionUnit</param>
        /// <returns></returns>
        public static List<ERPMessage> DataValuesToERPMessages(string Label, List<ERPValues> ERPVals,
                                                        MOO.Plant Plant,
                                                        TimePeriod Period,
                                                        ProcessCode ProcCode,
                                                        Material ERPMaterial,
                                                        Activity ERPActivity,
                                                        int DecimalPlaces,
                                                        string Units,
                                                        string Measurement,
                                                        Func<int, string> LineToProdUnit)
        {
            List<ERPMessage> retVal = new();
            string prodUnit, decFormat;

            
            foreach (var val in ERPVals)
            {
                if (DecimalPlaces == 0 || val.Value == 0)
                    decFormat = "{0:0}";
                else
                    decFormat = "{0:0." + "".PadRight(DecimalPlaces, '0') + "}";

                prodUnit = LineToProdUnit(val.Line.GetValueOrDefault(0));
                MSTRec mst = new(Plant, val.Shift_Date, val.Shift_Date, Period);
                MPARec mpa = new(prodUnit, ProcCode, ERPMaterial, ERPActivity,
                                String.Format(decFormat, Math.Round(val.Value, DecimalPlaces, MidpointRounding.AwayFromZero)), 
                                Units, Measurement);
                retVal.Add(new ERPMessage(Label, mst, mpa, new List<RAWRec>()));
            }
            return retVal;
        }


        /// <summary>
        /// Creates an ERP Message list from a list of ERP Values
        /// </summary>
        /// <param name="ERPVals">List of ERP Values to convert into messages</param>
        /// <param name="Plant">Minntac/Keetac</param>
        /// <param name="Period">Time Period of the message</param>
        /// <param name="ProcCode">Process Code</param>
        /// <param name="ERPMaterial">Message Material</param>
        /// <param name="ERPActivity">ERP Activity type</param>
        /// <param name="DecimalPlaces">Number of decimal places to show in the value</param>
        /// <param name="Units">Units of Measure</param>
        /// <param name="Measurement">ACT or CALC</param>
        /// <param name="ProductionUnit">Production unit</param>
        /// <returns></returns>
        public static List<ERPMessage> DataValuesToERPMessages(string Label, List<ERPValues> ERPVals,
                                                        MOO.Plant Plant,
                                                        TimePeriod Period,
                                                        ProcessCode ProcCode,
                                                        Material ERPMaterial,
                                                        Activity ERPActivity,
                                                        int DecimalPlaces,
                                                        string Units,
                                                        string Measurement,
                                                        string ProductionUnit)
        {
            return DataValuesToERPMessages(Label, ERPVals, Plant, Period, ProcCode, ERPMaterial, ERPActivity, DecimalPlaces, Units, Measurement, x => ProductionUnit);
        }

        /// <summary>
        /// Gets the daily messages for Minntac
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPMessage> GetMinntacDaily(DateTime StartDate, DateTime EndDate)
        {
            List<ERPMessage> retVal = new();

            //Minntac Agglomerator
            retVal.AddRange(MinntacAggSvc.GetAggProdLineDayMessages(StartDate, EndDate, MOO.DAL.ToLive.Models.Met_Material.Flux));
            retVal.AddRange(MinntacAggSvc.GetAggProdLineDayMessages(StartDate, EndDate, MOO.DAL.ToLive.Models.Met_Material.Acid));

            retVal.AddRange(MinntacAggSvc.GetAggForecastDayMessages(StartDate, EndDate, 2, MOO.DAL.ToLive.Models.Met_Material.Acid));
            retVal.AddRange(MinntacAggSvc.GetAggForecastDayMessages(StartDate, EndDate, 2, MOO.DAL.ToLive.Models.Met_Material.Flux));

            retVal.AddRange(MinntacAggSvc.GetAggForecastDayMessages(StartDate, EndDate, 3, MOO.DAL.ToLive.Models.Met_Material.Acid));
            retVal.AddRange(MinntacAggSvc.GetAggForecastDayMessages(StartDate, EndDate, 3, MOO.DAL.ToLive.Models.Met_Material.Flux));


            //Minntac Concentrator
            retVal.AddRange(MinntacConcSvc.GetConcProdLineDayMessages(StartDate, EndDate));
            retVal.AddRange(MinntacConcSvc.GetConcForecastDayMessages(StartDate, EndDate));

            retVal.AddRange(MinntacConcSvc.GetConcRMFDayMessages(StartDate, EndDate));
            retVal.AddRange(MinntacConcSvc.GetTotalConcProdDayMessages(StartDate, EndDate));


            //Minntac Crusher
            retVal.AddRange(MinntacCrushSvc.GetSecondaryCrushLineMessages(StartDate, EndDate));
            retVal.AddRange(MinntacCrushSvc.GetTertiaryCrushLineMessages(StartDate, EndDate));

            //Minntac Mine
            retVal.AddRange(MinntacMineSvc.GetCrushedProductionForecastMessages(StartDate, EndDate));
            retVal.AddRange(MinntacMineSvc.GetWasteForecastMessages(StartDate, EndDate));
            retVal.AddRange(MinntacMineSvc.GetMineProductionMessages(StartDate, EndDate));
            retVal.AddRange(MinntacMineSvc.GetMineWasteMessages(StartDate, EndDate));

            retVal.AddRange(MinntacCrushSvc.GetRMFMessages(StartDate, EndDate));
            return retVal;

        }

        /// <summary>
        /// Gets the daily messages for Minntac
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPMessage> GetMinntacMonthly(DateTime ERPMonth)
        {
            DateTime startDate = MOO.Dates.FirstDayOfMonth(ERPMonth);
            DateTime endDate = MOO.Dates.LastDayOfMonth(ERPMonth);

            List<ERPMessage> retVal = new();

            //Minntac Agglomerator
            retVal.AddRange(MinntacAggSvc.GetAggProdLineDayMessages(startDate, endDate, MOO.DAL.ToLive.Models.Met_Material.Flux));
            retVal.AddRange(MinntacAggSvc.GetAggProdLineMonthMessages(ERPMonth, MOO.DAL.ToLive.Models.Met_Material.Flux));  //Month end Flux Production/consumption

            retVal.AddRange(MinntacAggSvc.GetAggProdLineDayMessages(startDate, endDate, MOO.DAL.ToLive.Models.Met_Material.Acid));
            retVal.AddRange(MinntacAggSvc.GetAggProdLineMonthMessages(ERPMonth, MOO.DAL.ToLive.Models.Met_Material.Acid)); //Month end Acid Production/consumption

            retVal.AddRange(MinntacAggSvc.GetAggForecastDayMessages(startDate, endDate, 2, MOO.DAL.ToLive.Models.Met_Material.Acid));
            retVal.AddRange(MinntacAggSvc.GetAggForecastDayMessages(startDate, endDate, 2, MOO.DAL.ToLive.Models.Met_Material.Flux));

            retVal.AddRange(MinntacAggSvc.GetAggForecastDayMessages(startDate, endDate, 3, MOO.DAL.ToLive.Models.Met_Material.Acid));
            retVal.AddRange(MinntacAggSvc.GetAggForecastDayMessages(startDate, endDate, 3, MOO.DAL.ToLive.Models.Met_Material.Flux));


            //Minntac Concentrator
            retVal.AddRange(MinntacConcSvc.GetConcProdLineDayMessages(startDate, endDate));
            retVal.AddRange(MinntacConcSvc.GetConcForecastDayMessages(startDate, endDate));

            retVal.AddRange(MinntacConcSvc.GetConcRMFDayMessages(startDate, endDate));
            retVal.AddRange(MinntacConcSvc.GetTotalConcProdDayMessages(startDate, endDate));
            retVal.Add(MinntacConcSvc.GetConcProductionMonthMessage(ERPMonth));  //Minntac Month End Concentrate production/consumption


            //Minntac Crusher
            retVal.AddRange(MinntacCrushSvc.GetSecondaryCrushLineMessages(startDate, endDate));
            retVal.AddRange(MinntacCrushSvc.GetTertiaryCrushLineMessages(startDate, endDate));

            //Minntac Mine
            retVal.AddRange(MinntacMineSvc.GetCrushedProductionForecastMessages(startDate, endDate));
            retVal.Add(MinntacMineSvc.GetMineMonthProductionMessage(ERPMonth));  //Month end crude production/consumption


            retVal.AddRange(MinntacMineSvc.GetWasteForecastMessages(startDate, endDate));
            retVal.AddRange(MinntacMineSvc.GetMineProductionMessages(startDate, endDate));
            retVal.AddRange(MinntacMineSvc.GetMineWasteMessages(startDate, endDate));

            retVal.AddRange(MinntacCrushSvc.GetRMFMessages(startDate, endDate));
            return retVal;

        }



        /// <summary>
        /// Gets the daily messages for Minntac
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPMessage> GetKeetacDaily(DateTime StartDate, DateTime EndDate)
        {
            List<ERPMessage> retVal = new();

            //Keetac Agglomerator
            retVal.AddRange(KeetacAggSvc.GetBallingMessages(StartDate, EndDate));
            retVal.AddRange(KeetacAggSvc.GetPelletForecastMessages(StartDate, EndDate));
            retVal.AddRange(KeetacAggSvc.GetPelletProductionMessages(StartDate, EndDate, Core.Enums.KTCPelletType.BlastFurnace));
            retVal.AddRange(KeetacAggSvc.GetPelletProductionMessages(StartDate, EndDate, Core.Enums.KTCPelletType.DRPellet));



            //Keetac Conc
            retVal.AddRange(KeetacConcSvc.GetConcPrimaryLineMessages(StartDate, EndDate));
            retVal.AddRange(KeetacConcSvc.GetConcentrateForecastMessages(StartDate, EndDate));
            retVal.AddRange(KeetacConcSvc.GetMillFeedForecastMessages(StartDate, EndDate));
            retVal.AddRange(KeetacConcSvc.GetConcProductionMessages(StartDate, EndDate));
            retVal.AddRange(KeetacConcSvc.GetDRConcProductionMessages(StartDate, EndDate));
            retVal.AddRange(KeetacConcSvc.GetConcMillFeedMessages(StartDate, EndDate));


            //Keetac Mine/Crusher
            retVal.AddRange(KeetacMineSvc.GetCrusherMessages(StartDate, EndDate));
            retVal.AddRange(KeetacMineSvc.GetCrushStockedMessages(StartDate, EndDate));
            retVal.AddRange(KeetacMineSvc.GetCrushedProductionForecastMessages(StartDate, EndDate));
            retVal.AddRange(KeetacMineSvc.GetWasteForecastMessages(StartDate, EndDate));
            retVal.AddRange(KeetacMineSvc.GetWasteMessages(StartDate, EndDate));
            retVal.AddRange(KeetacMineSvc.GetTotalCrudeMinedMessages(StartDate, EndDate));



            return retVal;

        }



        /// <summary>
        /// Gets the daily messages for Minntac
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPMessage> GetKeetacMonthly(DateTime ERPMonth)
        {
            DateTime startDate = MOO.Dates.FirstDayOfMonth(ERPMonth);
            DateTime endDate = MOO.Dates.LastDayOfMonth(ERPMonth);
            List<ERPMessage> retVal = new();

            //Keetac Agglomerator
            retVal.AddRange(KeetacAggSvc.GetBallingMessages(startDate, endDate));
            retVal.AddRange(KeetacAggSvc.GetPelletForecastMessages(startDate, endDate));
            retVal.AddRange(KeetacAggSvc.GetPelletProductionMessages(startDate, endDate, Core.Enums.KTCPelletType.BlastFurnace));
            retVal.AddRange(KeetacAggSvc.GetPelletProductionMessages(startDate, endDate, Core.Enums.KTCPelletType.DRPellet));
            retVal.Add(KeetacAggSvc.GetK1PellProdMonthMessages(ERPMonth));   //Monthly Production/consumption message
            retVal.Add(KeetacAggSvc.GetDRPellProdMonthMessages(ERPMonth));   //Monthly Production/consumption message



            //Keetac Conc
            retVal.AddRange(KeetacConcSvc.GetConcPrimaryLineMessages(startDate, endDate));
            retVal.AddRange(KeetacConcSvc.GetConcentrateForecastMessages(startDate, endDate));
            retVal.AddRange(KeetacConcSvc.GetMillFeedForecastMessages(startDate, endDate));
            retVal.AddRange(KeetacConcSvc.GetConcProductionMessages(startDate, endDate));
            retVal.AddRange(KeetacConcSvc.GetDRConcProductionMessages(startDate, endDate));
            retVal.AddRange(KeetacConcSvc.GetConcMillFeedMessages(startDate, endDate));
            retVal.Add(KeetacConcSvc.GetConcProductionMonthMessage(ERPMonth));   //Monthly Production/consumption message
            retVal.Add(KeetacConcSvc.GetDRConcProductionMonthMessage(ERPMonth));   //Monthly Production/consumption message



            //Keetac Mine/Crusher
            retVal.AddRange(KeetacMineSvc.GetCrusherMessages(startDate, endDate));
            retVal.AddRange(KeetacMineSvc.GetCrushStockedMessages(startDate, endDate));
            retVal.AddRange(KeetacMineSvc.GetCrushedProductionForecastMessages(startDate, endDate));
            retVal.AddRange(KeetacMineSvc.GetWasteForecastMessages(startDate, endDate));
            retVal.AddRange(KeetacMineSvc.GetWasteMessages(startDate, endDate));
            retVal.AddRange(KeetacMineSvc.GetTotalCrudeMinedMessages(startDate, endDate));
            retVal.Add(KeetacMineSvc.GetMineProductionMonthMessage(ERPMonth));   //Monthly Production/consumption message



            return retVal;

        }
    }
}
