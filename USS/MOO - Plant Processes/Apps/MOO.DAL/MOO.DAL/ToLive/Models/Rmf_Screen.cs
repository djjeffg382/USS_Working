using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Rmf_Screen
    {
        
        public DateTime Shift_Date { get; set; }
        public short Shift { get; set; }
        public short Step { get; set; }
        public decimal? Start_Wgt { get; set; }


        public decimal? Scn_1_Inch { get; set; }

        public decimal? Scn_1_InchPct
        {
            get
            {
                if (Start_Wgt.HasValue && Scn_1_Inch.HasValue && Start_Wgt.Value > 0)
                    return Scn_1_Inch.Value / Start_Wgt.Value * 100;
                else
                    return null;
            }
        }
        public decimal? Scn_1_Cumulative
        {
            get
            {
                return Scn_3_4_Cumulative + Scn_3_4_InchPct;
            }
        }


        public decimal? Scn_3_4_Inch { get; set; }
        public decimal? Scn_3_4_InchPct
        {
            get
            {
                if (Start_Wgt.HasValue && Scn_3_4_Inch.HasValue && Start_Wgt.Value > 0)
                    return Scn_3_4_Inch.Value / Start_Wgt.Value * 100;
                else
                    return null;
            }
        }
        public decimal? Scn_3_4_Cumulative
        {
            get
            {
                return Scn_1_2_Cumulative + Scn_1_2_InchPct;
            }
        }

        public decimal? Scn_1_2_Inch { get; set; }
        public decimal? Scn_1_2_InchPct
        {
            get
            {
                if (Start_Wgt.HasValue && Scn_1_2_Inch.HasValue && Start_Wgt.Value > 0)
                    return Scn_1_2_Inch.Value / Start_Wgt.Value * 100;
                else
                    return null;
            }
        }

        public decimal? Scn_1_2_Cumulative
        {
            get
            {
                return Scn_1_4_Cumulative + Scn_1_4_InchPct;
            }
        }

        public decimal? Scn_1_4_Inch { get; set; }
        public decimal? Scn_1_4_InchPct
        {
            get
            {
                if (Start_Wgt.HasValue && Scn_1_4_Inch.HasValue && Start_Wgt.Value > 0)
                    return Scn_1_4_Inch.Value / Start_Wgt.Value * 100;
                else
                    return null;
            }
        }
        public decimal? Scn_1_4_Cumulative
        {
            get
            {
                return Scn_6m_Pct + Scn_6m_Cumulative;
            }
        }


        public decimal? Scn_6m { get; set; }
        public decimal? Scn_6m_Pct
        {
            get
            {
                if (Start_Wgt.HasValue && Scn_6m.HasValue && Start_Wgt.Value > 0)
                    return Scn_6m.Value / Start_Wgt.Value * 100;
                else
                    return null;
            }
        }
        public decimal? Scn_6m_Cumulative
        {
            get
            {
                return Scn_Minus_6m_Pct;
            }
        }

        public decimal? Scn_Minus_6m { get; set; }
        public decimal? Scn_Minus_6m_Pct
        {
            get
            {
                if (Start_Wgt.HasValue && Scn_Minus_6m.HasValue && Start_Wgt.Value > 0)
                    return Scn_Minus_6m.Value / Start_Wgt.Value * 100;
                else
                    return null;
            }
        }

        /// <summary>
        /// sum of all of the weights
        /// </summary>
        public decimal CumulativeWeight
        {
            get
            {
                return TrimValue(Scn_1_Inch.GetValueOrDefault(0)) + TrimValue(Scn_3_4_Inch.GetValueOrDefault(0)) +
                            TrimValue(Scn_1_2_Inch.GetValueOrDefault(0)) + TrimValue(Scn_1_4_Inch.GetValueOrDefault(0)) +
                            TrimValue(Scn_6m.GetValueOrDefault(0)) + TrimValue(Scn_Minus_6m.GetValueOrDefault(0));
            }
        }

        /// <summary>
        /// trims the weight values to one decimal before saving to the database
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private static decimal TrimValue(decimal val)
        {
            return Math.Floor(val * 10) / 10;
        }

    }
}
