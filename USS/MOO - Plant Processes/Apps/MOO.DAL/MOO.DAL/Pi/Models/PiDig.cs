using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Pi.Models
{
    /// <summary>
    /// Used to return PI Digital Values from the PI Archives
    /// </summary>
    public sealed class PiDig
    {
        public string Expr { get; set; }
        public DateTime Time { get; set; }
        public int? Value { get; set; }
        public TimeSpan TimeStep { get; set; }
    }
}
