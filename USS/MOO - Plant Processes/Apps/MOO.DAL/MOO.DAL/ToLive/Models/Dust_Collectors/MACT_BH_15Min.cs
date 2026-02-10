using MOO.DAL.ToLive.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class MACT_BH_15Min
    {
        public string Equip_No { get; set; }
        public int? Read_No { get; set; }
        public DateTime Read_Date { get; set; }
        public decimal? BH_Val { get; set; }
        public DustCollReadIndicator Read_Ind { get; set; }
    }
}
