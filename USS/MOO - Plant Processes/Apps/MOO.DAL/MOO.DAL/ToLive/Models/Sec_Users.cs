using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    /// <summary>
    /// Model uses for the sec_users table
    /// </summary>
    public class Sec_Users
    {
        /// <summary>
        /// Primary key to the sec_users Table
        /// </summary>
        public int User_Id { get; set; } = -1;
        /// <summary>
        /// Windows AD Username
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Windows Domain
        /// </summary>
        public string Domain { get; set; }
        /// <summary>
        /// Active Flag
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// Date record was created
        /// </summary>
        public DateTime Created_Date { get; set; } = DateTime.Now;
        /// <summary>
        /// Windows AD user that created the record
        /// </summary>
        public string Created_By { get; set; }
        /// <summary>
        /// Email address of the user
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// First name of the user
        /// </summary>
        public string First_Name { get; set; }
        /// <summary>
        /// Last Name of the user
        /// </summary>
        public string Last_Name { get; set; }
        /// <summary>
        /// (Deprecated) Reference to old Oracle Forms login id
        /// </summary>
        public int Sy_User_Id { get; set; }
        /// <summary>
        /// User that last modified the record
        /// </summary>
        public string Modified_By { get; set; }
        /// <summary>
        /// (Deprecated) Last time Web form checked to see if AD Password was expiring.  This would display message to user if close to expiring
        /// </summary>
        public DateTime? Pw_Expire_Check { get; set; }
        /// <summary>
        /// Date of last edit
        /// </summary>
        public DateTime? Last_Edited_Date { get; set; } = DateTime.Now;
        /// <summary>
        /// Flag indicating if the user is no longer with the company
        /// </summary>
        public bool Terminated { get; set; }

        /// <summary>
        /// JSON String containing user options
        /// </summary>
        public string UserOptions { get; set; }

        private List<Sec_Role> _roles = null;

        [NotMapped]
        public string FullName
        {
            get
            {
                return $"{First_Name} {Last_Name}";
            }
        }


        /// <summary>
        /// List of roles the user is in
        /// </summary>
        [NotMapped]
        public List<Sec_Role> Roles
        {
            get
            {
                //we will use this as a get property so we will only make the DB call if this property is asked for
                if (_roles == null)
                {
                    _roles = MOO.DAL.ToLive.Services.Sec_RoleSvc.GetRolesByUser(this);
                }
                return _roles;
            }
        }


    }
}
