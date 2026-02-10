using System;

namespace MOO.DAL.ToLive.Models
{
    
    /// <summary>
    /// model for the agg2_shift tables
    /// </summary>
    public class Agg2_Shift
    {
        public int Agg2_Shift_Id { get; set; }
        public DateTime Report_Date { get; set; }
        public byte Shift { get; set; }
        public decimal? Lines_Running_Avg { get; set; }
        public decimal? Lines_Running_Sum { get; set; }
        public decimal? Tank_Flux_1_Pct { get; set; }
        public decimal? Tank_Flux_2_Pct { get; set; }
        public decimal? Tank_Slurry_1_1_Pct { get; set; }
        public decimal? Tank_Slurry_1_2_Pct { get; set; }
        public decimal? Ball_Hrs { get; set; }
        public decimal? Bent_Lbs { get; set; }
        public decimal? Bent_Lbs_Lton { get; set; }
        public decimal? Coal_Stons { get; set; }
        public decimal? DD1_InH2O { get; set; }
        public decimal? Fuel_Oil_Gals { get; set; }
        public decimal? Fuel_Oil_Tank_Lvl { get; set; }
        public decimal? Gas_MSCF { get; set; }
        public decimal? Grate_Hrs { get; set; }
        public decimal? MBTU_Lton { get; set; }
        public decimal? MBTUs { get; set; }
        public decimal? Pel_Ltons { get; set; }
        public decimal? Wood_Stons { get; set; }

        //Reclaim code
        public decimal? Recl_In_Ltons
        {
            get { return Recl_In_Act_Ltons + Recl_In_Adj_Ltons.GetValueOrDefault(0); }
            
        }
        public decimal? Recl_In_Act_Ltons { get; set; }

        public decimal? Recl_In_Adj_Ltons { get; set; }






        public decimal? Recl_Out_Ltons
        {
            get { return Recl_Out_Act_Ltons + Recl_Out_Adj_Ltons.GetValueOrDefault(0); }
            
        }
        public decimal? Recl_Out_Act_Ltons { get; set; }
        public decimal? Recl_Out_Adj_Ltons { get; set; }

        public double? Bin1_Pct { get; set; }
        public double? Bin2_Pct { get; set; }

    }
}
