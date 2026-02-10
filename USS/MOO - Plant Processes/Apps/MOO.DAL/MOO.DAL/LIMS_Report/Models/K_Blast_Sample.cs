using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.LIMS_Report.Models
{
    public class K_Blast_Sample
    {
        public long Sample_Nbr { get; set; }
        public long Parent_Sample_Nbr { get; set; }
        public string Pattern_Number { get; set; } = "";
        public string Hole_Number { get; set; } = "";
        public string Preparation { get; set; } = "";

        public string Status { get; set; }
        public DateTime Login_Date { get; set; }
        public DateTime? Sampled_Date { get; set; }
        public DateTime? Recd_Date { get; set; }
        public DateTime? First_Test_Date { get; set; }
        public DateTime? Date_Completed { get; set; }
        public string Sample_Point { get; set; }
        public DateTime? Date_Authorised { get; set; }
        public string Authoriser { get; set; }


        public decimal ScreenMinus325Mesh { get; set; }
        public decimal FeCalc { get; set; }
        public decimal SiO2 { get; set; }
        public decimal SiO2Calc { get; set; }
        public decimal AL2O3 { get; set; }
        public decimal CaO { get; set; }
        public decimal CaOCalc { get; set; }
        public decimal MgO { get; set; }
        public decimal P2O5 { get; set; }
        public decimal TiO2 { get; set; }
        public decimal Fe2O3 { get; set; }
        public decimal Mn3O4 { get; set; }
        public decimal LOI_Balance { get; set; }
        public decimal Na2O { get; set; }
        public decimal K2O { get; set; }
        public decimal SO3 { get; set; }
        public decimal V2O5 { get; set; }
        public decimal Cr2O3 { get; set; }
        public decimal SrO { get; set; }
        public decimal ZrO2 { get; set; }
        public decimal BaO { get; set; }
        public decimal NiO { get; set; }
        public decimal CuO { get; set; }
        public decimal ZnO { get; set; }
        public decimal PbO { get; set; }
        public decimal HfO2 { get; set; }
        public decimal DT_WgtRecovery { get; set; }
        public decimal Fe2Plus { get; set; }

    }
}
