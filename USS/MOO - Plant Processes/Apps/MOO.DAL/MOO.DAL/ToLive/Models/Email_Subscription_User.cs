using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Email_Subscription_User
    {
        public int Subscriber_Id { get; set; }
        public int Email_Subscription_Id { get; set; }
        public string Username { get; set; }
        public string Email_Address { get; set; }
        public string Comments { get; set; }
    }
}
