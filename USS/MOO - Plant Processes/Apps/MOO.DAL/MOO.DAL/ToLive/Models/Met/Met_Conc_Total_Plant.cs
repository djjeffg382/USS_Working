using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Met_Conc_Total_Plant
    {
        public DateTime Datex { get; set; }
        public Met_DMY Dmy { get; set; }
        public decimal? Rmf_Tons_Natural { get; set; }
        public decimal? Rmf_Moisture { get; set; }
        public decimal? Rmf_Tons_Dry { get; set; }
        public decimal? Rmf_Avg_Ltph { get; set; }
        public decimal? Rmf_Pct_Mag_Fe { get; set; }
        public decimal? Rmf_Pct_Mag_Fe_Coils { get; set; }
        public decimal? Conc_Tons_Nat { get; set; }
        public decimal? Conc_Tons_Dry { get; set; }
        public decimal? Conc_Nat_Ltph { get; set; }
        public decimal? Conc_Pct_Sio2 { get; set; }
        public decimal? Conc_Pct_Mag_Fe { get; set; }
        public decimal? Conc_Pct_270m { get; set; }
        public decimal? Coarse_Tails_Tons_Dry { get; set; }
        public decimal? Fine_Tails_Tons_Dry { get; set; }
        public decimal? Coarse_Tails_Pct_Mag_Fe { get; set; }
        public decimal? Fine_Tails_Pct_Mag_Fe { get; set; }
        public decimal? Pct_Recovery_Natural { get; set; }
        public decimal? Pct_Recovery_Dry { get; set; }
        public decimal? Pct_Mag_Fe_Recovery { get; set; }
        public decimal? Scheduled_Hours { get; set; }
        public decimal? Actual_Hours { get; set; }
        public decimal? Pct_Operating_Time { get; set; }
        public decimal? Scheduled_Maint_Hours { get; set; }
        public decimal? Pct_Scheduled_Maint { get; set; }
        public decimal? Unscheduled_Maint_Hours { get; set; }
        public decimal? Pct_Unscheduled_Maint { get; set; }
        public decimal? Aggl_Request_Hours { get; set; }
        public decimal? Pct_Aggl_Request { get; set; }
        public decimal? High_Power_Limit_Hours { get; set; }
        public decimal? Pct_High_Power_Limit { get; set; }
        public decimal? No_Ore_Hours { get; set; }
        public decimal? Pct_No_Ore { get; set; }
        public decimal? Other_Delay_Hours { get; set; }
        public decimal? Pct_Other_Delay { get; set; }
        public decimal? Avail_Hours { get; set; }
        public decimal? Pct_Avail { get; set; }
        public decimal? Kwh_Per_Ton_Rmf { get; set; }
        public decimal? Rod_Mill_Dav_Tube_Sio2 { get; set; }
        public decimal? Rmf_Pct_34_Inch { get; set; }
        public decimal? Rmf_Pct_12_Inch { get; set; }
        public decimal? Flux_Conc_To_Agglom { get; set; }
        public decimal? Acid_Conc_To_Agglom { get; set; }
        public decimal? Total_Conc_To_Agglom { get; set; }

    }
}
