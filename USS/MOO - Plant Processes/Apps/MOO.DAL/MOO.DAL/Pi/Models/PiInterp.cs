using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Pi.Models
{
    public sealed class PiInterp
    {
        public string Tag { get; set; }
        public DateTime Time { get; set; }
        public double? Value { get; set; }
        public string SValue { get; set; }
        public int Status { get; set; }
        public TimeSpan TimeStep { get; set; }
    }
}
