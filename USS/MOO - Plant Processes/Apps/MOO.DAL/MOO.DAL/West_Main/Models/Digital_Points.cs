using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.West_Main.Models
{
    public class Digital_Points
    {
        public MOO.Plant Plant { get; set; }
        public string Area { get; set; }
        public string Tag { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string On_Val { get; set; }
        public string Off_Val { get; set; }

        public DateTime Last_Update { get; set; }
    }
}
