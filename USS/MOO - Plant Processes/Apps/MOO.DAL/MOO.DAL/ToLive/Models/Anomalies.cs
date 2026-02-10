using MOO.DAL.ToLive.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Anomalies
    {
        public MOO.Plant Plant { get; set; }
        public string Location { get; set; }
        public short Line { get; set; }
        public string Sensor { get; set; }
        public DateTime Tag_Time { get; set; }
        public AnomalyFlag Flag { get; set; }

    }
}
