using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Auto_Rpt_Item
    {
        public long Item_Id { get; set; }
        public string Item_Name { get; set; }
        public string Item_Description { get; set; }
        public string Item_Value { get; set; }
        public Auto_Rpt_Type Report_Type { get; set; }
        public bool Copy_To_Exec_Folder { get; set; }
    }
}
