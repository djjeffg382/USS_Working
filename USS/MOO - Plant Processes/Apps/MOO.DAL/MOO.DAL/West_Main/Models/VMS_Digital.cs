using MOO.DAL.West_Main.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.West_Main.Models
{
    public sealed class VMS_Digital
    {
        public WestMainPlants Plant { get; set; }
        public short Point_Id { get; set; }
        public string Point_Name { get; set; }
        public string Description { get; set; }
        public DateTime Last_Update { get; set; }
        public string Pi_Pnt_Type { get; set; }

        /// <summary>
        /// Gets all analog points dependent on this point
        /// </summary>
        /// <param name="DigPoint"></param>
        /// <returns></returns>
        public List<VMS_Analog> GetDependencies()
        {
            return VMS_AnalogSvc.GetDependencies(this);
        }
    }
}
