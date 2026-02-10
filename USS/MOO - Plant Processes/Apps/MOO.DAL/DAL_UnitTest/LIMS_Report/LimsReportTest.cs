using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.LIMS_Report;

namespace DAL_UnitTest.LIMS_Report
{
    public class LimsReportTest : BaseTestClass
    {
        [Fact]
        public void OilReultsTest()
        {
            var o = DAL.Services.Oil_ResultsSvc.GetBySampleDate(DateTime.Parse("11/29/2018"), DateTime.Parse("11/30/2018"));
            Assert.NotNull(o);
            o[0].Status = "testing";
            o[0].Equip_Id = "abc";
            o[0].Component_Id = 123;
            o[0].Unit_Type = 10;

            DAL.Services.Oil_ResultsSvc.Update(o[0]);

            var r = new Random();
            o[0].Sample_Nbr = r.Next() * -1;
            DAL.Services.Oil_ResultsSvc.Insert(o[0]);
        }


        [Fact]
        public void WaterReultsTest()
        {
            var o = DAL.Services.Water_ResultsSvc.GetBySampleDate(DateTime.Parse("3/20/2018"), DateTime.Parse("3/25/2018"));
            Assert.NotNull(o);
            //o[0].Status = "testing";
            //DAL.Services.Water_ResultsSvc.Update(o[0]);


            //o[0].Sample_Nbr = -1;
            //DAL.Services.Water_ResultsSvc.Insert(o[0]);

        }


        [Fact]
        public void K_ConcTest()
        {

            Random rnd = new();

            DAL.Models.K_Conc conc = new()
            {
                Sampled_Date = DateTime.Now,
                Sample_Nbr = rnd.Next() * -1,
                Login_Date = DateTime.Now,
                Shift_Date12 = DateTime.Today,
                Shift_Date8 = DateTime.Today,
                Shift_Nbr12 = 1,
                Shift_Nbr8 = 1,
                Status = "Test",
                Sample_Point = "TestPoint"
            };
            DAL.Services.K_ConcSvc.Insert(conc);

            var abc = DAL.Services.K_ConcSvc.GetBySampleDate(DateTime.Today, DateTime.Now);
            Assert.NotNull(abc);

            conc.Al2O3 = 123;
            DAL.Services.K_ConcSvc.Update(conc);


        }



        [Fact]
        public void K_PellTest()
        {

            Random rnd = new();

            DAL.Models.K_Pellet p = new()
            {
                Sampled_Date = DateTime.Now,
                Sample_Nbr = rnd.Next() * -1,
                Login_Date = DateTime.Now,
                Shift_Date12 = DateTime.Today,
                Shift_Date8 = DateTime.Today,
                Shift_Nbr12 = 1,
                Shift_Nbr8 = 1,
                Status = "Test",
                Sample_Point = "TestPoint"
            };
            DAL.Services.K_PelletSvc.Insert(p);

            var abc = DAL.Services.K_PelletSvc.GetBySampleDate(DateTime.Today, DateTime.Now);
            Assert.NotNull(abc);

            p.Al2O3 = 123;
            DAL.Services.K_PelletSvc.Update(p);


        }



        [Fact]
        public void K_PellXRFTest()
        {

            Random rnd = new();

            DAL.Models.K_XRF_PellShift_Std p = new()
            {
                Sampled_Date = DateTime.Now,
                Sample_Nbr = rnd.Next() * -1,
                Login_Date = DateTime.Now,
                Status = "Test",
                Sample_Point = "TestPoint"
            };
            DAL.Services.K_XRF_PellShift_StdSvc.Insert(p);

            var abc = DAL.Services.K_XRF_PellShift_StdSvc.GetBySampleDate(DateTime.Today, DateTime.Now);
            Assert.NotNull(abc);

            p.Al2O3 = 123;
            DAL.Services.K_XRF_PellShift_StdSvc.Update(p);


        }


        [Fact]
        public void K_BlastTest()
        {

            Random rnd = new();

            DAL.Models.K_Blast_Sample p = new()
            {
                Sampled_Date = DateTime.Now,
                Sample_Nbr = rnd.Next() * -1,
                Login_Date = DateTime.Now,
                Status = "Test",
                Sample_Point = "TestPoint",
                Parent_Sample_Nbr = rnd.Next() * -1,
                Pattern_Number = rnd.Next().ToString(),
                Hole_Number = rnd.Next().ToString()
            };
            DAL.Services.K_Blast_SampleSvc.Insert(p);

            var abc = DAL.Services.K_Blast_SampleSvc.GetBySampleDate(DateTime.Today, DateTime.Now);
            Assert.NotNull(abc);

            p.Fe2O3 = 123;
            DAL.Services.K_Blast_SampleSvc.Update(p);


        }
    }
}
