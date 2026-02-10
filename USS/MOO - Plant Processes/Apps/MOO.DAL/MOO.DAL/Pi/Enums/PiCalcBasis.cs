using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Pi.Enums
{
    public enum PiCalcBasis
    {
        /// <summary>
        /// Values are weighted by time over which they apply.  Interpolation is based on the point "step" attribute. Boundary values are interpolated.
        /// </summary>
        TimeWeighted,
        /// <summary>
        /// Each value is weighted. For one interval, both boundary values are used. For multiple intervals, values at start time are used.
        /// </summary>
        EventWeighted,
        /// <summary>
        /// Values are weighted the same way as in TimeWeighted. The linear interpolation is used.
        /// </summary>
        TimeWeightedContinuous,
        /// <summary>
        /// Values are weighted the same way as in TimeWeighted. The "stair step plot" interpolation is used.
        /// </summary>
        TimeWeightedDiscrete,
        /// <summary>
        /// Values are weighted the same way as in EventWeighted. For multiple intervals, the most recent event is excluded.
        /// </summary>
        EventWeightedExcludeMostRecentEvent,
        /// <summary>
        /// Values are weighted the same way as in EventWeighted. For multiple intervals, the earliest event is excluded.
        /// </summary>
        EventWeightedExcludeEarliestEvent,
        /// <summary>
        /// Values are weighted the same way as in EventWeighted. Both boundaries are always included.
        /// </summary>
        EventWeightedIncludeBothEnds
    }
}
