using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Drill.Models
{

    public enum Redrill_Type
    {
        /// <summary>
        /// This hole is not a redrill and there are no redrills of this hole
        /// </summary>
        [Display(Name = "No Redrill")]
        No_Redrill = 0,
        //This hole is a redrill or is part of a list of redrills
        Redrill = 1,
        /// <summary>
        /// This hole is the final drilling of the hole
        /// </summary>
        [Display(Name = "Final Redrill")]
        Final_Redrill = 2
    }
}
