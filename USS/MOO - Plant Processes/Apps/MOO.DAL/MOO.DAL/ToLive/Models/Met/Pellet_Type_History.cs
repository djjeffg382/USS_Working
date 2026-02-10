using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Pellet_Type_History
    {
        public byte Ag_Step { get; set; }
        public Pellet_Type Pel_Type { get; set; }
        public DateTime Start_Date { get; set; }

    }
}
