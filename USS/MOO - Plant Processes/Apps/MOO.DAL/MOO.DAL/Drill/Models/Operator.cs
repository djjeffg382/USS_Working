using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Drill.Models
{
    public class Operator
    {
        public MOO.Plant Plant { get; set; }
        public int Operator_Id { get; set; }
        public string Employee_Number { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public bool Active { get; set; }


        public string Full_Name { get { return $"{First_Name} {Last_Name}"; } }


    }
}
