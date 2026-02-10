using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Mine_Production
    {
        /// <summary>
        /// class that will be used to get pulse/analog points for a given belt from the PI System
        /// </summary>
        public sealed class Belt_Pi_Points
        {
            public double? Pulse_Total { get; set; }
            public double? Analog_Total { get; set; }
        }

        /// <summary>
        /// Crusher 001 belts 
        /// </summary>
        public enum CrusherBelts
        {
            Belt_01,
            Belt_02,
            Belt_03
        }

        public DateTime Shift_Date { get; set; }
        public short Shift { get; set; }
        public decimal? Production_Tons { get; set; }
        public DateTime? Modified_Date { get; set; }
        public string Modified_By { get; set; }



    }
   
}
