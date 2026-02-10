using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public class WoodTruckShipments : BaseTestClass
    {
        [Fact]
        public void TestGet()
        {
            var ship = DAL.Services.Wood_Truck_ShipmentsSvc.GetPagedData(0, 20);
            Assert.NotNull(ship);
            Assert.NotEmpty(ship.Data);
            ship = DAL.Services.Wood_Truck_ShipmentsSvc.GetPagedData(0, 20,DeliveryDateStart:DateTime.Parse("1/1/2020"),DeliveryDateEnd:DateTime.Parse("1/31/2020"),
                                 orderBy: "", orderDirection: "");

            Assert.NotNull(ship);
            Assert.NotEmpty(ship.Data);
            ship = DAL.Services.Wood_Truck_ShipmentsSvc.GetPagedData(0, 20, DeliveryDateStart: DateTime.Parse("1/1/2020"), DeliveryDateEnd: DateTime.Parse("1/31/2020"),
                                 orderBy: "Delivery_Date", orderDirection: "Desc");

            Assert.NotNull(ship);
            Assert.NotEmpty(ship.Data);
        }

        [Fact]
        public void WriteTest()
        {
            var ship = DAL.Services.Wood_Truck_ShipmentsSvc.Get("130946");
            Assert.NotNull(ship);
            //change the invoice number so we can test insert
            ship.Invoice_Nbr = "Test123";
            DAL.Services.Wood_Truck_ShipmentsSvc.Insert(ship);
            ship.Btu_Lb = 123;
            DAL.Services.Wood_Truck_ShipmentsSvc.Update(ship);
            DAL.Services.Wood_Truck_ShipmentsSvc.Delete(ship);
        }
        [Fact]
        public void TestAdjustBTU()
        {
            int recsAffected = DAL.Services.Wood_Truck_ShipmentsSvc.AdjustBTU(DateTime.Parse("1/1/2020"), DateTime.Parse("1/5/2020"), 1.23M);
            Assert.NotEqual(0, recsAffected);

        }
    }
}
