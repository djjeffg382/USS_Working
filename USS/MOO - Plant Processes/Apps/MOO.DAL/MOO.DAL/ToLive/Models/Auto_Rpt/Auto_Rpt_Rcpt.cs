using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Auto_Rpt_Rcpt
    {
        public long Rcpt_Id { get; set; }
        public string Recipient { get; set; }
        public Auto_Rpt_Item Report { get; set; }
        public bool Is_Printer { get; set; }
        public string Email { get; set; }

    }
}
