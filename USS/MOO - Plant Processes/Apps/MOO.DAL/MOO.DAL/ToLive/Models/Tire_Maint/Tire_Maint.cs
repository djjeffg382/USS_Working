using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Tire_Maint
    {
        public long Tire_Maint_Id { get; set; }
        public DateTime Maint_Date { get; set; }
        public MOO.Plant Plant { get; set; }
        public string Equip_Id { get; set; }
        public string Workorder_Nbr { get; set; }
        public bool? Mems_Verified { get; set; }
        public List<People> Mechanics { get; set; } = [];
    }
}
