using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Drill.Models
{
    public class Equip_Hours
    {
        public int Equip_Hours_Id { get; set; }
        public Equip Equip { get; set; }
        public DateTime ShiftDate { get; set; }
        public byte Shift { get; set; }
        public decimal Oper { get; set; }
        public decimal Maint { get; set; }
        public decimal Standby { get; set; }
        public decimal Sched { get; set; }
        public decimal OperDelay { get; set; }
        public decimal MgmtDec { get; set; }

    }
}
