using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class NodeScan_Tbl
    {
        public string NodeName { get; set; }
        public bool IsActive { get; set; }
        public DateTime Datetime { get; set; }
        public string Description { get; set; }
        public bool IsScheduledDown { get; set; }
        public string Email_List { get; set; }
    }
}
