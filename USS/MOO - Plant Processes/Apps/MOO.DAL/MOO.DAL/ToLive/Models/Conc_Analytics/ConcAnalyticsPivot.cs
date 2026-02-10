using MOO.DAL.ToLive.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class ConcAnalyticsPivot
    {
        public Conc_Analytics_Group DataGroup { get; set; }
        public int LineNumber { get; set; }
        public DateTime[] Dates { get; set; }
        public List<ConcAnalyticsPivotRec> Records { get; set; } = new();
    }
}
