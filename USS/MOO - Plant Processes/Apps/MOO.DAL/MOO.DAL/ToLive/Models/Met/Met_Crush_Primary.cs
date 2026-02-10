using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public  class Met_Crush_Primary
    {
        public DateTime Datex { get; set; }
        public byte Unit { get; set; }
        public Met_DMY Dmy { get; set; }
        public byte? Step { get; set; }
        public decimal? Tons { get; set; }
        public decimal? Feed_Time { get; set; }
        public decimal? Down_Time { get; set; }
        public decimal? Crushing_Time { get; set; }
        public decimal? Green_Idle_Time { get; set; }
        public decimal? Red_Idle_Time { get; set; }
        public decimal? Green_Lite_Hrs { get; set; }
        public decimal? No_Ore_Hrs { get; set; }
        public decimal? Hangup_Hrs { get; set; }
        public decimal? Sched_Hours { get; set; }
        public decimal? Avail_Hours { get; set; }
        public decimal? Trucks_Dumped { get; set; }

    }
}
