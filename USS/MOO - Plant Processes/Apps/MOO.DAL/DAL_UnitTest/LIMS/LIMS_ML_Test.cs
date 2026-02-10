using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;




namespace DAL_UnitTest.LIMS
{
    /// <summary>
    /// Tests for LIMS ML
    /// </summary>
    /// <remarks>
    /// IMPORTANT!!!  Make sure to check "C:\MOOGlobalSettings\GlobalSettings.json" to verify you are connecting to correct LIMS
    /// 
    /// </remarks>
    public class LIMS_ML_Test
    {
        [Fact]
        public void ReceiveTest()
        {
            //use a sample number that is in LIMS and debug
            var ret = MOO.DAL.LIMS.Services.LimsML.ExecuteReceiveSample(502459, DateTime.Now.AddDays(-3), DateTime.Now.AddDays(-2));
            Assert.Empty(ret);
        }

        [Fact]
        public void CancelTest()
        {
            //use a sample number that is in LIMS and debug
            var ret = MOO.DAL.LIMS.Services.LimsML.ExecuteCancelSample(502455);
            Assert.Empty(ret);
        }


        [Fact]
        public void TransferTest()
        {
            //use a sample number that is in LIMS and debug
            var ret = MOO.DAL.LIMS.Services.LimsML.SetTransferDate(502457,DateTime.Now);
            Assert.Empty(ret);
        }

        [Fact]
        public void LoginSample()
        {
            var a = MOO.DAL.LIMS.Services.LimsML.LoginOilSample("M0580_1", "TESTING", "M_MINE_LBL");
            Assert.NotEqual(-1, a);
        }

        [Fact]
        public void SetCollected()
        {
            //use a sample number that is in LIMS and debug
            var result = MOO.DAL.LIMS.Services.LimsML.SetCollected(502457,DateTime.Now);
            Assert.Empty(result);
        }
    }
}
