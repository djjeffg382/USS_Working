using MOO.DAL.ToLive.Models;
using System;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public class MineSchedTest
    {
        [Fact]
        public void SchedTest()
        {
            var sched = DAL.Services.Mine_SchedSvc.Get(9);
            Assert.Equal("East",sched.Blast_Location);

            var scheds = DAL.Services.Mine_SchedSvc.GetAll();
            Assert.NotEmpty(scheds);

            Mine_Sched newSched = new()
            {
                Plant = MOO.Plant.Keetac,
                Sched_Date = DateTime.Today,
                Blast_Location = "East",
                Blast_Delay = "Blast Delay Description",
                Maintenance = "Maintenance Description",
                Road_Maint = "Road Maintenance Description",
                Dump_Maint = "Dump Maintenance Description",
            };

            //Assert.Equal(1, DAL.Services.Mine_SchedSvc.Insert(newSched));

            newSched.Blast_Location = "West";
            //Assert.Equal(1, DAL.Services.Mine_SchedSvc.Update(newSched));

            scheds = DAL.Services.Mine_SchedSvc.GetByMineSchedDate(DateTime.Today, DateTime.Today.AddDays(1));
            Assert.NotEmpty(scheds);
        }
    }
}
