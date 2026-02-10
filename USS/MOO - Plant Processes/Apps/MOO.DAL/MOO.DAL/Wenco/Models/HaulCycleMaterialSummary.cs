using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Wenco.Models
{
    public class HaulCycleMaterialSummary
    {
        public DateTime ShiftDate { get; set; }
        public byte Shift { get; set; }

        /// <summary>
        /// Ore that was dumped to the primary crusher stockpile
        /// </summary>
        public decimal OreStocked { get; set; } = 0;

        /// <summary>
        /// Ore taken from the crusher stockpile and dumped into the crusher
        /// </summary>
        public decimal OreDestocked { get; set; } = 0;

        /// <summary>
        /// Ore dumped into crusher that came directly from a loading unit
        /// </summary>
        public decimal OreDirect { get; set; } = 0;

        /// <summary>
        /// Other Material Handled
        /// </summary>
        public decimal Omh { get; set; } = 0;

        public decimal Surface { get; set; } = 0;

        public decimal Waste { get; set; } = 0;

        /// <summary>
        /// Total Ore dropped into crusher
        /// </summary>
        public decimal OreCrushed { get { return OreDirect + OreDestocked; } }

    }
}
