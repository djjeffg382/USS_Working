using MOO.DAL.ToLive.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Equipment_Unit_Daily_Totals
    {
        public Equipment_Master Equip { get; set; }
        public Equipment_Unit_Types UnitType { get; set; }
        public DateTime Unit_Date { get; set; }
        public decimal? Nbr_Of_Units { get; set; }

        //Proc_Ind was used back when we had Passport (prior to ERP).  This was used to send data to passport.  This is no longer needed
        //public string Proc_Ind { get; set; }  

    }
}
