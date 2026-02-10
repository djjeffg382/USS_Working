using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Met_Crush_Secondary
    {
        public DateTime Datex { get; set; }
        public byte Unit { get; set; }
        public Met_DMY Dmy { get; set; }
        public byte? Step { get; set; }
        public decimal? Crush_Tons { get; set; }
        public decimal? Crush_Hours { get; set; }
        public decimal? Crush_Kwh { get; set; }
        public decimal? Metal_Dly_Hours { get; set; }
        public decimal? Sched_Hours { get; set; }
    }
}
