using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Lineup_Meeting
    {
        public DateTime Lineup_Date { get; set; }
        public string Lineup_Name { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

    }
}
