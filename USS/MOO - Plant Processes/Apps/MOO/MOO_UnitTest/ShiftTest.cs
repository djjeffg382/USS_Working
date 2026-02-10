using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Oracle.ManagedDataAccess.Client;
using System.Security.Cryptography;
using System.ComponentModel.Design;
using System.Data;
using MOO.Shifts;

namespace MOO_UnitTest
{
    
    public class ShiftTest
    {

        private int GetCrewFromDMARTShiftPkg(MOO.Plant Plant, MOO.Area Area, DateTime ShiftDate, short ShiftNumber)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            OracleCommand cmd = new("Shifts.Crew")
            {
                Connection = conn,
                CommandType = System.Data.CommandType.StoredProcedure,
                BindByName = true
            };
            cmd.Parameters.Add(new OracleParameter("in_mine", OracleDbType.Decimal));
            if (Plant == MOO.Plant.Minntac)
                cmd.Parameters["in_mine"].Value = 1;
            else if (Plant == MOO.Plant.Keetac)
                cmd.Parameters["in_mine"].Value = 2;
            else
                throw new Exception("Invalid Plant for GetDBCrew");

            cmd.Parameters.Add(new OracleParameter("in_area", OracleDbType.Decimal));
            switch (Area)
            {
                case MOO.Area.Pit:
                    cmd.Parameters["in_area"].Value = 1200;
                    break;
                case MOO.Area.Crusher:
                    cmd.Parameters["in_area"].Value = 1201;
                    break;
                case MOO.Area.Concentrator:
                    cmd.Parameters["in_area"].Value = 1202;
                    break;
                case MOO.Area.Agglomerator:
                    cmd.Parameters["in_area"].Value = 1203;
                    break;
                default: throw new Exception($"GetDBCrew not implemented for area {Area}");
            }


            cmd.Parameters.Add(new OracleParameter("in_shift_date", OracleDbType.Date));
            cmd.Parameters["in_shift_date"].Value = ShiftDate;

            cmd.Parameters.Add(new OracleParameter("in_shift", OracleDbType.Decimal));
            cmd.Parameters["in_shift"].Value = ShiftNumber;

            cmd.Parameters.Add("return_value", OracleDbType.Int32).Direction = ParameterDirection.ReturnValue;

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            var result = cmd.Parameters["return_value"].Value;
            return ((Oracle.ManagedDataAccess.Types.OracleDecimal)result).ToInt32();

        }


        [Fact]
        public void CompareCrewScheduleToDMART()
        {
            //this can be used to verify that the crew schedule matches the code in DMART
            DateTime LoopDate = DateTime.Today.AddDays(-365);
            int dbVal, libVal;
            while(LoopDate < DateTime.Today)
            {
                for (byte i = 1; i <= 2; i++)
                {
                    dbVal = GetCrewFromDMARTShiftPkg(MOO.Plant.Minntac, MOO.Area.Concentrator, LoopDate, i);
                    libVal = Shift.Crew(MOO.Plant.Minntac, MOO.Area.Crusher, LoopDate, i);
                    Assert.Equal(dbVal, libVal);
                }
                LoopDate = LoopDate.AddDays(1);
            }
            



        }

        [Fact]
        public void TestShiftTimes8()
        {
            Assert.Equal(1, MOO.Shifts.Shift8.ShiftHour(DateTime.Parse("1/1/2020 22:30"), MOO.Plant.Minntac));
            Assert.Equal(1, MOO.Shifts.Shift8.ShiftHour(DateTime.Parse("1/1/2020 23:29:59"), MOO.Plant.Minntac));
            Assert.Equal(2, MOO.Shifts.Shift8.ShiftHour(DateTime.Parse("1/1/2020 23:30"), MOO.Plant.Minntac));
            Assert.Equal(24, MOO.Shifts.Shift8.ShiftHour(DateTime.Parse("1/1/2020 22:29:59"), MOO.Plant.Minntac));


            Assert.Equal(1, MOO.Shifts.Shift8.ShiftHour(DateTime.Parse("1/1/2020 21:00"), MOO.Plant.Keetac));
            Assert.Equal(1, MOO.Shifts.Shift8.ShiftHour(DateTime.Parse("1/1/2020 21:59:59"), MOO.Plant.Keetac));
            Assert.Equal(2, MOO.Shifts.Shift8.ShiftHour(DateTime.Parse("1/1/2020 22:00"), MOO.Plant.Keetac));
            Assert.Equal(24, MOO.Shifts.Shift8.ShiftHour(DateTime.Parse("1/1/2020 20:59:59"), MOO.Plant.Keetac));


            Assert.Equal(1, MOO.Shifts.Shift8.HalfShift(DateTime.Parse("1/1/2020 22:30"), MOO.Plant.Minntac));
            Assert.Equal(1, MOO.Shifts.Shift8.HalfShift(DateTime.Parse("1/1/2020 02:29:59"), MOO.Plant.Minntac));
            Assert.Equal(2, MOO.Shifts.Shift8.HalfShift(DateTime.Parse("1/1/2020 02:30"), MOO.Plant.Minntac));

            Assert.Equal(1, MOO.Shifts.Shift8.HalfShift(DateTime.Parse("1/1/2020 21:00"), MOO.Plant.Keetac));
            Assert.Equal(1, MOO.Shifts.Shift8.HalfShift(DateTime.Parse("1/1/2020 00:59"), MOO.Plant.Keetac));
            Assert.Equal(2, MOO.Shifts.Shift8.HalfShift(DateTime.Parse("1/1/2020 01:00"), MOO.Plant.Keetac));

            Assert.Equal(1, MOO.Shifts.Shift8.ShiftNumber(DateTime.Parse("1/1/2020 22:30"), MOO.Plant.Minntac));
            Assert.Equal(1, MOO.Shifts.Shift8.ShiftNumber(DateTime.Parse("1/1/2020 06:29"), MOO.Plant.Minntac));
            Assert.Equal(2, MOO.Shifts.Shift8.ShiftNumber(DateTime.Parse("1/1/2020 06:30"), MOO.Plant.Minntac));
            Assert.Equal(3, MOO.Shifts.Shift8.ShiftNumber(DateTime.Parse("1/1/2020 14:30"), MOO.Plant.Minntac));

            Assert.Equal(1, MOO.Shifts.Shift8.ShiftNumber(DateTime.Parse("1/1/2020 21:00"), MOO.Plant.Keetac));
            Assert.Equal(1, MOO.Shifts.Shift8.ShiftNumber(DateTime.Parse("1/1/2020 04:59"), MOO.Plant.Keetac));
            Assert.Equal(2, MOO.Shifts.Shift8.ShiftNumber(DateTime.Parse("1/1/2020 05:00"), MOO.Plant.Keetac));


            Assert.Equal(DateTime.Parse("1/1/2020"), MOO.Shifts.Shift8.ShiftDate(DateTime.Parse("1/1/2020 22:29"), MOO.Plant.Minntac));
            Assert.Equal(DateTime.Parse("1/2/2020"), MOO.Shifts.Shift8.ShiftDate(DateTime.Parse("1/1/2020 22:30"), MOO.Plant.Minntac));

            Assert.Equal(DateTime.Parse("1/1/2020"), MOO.Shifts.Shift8.ShiftDate(DateTime.Parse("1/1/2020 20:59"), MOO.Plant.Keetac));
            Assert.Equal(DateTime.Parse("1/2/2020"), MOO.Shifts.Shift8.ShiftDate(DateTime.Parse("1/1/2020 21:00"), MOO.Plant.Keetac));
        }


        [Fact]
        public void TestShiftTimes()
        {

            //MOO.Shifts.ShiftVal lastShift = MOO.Shifts.Shift.GetShiftVal(MOO.Plant.Minntac, MOO.Area.Drilling, DateTime.Now.AddMinutes(-480));


            var s = MOO.Shifts.Shift.GetShiftVal(MOO.Plant.Minntac, MOO.Area.Pit, DateTime.Now);
            Assert.NotNull(s);

            s = MOO.Shifts.Shift.GetShiftVal(MOO.Plant.Minntac, MOO.Area.Pit, DateTime.Today, 1);   //Gets Shift 1 for today
            var s_minus_one = s.PreviousShift();
            var s_plus_one = s.NextShift();

            Assert.NotNull(s_minus_one);
            Assert.NotNull(s_plus_one);
        }

        [Fact]
        public void ShiftSchedDays()
        {
            
            int[] shifts = new int[90];
            int loopNbr = 0;
            DateTime loopDate = DateTime.Parse("1/1/2022");
            while (loopDate < DateTime.Parse("2/1/2022"))
            {
                shifts[loopNbr] = MOO.Shifts.Shift.CrewShift(MOO.Plant.Minntac, MOO.Area.Pit, loopDate, 1);
                loopDate = loopDate.AddDays(1);
                loopNbr++;
            }
            //var a = 0;
        }


        [Fact]
        public void RandomTests()
        {
            DateTime dt = DateTime.Parse("1/7/2023");
            var sv = MOO.Shifts.Shift.GetShiftVal(MOO.Plant.Minntac,MOO.Area.Crusher,dt, 1);
            StringBuilder output = new();
            while(sv.ShiftDate < DateTime.Today)
            {
                output.AppendLine($"{sv.ShiftStartTime} - {sv.Crew}");
                sv = sv.NextShift();

            }
            //int i = 0;
        }

        [Fact]
        public void TestCrew()
        {
            var crew = MOO.Shifts.Shift.Crew(MOO.Plant.Minntac, MOO.Area.Pit, DateTime.Parse("1/1/2025"), 1);
            Assert.Equal(1, crew);
        }
    }

}
