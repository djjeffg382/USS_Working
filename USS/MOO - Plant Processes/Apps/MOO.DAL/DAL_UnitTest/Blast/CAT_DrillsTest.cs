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
    public class CAT_DrillsTest : BaseTestClass
    {
        [Fact]
        public void TestPits()
        {
            var d = CAT_DrillsSvc.Get(4);
            Assert.NotNull(d);
            var list = CAT_DrillsSvc.GetAll();
            Assert.NotEmpty(list);
        }
    }
}
