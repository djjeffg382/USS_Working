using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Analytics
    {
        public MOO.Plant Plant { get; set; }
        public string Area { get; set; }
        public string Data_Group { get; set; }
        public short Line_Nbr { get; set; }
        public string Label { get; set; }
        public DateTime Date_Val { get; set; }
        public string Value { get; set; }
        public double ValueDbl { get
            {
                if(Value.Trim().ToLower() != "nan" && double.TryParse(Value, out double newVal))
                    return newVal;
                return 0;
            }
        }
        public short Sort_Order { get; set; }
        public string Quality { get; set; }

    }
}
