using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Core.Models
{
    public class Collection_Type
    {
        public int Coll_Type_Id { get; set; }
        public string Coll_Type_Name { get; set; }
        public string Description { get; set; }
        public bool Manually_Entered { get; set; }

    }
}
