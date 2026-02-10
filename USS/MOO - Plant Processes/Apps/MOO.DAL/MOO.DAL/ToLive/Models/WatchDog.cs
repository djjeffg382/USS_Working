using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class WatchDog
    {
        public string WDName { get; set; }
        public bool Active { get; set; }
        public string ConnectionName { get; set; }
        public string Command { get; set; }
        public bool EmailWebmaster { get; set; }
        public string EmailList { get; set; }
        public string Runtimes { get; set; }
        public DateTime? LastRun { get; set; }
        public decimal? Failures { get; set; }
        public string EmailNote { get; set; }
    }
}
