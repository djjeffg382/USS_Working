using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    //this is a pivot of the Conc Analytics table
    public sealed class ConcAnalyticsPivotRec
    {

        public ConcAnalyticsPivot Parent { get; set; }
        public string Label { get; set; }
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
