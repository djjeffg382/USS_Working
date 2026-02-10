using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.West_Main.Models
{
    public class West_Daily
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
        
        public decimal Day_Total { get; set; }
        public int N_Count { get; set; }
        public decimal Day_Average
        {
            get
            {
                return Math.Round(Day_Total / N_Count, 8);
            }
        }

    }
}
