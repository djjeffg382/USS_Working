using MOO.DAL.ToLive.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Equipment_Comp_Units_Rollup
    {
        public Equipment_Component_Master EquipmentComponent { get; set; }
        public Equipment_Unit_Types UnitType { get; set; }
        public DateTime Rollup_Date { get; set; }
        public decimal? Daily_Total { get; set; }
        public decimal? Weekly_Total { get; set; }
        public decimal? Monthly_Total { get; set; }
        public decimal? Ytd_Units { get; set; }
        public decimal? Ltd_Units { get; set; }
        public decimal? Inspection_Units { get; set; }
        public bool Active_Status { get; set; }

    }
}
