using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ERP.Enums
{
    /// <summary>
    /// Enum for the Process Code Field in the MPA Record of the ERP Message
    /// </summary>
    public enum ProcessCode
    {
        /// <summary>
        /// Agglomerating
        /// </summary>
        XAG,
        /// <summary>
        /// Concentrating
        /// </summary>
        XCN,
        /// <summary>
        /// Crushing
        /// </summary>
        XCR
    }
}
