using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class PLO_Pellet_Spec
    {
        public long Plo_Pellet_Spec_Id { get; set; }
        public DateTime Start_Date { get; set; }
        public DateTime? End_Date { get; set; }
        public DateTime Modified_Date { get; set; }
        public string Modified_By { get; set; }
        public PLO_Product Product { get; set; }


    }
}
