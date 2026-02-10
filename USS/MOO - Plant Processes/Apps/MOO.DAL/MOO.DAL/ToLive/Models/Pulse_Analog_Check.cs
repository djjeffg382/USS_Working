using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Pulse_Analog_Check
    {

        public enum PACheckArea
        {
            Crusher,
            Conc,
            Agg2,
            Agg3
        }

        public bool Active { get; set; }
        public string Pulse_Point { get; set; }
        public decimal Pulse_Multiplier { get; set; }
        public string Analog_Point { get; set; }
        public decimal Analog_Multiplier { get; set; }
        public string Email_List { get; set; }
        public PACheckArea Area { get; set; }
        public decimal Percent_Diff { get; set; }
        public decimal Minimum_Value { get; set; }
        public string Description { get; set; }
        public bool Notify { get; set; }


        //following 2 items are not in the table but will be filled for display purposes
        internal Pi.Models.PiTotal PiPulseValue { get; set; }
        internal Pi.Models.PiTotal PiAnalogValue { get; set; }

        public decimal PulseVal
        {
            get
            {
                if (PiPulseValue != null)
                    return Math.Round((decimal)PiPulseValue.Value.GetValueOrDefault(0) * Pulse_Multiplier,4);
                else
                    return 0;
            }
        }
        public decimal AnalogVal
        {
            get
            {
                if (PiAnalogValue != null)
                    return Math.Round((decimal)PiAnalogValue.Value.GetValueOrDefault(0) * Analog_Multiplier,4);
                else
                    return 0;
            }
        }

        public decimal? ValueDifference
        {
            get
            {

                //figure out denominator take the larger of the 2 values
                decimal denominator = Math.Max(AnalogVal, PulseVal);
                decimal numerator = Math.Abs((AnalogVal - PulseVal));
                if (denominator != 0)
                    return Math.Round((numerator / denominator) * 100, 4);
                else
                    return 0;

            }
        }

    }
}
