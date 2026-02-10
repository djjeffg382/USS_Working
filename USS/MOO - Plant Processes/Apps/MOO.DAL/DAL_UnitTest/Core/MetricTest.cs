using MOO.DAL.Core.Services;
using MOO.DAL.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.Core;
using MOO.DAL.Core.Enums;

namespace DAL_UnitTest.Core
{
    public class MetricTest : BaseTestClass
    {
        [Fact]

        public void MetricTesting()
        {
            var get = MetricSvc.Get(601);
            Assert.NotNull(get);
            var getAll = MetricSvc.GetAll();
            Assert.NotNull(getAll);

            var mt = new Metric()
            {
                Metric_Id = 2002,
                Metric_Name = "test2",
                Tag_Name = "test2",
                Metric_Comments = "tes2t",
                Uom = new Uom()
                {
                    Uom_Id = 4,
                },
                Metric_Type = Metric_Type.Average,
                Coll_Time = new Collection_Time()
                {
                    Coll_Time_Id = 1,
                },
                Coll_Type = new Collection_Type()
                {
                    Coll_Type_Id = 1,
                },
                Process_Level = Process_LevelSvc.GetAll().FirstOrDefault(),
                Approvable = 3,
                Isactive = true,
                UserListCSV = "alt7747"
                
            };
            MetricSvc.Insert(mt);
            mt.Metric_Name = "Test1234";
            MetricSvc.Update(mt);
            MetricSvc.Delete(mt);
        }


        [Fact]
        public void TestMetricValueAdjustments()
        {
            var adjMetrics = DAL.Services.MetricSvc.GetMetricValueAdjustments();
            Assert.NotEmpty(adjMetrics);
            var val = DAL.Services.MetricSvc.GetMetricValueAdjustmentValue(adjMetrics[0], DateTime.Parse("4/5/2024"));
            Assert.NotNull(val);
        }

        [Fact]
        public void testMetricSecurity()
        {
            var m = DAL.Services.MetricSvc.Get(795);
            Assert.NotNull(m);
            //we will test this on the Conc Shell liner width metric
            //get a role.
            var r = MOO.DAL.ToLive.Services.Sec_RoleSvc.GetAll(true);
            //m.RoleList.Add(r[0].Role_Name);
            //DAL.Services.MetricSvc.Update(m);
            //now test removing the role and updating
            m.RoleList.Clear();
            DAL.Services.MetricSvc.Update(m);
        }

        //public void TestUom()
        //{
        //    var u = DAL.Services.UomSvc.GetAll();
        //    Assert.NotNull(u);
        //}
        //[Fact]
        //public void TestCollType()
        //{
        //    var c = DAL.Services.Collection_TypeSvc.GetAll();
        //    Assert.NotNull(c);
        //}
        //[Fact]
        //public void TestCollTime()
        //{
        //    var c = DAL.Services.Collection_TimeSvc.GetAll();
        //    Assert.NotNull(c);
        //}
        //[Fact]
        //public void TestProcLevel()
        //{
        //    var p = DAL.Services.Process_LevelSvc.GetAll();
        //    Assert.NotNull(p);
        //}
        //[Fact]
        //public void TestMetric()
        //{
        //    var m = DAL.Services.MetricSvc.GetAll();
        //    Assert.NotNull(m);
        //}
    }
}
