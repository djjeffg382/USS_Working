using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Enums
{
    public enum SecWebMenuType
    {
        Link = 0,
        Folder_Location = 1,
        [Obsolete("This Was Used for Old Oracle Forms")]
        SY_Program = 2,
        [Obsolete("PI Displays Only Worked in Old Internet Explorer")]
        PI_Display = 3
    }
}
