using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.Core;

namespace DAL_UnitTest.Core
{
    public class MetricValueTest: BaseTestClass
    {
        [Fact]
        public void TestGet()
        {
            List<DAL.Models.Metric_Value> mv =
                    DAL.Services.Metric_ValueSvc.GetByDateRange(203, DateTime.Parse("5/15/2018"), DateTime.Parse("5/16/2018"));

            Assert.NotNull(mv);

            mv = DAL.Services.Metric_ValueSvc.GetByShiftDate(203, DateTime.Parse("5/15/2018"), DateTime.Parse("5/16/2018"));

            Assert.NotNull(mv);


        }

        [Fact]
        public void TestInertUpdateDelete()
        {
            DAL.Models.Metric_Value mv = new(MOO.Plant.Minntac, MOO.Area.Agglomerator, DateTime.Parse("1/1/1950"), true, 203, 123);
            //try to insert with datetime before 1980
            //Assert.Throws<Exception>(() => DAL.Services.Metric_ValueSvc.Insert(mv));

            //now set the start_date and start_date_dst to be further than one hour apart
            mv.Start_Date = DateTime.Now;
            mv.Start_Date_No_DST = DateTime.Now.AddHours(5);
            //Assert.Throws<Exception>(() => DAL.Services.Metric_ValueSvc.Insert(mv));

            //now attempt a real insert
            mv = new(MOO.Plant.Minntac, MOO.Area.Agglomerator, DateTime.Now, true, 203, 123);
            DAL.Services.Metric_ValueSvc.Insert(mv);
            //attempt an update
            mv.Value = 456;
            Assert.Equal(1, DAL.Services.Metric_ValueSvc.Update(mv));
            //attempt delete
            Assert.Equal(1, DAL.Services.Metric_ValueSvc.Delete(mv));

            
        }

        [Fact]
        public void TestAudits()
        {
            DAL.Models.Metric_Value mv = new(MOO.Plant.Minntac, MOO.Area.Agglomerator, DateTime.Now.AddMinutes(1), true, 203, 123);
            DAL.Services.Metric_ValueSvc.InsertWithAudit(mv, "mno\\alt7747");
            mv.Value = 999;
            DAL.Services.Metric_ValueSvc.UpdateWithAudit(mv, "mno\\alt7747");
            DAL.Services.Metric_ValueSvc.DeleteWithAudit(mv, "mno\\alt7747");
        }
    }
}
