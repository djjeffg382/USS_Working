using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    /// <summary>
    /// Data model for data coming from the Cat Visionlink API
    /// </summary>
    public sealed class Cat_Equipment_Snapshot
    {
        public long Eq_Id { get; set; }
        public DateTime Snapshot_Date { get; set; }
        public string Eqp_Model { get; set; }
        public string Eqp_Id { get; set; }
        public string Eqp_Serial { get; set; }
        public MOO.Plant? Plant { get; set; }
        public string Wenco_Equip_Ident { get; set; }
        public long? Eqp_Load_Count { get; set; }
        public DateTime? Eqp_Load_Date { get; set; }
        public long? Eqp_Payload_Ttl { get; set; }
        public DateTime? Eqp_Payload_Date { get; set; }
        public string Loc_Lat { get; set; }
        public string Loc_Long { get; set; }
        public string Loc_Alt { get; set; }


        public double? Loc_Lat_Nbr { 
            get {
                if (double.TryParse(Loc_Lat, out double val))
                    return val;
                else return null;
            } 
        }
        public double? Loc_Long_Nbr
        {
            get
            {
                if (double.TryParse(Loc_Long, out double val))
                    return val;
                else return null;
            }
        }
        public double? Loc_Alt_Nbr
        {
            get
            {
                if (double.TryParse(Loc_Alt, out double val))
                    return val;
                else return null;
            }
        }



        public DateTime? Loc_Datetime { get; set; }
        public double? Hours_Idle_Time { get; set; }
        public DateTime? Hours_Idle_Date { get; set; }
        public double? Hours_Oper_Time { get; set; }
        public DateTime? Hours_Oper_Date { get; set; }
        public string Dist_Odometer_Type { get; set; }
        public double? Dist_Odometer_Total { get; set; }
        public DateTime? Dist_Odometer_Date { get; set; }
        public string Engine_Number { get; set; }
        public bool? Engine_Running { get; set; }
        public DateTime? Engine_Date { get; set; }
        public string Fuel_Units { get; set; }
        public double? Fuel_Consumed { get; set; }
        public short? Fuel_Remaining_Pct { get; set; }
        public DateTime? Fuel_Date { get; set; }

    }
}
