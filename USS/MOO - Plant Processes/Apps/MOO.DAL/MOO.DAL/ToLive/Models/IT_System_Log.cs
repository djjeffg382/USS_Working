using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class IT_System_Log
    {
        [Key]
        public int It_Log_Id { get; set; }

        [NotMapped()]
        public IT_System It_System { get; set; }

        public int? IT_System_ID => It_System?.It_System_Id;

        public string ChangedBy { get; set; }
        public DateTime LogDate { get; set; }
        public string LogChange { get; set; }
        public string ChangeSet { get; set; }
        public string RequestedBy { get; set; }
        public string Version { get; set; }
        public DateTime Entered_Date { get; set; }

    }
}
