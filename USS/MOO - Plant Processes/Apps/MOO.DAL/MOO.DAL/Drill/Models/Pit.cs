using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Drill.Models
{
    public enum Pit
    {

        [Description("Mtc East Pit")]
        MTC_East_Pit,

        [Description("Mtc West Pit")]
        MTC_West_Pit,


        [Description("Keetac")]
        Keetac,

        [Description("Not Set")]
        Not_Set
    }
}
