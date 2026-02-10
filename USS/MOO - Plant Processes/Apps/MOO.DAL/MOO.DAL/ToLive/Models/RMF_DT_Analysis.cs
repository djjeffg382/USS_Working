using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class RMF_DT_Analysis
    {
        public DateTime Datex { get; set; }
        public byte Step { get; set; }
        public decimal? Si { get; set; }
        public string Ana_Inits { get; set; }
        public DateTime? Rdate { get; set; }

    }
}
