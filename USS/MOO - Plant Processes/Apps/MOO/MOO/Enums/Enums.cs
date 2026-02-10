using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO
{
    /// <summary>
    /// Enum for the plant
    /// </summary>
    public enum Plant
    {
        /// <summary>
        /// Minntac
        /// </summary>
        [Description("Minntac")]
        Minntac,
        /// <summary>
        /// Keetac
        /// </summary>
        [Description("Keetac")]
        Keetac
    }
    /// <summary>
    /// Plant areas
    /// </summary>
    public enum Area
    {
        /// <summary>
        /// Drilling Crew
        /// </summary>
        Drilling,
        /// <summary>
        /// Mine or Pit
        /// </summary>
        Pit,
        /// <summary>
        /// Crusher
        /// </summary>
        Crusher,
        /// <summary>
        /// Concentrator
        /// </summary>
        Concentrator,
        /// <summary>
        /// Agglomerator
        /// </summary>
        Agglomerator
    }

    /// <summary>
    /// Minntac only areas
    /// </summary>
    public enum MinntacAreas
    {
        /// <summary>
        /// agglomerator step 2
        /// </summary>
        [Display(Name = "Agg Step 2")]
        AggStep2,

        /// <summary>
        /// Agglomerator step 3
        /// </summary>
        [Display(Name = "Agg Step 3")]
        AggStep3,

        /// <summary>
        /// Concentrator
        /// </summary>
        [Display(Name = "Concentrator")]
        Conc,

        /// <summary>
        /// Agglomerator step 3
        /// </summary>
        [Display(Name = "Crusher")]
        Crusher,

        /// <summary>
        /// Agglomerator step 3
        /// </summary>
        [Display(Name = "Mine")]
        Mine,

    }
}
