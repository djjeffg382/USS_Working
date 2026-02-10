using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Met_Agg_Standards
    {
        public DateTime Datex { get; set; }
        public Met_DMY Dmy { get; set; }
        public decimal? Ltd_Standard { get; set; }
        public decimal? Ltd_Standard_Furnace { get; set; }
        public decimal? Reduce_Standard { get; set; }
        public decimal? Reduce_Standard_Furnace { get; set; }
        public decimal? New_Ltd_Reduce_Standard { get; set; }

    }
}
