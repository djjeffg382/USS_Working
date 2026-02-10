using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Agg3_Day
    {
        public int Agg3_Day_Id { get; set; }
        public DateTime Report_Date { get; set; }
        public decimal? Lines_Running_Avg { get; set; }
        public decimal? Pday_Lines_Running_Avg { get; set; }
        public string Created_User { get; set; }
        public DateTime Created_Date { get; set; }
        public string Updated_User { get; set; }
        public DateTime Updated_Date { get; set; }
        public decimal? Ball_Hrs { get; set; }
        public decimal? Bent_Lbs { get; set; }
        public decimal? Bent_Lbs_Lton { get; set; }
        public decimal? Bin_6_Pct { get; set; }
        public decimal? Bin_7_Pct { get; set; }
        public decimal? Coal_Stons { get; set; }
        public decimal? Dd1_Inh2o { get; set; }
        public decimal? Fuel_Oil_Gals { get; set; }
        public decimal? Fuel_Oil_Tank_Lvl { get; set; }
        public decimal? Gas_Mscf { get; set; }
        public decimal? Grate_Hrs { get; set; }
        public decimal? Lines_Running_Sum { get; set; }
        public decimal? Mbtu_Lton { get; set; }
        public decimal? Mbtus { get; set; }
        public decimal? Mo_To_Day_Pel_Ltons { get; set; }
        public decimal? Mo_To_Day_Pel_Frcst_Ltons { get; set; }
        public decimal? Pel_Ltons { get; set; }
        public decimal? Recl_In_Ltons { get; set; }
        public decimal? Recl_Out_Ltons { get; set; }
        public decimal? Recl_Flux_Bal_Ltons { get; set; }
        public decimal? Tank_Slurry_2_1_Pct { get; set; }
        public decimal? Tank_Slurry_2_2_Pct { get; set; }
        public decimal? Wood_Stons { get; set; }
        public decimal? Drum_Down_Time_Min { get; set; }
        public decimal? Down_Time_Min { get; set; }
        public decimal? Recl_In_Act_Ltons { get; set; }
        public decimal? Recl_Out_Act_Ltons { get; set; }
        public decimal? Recl_In_Adj_Ltons { get; set; }
        public decimal? Recl_Out_Adj_Ltons { get; set; }
        public decimal? Recl_Bal_Ltons { get; set; }
        public decimal? Nox_Lbs_Hr { get; set; }
        public decimal? So2_Lbs_Hr { get; set; }

    }
}
