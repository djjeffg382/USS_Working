using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public class ScheduleTest : BaseTestClass
    {
        [Fact]
        public void TestSchedule()
        {
            //DAL.Models.ScheduleAssign sa = new()
            //{
            //    Assign_Id = 7,
            //    End_Date = DateTime.Parse("11/29/2021 12:00:00 AM"),
            //    Start_Date = DateTime.Parse("11/28/2021 12:00:00 AM"),
            //    Group_Id = 3,
            //    Short_View = "Plants - Terry R Crayne;Troy C Barteck",
            //    Long_View = "Plants - Terry R Crayne (Home#, Mobile#, Ext#) / Troy C Barteck (Home#, Mobile#, Ext#)",
            //    People_Assign = "48658; 48727",
            //    Sched_Name = "Weekend Duty"

            //};

            //DAL.Services.ScheduleAssignSvc.Update(sa);

            //DAL.Services.ScheduleAssignSvc.Delete(sa);

            //var ScheduleNames = DAL.Services.ScheduleGroupsSvc.GetAll();

            //var x = DAL.Services.ScheduleAssignSvc.GetAll(0, "", DateTime.Parse("12/01/2021 00:00:00"), DateTime.Parse("12/31/2021 23:59:59"));
            _ = DAL.Services.ScheduleAssignSvc.Get(3, DateTime.Parse("01/01/2022 00:00:00"), DateTime.Parse("01/02/2022 23:59:59"));


            //var x = DAL.Services.ScheduleAssignSvc.GetAll(0, "Weekend Duty");
            //var y = DAL.Services.ScheduleGroupsSvc.GetAll();
            //var t = 0;
        }
    }
}
