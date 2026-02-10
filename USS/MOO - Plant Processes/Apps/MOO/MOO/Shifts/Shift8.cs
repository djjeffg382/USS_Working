using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.Shifts
{
    /// <summary>
    /// class for working with the old 8 hour shifts.  For the new 12 hour shifts starting in 2012 use the shift class
    /// </summary>
    public class Shift8 
    {
        private const short MINNTAC_OFFSET_MINUTES = 90;
        private const short KEETAC_OFFSET_MINUTES = 180;


        /// <summary>
        /// Gets the Shift Values for the specified time
        /// </summary>
        /// <param name="Plant">The plant (minntac or keetac)</param>
        /// <param name="ShiftTime">The actual time for obtaining the shift values</param>
        /// <param name="Area">The area at the plant</param>
        /// <returns></returns>
        public static ShiftVal GetShiftVal(MOO.Plant Plant, MOO.Area Area, DateTime ShiftTime)
        {
            ShiftVal retVal = new()
            {
                Plant = Plant,
                Area = Area,
                ShiftType = ShiftTypes.Shift_8_Hour,
                ShiftDate = ShiftDate(ShiftTime, Plant),
                ShiftNumber = ShiftNumber(ShiftTime, Plant)
            };


            DateTime[] startEnd = ShiftStartEndTime(ShiftTime, retVal.ShiftNumber,Plant);
            retVal.ShiftStartTime = startEnd[0];
            retVal.EndShiftTime = startEnd[1];

            retVal.ShiftLength = 8;
            if (Plant == MOO.Plant.Minntac)
                retVal.ShiftOffset = MINNTAC_OFFSET_MINUTES * -1;
            else
                retVal.ShiftOffset = KEETAC_OFFSET_MINUTES * -1;


            retVal.Crew = CrewNumber(ShiftTime,Plant, Area);
            try
            {
                retVal.CrewName = retVal.Crew.ToString();
            }
            catch { }

            return retVal;
        }


        /// <summary>
        /// Gets the Shift Values for the specified time
        /// </summary>
        /// <param name="Plant">The plant (minntac or keetac)</param>
        /// <param name="ShiftNbr">The shift Number</param>
        /// <param name="ShiftDate">The shift date</param>
        /// <param name="Area">The area at the plant</param>
        /// <returns></returns>
        public static ShiftVal GetShiftVal(MOO.Plant Plant, MOO.Area Area, DateTime ShiftDate, byte ShiftNbr)
        {
            DateTime[] startEnd = ShiftStartEndTime(ShiftDate, ShiftNbr,Plant);
            ShiftVal retVal = GetShiftVal(Plant, Area, startEnd[0]);
            return retVal;
        }


        /// <summary>
        /// Gets the datetime with the plant's offset factored in
        /// </summary>
        /// <param name="plant"></param>
        /// <param name="theDate">The Date to obtain offset</param>
        /// <returns></returns>
        /// <remarks>Added with version 1.0.0.0 alt7747</remarks>
        private static DateTime GetOffsetDate(DateTime theDate, MOO.Plant plant)
        {
            if (plant == MOO.Plant.Minntac)
                return theDate.AddMinutes(MINNTAC_OFFSET_MINUTES);
            else
                return theDate.AddMinutes(KEETAC_OFFSET_MINUTES);
        }


        /// <summary>
        /// Gets hour number of the plant since the start of day
        /// </summary>
        /// <param name="theDate">Datetime</param>
        /// <param name="plant">Plant Enumerator</param>
        /// <returns>The hour number since the plant's start of day</returns>
        public static byte ShiftHour(DateTime theDate, MOO.Plant plant)
        {
            DateTime dtOffset = GetOffsetDate(theDate, plant);
            //Must add 60 minutes (1/24) to the offset to get hour of the day (ie. 1st, 5th) rather than time hour (ie. 00, 04)
            dtOffset = dtOffset.AddHours(1);
            if (dtOffset.Hour == 0)
                return 24;
            else return Convert.ToByte(dtOffset.Hour);

        }



        /// <summary>
        ///  Gets the Half Shift number of the plant since the start of day
        /// </summary>
        /// <param name="theDate">Datetime</param>
        /// <param name="plant">Plant Enumerator</param>
        /// <returns></returns>
        public static byte HalfShift(DateTime theDate, MOO.Plant plant)
        {
            double nHour = Convert.ToDouble(ShiftHour(theDate, plant));
            return Convert.ToByte(Math.Floor(((nHour - 1) / 4) + 1));

        }


        /// <summary>
        /// Gets the shift number for the the given datetime and plant
        /// </summary>
        /// <param name="theDate">The Datetime</param>
        /// <param name="plant">The plant enumerator</param>
        /// <returns></returns>
        public static byte ShiftNumber(DateTime theDate, MOO.Plant plant)
        {
            DateTime dtOffset = GetOffsetDate(theDate, plant);
            return Convert.ToByte(Math.Floor(dtOffset.TimeOfDay.TotalHours / 8 + 1));
        }

        /// <summary>
        /// Gets the shift for a given date and time (returns the next day if it
        /// is past 9 PM for Keetac or 10:30 PM for Minntac)
        /// </summary>
        /// <param name="theDate"></param>
        /// <param name="plant"></param>
        /// <returns></returns>
        public static DateTime ShiftDate(DateTime theDate, MOO.Plant plant)
        {
            DateTime shiftDate;
            if (plant == MOO.Plant.Keetac)
                shiftDate = theDate.AddMinutes(KEETAC_OFFSET_MINUTES).Date;
            else
                shiftDate = theDate.AddMinutes(MINNTAC_OFFSET_MINUTES).Date;
            return shiftDate;
        }


        /// <summary>
        /// Gets the crew number for the specified parameters
        /// </summary>
        /// <param name="theDate">The datetime</param>
        /// <param name="plant">The plant Area</param>
        /// <param name="area">The plant</param>
        /// <returns>The crew number</returns>
        public static byte CrewNumber(DateTime theDate, MOO.Plant plant, MOO.Area area)
        {
            byte nCrewCount = 4;
            if (area == Area.Drilling)
                return 3;
            //get the shift number
            byte nShift = ShiftNumber(theDate, plant);
            //get the shift date
            byte nCrew = 0;
            for (byte nCount = 1; nCount <= nCrewCount; nCount++)
            {
                nCrew = Convert.ToByte(CrewShift(theDate, nCount, plant, area));
                if (nCrew == nShift)
                    break;
                if (nCrew == nCrewCount) //no crew found for this date
                    nCrew = 0;
            }
            return nCrew;
        }


        /// <summary>
        /// Gets the shift number for a specific crew and date
        /// </summary>
        /// <param name="theDate">The date</param>
        /// <param name="crewNumber">The crew number</param>
        /// <param name="plant">The plant area</param>
        /// <param name="area">The plant</param>
        /// <returns></returns>
        public static int CrewShift(DateTime theDate, byte crewNumber, MOO.Plant plant, MOO.Area area)
        {
            byte[] crewShift;
            if (plant == Plant.Minntac)
            {
                if (area == Area.Drilling)
                    crewShift = new byte[] { 1, 1, 1, 1, 1, 0, 0, 3, 3, 3, 3, 3, 0, 0, 2, 2, 2, 2, 2, 0, 0 };
                else
                    crewShift = new byte[] { 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 3, 3, 3, 3, 3, 3, 3, 0, 2, 2, 2, 2, 2, 2, 2, 0, 0 };
            }
            else if (plant == Plant.Keetac)
            {
                if (area == Area.Pit)
                    crewShift = new byte[] { 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 3, 3, 3, 3, 3, 3, 3, 0, 0, 2, 2, 2, 2, 2, 2, 2, 0 };
                else if (area == Area.Drilling)
                    crewShift = new byte[] { 1, 0, 0, 0, 0, 3, 3, 3, 3, 3, 3, 3, 0, 0, 2, 2, 2, 2, 2, 2, 2 };
                else //Plant
                    crewShift = new byte[] { 1, 1, 1, 1, 1, 1, 1, 0, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 3, 3, 3, 3, 3, 3, 3, 0, 0 };

            }
            else
                throw new Exception("Invalid Plant");

            DateTime? dtStartDate = CrewStartDate(crewNumber, plant, area);
            if (!dtStartDate.HasValue)
                return -1;

            //figure out the start date of each area
            int dateDiff = (theDate - dtStartDate.Value).Days;
            int shiftDay;
            if (dateDiff > 0)
                shiftDay = (dateDiff % crewShift.Length) + 1;
            else
            {
                shiftDay = (dateDiff % -crewShift.Length) + (crewShift.Length + 1);
                if (shiftDay == crewShift.Length + 1)
                    shiftDay = 1;
            }
            return crewShift[shiftDay - 1];


        }


        /// <summary>
        /// gets the first midnight shift of a specified crew at a specified area
        /// </summary>
        /// <param name="crewNumber">The crew number</param>
        /// <param name="plant">The area</param>
        /// <param name="area">The plant</param>
        /// <returns></returns>
        private static DateTime? CrewStartDate(byte crewNumber, MOO.Plant plant, MOO.Area area)
        {
            DateTime?[] crewDate = new DateTime?[4];

            if (plant == Plant.Minntac)
            {
                //get the first shift (midnight shift) in 2008
                switch (area)
                {
                    case Area.Pit:
                        crewDate[0] = DateTime.Parse("01/04/2008");
                        crewDate[1] = DateTime.Parse("01/25/2008");
                        crewDate[2] = DateTime.Parse("01/18/2008");
                        crewDate[3] = DateTime.Parse("01/11/2008");
                        break;
                    case Area.Crusher:
                        crewDate[0] = DateTime.Parse("01/25/2008");
                        crewDate[1] = DateTime.Parse("01/18/2008");
                        crewDate[2] = DateTime.Parse("01/11/2008");
                        crewDate[3] = DateTime.Parse("01/04/2008");
                        break;
                    case Area.Concentrator:
                        crewDate[0] = DateTime.Parse("01/25/2008");
                        crewDate[1] = DateTime.Parse("01/18/2008");
                        crewDate[2] = DateTime.Parse("01/11/2008");
                        crewDate[3] = DateTime.Parse("01/04/2008");
                        break;
                    case Area.Agglomerator:
                        crewDate[0] = DateTime.Parse("01/18/2008");
                        crewDate[1] = DateTime.Parse("01/11/2008");
                        crewDate[2] = DateTime.Parse("01/04/2008");
                        crewDate[3] = DateTime.Parse("01/25/2008");
                        break;
                    case Area.Drilling:
                        crewDate[0] = DateTime.Parse("07/28/2008");
                        crewDate[1] = DateTime.Parse("07/21/2008");
                        crewDate[2] = DateTime.Parse("07/14/2008");
                        crewDate[3] = null;  //Drilling only has 3 crews
                        break;
                }
            }
            else
            {
                //Keetac
                switch (area)
                {
                    case Area.Pit:
                        crewDate[0] = DateTime.Parse("01/18/2008");
                        crewDate[1] = DateTime.Parse("01/25/2008");
                        crewDate[2] = DateTime.Parse("01/04/2008");
                        crewDate[3] = DateTime.Parse("01/11/2008");
                        break;
                    case Area.Drilling:
                        crewDate[0] = DateTime.Parse("07/24/2008");
                        crewDate[1] = DateTime.Parse("07/17/2008");
                        crewDate[2] = DateTime.Parse("07/10/2008");
                        crewDate[3] = null;
                        break;
                    default:
                        crewDate[0] = DateTime.Parse("01/17/2008");
                        crewDate[1] = DateTime.Parse("01/10/2008");
                        crewDate[2] = DateTime.Parse("01/03/2008");
                        crewDate[3] = DateTime.Parse("01/24/2008");
                        break;
                }
            }
            return crewDate[crewNumber - 1];
        }
        /// <summary>
        /// Returns the start time and end time of a given shift
        /// </summary>
        /// <param name="theDate">The Shift Date</param>
        /// <param name="shift">The Shift Number</param>
        /// <param name="plant">The plant</param>
        /// <returns></returns>
        ///  <remarks> Shift 1 will return the previous day for the start time
        /// example shift 1 at Keetac for Jan 2 will return 9:00PM on Jan 1</remarks>
        public static DateTime[] ShiftStartEndTime(DateTime theDate, byte shift, MOO.Plant plant)
        {
            int addMinutes;
            addMinutes = (shift - 1) * 8 * 60;
            if (plant == Plant.Keetac)
                addMinutes -= KEETAC_OFFSET_MINUTES;
            else
                addMinutes -= MINNTAC_OFFSET_MINUTES;

            DateTime dDay;
            DateTime[] retVal = new DateTime[2];
            dDay = theDate.Date;
            retVal[0] = dDay.AddMinutes(addMinutes);
            retVal[1] = retVal[0].AddHours(8);
            return retVal;


        }


        /// <summary>
        /// Returns the start time and end time of a given half shift
        /// </summary>
        /// <param name="theDate">The Shift Date</param>
        /// <param name="halfNum">The half Shift Number (1-6)</param>
        /// <param name="plant">The plant</param>
        /// <returns></returns>
        public static DateTime[] HalfShiftStartEndTime(DateTime theDate, byte halfNum, MOO.Plant plant)
        {
            int addMinutes;
            addMinutes = (halfNum - 1) * 4 * 60;
            if (plant == Plant.Keetac)
                addMinutes -= KEETAC_OFFSET_MINUTES;
            else
                addMinutes -= MINNTAC_OFFSET_MINUTES;
            DateTime dDay;
            DateTime[] retVal = new DateTime[2];
            dDay = theDate.Date;
            retVal[0] = dDay.AddMinutes(addMinutes);
            retVal[1] = retVal[0].AddHours(4);
            return retVal;
        }

        /// <summary>
        /// Returns how many crews there are for a specific plant and area
        /// </summary>
        /// <param name="area">The area</param>
        /// <param name="plant">The plant</param>
        /// <returns></returns>
        /// <remarks>Most areas have 4 crews.  Drilling has 3 crews</remarks>
#pragma warning disable IDE0060 // Remove unused parameter
        public static byte CrewCount(MOO.Plant plant, MOO.Area area)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            if (area == Area.Drilling)
                return 3;
            else
                return 4;
        }

        /// <summary>
        /// Get the start and the end datetime of the shift
        /// </summary>
        /// <param name="theDate">The date time of the shift</param>
        /// <param name="plant">The Plant</param>
        /// <returns>An array with index 0 being the start time and index 1 being the end time</returns>
        public static DateTime[] ShiftStartEndTime(DateTime theDate, MOO.Plant plant)
        {
            return ShiftStartEndTime(ShiftDate(theDate, plant), ShiftNumber(theDate, plant), plant);
        }

    }

}
