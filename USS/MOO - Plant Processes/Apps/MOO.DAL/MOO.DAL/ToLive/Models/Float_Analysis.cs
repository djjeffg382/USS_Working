using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Float_Analysis
    {
        /// <summary>
        /// The different components of this table
        /// </summary>
        public enum FloatAnalysisComponent
        {
            Si,
            Al,
            Ca,
            Mg,
            Mn
        }

        public enum FloatAnalysisType
        {
            /// <summary>
            /// Filter Cake
            /// </summary>
            FC,

            /// <summary>
            /// Float Feed
            /// </summary>
            FF,

            /// <summary>
            /// Float Tails
            /// </summary>
            FT
        }

        public DateTime Datex { get; set; }
        public byte Shift { get; set; }
        public FloatAnalysisType SType { get; set; }
        public decimal? Si { get; set; }
        public decimal? Al { get; set; }
        public decimal? Ca { get; set; }
        public decimal? Mg { get; set; }
        public decimal? Mn { get; set; }
        public DateTime? Rdate { get; set; }
        public string Ana_Inits { get; set; }
    }
}
