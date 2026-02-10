using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.Pi;

namespace DAL_UnitTest.Pi
{
    public class PiTest
    {
        [Fact]
        public void TestSnapshot()
        {
            DAL.Models.PiSnapshot snap = DAL.Services.PiSnapshotSvc.Get("sinusoid");
            Assert.NotNull(snap);
            snap = DAL.Services.PiSnapshotSvc.Get("AC217100");

            Assert.NotNull(snap);
            snap = DAL.Services.PiSnapshotSvc.Get("MTC_STSP_Wind_Dir_Str");

            Assert.NotNull(snap);
        }

        [Fact]
        public void TestPiTotal()
        {
            List<DAL.Models.PiTotal> pt;
            pt = DAL.Services.PiAggregateSvc.GetPiTotal(DateTime.Now.AddHours(-10), DateTime.Now, Tag: "sinusoid");
            Assert.NotNull(pt);
            pt = DAL.Services.PiAggregateSvc.GetPiTotal(DateTime.Now.AddHours(-10), DateTime.Now, Tag: "sinusoid",TimeStep:"1h");
            Assert.NotNull(pt);

        }


        [Fact]
        public void TestPiAvg()
        {
            List<DAL.Models.PiAvg> p;
            p = DAL.Services.PiAggregateSvc.GetPiAvg(DateTime.Now.AddHours(-10), DateTime.Now, Tag: "sinusoid", TimeStep: "1h");
            Assert.NotNull(p);

        }


        [Fact]
        public void TestPiCalc()
        {
            List<DAL.Models.PiCalc> p;
            p = DAL.Services.PiCalcSvc.Get(DateTime.Now.AddHours(-10), DateTime.Now, "'sinusoid'");
            Assert.NotNull(p);
            
            //Test on a point that returns a calc failed, we will return null values
            p = DAL.Services.PiCalcSvc.Get(DateTime.Parse("1/1/2022"), DateTime.Parse("1/2/2022"), "'Q4-MACT-400107'");
            Assert.NotNull(p);
        }

        [Fact]
        public void TestPiPoint()
        {
            List<DAL.Models.PiPointClassic> ptList = DAL.Services.PiPointClassicSvc.SearchByTag("DI601");
            Assert.NotEmpty(ptList);
            ptList = DAL.Services.PiPointClassicSvc.SearchByPointSource("PI_Targets");
            Assert.NotEmpty(ptList);

            //test insert
            //DAL.Models.PiPointClassic pt = new()
            //{
            //    Tag = "BrianDALTest",
            //    Descriptor = "Brian MOO.DAL Test Point",
            //    PointSource = "M_AGG2_DROP202",
            //    EngUnits = "Deg F",
            //    PointTypeX = "float32",
            //    PtSecurity = "piadmin: A(r,w) | piadmins: A(r,w) | PIWorld: A(r)",
            //    TypicalValue = 0,
            //    Zero = 0,
            //    Span = 200,
            //    Step = false,
            //    ExcDev = 0.2f,
            //    CompDev = 0.4f,
            //    Location1 = 1, 
            //    Location2= 0,
            //    Location3 = 1,
            //    Location4 = 19,
            //    Location5 = 0,
            //    Convers = 0, 
            //    TotalCode = 0,
            //    InstrumentTag = "AI601000.DROP1/51.UNIT0@AGL12.AV"
            //};

            //var pt = DAL.Services.PiPointClassicSvc.InsertAnalogPoint(DAL.Models.PiPointSource.MTC_Agg2, "BrianDALTest", "Brian MOO.DAL Test Point", "AI601000.DROP1/51.UNIT0@AGL12.AV",
            //                                        "GPH", typeof(float), 0, 200, false, true, .1f, true, .2f, DAL.Models.ScanTime.Seconds_30);

            //var pt = DAL.Services.PiPointClassicSvc.InsertDigitalPoint(DAL.Models.PiPointSource.MTC_Agg2, "BrianDALTest", "Brian MOO.DAL Test Point", "DI601036.DROP1/51.UNIT0@AGL12.DS",
            //                "On/Off", DAL.Models.ScanTime.Seconds_30, "OFF/ON");

            //Assert.NotNull(pt);


        }

        [Fact]
        public void TestDigitalStates()
        {
            var dsList = DAL.Services.DigitalStateSvc.GetAll();
            Assert.NotEmpty(dsList);
        }


        [Fact]
        public void TestComp2()
        {
            var sinusoid = DAL.Services.PiComp2Svc.Get(DateTime.Now.AddDays(-1), DateTime.Now, "SINUSOID");
            Assert.NotEmpty(sinusoid);
        }


        [Fact]
        public void TestNOLA_Audit()
        {
            var na = DAL.Services.NOLA_AuditSvc.GetNolaData(DAL.Models.NOLA_Audit.NOLA_Type.NOLA2_Step2, DateTime.Now.AddDays(-10), DateTime.Now);
            Assert.NotEmpty(na);
        }

        [Fact]
        public void TestPSIAudit()
        {
            //DAL.Services.PSI_AuditSvc.InsertPSILabDelta(3, DateTime.Parse("6/28/2025 07:24"));
            var psi = DAL.Services.PSI_AuditSvc.GetPSIData(7, DateTime.Now.AddDays(-5), DateTime.Now);
            Assert.NotEmpty(psi);
        }

        [Fact]
        public void RunPiFunctions()
        {
            var result = DAL.Services.PIFunctions.RunPiFunction("TimeEq ('M21X063-MAN','*','*-30d','Auto') ");
            result = DAL.Services.PIFunctions.RunPiFunction("TimeEq ('DC218004','*','*-365d',1) ");
            Assert.NotNull(result);
            //var result2 = DAL.Services.PIFunctions.GetTimeTrueFromExprSeconds(DateTime.Now.AddDays(-1), DateTime.Now, "'M21X063-MAN' = \"Auto\"", "M21X063-MAN");
            //Assert.NotNull(result2);
        }


        [Fact]
        public void TestPIInterp()
        {
            //var pts = DAL.Services.PiInterpSvc.Get(DateTime.Now.AddDays(-1), DateTime.Now, "sinusoid", "10s");
            var pts = DAL.Services.PiInterpSvc.Get(DateTime.Now.AddDays(-1), DateTime.Now, "Device_100_Whr_3P_IN", "10s");
            Assert.NotEmpty(pts);
        }

        [Fact]
        public void TestPIDig()
        {
            var pts = DAL.Services.PiDigSvc.Get(DateTime.Parse("3/3/2023 12:30"), DateTime.Parse("3/3/2023 13:00"), "DC203046", "10s");
            Assert.NotEmpty(pts);
            //DI613592 is a digital point stored as analog in PI
            pts = DAL.Services.PiDigSvc.Get(DateTime.Now.AddDays(-1), DateTime.Now, "DI613592", "10s");
            Assert.NotEmpty(pts);
        }

        [Fact]
        public void TestInsertUpdate()
        {
            ///test code is commented out.  uncomment items you want to test
            ///inserting.....
            //DAL.Services.PiPointClassicSvc.InsertFuturePoint("AAABrianTest", "Test Point From MOO.DAL Unit Test", "tons", 0, 100, "Created for testing", "PI_Targets");
            
            ///updating....
            //var pt = DAL.Services.PiPointClassicSvc.Get("AAABrianTest");
            //pt.Descriptor = "Test Updating";
            //DAL.Services.PiPointClassicSvc.Update(pt);
        }

        [Fact]
        public void TestPiPlot()
        {
            var plot = DAL.Services.PiPlotSvc.Get(DateTime.Now.AddHours(-8), DateTime.Now, "AC204000", 1000);
            Assert.NotEmpty(plot);
        }
    }
}
