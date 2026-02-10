using MOO.Enums.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public class ConcAnalyticsTest : BaseTestClass
    {
        [Fact]
        public void TestConcAnRecovKPI()
        {
            var a = DAL.Services.Conc_Analytics_Flot_Recov_KPISvc.GetLatest();
            Assert.NotNull(a);
            
        }

        [Fact]
        public void TestConc_Analytics()
        {
            var a = DAL.Services.Conc_AnalyticsSvc.Get(DAL.Enums.Conc_Analytics_Group.Performance, 10);
            Assert.NotEmpty(a);

            var avgVal = DAL.Services.Conc_AnalyticsSvc.GetAvg(DAL.Enums.Conc_Analytics_Group.Performance, "Current Rod Mill Feed ltph", 3, 7, 4);
            Assert.True(avgVal > 0);
            var piv = DAL.Services.Conc_AnalyticsSvc.GetPivot(DAL.Enums.Conc_Analytics_Group.Performance, 3);
            Assert.NotNull(piv);
            Assert.NotEmpty(piv.Records);
        }
        [Fact]
        public void TestConcAnalyticsGeneral()
        {
            var a = DAL.Services.Conc_Analytics_GeneralSvc.Get(DAL.Enums.ConcAnalyticsGnrlGroup.Actionable_Parameters, 10);
            Assert.NotEmpty(a);
        }

        [Fact]
        public void TestConcConstraintSummary()
        {
            var a = DAL.Services.Conc_Analytics_GeneralSvc.GetConstraintSummary(10);
            Assert.NotEmpty(a);
        }

        [Fact]
        public void TestFloationPotential()
        {
            var a = DAL.Services.Conc_Analytics_GeneralSvc.GetFloataionPotential();
            Assert.NotEmpty(a);
        }


        [Fact]
        public void TestAnomolies()
        {
            var piv = DAL.Services.Conc_AnalyticsSvc.GetPivot(DAL.Enums.Conc_Analytics_Group.Floatation_Anomalies, 0);
            Assert.NotNull(piv);
            Assert.NotEmpty(piv.Records);
        }
        [Fact]
        public void TestRecovery()
        {
            var recovery = DAL.Services.Conc_Analytics_RecoverySvc.GetLatest();
            Assert.NotNull(recovery);
            var recovery2 = DAL.Services.Conc_Analytics_RecoverySvc.GetByDateRangeWeightedAvg(DateTime.Today.AddDays(-5), DateTime.Today);
            Assert.NotNull(recovery2);
            recovery2 = DAL.Services.Conc_Analytics_RecoverySvc.GetByDateRangeWeightedAvg(DateTime.Today.AddDays(-5), DateTime.Today, true);
            Assert.NotNull(recovery2);
            var recovery3 = DAL.Services.Conc_Analytics_RecoverySvc.GetByDateRange(DateTime.Today.AddDays(-5), DateTime.Today);
            Assert.NotNull(recovery3);
        }
        [Fact]
        public void GetConcAnalyticsUplift()
        {
            var ca = DAL.Services.Conc_Analytics_UpliftSvc.GetLatest();
            Assert.NotEmpty(ca);
            var caList = DAL.Services.Conc_Analytics_UpliftSvc.GetAll(DateTime.Today.AddDays(-10), DateTime.Today);
            Assert.NotEmpty(caList);

            caList = DAL.Services.Conc_Analytics_UpliftSvc.GetAll(DateTime.Today.AddDays(-10), DateTime.Today, DAL.Models.Conc_Analytics_Uplift.Level.Line);
            Assert.NotEmpty(caList);

            caList = DAL.Services.Conc_Analytics_UpliftSvc.GetAll(DateTime.Today.AddDays(-10), DateTime.Today, 
                        DAL.Models.Conc_Analytics_Uplift.Level.Line, DAL.Models.Conc_Analytics_Uplift.UpliftFrequency.Today);
            Assert.NotEmpty(caList);
        }

    }
}
