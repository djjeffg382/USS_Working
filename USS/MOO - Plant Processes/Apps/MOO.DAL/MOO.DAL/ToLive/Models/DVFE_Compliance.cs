using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    /// <summary>
    /// model for tolive.DVFE_Compliance table
    /// </summary>
    public sealed class DVFE_Compliance
    {

        public enum Facility
        {
            Minntac = 1,
            Keetac = 2
        }

        public enum Plant
        {
            Agglomerator1_2 = 1,
            Agglomerator3 = 2,
            Crusher
        }

        public long DVFE_Key { get; set; }
        /// <summary>
        /// Date of the DVFE record (truncated to day level)
        /// </summary>
        public DateTime RegisterDate { get; set; }

        public Facility Facility_Id { get; set; }
        public Plant Plant_Id { get; set; }
        /// <summary>
        /// Username entering the record
        /// </summary>
        public string WebLoginName { get; set; }

        /// <summary>
        /// Username entering the record
        /// </summary>
        public string NTUserName { get; set; }
        /// <summary>
        /// IP Address of the PC entering the record
        /// </summary>
        public string ChangedFromIp { get; set; }
        public DateTime RegisterDateFull { get; set; }
    }
}
