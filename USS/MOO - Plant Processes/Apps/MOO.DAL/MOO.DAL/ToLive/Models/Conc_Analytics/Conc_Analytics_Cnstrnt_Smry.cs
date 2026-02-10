using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Conc_Analytics_Cnstrnt_Smry
    {
        public DateTime Date_Val { get; set; }
        public decimal Line_Nbr { get; set; }
        public string Label { get; set; }
        public decimal? ActiveCount { get; set; }
        public decimal? InactiveCount { get; set; }
        public string Frequency { get; set; }

        public decimal ConstraintPct
        {
            get
            {
                if (InactiveCount.HasValue && ActiveCount.HasValue && (InactiveCount.Value + ActiveCount.Value) > 0)
                    return Math.Round((ActiveCount.Value / (ActiveCount.Value + InactiveCount.Value)) * 100, 2, MidpointRounding.AwayFromZero);
                else
                    return 0;
            }
        }

    }
}
