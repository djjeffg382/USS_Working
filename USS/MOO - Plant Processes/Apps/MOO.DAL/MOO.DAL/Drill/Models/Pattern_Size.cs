using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Drill.Models
{
    public class Pattern_Size
    {
        public MOO.Plant Plant { get; set; }
        public int Pattern_Size_Id { get; set; }
        public string Description { get; set; }
        public int Holes_Length { get; set; }
        public int Holes_Width { get; set; }
        public decimal LTons_Per_Foot { get; set; }
    }
}
