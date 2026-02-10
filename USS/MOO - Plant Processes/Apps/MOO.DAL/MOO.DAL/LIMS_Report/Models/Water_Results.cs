using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.LIMS_Report.Models
{
    public class Water_Results
    {
        public long Sample_Nbr { get; set; }
        public string Status { get; set; }
        public DateTime Login_Date { get; set; }
        public DateTime? Sampled_Date { get; set; }
        public DateTime? Recd_Date { get; set; }
        public DateTime? First_Test_Date { get; set; }
        public DateTime? Date_Completed { get; set; }
        public string Sample_Point { get; set; }
        public DateTime? Date_Authorised { get; set; }
        public string Authoriser { get; set; } = "";

        public double? Alkalinity { get; set; }
        public double? Conductivity { get; set; }
        public double? Fluoride { get; set; }
        public double? Chloride { get; set; }
        public double? Nitrite { get; set; }
        public double? Bromide { get; set; }
        public double? Nitrate { get; set; }
        public double? Phosphate { get; set; }
        public double? Sulfate { get; set; }
        public double? Dilution { get; set; }
        public double? Fe { get; set; }
        public double? Ca { get; set; }
        public double? Mg { get; set; }
        public double? K { get; set; }
        public double? Na { get; set; }
        public double? Ph { get; set; }
    }
}
