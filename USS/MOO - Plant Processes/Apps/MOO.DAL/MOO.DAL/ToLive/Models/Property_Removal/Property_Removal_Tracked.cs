using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Property_Removal_Tracked
    {
        public Sec_Users Tracked_By { get; set; }
        public Property_Removal_Reasons Reason { get; set; }
        public MOO.Plant Plant { get; set; }
    }
}
