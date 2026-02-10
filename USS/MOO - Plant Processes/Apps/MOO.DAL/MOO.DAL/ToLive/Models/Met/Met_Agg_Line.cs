using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Met_Agg_Line
    {
        public Met_Material Material { get; set; }
        public byte Line { get; set; }
        public DateTime Datex { get; set; }
        public Met_DMY Dmy { get; set; }
        public decimal? GbTons { get; set; }
        public decimal? PelTons { get; set; }
        public decimal? GrateHours { get; set; }
        public decimal? BdHours1 { get; set; }
        public decimal? BdHours2 { get; set; }
        public decimal? BdHours3 { get; set; }
        public decimal? BdHours4 { get; set; }
        public decimal? BdHours5 { get; set; }
        public decimal? BentLbs { get; set; }
        public decimal? GasMCF { get; set; }
        public decimal? FuelOilGals { get; set; }
        public decimal? CoalTons { get; set; }
        public decimal? WoodTons { get; set; }
        public decimal? B58 { get; set; }
        public decimal? B916 { get; set; }
        public decimal? B12 { get; set; }
        public decimal? B716 { get; set; }
        public decimal? B38 { get; set; }
        public decimal? B14 { get; set; }
        public decimal? B4m { get; set; }
        public decimal? B28 { get; set; }
        public decimal? Bm28 { get; set; }
        public decimal? A58 { get; set; }
        public decimal? A916 { get; set; }
        public decimal? A12 { get; set; }
        public decimal? A716 { get; set; }
        public decimal? A38 { get; set; }
        public decimal? A14 { get; set; }
        public decimal? A4m { get; set; }
        public decimal? A28 { get; set; }
        public decimal? Am28 { get; set; }
        public decimal? Comp { get; set; }
        public decimal? Comp200 { get; set; }
        public decimal? Comp300 { get; set; }
        public decimal? Comp_Std_Dev { get; set; }
        public decimal? PelFe { get; set; }
        public decimal? PelSio2 { get; set; }
        public decimal? PelAl { get; set; }
        public decimal? PelCa { get; set; }
        public decimal? PelMg { get; set; }
        public decimal? PelMn { get; set; }
        public decimal? Pel_Ton_Gas { get; set; }
        public decimal? Pel_Ton_Oil { get; set; }
        public decimal? Pel_Ton_Coal { get; set; }
        public decimal? Pel_Ton_Wood { get; set; }
        public decimal? Ltb { get; set; }
        public decimal? GbDrop { get; set; }
        public decimal? Eom_Lock { get; set; }
        public decimal? GbTons_Adj { get; set; }
        public decimal? PelTons_Adj { get; set; }
        public decimal? SchedHours { get; set; }
        public decimal? Reduce { get; set; }
        public decimal? Coal_Mmbtus { get; set; }
        public decimal? FuelOil_Mmbtus { get; set; }
        public decimal? Gas_Mmbtus { get; set; }
        public decimal? Wood_Mmbtus { get; set; }

        /// <summary>
        /// Before tumbles greater than 1/4 inch
        /// </summary>
        public decimal? BT_Plus14 { get
            {
                if (B58.HasValue && B916.HasValue && B12.HasValue && B716.HasValue && B38.HasValue && B14.HasValue)
                    return B58 + B916 + B12 + B716 + B38 + B14;
                else
                    return null;
            }
        }


        /// <summary>
        /// After tumbles greater than 1/4 inch
        /// </summary>
        public decimal? AT_Plus14
        {
            get
            {
                if (A58.HasValue && A916.HasValue && A12.HasValue && A716.HasValue && A38.HasValue && A14.HasValue)
                    return A58 + A916 + A12 + A716 + A38 + A14;
                else
                    return null;
            }
        }

    }
}
