using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Prop_Rmv_Reason
    {
        [Key]
        public int Prop_Rmv_Reason_Id { get; set; }
        public string Reason_Name { get; set; }
        public bool Active { get; set; } = true;

        /// <summary>
        /// If true, approval is required by all in the reason approval list
        /// </summary>
        public bool Req_Special_Approval { get; set; } = false;

    }
}
