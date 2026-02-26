using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Pi.Models
{

    /// <summary>
    /// Used by the Jobs/MTC_Aggl_Scale_monitor and also available for viewing in the bzr/MTC_Agglomerator
    /// </summary>
    public sealed class MtcAggScaleMonitor
    {

        /// <summary>
        /// Title/name of Balling drum
        /// </summary>
        public string BallingDrum { get; set; }

        /// <summary>
        /// Tag for the 027 Belt motor contact
        /// </summary>
        public string Mcx027Tag { get; set; }

        /// <summary>
        /// Tag for the 030 belt motor contact
        /// </summary>
        public string Mcx030Tag { get; set; }

        /// <summary>
        /// Tag for the 027 belt setpoint
        /// </summary>
        public string Sp027Tag { get; set; }

        /// <summary>
        /// Analog tag for the 027 belt
        /// </summary>
        public string Ai027Tag { get; set; }

        /// <summary>
        /// Analog tag for the 030 belt
        /// </summary>
        public string Ai030Tag { get; set; }

        /// <summary>
        /// Pulse tag for the 030 belt
        /// </summary>
        public string Pa030Tag { get; set; }

        /// <summary>
        /// Average of the 27 belt analog values over a Good reading defined by value above setpoint for specified period
        /// </summary>
        public double Ana027Avg { get; set; }

        /// <summary>
        /// Average of the 30 belt analog values over a Good reading defined by value above setpoint for specified period
        /// </summary>
        public double Ana030Avg { get; set; }

        /// <summary>
        /// Average of the 30 belt pulse values over a Good reading defined by value above setpoint for specified period
        /// </summary>
        public double Pulse030Avg { get; set; }


        /// <summary>
        /// percentage difference between the analog 027 belt and analog 030 belt
        /// </summary>
        public double Ana027To030Pct
        {
            get
            {
                if (Ana027Avg > 0)
                    return Math.Round((Ana027Avg - Ana030Avg) / Ana027Avg * 100, 2, MidpointRounding.AwayFromZero);
                else
                    return 0;
            }
        }

        /// <summary>
        /// percentage difference between the analog 030 belt and the pulse 030 belt
        /// </summary>
        public double Pulse030ToAna030Pct
        {
            get
            {
                if (Ana030Avg > 0)
                    return Math.Round((Ana030Avg - Pulse030Avg) / Ana030Avg * 100, 2, MidpointRounding.AwayFromZero);
                else
                    return 0;
            }
        }
    }
}
