using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Drill.Models
{
    public class Drilled_Hole
    {
        public MOO.Plant Plant { get; set; }
        public int Drilled_Hole_Id { get; set; }
        public Equip Equip { get; set; }
        public Pattern Pattern { get; set; }
        public Drill_Bit Drill_Bit { get; set; }
        public Operator Operator { get; set; }
        public Material Material { get; set; }
        public string Hole_Number { get; set; }
        public decimal Planned_Depth { get; set; }
        public decimal Drilled_Depth { get; set; }

        private DateTime _start_Date;
        public DateTime Start_Date {
            get { return new DateTime(_start_Date.Year, _start_Date.Month, _start_Date.Day, _start_Date.Hour, _start_Date.Minute, _start_Date.Second); ; }
            set { _start_Date = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second); }  //truncate any data to the second to eliminate milliseconds that may come from external systems
        }

        private DateTime _end_Date;
        public DateTime End_Date
        {
            get { return new DateTime(_end_Date.Year, _end_Date.Month, _end_Date.Day, _end_Date.Hour, _end_Date.Minute, _end_Date.Second); }
            set { _end_Date = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second); }  //truncate any data to the second to eliminate milliseconds that may come from external systems
        }



        public DateTime Shift_Date { get; set; }
        public short Shift { get; set; }

        public decimal? Design_Northing { get; set; }
        public decimal? Actual_Northing { get; set; }
        public decimal? Design_Easting { get; set; }
        public decimal? Actual_Easting { get; set; }
        public decimal? Design_Bottom { get; set; }
        public decimal? Actual_Bottom { get; set; }
        public decimal? Collar { get; set; }

        /// <summary>
        /// The id that links back to the original record for the source system
        /// </summary>
        public string Reference_Key { get; set; }

        public decimal? ROP_Ft_Per_Min { get; set; }

        public Redrill_Type Redrill_Type { get; set; }


        public decimal? Measured_Depth { get; set; }
        public bool Deep_Hole { get; set; }
        public Drilled_Hole_Notes Hole_Notes;

    }
}
