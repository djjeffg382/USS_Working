using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.Drill;

namespace DAL_UnitTest.Drill
{
    public class DrillTests: BaseTestClass
    {
        [Fact]
        public void MiscTests()
        {
            var h = DAL.Services.Drilled_HoleSvc.GetByPattern(328);
            Assert.NotNull(h);

            h = DAL.Services.Drilled_HoleSvc.GetByPattern(MOO.Plant.Minntac, "13165");
            Assert.NotEmpty(h);
        }


        [Fact]
        public void TestEquipHrsImport()
        {
            DAL.Imports.HoursImport imp = new(MOO.Plant.Minntac);

            imp.ImportHours( DateTime.Parse("3/31/2021"), 2);
            //imp.ImportHours(DateTime.Parse("1/1/2022"), DateTime.Today);
        }


        [Fact]
        public void TestDrilledHoleStatus()
        {
            DAL.Models.Drilled_Hole_Status dhs = new()
            {
                Drilled_Hole_Id = 51949,
                Shift_Date = DateTime.Today,
                Shift = 1,
                Start_Time = DateTime.Now,
                End_Time = DateTime.Now,
                StatusBucket = DAL.Models.Drilled_Hole_Status.StatusType.OpDelay,
                StatusCode = "O10",
                Plant = MOO.Plant.Minntac                


            };
            DAL.Services.Drilled_Hole_StatusSvc.Insert(dhs);
            dhs = DAL.Services.Drilled_Hole_StatusSvc.Get(dhs.Drilled_Hole_Id, dhs.Start_Time);
            Assert.NotNull(dhs);


            dhs.StatusCode = "AA";
            DAL.Services.Drilled_Hole_StatusSvc.Update(dhs);

            DAL.Services.Drilled_Hole_StatusSvc.Delete(dhs);
        }

        [Fact]
        public void TestEquipHrs()
        {
            DAL.Models.Equip_Hours eh = new()
            {
                Equip = DAL.Services.EquipSvc.Get(MOO.Plant.Minntac, "0060"),
                Maint = 1,
                Oper = 2,
                Sched = 3,
                OperDelay = 4,
                MgmtDec = 5,
                Standby = 6,
                ShiftDate = DateTime.Today,
                Shift = 1

            };
            DAL.Services.Equip_HoursSvc.Insert(eh);
            eh = DAL.Services.Equip_HoursSvc.Get(eh.Equip,eh.ShiftDate,eh.Shift);
            Assert.NotNull(eh);


            eh.Maint = 22;
            DAL.Services.Equip_HoursSvc.Update(eh);

            List<DAL.Models.Equip_Hours> psList = DAL.Services.Equip_HoursSvc.GetByDateRange(MOO.Plant.Minntac,DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1));
            Assert.NotEmpty(psList);
            DAL.Services.Equip_HoursSvc.Delete(eh);
        }

        [Fact]
        public void TestPatternSize()
        {
            DAL.Models.Pattern_Size ps = new()
            {
                Plant = MOO.Plant.Minntac,
                Description= "Test",
                Holes_Width = 10,
                Holes_Length = 20,
                LTons_Per_Foot = 100
            };
            DAL.Services.Pattern_SizeSvc.Insert(ps);
            ps = DAL.Services.Pattern_SizeSvc.Get(ps.Pattern_Size_Id);
            Assert.NotNull(ps);


            ps.Description = "1111111";
            DAL.Services.Pattern_SizeSvc.Update(ps);

            List<DAL.Models.Pattern_Size> psList = DAL.Services.Pattern_SizeSvc.GetAll(MOO.Plant.Minntac);
            Assert.NotEmpty(psList);
            DAL.Services.Pattern_SizeSvc.Delete(ps);
        }

        [Fact]
        public void TestDrillBits()
        {

            var dbListFiltered = DAL.Services.Drill_BitSvc.GetActiveByDateRange(MOO.Plant.Minntac, DateTime.Parse("1/1/2019"), DateTime.Parse("1/1/2020"));
            Assert.NotNull(dbListFiltered);
            DAL.Models.Drill_Bit db = new()
            {
                Plant = MOO.Plant.Minntac,
                Serial_Number = "123456"
            };
            DAL.Services.Drill_BitSvc.Insert(db);
            db =DAL.Services.Drill_BitSvc.Get(db.Drill_Bit_Id);
            Assert.NotNull(db);

            db = DAL.Services.Drill_BitSvc.Get(db.Plant, db.Serial_Number);
            Assert.NotNull(db);

            db.Serial_Number = "1111111";
            DAL.Services.Drill_BitSvc.Update(db);

            List<DAL.Models.Drill_Bit> dbList = DAL.Services.Drill_BitSvc.GetAll(MOO.Plant.Minntac);
            Assert.NotEmpty(dbList);
            DAL.Services.Drill_BitSvc.Delete(db);
        }


        [Fact]
        public void TestEquip()
        {
            DAL.Models.Equip Eq = new()
            {
                Plant = MOO.Plant.Minntac,
                Equip_Number = "123456",
                Drill_System = DAL.Models.DrillSystem.Epiroc,
                Reference_Id = "123456",
                Active = true
            };
            DAL.Services.EquipSvc.Insert(Eq);
            Eq = DAL.Services.EquipSvc.Get(Eq.Equip_Id);
            Assert.NotNull(Eq);

            Eq = DAL.Services.EquipSvc.Get(Eq.Plant, Eq.Equip_Number);
            Assert.NotNull(Eq);

            Eq = DAL.Services.EquipSvc.GetByReferenceId(Eq.Plant, Eq.Reference_Id);
            Assert.NotNull(Eq);

            Eq.Equip_Number = "1111111";
            DAL.Services.EquipSvc.Update(Eq);

            List<DAL.Models.Equip> eqList = DAL.Services.EquipSvc.GetAll(MOO.Plant.Minntac);
            Assert.NotEmpty(eqList);
            DAL.Services.EquipSvc.Delete(Eq);
        }




        [Fact]
        public void TestOperator()
        {
            DAL.Models.Operator op = new()
            {
                Plant = MOO.Plant.Minntac,
                Employee_Number = "11111",
                First_Name = "John",
                Last_Name = "Doe",
                Active = true
            };
            DAL.Services.OperatorSvc.Insert(op);
            op = DAL.Services.OperatorSvc.Get(op.Operator_Id);
            Assert.NotNull(op);

            op = DAL.Services.OperatorSvc.Get(op.Plant, op.Employee_Number);
            Assert.NotNull(op);

            op.First_Name = "Duh";
            DAL.Services.OperatorSvc.Update(op);

            List<DAL.Models.Operator> opList = DAL.Services.OperatorSvc.GetAll(MOO.Plant.Minntac);
            Assert.NotEmpty(opList);
            DAL.Services.OperatorSvc.Delete(op);
        }




        [Fact]
        public void TestPattern()
        {
            DAL.Models.Pattern p = new()
            {
                Plant = MOO.Plant.Minntac,
                Pattern_Number = "111bbb"
            };
            DAL.Services.PatternSvc.Insert(p);
            p = DAL.Services.PatternSvc.Get(p.Pattern_Id);
            Assert.NotNull(p);

            p = DAL.Services.PatternSvc.Get(p.Plant, p.Pattern_Number);
            Assert.NotNull(p);

            p.Pattern_Number = "123444";
            DAL.Services.PatternSvc.Update(p);

            List<DAL.Models.Pattern> opList = DAL.Services.PatternSvc.GetAll(MOO.Plant.Minntac);
            Assert.NotEmpty(opList);
            DAL.Services.PatternSvc.Delete(p);
        }

        [Fact]
        public void TestDrilled_Hole()
        {
            Random rnd = new();
            //Need to have Equip, Pattern, Operator, and optionally a drill bit before inserting
            DAL.Models.Drill_Bit db = new()
            {
                Plant = MOO.Plant.Minntac,
                Serial_Number = rnd.Next().ToString()
            };
            DAL.Services.Drill_BitSvc.Insert(db);

            DAL.Models.Equip Eq = new()
            {
                Plant = MOO.Plant.Minntac,
                Equip_Number = rnd.Next().ToString(),
                Drill_System = DAL.Models.DrillSystem.Thunderbird,
                Reference_Id = rnd.Next().ToString(),
                Active = true
            };
            DAL.Services.EquipSvc.Insert(Eq);

            DAL.Models.Operator op = new()
            {
                Plant = MOO.Plant.Minntac,
                Employee_Number = rnd.Next().ToString(),
                First_Name = "John",
                Last_Name = "Doe",
                Active = true
            };
            DAL.Services.OperatorSvc.Insert(op);

            DAL.Models.Pattern p = new()
            {
                Plant = MOO.Plant.Minntac,
                Pattern_Number = rnd.Next().ToString()
            };
            DAL.Services.PatternSvc.Insert(p);


            //Parent records exist, now test drilled hole
            DAL.Models.Drilled_Hole dh = new()
            {
                Plant = MOO.Plant.Minntac,
                Equip = Eq,
                Pattern = p,
                Drill_Bit = db,
                Operator = op,
                Material = DAL.Models.Material.Waste_Rock,
                Hole_Number = rnd.Next().ToString(),
                Planned_Depth = 30,
                Drilled_Depth = 32,
                Start_Date = DateTime.Now.AddHours(-1),
                End_Date = DateTime.Now,
                Shift_Date = DateTime.Today,
                Shift = 1,
                Design_Northing = null,
                Actual_Northing = null,
                Design_Easting = null,
                Actual_Easting = null,
                Design_Bottom = null,
                Actual_Bottom = null,
                Collar = null,
                Reference_Key = "abc123",
                Hole_Notes = DAL.Services.Drilled_Hole_NotesSvc.Get(1)
            };
            DAL.Services.Drilled_HoleSvc.Insert(dh);
            dh = DAL.Services.Drilled_HoleSvc.Get(dh.Drilled_Hole_Id);
            Assert.NotNull(dh);
            dh.Planned_Depth = 40;
            dh.Start_Date = dh.Start_Date.AddDays(-10);
            dh.End_Date = dh.End_Date.AddDays(-10);
            DAL.Services.Drilled_HoleSvc.Update(dh);

            dh = DAL.Services.Drilled_HoleSvc.Get(dh.Drilled_Hole_Id);

            List<DAL.Models.Drilled_Hole> dhList = DAL.Services.Drilled_HoleSvc.GetByShift(MOO.Plant.Minntac, DateTime.Today,1);
            Assert.NotEmpty(dhList);


            DAL.Services.Drilled_HoleSvc.Delete(dh);


            DAL.Services.PatternSvc.Delete(p);
            DAL.Services.OperatorSvc.Delete(op);
            DAL.Services.EquipSvc.Delete(Eq);
            DAL.Services.Drill_BitSvc.Delete(db);
            DAL.Services.Drilled_HoleSvc.DeleteShift(MOO.Plant.Minntac, DateTime.Parse("1/1/2010"), 1);

        }
        [Fact]
        public void DrilledHoleNotes()
        {
            var dhn = DAL.Services.Drilled_Hole_NotesSvc.Get(1);
            Assert.NotNull(dhn);

            var dhnList = DAL.Services.Drilled_Hole_NotesSvc.GetAll();
            Assert.NotEmpty(dhnList);
        }

        [Fact]
        public void TestGetPit()
        {
            Assert.Equal(DAL.Models.Pit.MTC_West_Pit, DAL.Services.PatternSvc.GetMTCPit(-10000, 3100));
            Assert.Equal(DAL.Models.Pit.MTC_West_Pit, DAL.Services.PatternSvc.GetMTCPit(-4000, 3100));
            Assert.Equal(DAL.Models.Pit.MTC_West_Pit, DAL.Services.PatternSvc.GetMTCPit(695, 2284));
            Assert.Equal(DAL.Models.Pit.MTC_West_Pit, DAL.Services.PatternSvc.GetMTCPit(3200, 350));
            Assert.Equal(DAL.Models.Pit.MTC_West_Pit, DAL.Services.PatternSvc.GetMTCPit(5700, -600));
            Assert.Equal(DAL.Models.Pit.MTC_West_Pit, DAL.Services.PatternSvc.GetMTCPit(9036, 1000));
            Assert.Equal(DAL.Models.Pit.MTC_West_Pit, DAL.Services.PatternSvc.GetMTCPit(30000, 1000));





            Assert.Equal(DAL.Models.Pit.MTC_East_Pit, DAL.Services.PatternSvc.GetMTCPit(1489, 2769));
            Assert.Equal(DAL.Models.Pit.MTC_East_Pit, DAL.Services.PatternSvc.GetMTCPit(2720, 1510));
            Assert.Equal(DAL.Models.Pit.MTC_East_Pit, DAL.Services.PatternSvc.GetMTCPit(5105, 323));
            Assert.Equal(DAL.Models.Pit.MTC_East_Pit, DAL.Services.PatternSvc.GetMTCPit(5765, 113));
            Assert.Equal(DAL.Models.Pit.MTC_East_Pit, DAL.Services.PatternSvc.GetMTCPit(7000, 775));
            Assert.Equal(DAL.Models.Pit.MTC_East_Pit, DAL.Services.PatternSvc.GetMTCPit(9261, 2110));
            Assert.Equal(DAL.Models.Pit.MTC_East_Pit, DAL.Services.PatternSvc.GetMTCPit(12000, 2600));
            Assert.Equal(DAL.Models.Pit.MTC_East_Pit, DAL.Services.PatternSvc.GetMTCPit(25000, 2600));
        }
    }
}
