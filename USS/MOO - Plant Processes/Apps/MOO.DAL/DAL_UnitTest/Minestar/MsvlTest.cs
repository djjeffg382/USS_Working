using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.MineStar;

namespace DAL_UnitTest.Minestar
{
    public class MsvlTest
    {
        [Fact]
        public void TestFleetSummarySMU()
        {
            var fsu = DAL.Services.Fleet_Summary_SMUSvc.GetLatestSnapshot();
            Assert.NotEmpty(fsu);
        }
    }
}
