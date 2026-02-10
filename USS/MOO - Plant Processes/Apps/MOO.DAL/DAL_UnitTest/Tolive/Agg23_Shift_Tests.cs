using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    /// <summary>
    /// class for running tests on the AGG3_SHIFT* and AGG2_SHIFT* Tables
    /// </summary>
    public class Agg23_Shift_Tests :BaseTestClass
    {
        [Fact]
        public void Agg2Shift_Get_Test()
        {

            //Testing for Agg2_Shift Table
            //test get function with null values 
            DAL.Models.Agg2_Shift ag = DAL.Services.Agg2_ShiftSvc.Get(1);
            Assert.NotNull(ag);
            ag = DAL.Services.Agg2_ShiftSvc.Get(DateTime.Parse("1/2/2020"), 2);
            Assert.NotNull(ag);

            //test get function with good values
            //ag = DAL.Services.Agg2_ShiftSvc.Get(171298);
            //Assert.NotNull(ag);

            //test get function with date
            DateTime dt = DateTime.Parse("2017-12-13");
            ag = DAL.Services.Agg2_ShiftSvc.Get(dt, 1);
            Assert.NotNull(ag);

        }

        [Fact]
        public void Agg3Shift_Get_Test()
        {

            //Testing for Agg2_Shift Table
            //test get function with null values 
            DAL.Models.Agg3_Shift ag = DAL.Services.Agg3_ShiftSvc.Get(1);
            Assert.NotNull(ag);
            ag = DAL.Services.Agg3_ShiftSvc.Get(DateTime.Parse("12/8/2021"),1);
            Assert.NotNull(ag);

            //test get function with good values
            ag = DAL.Services.Agg3_ShiftSvc.Get(171298);
            Assert.NotNull(ag);

            //test get function with date
            DateTime dt = DateTime.Parse("1/1/2018");
            ag = DAL.Services.Agg3_ShiftSvc.Get(dt, 1);
            Assert.NotNull(ag);

        }



        [Fact]
        public void Agg2Shift_Update_Test()
        {
            DateTime dt = DateTime.Parse("1/1/2018");
            DAL.Models.Agg2_Shift ag = DAL.Services.Agg2_ShiftSvc.Get(dt, 1);
            Random r = new();
            var in_adj = r.Next(1000);
            var out_adj = r.Next(1000);

            ag.Recl_In_Adj_Ltons = in_adj;
            ag.Recl_Out_Adj_Ltons = out_adj;
            DAL.Services.Agg2_ShiftSvc.UpdateFromShiftInput(ag);
            //now read it back from the database and ensure it is updated
            ag = DAL.Services.Agg2_ShiftSvc.Get(dt, 1);
            Assert.Equal(in_adj, ag.Recl_In_Adj_Ltons);
            Assert.Equal(out_adj, ag.Recl_Out_Adj_Ltons);


        }

        [Fact]
        public void Agg3Shift_Update_Test()
        {
            DateTime dt = DateTime.Parse("1/1/2018");
            DAL.Models.Agg3_Shift ag = DAL.Services.Agg3_ShiftSvc.Get(dt, 1);
            Random r = new();
            var in_adj = r.Next(1000);
            var out_adj = r.Next(1000);

            ag.Recl_In_Adj_Ltons = in_adj;
            ag.Recl_Out_Adj_Ltons = out_adj;
            DAL.Services.Agg3_ShiftSvc.UpdateFromShiftInput(ag);
            ag = DAL.Services.Agg3_ShiftSvc.Get(dt, 1);
            Assert.Equal(in_adj, ag.Recl_In_Adj_Ltons);
            Assert.Equal(out_adj, ag.Recl_Out_Adj_Ltons);
        }


        [Fact]
        public void Agg2ShiftStep_Get_Test()
        {
            //test record with a lot of nulls
            DAL.Models.Agg2_Shift_Step ag = DAL.Services.Agg2_Shift_StepSvc.Get(1,2);
            Assert.NotNull(ag);

            //test record with a lot of data filled out
            ag = DAL.Services.Agg2_Shift_StepSvc.Get(171298, 2);
            Assert.NotNull(ag);

            //test get data by date
            DateTime dt = DateTime.Parse("1/1/2018");
            ag = DAL.Services.Agg2_Shift_StepSvc.Get(dt, 1, 2);
            Assert.NotNull(ag);
        }

        [Fact]
        public void Agg3ShiftStep_Get_Test()
        {
            //test record with a lot of nulls
            DAL.Models.Agg3_Shift_Step ag = DAL.Services.Agg3_Shift_StepSvc.Get(1, 3);
            Assert.NotNull(ag);

            //test record with a lot of data filled out
            ag = DAL.Services.Agg3_Shift_StepSvc.Get(171298, 3);
            Assert.NotNull(ag);

            //test get data by date
            DateTime dt = DateTime.Parse("1/1/2018");
            ag = DAL.Services.Agg3_Shift_StepSvc.Get(dt, 1, 3);
            Assert.NotNull(ag);
        }


        [Fact]
        public void Agg2ShiftStep_Update_Test()
        {
            DateTime dt = DateTime.Parse("1/1/2015");
            DAL.Models.Agg2_Shift_Step ag = DAL.Services.Agg2_Shift_StepSvc.Get(dt, 1, 2);

            Random r = new();
            var favail = r.Next(20);
            var fon = r.Next(20);
            var frate = r.Next(20);
            ag.Filter_Avail = favail;
            ag.Filter_On = fon;
            ag.FiltRate = frate;
            DAL.Services.Agg2_Shift_StepSvc.UpdateFromShiftInput(ag);
            //now read back from DB and ensure it changed
            ag = DAL.Services.Agg2_Shift_StepSvc.Get(dt, 1, 2);
            Assert.Equal(favail, ag.Filter_Avail);
            Assert.Equal(fon, ag.Filter_On);
            Assert.Equal(frate, ag.FiltRate);
        }

        [Fact]
        public void Agg3ShiftStep_Update_Test()
        {
            DateTime dt = DateTime.Parse("1/1/2015");
            DAL.Models.Agg3_Shift_Step ag = DAL.Services.Agg3_Shift_StepSvc.Get(dt, 1, 3);
            Random r = new();
            var favail = r.Next(20);
            var fon = r.Next(20);
            var frate = r.Next(20);
            ag.Filter_Avail = favail;
            ag.Filter_On = fon;
            ag.FiltRate = frate;
            DAL.Services.Agg3_Shift_StepSvc.UpdateFromShiftInput(ag);
            //now read back from DB and ensure it changed
            ag = DAL.Services.Agg3_Shift_StepSvc.Get(dt, 1, 3);
            Assert.Equal(favail, ag.Filter_Avail);
            Assert.Equal(fon, ag.Filter_On);
            Assert.Equal(frate, ag.FiltRate);
        }

        [Fact]
        public void Agg2ShiftLine_Get_Test()
        {
            //test get function with null values 
            DAL.Models.Agg2_Shift_Line ag = DAL.Services.Agg2_Shift_LineSvc.Get(1, 3);
            Assert.NotNull(ag);

            //test get function with good values
            ag = DAL.Services.Agg2_Shift_LineSvc.Get(171298, 3);
            Assert.NotNull(ag);

            //test get function with date
            DateTime dt = DateTime.Parse("1/1/2018");
            ag = DAL.Services.Agg2_Shift_LineSvc.Get(dt, 1, 3);
            Assert.NotNull(ag);

        }

        [Fact]
        public void Agg3ShiftLine_Get_Test()
        {

            var a = MOO.DAL.ToLive.Services.Agg3_Shift_LineSvc.Get(256809, 6);

            //test get function with null values 
            DAL.Models.Agg3_Shift_Line ag = DAL.Services.Agg3_Shift_LineSvc.Get(1, 6);
            Assert.NotNull(ag);

            //test get function with good values
            ag = DAL.Services.Agg3_Shift_LineSvc.Get(171298, 6);
            Assert.NotNull(ag);

            //test get function with date
            DateTime dt = DateTime.Parse("1/1/2018");
            ag = DAL.Services.Agg3_Shift_LineSvc.Get(dt, 1, 6);
            Assert.NotNull(ag);

        }

        [Fact]
        public void Agg2ShiftLine_Update_Test()
        {

            DAL.Models.Agg2_Shift_Line ag = DAL.Services.Agg2_Shift_LineSvc.Get(2500, 3);
            Random r = new();
            //modifying any of the pellet tons fields should always keep the calculation correct
            ag.Pel_Adj_Ltons = r.Next(1000);
            Assert.Equal(ag.Pel_Ltons, ag.Pel_Act_Ltons + ag.Pel_Adj_Ltons);
            ag.Pel_Ltons = r.Next(1000);
            Assert.Equal(ag.Pel_Ltons, ag.Pel_Act_Ltons + ag.Pel_Adj_Ltons);
            ag.Pel_Act_Ltons = r.Next(1000);
            Assert.Equal(ag.Pel_Ltons, ag.Pel_Act_Ltons + ag.Pel_Adj_Ltons);

            ag = DAL.Services.Agg2_Shift_LineSvc.Get(2500, 3);
            var bin = r.Next(100);
            var generalDesc = "Test" + r.Next(1000);


            ag.Bin_Pct = bin;
            ag.General_Desc = generalDesc;
            ag.Pel_Adj_Ltons = null;
            DAL.Services.Agg2_Shift_LineSvc.UpdateFromShiftInput(ag);

            ag.Pel_Adj_Ltons = 0;
            DAL.Services.Agg2_Shift_LineSvc.UpdateFromShiftInput(ag);

            var peladj = r.Next(1000);
            ag.Pel_Adj_Ltons = peladj;
            DAL.Services.Agg2_Shift_LineSvc.UpdateFromShiftInput(ag);

            //now get data from DB and ensure it changed
            ag = DAL.Services.Agg2_Shift_LineSvc.Get(2500, 3);
            Assert.Equal(bin, ag.Bin_Pct);
            Assert.Equal(generalDesc, ag.General_Desc);
            Assert.Equal(peladj, ag.Pel_Adj_Ltons);

        }

        [Fact]
        public void Agg3ShiftLine_Update_Test()
        {

            DAL.Models.Agg3_Shift_Line ag = DAL.Services.Agg3_Shift_LineSvc.Get(2500, 6);
            Random r = new();
            //modifying any of the pellet tons fields should always keep the calculation correct
            ag.Pel_Adj_Ltons = r.Next(1000);
            Assert.Equal(ag.Pel_Ltons, ag.Pel_Act_Ltons + ag.Pel_Adj_Ltons);
            ag.Pel_Ltons = r.Next(1000);
            Assert.Equal(ag.Pel_Ltons, ag.Pel_Act_Ltons + ag.Pel_Adj_Ltons);
            ag.Pel_Act_Ltons = r.Next(1000);
            Assert.Equal(ag.Pel_Ltons, ag.Pel_Act_Ltons + ag.Pel_Adj_Ltons);

            ag = DAL.Services.Agg3_Shift_LineSvc.Get(2500, 6);
            var bin = r.Next(100);
            var generalDesc = "Test" + r.Next(1000);

            ag.Bin_Pct = bin;
            ag.General_Desc = generalDesc;
            ag.Pel_Adj_Ltons = null;
            DAL.Services.Agg3_Shift_LineSvc.UpdateFromShiftInput(ag);

            ag.Pel_Adj_Ltons = 0;
            DAL.Services.Agg3_Shift_LineSvc.UpdateFromShiftInput(ag);

            var peladj = r.Next(1000);
            ag.Pel_Adj_Ltons = peladj;
            DAL.Services.Agg3_Shift_LineSvc.UpdateFromShiftInput(ag);


            //now get data from DB and ensure it changed
            ag = DAL.Services.Agg3_Shift_LineSvc.Get(2500, 6);
            Assert.Equal(bin, ag.Bin_Pct);
            Assert.Equal(generalDesc, ag.General_Desc);
            Assert.Equal(peladj, ag.Pel_Adj_Ltons);

        }

        [Fact]
        public void Agg2DayGet()
        {
            var ad = DAL.Services.Agg2_DaySvc.Get(DateTime.Parse("1/1/2020"));
            Assert.NotNull(ad);
        }


        [Fact]
        public void Agg3DayGet()
        {
            var ad = DAL.Services.Agg3_DaySvc.Get(DateTime.Parse("1/1/2020"));
            Assert.NotNull(ad);
        }
    }
}
