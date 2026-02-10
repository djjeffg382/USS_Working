using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Temp_Badge_Department
    {
        public long Department_Id { get; set; }
        public string Department { get; set; }
        public string Email_Address { get; set; }
        public MOO.Plant Plantloc { get; set; }

    }
}
