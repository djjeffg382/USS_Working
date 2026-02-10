using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Drill.Models
{
    /// <summary>
    /// Raw data of the drilled hole from the source Drill system
    /// </summary>
    /// <remarks>This class will be used for a drill system to fill out information about holes drilled.  The import will then use this information
    /// to translate into a Drilled_Hole object that will then be inserted into the Drill database.</remarks>
    public class Raw_Drilled_Hole
    {
        public MOO.Plant Plant { get; set; }
        public string Equipment_Number { get; set; } = "";
        public string Pattern_Number { get; set; } = "";

        public string Drill_Bit_Manufacturer { get; set; }
        public string Drill_Bit_Serial { get; set; } = "";
        public string Operator_Number { get; set; } = "";
        public string Operator_First_Name { get; set; } = "";
        public string Operator_Last_Name{ get; set; } = "";
        public Material Material { get; set; } = MOO.DAL.Drill.Models.Material.Unknown;
        public string Hole_Number { get; set; } = "";
        public decimal Planned_Depth { get; set; }
        public decimal Drilled_Depth { get; set; }
        public DateTime Start_Date { get; set; }
        public DateTime End_Date { get; set; }

        public decimal? Design_Northing { get; set; }
        public decimal? Actual_Northing { get; set; }
        public decimal? Design_Easting { get; set; }
        public decimal? Actual_Easting { get; set; }
        public decimal? Design_Bottom { get; set; }
        public decimal? Actual_Bottom { get; set; }
        /// <summary>
        /// Collar Elevation
        /// </summary>
        public decimal? Collar { get; set; }

        /// <summary>
        /// The id that links back to the original record for the source system
        /// </summary>
        public string Reference_Key { get; set; } = "";
        public decimal? ROP_Ft_Per_Min { get; set; }
    }
}