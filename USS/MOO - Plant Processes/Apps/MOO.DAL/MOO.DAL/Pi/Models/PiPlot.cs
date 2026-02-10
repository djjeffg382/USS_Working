using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Pi.Models
{
    public sealed class PiPlot
    {
        public string Tag { get; set; }
        public DateTime Time { get; set; }
        public decimal? Value { get; set; } = null;
        public int Status { get; set; }
        public bool Questionable { get; set; }
        public bool Substituted { get; set; }
        public bool Annotated { get; set; }
        public string Annotations { get; set; }
        public int IntervalCount { get; set; }
    }
}
