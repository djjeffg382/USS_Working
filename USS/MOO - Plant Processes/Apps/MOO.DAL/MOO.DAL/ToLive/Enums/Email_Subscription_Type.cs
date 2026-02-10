using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Enums
{
    public enum Email_Subscription_Type
    {
        Auto_Email_Group = 0,
        Manual_Subscribe = 1,
        PI_Group = 2,
        PI_Notification = 3  //this item will not be stored in the database it will be pulled from PI
    }
}
