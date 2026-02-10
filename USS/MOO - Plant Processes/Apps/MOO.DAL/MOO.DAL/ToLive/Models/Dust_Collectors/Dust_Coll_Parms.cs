using MOO.DAL.ToLive.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Dust_Coll_Parms
    {
        public string Equip_No { get; set; }
        public bool Pressure_Ind { get; set; }
        public bool Flow_Ind { get; set; }
        public double? Pressure_Lower_Limit { get; set; }
        public double? Pressure_Upper_Limit { get; set; }
        public double? Flow_Lower_Limit { get; set; }
        public double? Flow_Upper_Limit { get; set; }
        public DateTime? Last_Updated_Date { get; set; }
        public string Equip_Desc { get; set; }
        public string Area { get; set; }
        public string Sv_Nbr { get; set; }
        public int? Line { get; set; }
        public DustCollPressAmps Press_Or_Amps_Ind { get; set; }
        public bool Nsps_Boolean { get; set; }
        public int? Press_Id { get; set; }
        public int? Flow_Id { get; set; }
        public bool Mact_Ind { get; set; }
        public MOO.Plant Plant { get; set; }
        public string Flow_Tag { get; set; }
        public string Pressure_Tag { get; set; }
        public string DC_Tag { get; set; }
        public string EU_Tag1 { get; set; }
        public string EU_Tag2 { get; set; }
        public string EU_Tag3 { get; set; }
        public string EU_Tag4 { get; set; }
        public string Particulate_Tag { get; set; }
        public DustCollectorType? DC_Type { get; set; }
        public string DC_Comments { get; set; }
        public string Eu_Analog_Override { get; set; }
        public string Snapshot_Time { get; set; }
    }
}
