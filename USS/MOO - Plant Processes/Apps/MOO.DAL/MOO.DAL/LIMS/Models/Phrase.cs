using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.LIMS.Models
{
    public class Phrase
    {
        //note: I did not put every field from the DB in here.  Only put fields we may use
        public string Phrase_Type { get; set; }
        public string Phrase_Id { get; set; }
        public int Order_Num { get; set; }
        public string Phrase_Text { get; set; }
        public string Icon { get; set; }
    }
}
