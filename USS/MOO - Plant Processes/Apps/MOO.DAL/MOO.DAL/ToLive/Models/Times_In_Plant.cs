using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Times_In_Plant
    {
        public int In_Plant_Id { get; set; }
        public string Division { get; set; }
        public string Department { get; set; }
        public string Crew { get; set; }
        public string Pay_Number { get; set; }
        public string Location { get; set; }
        public string Employee_Name { get; set; }
        public string Uss_Id { get; set; }
        public DateTime In_Date { get; set; }
        public DateTime Shift_Date { get; set; }
        public short Shift { get; set; }

    }
}
