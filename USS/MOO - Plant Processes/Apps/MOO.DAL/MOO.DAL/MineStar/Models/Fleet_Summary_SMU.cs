using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.MineStar.Models
{
    /// <summary>
    /// This is pretty much the same thing as the DMART tolive.Cat_Equipment_Snapshot.  This has the same data but is used by Cat Reporting
    /// </summary>
    public sealed class Fleet_Summary_SMU
    {

        public Fleet_Summary_SMU() { }

        /// <summary>
        /// used to create a new Fleet_Summary_SMU object from a Cat_Equipment_Snapshot object
        /// </summary>
        /// <param name="EqpSnapShot"></param>
        public Fleet_Summary_SMU(ToLive.Models.Cat_Equipment_Snapshot EqpSnapShot) {

            Snapshot_Date = EqpSnapShot.Snapshot_Date;
            Eqp_Model = EqpSnapShot.Eqp_Model;
            Eqp_Id = EqpSnapShot.Eqp_Id;
            Eqp_Serial = EqpSnapShot.Eqp_Serial;
            Eqp_Load_Count = EqpSnapShot.Eqp_Load_Count;
            Eqp_Load_Date = EqpSnapShot.Eqp_Load_Date;
            Eqp_Payload_Ttl = EqpSnapShot.Eqp_Payload_Ttl;
            Eqp_Payload_Date = EqpSnapShot.Eqp_Payload_Date;
            Loc_Lat = EqpSnapShot.Loc_Lat_Nbr;
            Loc_Long = EqpSnapShot.Loc_Long_Nbr;
            Loc_Alt = EqpSnapShot.Loc_Lat_Nbr;
            Loc_Datetime = EqpSnapShot.Loc_Datetime;
            Hours_Idle_Time = EqpSnapShot.Hours_Idle_Time;
            Hours_Idle_Date = EqpSnapShot?.Hours_Idle_Date;
            Hours_Oper_Time = EqpSnapShot.Hours_Oper_Time;
            Hours_Oper_Date = EqpSnapShot.Hours_Oper_Date;
            Dist_Odometer_Type = EqpSnapShot.Dist_Odometer_Type;
            Dist_Odometer_Total = EqpSnapShot.Dist_Odometer_Total;
            Dist_Odometer_Date = EqpSnapShot.Dist_Odometer_Date;
            Engine_Number = EqpSnapShot.Engine_Number;
            Engine_Running = EqpSnapShot.Engine_Running;
            Engine_Date = EqpSnapShot.Engine_Date;
            Fuel_Units = EqpSnapShot.Fuel_Units;
            Fuel_Consumed = EqpSnapShot.Fuel_Consumed;
            Fuel_Remaining_Pct = EqpSnapShot.Fuel_Remaining_Pct;
            Fuel_Date = EqpSnapShot.Fuel_Date;
        }

        public int Row_Id { get; set; }
        public DateTime Snapshot_Date { get; set; }
        public string Eqp_Model { get; set; }
        public string Eqp_Id { get; set; }
        public string Eqp_Serial { get; set; }
        public long? Eqp_Load_Count { get; set; }
        public DateTime? Eqp_Load_Date { get; set; }
        public long? Eqp_Payload_Ttl { get; set; }
        public DateTime? Eqp_Payload_Date { get; set; }
        public double? Loc_Lat { get; set; }
        public double? Loc_Long { get; set; }
        public double? Loc_Alt { get; set; }
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
