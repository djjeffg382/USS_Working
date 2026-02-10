using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DAL_UnitTest.Tolive
{
    public class HangupTest : BaseTestClass
    {
        [Fact]
        public void Test()
        {
            var hangups= MOO.DAL.ToLive.Services.HangupSvc.GetAll();

            Assert.NotEmpty(hangups);

            var x = MOO.DAL.ToLive.Services.HangupSvc.Get(52907);
            Assert.NotNull(x);
            x.Hangup_Comments = "Test";

            MOO.DAL.ToLive.Services.HangupSvc.Update(x);


        }
        [Fact]
        public void Test2()
        {
            MOO.DAL.ToLive.Services.HangupSvc.CleanupHaulCycleTrans(100);
        }
    }
}
