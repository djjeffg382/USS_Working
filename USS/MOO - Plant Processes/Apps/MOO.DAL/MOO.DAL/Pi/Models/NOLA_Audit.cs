using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Pi.Models
{
    public class NOLA_Audit
    {
        public enum NOLA_Type
        {
            NOLA2_Step2,
            NOLA2_Step3,
            NOLA3_Step2,
            NOLA3_Step3,
            NOLA3_FF
        }

        public DateTime Time { get; set; }
        public double Lab { get; set; }
        public double NOLA { get; set; }
        public double Solids { get; set; }
        public NOLA_Type Type { get; set; }
    }
}
