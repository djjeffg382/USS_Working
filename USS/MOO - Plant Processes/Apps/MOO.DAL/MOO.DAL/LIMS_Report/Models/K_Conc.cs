using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.LIMS_Report.Models
{
    public class K_Conc
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
        public string Authoriser { get; set; }
        public DateTime Shift_Date8 { get; set; }
        public byte Shift_Nbr8 { get; set; }
        public DateTime Shift_Date12 { get; set; }
        public byte Shift_Nbr12 { get; set; }
        public decimal? Fe { get; set; }
        public decimal? SiO2 { get; set; }
        public decimal? Al2O3 { get; set; }
        public decimal? CaO { get; set; }
        public decimal? MgO { get; set; }
        public decimal? Mn { get; set; }
        public decimal? P2O5 { get; set; }
        public decimal? TiO2 { get; set; }
        public decimal? Total { get; set; }
        public decimal? Fe2O3 { get; set; }
        public decimal? MnO { get; set; }

    }
}
