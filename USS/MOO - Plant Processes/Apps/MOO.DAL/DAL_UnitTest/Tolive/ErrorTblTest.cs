using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;


namespace DAL_UnitTest.Tolive
{
    public class ErrorTblTest: BaseTestClass
    {
        [Fact]
        public void TestPagedData()
        {
            DAL.Services.ErrorSvc.OraErrorLogType[] errTypes = { DAL.Services.ErrorSvc.OraErrorLogType.Error };
            MOO.DAL.PagedData<List<DAL.Models.Error>> pd = DAL.Services.ErrorSvc.GetPagedData(null, null, errTypes, "", "", "", 0, 50);
            Assert.NotNull(pd);


            pd = DAL.Services.ErrorSvc.GetPagedData(DateTime.Parse("1/1/2000 10:00"), DateTime.Parse("1/1/2050"), errTypes, "", "", "", 0, 50);
            Assert.NotNull(pd);

            pd = DAL.Services.ErrorSvc.GetPagedData(DateTime.Parse("1/1/2000"), DateTime.Parse("1/1/2050"), errTypes, "Test", "Test", "Test", 0, 50);
            Assert.NotNull(pd);


        }
    }
}
