using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Blast.Models
{
    public class Blast
    {
        public int Id { get; set; }
        public Pattern Pattern { get; set; }
        public DateTime? Blast_Meeting_Time { get; set; }
        public decimal? Wind_Direction { get; set; }
        public decimal? Wind_Speed_Mph { get; set; }
        public decimal? Delay_Lbs { get; set; }
        public decimal? Blast_Agent_Lbs { get; set; }
        public CAT_Pits Pit { get; set; }
        public CAT_Rock_Type Rock_Type { get; set; }
        public decimal? Taconite_Tons { get; set; }
        public decimal? Waste_Tons { get; set; }
        public string Sky_Cond { get; set; }
        public decimal? Temp_2200 { get; set; }
        public decimal? Temp_2700 { get; set; }
        public decimal? Temp_3200 { get; set; }
        public decimal? Temp_3700 { get; set; }
        public decimal? Temp_4200 { get; set; }
        public decimal? Temp_4700 { get; set; }
        public decimal? Temp_5200 { get; set; }
        public decimal? Temp_5700 { get; set; }
        public decimal? Temp_6200 { get; set; }
        public decimal? Temp_6700 { get; set; }
        public Enums.Open_Sinking_Type Open_Sinking { get; set; }
        public string Flying { get; set; }
        public string Position_Code { get; set; }
        public int? Pattern_Size_Id { get; set; }
        public long? Northing { get; set; }
        public long? Easting { get; set; }
        public string Blast_Time { get; set; }
        public decimal? Temp_Surface { get; set; }
        public decimal? Test_Lbs { get; set; }
        public decimal? Complaint { get; set; }
        public string Comments { get; set; }
        public decimal? Barometric_Pressure { get; set; }
        public short? Subdrill { get; set; }
    }
}
