using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Warehouse.Models
{
    public class K_Pell_Ind_Line_Half
    {
        public DateTime Shift_Date { get; set; }
        public byte Line { get; set; }
        public byte? Shift { get; set; }
        public byte Half { get; set; }
        public decimal? Coal_Mmbtu { get; set; }
        public decimal? Coal_Usage_Lbs { get; set; }
        public decimal? Kiln_Gas_Mmbtu { get; set; }
        public decimal? Kiln_Gas_Usage_Mscf { get; set; }
        public decimal? Pellet_Tons_Dry_Gt { get; set; }
        public decimal? Pellet_Tons_Gt { get; set; }
        public decimal? Pell_Scn_Plus_1_2_Pct { get; set; }
        public decimal? Pell_Scn_Plus_1_4_Pct { get; set; }
        public decimal? Pell_Tumble_Plus_1_4_Pct { get; set; }
        public decimal? Pell_Tumble_Plus_30_Pct { get; set; }
        public decimal? Pell_Compression_Lbs { get; set; }
        public decimal? Pell_Less_300_Pct { get; set; }
        public decimal? Pell_Less_200_Pct { get; set; }
        public decimal? Pell_Fe_Pct { get; set; }
        public decimal? Pell_Sio2_Pct { get; set; }
        public decimal? Pell_Cao_Pct { get; set; }
        public decimal? Pell_Moisture_Pct { get; set; }
        public decimal? Pell_Mag_Pct { get; set; }
        public DateTime? Start_Date { get; set; }
        public decimal? Pell_Mgo_Pct { get; set; }
        public decimal? Pell_Al2o3_Pct { get; set; }
        public decimal? Pell_Mn_Pct { get; set; }
        public decimal? Pell_Phos_Pct { get; set; }
        public decimal? Pell_Sulfur_Pct { get; set; }
        public decimal? Pell_Fe_Plus_3_Pct { get; set; }
        public decimal? Pell_Scn_Plus_7_16_Pct { get; set; }
        public decimal? Pell_Scn_Plus_3_8_Pct { get; set; }
        public decimal? Pell_Tumble_Plus_1_2_Pct { get; set; }
        public decimal? Pell_Tumble_Plus_7_16_Pct { get; set; }
        public decimal? Pell_Tumble_Plus_3_8_Pct { get; set; }

    }
}
