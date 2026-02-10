using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class GCP_Deviations
    {
        public byte Line { get; set; }
        public DateTime The_Date { get; set; }
        public string Deviation_Type { get; set; }
        public short Num_Of_Good_Reads { get; set; }
        public short Num_Of_Bad_Reads { get; set; }
        public string Corrective_Action { get; set; }
    }
}
