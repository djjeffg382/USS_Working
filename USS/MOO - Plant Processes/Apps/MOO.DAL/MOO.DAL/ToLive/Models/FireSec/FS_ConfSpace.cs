using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class FS_ConfSpace
    {
        public decimal Confinedspace_Id { get; set; }
        public DateTime Date_Open { get; set; }
        public DateTime Created_Date { get; set; }
        public string Call_Back_Nbr { get; set; }
        public string Location { get; set; }
        public DateTime? Clear_Time { get; set; }
        public string Supervisor { get; set; }
        public MOO.Plant Plant { get; set; }
        public bool? Audited { get; set; }
        public string Audited_By { get; set; }
        public bool? Permitted { get; set; }

    }
}
