using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Property_Removal_Line_Item
    {
        [Key]
        public decimal Form_Nbr { get; set; }
        [Key]
        public short Line_Item_Nbr { get; set; }
        public decimal? Quantity { get; set; }
        public string Line_Desc { get; set; }
        public DateTime? Est_Return_Date { get; set; }
        public string Po_Req_Mr_Pt_Rz { get; set; }
        public string Line_Comment { get; set; }
        public decimal? Req_Nbr { get; set; }
        public string Line_Release { get; set; }

    }
}
