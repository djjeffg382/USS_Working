using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Motor_Maintenance
    {
        public int Motor_Maintenance_Id { get; set; }
        public Motor_Equipment Motor_Equipment { get; set; }
        public DateTime? Date_Of_Maint { get; set; }
        public string Notes { get; set; }
        public string Work_Order { get; set; }
        public string Maint_Done_By { get; set; }
        public bool Deleted_Record { get; set; } = false;
        public DateTime? Last_Modified_Date { get; set; }
        public string Last_Modified_By { get; set; }
    }
}
