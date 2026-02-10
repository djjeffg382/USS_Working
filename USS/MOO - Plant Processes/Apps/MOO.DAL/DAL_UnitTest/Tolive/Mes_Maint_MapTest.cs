using MOO.DAL.ToLive.Models;
using MOO.DAL.ToLive.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DAL_UnitTest.Tolive
{
    public class Mes_Maint_MapTest : BaseTestClass
    {
        [Fact]
        public void MMM_Test()
        {
            var mmmList = Mes_Maint_MapSvc.GetAll();
            Assert.NotEmpty(mmmList);

            var mmm = Mes_Maint_MapSvc.Get("WOUNDED", 2);
            Assert.NotNull(mmm);

            mmm.Value = "Turbo";
            Mes_Maint_MapSvc.Update(mmm);
        }
    }
}
