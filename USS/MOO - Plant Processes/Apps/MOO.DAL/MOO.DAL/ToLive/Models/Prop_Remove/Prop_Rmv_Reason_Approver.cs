using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Prop_Rmv_Reason_Approver
    {
        [Key]
        public int Prop_Rmv_Rsn_Apv_Id { get; set; }

        //these 2 internal properties are included for the SQL Builder
        internal int Prop_Rmv_Reason_Id { get { return Reason.Prop_Rmv_Reason_Id; } }
        internal int Sec_User_Id { get { return User.User_Id; } }

        [NotMapped]
        public Prop_Rmv_Reason Reason { get; set; }
        [NotMapped]
        public Sec_Users User { get; set; }

    }
}
