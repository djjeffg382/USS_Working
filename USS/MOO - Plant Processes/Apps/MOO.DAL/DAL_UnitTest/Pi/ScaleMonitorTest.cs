using MOO.DAL.Pi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.Pi;

namespace DAL_UnitTest.Pi
{
    public class ScaleMonitorTest
    {
        [Fact]
        public void TestMtcAgglScaleMonitor()
        {
            var result = MtcAggScaleMonitorSvc.GetScaleMonitorData(DateTime.Now.AddDays(-1), DateTime.Now);
            Assert.NotEmpty(result);

        }
    }
}
