using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public class ERP_ShippingRZ_Test : BaseTestClass
    {
        [Fact]
        public void Test()
        {
            var lst = DAL.Services.ERP_Shipping_RZ_OrdersSvc.GetAll();
            Assert.NotEmpty(lst);

            //make a new one by just taking the first record and changing the order_num
            lst[0].Order_Num = "ZZ99999";
            DAL.Services.ERP_Shipping_RZ_OrdersSvc.Insert(lst[0]);


            lst[0].State = "ZZ";
            Assert.Equal(1,DAL.Services.ERP_Shipping_RZ_OrdersSvc.Update(lst[0]));

            Assert.Equal(1,DAL.Services.ERP_Shipping_RZ_OrdersSvc.Delete(lst[0]));


        }
    }
}
