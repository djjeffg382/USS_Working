using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Property_Removal_Vendors
    {
        public string Company_Name { get; set; }
        public bool Active { get; set; }
        [Key]
        public int Company_Id { get; set; }
    }
}
