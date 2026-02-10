using MOO.DAL.ToLive.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Sec_Request
    {
        public long Sec_Request_Id { get; set; }
        public string Request_First_Name { get; set; }
        public string Request_Last_Name { get; set; }
        public string Request_Username { get; set; }
        public Sec_Users Manager { get; set; }
        public Sec_Request_Item Sec_Request_Item { get; set; }
        public string Request_Comments { get; set; }
        public DateTime? Created_Date { get; set; }
        public Sec_Users? Additional_Info_User { get; set; }
        public string Additional_Info_Comments { get; set; }
        public Sec_Request_Status Current_Status { get; set; }
        public Sec_Users? Approval_By { get; set; }
        public DateTime? Approval_Date { get; set; }
        public string Approval_Comments { get; set; }
        public Sec_Users? Closed_User { get; set; }
        public string Closed_Comments { get; set; }
        public bool? Approved { get; set; }
        public Sec_Users User { get; set; }
        public bool Email_Flag { get; set; }
    }
    public class Sec_Request_Status
    {
        public long Status_Id { get; set; }
        public string Status_Name { get; set; }
        public string Code { get; set; }
    }
}
