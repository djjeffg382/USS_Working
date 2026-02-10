using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.LIMS.Models
{
    public class WaterResults
    {
        public long Sample_Nbr { get; set; }
        public DateTime Sample_Date { get; set; }
        public DateTime Login_Date { get; set; }
        public DateTime? Recd_Date { get; set; }
        public string SamplePoint { get; set; }

        public double? Ca { get; set; }
        public double? Mg { get; set; }
        public double? Na { get; set; }
        public double? K { get; set; }
        public double? Fe { get; set; }
        public double? Bromide { get; set; }
        public double? Chloride { get; set; }
        public double? Fluoride { get; set; }
        public double? Nitrate { get; set; }
        public double? Nitrite { get; set; }
        public double? Phosphate { get; set; }
        public double? Sulfate { get; set; }
        public double? PH { get; set; }
        public double? Alkalinity { get; set; }
        public double? Conductivity { get; set; }
    }
}
