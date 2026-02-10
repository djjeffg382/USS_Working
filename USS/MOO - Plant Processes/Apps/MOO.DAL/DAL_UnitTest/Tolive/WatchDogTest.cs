using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public  class WatchDogTest : BaseTestClass
    {
        [Fact]
        public void WatchDog_Test()
        {
            var WDList = DAL.Services.WatchDogSvc.GetAll();
            Assert.NotEmpty(WDList);
            DAL.Services.WatchDogSvc.SetActive(WDList[0], true);
            DataSet ds = DAL.Services.WatchDogSvc.RunCommand(WDList[0]);
            Assert.NotNull(ds);
        }
    }
}
