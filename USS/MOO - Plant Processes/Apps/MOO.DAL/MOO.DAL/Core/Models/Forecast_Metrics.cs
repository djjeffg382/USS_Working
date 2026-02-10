using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Core.Models
{
    public sealed class Forecast_Metrics
    {
        public string Name { get; set; }
        public int CoreId { get; set; }
        public string AccessGroup { get; set; }

        public List<string> Users { get; set; } = new();

        public string UserCSV { get { return string.Join(",", Users); } }        

        public string Uom { get; set; }
        public MOO.Plant Plant { get; set; }
        public decimal ValueType { get; set; }
        public int? SortOrder { get; set; }
        public string DailyVals { get; set; }
        public bool BpVal { get; set; }
        public MOO.Area? StoreByShiftArea { get; set; }
        public bool Active { get; set; }
        public byte? ShiftCount { get; set; }
    }
}
