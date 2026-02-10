using MOO.DAL.Blast.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.Blast;

namespace DAL_UnitTest.Blast
{
    public class PatternTest : BaseTestClass
    {
        [Fact]
        public void TestPattern()
        {
            var patterns0 = PatternSvc.GetByMineSchedDate(DateTime.Today, DateTime.Today.AddDays(7));
            Assert.NotEmpty(patterns0);
            patterns0 = PatternSvc.GetByUpdateDate(DateTime.Today.AddYears(-3), DateTime.Today.AddDays(7));
            Assert.NotEmpty(patterns0);
            var patterns = DAL.Services.PatternSvc.GetAll();
            Assert.NotEmpty(patterns);
            var pattern = DAL.Services.PatternSvc.Get("14119");
            Assert.NotNull(pattern);
            var id1 = DAL.Services.PatternSvc.Get(53);
            Assert.NotNull(id1);
            id1.Mine_Sched = MOO.DAL.ToLive.Services.Mine_SchedSvc.Get(9);
            Assert.Equal(1, PatternSvc.Update(id1));
        }


        [Fact]
        public void TestPatternHoles()
        {
            var p = DAL.Services.Pattern_HolesSvc.GetByPatternNumber("21001");
            Assert.NotEmpty(p);
            var ph = DAL.Services.Pattern_HolesSvc.Get("21001", "103");
            Assert.NotNull(ph);
            var id = DAL.Services.Pattern_HolesSvc.Get(28654);
            Assert.NotNull(id);
        }

        [Fact]
        public void TestPatternHoleDelete()
        {
            //Assert.True( DAL.Services.Pattern_HolesSvc.DeletePatternHolesByPattern("21001") > 0);
        }
    }
}
