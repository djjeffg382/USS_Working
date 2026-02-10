using System;

namespace MOO.DAL.ToLive.Models
{
    /// <summary>
    /// model for the tolive.agg3_shift_step
    /// </summary>
    public class Agg3_Shift_Step
    {
        public int Agg3_Shift_Id { get; set; }
        public int Step { get; set; }
        public DateTime Report_Date { get; set; }
        public byte Shift { get; set; }

        public decimal? Ball_Hrs { get; set; }
        public decimal? Bent_Lbs { get; set; }
        public decimal? Bent_Lbs_Lton { get; set; }
        public decimal? Coal_Stons { get; set; }
        public decimal? DD1_InH2O { get; set; }
        public decimal? Fuel_Oil_Gals { get; set; }
        public decimal? Gas_MSCF { get; set; }
        public decimal? Grate_Hrs { get; set; }
        public decimal? MBTU_Lton { get; set; }
        public decimal? MBTUs { get; set; }
        public decimal? Pel_Ltons { get; set; }
        public decimal? Wood_Stons { get; set; }
        public decimal? Drum_Down_Time_Min { get; set; }
        public decimal? Down_Time_Min { get; set; }
        public decimal? Filter_Avail { get; set; }
        public decimal? Filter_On { get; set; }
        public decimal? FiltRate { get; set; }
        public decimal? Tot_203_Ltons { get; set; }
        public decimal? Trip_Moist { get; set; }
        public decimal? Vac_Pump_On { get; set; }
    }
}
