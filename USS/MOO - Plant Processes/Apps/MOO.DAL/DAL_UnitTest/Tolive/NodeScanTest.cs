using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public  class NodeScanTest : BaseTestClass
    {
        [Fact]
        public void NodeScan_Test()
        {
            var NSList = DAL.Services.NodeScan_TblSvc.GetAll();
            Assert.NotEmpty(NSList);
            DAL.Services.NodeScan_TblSvc.SetScheduledDown(NSList[0], true);
            DAL.Services.NodeScan_TblSvc.SetActive(NSList[0], true);
        }
    }
}
