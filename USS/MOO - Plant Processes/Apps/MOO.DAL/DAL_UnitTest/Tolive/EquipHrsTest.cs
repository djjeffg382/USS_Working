using MOO.DAL.ToLive.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public class EquipHrsTest : BaseTestClass
    {
        [Fact]
        public void TestEquipMaster()
        {
            var eqList = DAL.Services.Equipment_MasterSvc.GetAll("Minntac", "Concentrator", "Mills");
            Assert.NotEmpty(eqList);
        }
        [Fact]
        public void TestEquipUnitAssoc()
        {
            var eua = DAL.Services.Equipment_Unit_AssocSvc.GetAll();
            Assert.NotEmpty(eua);
        }

        [Fact]
        public void EquipCompMasterTest()
        {
            var ecm = DAL.Services.Equipment_Component_MasterSvc.GetEquipComponents("4257");
            Assert.NotEmpty(ecm);
        }

        [Fact]
        public void TestEquipCompUnitRollup()
        {
            var ecr = Equipment_Comp_Units_RollupSvc.GetEquipCompUnit("4A0401", 0, DAL.Enums.EqUnitTypes.Hours, DateTime.Parse("1/1/2020"), DateTime.Parse("1/10/2020"));
            Assert.NotEmpty(ecr);

            var ecLatest = Equipment_Comp_Units_RollupSvc.GetLatestValue("4A0401", 0, DAL.Enums.EqUnitTypes.Hours, DateTime.Parse("1/1/2020"));
            Assert.NotNull(ecLatest);
        }
    }
}
