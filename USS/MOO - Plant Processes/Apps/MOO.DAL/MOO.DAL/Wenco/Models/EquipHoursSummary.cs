using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Wenco.Models
{
    public class EquipHoursSummary
    {
        public DateTime ShiftDate { get; set; }
        public byte Shift { get; set; }

        public decimal OperatingHrs { get; set; } = 0;
        public decimal OperatingDelayHrs { get; set; } = 0;
        public decimal StandbyHrs { get; set; } = 0;
        public decimal MaintenanceHrs { get; set; } = 0;
        public decimal MgmtDecisionHrs { get; set; } = 0;
        public decimal SchedDownHrs { get; set; } = 0;
    }
}
