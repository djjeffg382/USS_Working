using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Corporate_Delays
    {
        public Corporate_Delay_Types Delay_Type { get; set; }
        public DateTime Start_Of_Delay { get; set; }
        public DateTime? End_Of_Delay { get; set; }
        public decimal Delay_Id { get; set; }

    }
}
