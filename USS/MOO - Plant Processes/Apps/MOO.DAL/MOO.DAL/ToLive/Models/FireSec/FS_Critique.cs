using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    /// <summary>
    /// New form for managers to fill out. Inserted into FS_Critique.
    /// </summary>
    public class FS_Critique
    {
        public decimal Post_ID { get; set; }

        [Required]
        public MOO.Plant Plant { get; set; }

        [Required]
        public MOO.PlantArea Location { get; set; }

        [Required]
        public DateTime DateEntered { get; set; }

        [Required]
        public int Critique_Type { get; set; }
        public string Entered_By { get; set; }

        [Required]
        public People Prepared_By { get; set; }

        public string Specific_Location { get; set; }
        public string Scenario_Desc { get; set; }

        [Required]
        public int Alarms_Report { get; set; }
        public string AlarmsReport_Desc { get; set; }

        [Required]
        public int Accountability { get; set; }
        public string Accountability_Desc { get; set; }

        [Required]
        public int Response { get; set; }
        public string Response_Desc { get; set; }

        [Required]
        public int UtilitiesUpDown { get; set; }
        public string UtilitiesUpDown_Desc { get; set; }

        [Required]
        [Range(1, 3,
                ErrorMessage = "Please answer SOP's question.")]
        public int SOPsUsed { get; set; }
        public string SOPs_Desc { get; set; }
        public string Oper_Recommandation { get; set; }
        public string Safety_Recommandation { get; set; }
        public string Training_Recommedation { get; set; }

        public int Action_Required { get; set; }
        public string Action_Required_Desc { get; set; }

        public int Resolved { get; set; }
        public string Resolved_Desc { get; set; }

        public DateTime? Last_Email_Sent { get; set; }
    }
}
