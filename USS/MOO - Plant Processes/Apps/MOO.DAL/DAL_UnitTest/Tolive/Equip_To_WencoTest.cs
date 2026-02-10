using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public class Equip_To_WencoTest : BaseTestClass
    {
        [Fact]
        public void TestEquipToWenco()
        {

            DAL.Models.Equip_To_Wenco etw = new()
            {
                Plant = MOO.Plant.Minntac,
                Wenco_Equip_Ident = $"Test - {DateTime.Now:yyyyMMddHHmmss}",
                Foreign_Id = "Testabc",
                System_Name = DAL.Models.Equip_To_Wenco.SystemType.Epiroc,
                Active = false
            };
            DAL.Services.Equip_To_WencoSvc.Insert(etw);
            etw.Previous_Badge_Date = DateTime.Now;
            DAL.Services.Equip_To_WencoSvc.UpdateInterfaceData(etw);
            etw.Active = true;
            DAL.Services.Equip_To_WencoSvc.UpdateSettingData(etw);
            var etwList = DAL.Services.Equip_To_WencoSvc.GetAll();

            DAL.Services.Equip_To_WencoSvc.Delete(etw);
        }
    }
}
