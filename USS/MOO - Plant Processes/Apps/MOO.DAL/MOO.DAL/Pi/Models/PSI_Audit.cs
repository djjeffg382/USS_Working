using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Pi.Models
{
    public class PSI_Audit
    {
        /// <summary>
        /// Datetime of the manually entered Lab Data
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Lab determined PSI value.  This is manually entered by the lab
        /// </summary>
        public double Lab { get; set; }

        /// <summary>
        /// Datetime of the PSI Value
        /// </summary>
        public DateTime PsiDate { get; set; }

        /// <summary>
        /// PSI Process value obtained from the process control system
        /// </summary>
        public double Psi { get; set; }

        /// <summary>
        /// Solids value obtained from the process control system
        /// </summary>
        public double Solids { get; set; }

        /// <summary>
        /// The Concentrator Line Number
        /// </summary>
        public byte Line { get; set; }
    }
}
