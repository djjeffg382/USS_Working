using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Motor_Location
    {
        public int Motor_Location_Id { get; set; }
        public string Motor_Location_Desc { get; set; }
        public string Last_Modified_By { get; set; }
        public bool Deleted_Record { get; set; } = false;
        public DateTime? Last_Modified_Date { get; set; }
        public Motor_Area Motor_Area { get; set; }

        public bool Is_Asset_Location { get; set; }

        public string FullPath { get { return $"{this.Motor_Area?.Motor_Site?.Motor_Site_Name}/{this.Motor_Area.Motor_Area_Desc}/{this.Motor_Location_Desc}"; } }

    }
}
