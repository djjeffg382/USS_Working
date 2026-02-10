using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Coarse_Tails_Analysis
    {
        public DateTime Sdate { get; set; }
        public short Line { get; set; }
        public short Intvl { get; set; }
        public decimal Fe { get; set; }
        public DateTime Sample_Date { get
            {
                return Sdate.AddMinutes(-90).AddHours(Intvl);
            }
        }

    }
}
