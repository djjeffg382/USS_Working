using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ERP.Enums
{
    /// <summary>
    /// Enum for the Material Name field in the MPA Record of the ERP Message
    /// </summary>
    /// <remarks>Use Display.Name for the string value of what will be sent to ERP</remarks>
    public enum Material
    {
        [Display(Name = "Concentrate")]
        Concentrate,

        [Display(Name = "FPellet")]
        FPellet,

        [Display(Name = "APellet")]
        APellet,

        [Display(Name = "K1")]
        K1,

        [Display(Name = "CrushRock")]
        CrushRock,

        [Display(Name = "Taconite")]
        Taconite,

        [Display(Name = "Limestone")]
        Limestone,

        [Display(Name = "DR Concentrate")]
        DRCon,

        [Display(Name = "DR Pellet")]
        DRPel
    }
}
