using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Analytics_Key_Value
    {
        public string Key { get; set; }
        public DateTime Last_Update { get; set; }
        public string Value { get; set; }
    }
}
