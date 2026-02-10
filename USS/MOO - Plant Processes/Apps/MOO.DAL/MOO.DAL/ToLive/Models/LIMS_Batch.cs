using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.DAL.ToLive.Enums;

namespace MOO.DAL.ToLive.Models
{
    public class LIMS_Batch
    {
        public int Batch_Id { get; set; }
        public DateTime Created_Date { get; set; } = DateTime.Now;
        public DateTime Last_Edit_Date { get; set; } = DateTime.Now;
        public DateTime? Last_Instrument_Export { get; set; }
        public string Batch_Name { get; set; }
        public LIMS_Batch_Type Batch_Type { get; set; }
    }
}
