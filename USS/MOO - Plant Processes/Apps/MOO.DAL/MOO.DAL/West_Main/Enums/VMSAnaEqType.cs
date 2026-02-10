using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.West_Main.Enums
{
    /// <summary>
    /// VMS Analog Equation Type
    /// </summary>
    public enum VMSAnaEqType
    {
        [Display(Name = "None")]
        None = 0,

        [Display(Name = "Operating Time")]
        Operating_Time = 1,

        [Display(Name = "Summation")]
        Summation = 2,

        [Display(Name = "Point * Constant")]
        Point_Times_Constant = 3,

        [Display(Name = "Point/Constant")]
        Point_DividedBy_Point = 4,

        [Display(Name = "Point * Point")]
        Point_Times_Point = 5,

        [Display(Name = "Consecutive Sum")]
        Consecutive_Sum = 6,


        [Display(Name = "Average Of Sum")]
        Average_Of_Sum = 7,


        [Display(Name = "Accumulate Only")]
        Accumulate_Only = 8,

        [Display(Name = "Other????")]
        Other = 9  //Not sure what this is yet?
    }
}
