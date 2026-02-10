using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Equipment_Master
    {
        public string Equip_Id { get; set; }
        public string Equip_Desc { get; set; }
        public string Plant { get; set; }
        public string Area { get; set; }
        public string Equip_Group { get; set; }
        public string Equip_Type { get; set; }
        public decimal? Initial_Meter_Reading { get; set; }


        /*
         * following fields are not used anymore so we won't even show them
        public string Source { get; set; }
        public string Source_Id { get; set; }
        public string Passport_Ind { get; set; }
        public string Passport_Id { get; set; }
        */

    }
}
