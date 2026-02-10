using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public class FormSessionIdTest
    {
        [Fact]
        public void TestFormSession()
        {
            var fs = new DAL.Models.FormsSessionId()
            {
                Session_Id = $"{DateTime.Now:yyyyMMddHHmmss}_test",
                WebLoginName = "MOO.DAL.TEST",
                NtUserName = "MOO.DAL.TEST",
                ChangedFromIp = "0.0.0.0",
                Value = "0",
                RecordDate = DateTime.Now
            };
            DAL.Services.FormsSessionIdSvc.Insert(fs);

            var fsList = DAL.Services.FormsSessionIdSvc.GetAll();
            Assert.NotEmpty(fsList);
            fs = DAL.Services.FormsSessionIdSvc.GetByUsername(fs.NtUserName);
            Assert.NotNull(fs);
            fs.RecordDate = DateTime.Now;
            Assert.Equal(1,DAL.Services.FormsSessionIdSvc.Update(fs));
            Assert.Equal(1, DAL.Services.FormsSessionIdSvc.Delete(fs));

        }
    }
}
