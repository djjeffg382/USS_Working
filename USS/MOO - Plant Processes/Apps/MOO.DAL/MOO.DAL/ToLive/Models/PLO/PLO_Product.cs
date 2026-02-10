using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class PLO_Product
    {
        public int Product_Id { get; set; }
        public string Product_Number { get; set; }
        public string Product_Name { get; set; }
        public int? Edi_Product_Id { get; set; }

    }
}
