using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Rental_Equip
    {
        public decimal Rental_Equip_Id { get; set; }
        public MOO.Plant Plantloc { get; set; }
        public People Manager { get; set; }
        public string Vendor { get; set; }
        public string Vehicletype { get; set; }
        public string Location { get; set; }
        public string Locationdesc { get; set; }
        public DateTime Entrydate { get; set; }
        public DateTime? Outdate { get; set; }
        public DateTime? Inspectiondate { get; set; }
        public People Inspectedby { get; set; }
        public string Po_Number { get; set; }
        public string Soifilename { get; set; }
        public string Comments { get; set; }
        public decimal? Phonenumber { get; set; }
    }
}
