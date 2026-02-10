using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class ScheduleAssign
    {
        public decimal Assign_Id { get; set; }
        /// <summary>
        /// What comments/text to show on schedueler
        /// </summary>
        public string Short_View { get; set; }

        /// <summary>
        /// Refers to sched_groups table. Assign to department or specific cases
        /// </summary>
        public decimal Group_Id { get; set; }
        /// <summary>
        /// Name of the schedule
        /// </summary>
        public string Sched_Name { get; set; }
        /// <summary>
        /// Start date for people that will be assigned
        /// </summary>
        public DateTime Start_Date { get; set; }
        /// <summary>
        /// End date for people that will be assigned
        /// </summary>
        public DateTime End_Date { get; set; }
        /// <summary>
        /// Text to show on hover/More inforation can be displayed here
        /// </summary>
        public string Long_View { get; set; }
        /// <summary>
        /// A string that holds person_id. Allows for editing of people. Will show selected people on edit form.
        /// </summary>
        public string People_Assign { get; set; }

    }
}
