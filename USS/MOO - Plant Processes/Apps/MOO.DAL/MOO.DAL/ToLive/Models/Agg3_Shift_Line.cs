using System;

namespace MOO.DAL.ToLive.Models
{
    /// <summary>
    /// Model for tolive.agg3_shift_line tables
    /// </summary>
    /// <remarks>some columns were not included as they look to be not used or are irrelevant</remarks>
    public class Agg3_Shift_Line
    {
        public int Agg3_Shift_Id { get; set; }
        public DateTime Report_Date { get; set; }
        public byte Shift { get; set; }
        
        public byte Line { get; set; }
        public decimal? Status_On { get; set; }
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
        public decimal? Wood_Stons { get; set; }

        public string General_Desc { get; set; }
        /// <summary>
        /// After Tumbles 1/4 inch
        /// </summary>
        public decimal? At_14 { get; set; }
        public decimal? Bed_Depth_In { get; set; }
        public decimal? Belt_037_LTPH { get; set; }
        public decimal? Bin_Pct { get; set; }
        /// <summary>
        /// Before Tumbles 1/4 Inch
        /// </summary>
        public decimal? Bt_14 { get; set; }
        public decimal? Burn_Zone_Temp { get; set; }
        public decimal? Comp600 { get; set; }
        public decimal? DD1_Temp { get; set; }
        public decimal? DD2_Temp { get; set; }
        public decimal? Feed_Grate_LTPH { get; set; }
        public decimal? Feed_Rate_LTPH { get; set; }
        public decimal? Grate_Spd_IPM { get; set; }
        public decimal? Kiln_Exit_Temp { get; set; }
        public decimal? Comp600_M200 { get; set; }
        public decimal? U_Grate_Temp { get; set; }


        //Pellet tons fields, these values are interconnected so modify the values when one is changed
        private decimal? _pel_Ltons;
        public decimal? Pel_Ltons { 
            get
            {
                return _pel_Ltons;
            }
            set
            {
                _pel_Ltons = value;
                _pel_Adj_Ltons = _pel_Ltons.GetValueOrDefault(0) - _pel_Act_Ltons.GetValueOrDefault(0);
            }
        }
        private decimal? _pel_Act_Ltons;
        public decimal? Pel_Act_Ltons { 
            get
            {
                return _pel_Act_Ltons;
            }
            set
            {
                _pel_Act_Ltons = value;
                _pel_Ltons = _pel_Act_Ltons.GetValueOrDefault(0) + _pel_Adj_Ltons.GetValueOrDefault(0);
            }
        }

        private decimal? _pel_Adj_Ltons;
        public decimal? Pel_Adj_Ltons { 
            get
            {
                return _pel_Adj_Ltons;
            }
            set
            {
                _pel_Adj_Ltons = value;
                _pel_Ltons = _pel_Act_Ltons.GetValueOrDefault(0) + _pel_Adj_Ltons.GetValueOrDefault(0);
            }
        }

    }
}
