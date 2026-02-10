using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Drill.Models
{
    public sealed class Drilled_Hole_Notes
    {
        public short Drilled_Hole_Notes_Id { get; set; }
        public string Notes { get; set; } = "";
        public bool Active { get; set; }
    }
}
