using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL = MOO.DAL.ToLive;
using Xunit;

namespace DAL_UnitTest.Tolive
{
    public class RMF_ScreenTest : BaseTestClass
    {
        [Fact]
        public void Test()
        {
            //create a record
            DAL.Models.Rmf_Screen scn = new()
            {
                Shift_Date = DateTime.Today,
                Shift = 1,
                Step = 2,
                Start_Wgt = 15.4M,
                Scn_1_Inch = .1M,
                Scn_3_4_Inch = .5M,
                Scn_1_2_Inch = 2.8M,
                Scn_1_4_Inch = 4.5M,
                Scn_6m = 2.4M,
                Scn_Minus_6m = 5.1M
            };
            //validate the readonly calculations
            Assert.Equal( 33.1M, Math.Round(scn.Scn_Minus_6m_Pct.Value,1,MidpointRounding.AwayFromZero));
            //6 Mesh
            Assert.Equal(15.6M, Math.Round(scn.Scn_6m_Pct.Value, 1, MidpointRounding.AwayFromZero));
            Assert.Equal(33.1M, Math.Round(scn.Scn_6m_Cumulative.Value, 1, MidpointRounding.AwayFromZero));
            //1/4 inch
            Assert.Equal(29.2M, Math.Round(scn.Scn_1_4_InchPct.Value, 1, MidpointRounding.AwayFromZero));
            Assert.Equal(48.7M, Math.Round(scn.Scn_1_4_Cumulative.Value, 1, MidpointRounding.AwayFromZero));
            //1/2 inch
            Assert.Equal(18.2M, Math.Round(scn.Scn_1_2_InchPct.Value, 1, MidpointRounding.AwayFromZero));
            Assert.Equal(77.9M, Math.Round(scn.Scn_1_2_Cumulative.Value, 1, MidpointRounding.AwayFromZero));
            //3/4 inch
            Assert.Equal(3.2M, Math.Round(scn.Scn_3_4_InchPct.Value, 1, MidpointRounding.AwayFromZero));
            Assert.Equal(96.1M, Math.Round(scn.Scn_3_4_Cumulative.Value, 1, MidpointRounding.AwayFromZero));
            //1 inch
            Assert.Equal(.6M, Math.Round(scn.Scn_1_InchPct.Value, 1, MidpointRounding.AwayFromZero));
            Assert.Equal(99.4M, Math.Round(scn.Scn_1_Cumulative.Value, 1, MidpointRounding.AwayFromZero));

            //start weight and cumulative weight should be equal
            Assert.Equal(scn.Start_Wgt, scn.CumulativeWeight);


            //scn.Scn_Minus_6m_Inch = null;
            DAL.Services.Rmf_ScreenSvc.Insert(scn);

            scn.Scn_1_Inch = .2M;
            DAL.Services.Rmf_ScreenSvc.Update(scn);

            var sList = DAL.Services.Rmf_ScreenSvc.GetShiftDate(DateTime.Today, 2);
            Assert.NotEmpty(sList);

            scn = DAL.Services.Rmf_ScreenSvc.Get(DateTime.Today, 1, 2);
            Assert.NotNull(scn);

            DAL.Services.Rmf_ScreenSvc.Delete(scn);

        }
    }
}
