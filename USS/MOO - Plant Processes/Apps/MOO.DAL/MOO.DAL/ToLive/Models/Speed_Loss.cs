using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Speed_Loss
    {
        /// <summary>
        /// The PI AF Unique Id of the speed loss event
        /// </summary>
        [Key]
        public Guid Speed_Loss_Id { get; set; } = Guid.Empty;
        public MOO.Plant Plant { get; set; }
        public string Area { get; set; }
        public string Line { get; set; }

        /// <summary>
        /// start time of the speed loss event
        /// </summary>
        public DateTime Start_Date { get; set; }

        /// <summary>
        /// End time of the speed loss event
        /// </summary>
        public DateTime? End_Date { get; set; }

        /// <summary>
        /// Average Long Tons Per hour during the event
        /// </summary>
        public double? Avg_Ltph { get; set; }

        /// <summary>
        /// Amount of tonnage lost during the event
        /// </summary>
        public double? Loss_Tons { get; set; }
        public string Reason { get; set; }
        public string Comments { get; set; }

    }
}
