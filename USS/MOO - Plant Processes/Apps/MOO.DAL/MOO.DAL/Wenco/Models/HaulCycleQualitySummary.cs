using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Wenco.Models
{
    public class HaulCycleQualitySummary
    {
        public DateTime ShiftDate { get; set; }
        public byte Shift { get; set; }

        private readonly Dictionary<string, decimal> _qualityVals = new();

        public Dictionary<string,decimal> QualityVals { get { return _qualityVals; } }


        public decimal? Quality(string QualityName)
        {
            if (QualityVals.ContainsKey(QualityName))
               return QualityVals[QualityName];
            else
                return null;
        }
    }
}
