using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Mill_Charging
    {
        public DateTime Charge_Date { get; set; }
        public string Equip_Id { get; set; }
        public int Charge_Count { get; set; }
        public decimal Lbs_Per_Charge { get; set; }

    }
}
