using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
namespace DAL_UnitTest.Tolive
{
    public class AnalyticsTest : BaseTestClass
    {

        [Fact]
        public void TestAnalytics()
        {
            var aa = MOO.DAL.ToLive.Services.AnalyticsSvc.Get("ball_mill_constraints");
            Assert.NotEmpty(aa);

            var bb = MOO.DAL.ToLive.Services.AnalyticsSvc.GetGrouped("sag_mill_constraints");
            Assert.NotNull(bb);
        }
    }
}
