using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class ScheduleGroups
    {
        public decimal Group_Id { get; set; }
        public string Group_Name { get; set; }
        public string Calendar_Name { get; set; }
        public string Color { get; set; }
        public bool Active { get; set; }

    }
}
