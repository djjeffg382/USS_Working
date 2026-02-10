using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Prop_Rmv_Area
    {
        [Key]
        public int Prop_Rmv_Area_Id { get; set; }
        public MOO.Plant Plant { get; set; }
        public string Area_Name { get; set; }
        public bool Active { get; set; } = true;

    }
}
