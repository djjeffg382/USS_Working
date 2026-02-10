using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public class Tails_AnalysisTest : BaseTestClass
    {

        [Fact]
        public void CoarseTailsTest()
        {
            var ctList = DAL.Services.Coarse_Tails_AnalysisSvc.GetByDate(DateTime.Parse("1/1/2018"), DateTime.Parse("2/1/2018"));
            Assert.NotEmpty(ctList);

            DAL.Models.Coarse_Tails_Analysis ct = new()
            {
                Sdate = DateTime.Today,
                Intvl = 1,
                Line = 3,
                Fe = 1.23M
            };

            DAL.Services.Coarse_Tails_AnalysisSvc.Insert(ct);
            ct.Fe = 3.21M;
            Assert.Equal(1,DAL.Services.Coarse_Tails_AnalysisSvc.Update(ct));
            Assert.Equal(1,DAL.Services.Coarse_Tails_AnalysisSvc.Delete(ct));
        }


        [Fact]
        public void FineTailsTest()
        {
            var ftList = DAL.Services.Fine_Tails_AnalysisSvc.GetByDate(DateTime.Parse("1/1/2018"), DateTime.Parse("1/1/2018"), false);
            Assert.NotEmpty(ftList);

            DAL.Models.Fine_Tails_Analysis ft = new()
            {
                Sdate = DateTime.Today,
                Shift = 1,
                Line = 3,
                Fe = 1.23M
            };

            DAL.Services.Fine_Tails_AnalysisSvc.Insert(ft);
            ft.Fe = 3.21M;
            //expect 2 records to be affected on writes as the odd row affects the even line, see comments on Fine_Tails Class
            Assert.Equal(2,DAL.Services.Fine_Tails_AnalysisSvc.Update(ft));
            Assert.Equal(2,DAL.Services.Fine_Tails_AnalysisSvc.Delete(ft));
        }
    }
}
