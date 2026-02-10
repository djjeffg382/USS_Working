using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{

    //pulled from the Conc_Analytics_General table
    public sealed class ConcAnalyticsImpact
    {
        public DateTime TheDate { get; set; }
        public short LineNbr { get; set; }
        public string TimeCat { get; set; } = "";
        public decimal Uplift_Long_Tons { get; set; }
        public decimal Uplift_Pct { get; set; }
    }
}
