using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class CO_User_Dept
    {
        public CO_Dept Dept { get; set; }
        public Sec_Users Sec_User { get; set; }
        /// <summary>
        /// Indicates if user will receive an email once a call off is entered for this department
        /// </summary>
        public bool Instant_Email { get; set; }
        /// <summary>
        /// Indicates whether a user will receive an email with the frequent flyer report
        /// </summary>
        public bool Freq_Flyer_Email { get; set; }
        /// <summary>
        /// Indicates whether a user receives a daily email of call offs for this department
        /// </summary>
        public bool Daily_Email { get; set; }

        /// <summary>
        /// indicates whether a user receives an email of call offs not verified
        /// </summary>
        public  bool CO_Verified_Email { get; set; }

    }
}
