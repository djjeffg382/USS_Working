using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Prop_Rmv_Area_Approver
    {
        public int Prop_Rmv_Area_Apv_Id { get; set; }


        //these 2 internal properties are included for the SQL Builder
        internal int Prop_Rmv_Area_Id { get { return Area.Prop_Rmv_Area_Id; } }
        internal int Sec_User_Id { get { return User.User_Id; } }

        [NotMapped]
        public Prop_Rmv_Area Area { get; set; }
        [NotMapped]
        public Sec_Users User { get; set; }
    }
}
