using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Drill.Models
{
    public class Equip
    {
        public MOO.Plant Plant { get; set; }
        public int Equip_Id { get; set; }
        public string Equip_Number { get; set; }
        public DrillSystem Drill_System { get; set; } = DrillSystem.Not_Set;
        public bool Active { get; set; } = true;

        /// <summary>
        /// How the equipment number is stored in the foreign program
        /// </summary>
        public string Reference_Id { get; set; }
    }
}
