using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Mine_Sched
    {
        public long Mine_Sched_Id { get; set; }
        public MOO.Plant Plant { get; set; }
        public DateTime Sched_Date { get; set; }
        public string Blast_Location { get; set; }
        public string Blast_Delay { get; set; }
        public string Maintenance { get; set; }
        public string Road_Maint { get; set; }
        public string Dump_Maint { get; set; }
    }
}
