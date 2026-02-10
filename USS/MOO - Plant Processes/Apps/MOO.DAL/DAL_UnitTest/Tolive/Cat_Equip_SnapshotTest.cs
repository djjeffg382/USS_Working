using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public class Cat_Equip_SnapshotTest : BaseTestClass
    {
        [Fact]
        public void Test()
        {
            var c = DAL.Services.Cat_Equipment_SnapshotSvc.GetLatestSnapshot();
            Assert.NotEmpty(c);
        }
    }
}
