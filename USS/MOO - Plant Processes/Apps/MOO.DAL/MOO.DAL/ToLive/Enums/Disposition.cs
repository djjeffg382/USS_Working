using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Enums
{
    public enum Disposition
    {
        [Display(Name ="Spare")]
        Spare,

        [Display(Name ="Inspection")]
        Inspection,
    }
}
