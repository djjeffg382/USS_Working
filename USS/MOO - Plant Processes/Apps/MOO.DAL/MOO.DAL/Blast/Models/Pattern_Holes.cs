using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Blast.Models
{
    public  class Pattern_Holes
    {
        public int Pattern_Hole_Id { get; set; }
        public string Hole_Number { get; set; }
        public decimal? Northing { get; set; }
        public decimal? Easting { get; set; }
        public decimal? Planned_Depth { get; set; }
        public decimal? Bottom_Altitude { get; set; }
        public Pattern Pattern { get; set; }

    }
}
