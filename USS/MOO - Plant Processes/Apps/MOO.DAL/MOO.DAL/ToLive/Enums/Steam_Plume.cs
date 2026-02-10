using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Enums
{
    public enum Steam_Plume
    {
        [Display(Name = "None")] None = 1,
        [Display(Name = "Detached")] Detached = 2,
        [Display(Name = "Attached")] Attached = 3,
    }
}
