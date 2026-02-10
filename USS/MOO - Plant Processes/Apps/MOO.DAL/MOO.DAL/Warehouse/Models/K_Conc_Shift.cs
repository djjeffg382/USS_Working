using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Warehouse.Models
{
    public class K_Conc_Shift
    {
        public DateTime Shift_Date { get; set; }
        public byte Shift { get; set; }
        public decimal? Concentrate_Slurry_Gt { get; set; }
        public decimal? Tailings_Flow_Gpm { get; set; }
        public decimal? Tails_Density_Pct { get; set; }
        public decimal? Pri_Conc_Feed_Gt { get; set; }
        public decimal? Pri_Power_Consumption_Kwh { get; set; }
        public decimal? Sec_Power_Consumption_Kwh { get; set; }
        public decimal? Pri_Conc_Feed_Rate_Gtph { get; set; }
        public decimal? Tails_Mag_Fe_Pct { get; set; }
        public decimal? Nola_Sio2_Pct { get; set; }
        public decimal? Conc_Sio2_Pct { get; set; }
        public decimal? Conc_Fe_Pct { get; set; }
        public decimal? Blaine_Surface_Area_Cm2_G { get; set; }
        public DateTime? Start_Date { get; set; }
        public decimal? Coarse_Tails_Mag_Fe_Pct { get; set; }
        public decimal? Fine_Tails_Mag_Fe_Pct { get; set; }

    }
}
