using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Conc_Analytics_Recovery
    {
        public DateTime Datetime { get; set; }
        public double? Rmf_Rate_Total_Lt { get; set; }
        public double? S12_Magfe_Median_Pct { get; set; }
        public double? S3_Magfe_Median_Pct { get; set; }
        public double? Crse_Tail_Magfe_Pct { get; set; }
        public double? Fine_Tail_Magfe_Pct { get; set; }
        public double? Mag_Plnt_Conc_Sio2_Pct { get; set; }
        public double? Rougher_Fd_Sio2_Pct { get; set; }
        public double? Conc_Sio2_Pct { get; set; }
        public double? Float_Tail_Sio2_Pct { get; set; }
        public double? Mag_Plnt_Recov_Magfe_Pct { get; set; }
        public double? Flot_Recov_Magfe_Pct { get; set; }
        public double? Tot_Recov_Magfe_Pct { get; set; }
        public double? Mag_Plnt_Wgt_Magfe_Pct { get; set; }
        public double? Flot_Wgt_Magfe_Pct { get; set; }
        public double? Tot_Wgt_Magfe_Pct { get; set; }
        public double? Prev_Mag_Plnt_Recov_Magfe_Pct { get; set; }
        public double? Prev_Flot_Recov_Magfe_Pct { get; set; }
        public double? Prev_Tot_Recov_Mag_Fe_Pct { get; set; }
        public double? Prev_Mag_Plnt_Wgt_Magfe_Pct { get; set; }
        public double? Prev_Flot_Wgt_Magfe_Pct { get; set; }
        public double? Prev_Tot_Wgt_Magfe_Pct { get; set; }
        public double? Baseline_Norm_Mag_Recov { get; set; }
        public double? Baseline_Norm_Flot_Recov { get; set; }
        public double? Tot_Plnt_Magfe_Median_Pct { get; set; }
        public bool? Abnormal_Period { get; set; }
        public double? Num_Lines_Running { get; set; }  //this is set to a double because we can call a function to get average, otherwise this should be a short
        /// <summary>
        /// AA conc tons for the period, if the GetByDateRangeWeightedAvg is called, this will be the sum per day
        /// </summary>
        public double? AA_Conc_Tons { get; set; }
        public double? AA_Conc_Tons_Last_Day { get; set; }

    }
}
