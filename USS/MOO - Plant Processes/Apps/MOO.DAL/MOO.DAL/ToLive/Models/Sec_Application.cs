using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Sec_Application
    {
        public int Application_Id { get; set; } = -1;
        public string Application_Name { get; set; }
        public string Application_Description { get; set; }
        public DateTime Created_Date { get; set; }
        public string Created_By { get; set; }
        public string Modified_By { get; set; }
        public string Application_Notes { get; set; }

    }
}
