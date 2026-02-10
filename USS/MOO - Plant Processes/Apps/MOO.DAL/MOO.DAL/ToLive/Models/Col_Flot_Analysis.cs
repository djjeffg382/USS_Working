using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Col_Flot_Analysis
    {
        public enum SampleType
        {
            Froth,
            Conc,
            Feed
        }

        public DateTime Datex { get; set; }
        public byte Shift { get; set; }
        public SampleType Samplet { get; set; }
        public decimal? Si { get; set; }
        public string Ana_Inits { get; set; }
        public DateTime? Rdate { get; set; }
    }
}
