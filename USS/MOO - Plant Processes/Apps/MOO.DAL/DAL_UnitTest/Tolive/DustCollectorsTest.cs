using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public class DustCollectorsTest : BaseTestClass
    {
        [Fact]
        public void DustCollParms()
        {
            var dc = DAL.Services.Dust_Coll_ParmsSvc.Get("2D-01-01");
            Assert.NotNull(dc);

            var dcList = DAL.Services.Dust_Coll_ParmsSvc.GetAll(MOO.Plant.Minntac);
            Assert.NotEmpty(dcList);

            var areaList = DAL.Services.Dust_Coll_ParmsSvc.GetAreaListing();
            Assert.NotEmpty(areaList);
        }

        [Fact]
        public void DCParmUpdate()
        {
            var dc = DAL.Services.Dust_Coll_ParmsSvc.Get("2D-01-01");
            Assert.NotNull(dc);
            dc.DC_Comments = $"Test Update {DateTime.Now:MM/dd/yyyy HH:mm:ss}";
            DAL.Services.Dust_Coll_ParmsSvc.Update(dc);
        }

        [Fact]
        public void DustCollEpaReading()
        {
            var reading = DAL.Services.Dust_Coll_Epa_ReadingSvc.Get("2D-02-02");
            Assert.NotNull(reading);

            reading = DAL.Services.Dust_Coll_Epa_ReadingSvc.GetLatest("3D-01-01");
            Assert.NotNull(reading);

            var readings = DAL.Services.Dust_Coll_Epa_ReadingSvc.GetByDateRange(DateTime.Today.AddYears(-2), DateTime.Today);
            Assert.NotEmpty(readings);

            DAL.Models.Dust_Coll_Epa_Reading newReading = new()
            {
                Equip_No = "2D-01-01",
                Virtual_Read_Date = DateTime.Today,
                Pressure_Date = DateTime.Today,
                Pressure_Val = 0,
                Flow_Date = DateTime.Today,
                Flow_Val = 0,
                Reading_Ind = "F",
                Processed_Ind = "O",
            };

            Assert.Equal(1,DAL.Services.Dust_Coll_Epa_ReadingSvc.Insert(newReading));
            newReading.Pressure_Val = 234;
            Assert.Equal(1,DAL.Services.Dust_Coll_Epa_ReadingSvc.Update(newReading));
        }

        [Fact]
        public void BH15()
        {
            var bh = DAL.Services.MACT_BH_15MinSvc.GetByDateRange("058-02-1M1", DateTime.Parse("1/1/2020"), DateTime.Parse("1/5/2020"));
            Assert.NotEmpty(bh);
        }


        [Fact]
        public void DC15()
        {
            var dc = DAL.Services.MACT_DC_15MinSvc.GetByDateRange("298-05-7", DateTime.Parse("1/1/2020"), DateTime.Parse("1/5/2020"));
            Assert.NotEmpty(dc);
        }
    }
}
