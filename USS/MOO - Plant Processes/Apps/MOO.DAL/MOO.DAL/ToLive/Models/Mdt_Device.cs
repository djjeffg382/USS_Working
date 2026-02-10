using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Mdt_Device
    {
        public string Mac_Addr1 { get; set; }
        public string Mac_Addr2 { get; set; }
        public string Mesh_Mac_Addr { get; set; }
        public string Device_Name { get; set; }
        public string Ip_Addr { get; set; }
        public string Software { get; set; }
        public DateTime Last_Update { get; set; }
        public string Model { get; set; }
        public string Os_Version { get; set; }
        public int? Disk_Size_Mb { get; set; }
        public string Equipment_Id { get; set; }
        public DateTime Insert_Date { get; set; }
        public MOO.Plant Plant { get; set; }
    }
}
