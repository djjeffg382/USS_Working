using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Prop_Rmv_Form_Line
    {
        [Key]
        public int Prop_Rmv_Form_Line_Id { get; set; }
        public int Prop_Rmv_Form_Id { get; set; }
        public short Item_Nbr { get; set; }
        public double Quantity { get; set; }
        public string Line_Description { get; set; }
        public string Line_Comment { get; set; }
        public DateTime? Est_Return_Date { get; set; }
        public string Mr_Pr_Pt_Rz { get; set; }
        public string Po_Nbr { get; set; }
        public string Line_Release { get; set; }

    }
}