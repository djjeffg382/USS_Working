using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Motor_Area
    {
        public int Motor_Area_Id { get; set; }
        public string Motor_Area_Desc { get; set; }
        public string Last_Modified_By { get; set; }
        public bool Deleted_Record { get; set; } = false;
        public DateTime? Last_Modified_Date { get; set; }
        public Motor_Site Motor_Site { get; set; }

    }
}
