using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Email_Subscription
    {
        public int Email_Subscription_Id { get; set; }
        public string Subscription_Name { get; set; }
        public string Description { get; set; }
        public string Application { get; set; }
        public bool Hide_User_Emails { get; set; }
        public DateTime? Last_Used { get; set; }
        public string Tags { get; set; }
        //Manual subscribe was once just a field, this was moved to they subscription_type column
        public bool Manual_Subscribe { get { return Subscription_Type == Enums.Email_Subscription_Type.Manual_Subscribe; } }
        public string Directions { get; set; }
        public string Comments { get; set; }
        public ToLive.Enums.Email_Subscription_Type Subscription_Type { get; set; }
    }
}
