using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class FS_Hotworks
    {
        public decimal Hotworks_Id { get; set; }
        public string Permit_Nbr { get; set; }
        public string Supervisor { get; set; }
        public string Employee_Nbr { get; set; }
        public string Phone_Nbr { get; set; }
        public string Location { get; set; }
        public DateTime? Date_Open { get; set; }
        public string Badge_Nbr_Open { get; set; }
        public DateTime? Date_Close { get; set; }
        public string Badge_Nbr_Close { get; set; }
        public string Comments { get; set; }
        public MOO.Plant Plant { get; set; }
        public bool? Audited { get; set; }
        public string Audited_By { get; set; }
    }
}
