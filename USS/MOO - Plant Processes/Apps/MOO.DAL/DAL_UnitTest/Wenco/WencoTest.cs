using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.Wenco;

namespace DAL_UnitTest.Wenco
{
    public class WencoTest : BaseTestClass
    {

        [Fact]
        public void TestHaulCycyleMaterialSummary()
        {
            DAL.Models.HaulCycleMaterialSummary hs = DAL.Services.HaulCycleMaterialSummarySvc.GetByShift(MOO.Plant.Minntac,DateTime.Today.AddDays(-1), 1);
            Assert.NotNull(hs);
            var hsList = DAL.Services.HaulCycleMaterialSummarySvc.GetByDateRange(MOO.Plant.Minntac, DateTime.Today.AddDays(-20), DateTime.Today.AddDays(1));
            Assert.NotEmpty(hsList);
        }

        [Fact]
        public void TestHours()
        {
            var h = DAL.Services.EquipHoursSummarySvc.GetByEquipTypeAndShift(MOO.Plant.Minntac, new string[] { "H" }, DateTime.Today, 1);
            Assert.NotNull(h);
        }

        [Fact]
        public void TestQuality()
        {
            var q = DAL.Services.HaulCycleQualitySummarySvc.GetByShift(MOO.Plant.Minntac, DateTime.Today, 1);
            Assert.NotNull(q);
            var His = q.Quality("HIS");
            //var Silica = q.Quality("SIO2");
            //var MagFe = q.Quality("MAGF");
            Assert.NotNull(His);
            var test = q.Quality("test123");
            Assert.Null(test);
        }

    }
}
