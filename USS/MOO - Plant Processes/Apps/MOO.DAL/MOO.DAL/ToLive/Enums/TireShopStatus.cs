using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Enums
{
    public enum TireShopStatus
    {
        [Display(Name = "Repair")]
        Repair,
        
        [Display(Name = "Junk")]
        Junk,

        [Display(Name = "On Hold")]
        On_Hold,
    }
}
