using MOO.DAL.ToLive.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Equipment_Unit_Types
    {
        public  EqUnitTypes UnitType { get; set; }
        public short Unit_Type { get; set; }
        public string Unit_Desc { get; set; }
        public string Unit_Comment { get; set; }
    }
}
