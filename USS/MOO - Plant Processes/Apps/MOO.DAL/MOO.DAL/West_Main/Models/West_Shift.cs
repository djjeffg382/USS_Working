using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.West_Main.Models
{
    public class West_Shift
    {
        public WestMainPlants Plant { get; set; }
        public long Id { get; set; }
        /// <summary>
        /// Timestamp the data was entered
        /// </summary>
        public DateTime TimeStamp { get; set; }
        /// <summary>
        /// The Year Month Day of the record
        /// </summary>
        public int Ymd { get; set; }
        /// <summary>
        /// Shift number of the record
        /// </summary>
        public short Shift { get; set; }
        
        public decimal Shift_Total { get; set; }
        public int N_Count { get; set; }
        public decimal Shift_Average
        {
            get
            {
                return Math.Round(Shift_Total / N_Count, 8);
            }
        }

    }
}
