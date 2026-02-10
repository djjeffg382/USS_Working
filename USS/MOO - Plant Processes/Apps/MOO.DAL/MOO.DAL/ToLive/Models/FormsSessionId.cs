using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class FormsSessionId
    {
        [Key]
        public string Session_Id { get; set; }

        public string WebLoginName { get; set; }
        public string NtUserName { get; set; }
        public string ChangedFromIp { get; set; }
        public string Value { get; set; }
        public DateTime RecordDate { get; set; }

    }
}
