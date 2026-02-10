using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.Core;

namespace DAL_UnitTest.Core
{
    public class ForecastMetricsTest : BaseTestClass
    {
        [Fact]
        public void GetAll()
        {
            var aa = DAL.Services.Forecast_MetricsSvc.GetAll();
            Assert.NotEmpty(aa);
        }

        [Fact]
        public void InsUpdDel()
        {
            DAL.Models.Forecast_Metrics a = new()
            {
                Name = "Brian Test",
                Plant = MOO.Plant.Minntac,
                CoreId = 111,
                Uom = "test",
                Active = true
            };
            DAL.Services.Forecast_MetricsSvc.Insert(a);

            a.Name = "Brian Test2";
            DAL.Services.Forecast_MetricsSvc.Update(a);

            DAL.Services.Forecast_MetricsSvc.Delete(a);

        }
    }
}
