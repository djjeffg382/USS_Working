using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public class OM_Tests:BaseTestClass
    {

        [Fact]
        public void TestVessel()
        {

            DAL.Models.OM_Vessel v = new()
            {
                Vessel_Name = $"Brian Test{DateTime.Now:yyMMddHH:mm:ss}",
                Active = true
            };
            DAL.Services.OM_VesselSvc.InsertAsync(v).GetAwaiter().GetResult();
            v = DAL.Services.OM_VesselSvc.GetAsync(v.Vessel_Id).GetAwaiter().GetResult();
            Assert.NotNull(v);

            var vList = DAL.Services.OM_VesselSvc.GetAllAsync().GetAwaiter().GetResult();
            Assert.NotEmpty(vList);

            v.Vessel_Name = $"BrianTestUpdate{DateTime.Now:yyMMddHH:mm:ss}";
            DAL.Services.OM_VesselSvc.UpdateAsync(v).GetAwaiter().GetResult();

            DAL.Services.OM_VesselSvc.DeleteAsync(v).GetAwaiter().GetResult();
        }

        [Fact]
        public void TestSite()
        {
            var s = new DAL.Models.OM_Site()
            {
                Site_Name = $"Brian Test Site{DateTime.Now:yyMMddHH:mm:ss}",
                SiteType = DAL.Models.OM_Site.SiteTypeEnum.UssPlant
            };
            DAL.Services.OM_SiteSvc.InsertAsync(s).GetAwaiter().GetResult();

            s= DAL.Services.OM_SiteSvc.GetAsync(s.Site_Id).GetAwaiter().GetResult();
            Assert.NotNull(s);

            var sList = DAL.Services.OM_SiteSvc.GetAllAsync().GetAwaiter().GetResult();
            Assert.NotEmpty(sList);

            sList = DAL.Services.OM_SiteSvc.GetAllAsync(s.SiteType).GetAwaiter().GetResult();
            Assert.NotEmpty(sList);

            s.Site_Name = $"Brian Test Site{DateTime.Now:yyMMddHH:mm:ss}";
            Assert.Equal(1,DAL.Services.OM_SiteSvc.UpdateAsync(s).GetAwaiter().GetResult());

            Assert.Equal(1, DAL.Services.OM_SiteSvc.DeleteAsync(s).GetAwaiter().GetResult());

        }

        [Fact]
        public void TestLoc()
        {

            var s = new DAL.Models.OM_Site()
            {
                Site_Name = $"Test Site_{DateTime.Now:yyMMddHH:mm:ss}",
                SiteType = DAL.Models.OM_Site.SiteTypeEnum.UssPlant
            };
            DAL.Services.OM_SiteSvc.InsertAsync(s).GetAwaiter().GetResult();

            var loc = new DAL.Models.OM_Location()
            {
                Location_Name = $"Test Loc_{DateTime.Now:yyMMddHH:mm:ss}",
                Site = s,
                LocationType = DAL.Models.OM_Location.LocationTypeEnum.Stockpile
            };

            Assert.Equal(1, DAL.Services.OM_LocationSvc.InsertAsync(loc).GetAwaiter().GetResult());

            var locList = DAL.Services.OM_LocationSvc.GetAllAsync().GetAwaiter().GetResult();
            Assert.NotEmpty(locList);

            loc = DAL.Services.OM_LocationSvc.GetAsync(loc.Location_Id).GetAwaiter().GetResult();   
            Assert.NotNull(loc);

            loc.Location_Name = $"Tes Loc2{DateTime.Now:yyMMddHH:mm:ss}";
            Assert.Equal(1, DAL.Services.OM_LocationSvc.UpdateAsync(loc).GetAwaiter().GetResult());

            Assert.Equal(1, DAL.Services.OM_LocationSvc.DeleteAsync(loc).GetAwaiter().GetResult());

        }

        [Fact]
        public void TestLocationInventory()
        {
            var locInvList = DAL.Services.OM_Location_InvSvc.GetByLocationAsync(DateTime.Parse("1/1/1970"), DateTime.Now, 1).GetAwaiter().GetResult();
            Assert.NotEmpty(locInvList);
        }

        [Fact]
        public void LocTransTest()
        {
            
            var lt = new DAL.Models.OM_Location_Trans()
            {
                Source_Id = 1,
                Source_Type_Id = 1,
                Dest_Id = 2,
                Dest_Type_Id = 1,
                Transaction_Date = DateTime.Now,
                Fe = 65,
                Quantity = 10000
            };

            Assert.Equal(1, DAL.Services.OM_Location_TransSvc.InsertAsync(lt).GetAwaiter().GetResult());

            var ltList = DAL.Services.OM_Location_TransSvc.GetTransactionsByLocationAsync(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1), 1).GetAwaiter().GetResult();
            Assert.NotEmpty(ltList);

            lt = DAL.Services.OM_Location_TransSvc.GetAsync(lt.Location_Trans_Id).GetAwaiter().GetResult();
            Assert.NotNull(lt);

            lt.Fe = 66;
            Assert.Equal(1, DAL.Services.OM_Location_TransSvc.UpdateAsync(lt).GetAwaiter().GetResult());

            Assert.Equal(1, DAL.Services.OM_Location_TransSvc.DeleteAsync(lt).GetAwaiter().GetResult());

        }
    }
}
