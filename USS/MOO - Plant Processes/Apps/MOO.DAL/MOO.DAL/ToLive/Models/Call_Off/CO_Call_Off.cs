using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class CO_Call_Off
    {
        public int Call_Off_Id { get; set; }
        public DateTime Entry_Date { get; set; }
        public DateTime Start_Date { get; set; }
        public DateTime? Return_Date { get; set; }
        public bool Shift1 { get; set; }
        public bool Shift2 { get; set; }
        public bool Shift3 { get; set; }
        public People Person { get; set; }
        public CO_Dept Dept { get; set; }
        public List<CO_Reason> Reasons { get; } = new();
        public string Comments { get; set; }
        public string Manager_Comments { get; set; }
        public string File_Path { get; set; }
        public string File_Desc { get; set; }
        public bool Acceptable_Excuse { get; set; }
        public bool Forced_Turn { get; set; }
        public bool Extension_Marker { get; set; }
        public int Shifts_Missed { get; set; }
        public string Created_By { get; set; }

        /// <summary>
        /// Earned Safe and Sick Time Hours
        /// </summary>
        public float? Esst_Hrs { get; set; }
    }
}
