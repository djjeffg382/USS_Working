using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Sec_Request_Item
    {
        public long Sec_Request_Item_Id { get; set; }
        public string Item_Name { get; set; }
        public Sec_Role? Additional_Info_Role { get; set; }
        public Sec_Role? Approver_Role { get; set; }
        public Sec_Role Action_Role { get; set; }
        public bool Active { get; set; }
        public string Request_Comment_Header { get; set; }
    }
}
