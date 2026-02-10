using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MOO.DAL.ToLive.Models
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public sealed class OM_Location_Inv
    {
        public int Location_Inv_Id { get; set; }
        public int Location_Id => Location.Location_Id;

        [NotMapped]
        public OM_Location Location { get; set; }

        public int Location_Trans_Id => Location_Transaction.Location_Trans_Id;

        [NotMapped]
        public OM_Location_Trans Location_Transaction { get; set; }

        public DateTime Inventory_Date { get; set; }
        public double Quantity { get; set; }
        public float Fe { get; set; } = 0;
    }
}
