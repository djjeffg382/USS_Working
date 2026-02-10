using MOO.DAL.ToLive.Models;
using MOO.DAL.ToLive.Services;
using System;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public class TireMaintTest : BaseTestClass
    {
        [Fact]
        public void BrandTest()
        {
            var brands = DAL.Services.Tire_Maint_BrandSvc.GetAll();
            Assert.NotEmpty(brands);

            var brand = DAL.Services.Tire_Maint_BrandSvc.Get(1);
            Assert.Equal("Michelin XDR250", brand.Tire_Brand);
        }

        [Fact]
        public void ReasonTest()
        {
            var reasons = DAL.Services.Tire_Maint_ReasonSvc.GetAll();
            Assert.NotEmpty(reasons);

            var reason = DAL.Services.Tire_Maint_ReasonSvc.Get(1);
            Assert.Equal(DAL.Enums.TireMaintReason.Worn_Out,reason.Reason_Name);
        }

        [Fact]
        public void MaintTest()
        {
            var tireMaint = Tire_MaintSvc.Get(13);
            Assert.NotNull(tireMaint);

            var mech1 = PeopleSvc.Get(234837);
            var mech2 = PeopleSvc.Get(153091);
            var mech3 = PeopleSvc.Get(251032);

            tireMaint = new()
            {
                Maint_Date = DateTime.Now,
                Plant = MOO.Plant.Minntac,
                Equip_Id = "temp",
                Workorder_Nbr = "temp",
                Mems_Verified = false,
                Mechanics = [mech1, mech2],
            };
            Assert.Equal(3, Tire_MaintSvc.Insert(tireMaint));

            var tireMaints = Tire_MaintSvc.GetAll();
            Assert.NotEmpty(tireMaints);

            tireMaint.Mechanics.Add(mech3);
            Assert.Equal(1 + 2 + 3, Tire_MaintSvc.Update(tireMaint)); 


            //tireMaint.Plant = MOO.Plant.Keetac;
            //Assert.Equal(1, Tire_MaintSvc.Update(tireMaint));

            Assert.Equal(4, Tire_MaintSvc.Delete(tireMaint));
        }

        [Fact]
        public void MaintTireTest()
        {
            var tireMaintTire = Tire_Maint_TireSvc.Get(18);
            Assert.NotNull(tireMaintTire);

            var tireMaintTires = Tire_Maint_TireSvc.GetAll();
            Assert.NotEmpty(tireMaintTires);


            Tire_Maint tireMaint = new()
            {
                Maint_Date = DateTime.Now,
                Plant = MOO.Plant.Minntac,
                Equip_Id = "temp",
                Workorder_Nbr = "temp",
                Mems_Verified = false,
            };
            Assert.Equal(1,Tire_MaintSvc.Insert(tireMaint));

            tireMaintTire = new()
            {
                Tire_Maint_Id = tireMaint.Tire_Maint_Id,
                Tire_Position = 1,
                Brand = Tire_Maint_BrandSvc.Get(1),
                Serial_Nbr = "Temp",
                Tread_Depth_In = "in",
                Tread_Depth_Out = "out",
                Removal_Reason1 = DAL.Enums.TireMaintReason.Puncture,
                Disposition = DAL.Enums.Disposition.Inspection,
                Remove_Install = DAL.Enums.RemoveInstall.Remove,
                Nuts_Torqued = true,
                Tire_Shop_Date = DateTime.Now,
                Tire_Shop_Status = DAL.Enums.TireShopStatus.Junk,
            };

            Assert.Equal(1,Tire_Maint_TireSvc.Insert(tireMaintTire));

            tireMaintTire.Removal_Reason2 = DAL.Enums.TireMaintReason.Accidental_Damage;
            Assert.Equal(1,Tire_Maint_TireSvc.Update(tireMaintTire));

            Assert.Equal(1,Tire_Maint_TireSvc.Delete(tireMaintTire));
            Assert.Equal(1, Tire_MaintSvc.Delete(tireMaint));

        }
    }
}

