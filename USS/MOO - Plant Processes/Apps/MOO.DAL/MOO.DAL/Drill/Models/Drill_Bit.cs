using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Drill.Models
{
    public class Drill_Bit
    {
        public MOO.Plant Plant { get; set; }
        public int Drill_Bit_Id { get; set; }
        public string Manufacturer { get; set; }
        public string Serial_Number { get; set; }
    }
}
