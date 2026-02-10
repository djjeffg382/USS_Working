using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{ /// <summary>
  /// Data model for data coming from the Cat Visionlink API
  /// </summary>
    public sealed class Cat_Equipment_Fault
    {
        public string Eqp_Model { get; set; }
        public string Eqp_Id { get; set; }
        public string Eqp_Serial { get; set; }
        public MOO.Plant? Plant { get; set; }
        public string Wenco_Equip_Ident { get; set; }

        public DateTime Fault_Date { get; set; }
        public string CodeIdentifier { get; set; }
        public string CodeDescription { get; set; }
        public string CodeSeverity { get; set; }
        public string CodeSource { get; set; }
    }
}
