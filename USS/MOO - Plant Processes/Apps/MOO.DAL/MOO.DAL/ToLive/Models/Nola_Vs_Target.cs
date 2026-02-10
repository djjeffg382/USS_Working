using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Nola_Vs_Target
    {
        public enum NvtStatus
        {
            [Display(Name = "Conc Entry")]
            [Description("Awaiting Concentrator Entry")]
            Conc_Entry,

            [Display(Name = "Mine Entry")]
            [Description("Awaiting Mine Entry")]
            Mine_Entry,


            [Display(Name = "Mgmt Review")]
            [Description("Awaiting Management Review")]
            Mgmt_Review,

            [Display(Name = "Complete")]
            [Description("Complete")]
            Complete,


            [Display(Name = "Historical")]
            [Description("Historical")]
            Historical

        }

        private DateTime _theDate;

        [Key]
        public DateTime TheDate
        {
            get
            {
                return _theDate;
            }
            set
            {
                //need to make sure we truncate off any milliseconds as this is used as a key
                _theDate = new DateTime(
                            value.Year,
                            value.Month,
                            value.Day,
                            value.Hour,
                            value.Minute,
                            value.Second,
                            value.Kind
                    );
            }
        }




        [Key]
        public short Step { get; set; }

        public bool Is_Out_Of_Control { get; set; } = false;
        public bool Nola_Reliable { get; set; } = false;
        public bool Conc_Checked { get; set; } = false;
        public DateTime? Conc_Response_Date { get; set; }
        public string Conc_Responded_By { get; set; }
        public string Conc_Comments { get; set; }
        public bool Solved_At_Conc { get; set; } = false;
        public DateTime? Sent_To_Mine_Date { get; set; }
        public DateTime? Mine_Response_Date { get; set; }
        public string Mine_Responded_By { get; set; }
        public string Mine_Actions { get; set; }
        public bool Solved_At_Mine { get; set; } = false;
        public string Mgmt_Review_By { get; set; }
        public string Mgmt_Review_Comments { get; set; }
        public DateTime? Mgmt_Review_Date { get; set; }

        [NotMapped]
        public NvtStatus Status { get
            {
                if (!Solved_At_Conc && !Sent_To_Mine_Date.HasValue)
                    return NvtStatus.Conc_Entry;
                if (!Solved_At_Mine && Sent_To_Mine_Date.HasValue)
                    return NvtStatus.Mine_Entry;
                if ((Solved_At_Conc || Solved_At_Mine) && string.IsNullOrEmpty(Mgmt_Review_By))
                    return NvtStatus.Mgmt_Review;
                if (DateTime.Now.Subtract(TheDate).TotalDays >= 365)
                    return NvtStatus.Historical;

                return NvtStatus.Complete;
            }
        }

    }
}
