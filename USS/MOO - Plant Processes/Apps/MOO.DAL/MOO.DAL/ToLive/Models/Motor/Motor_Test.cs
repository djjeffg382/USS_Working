using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MOO.DAL.ToLive.Models
{
    public class Motor_Test
    {
        public int Motor_Test_Id { get; set; }
        public Motor_Motors Motor_Equipment { get; set; }
        public DateTime? Date_Tested { get; set; }
        public string Notes { get; set; }
        public int? Motor_Change_Id { get; set; }
        public string Type_Of_Test { get; set; } = "1";
        public string Test_Done_By { get; set; }
        public decimal? Measured_Reading { get; set; }
        public decimal? Test_Lead_Reading { get; set; }
        public decimal? Actual_Reading { get; set; }
        public decimal? Ground_Check { get; set; }

        public bool Deleted_Record { get; set; } = false;
        public DateTime? Last_Modified_Date { get; set; }

        public string Last_Modified_By { get; set; }
    }
}
