using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public class MOO_WeatherTest : BaseTestClass
    {
        [Fact]
        public void TestMOOWeather()
        {
            var latest = DAL.Services.MOO_WeatherSvc.GetLatest(DAL.Models.MOO_Weather.Weather_City.Hibbing);
            Assert.NotNull(latest);

            var list = DAL.Services.MOO_WeatherSvc.GetByDateRange(DAL.Models.MOO_Weather.Weather_City.Eveleth, DateTime.Now.AddYears(-4).AddDays(-10), DateTime.Now.AddYears(-4));
            Assert.NotEmpty(list);

            latest.Thedate = DateTime.Now;
            Assert.Equal(1, DAL.Services.MOO_WeatherSvc.Insert(latest));
            latest.Temperature = 900;
            Assert.Equal(1, DAL.Services.MOO_WeatherSvc.Update(latest));
            Assert.Equal(1, DAL.Services.MOO_WeatherSvc.Delete(latest));

        }
    }
}
