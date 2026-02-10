using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Property_Removal_Reasons
    {
        [Key]
        public int Reason_Id { get; set; }
        public string Reason { get; set; }
        public bool Active { get; set; }
    }
}
