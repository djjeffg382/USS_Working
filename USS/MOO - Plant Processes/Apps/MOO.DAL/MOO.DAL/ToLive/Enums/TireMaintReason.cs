using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Enums
{
    public enum TireMaintReason
    {
        //a
        [Display(Name = "Worn Out")]
        Worn_Out = 1,
        //b
        [Display(Name = "Impact/ Cut in Face")]
        Impact = 2,
        //c
        [Display(Name = "O Ring")]
        O_Ring = 3,
        //d
        [Display(Name = "Cut/ Separation")]
        Cut_Separation = 4,
        //e
        [Display(Name = "Out of Round, Run Out, Balance")]
        Out_Of_Round = 5,
        //f
        [Display(Name = "Cupping")]
        Cupping = 6,
        //g
        [Display(Name = "Damaged Valve")]
        Damaged_Valve = 7,
        //h
        [Display(Name = "Matching")]
        Matching = 8,
        //i
        [Display(Name = "Broken Wheel Flange/ Cracked Rim")]
        Broken_Wheel_Flange = 9,
        //j
        [Display(Name = "Tread Separation")]
        Tread_Separation = 10,
        //k
        [Display(Name = "Foreign Object Puncture")]
        Puncture = 11,
        //l
        [Display(Name = "Mechanical Separation")]
        Mechanical_Separation = 12,
        //m
        [Display(Name = "Loose Wheel Nuts")]
        Loose_Wheel_Nuts = 13,
        //n
        [Display(Name = "Accidental Damage")]
        Accidental_Damage = 14,
        //o
        [Display(Name = "Lug Cracking")]
        Lug_Cracking = 15,
        //p
        [Display(Name = "Sidewall Damage Tire")]
        Sidewall_Damage_Tire = 16,
        //q
        [Display(Name = "Shoulder Wear")]
        Shoulder_Wear = 17,
        //r
        [Display(Name = "Tire Fatigue")]
        Tire_Fatigue = 18,
        //s
        [Display(Name = "Tread Chipping")]
        Tread_Chipping = 19,
        //t
        [Display(Name = "Radial Cracks")]
        Radial_Cracks = 20,
        //u
        [Display(Name = "Repair Failure")]
        Repair_Failure = 21,
        //v
        [Display(Name = "Bead Erosion")]
        Bead_Erosion = 22,
        //w
        [Display(Name = "Rotation/ High Hours")]
        Rotation_High_Hours = 23,
        //x
        [Display(Name = "Liner Failure")]
        Liner_Failure = 24,
        //y
        [Display(Name = "Heat Separation")]
        Heat_Separation = 25,
        //z
        [Display(Name = "Mechanical Maintenance")]
        Mechanical_Maint = 26,
    }
}
