using MOO.DAL.Blast.Models;
using MOO.DAL.Blast.Services;
using MOO.DAL.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.Blast;

namespace DAL_UnitTest.Blast
{
    public class CAT_PitsTest : BaseTestClass
    {
        [Fact]
        public void TestPits()
        {
            var p = CAT_PitsSvc.Get(1);
            Assert.NotNull(p);
            var list = CAT_PitsSvc.GetAll();
            Assert.NotEmpty(list);
            var pit = CAT_PitsSvc.GetName("East");
            Assert.NotNull(pit);
        }
    }
}
