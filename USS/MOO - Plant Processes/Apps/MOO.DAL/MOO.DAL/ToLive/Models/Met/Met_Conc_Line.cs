using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Met_Conc_Line
    {
        public DateTime Datex { get; set; }
        public byte Line { get; set; }
        public Met_DMY Dmy { get; set; }
        public decimal? Rmf_Tons { get; set; }
        public decimal? Flux_Tons { get; set; }
        public decimal? Rmf_Hours { get; set; }
        public decimal? Rgrnd_Hours { get; set; }
        public decimal? Flux_Hours { get; set; }
        public decimal? Rmf_Rm_Power { get; set; }
        public decimal? Flux_Rm_Power { get; set; }
        public decimal? Rmf_Pbm_Power { get; set; }
        public decimal? Regrind_Pbm_Power { get; set; }
        public decimal? Flux_Pbm_Power { get; set; }
        public decimal? Rmf_Sbm_Power { get; set; }
        public decimal? Regrind_Sbm_Power { get; set; }
        public decimal? Flux_Sbm_Power { get; set; }
        public decimal? Coil_Mag_Fe { get; set; }
        public decimal? Conc_Sio2 { get; set; }
        public decimal? Conc_270 { get; set; }
        public decimal? Conc_Tons { get; set; }
        public decimal? Ct_Mag_Fe { get; set; }
        public decimal? Ft_Mag_Fe { get; set; }
        public decimal? Rmf_Tons_Adj { get; set; }
        public decimal? Schedhours { get; set; }
        public decimal? Flux_Minus_200 { get; set; }

    }
}
