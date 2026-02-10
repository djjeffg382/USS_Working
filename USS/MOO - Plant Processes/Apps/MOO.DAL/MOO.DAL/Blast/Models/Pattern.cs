using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Blast.Models
{
    public class Pattern
    {
        public int Id { get; set; }
        public string Pattern_Number { get; set; }
        public DateTime? Update_Date { get; set; }
        public ToLive.Models.Mine_Sched Mine_Sched { get; set; }
    }
}
