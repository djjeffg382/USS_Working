using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.MinVu.Models
{
    public class Forecast
    {
        public static readonly int[] TagMtcDrill = new int[] { 691 };
        public static readonly int[] TagMtcOre = new int[] { 533, 690 };
        public static readonly int[] TagMtcWaste = new int[] { 717, 718 };
        public static readonly int[] TagMtcOMH = new int[] { 713 };
        public static readonly int[] TagMtcStripping = new int[] { 719 };


        public static readonly int[] TagKtcDrill = new int[] { 18, 5 };
        public static readonly int[] TagKtcOre = new int[] { 1 };
        public static readonly int[] TagKtcWaste = new int[] { 8 };
        public static readonly int[] TagKtcOMH = new int[] { 55 };

        public DateTime ShiftDate { get; set; }
        public byte Shift { get; set; }
        public double ForecastValue { get; set; }
    }
}