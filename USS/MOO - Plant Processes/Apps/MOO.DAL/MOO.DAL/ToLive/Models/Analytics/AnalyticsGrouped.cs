using MOO.DAL.ToLive.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class AnalyticsGrouped
    {
        public string DataGroup { get; set; }
        public DateTime[] Dates { get; set; } = [];
        public List<AnalyticsGrpRec> Records { get; set; } = [];
    }
}
