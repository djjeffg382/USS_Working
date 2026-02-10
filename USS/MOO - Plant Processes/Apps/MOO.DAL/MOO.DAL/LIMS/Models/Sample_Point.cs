using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.LIMS.Models
{
    public class Sample_Point
    {
        //note: I did not put every field from the DB in here.  Only put fields we may use
        public string  Identity { get; set; }
        public Location Point_Location { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public Phrase Oil_Type { get; set; }
        public Phrase Lube_Point_Type { get; set; }
    }
}
