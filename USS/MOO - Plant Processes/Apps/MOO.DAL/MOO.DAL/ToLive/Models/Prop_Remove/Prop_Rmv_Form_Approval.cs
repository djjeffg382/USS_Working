using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Prop_Rmv_Form_Approval
    {
        [Key]
        public int Prop_Rmv_Form_Apv_Id { get; set; }
        public int Prop_Rmv_Form_Id { get; set; }
        public int Approval_User_Id { get { return Approval_User.User_Id; } }

        [NotMapped]
        public Sec_Users Approval_User { get; set; }
        public DateTime Approval_Date { get; set; }

        public bool Admin_Override { get; set; } = false;


    }
}
