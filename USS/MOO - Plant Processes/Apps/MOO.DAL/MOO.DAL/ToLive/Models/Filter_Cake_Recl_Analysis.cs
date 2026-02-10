using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Filter_Cake_Recl_Analysis
    {
        /// <summary>
        /// The different components of this table
        /// </summary>
        public enum FilterCakeReclaimComponent
        {
            Si,
            Al,
            Ca,
            Mg,
            Mn
        }
        public DateTime Sdate { get; set; }
        public byte Step { get; set; }
        public byte Intv { get; set; }
        public decimal? Si { get; set; }
        public decimal? Al { get; set; }
        public decimal? Ca { get; set; }
        public decimal? Mg { get; set; }
        public decimal? Mn { get; set; }
        public DateTime? Rdate { get; set; }
        public string Ana_Inits { get; set; }

    }
}
