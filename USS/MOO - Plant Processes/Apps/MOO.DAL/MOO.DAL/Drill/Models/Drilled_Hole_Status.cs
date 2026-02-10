using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Drill.Models
{
    public class Drilled_Hole_Status
    {
        public enum StatusType
        {
            Operating,
            OpDelay,
            Maint,
            SchedDown,
            Standby,
            MgmtDec
        }

        public int Drilled_Hole_Status_Id { get; set; }
        public int Drilled_Hole_Id { get; set; }
        public MOO.Plant Plant { get; set; }
        public DateTime Shift_Date { get; set; }
        public short Shift { get; set; }
        public DateTime Start_Time { get; set; }
        public DateTime End_Time { get; set; }
        public string StatusCode { get; set; }
        public StatusType StatusBucket { get; set; }

    }
}
