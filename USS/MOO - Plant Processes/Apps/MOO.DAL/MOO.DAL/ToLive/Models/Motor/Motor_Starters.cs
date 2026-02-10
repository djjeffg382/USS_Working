using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Motor_Starters : Motor_Equipment
    {
        public enum StarterType
        {
            Center_Line,
            Limit_Amp
        }

        public string Notes { get; set; }
        public StarterType Starter_Type { get; set; }
        public int Current_Rating { get; set; }
    }
}
