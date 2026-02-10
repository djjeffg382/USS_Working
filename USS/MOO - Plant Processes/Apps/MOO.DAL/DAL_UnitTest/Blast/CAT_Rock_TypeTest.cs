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
    public class CAT_Rock_TypeTest : BaseTestClass
    {
        [Fact]
        public void TestPits()
        {
            var r = CAT_Rock_TypeSvc.Get(1);
            Assert.NotNull(r);
            var list = CAT_Rock_TypeSvc.GetAll();
            Assert.NotEmpty(list);
            var rock = CAT_Rock_TypeSvc.GetName("Waste");
            Assert.NotNull(rock);
        }
    }
}
