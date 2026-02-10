using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.Blazor.Models
{
    /// <summary>
    /// class used for getting pi notifications from the MOO_API_Fx system
    /// </summary>
    internal class PiNotification
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsSubscribed { get; set; } = false;
        public string NotificationType { get; set; } = "";
        public string AFPath { get; set; } = "";
    }
}
