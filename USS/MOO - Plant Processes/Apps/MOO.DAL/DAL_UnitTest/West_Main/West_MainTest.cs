using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.DAL.West_Main;
using MOO.DAL.West_Main.Models;
using Xunit;
using DAL = MOO.DAL.West_Main;

namespace DAL_UnitTest.West_Main
{
    public class West_MainTest : BaseTestClass
    {
        [Fact] 
        public void TestWestHourly()
        {
            var wh = DAL.Services.West_HourlySvc.GetYMDRange(WestMainPlants.Agg2, 123, 20150101, 20150102);
            Assert.NotEmpty(wh);

            DAL.Models.West_Hourly whNew = new()
            {
                Id = 123,
                Hour = 1,
                Half = 1,
                Hour_Total = 123,
                N_Count = 123,
                Plant = WestMainPlants.Agg2,
                Shift = 1,
                TimeStamp = DateTime.Now,
                Ymd = 20000101
            };
            DAL.Services.West_HourlySvc.Insert(whNew);
            whNew.Hour_Total = 456;
            DAL.Services.West_HourlySvc.Update(whNew);
            DAL.Services.West_HourlySvc.Delete(whNew);


        }

        [Fact]
        public void TestWestShift()
        {
            var ws = DAL.Services.West_ShiftSvc.GetYMDRange(WestMainPlants.Agg2, 123, 20150101, 20150102);
            Assert.NotEmpty(ws);

            DAL.Models.West_Shift wsNew = new()
            {
                Id = 123,
                Shift_Total = 123,
                N_Count = 123,
                Plant = WestMainPlants.Agg2,
                Shift = 1,
                TimeStamp = DateTime.Now,
                Ymd = 20000101
            };
            DAL.Services.West_ShiftSvc.Insert(wsNew);
            wsNew.Shift_Total = 456;
            DAL.Services.West_ShiftSvc.Update(wsNew);
            DAL.Services.West_ShiftSvc.Delete(wsNew);


        }


        [Fact]
        public void TestWestDaily()
        {
            var wd = DAL.Services.West_DailySvc.GetYMDRange(WestMainPlants.Agg2, 123, 20150101, 20150102);
            Assert.NotEmpty(wd);

            DAL.Models.West_Daily wdNew = new()
            {
                Id = 123,
                Day_Total = 123,
                N_Count = 123,
                Plant = WestMainPlants.Agg2,
                TimeStamp = DateTime.Now,
                Ymd = 20000101
            };
            DAL.Services.West_DailySvc.Insert(wdNew);
            wdNew.Day_Total = 456;
            DAL.Services.West_DailySvc.Update(wdNew);
            DAL.Services.West_DailySvc.Delete(wdNew);


        }

        [Fact]
        public void TestAnalogPoints()
        {
            DAL.Models.Analog_Points ap = new()
            {
                Plant = MOO.Plant.Minntac,
                Area = "CONC",
                Tag = "abc123",
                Description = "Hello World",
                Location = "aaaaa",
                UOM = "ltons",
                Min = 0,
                Max = 100
            };
            DAL.Services.Analog_PointsSvc.Insert(ap);
            ap.UOM = "gph";
            DAL.Services.Analog_PointsSvc.Update(ap);

            List<DAL.Models.Analog_Points> apList = DAL.Services.Analog_PointsSvc.SearchByTagName("abc");
            Assert.NotEmpty(apList);
            apList = DAL.Services.Analog_PointsSvc.SearchByDescription("hello");
            Assert.NotEmpty(apList);

            DAL.Services.Analog_PointsSvc.Delete(ap);
        }

        
        [Fact]
        public void TestDigitalPoints()
        {
            DAL.Models.Digital_Points dp = new()
            {
                Plant = MOO.Plant.Minntac,
                Area = "CONC",
                Tag = "abc123",
                Description = "Hello World",
                Location = "aaaaa",
                On_Val = "On",
                Off_Val = "Off"
            };
            DAL.Services.Digital_PointsSvc.Insert(dp);
            dp.On_Val = "OFF";
            DAL.Services.Digital_PointsSvc.Update(dp);

            List<DAL.Models.Digital_Points> apList = DAL.Services.Digital_PointsSvc.SearchByTagName("abc");
            Assert.NotEmpty(apList);
            apList = DAL.Services.Digital_PointsSvc.SearchByDescription("hello");
            Assert.NotEmpty(apList);

            DAL.Services.Digital_PointsSvc.Delete(dp);
        }

        [Fact]
        public void TestVMSAnalog()
        {

            var vaList = DAL.Services.VMS_AnalogSvc.GetAll();
            DAL.Models.VMS_Analog va = new()
            {
                Plant = WestMainPlants.Crush,
                Point_Id = 9999,
                Description = "Test Point"

            };

            DAL.Services.VMS_AnalogSvc.Insert(va);
            va.Description = "TestPt";
            va = DAL.Services.VMS_AnalogSvc.Get(va.Plant, va.Point_Id);

            DAL.Services.VMS_AnalogSvc.Update(va);

            Assert.NotEmpty(vaList);

            DAL.Services.VMS_AnalogSvc.Delete(va);
        }
        [Fact]
        public void TestVMSDigital()
        {

            var vaList = DAL.Services.VMS_DigitalSvc.GetAll();
            DAL.Models.VMS_Digital vd = new()
            {
                Plant = WestMainPlants.Agg2,
                Point_Id = -9999,
                Point_Name = "DI201157",
                Description = "Test Point"

            };

            DAL.Services.VMS_DigitalSvc.Insert(vd);
            vd.Description = "TestPt";
            vd = DAL.Services.VMS_DigitalSvc.Get(vd.Plant, vd.Point_Id);

            DAL.Services.VMS_DigitalSvc.Update(vd);

            Assert.NotEmpty(vaList);

            DAL.Services.VMS_DigitalSvc.Delete(vd);
        }

        [Fact]
        public void TestGetDependencies()
        {
            var va = DAL.Services.VMS_AnalogSvc.Get(WestMainPlants.Crush, 374);

            var vaList = DAL.Services.VMS_AnalogSvc.GetDependencies(va);
            Assert.NotEmpty(vaList);


            //check related contact - 153 should be a related contact
            var vd = DAL.Services.VMS_DigitalSvc.Get(WestMainPlants.Crush, 153);
            var vdList = vd.GetDependencies();
            Assert.NotEmpty(vdList);


            //check Pnt# - 2297 should be a related point for Op Time equation type
            vd = DAL.Services.VMS_DigitalSvc.Get(WestMainPlants.Crush, 2297);
            vdList = vd.GetDependencies();
            Assert.NotEmpty(vdList);

        }

        [Fact]
        public void TestRandomCrap()
        {
            var vaList = DAL.Services.VMS_AnalogSvc.GetAll();

            var a = new DAL.Models.VMS_Analog();
            a.ProcessPoints[0] = 1;
            



            var test = DAL.Models.VMS_Analog.GetBitArray(7);

            Assert.NotEmpty(test); 


        }
    }
}
