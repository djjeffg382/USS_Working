using MOO.DAL.ToLive.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class FE_K_Mine
    {
        public long Fe_K_Mine_Id { get; set; }
        public long Fe_Report_Id { get; set; }
        public string Source { get; set; }
        public string Equip { get; set; }
        public bool Equip_Operating { get; set; }
        public Steam_Plume? Steam_Plume { get; set; }
        public bool Req_Attention { get; set; }
        public string Comments { get; set; }
        public string Emission_Code { get; set; }
        public int Sort_Order { get; set; }
        public string Fe_Category { get; set; }
    }
}
