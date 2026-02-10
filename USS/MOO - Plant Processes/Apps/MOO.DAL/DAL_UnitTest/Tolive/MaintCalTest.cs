using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public class MaintCalTest : BaseTestClass
    {

        [Fact]
        public void Maint_CalTest()
        {
            var evt = new DAL.Models.Maint_Calendar()
            {
                Title = "Test Event",
                StartDate = DateTime.Today.AddDays(-1),
                EndDate = DateTime.Today,
                Area = DAL.Enums.MaintCalArea.SUPP,
                Description = "AAAAA",
                Major = false
            };
            DAL.Services.Maint_CalendarSvc.Insert(evt);
            //inserted the record now call get on the record to test the get
            evt = DAL.Services.Maint_CalendarSvc.Get(evt.Event_Id);
            Assert.NotNull(evt);


            var evtList = DAL.Services.Maint_CalendarSvc.GetAll(DateTime.Today.AddDays(-10), DateTime.Today);
            Assert.NotEmpty(evtList);

            evt.Description = "BBBBB";
            DAL.Services.Maint_CalendarSvc.Update(evt);

            DAL.Services.Maint_CalendarSvc.Delete(evt);
        }
    }

}
