using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Enums
{
    /// <summary>
    /// Enum for Equipment_Unit_Types.  See tolive.equipment_unit_types
    /// </summary>
    public enum EqUnitTypes
    {
        [Description("Operating Hours")]
        Hours = 1,

        [Description("Repair Hours")]
        Repair_Hours = 2,

        [Description("Standby Hours")]
        Standby_Hours = 3,

        [Description("Tons")]
        Tons = 4,

        [Description("Operating Down")]
        Oper_Down = 5,

        [Description("Repair Unscheduled")]
        Repair_Unsched = 6,

        [Description("Cycles")]
        Cycles = 7,

        [Description("Total Operating Hours")]
        Oper_Hours = 8,

        [Description("Management Decision Down")]
        Mgmt_Decision_Down = 9,

        [Description("Meter Hours")]
        Meter_Hours = 10,

        [Description("Unknown")]
        UnKnown = 99
    }
}
