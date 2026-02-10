using MOO.DAL.ToLive.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Tire_Maint_Tire
    {
        public long Tire_Maint_Tire_Id { get; set; }
        public long Tire_Maint_Id { get; set; }
        public int Tire_Position { get; set; }
        public Tire_Maint_Brand Brand { get; set; }
        public string Serial_Nbr { get; set; }
        public string Tread_Depth_In { get; set; }
        public string Tread_Depth_Out { get; set; }
        public TireMaintReason? Removal_Reason1 { get; set; }
        public TireMaintReason? Removal_Reason2 { get; set; }
        public Disposition Disposition { get; set; }
        public RemoveInstall  Remove_Install { get; set; }
        public bool? Nuts_Torqued { get; set; }
        public DateTime? Tire_Shop_Date { get; set; }
        public TireShopStatus Tire_Shop_Status { get; set; }

    }
}
