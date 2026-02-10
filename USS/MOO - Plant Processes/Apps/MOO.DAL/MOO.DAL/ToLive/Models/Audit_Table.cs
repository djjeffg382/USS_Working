using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Audit_Table
    {
        public DateTime Thedate { get; set; }
        public string Modified_By { get; set; }
        public string Table_Name { get; set; }
        public string Column_Name { get; set; }
        public string Key_Value { get; set; }
        public string Old_Value { get; set; }
        public string New_Value { get; set; }

    }
}
