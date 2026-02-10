using MOO.DAL.ToLive.Enums;
using MOO.DAL.ToLive.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Equipment_Unit_Assoc
    {
        public Equipment_Master Equip { get; set; }
        public Equipment_Unit_Types UnitType { get; set; }
        public EqUnitAssocSource Source { get; set; }
        public string Source_Id { get; set; }
        public string On_State_Value { get; set; }
        public decimal? Multiplier { get; set; }
        public string Pi_Sql { get; set; }
    }
}
