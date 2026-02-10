using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public class Mill_ChargingTest : BaseTestClass
    {
        [Fact]
        public void TestMillCharge()
        {
            DAL.Models.Mill_Charging newMC = new()
            {
                Equip_Id = "test",
                Charge_Count = 500,
                Charge_Date = DateTime.Today,
                Lbs_Per_Charge = 100.123M
            };
            DAL.Services.Mill_ChargingSvc.Insert(newMC);
            var mcList = DAL.Services.Mill_ChargingSvc.GetByDate(DateTime.Today.AddDays(-1),DateTime.Today.AddDays(1));
            Assert.NotEmpty(mcList);

            newMC.Charge_Count = 555;
            DAL.Services.Mill_ChargingSvc.Update(newMC);
            DAL.Services.Mill_ChargingSvc.Delete(newMC);

            var a = DAL.Services.Mill_ChargingSvc.GetLastLbPerCharge(DateTime.Now, "110");
            Assert.NotEqual(0,a);
        }
        [Fact]
        public void TestMillChargeMeasure()
        {

            var mcList = DAL.Services.Mill_Charge_MeasureSvc.GetAll("120");
            Assert.NotEmpty(mcList);
            mcList[0].Measure_Date = DateTime.Today;
            DAL.Services.Mill_Charge_MeasureSvc.Update(mcList[0]);
        }
    }
}
