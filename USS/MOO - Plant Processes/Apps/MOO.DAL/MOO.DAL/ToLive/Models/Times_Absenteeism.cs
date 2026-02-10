using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class TIMES_Absenteeism
    {
        public decimal Absent_Id { get; set; }
        public string Division { get; set; } = String.Empty;
        public string Department { get; set; } = String.Empty;
        public string Crew { get; set; } = String.Empty;
        public DateTime Absent_Date { get; set; }
        public string Pay_Number { get; set; } = String.Empty;
        public string Employee_Name { get; set; } = String.Empty;
        public string Position { get; set; } = String.Empty;
        public decimal Shift { get; set; }
        public string Reason_Code { get; set; } = String.Empty;
        public string Reason_Desc { get; set; } = String.Empty;
        public decimal Hours { get; set; }
        public DateTime Imported_Date { get; set; }
        public string Comments { get; set; } = String.Empty;
        public string Location { get; set; } = String.Empty;
        public People Person { get; set; }
        public People Supervisor { get; set; }
    }
}
