using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ERP.Models
{
    public sealed class ERPLeaseValues
    {
        public DateTime Shift_Date { get; set; }
        public decimal Value { get; set; }
        public string PropertyCode { get; set; }
        public string LeaseCode { get; set; }
        public string LeaseName { get; set; }
        public int LessorGroupCode { get; set; }
        public string LessorGroupName { get; set; }
    }
}
