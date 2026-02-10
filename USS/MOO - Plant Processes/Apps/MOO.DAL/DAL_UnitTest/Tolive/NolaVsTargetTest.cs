using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public class NolaVsTargetTest
    {
        [Fact]
        public void RunTest()
        {
            DAL.Models.Nola_Vs_Target nvt = new()
            {
                TheDate = DateTime.Now,
                Step = 23
            };

            var result = DAL.Services.Nola_Vs_TargetSvc.InsertAsync(nvt).GetAwaiter().GetResult();

            nvt.Solved_At_Conc = true;
            result = DAL.Services.Nola_Vs_TargetSvc.UpdateAsync(nvt).GetAwaiter().GetResult();
            Assert.Equal(1, result);

            nvt = DAL.Services.Nola_Vs_TargetSvc.GetAsync(nvt.TheDate, nvt.Step).GetAwaiter().GetResult();
            Assert.NotNull(nvt);

            result = DAL.Services.Nola_Vs_TargetSvc.DeleteAsync(nvt).GetAwaiter().GetResult();
            Assert.Equal(1, result);

            var nvtList = DAL.Services.Nola_Vs_TargetSvc.GetAllAsync().GetAwaiter().GetResult();
            Assert.NotEmpty(nvtList);



        }
    }
}
