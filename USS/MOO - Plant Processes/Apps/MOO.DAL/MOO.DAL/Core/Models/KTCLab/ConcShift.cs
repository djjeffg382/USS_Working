using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Core.Models.KTCLab
{
    public class ConcShift
    {
        public DateTime Start_Date_No_DST { get; set; }
        public DateTime Start_Date { get; set; }
        /// <summary>
        /// Shift Number
        /// </summary>
        public byte? Shift { get; set; }

        /// <summary>
        /// Shift Half
        /// </summary>
        public byte? Half { get; set; }

        /// <summary>
        /// Shift hour
        /// </summary>
        public byte? Hour { get; set; }

        /// <summary>
        /// Shift Date
        /// </summary>
        public DateTime? Shift_Date { get; set; }
        public decimal? TailsHydro { get; set; }
        public decimal? TailsThickener { get; set; }
        public decimal? NOLA_SiO2 { get; set; }

        //the following 2 properties were at one time recorded every 8 hours.  This was changed to a 4 hour.  As this model is per 8 hour, 
        //I turned this into an array
        public decimal?[] Control_SiO2 { get; set; } = [null, null];
        public decimal?[] ConcFe { get; set; } = [null, null];

        public Approval Approval { get; set; }
    }
}
