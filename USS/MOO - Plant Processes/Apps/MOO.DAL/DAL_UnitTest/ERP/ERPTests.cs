using MOO.DAL.ERP.Models;
using MOO.DAL.ERP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace DAL_UnitTest.ERP
{
    public class ERPTests:BaseTestClass
    {

        [Fact]
        public void TestNewDR()
        {
            DateTime StartDate = DateTime.Parse("5/14/2024");
            DateTime EndDate = DateTime.Parse("5/14/2024");
            var DRPell = KeetacAggSvc.GetPelletProductionMessages(StartDate, EndDate, MOO.DAL.Core.Enums.KTCPelletType.DRPellet);
            var drPellStr = DRPell[0].GetERPMessageString();
            Assert.NotEmpty(DRPell);

            var drCon = KeetacConcSvc.GetDRConcProductionMessages(StartDate, EndDate);
            var drConStr = drCon[0].GetERPMessageString();
            Assert.NotEmpty(drCon);

            var drPellMonth = KeetacAggSvc.GetDRPellProdMonthMessages(StartDate);
            var drPellMonthStr = drPellMonth.GetERPMessageString();
            Assert.NotNull(drPellMonthStr);

            var drConMonth = KeetacConcSvc.GetDRConcProductionMonthMessage(StartDate);
            var drConMonthStr = drConMonth.GetERPMessageString();
            Assert.NotNull(drPellMonthStr);



        }

        [Fact]
        public void TestMinntacConc()
        {
            DateTime StartDate = DateTime.Parse("1/1/2020");
            DateTime EndDate = DateTime.Parse("1/31/2020");

            var concProdDay = MOO.DAL.ERP.Services.MinntacConcSvc.GetConcProdLineDayValues(StartDate, EndDate);
            Assert.NotEmpty(concProdDay);

            var concProdDayMsgs = MOO.DAL.ERP.Services.MinntacConcSvc.GetConcProdLineDayMessages(StartDate, EndDate);
            Assert.NotEmpty(concProdDayMsgs);

            var concFore = MOO.DAL.ERP.Services.MinntacConcSvc.GetConcForecastDayValues(StartDate, EndDate);
            Assert.NotEmpty(concFore);

            var crushFore = MOO.DAL.ERP.Services.MinntacConcSvc.GetCrusherForecastValues(StartDate, EndDate);
            Assert.NotEmpty(crushFore);

            var RMFTotal = MOO.DAL.ERP.Services.MinntacConcSvc.GetConcRMFDayValues(StartDate, EndDate);
            Assert.NotEmpty(RMFTotal);

        }

        [Fact]
        public void TestMinntacAgg()
        {
            DateTime StartDate = DateTime.Parse("1/1/2020");
            DateTime EndDate = DateTime.Parse("1/31/2020");

            var aggLineDay = MOO.DAL.ERP.Services.MinntacAggSvc.GetAggProdLineDayValues(StartDate, EndDate, MOO.DAL.ToLive.Models.Met_Material.Flux);
            Assert.NotEmpty(aggLineDay);

            var agglLineDayMsg = MOO.DAL.ERP.Services.MinntacAggSvc.GetAggProdLineDayMessages(StartDate, EndDate, MOO.DAL.ToLive.Models.Met_Material.Flux);
            Assert.NotEmpty(agglLineDayMsg);

            var AggStepFore = MOO.DAL.ERP.Services.MinntacAggSvc.GetAggForecastDayValues(StartDate, EndDate, 2, MOO.DAL.ToLive.Models.Met_Material.Flux);
            Assert.NotEmpty(AggStepFore);

            var agglStepForeMsg = MOO.DAL.ERP.Services.MinntacAggSvc.GetAggForecastDayMessages(StartDate, EndDate, 2, MOO.DAL.ToLive.Models.Met_Material.Flux);
            Assert.NotEmpty(agglStepForeMsg);
        }

        [Fact]
        public void TestMinntacCrusher()
        {
            DateTime StartDate = DateTime.Parse("1/1/2020");
            DateTime EndDate = DateTime.Parse("1/31/2020");

            var secCrsh = MOO.DAL.ERP.Services.MinntacCrushSvc.GetSecondaryCrushLineValues(StartDate, EndDate);
            Assert.NotEmpty(secCrsh);

            var secCrshMsg = MOO.DAL.ERP.Services.MinntacCrushSvc.GetSecondaryCrushLineMessages(StartDate, EndDate);
            Assert.NotEmpty(secCrshMsg);

            var terCrsh = MOO.DAL.ERP.Services.MinntacCrushSvc.GetTertiaryCrushLineValues(StartDate, EndDate);
            Assert.NotEmpty(terCrsh);

            var terCrshMsg = MOO.DAL.ERP.Services.MinntacCrushSvc.GetTertiaryCrushLineMessages(StartDate, EndDate);
            Assert.NotEmpty(terCrshMsg);

            var rmf = MOO.DAL.ERP.Services.MinntacCrushSvc.GetRMFValues(StartDate, EndDate);
            Assert.NotEmpty(rmf);

            var rmfMsg = MOO.DAL.ERP.Services.MinntacCrushSvc.GetRMFMessages(StartDate, EndDate);
            Assert.NotEmpty(rmfMsg);

        }
        [Fact]
        public void TestMinntacMine()
        {
            DateTime StartDate = DateTime.Parse("4/1/2024");
            DateTime EndDate = DateTime.Parse("4/30/2024");

            var monthEnd = MinntacMineSvc.GetMineMonthProductionMessage(StartDate);

            var Crushed = MOO.DAL.ERP.Services.MinntacMineSvc.GetMineProductionValues(StartDate, EndDate);
            Assert.NotEmpty(Crushed);

            var crshMsg = MOO.DAL.ERP.Services.MinntacMineSvc.GetMineProductionMessages(StartDate, EndDate);
            Assert.NotEmpty(crshMsg);

            var crshWste = MOO.DAL.ERP.Services.MinntacMineSvc.GetMineWasteValues(StartDate, EndDate);
            Assert.NotEmpty(crshWste);

            var crshWsteMsg = MOO.DAL.ERP.Services.MinntacMineSvc.GetMineWasteMessages(StartDate, EndDate);
            Assert.NotEmpty(crshWsteMsg);

            var crshFore = MOO.DAL.ERP.Services.MinntacMineSvc.GetCrushedProductionForecastValues(StartDate, EndDate);
            Assert.NotEmpty(crshFore);

            var crshForeMsg = MOO.DAL.ERP.Services.MinntacMineSvc.GetCrushedProductionForecastMessages(StartDate, EndDate);
            Assert.NotEmpty(crshForeMsg);

            var wsteFore = MOO.DAL.ERP.Services.MinntacMineSvc.GetWasteForecastValues(StartDate, EndDate);
            Assert.NotEmpty(wsteFore);

            var wsteForeMsg = MOO.DAL.ERP.Services.MinntacMineSvc.GetWasteForecastMessages(StartDate, EndDate);
            Assert.NotEmpty(wsteForeMsg);

        }

        [Fact]
        public void TestKeetacAgg()
        {
            DateTime StartDate = DateTime.Parse("1/1/2020");
            DateTime EndDate = DateTime.Parse("1/31/2020");

            var ball = KeetacAggSvc.GetBallingValues(StartDate, EndDate);
            Assert.NotEmpty(ball);

            var fore = KeetacAggSvc.GetPelletForecastValues(StartDate, EndDate);
            Assert.NotEmpty(fore);

            var prod = KeetacAggSvc.GetPelletProductionValues(StartDate, EndDate, MOO.DAL.Core.Enums.KTCPelletType.BlastFurnace);
            Assert.NotEmpty(prod);

            var DRPell = KeetacAggSvc.GetPelletProductionMessages(StartDate, EndDate, MOO.DAL.Core.Enums.KTCPelletType.DRPellet);
            Assert.NotEmpty(DRPell);
        }

        [Fact]
        public void TestKeetacConc()
        {
            DateTime StartDate = DateTime.Parse("1/1/2020");
            DateTime EndDate = DateTime.Parse("1/31/2020");

            var lineFd = KeetacConcSvc.GetConcPrimaryLineValues(StartDate, EndDate);
            Assert.NotEmpty(lineFd);

            var fore = KeetacConcSvc.GetConcentrateForecastValues(StartDate, EndDate);
            Assert.NotEmpty(fore);

            var fdFore = KeetacConcSvc.GetMillFeedForecastValues(StartDate, EndDate);
            Assert.NotEmpty(fdFore);

            var prod = KeetacConcSvc.GetConcProductionValues(StartDate, EndDate);
            Assert.NotEmpty(prod);

            var millFd = KeetacConcSvc.GetConcMillFeedMessages(StartDate, EndDate);
            Assert.NotEmpty(millFd);

        }

        [Fact]
        public void TestKeetacMine()
        {
            DateTime StartDate = DateTime.Parse("1/1/2021");
            DateTime EndDate = DateTime.Parse("1/31/2021");

            var crush = KeetacMineSvc.GetCrusherValues(StartDate, EndDate);
            Assert.NotEmpty(crush);

            var stock = KeetacMineSvc.GetCrushStockedValues(StartDate, EndDate);
            Assert.NotEmpty(stock);

            var prodFore = KeetacMineSvc.GetCrushedProductionForecastValues(StartDate, EndDate);
            Assert.NotEmpty(prodFore);

            var wsteFore = KeetacMineSvc.GetWasteForecastValues(StartDate, EndDate);
            Assert.NotEmpty(wsteFore);

            var waste = KeetacMineSvc.GetWasteValues(StartDate, EndDate);
            Assert.NotEmpty(waste);

            var crude = KeetacMineSvc.GetTotalCrudeMinedValues(StartDate, EndDate);
            Assert.NotEmpty(crude);
        }


        [Fact]
        public void MinntacMonthEndTests()
        {
            DateTime StartDate = DateTime.Parse("1/1/2024");
            DateTime EndDate = DateTime.Parse("1/31/2024");


            var lime = MinntacAggSvc.GetLimestoneMonthConsumption(StartDate);
            //Test getting lease values (Crusher Consumption)
            var crushed = MinntacMineSvc.GetCrusherConsumptionValues(StartDate);
            Assert.NotEmpty(crushed);
            var totTons = crushed.Sum(x => x.Value);


            var leaseGrp = (from crush in crushed
                            group crush by new { crush.PropertyCode, crush.LeaseCode, crush.LeaseName, crush.LessorGroupCode, crush.LessorGroupName } into grp
                            orderby grp.Sum(x => x.Value)
                            select new ERPLeaseValues()
                            {
                                Shift_Date = EndDate,
                                PropertyCode = grp.Key.PropertyCode,
                                LeaseCode = grp.Key.LeaseCode,
                                LeaseName = grp.Key.LeaseName,
                                LessorGroupCode = grp.Key.LessorGroupCode,
                                LessorGroupName = grp.Key.LessorGroupName,
                                Value = grp.Sum(x => x.Value)
                            }
                          ).ToList();
            Assert.NotEmpty(leaseGrp);


            //var crushMsg = MinntacMineSvc.GetCrusherConsumptionMessages(StartDate).GetERPMessageString();

            //Minntac Agglomerator Test
            var agg = MinntacAggSvc.GetAggProdLineMonthMessages(StartDate, MOO.DAL.ToLive.Models.Met_Material.Acid);
            StringBuilder msg = new StringBuilder();
            foreach (var item in agg)
            {
                msg.AppendLine(item.GetERPMessageString());
            }
            Assert.NotNull(msg.ToString());
            Assert.NotEmpty(agg);



            //Minntac Concentrator Test
            var cncVal = MinntacConcSvc.GetConcProductionMonthTotal(StartDate);
            var cncMsg = MinntacConcSvc.GetConcProductionMonthMessage(StartDate);
            string cncStr = cncMsg.GetERPMessageString();
            Assert.NotNull(cncMsg);

        }
        [Fact]
        public void TestERPProdTable()
        {
            var ERP = MOO.DAL.ToLive.Services.ERP_Production_ConsumptionSvc.Get(MOO.Plant.Minntac, DateTime.Parse("1/1/2020"));
            Assert.NotNull(ERP);
            ERP.ERP_Message = "TESTING";
            MOO.DAL.ToLive.Services.ERP_Production_ConsumptionSvc.Update(ERP);

            //change the date and insert
            ERP.Month_Date = DateTime.Parse("1/1/2099");
            ERP.DRCon_Cons = 1234;
            ERP.DRPell_Cons = 5678;
            var recs = MOO.DAL.ToLive.Services.ERP_Production_ConsumptionSvc.Insert(ERP);
            Assert.Equal(1, recs);
            //delete the new testing one
            recs = MOO.DAL.ToLive.Services.ERP_Production_ConsumptionSvc.Delete(ERP);
            Assert.Equal(1, recs);


        }

        [Fact]
        public void KeetacMonthEndTests()
        {
            DateTime StartDate = DateTime.Parse("1/1/2024");
            DateTime EndDate = DateTime.Parse("1/31/2024");
            var erpLeases = MOO.DAL.ToLive.Services.ERP_LeasesSvc.GetAllByMonth(MOO.Plant.Keetac, DateTime.Parse("1/1/2020"));


            var concConcsume = KeetacConcSvc.GetMonthConcConsumption(StartDate);
            var concProd = KeetacConcSvc.GetMonthConcProduction(StartDate);
            var drConcProd = KeetacConcSvc.GetMonthDRConcProduction(StartDate);
            var concMsg = KeetacConcSvc.GetConcProductionMonthMessage(StartDate).GetERPMessageString();


            var drConcMsg = KeetacConcSvc.GetDRConcProductionMonthMessage(StartDate).GetERPMessageString();
            int i = 0;
        }
    }
}
