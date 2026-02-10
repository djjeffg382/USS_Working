using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public class MDT_Hist:BaseTestClass
    {
        [Fact]
        public void TestMDT_Hist()
        {
            //var x = DAL.Services.Mdt_HistSvc.Get("00-1B-EB-02-AE-F3", "0555", "MTC-M000555");
            //x.Equipment_Id = "1234";
            //DAL.Services.Mdt_HistSvc.Insert(x)

            var y = DAL.Services.Mdt_HistSvc.GetAll("00-1B-EB-02-AE-F3");
        }
    }
}
