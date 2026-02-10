using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Enums
{
    public enum EqUnitAssocSource
    {
        [Description("Inactive")]
        Inactive,


        [Description("MinVu")]
        MinVu,


        [Description("MinVu MPA")]
        MinVu_MPA,


        [Description("VisionLink")]
        VisionLink,


        [Description("iFix")]
        iFix,


        [Description("Meter Hours Insert")]
        Meter_Hours_Insert,


        [Description("Manual Data Entry")]
        Data_Entry,


        [Description("PI")]
        PI,


        [Description("KTC MinVu Crusher")]
        MinVu_Crusher,


        [Description("MineStar Health")]
        MineStar,

        [Description("Epiroc")]
        Epiroc
    }
}
