using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class ERP_Leases
    {
        public string Lease { get; set; }
        public DateTime Month_Date { get; set; }
        public decimal? Weight { get; set; }
        public MOO.Plant Site { get; set; }

    }
}
