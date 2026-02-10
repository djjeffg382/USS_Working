using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Drill.Models
{
    public class Pattern
    {
        public MOO.Plant Plant { get; set; }
        public int Pattern_Id { get; set; }
        public string Pattern_Number { get; set; }
        public Pit Pit { get; set; } = Pit.Not_Set;
        public Pattern_Size Pattern_Size { get; set; }
    }
}
