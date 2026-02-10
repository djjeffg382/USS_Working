using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public class AnomaliesTest
    {
        [Fact]
        public void TestAnomalies() {
            var anoms = DAL.Services.AnomaliesSvc.GetLatestValues(24, MOO.Plant.Minntac, "Floatation");
            Assert.NotEmpty(anoms);
        }

    }
}
