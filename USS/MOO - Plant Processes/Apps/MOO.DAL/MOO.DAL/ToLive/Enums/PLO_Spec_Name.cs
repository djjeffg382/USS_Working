using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MOO.DAL.ToLive.Enums
{
    /// <summary>
    /// We will use display attribute to add some extra information about these
    /// </summary>
    /// <remarks>Use MOO.Enums.EnumExtension to help pull this information out</remarks>
    public enum PLO_Spec_Name
    {

        [Display(Name="Fe",GroupName ="Chemistry",Description ="Fe",Order = 1)]
        Fe,

        [Display(Name = "Fe++", GroupName = "Chemistry", Description = "Fe++", Order = 2)]
        FePlusPlus,


        [Display(Name = "SiO2", GroupName = "Chemistry", Description = "SiO2", Order = 3)]
        SiO2,

        [Display(Name = "Al2O3", GroupName = "Chemistry", Description = "Al2O3", Order = 4)]
        Al2O3,

        [Display(Name = "SiO2 + Al2O3", GroupName = "Chemistry", Description = "SiO2 + Al2O3", Order = 5)]
        SiPlusAl,

        [Display(Name = "CaO/SiO2", GroupName = "Chemistry", Description = "CaO/SiO2", Order = 6)]
        CaDivSi,

        [Display(Name = "CaO", GroupName = "Chemistry", Description = "CaO", Order = 7)]
        CaO,

        [Display(Name = "MgO", GroupName = "Chemistry", Description = "MgO", Order = 8)]
        MgO,

        [Display(Name = "CaO + MgO", GroupName = "Chemistry", Description = "CaO + MgO", Order =9)]
        CaOPlusMgO,

        [Display(Name = "B/A", GroupName = "Chemistry", Description = "B/A", Order = 10)]
        BA,

        [Display(Name = "Mn", GroupName = "Chemistry", Description = "Mn", Order = 11)]
        Mn,

        [Display(Name = "P", GroupName = "Chemistry", Description = "Phos", Order = 12)]
        P,

        [Display(Name = "S", GroupName = "Chemistry", Description = "Sulfur", Order = 13)]
        S,

        [Display(Name = "Na2O", GroupName = "Chemistry", Description = "Na2O", Order = 14)]
        Na2O,

        [Display(Name = "K2O", GroupName = "Chemistry", Description = "K2O", Order = 15)]
        K2O,

        [Display(Name = "H2O", GroupName = "Chemistry", Description = "Moisture", Order = 16)]
        H2O,

        [Display(Name = "BT 1/2 \"", GroupName = "Physical", Description = "%+12.5mm (+1/2\")", Order = 17)]
        BTPlus12,

        [Display(Name = "BT (-1/2\" +3/8\")", GroupName = "Physical", Description = "%-12.5mm + 9.5mm (-1/2\" + 3/8\")", Order = 18)]
        BTMinus12Plus38,

        [Display(Name = "BT 3/8 \"", GroupName = "Physical", Description = "%+9.5mm (+3/8\")", Order = 19)]
        BTPlus38,

        [Display(Name = "BT 1/4 \"", GroupName = "Physical", Description = "%+6.3mm (+1/4\")", Order = 20)]
        BTPlus14,

        [Display(Name = "BT -1/4 \"", GroupName = "Physical", Description = "%-6.3mm (-1/4\")", Order = 21)]
        BTMinus14,

        [Display(Name = "AT 1/4 \"", GroupName = "Physical", Description = "Tumble %+6.3mm (+1/4\")", Order = 22)]
        ATPlus14,

        [Display(Name = "AT -28M", GroupName = "Physical", Description = "Tumble %-0.6mm (-28M)", Order = 23)]
        ATMinus28,

        [Display(Name = "Avg Comp Lb", GroupName = "Physical", Description = "Average Compression Lb", Order = 24)]
        AvgCompLb,

        [Display(Name = "% -200", GroupName = "Physical", Description = "%-90kg (200 lb)", Order = 25)]
        Comp200,

        [Display(Name = "% -300", GroupName = "Physical", Description = "%-130kg (300 lb)", Order = 26)]
        Comp300,

        [Display(Name = "LTD", GroupName = "Metalurgical", Description = "LTD W/2%H2 Static %+6.30mm", Order = 27)]
        Ltd,

        [Display(Name = "Reducibility", GroupName = "Metalurgical", Description = "Reducibility %/Minute", Order = 28)]
        Reducibility,

        [Display(Name = "Linder Metallization", GroupName = "Metalurgical", Description = "Linder Metallization", Order = 29)]
        LinderMetallization,

        [Display(Name = "ISO Clustering Index", GroupName = "Metalurgical", Description = "ISO Clustering Index", Order = 30)]
        ISOClusteringIdx

    }
}
