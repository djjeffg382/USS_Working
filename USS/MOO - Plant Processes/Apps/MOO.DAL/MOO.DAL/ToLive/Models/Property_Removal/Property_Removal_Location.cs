using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Property_Removal_Location
    {
        [Key]
        public string Plant_Area { get; set; }
        public string Active { get; set; }

    }
}
