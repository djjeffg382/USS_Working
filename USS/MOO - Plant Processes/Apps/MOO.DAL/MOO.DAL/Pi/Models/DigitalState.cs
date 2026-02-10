using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Pi.Models
{
    public class DigitalState
    {
        public string DigitalSet { get; set; }
        public int Code { get; set; }


        private readonly Dictionary<string, int> _digValues = new();
        public Dictionary<string, int> DigValues { get { return _digValues; } }


        public int? DigValue(string QualityName)
        {
            if (_digValues.ContainsKey(QualityName))
                return _digValues[QualityName];
            else
                return null;
        }
    }
}
