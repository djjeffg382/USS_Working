using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{

    /// <summary>
    /// This class is abstract as it will be inherited by motor_motors and motor_starters as
    /// these have a 1 to 1 relationship in Oracle
    /// </summary>
    public abstract class Motor_Equipment
    {

        /// <summary>
        /// Enum for the type of motor in the motor equipment table
        /// </summary>
        /// <remarks>There is a motor_type table in the oracle datbase, but this should never have more than 2 items in it</remarks>
        public enum EqpType
        {
            Motor = 1,
            Starter = 2
        }


        public int Motor_Equipment_Id { get; set; }

        public EqpType EquipType {
            get
            {
                if (this.GetType() == typeof(Motor_Motors))
                    return EqpType.Motor;
                else
                    return EqpType.Starter;
            }
        }  //this will refer to the motor_equipment motor_type_id field
        public int? Voltage_Rating { get; set; }
        public string Instruction_Book { get; set; }
        public string Equip_Serial_Num { get; set; }
        public string Last_Modified_By { get; set; }
        public bool Deleted_Record { get; set; } = false;
        public DateTime? Last_Modified_Date { get; set; }
        public Motor_Status Motor_Status { get; set; }
        public Motor_Location Motor_Location { get; set; }
        public Motor_Manufacturer Motor_Manufacturer { get; set; }

    }
}
