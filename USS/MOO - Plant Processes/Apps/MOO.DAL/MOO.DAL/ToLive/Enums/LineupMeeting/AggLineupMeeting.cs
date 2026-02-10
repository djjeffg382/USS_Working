using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Enums.LineupMeeting
{
    public enum AggLineupMeeting
    {
        Safety,
        Environmental,
        ResourceNeeds,
        Quality,
        Recognition,
        Cost,

        OpS2VacFilterRatio,
        OpS3VacFilterRatio,

        [Obsolete("Removed Nov. 2025")]
        OpS2Inventory,
        [Obsolete("Removed Nov. 2025")]
        OpS3Inventory,
        OpS2PreheatBurner,
        OpS3PreheatBurner,
        OpS2Water,
        OpS3Water,
        OpS2Reclaim,
        OpS3Reclaim,
        OpS2Flopgates,
        OpS3Flopgates,
        OpS2EmergencyWO,
        OpS3EmergencyWO,
        OpS2WREntered,
        OpS3WREntered,
        OpS2Ocs,
        OpS3Ocs,


        MaintS2VacAvail,
        MaintS3VacAvail,
        MaintS2FilterAvail,
        MaintS3FilterAvail,
        MaintS2Reclaim,
        MaintS3Reclaim,
        MaintS2FlopgateWO,
        MaintS3FlopgateWO,
        MaintS2WOReturned,
        MaintS3WOReturned,
        MaintS2PMComplete,
        MaintS3PMComplete,

        [Obsolete("Removed Oct. 2025")]
        MaintMajorMinorBasement,

        [Obsolete("Removed Oct. 2025")]
        MaintMajorMinorHotSide,

        [Obsolete("Removed Oct. 2025")]
        MaintMajorMinorColdSide,



        PlanningSAMP,
        PlanningCapital,
        PlanningOutages,

        /// <summary>
        /// Maintenance Issues Step 1-2
        /// </summary>
        MaintIssueS12,

        /// <summary>
        /// Maintenance Issues Step 3
        /// </summary>
        MaintIssueS3

    }
}
