using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.LIMS.Models
{
    public class Sample
    {
        //note: I did not put every field from the DB in here.  Only put fields we may use
        public int Id_Numeric { get; set; }
        public string Id_Text { get; set; }
        public Phrase Status { get; set; }
        public bool Compared { get; set; }
        public bool On_Spec { get; set; }
        public DateTime? Login_Date { get; set; }
        public string Login_By { get; set; }
        public DateTime? Sampled_Date { get; set; }
        public DateTime? Recd_Date { get; set; }
        public DateTime? DateStarted { get; set; }
        public DateTime? Date_Completed { get; set; }
        public DateTime? Date_Authorised { get; set; }
        public string Authoriser { get; set; }
        public Sample_Point Sampling_Point { get; set; }
        public Phrase Sample_Type { get; set; }
        public Phrase Oil_Type { get; set; }
        public string Description { get; set; }
        public string Waybill { get; set; }
        public DateTime? Transfer_Date { get; set; }
        public DateTime? Collected_Date { get; set; }
        public string[] Tests { get; set; } = new string[] { "" };

    }
}
