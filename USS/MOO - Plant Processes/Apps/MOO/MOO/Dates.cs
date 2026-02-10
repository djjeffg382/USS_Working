using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO
{
    /// <summary>
    /// Class used for manipulating dates
    /// </summary>
    public static class Dates
    {
        /// <summary>
        /// Converts the datetime to an oracle TO_DATE string
        /// </summary>
        /// <param name="dt">Date to be converted to string</param>
        /// <returns></returns>
        public static string OraDate(DateTime dt)
        {
            return "to_date('" + dt.ToString("yyyyMMdd HH:mm:ss") + "','yyyymmdd hh24:mi:ss')";
        }

        /// <summary>
        /// Converts the datetime to an oracle TO_DATE string
        /// </summary>
        /// <param name="dt">Date to be converted to string</param>
        /// <param name="TruncateDate">Boolean indicating if the date should be truncated to midnight</param>
        /// <returns></returns>
        public static string OraDate(DateTime dt, bool TruncateDate)
        {
            if (TruncateDate)
                return "to_date('" + dt.ToString("yyyyMMdd") + "','yyyymmdd')";
            else
                return "to_date('" + dt.ToString("yyyyMMdd HH:mm:ss") + "','yyyymmdd hh24:mi:ss')";
        }


        /// <summary>
        /// Returns the last day of the month for the given date
        /// </summary>
        /// <param name="TheDate">The date</param>
        /// <returns></returns>
        public static DateTime LastDayOfMonth(DateTime TheDate)
        {
            return new DateTime(TheDate.Year, TheDate.Month, 1).AddMonths(1).AddDays(-1);
        }

        /// <summary>
        /// Returns the First day of the month for the given date
        /// </summary>
        /// <param name="TheDate"></param>
        /// <returns></returns>
        public static DateTime FirstDayOfMonth(DateTime TheDate)
        {
            return new DateTime(TheDate.Year, TheDate.Month, 1);
        }

        /// <summary>
        /// Returns the first day of the year for the given date
        /// </summary>
        /// <param name="TheDate"></param>
        /// <returns></returns>
        public static DateTime FirstDayOfYear(DateTime TheDate)
        {
            return new DateTime(TheDate.Year, 1, 1);
        }

        /// <summary>
        /// Gets the Begin/End dates of a given quarter
        /// </summary>
        /// <param name="TheDate"></param>
        /// <returns></returns>
        public static (DateTime, DateTime) GetQuarterDates(DateTime TheDate)
        {
            int quarter = (TheDate.Month - 1) / 3 + 1;
            DateTime firstDay = new(TheDate.Year, (quarter - 1) * 3 + 1, 1);
            DateTime lastDay = firstDay.AddMonths(3).AddDays(-1);
            return (firstDay, lastDay);
        }

        /// <summary>
        /// Returns the last day of the year for the given date
        /// </summary>
        /// <param name="TheDate"></param>
        /// <returns></returns>
        public static DateTime LastDayOfYear(DateTime TheDate)
        {
            return new DateTime(TheDate.Year + 1, 1, 1).AddDays(-1);
        }

        /// <summary>
        /// Converts a YYYYMMDD string to a datetime
        /// </summary>
        /// <param name="YMDDate">YYYYMMDD string</param>
        /// <returns></returns>
        public static DateTime YMDToDate(string YMDDate)
        {
            string InvalidFormatMessage = "Invalid date format, format must be YYYYMMDD";
            //Validate the data first
            byte month, day;
            if (YMDDate.Length != 8)
                throw new FormatException(InvalidFormatMessage);
            if (!int.TryParse(YMDDate, out _))
                throw new FormatException(InvalidFormatMessage);

            //Year must be greater than 1900
            if (int.Parse(YMDDate.Substring(0,4)) < 1900)
                throw new FormatException(InvalidFormatMessage);
            //month must not be greater than 12
            month = byte.Parse(YMDDate.Substring(4, 2));
            if (month > 12 || month == 0)
                throw new FormatException(InvalidFormatMessage);

            //Day must not be greater than 31
            day = byte.Parse(YMDDate.Substring(6, 2));
            if (day > 31 || day == 0)
                throw new FormatException(InvalidFormatMessage);

            return DateTime.ParseExact(YMDDate,
                        "yyyyMMdd", new System.Globalization.CultureInfo("en-us"));
        }

        /// <summary>
        /// Converts Unix time (seconds since 1970) to DateTime
        /// </summary>
        /// <param name="UnixTime"></param>
        /// <returns></returns>
        public static DateTime UnixTimeToDatetime(long UnixTime)
        {
            DateTime UnixStart = DateTime.Parse("1/1/1970");
            return UnixStart.AddSeconds(UnixTime);
        }

        /// <summary>
        /// Converts given datetime to Unix time (seconds since 1/1/1970)
        /// </summary>
        /// <param name="TheDate"></param>
        /// <returns></returns>
        public static long DateTimeToUnixTime(DateTime TheDate)
        {
            DateTime UnixStart = DateTime.Parse("1/1/1970");
            return (long)TheDate.Subtract(UnixStart).TotalSeconds;
        }


        /// <summary>
        /// converts the local time which is assumed Central to Central Standard Time.  Used for Wenco
        /// </summary>
        /// <param name="Time"></param>
        /// <returns></returns>
        public static DateTime LocalToStandardTime(DateTime Time)
        {
            TimeZoneInfo centralTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            bool isDaylight = centralTimeZone.IsDaylightSavingTime(Time);
            if (isDaylight)
                return Time.AddHours(-1);
            else
                return Time;
        }

        /// <summary>
        /// Converts Wenco Standard time to local central time
        /// </summary>
        /// <param name="Time"></param>
        /// <returns></returns>
        public static DateTime StandardToLocalTime(DateTime Time)
        {
            //Standard time should be -6 so add 6 hours to get to UTC
            DateTime utcTime = Time.AddHours(6);
            utcTime = DateTime.SpecifyKind(utcTime, DateTimeKind.Utc);
            TimeZoneInfo centralTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            DateTime centralTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, centralTimeZone);
            return centralTime;
        }

    }
}
