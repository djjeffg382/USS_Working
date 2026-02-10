using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Prop_Rmv_Form
    {
        public enum PrStatus
        {
            Open,
            Approved,
            Closed,
            Voided
        }

        [Key]
        public int Prop_Rmv_Form_Id { get; set; }


        internal int Prop_Rmv_Area_Id { get { return Area.Prop_Rmv_Area_Id; } }
        [NotMapped]
        public Prop_Rmv_Area Area { get; set; }


        public int Prop_Rmv_Reason_Id { get { return Reason.Prop_Rmv_Reason_Id; } }
        [NotMapped]
        public Prop_Rmv_Reason Reason { get; set; }


        internal int Prop_Rmv_Vendor_Id { get { return Vendor.Prop_Rmv_Vendor_Id; } }
        [NotMapped]
        public Prop_Rmv_Vendor Vendor { get; set; }

        public int Created_By_Id { get { return Created_By.User_Id; } }
        [NotMapped]
        public Sec_Users Created_By { get; set; }


        public DateTime Created_Date { get; set; }
        public DateTime? Closed_Date { get; set; }


        public PrStatus Status { get; set; } = PrStatus.Open;
        public string Vendor_Contact { get; set; }
        public string Explain_Other { get; set; }
        public bool To_Be_Returned { get; set; } = true;

    }
}
