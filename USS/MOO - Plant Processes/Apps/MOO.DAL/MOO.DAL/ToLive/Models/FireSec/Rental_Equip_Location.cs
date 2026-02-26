using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Rental_Equip_Location
    {
        [Key]
        public MOO.Plant PlantLoc { get; set; }
        [Key]
        public string Location { get; set; }

        /// <summary>
        /// Semi-colon delimited list of email addresses to send notifications to when rental equipment is at this location.  
        /// This is used for the rental equipment notification process that runs daily and sends out notifications for any rental equipment that has not been inspected for 30 days or more.
        /// </summary>
        public string Email_List { get; set; }

        [NotMapped]
        public  List<string> Emails_As_List { get
            {
                if(string.IsNullOrEmpty(Email_List))
                {
                    return [];
                }
                var eList = Email_List.Split(';').ToList();
                return eList;
            }
            set {
                Email_List = string.Join(";", value);
            } 
        }
    }
}
