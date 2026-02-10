using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive 
{
    public class Pulse_Analog_CheckTest : BaseTestClass
    {
        [Fact]
        public void TestPulseAnalog()
        {
            List<DAL.Models.Pulse_Analog_Check> pac;
            pac = DAL.Services.Pulse_Analog_CheckSvc.GetByArea(DAL.Models.Pulse_Analog_Check.PACheckArea.Conc);
            Assert.NotNull(pac);
            Assert.NotEmpty(pac);

            pac = DAL.Services.Pulse_Analog_CheckSvc.GetByAreaWithPiValues(
                        DAL.Models.Pulse_Analog_Check.PACheckArea.Agg2,DateTime.Now.AddHours(-2),DateTime.Now);

            pac = DAL.Services.Pulse_Analog_CheckSvc.GetByAreaWithPiValues(
                        DAL.Models.Pulse_Analog_Check.PACheckArea.Crusher, DateTime.Now.AddHours(-2), DateTime.Now);
            foreach (DAL.Models.Pulse_Analog_Check p in pac)
            {
                Assert.IsType<decimal>(p.AnalogVal);
                Assert.IsType<decimal>(p.PulseVal);
                Assert.IsType<decimal>(p.ValueDifference);

            }
                
            Assert.NotNull(pac);
            Assert.NotEmpty(pac);
        }
    }
}
