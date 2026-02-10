using MOO.DAL.ToLive.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class FE_Report
    {
        public long Fe_Report_Id { get; set; }
        public FE_Type Fe_Type { get; set; }
        public DateTime Record_Date { get; set; }
        public Sec_Users Sec_User_Entered_By { get; set; }
        public double Temperature { get; set; }
        public string Wind_Direction { get; set; }
        public double Wind_Speed { get; set; }
        public string Observed_Weather { get; set; }
        public string Comments { get; set; }
        public DateTime Entered_Date { get; set; }
    }
}
