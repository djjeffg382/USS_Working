using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MOO.DAL.ToLive.Models
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public sealed class OM_Vessel
    {
        [Key]
        public int Vessel_Id { get; set; }
        public string Vessel_Name { get; set; }
        public bool Active { get; set; } = true;

    }
}
