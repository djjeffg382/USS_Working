using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Motor_Change
    {

        public int Motor_Change_Id { get; set; }
        public Motor_Equipment Motor_Equipment { get; set; }
        public DateTime Date_Of_Change { get; set; }
        public string Work_Order { get; set; }
        public string Change_Done_By { get; set; }
        public string Notes { get; set; }
        public Motor_Status Old_Motor_Status { get; set; }
        public Motor_Status New_Motor_Status { get; set; }
        public Motor_Location Old_Motor_Location { get; set; }
        public Motor_Location New_Motor_Location { get; set; }

        /// <summary>
        /// Requires Ground Check
        /// </summary>
        public bool Req_Grd_Chk { get; set; }
        public bool Deleted_Record { get; set; } = false;
        public DateTime? Last_Modified_Date { get; set; }
        public string Last_Modified_By { get; set; }
    }
}
