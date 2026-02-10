using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class FS_WorkOrders
    {
        public decimal Workorders_Id { get; set; }
        public decimal Workorder_Nbr { get; set; }
        public DateTime Date_Open { get; set; }
        public string Dispatcher { get; set; }
        public string Requester { get; set; }
        public string Phone_Nbr { get; set; }
        public string Location { get; set; }
        public string Service_Request { get; set; }
        public bool? Fire_Chief_Ntfy { get; set; }
        public string Firefighter_Notified { get; set; }
        public DateTime? Time_Notified { get; set; }
        public string Event_Type { get; set; }
        public string Comments { get; set; }
        public string Work_Completed_By { get; set; }
        public DateTime? Date_Completed { get; set; }
        public string Security_Notified { get; set; }
        public MOO.Plant Plant { get; set; }
        public bool? Audited { get; set; }
        public string Audited_By { get; set; }

    }
}
