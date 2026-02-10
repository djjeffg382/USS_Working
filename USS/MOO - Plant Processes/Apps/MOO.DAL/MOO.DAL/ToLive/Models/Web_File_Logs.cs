using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Web_File_Logs
    {
        public DateTime Web_Date { get; set; }
        public string Filename { get; set; }
        public int View_Count { get; set; }
        public string Users { get; set; }

    }
}
