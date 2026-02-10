using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.LIMS.Models
{
    public class OilResults
    {
        public long Sample_Nbr { get; set; }
        public DateTime Sample_Date { get; set; }
        public DateTime Login_Date { get; set; }
        public DateTime? Recd_Date { get; set; }
        public string Status { get; set; }
        public bool Out_Of_Spec { get; set; }

        public string SamplePoint { get; set; }

        public double? Glycol { get; set; }
        public bool? Glycol_OOS { get; set; }
        public double? Glycol_Min { get; set; }
        public double? Glycol_Max { get; set; }
        public double? Soot_3800 { get; set; }
        public bool? Soot_3800_OOS { get; set; }
        public double? Soot_3800_Min { get; set; }
        public double? Soot_3800_Max { get; set; }
        public double? Soot_1980 { get; set; }
        public bool? Soot_1980_OOS { get; set; }
        public double? Soot_1980_Min { get; set; }
        public double? Soot_1980_Max { get; set; }
        public double? Oxidation { get; set; }
        public bool? Oxidation_OOS { get; set; }
        public double? Oxidation_Min { get; set; }
        public double? Oxidation_Max { get; set; }
        public double? Water { get; set; }
        public bool? Water_OOS { get; set; }
        public double? Water_Min { get; set; }
        public double? Water_Max { get; set; }
        public double? Cu { get; set; }
        public bool? Cu_OOS { get; set; }
        public double? Cu_Min { get; set; }
        public double? Cu_Max { get; set; }
        public double? Fe { get; set; }
        public bool? Fe_OOS { get; set; }
        public double? Fe_Min { get; set; }
        public double? Fe_Max { get; set; }
        public double Cr { get; set; }
        public bool? Cr_OOS { get; set; }
        public double? Cr_Min { get; set; }
        public double? Cr_Max { get; set; }
        public double? Al { get; set; }
        public bool? Al_OOS { get; set; }
        public double? Al_Min { get; set; }
        public double? Al_Max { get; set; }
        public double? Si { get; set; }
        public bool? Si_OOS { get; set; }
        public double? Si_Min { get; set; }
        public double? Si_Max { get; set; }
        public double? Pb { get; set; }
        public bool? Pb_OOS { get; set; }
        public double? Pb_Min { get; set; }
        public double? Pb_Max { get; set; }
        public double? Ca { get; set; }
        public bool? Ca_OOS { get; set; }
        public double? Ca_Min { get; set; }
        public double? Ca_Max { get; set; }
        public double? Mg { get; set; }
        public bool? Mg_OOS { get; set; }
        public double? Mg_Min { get; set; }
        public double? Mg_Max { get; set; }
        public double? Zn { get; set; }
        public bool? Zn_OOS { get; set; }
        public double? Zn_Min { get; set; }
        public double? Zn_Max { get; set; }
        public double? P { get; set; }
        public bool? P_OOS { get; set; }
        public double? P_Min { get; set; }
        public double? P_Max { get; set; }
        public double? Mo { get; set; }
        public bool? Mo_OOS { get; set; }
        public double? Mo_Min { get; set; }
        public double? Mo_Max { get; set; }
        public double? Sn { get; set; }
        public bool? Sn_OOS { get; set; }
        public double? Sn_Min { get; set; }
        public double? Sn_Max { get; set; }
        public double? K { get; set; }
        public bool? K_OOS { get; set; }
        public double? K_Min { get; set; }
        public double? K_Max { get; set; }
        public double? Na { get; set; }
        public bool? Na_OOS { get; set; }
        public double? Na_Min { get; set; }
        public double? Na_Max { get; set; }
        public double? Ni { get; set; }
        public bool? Ni_OOS { get; set; }
        public double? Ni_Min { get; set; }
        public double? Ni_Max { get; set; }
        public double? Ag { get; set; }
        public bool? Ag_OOS { get; set; }
        public double? Ag_Min { get; set; }
        public double? Ag_Max { get; set; }
        public double? ISO4 { get; set; }
        public bool? ISO4_OOS { get; set; }
        public double? ISO4_Min { get; set; }
        public double? ISO4_Max { get; set; }
        public double? ISO6 { get; set; }
        public bool? ISO6_OOS { get; set; }
        public double? ISO6_Min { get; set; }
        public double? ISO6_Max { get; set; }
        public double? ISO14 { get; set; }
        public bool? ISO14_OOS { get; set; }
        public double? ISO14_Min { get; set; }
        public double? ISO14_Max { get; set; }
        public double? Viscosity { get; set; }
        public bool? Viscosity_OOS { get; set; }
        public double? Viscosity_Min { get; set; }
        public double? Viscosity_Max { get; set; }
        public double? PQ { get; set; }
        public bool? PQ_OOS { get; set; }
        public double? PQ_Min { get; set; }
        public double? PQ_Max { get; set; }
        public double? Glycol_GC { get; set; }
        public bool? Glycol_GC_OOS { get; set; }
        public double? Glycol_GC_Min { get; set; }
        public double? Glycol_GC_Max { get; set; }
        public double? Diesel { get; set; }
        public bool? Diesel_OOS { get; set; }
        public double? Diesel_Min { get; set; }
        public double? Diesel_Max { get; set; }
    }
}
