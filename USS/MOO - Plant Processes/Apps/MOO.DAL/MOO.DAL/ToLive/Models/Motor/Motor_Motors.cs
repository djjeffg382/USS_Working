using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Motor_Motors: Motor_Equipment
    {
        public enum Drive_Config
        {
            Unknown,
            Belt,
            Direct,
            Sump,
            Either
        }

        public enum JBox
        {
            Unknown,
            F1,
            F2,
            F3,
            F4,
            Vertical,
            F2_Or_Vertical,
            Dbl_Shaft
        }

        public string Notes { get; set; }
        public string Motor_Type { get; set; }
        public string Frame_Size { get; set; }
        public decimal? Horse_Power { get; set; }
        public int? Rpm { get; set; }
        public string Inner_Bearing { get; set; }
        public string Outer_Bearing { get; set; }
        public int? Phase { get; set; }
        public decimal? Full_Load_Amps { get; set; }
        public DateTime? Purchase_Date { get; set; }
        public bool? Inverter_Rated { get; set; }
        public Drive_Config Drive_Configuration { get; set; }
        public bool? Explosion_Proof { get; set; }
        public int? Weight { get; set; }
        public JBox J_Box_Setup { get; set; }
        public string RTD_Type { get; set; }
        public string Insulation_Class { get; set; }
        public double? Service_Factor { get; set; }
        public bool? Critical_Motor { get; set; }
        public long? Catalog_Id_Number { get; set; }
    }
}
