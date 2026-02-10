using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public class MDT_Test:BaseTestClass
    {
        [Fact]
        public void TestMDT_Device()
        {
            var dvcList = DAL.Services.Mdt_DeviceSvc.GetAll();
            Assert.NotEmpty(dvcList);
            var dvcNew = dvcList[0];
            dvcNew.Mac_Addr1 = "test123";
            DAL.Services.Mdt_DeviceSvc.Insert(dvcNew);
            dvcNew = DAL.Services.Mdt_DeviceSvc.Get(dvcNew.Mac_Addr1);
            Assert.NotNull(dvcNew);
            dvcNew.Device_Name = "Test1234556";
            DAL.Services.Mdt_DeviceSvc.Update(dvcNew);
            DAL.Services.Mdt_DeviceSvc.Delete(dvcNew);
        }
    }
}
