using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class CO_Dept
    {
        public int Dept_Id { get; set; }
        public string Dept_Name { get; set; }
        public MOO.Plant Plant { get; set; }
        public bool Enabled { get; set; } = true;

    }
}
