using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Pi.Models
{
    /// <summary>
    /// This class will be used as a base class for all PI Aggregates
    /// piavg 
    /// picount 
    /// pimax 
    /// pimean 
    /// pimin 
    /// pipstd 
    /// pirange
    /// pistd 
    /// pitotal 
    /// picalc
    /// </summary>
    public abstract class PiAggregate
    {
        

        public string Tag { get; set; }
        public string Expr { get; set; }
        public string FilterExpr { get; set; }
        public string FilterSampleType { get; set; }
        public string FilterSampleInterval { get; set; }
        public DateTime Time { get; set; }
        public TimeSpan TimeStep { get; set; }
        public double? Value { get; set; }
        public double? PctGood { get; set; }


    }
}
