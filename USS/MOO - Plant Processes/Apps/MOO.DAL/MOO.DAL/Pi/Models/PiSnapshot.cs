using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Pi.Models
{
    public class PiSnapshot
    {
        public string Tag { get; set; }
        public DateTime Time { get; set; }
        
        
        //The value that is returned from PI can be a string, a number, or a date.  We will provide 3 values to pull from
        public decimal? ValueNbr { get; set; } = null;

        public string ValueStr { get; set; } = "";

        public DateTime? ValueDate { get; set; } = null;

        /// <summary>
        /// Converted value to Digital State, use if PI Tag is a Digital tag
        /// </summary>
        public string DigitalState { get; set; } = "";
        public int Status { get; set; }
        public bool Questionable { get; set; }
        public bool Substituted { get; set; }
        public bool Annotated { get; set; }
        public string Annotations { get; set; }


    }
}
