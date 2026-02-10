using MOO.DAL.ToLive.Models;
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
    public class OM_Test_Transactions
    {
        private DAL.Models.OM_Location CreateDummyLoc(string LocationName, OM_Location.LocationTypeEnum LocationType)
        {
            //just use whatever the first site in the DB is for testing
            var site = DAL.Services.OM_SiteSvc.GetAllAsync().GetAwaiter().GetResult()[0];

            DAL.Models.OM_Location newLoc = new DAL.Models.OM_Location()
            {
                Location_Name = $"{LocationName}{DateTime.Now:yyMMdd HHmmss}",
                Site = site,
                LocationType = LocationType                
            };
            OM_LocationSvc.InsertAsync(newLoc).GetAwaiter().GetResult(); 
            return newLoc;
        }


        [Fact]
        public void TestFromTrainToLocation()
        {
            //start by creating a dummy location
            var myStockpile = CreateDummyLoc("B Test Stockpile", OM_Location.LocationTypeEnum.Stockpile);

            //create a transaction
            OM_Location_Trans newTrans = new()
            {
                SourceType = OM_Location_Trans.LocTransSrcDestType.Train,
                DestType = OM_Location_Trans.LocTransSrcDestType.Location,
                Dest_Id = myStockpile.Location_Id,
                Transaction_Date = DateTime.Now,
                Quantity = 500,
                Fe = 65                
            };
            OM_Location_TransSvc.InsertAsync(newTrans).GetAwaiter().GetResult();
        }
    }
}
