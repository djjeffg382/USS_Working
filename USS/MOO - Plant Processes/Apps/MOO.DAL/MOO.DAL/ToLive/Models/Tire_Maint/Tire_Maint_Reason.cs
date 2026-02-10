using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.Enums.Extension;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Tire_Maint_Reason
    {
        public int Tire_Maint_Reason_Id { get; set; }
        public MOO.DAL.ToLive.Enums.TireMaintReason Reason_Name { get; set; }
    }
}
