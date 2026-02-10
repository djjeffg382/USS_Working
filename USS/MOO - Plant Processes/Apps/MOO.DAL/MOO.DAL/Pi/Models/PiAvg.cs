using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.DAL.Pi.Enums;

namespace MOO.DAL.Pi.Models
{
    public class PiAvg : PiAggregate
    {
        public PiCalcBasis CalcBasis { get; set; }
    }
}
