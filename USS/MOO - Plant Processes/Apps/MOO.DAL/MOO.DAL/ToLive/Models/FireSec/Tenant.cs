using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Tenant
    {
        public int Tenant_Id { get; set; }
        public DateTime Complaint_Date { get; set; }
        public string Plant { get; set; }
        public string Entered_By { get; set; }
        public string Tenant_Name { get; set; }
        public string Tenant_Callback_Nbr { get; set; }
        public string Tenant_Address { get; set; }
        public string Tenant_Issue { get; set; }

    }
}
