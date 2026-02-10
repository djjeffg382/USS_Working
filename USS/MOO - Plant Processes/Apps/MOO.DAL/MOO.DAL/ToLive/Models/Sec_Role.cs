using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    /// <summary>
    /// model for the Sec_Role table
    /// </summary>
    public class Sec_Role
    {
        /// <summary>
        /// Primary key of the sec_role table
        /// </summary>
        public int Role_Id { get; set; } = -1;
        /// <summary>
        /// The name of the role
        /// </summary>
        public string Role_Name { get; set; }
        /// <summary>
        /// Description of the role
        /// </summary>
        public string Role_Description { get; set; }
        /// <summary>
        /// Date the role was created
        /// </summary>
        public DateTime Created_Date { get; set; }
        /// <summary>
        /// Windows AD that created the role
        /// </summary>
        public string Created_By { get; set; }
        /// <summary>
        /// The sec_application that this role belongs to
        /// </summary>
        public int? Application_Id { get; set; }
        /// <summary>
        /// Windows AD that last modified the record
        /// </summary>
        public string Modified_By { get; set; }
        /// <summary>
        /// Notes about the role
        /// </summary>
        public string Role_Notes { get; set; }

        /// <summary>
        /// Flag whether the role is active
        /// </summary>
        public bool Active { get; set; }

    }
}
