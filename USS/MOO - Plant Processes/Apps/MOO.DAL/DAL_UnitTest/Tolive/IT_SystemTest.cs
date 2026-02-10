using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public class IT_SystemTest
    {
        [Fact]
        public void TestSystem()
        {
            DAL.Models.IT_System sys = new()
            {
                System_Name = "Test",
                Other_Documents = "aaaa",
                System_Type = DAL.Models.IT_System.ItSystemType.BlazorWeb,
                Running_On="MNO-WEB"
            };

            DAL.Services.IT_SystemSvc.Insert(sys);

            var lst = DAL.Services.IT_SystemSvc.GetAll();
            Assert.NotEmpty(lst);
            lst[0].System_Name = "Test Update";
            DAL.Services.IT_SystemSvc.Update(sys);
            var obj = DAL.Services.IT_SystemSvc.Get(lst[0].It_System_Id);
            Assert.NotNull(obj);

            DAL.Services.IT_SystemSvc.Delete(obj);
        }
        [Fact]
        public void TestLog()
        {
            DAL.Models.IT_System_Log log = new()
            {
                It_System = DAL.Services.IT_SystemSvc.Get(25),
                LogDate = DateTime.Now,
                LogChange = "Some Change",
                ChangeSet = "123",
                ChangedBy = "Brian"
            };
            Assert.Equal(1, DAL.Services.IT_System_LogSvc.Insert(log));
            log = DAL.Services.IT_System_LogSvc.Get(log.It_Log_Id);
            log.LogChange = "Different Change";
            Assert.Equal(1, DAL.Services.IT_System_LogSvc.Update(log));

            var logs = DAL.Services.IT_System_LogSvc.GetBySystem(log.It_System.It_System_Id);
            Assert.NotEmpty(logs);
            Assert.Equal(1, DAL.Services.IT_System_LogSvc.Delete(log));
        }
    }
}
