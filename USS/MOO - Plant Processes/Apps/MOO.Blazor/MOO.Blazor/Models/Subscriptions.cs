using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.Blazor.Models
{
    internal class Subscriptions
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Directions { get; set; } = "";
        public bool IsSubscribed { get; set; } = false;
        public MOO.DAL.ToLive.Enums.Email_Subscription_Type SubscriptionType;
        public PiNotification? PiNotify { get; set; }
        public MOO.DAL.ToLive.Models.Email_Subscription Email_Subscription { get; set; } = new();
    }
}
