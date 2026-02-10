using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Corporate_Delay_Types
    {
        public string Delay_Type_Id { get; set; }
        public string Delay_Description { get; set; }
        public string Delay_Grouping { get; set; }
        public MOO.Plant Plant { get; set; }
        /// <summary>
        /// ID referencing the Corp Delay Tracking Area ID
        /// </summary>
        public decimal? Area { get; set; }
        /// <summary>
        /// ID referencing the Corp Delay Tracking Operating Unit ID
        /// </summary>
        public decimal? Operating_Unit { get; set; }
        /// <summary>
        /// ID referencing the Corp Delay Tracking Equipment Category ID
        /// </summary>
        public decimal? Eqp_Category { get; set; }
        /// <summary>
        /// ID referencing the Corp Delay Tracking Equipment Op Sys ID
        /// </summary>
        public decimal? Op_Sys { get; set; }
        /// <summary>
        /// ID referencing the Corp Delay Tracking Equipment ID
        /// </summary>
        public decimal? Equipment { get; set; }
        /// <summary>
        /// ID referencing the Corp Delay Tracking Reason ID
        /// </summary>
        public decimal? Reason { get; set; }
        /// <summary>
        /// ID referencing the Corp Delay Tracking Detail ID
        /// </summary>
        public decimal? Detail { get; set; }
        /// <summary>
        /// Minimum number of minutes needed for delay to send to Corporate Delay Tracking
        /// </summary>
        public decimal Timed_Delay_Minutes { get; set; }
        /// <summary>
        /// ID referencing the Corp Delay Tracking Facility ID
        /// </summary>
        public decimal? Facility { get; set; }
        /// <summary>
        /// ID Referencing the Operating Unit if the equipment is a sub equipment in the Operating Unit Field (shows indented under another equipment)
        /// </summary>
        public decimal? Equipment_Affected { get; set; }
        /// <summary>
        /// ID referencing the Corp Delay Tracking Component ID
        /// </summary>
        public decimal? Component { get; set; }
        public bool Mark_Complete { get; set; }
        /// <summary>
        /// Flag for determining if the Dealy.EndOfShift job should split the delay
        /// </summary>
        public bool Proc_End_Of_Shift { get; set; }
        /// <summary>
        /// Flag for determining if splitting delay at shift change should contain a one second difference between end of first delay to begin of second delay
        /// </summary>
        public bool Split_New_Shift { get; set; }
        /// <summary>
        /// Whether this delay is active.  Inactive delays will not be recorded when calling DELAY.M or DELAY.K
        /// </summary>
        public bool Delay_Active { get; set; }

    }
}
