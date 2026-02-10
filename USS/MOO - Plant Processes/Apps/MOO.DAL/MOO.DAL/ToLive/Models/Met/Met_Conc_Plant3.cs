using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Met_Conc_Plant3
    {
        public DateTime Datex { get; set; }
        public Met_DMY Dmy { get; set; }
        public decimal? Sched_Hours { get; set; }
        public decimal? Sched_Maint_Hours { get; set; }
        public decimal? Unsched_Maint_Hours { get; set; }
        public decimal? Aggl_Request_Hours { get; set; }
        public decimal? Hi_Power_Lim_Hours { get; set; }
        public decimal? No_Ore_Hours { get; set; }
        public decimal? Rmf_H2o { get; set; }
        public decimal? Rmf_Mag_Fe { get; set; }
        public decimal? Plant_34_Inch { get; set; }
        public decimal? Plant_12_Inch { get; set; }
        public decimal? Dav_Tube_Rm_Sio2 { get; set; }
        public decimal? Step3_Nola_Sio2 { get; set; }
        public decimal? Lab_Flot_Sio2 { get; set; }
    }
}
