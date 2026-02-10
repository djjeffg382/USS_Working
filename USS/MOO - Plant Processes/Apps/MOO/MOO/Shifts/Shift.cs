using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.Shifts
{
    /// <summary>
    /// This class was made to handle shift schedules starting in 2012 when Minntac and Keetac had completed the changeover to 
    /// 12 hour shifts.  The shift numbers are assumed will be 1-based.  First shift is 1, second shift is 2 and so on.  In some areas, they do 
    /// not have a shift 1 (they have shift 2 and 3).  You must still use 1 and 2 as the shift numbers.  Use ShiftName function to convert
    /// it to 2 and 3 though.
    /// If there is a change to the shift at any level look at the following procedures to change
    /// - ShiftLength
    /// - ShiftOffset
    /// - Crew
    /// - ShiftName
    /// - CrewName
    /// - CrewCount
    /// </summary>
    public class Shift
    {

        /// <summary>
        /// this is the date minntac crusher switched from 8's to 12's
        /// </summary>
        private static readonly DateTime MinntacCrusher12ChangeDate = DateTime.Parse("17-APR-2022");



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
                ShiftType = ShiftTypes.Shift_12_Hour,
                ShiftDate = ShiftDay(Plant, Area, ShiftTime),
                ShiftNumber = ShiftNumber(Plant, Area, ShiftTime)
            };

            DateTime[] startEnd = ShiftStartEndTime(Plant, Area, retVal.ShiftDate, retVal.ShiftNumber);
            retVal.ShiftStartTime = startEnd[0];
            retVal.EndShiftTime = startEnd[1];

            retVal.ShiftLength = ShiftLength(Plant, Area, ShiftTime);
            retVal.ShiftOffset = ShiftOffset(Plant, Area, ShiftTime);
            retVal.Crew = Crew(Plant, Area, ShiftTime);
            try
            {
                string[] cNames = CrewNames(Plant, Area);
                retVal.CrewName = cNames[retVal.Crew - 1];
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
            DateTime[] startEnd = ShiftStartEndTime(Plant, Area, ShiftDate, ShiftNbr);
            ShiftVal retVal = GetShiftVal(Plant, Area, startEnd[0]);
            return retVal;
        }


        /// <summary>
        /// Returns the length of the shift in hours for the given plant, area, and date
        /// </summary>
        /// <param name="plant">The plant</param>
        /// <param name="area">The area of the plant</param>
        /// <param name="theDate">The date of which to find the shift length</param>
        /// <returns></returns>
        /// <remarks>TheDate is a parameter in this so that if schedules change, the shiftlength can return a different 
        /// value based on the date</remarks>
        public static byte ShiftLength(MOO.Plant plant, MOO.Area area, DateTime theDate)
        {
            if (plant == Plant.Keetac)
            {
                if (area == Area.Drilling)
                {
                    if (theDate >= DateTime.Parse("2/11/2019"))
                        return 12;
                    else
                        return 8;
                }
                else
                    return 12;
            }
            else
            {
                //Minntac
                if (area == Area.Crusher)
                {
                    if (theDate < MinntacCrusher12ChangeDate)
                        return 8;
                    else
                        return 12;

                }
                else
                    return 12;
            }
        }
        /// <summary>
        /// Gets the offset minutes of the first shift of the day
        /// </summary>
        /// <param name="plant">The plant</param>
        /// <param name="area">The area of the plant</param>
        /// <param name="theDate">The date of which to find the shift length</param>
        public static short ShiftOffset(MOO.Plant plant, MOO.Area area, DateTime theDate)
        {
            if (plant == Plant.Keetac)
            {
                if (area == Area.Drilling)
                {
                    if (theDate >= DateTime.Parse("2/11/2019"))
                        return -360;
                    else
                        return -60;
                }
                else if (area == Area.Pit || area == Area.Crusher)
                    return -300;
                else
                    return -360;
            }
            else
            {
                switch (area)
                {
                    case Area.Drilling:
                    case Area.Pit:
                        return -360;

                    case Area.Concentrator:
                    case Area.Agglomerator:
                        return -300;

                    case Area.Crusher:
                        {
                            if (theDate < MinntacCrusher12ChangeDate)  //Minntac crusher switched to 12's on this date
                                return -90;
                            else
                                return -360; //as of 2023 technically crusher shifts start at 18:30.  We will use 18:00 though to align with Mine
                        }

                    default:
                        return 0;

                }

            }
        }

        /// <summary>
        /// Gets the shift date for the specified time
        /// </summary>
        /// <param name="plant">The plant</param>
        /// <param name="area">The area of the plant</param>
        /// <param name="theDate">The date of which to find the shift length</param>
        public static DateTime ShiftDay(MOO.Plant plant, MOO.Area area, DateTime theDate)
        {
            short offset = ShiftOffset(plant, area, theDate);
            DateTime retVal = theDate.AddMinutes(-offset);
            return retVal.Date;
        }

        /// <summary>
        /// Returns the current shift hour of the day (example if 6:00 am is start of first shift, then 6-7 am is hour 1)
        /// </summary>
        /// <param name="plant">The plant</param>
        /// <param name="area">The area of the plant</param>
        /// <param name="theDate">The date of which to find the shift hour</param>
        public static byte ShiftHour(MOO.Plant plant, MOO.Area area, DateTime theDate)
        {
            short offset = ShiftOffset(plant, area, theDate);
            return Convert.ToByte(theDate.AddMinutes(-offset).Hour + 1);
        }

        /// <summary>
        /// Gives the Shift number for the given time
        /// </summary>
        /// <param name="plant">The plant</param>
        /// <param name="area">The area of the plant</param>
        /// <param name="theDate">The date of which to find the shift Number</param>
        public static byte ShiftNumber(MOO.Plant plant, MOO.Area area, DateTime theDate)
        {
            double hour = ShiftHour(plant, area, theDate);
            double sLength = ShiftLength(plant, area, theDate);
            short shift = Convert.ToInt16(Math.Floor(((hour - 1) / sLength) + 1));
            return Convert.ToByte(shift);
        }
        /// <summary>
        /// Gets the start and end date of the shift, this will return a date aray with position zero holding the start time and position 1 holding the end time
        /// </summary>
        /// <param name="plant">The plant</param>
        /// <param name="area">The area of the plant</param>
        /// <param name="theDate">The date of which to find the shift Start/End (Truncate to midnight)</param>
        /// <param name="shiftNbr">The shift number of which to find the shift Start/End</param>
        public static DateTime[] ShiftStartEndTime(MOO.Plant plant, MOO.Area area, DateTime theDate, byte shiftNbr)
        {
            int addMinutes;
            int sLength = ShiftLength(plant, area, theDate) * 60;
            addMinutes = ShiftOffset(plant, area, theDate) + ((shiftNbr - 1) * sLength);
            DateTime[] retVal = new DateTime[2];
            retVal[0] = theDate.Date.AddMinutes(addMinutes);
            retVal[1] = retVal[0].AddMinutes(sLength);
            return retVal;
        }

        /// <summary>
        /// Gets the current crew for the given Plant, Area, and date
        /// </summary>
        /// <param name="plant"></param>
        /// <param name="area"></param>
        /// <param name="theDate"></param>
        public static byte Crew(MOO.Plant plant, MOO.Area area, DateTime theDate)
        {
            byte shiftNbr = ShiftNumber(plant, area, theDate);
            DateTime shiftDate = ShiftDay(plant, area, theDate);
            return Crew(plant, area, shiftDate, shiftNbr);

        }
        /// <summary>
        /// Gets the current crew for the given Plant, Area, and date
        /// </summary>
        /// <param name="plant"></param>
        /// <param name="area"></param>
        /// <param name="ShiftDate"></param>
        /// <param name="shiftNbr">Shift Number</param>
        public static byte Crew(MOO.Plant plant, MOO.Area area, DateTime ShiftDate, byte shiftNbr)
        {
            byte[] crewPattern = new byte[1];
            DateTime truncDate = ShiftDate.Date;
            DateTime[] crewStartDate = new DateTime[1];
            //start date is the date of the beginning of the schedule
            if (plant == Plant.Keetac)
            {
                if (area == Area.Drilling)
                {
                    crewPattern = [0, 1, 1, 1, 1, 1, 0, 0, 2, 2, 2, 2, 2, 0, 0, 3, 3, 3, 3, 3, 0];
                    crewStartDate = [
                        DateTime.Parse("6/5/2011"),
                                 DateTime.Parse("6/12/2011"),
                                 DateTime.Parse("6/19/2011")];
                }
                else
                {
                    crewPattern = [1, 1, 0, 0, 0, 2, 2, 0];
                    if (area == Area.Pit || area == Area.Crusher)
                    {
                        crewStartDate = [
                            DateTime.Parse("1/8/2013"),
                                     DateTime.Parse("1/4/2013"),
                                     DateTime.Parse("1/10/2013"),
                                     DateTime.Parse("1/6/2013")];

                    }
                    else
                    {
                        crewStartDate = [
                            DateTime.Parse("1/8/2013"),
                                     DateTime.Parse("1/4/2013"),
                                     DateTime.Parse("1/10/2013"),
                                     DateTime.Parse("1/6/2013")];
                    }
                }
            }
            else
            {
                //Minntac
                switch (area)
                {
                    case Area.Drilling:
                        throw new NotImplementedException("Drilling Shift Not Implemented");
                    case Area.Pit:
                        crewPattern = [1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 0, 0, 1, 1, 1, 0, 0, 2, 2, 2, 0, 0];
                        crewStartDate = [
                            DateTime.Parse("1/14/2023"),
                                     DateTime.Parse("1/7/2023"),
                                     DateTime.Parse("1/28/2023"),
                                     DateTime.Parse("1/21/2023")];
                        break;
                    case Area.Crusher:
                        if (truncDate < MinntacCrusher12ChangeDate)
                        {
                            crewPattern = [1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 3, 3, 3, 3, 3, 3, 3, 0, 2, 2, 2, 2, 2, 2, 2, 0, 0];
                            crewStartDate = [
                            DateTime.Parse("1/25/2008"),
                                     DateTime.Parse("1/18/2008"),
                                     DateTime.Parse("1/11/2008"),
                                     DateTime.Parse("1/4/2008")];
                        }
                        else
                        {
                            //CRUSHER 12 hour schedule the shift date of the first 4-day night shift is the start of the schedule
                            crewPattern = [1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 0, 0, 1, 1, 1, 0, 0, 2, 2, 2, 0, 0];
                            crewStartDate = [
                            DateTime.Parse("1/7/2023"),
                                     DateTime.Parse("1/28/2023"),
                                     DateTime.Parse("1/21/2023"),
                                     DateTime.Parse("1/14/2023")];
                        }

                        break;
                    case Area.Concentrator:
                        //conc pattern and crews matches crusher pattern
                        crewPattern = [1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 0, 0, 1, 1, 1, 0, 0, 2, 2, 2, 0, 0];
                        crewStartDate = [
                            DateTime.Parse("1/7/2023"),
                                     DateTime.Parse("1/28/2023"),
                                     DateTime.Parse("1/21/2023"),
                                     DateTime.Parse("1/14/2023")];
                        break;
                    case Area.Agglomerator:
                        crewPattern = [1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 0, 0, 1, 1, 1, 0, 0, 2, 2, 2, 0, 0];
                        crewStartDate = [
                            DateTime.Parse("1/14/2012"),
                              DateTime.Parse("1/7/2012"),
                              DateTime.Parse("1/21/2012"),
                              DateTime.Parse("1/28/2012")];
                        break;
                }
            }

            int rotationDay;
            byte crew;
            for (crew = 1; crew <= crewStartDate.Length; crew++)
            {
                rotationDay = Convert.ToInt32(Math.Floor(truncDate.Subtract(crewStartDate[crew - 1]).TotalDays) % crewPattern.Length);
                if (rotationDay < 0)
                    rotationDay = crewPattern.Length + rotationDay;

                if (shiftNbr == crewPattern[rotationDay])
                    break;

            }
            return crew;

        }

        /// <summary>
        /// Gets the current shift of the specified crew on the specified date
        /// </summary>
        /// <param name="plant"></param>
        /// <param name="area"></param>
        /// <param name="theDate"></param>
        /// <param name="crewNbr">Crew Number</param>
        public static byte CrewShift(MOO.Plant plant, MOO.Area area, DateTime theDate, byte crewNbr)
        {
            int shiftCount = 24 / ShiftLength(plant, area, theDate);
            byte theCrew;
            byte retVal = 0;
            for (byte shift = 1; shift <= shiftCount; shift++)
            {
                theCrew = Crew(plant, area, theDate, shift);
                if (theCrew == crewNbr)
                {
                    retVal = shift;
                    break;
                }
            }
            return retVal;
        }

        /// <summary>
        /// Returns how many crews there are for a specific plant and area
        /// </summary>
        /// <param name="plant">The plant</param>
        /// <param name="area">The area</param>
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
        /// Gives the string name of the crew (A, B, C, D, E, F, G, H etc.)
        /// </summary>
        /// <param name="plant"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        public static string[] CrewNames(MOO.Plant plant, MOO.Area area)
        {
            if (plant == Plant.Keetac)
            {
                if (area == Area.Pit)
                    return ["E", "F", "G", "H"];
                else
                    return ["A", "B", "C", "D"];
            }
            else
            {
                //minntac
                if (area == Area.Crusher)
                    return ["1", "2", "3", "4"];
                else
                    return ["A", "B", "C", "D"];
            }
        }

    }
}
