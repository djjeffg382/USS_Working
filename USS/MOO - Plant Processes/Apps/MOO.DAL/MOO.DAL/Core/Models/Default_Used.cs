using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Core.Models
{
    public class Default_Used
    {
        public decimal Default_Group_Id { get; set; }
        public DateTime Start_Date_No_Dst { get; set; }
        public DateTime? Shift_Date { get; set; }
        public decimal? Shift { get; set; }
        public decimal? Half { get; set; }
        public decimal? Hour { get; set; }
    }
}
