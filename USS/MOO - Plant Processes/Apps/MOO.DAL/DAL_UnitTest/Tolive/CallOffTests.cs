using MOO.DAL.ToLive.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public class CallOffTests : BaseTestClass
    {

        [Fact]
        public void TestDept()
        {
            DAL.Models.CO_Dept d = new()
            {
                Dept_Name = "DAL_Unit_Test",
                Plant = MOO.Plant.Minntac
            };
            DAL.Services.CO_DeptSvc.Insert(d);
            d.Dept_Name = "test123";
            DAL.Services.CO_DeptSvc.Update(d);
            d = DAL.Services.CO_DeptSvc.Get(d.Dept_Id);
            Assert.NotNull(d);

            var dList = DAL.Services.CO_DeptSvc.GetAll();
            Assert.NotEmpty(dList);

            DAL.Services.CO_DeptSvc.Delete(d);
        }

        [Fact]
        public void TestCallOffDept()
        {
            //get a dept to test with
            var dList = DAL.Services.CO_DeptSvc.GetAll();
            //get a user
            var u = DAL.Services.Sec_UserSvc.Get("mno\\cir2318");
            DAL.Models.CO_User_Dept ud = new()
            {
                Dept = dList[0],
                Sec_User = u,
                Daily_Email = true,
                Freq_Flyer_Email = true,
            };

            DAL.Services.CO_User_DeptSvc.Insert(ud);
            ud = DAL.Services.CO_User_DeptSvc.Get(dList[0], u);
            Assert.NotNull(ud);

            var uList = DAL.Services.CO_User_DeptSvc.GetUsersInDepartment(dList[0]);
            Assert.NotEmpty(uList);

            var udList = DAL.Services.CO_DeptSvc.GetUserDepartments(u);
            Assert.NotEmpty(udList);


            ud.Freq_Flyer_Email = true;
            DAL.Services.CO_User_DeptSvc.Update(ud);
            DAL.Services.CO_User_DeptSvc.Delete(ud);

        }

        [Fact]
        private void TestCallOff()
        {
            var coList = DAL.Services.CO_Call_OffSvc.GetByEnteredDate(DateTime.Today.AddDays(-365), DateTime.Today.AddDays(1));
            Assert.NotEmpty(coList);
            var deptList = DAL.Services.CO_DeptSvc.GetAll();
            //make a test list of departments to test the Get with date and dept
            deptList = deptList.FindAll(x => x.Dept_Id == 9 || x.Dept_Id == 50).ToList();
            coList = DAL.Services.CO_Call_OffSvc.GetByEnteredDate(DateTime.Today.AddDays(-365), DateTime.Today.AddDays(1),deptList);

            //get a dept to test with
            var dList = DAL.Services.CO_DeptSvc.GetAll();
            //get a person
            var p = DAL.Services.PeopleSvc.Get(236706); //Brian Altman

            var rsnList = DAL.Services.CO_ReasonSvc.GetAll();

            DAL.Models.CO_Call_Off co = new()
            {
                Entry_Date = DateTime.Now,
                Dept = dList[0],
                Person = p,
                Start_Date = DateTime.Today,
                Return_Date = DateTime.Today.AddDays(3),
                Comments = "Test",
                Shift1 = true,
                Shifts_Missed=5,
                Esst_Hrs=8
            };
            //add all of the reasons
            foreach (CO_Reason r in rsnList)
                co.Reasons.Add(r);

            DAL.Services.CO_Call_OffSvc.Insert(co);
            co.Comments = "Test 123";
            DAL.Services.CO_Call_OffSvc.Update(co);

           

            co = DAL.Services.CO_Call_OffSvc.Get(co.Call_Off_Id);
            Assert.NotNull(co);

            DAL.Services.CO_Call_OffSvc.Delete(co);
        }
    }
}
