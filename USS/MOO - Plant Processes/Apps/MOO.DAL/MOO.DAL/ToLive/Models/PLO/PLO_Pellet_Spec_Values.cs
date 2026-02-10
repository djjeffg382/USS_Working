using MOO.DAL.ToLive.Enums;
using MOO.Enums.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class PLO_Pellet_Spec_Values
    {
       

        public long Plo_Pellet_Spec_Values_Id { get; set; }
        public PLO_Pellet_Spec Plo_Pellet_Spec { get; set; }
        public PLO_Spec_Name Spec { get; set; }
        public int Sort_Order { get => Spec.GetDisplay().Order; }
        public double? Typical_Value { get; set; }
        public double? Min_Value { get; set; }
        public double? Max_Value { get; set; }

    }
}
