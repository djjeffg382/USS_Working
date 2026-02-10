using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Dust_Coll_Epa_Reading
    {
        public string Equip_No { get; set; }
        public DateTime Virtual_Read_Date { get; set; }
        public DateTime? Pressure_Date { get; set; }
        public double? Pressure_Val { get; set; }
        public DateTime? Flow_Date { get; set; }
        public double? Flow_Val { get; set; }
        public string Reading_Ind { get; set; }
        public string Processed_Ind { get; set; }

    }
}
