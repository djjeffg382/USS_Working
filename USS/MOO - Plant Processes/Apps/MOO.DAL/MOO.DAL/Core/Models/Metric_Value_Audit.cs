using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Core.Models
{
    /// <summary>
    /// Audit information for values inserted into Metric_Value table (core.metric_value_audit)
    /// </summary>
    public class Metric_Value_Audit
    {
        public long Metric_Id { get; set; }
        public DateTime Start_Date_No_DST { get; set; }
        public decimal? Old_Value { get; set; }
        public decimal? New_Value { get; set; }
        public DateTime Date_Changed { get; set; }
        public string Changed_By { get; set; }
    }
}
