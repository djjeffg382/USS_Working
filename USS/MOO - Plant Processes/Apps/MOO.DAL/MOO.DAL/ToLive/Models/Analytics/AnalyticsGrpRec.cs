using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    /// <summary>
    /// Grouping of Analytics record by Plant, Area, Data_Group, Line, Label of the Analytics table
    /// </summary>
    /// <remarks>
    /// This will be a grouping of the analytics table by Analytics record by Plant, Area, Data_Group, Line, Label with records for each Date
    /// </remarks>
    public sealed class AnalyticsGrpRec
    {

        public AnalyticsGrouped Parent { get; set; }
        public string Label { get; set; }
        public int LineNbr { get; set; }
        public int SortOrder { get; set; }

        public string[] Values;
        public DateTime[] Dates
        {
            get
            {
                if (Parent != null)
                    return Parent.Dates;
                return null;
            }

        }
        public string[] Quality;
    }
}
