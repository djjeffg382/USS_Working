using MOO.DAL.ToLive.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Equipment_Component_Master
    {
        public Equipment_Master Equip { get; set; }
        public short Component_Id { get; set; }
        public string Equip_Desc { get; set; }
        public bool Active_Status { get; set; }
        public string Erp_Desc { get; set; }
        public string Lims_Samp_Pnt { get; set; }
        public Equipment_Unit_Types Lims_Unit { get; set; }
    }
}
