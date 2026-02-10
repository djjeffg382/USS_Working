using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Net;

namespace MOO_UnitTest
{
    public class NetworkTests
    {
        [Fact]
        public void TestMask()
        {
            int mask= MOO.Network.SubnetToBitMask(System.Net.IPAddress.Parse("255.255.255.0"));
            Assert.Equal(24, mask);
            mask = MOO.Network.SubnetToBitMask(System.Net.IPAddress.Parse("255.255.255.128"));
            Assert.Equal(25, mask);
        }


        [Fact]
        public void TestRange()
        {
            IPAddress[] range = MOO.Network.GetIpRange(IPAddress.Parse("170.4.133.128"), 23);
            Assert.NotEmpty(range);
            //var ipList = MOO.Network.GetIpList(range[0], range[1]);

        }
    }


}
