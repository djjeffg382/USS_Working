using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Core.Enums
{
    /// <summary>
    /// Keetac Pellet type Metric ID 1126
    /// </summary>
    /// <remarks>Metric ID should only have a value of 1 or 2.  0 will be used in the K_PELL_TD and K_PELL_IND_LINE_TD tables as a combined total of the 2</remarks>
    public enum KTCPelletType
    {
        /// <summary>
        /// The combined total of all pellet types
        /// </summary>
        Combined = 0,
        /// <summary>
        /// Blast Furnace (K1 Pellet)
        /// </summary>
        BlastFurnace = 1,
        /// <summary>
        /// Direct Reduced Pellet
        /// </summary>
        DRPellet = 2
    }
}
