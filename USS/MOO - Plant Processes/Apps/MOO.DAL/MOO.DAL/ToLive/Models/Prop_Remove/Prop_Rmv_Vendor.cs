using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Prop_Rmv_Vendor
    {
        [Key]
        public int Prop_Rmv_Vendor_Id { get; set; }
        public string Vendor_Name { get; set; }
        public bool Active { get; set; } = true;

    }
}
