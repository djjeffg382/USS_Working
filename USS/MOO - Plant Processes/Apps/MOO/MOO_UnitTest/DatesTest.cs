using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MOO_UnitTest
{
    public class DatesTest
    {
        [Fact]
        public void TestOraDates()
        {
            DateTime dt = DateTime.Parse("1/1/2000 01:23:45");
            string dtString;
            dtString = MOO.Dates.OraDate(dt, true);
            Assert.Equal("to_date('20000101','yyyymmdd')", dtString);
            dtString = MOO.Dates.OraDate(dt);
            Assert.Equal("to_date('20000101 01:23:45','yyyymmdd hh24:mi:ss')", dtString);
            dtString = MOO.Dates.OraDate(dt, false);
            Assert.Equal("to_date('20000101 01:23:45','yyyymmdd hh24:mi:ss')", dtString);



        }
        [Fact]
        public void TestYMDToDate()
        {
            Assert.Equal(DateTime.Parse("9/20/2021"), MOO.Dates.YMDToDate("20210920"));

            //test some invalid dates, ensure we get an exception

            //Invalid year
            Assert.Throws<FormatException>(() => MOO.Dates.YMDToDate("18000101"));
            //invalid month
            Assert.Throws<FormatException>(() => MOO.Dates.YMDToDate("20201301"));
            Assert.Throws<FormatException>(() => MOO.Dates.YMDToDate("20200001"));

            //invalid day
            Assert.Throws<FormatException>(() => MOO.Dates.YMDToDate("20201232"));
            Assert.Throws<FormatException>(() => MOO.Dates.YMDToDate("20201200"));
        }

        [Fact]
        public void TestFirstLastDayFunctions()
        {
            DateTime dt = DateTime.Parse("3/5/2000");
            Assert.Equal(DateTime.Parse("1/1/2000"), MOO.Dates.FirstDayOfYear(dt));
            Assert.Equal(DateTime.Parse("3/1/2000"), MOO.Dates.FirstDayOfMonth(dt));


            Assert.Equal(DateTime.Parse("3/31/2000"), MOO.Dates.LastDayOfMonth(dt));
            Assert.Equal(DateTime.Parse("12/31/2000"), MOO.Dates.LastDayOfYear(dt));

            var quarter = MOO.Dates.GetQuarterDates(DateTime.Parse("6/15/2020"));
            Assert.Equal(DateTime.Parse("4/1/2020"), quarter.Item1);
            Assert.Equal(DateTime.Parse("6/30/2020"), quarter.Item2);
        }


        [Fact]
        public void TestUnixTime()
        {
            /*One day is 86,400 seconds
            * 1/2/1970 should be 86400 in unix time
            */
            Assert.Equal(86400, MOO.Dates.DateTimeToUnixTime(DateTime.Parse("1/2/1970")));

            Assert.Equal(DateTime.Parse("1/2/1970"), MOO.Dates.UnixTimeToDatetime(86400));

        }

        [Fact]
        public void LocalStandardTest()
        {
            DateTime testDate = DateTime.Parse("9/1/2025");
            //this time is during Daylight time so time should be one hour off

            Assert.Equal(testDate.AddHours(-1), MOO.Dates.LocalToStandardTime(testDate));
            Assert.Equal(testDate.AddHours(1), MOO.Dates.StandardToLocalTime(testDate));


            testDate = DateTime.Parse("12/1/2025");
            //this time is during standard time so times should equal

            Assert.Equal(testDate, MOO.Dates.LocalToStandardTime(testDate));
            Assert.Equal(testDate, MOO.Dates.StandardToLocalTime(testDate));

        }

    }
}
