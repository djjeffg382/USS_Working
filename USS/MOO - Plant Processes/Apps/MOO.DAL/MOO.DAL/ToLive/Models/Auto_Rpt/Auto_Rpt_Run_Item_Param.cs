using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Auto_Rpt_Run_Item_Param
    {
        public long Param_Id { get; set; }
        public string Param_Name { get; set; }
        public string Param_Value { get; set; }
        public long Item_Id { get; set; }
        public string Dateformat { get; set; }


    }
}
