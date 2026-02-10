using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Core.Models
{
    public class Approval
    {
        public int Approval_Id { get; set; }
        public string Approved_By { get; set; }
        public DateTime Approved_Date { get; set; }
    }
}
