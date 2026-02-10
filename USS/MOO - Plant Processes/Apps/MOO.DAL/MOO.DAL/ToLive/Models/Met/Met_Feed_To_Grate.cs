using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Met_Feed_To_Grate
    {
        public DateTime Datex { get; set; }
        public byte Step { get; set; }
        public Met_Material Material { get; set; }
        public Met_DMY Dmy { get; set; }
        public decimal? Conc_Tons { get; set; }
        public decimal? F_T_G { get; set; }
        public decimal? Factor { get; set; }
        public decimal? Train_H2o { get; set; }
        public decimal? Inventory { get; set; }
    }
}
