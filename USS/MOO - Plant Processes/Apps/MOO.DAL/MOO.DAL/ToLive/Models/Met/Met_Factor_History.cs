using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Met_Factor_History
    {
        public Met_Factor Met_Factor { get; set; }
        public DateTime Effective_Date { get; set; }
        public decimal Factor_Value { get; set; }

    }
}
