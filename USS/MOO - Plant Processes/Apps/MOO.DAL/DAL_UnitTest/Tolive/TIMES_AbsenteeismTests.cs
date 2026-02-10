using MOO.DAL.ToLive.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DAL_UnitTest.Tolive
{
    public class TIMES_AbsenteeismTests : BaseTestClass
    {
        [Fact]
        public void TimesTest()
        {
            var x = TIMES_AbsenteeismSvc.GetAllByDate(DateTime.Today.AddYears(-3),DateTime.Today);
            Assert.NotEmpty(x);
            //create a new
            x[0].Absent_Id = -1;
            x[0].Absent_Date = DateTime.MinValue;
            TIMES_AbsenteeismSvc.Insert(x[0]);

            Assert.Equal(1,TIMES_AbsenteeismSvc.Update(x[0]));

            Assert.Equal(1, TIMES_AbsenteeismSvc.Delete(x[0]));

        }
        
    }
}
