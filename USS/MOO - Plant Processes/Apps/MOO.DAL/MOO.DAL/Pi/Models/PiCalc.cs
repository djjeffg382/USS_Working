using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Pi.Models
{
    public sealed class PiCalc
    {
        public string Expr { get; set; }
        public DateTime Time { get; set; }
        public double? Value { get; set; }
        public TimeSpan TimeStep { get; set; }
    }
}
