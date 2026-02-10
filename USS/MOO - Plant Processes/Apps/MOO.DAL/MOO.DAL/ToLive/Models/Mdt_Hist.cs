using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Mdt_Hist
    {
        public string Mac_Addr1 { get; set; }
        public string Device_Name { get; set; }
        public string Equipment_Id { get; set; }
        public MOO.Plant Plant { get; set; }
        public DateTime Insert_Date { get; set; }
    }
}
