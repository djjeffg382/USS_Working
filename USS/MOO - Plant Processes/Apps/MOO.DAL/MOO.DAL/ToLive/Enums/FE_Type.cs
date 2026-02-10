using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Enums
{
    public enum FE_Type
    {
        [Display(Name = "FE KTC Mine")] FE_KTC_Mine = 1,
        [Display(Name = "FE KTC Tails Basin")] FE_KTC_Tails_Basin = 2,
    }
}
