using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MOO.DAL.LIMS_Report.Models
{
    public class Oil_Results
    {
        public long Sample_Nbr { get; set; }
        public string Status { get; set; }
        public DateTime Login_Date { get; set; }
        public DateTime? Sampled_Date { get; set; }
        public DateTime? Recd_Date { get; set; }
        public DateTime? First_Test_Date { get; set; }
        public DateTime? Date_Completed { get; set; }
        public string Oil_Type { get; set; }
        public string Spec { get; set; }
        public bool Out_Of_Spec { get; set; }
        public MOO.Plant Plant { get; set; }
        public string Area { get; set; }
        public string Equipment { get; set; }
        public string Sample_Point { get; set; }
        public DateTime? Date_Authorised { get; set; }
        public string Authoriser { get; set; } = "";

        public double? Glycol { get; set; }
        public bool? Glycol_OOS { get; set; }
        public string Glycol_Range { get; set; }
        public double? Soot_3800 { get; set; }
        public bool? Soot_3800_OOS { get; set; }
        public string Soot_3800_Range { get; set; }
        public double? Soot_1980 { get; set; }
        public bool? Soot_1980_OOS { get; set; }
        public string Soot_1980_Range { get; set; }
        public double? Oxidation { get; set; }
        public bool? Oxidation_OOS { get; set; }
        public string Oxidation_Range { get; set; }
        public double? Water { get; set; }
        public bool? Water_OOS { get; set; }
        public string Water_Range { get; set; }
        public double? Cu { get; set; }
        public bool? Cu_OOS { get; set; }
        public string Cu_Range { get; set; }
        public double? Fe { get; set; }
        public bool? Fe_OOS { get; set; }
        public string Fe_Range { get; set; }
        public double Cr { get; set; }
        public bool? Cr_OOS { get; set; }
        public string Cr_Range { get; set; }
        public double? Al { get; set; }
        public bool? Al_OOS { get; set; }
        public string Al_Range { get; set; }
        public double? Si { get; set; }
        public bool? Si_OOS { get; set; }
        public string Si_Range { get; set; }
        public double? Pb { get; set; }
        public bool? Pb_OOS { get; set; }
        public string Pb_Range { get; set; }
        public double? Ca { get; set; }
        public bool? Ca_OOS { get; set; }
        public string Ca_Range { get; set; }
        public double? Mg { get; set; }
        public bool? Mg_OOS { get; set; }
        public string Mg_Range { get; set; }
        public double? Zn { get; set; }
        public bool? Zn_OOS { get; set; }
        public string Zn_Range { get; set; }
        public double? P { get; set; }
        public bool? P_OOS { get; set; }
        public string P_Range { get; set; }
        public double? Mo { get; set; }
        public bool? Mo_OOS { get; set; }
        public string Mo_Range { get; set; }
        public double? Sn { get; set; }
        public bool? Sn_OOS { get; set; }
        public string Sn_Range { get; set; }
        public double? K { get; set; }
        public bool? K_OOS { get; set; }
        public string K_Range { get; set; }
        public double? Na { get; set; }
        public bool? Na_OOS { get; set; }
        public string Na_Range { get; set; }
        public double? Ni { get; set; }
        public bool? Ni_OOS { get; set; }
        public string Ni_Range { get; set; }
        public double? Ag { get; set; }
        public bool? Ag_OOS { get; set; }
        public string Ag_Range { get; set; }
        public double? ISO4 { get; set; }
        public bool? ISO4_OOS { get; set; }
        public string ISO4_Range { get; set; }
        public double? ISO6 { get; set; }
        public bool? ISO6_OOS { get; set; }
        public string ISO6_Range { get; set; }
        public double? ISO14 { get; set; }
        public bool? ISO14_OOS { get; set; }
        public string ISO14_Range { get; set; }
        public double? Viscosity { get; set; }
        public bool? Viscosity_OOS { get; set; }
        public string Viscosity_Range { get; set; }
        public double? PQ { get; set; }
        public bool? PQ_OOS { get; set; }
        public string PQ_Range { get; set; }
        public double? Glycol_GC { get; set; }
        public bool? Glycol_GC_OOS { get; set; }
        public string Glycol_GC_Range { get; set; }
        public double? Diesel { get; set; }
        public bool? Diesel_OOS { get; set; }
        public string Diesel_Range { get; set; }
        public decimal Equip_LTD { get; set; }
        public decimal Component_LTD { get; set; }
        public string Equip_Type { get; set; }
        public string Lube_Pnt_Type { get; set; }

        public string Equip_Id { get; set; }
        public int? Component_Id { get; set; }
        public byte? Unit_Type { get; set; }
        public DateTime? Transfer_Date { get; set; }
        public DateTime? Collected_Date { get; set; }
    }
}
