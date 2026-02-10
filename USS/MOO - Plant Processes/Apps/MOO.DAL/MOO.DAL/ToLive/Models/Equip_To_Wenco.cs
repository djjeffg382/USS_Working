using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Equip_To_Wenco
    {
        public enum SystemType
        {
            Epiroc,
            Komatsu,
            Terrain,
            VisionLink,
            Samsara
        }


        public MOO.Plant Plant { get; set; }
        public string Wenco_Equip_Ident { get; set; }
        public string Foreign_Id { get; set; }
        public string Previous_Status { get; set; }
        public DateTime Previous_Status_Date { get; set; } = DateTime.Now;
        public DateTime Previous_Position_Date { get; set; } = DateTime.Now;
        public DateTime Previous_Badge_Date { get; set; } = DateTime.Now;
        public bool Active { get; set; } = true;

        public SystemType System_Name { get; set; }
        public DateTime Last_Sys_Access_Date { get; set; }
        public string Wenco_Gen_Production_Code { get; set; }
    }
}
