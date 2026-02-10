using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Error
    {
        public DateTime Error_Date { get; set; }
        public int Error_Num { get; set; }
        public string PName { get; set; }
        public string Error_Sql { get; set; }
        public int Error_Type { get; set; }
        public string Error_Desc { get; set; }
        public string Error_Stack { get; set; }
    }
}
