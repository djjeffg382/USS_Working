using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.Shifts
{
    /// <summary>
    /// Model for holding data related to a shift 
    /// </summary>
    public class ShiftVal
    {
        /// <summary>
        /// The plant (Minntac/Keetac)
        /// </summary>
        public Plant Plant { get; set; }
        /// <summary>
        /// The area at the plant
        /// </summary>
        public Area Area { get; set; }
        /// <summary>
        /// Start Time of the shift
        /// </summary>
        public DateTime ShiftStartTime { get; set; }
        /// <summary>
        /// EndTime of the shift
        /// </summary>
        public DateTime EndShiftTime { get; set; }
        /// <summary>
        /// Shift number
        /// </summary>
        public byte ShiftNumber { get; set; }
        /// <summary>
        /// The Shift date for the shift
        /// </summary>
        public DateTime ShiftDate { get; set; }

        /// <summary>
        /// Length of the shift in hours
        /// </summary>
        public byte ShiftLength { get; set; }

        /// <summary>
        /// Offset in minutes for the shift 1 from Midnight 
        /// </summary>
        public short ShiftOffset { get; set; }

        /// <summary>
        /// Crew for the specified shift
        /// </summary>
        public byte Crew { get; set; }

        /// <summary>
        /// Name of the crew
        /// </summary>
        public string CrewName { get; set; }

        /// <summary>
        /// Whether this is the 8 or 12 hour shift type
        /// </summary>
        public ShiftTypes ShiftType { get; set; }


        /// <summary>
        /// Returns the Next Shift object
        /// </summary>
        /// <returns></returns>
        public ShiftVal NextShift()
        {

            ShiftVal retVal;
            if (ShiftType == ShiftTypes.Shift_8_Hour)
                retVal = MOO.Shifts.Shift8.GetShiftVal(Plant, Area, ShiftStartTime.AddHours(ShiftLength));
            else
                retVal = MOO.Shifts.Shift.GetShiftVal(Plant, Area, ShiftStartTime.AddHours(ShiftLength));
            return retVal;
        }

        /// <summary>
        /// Returns the Next Shift object
        /// </summary>
        /// <returns></returns>
        public ShiftVal PreviousShift()
        {

            ShiftVal retVal;
            if (ShiftType == ShiftTypes.Shift_8_Hour)
                retVal = MOO.Shifts.Shift8.GetShiftVal(Plant, Area, ShiftStartTime.AddHours(ShiftLength * -1));
            else
                retVal = MOO.Shifts.Shift.GetShiftVal(Plant, Area, ShiftStartTime.AddHours(ShiftLength * -1));

            return retVal;
        }

    }
}
