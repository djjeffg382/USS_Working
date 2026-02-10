using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.MinVu;

namespace DAL_UnitTest.Drill
{
    public class MinvuTests : BaseTestClass
    {
        [Fact]
        public void TestPlan()
        {
            var h = DAL.Services.ForecastSvc.GetByDateRange(MOO.Plant.Minntac, DateTime.Today.AddDays(-10), 1, DateTime.Today.AddDays(10), 2, DAL.Models.Forecast.TagMtcOre);
            Assert.NotNull(h);
        }

    }
}
