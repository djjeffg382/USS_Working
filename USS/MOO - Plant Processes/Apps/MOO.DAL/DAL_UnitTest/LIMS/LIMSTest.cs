using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.LIMS;

namespace DAL_UnitTest.LIMS
{
    public class LIMSTest
    {
        [Fact]
        public void TestLocation()
        {

            var mineList = DAL.Services.LocationSvc.GetAll("M_MINE");
            Assert.NotEmpty(mineList);

            var locList = DAL.Services.LocationSvc.GetAll();
            Assert.NotEmpty(locList);

            var loc = DAL.Services.LocationSvc.Get("K_4100");
            Assert.NotNull(loc);
            loc = DAL.Services.LocationSvc.Get("Minntac");
            Assert.NotNull(loc);

        }

        [Fact]
        public void TestPhrase()
        {
            var p = DAL.Services.PhraseSvc.Get("SAMP_STAT", "U");
            Assert.NotNull(p);
            var pList = DAL.Services.PhraseSvc.GetAll("SAMP_STAT");
            Assert.NotEmpty(pList);
        }

        [Fact]
        public void SamplePointTest()
        {
            var spList = DAL.Services.Sample_PointSvc.GetAll();
            Assert.NotEmpty(spList);
            spList = DAL.Services.Sample_PointSvc.GetByLocation("M_0267");
            Assert.NotEmpty(spList);
            var sp = DAL.Services.Sample_PointSvc.Get("M0108_20");
            Assert.NotNull(sp);
        }

        [Fact]
        public void SampleTest()
        {
            var s = DAL.Services.SampleSvc.Get(571402);
            Assert.NotNull(s);
        }


        [Fact]
        public void TestOilResults()
        {
            List<DAL.Models.OilResults> o;
            o = DAL.Services.OilResultsSvc.GetAll(DateTime.Today.AddDays(-60), DateTime.Today);
            Assert.NotEmpty(o);
            o = DAL.Services.OilResultsSvc.GetAll(DateTime.Today.AddDays(-60), DateTime.Today, Location:"M_MINE");
            Assert.NotEmpty(o);

            //o = DAL.Services.OilResultsSvc.GetAll(DateTime.Today.AddDays(-600), DateTime.Today,LimsBatchId:11035);
            //Assert.NotEmpty(o);

        }
        [Fact]
        public void TestWaterResults()
        {
            List<DAL.Models.WaterResults> w;
            w = DAL.Services.WaterResultsSvc.GetAll(DateTime.Now.AddDays(-120), DateTime.Now,"M_ADBLD");
            Assert.NotEmpty(w);
        }

    }
}
