using MOO.Enums.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Conc_Analytics_Uplift
    {
        public enum Level
        {
            Line,
            Plant
        }

        public enum UpliftFrequency
        {

            //Use the prompt parameter to match the value in the database
            //we can then use this in the GetAll service function
            [Display(Name = "Crt", Description = "Current", Prompt ="Current")]
            Current,
            [Display(Name ="Shft", Description ="Shift", Prompt ="Shift")]
            Shift,
            [Display(Name = "Day", Description = "Today", Prompt = "Today")]
            Today,
            [Display(Name = "7Dy", Description = "7 Days", Prompt = "7_Days")]
            Days_7,
            [Display(Name = "30Dy", Description = "30 Days", Prompt = "30_Days")]
            Days_30
        }

        public DateTime TheDate { get; set; }
        public Level PlantLevel { get; set; }
        public byte Line { get; set; }
        public double Uplift_Long_Tons { get; set; }
        public double Uplift_Pct { get; set; }
        public UpliftFrequency Frequency { get; set; }
        public  string FrequencyStr { get{ return Frequency.GetDisplay().Name; } }

    }
}
