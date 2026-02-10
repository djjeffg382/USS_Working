using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Conc_Analytics_Flot_Recov_KPI
    {
        public DateTime Datetime { get; set; }
        public double? Total_Rmf { get; set; }
        public double? Flot_Rec { get; set; }
        public double? Ff_Sp { get; set; }
        public double? Flot_Base_Cor { get; set; }
        public double? Flot_Base_Norm { get; set; }
        public double? Mag_Act { get; set; }
        public double? Coil { get; set; }
        public double? Mag_Base_Cor { get; set; }
        public double? Mag_Base_Norm { get; set; }

    }
}
