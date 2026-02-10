using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public class CorpDelaysTest : BaseTestClass
    {
        [Fact]
        public void TestCorpDelayTypes()
        {
            //get a list of all delay types
            var dlyList = DAL.Services.Corporate_Delay_TypesSvc.GetAll();
            Assert.NotEmpty(dlyList);

        }

        [Fact]
        public void TestCorpDelaysTable()
        {

            var crush = DAL.Services.Corporate_DelaysSvc.GetOpenDelayLike("CR-SECONDARY");
            
            //test getting a delay object, need to make sure this delay_id exists in table
            var dly = DAL.Services.Corporate_DelaysSvc.Get(31933376);
            Assert.NotNull(dly);

            var dlyList = DAL.Services.Corporate_DelaysSvc.GetByDateRange(DateTime.Parse("5/1/2023"), DateTime.Parse("7/31/2023"), "CO-LINE-18-DELAY");
            Assert.NotEmpty(dlyList);



        }
        [Fact]
        public void TestRecordDelay()
        {
            //test recording a delay
            DAL.Services.Corporate_DelaysSvc.RecordMtcDelay("CO-LINE-18-DELAY", DateTime.Now.AddMinutes(-20), false);
            DAL.Services.Corporate_DelaysSvc.RecordMtcDelay("CO-LINE-18-DELAY", DateTime.Now, true);

        }
    }
}
