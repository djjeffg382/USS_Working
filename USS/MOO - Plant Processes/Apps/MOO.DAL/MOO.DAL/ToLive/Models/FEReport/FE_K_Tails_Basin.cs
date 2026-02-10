using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class FE_K_Tails_Basin
    {
        public long Fe_K_Tb_Id { get; set; }
        public long Fe_Report_Id { get; set; }
        public bool Observation_Normal { get; set; }
        public bool Req_Attention { get; set; }
        public bool Semi_Truck_Hauling { get; set; }
        public bool? Road_Req_Attention { get; set; }
    }
}
