using Azure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Met_Agg_Plant2
    {
        public Met_Material Material { get; set; }
        public DateTime Datex { get; set; }
        public Met_DMY Dmy { get; set; }
        public decimal? Sched_Hours { get; set; }
        public decimal? Sched_Maint_Hours { get; set; }
        public decimal? Unsched_Maint_Hours { get; set; }
        public decimal? Imposed_Hours { get; set; }
        public decimal? East_To_Stockpile { get; set; }
        public decimal? East_To_Plant { get; set; }
        public decimal? West_To_Stockpile { get; set; }
        public decimal? West_To_Plant { get; set; }
        public decimal? Truck_To_West { get; set; }
        public decimal? Truck_From_West { get; set; }
        public decimal? Truck_To_East { get; set; }
        public decimal? Truck_From_East { get; set; }
        public decimal? Bin1 { get; set; }
        public decimal? Bin2 { get; set; }
        public decimal? Bin3 { get; set; }
        public decimal? Bin4 { get; set; }
        public decimal? Bin5 { get; set; }
        public decimal? Tank1 { get; set; }
        public decimal? Tank2 { get; set; }
        public decimal? Tank3 { get; set; }
        public decimal? Tank4 { get; set; }
        public decimal? Grp13_Fines { get; set; }
        public decimal? Filt_Hrs_1_1 { get; set; }
        public decimal? Filt_Hrs_1_2 { get; set; }
        public decimal? Filt_Hrs_2_1 { get; set; }
        public decimal? Filt_Hrs_2_2 { get; set; }
        public decimal? Filt_Hrs_4_1 { get; set; }
        public decimal? Filt_Hrs_4_2 { get; set; }
        public decimal? Filt_Hrs_4_3 { get; set; }
        public decimal? Filt_Hrs_4_4 { get; set; }
        public decimal? Filt_Hrs_4_5 { get; set; }
        public decimal? Filt_Hrs_5_1 { get; set; }
        public decimal? Filt_Hrs_5_2 { get; set; }
        public decimal? Filt_Hrs_5_3 { get; set; }
        public decimal? Filt_Hrs_5_4 { get; set; }
        public decimal? Filt_Hrs_5_5 { get; set; }
        public decimal? Filt_Tons_1 { get; set; }
        public decimal? Filt_Tons_2 { get; set; }
        public decimal? Filtsi { get; set; }
        public decimal? Filtal { get; set; }
        public decimal? Filtca { get; set; }
        public decimal? Filtmg { get; set; }
        public decimal? Filtmn { get; set; }
        public decimal? Filt270 { get; set; }
        public decimal? Filt500 { get; set; }
        public decimal? Filth2o { get; set; }
        public decimal? Reclaimh2o { get; set; }
        public decimal? Recl_Bal_W { get; set; }
        public decimal? Recl_Bal_E { get; set; }
        public decimal? Pellet_Bal { get; set; }
        public decimal? Trucked_Recl_Bal { get; set; }


        /// <summary>
        /// recreation of the inventory calc used in the Feed To Grate calculation in the met packages
        /// </summary>
        public decimal Inventory
        {
            get
            {
                if (Datex <= DateTime.Parse("10/20/2024"))
                {
                    return Recl_Bal_W.GetValueOrDefault(0) + Recl_Bal_E.GetValueOrDefault(0) -
                                Trucked_Recl_Bal.GetValueOrDefault(0) + (Bin3.GetValueOrDefault(0) / 100 * 5400) +
                                ((Bin4.GetValueOrDefault(0) + Bin5.GetValueOrDefault(0)) / 200 * 4500) +
                                ((Tank3.GetValueOrDefault(0) + Tank4.GetValueOrDefault(0)) * 108);
                }
                else
                {
                    //new calculation starting October 2024 includes bin 1 and bin 2
                    return Recl_Bal_W.GetValueOrDefault(0) + Recl_Bal_E.GetValueOrDefault(0) -
                                Trucked_Recl_Bal.GetValueOrDefault(0) +
                                ((.97993M * SquareDecimal(Bin1 / 100) + .00077M * (Bin1.GetValueOrDefault(0) / 100) + .00987M) * 900) +
                                ((.97993M * SquareDecimal(Bin2 / 100) + .00077M * (Bin2.GetValueOrDefault(0) / 100) + .00987M) * 1800) +
                                ((.97993M * SquareDecimal(Bin3 / 100) + .00077M * (Bin3.GetValueOrDefault(0) / 100) + .00987M) * 1800) +
                                ((.97993M * SquareDecimal(Bin4 / 100) + .00077M * (Bin4.GetValueOrDefault(0) / 100) + .00987M) * 2250) +
                                ((.97993M * SquareDecimal(Bin5 / 100) + .00077M * (Bin5.GetValueOrDefault(0) / 100) + .00987M) * 2250) +
                                ((Tank3.GetValueOrDefault(0) + Tank4.GetValueOrDefault(0)) * 108);
                }

            }
        }
        private static decimal SquareDecimal(decimal? val)
        {
            var x = Math.Pow((double)val.GetValueOrDefault(0), 2);
            return (decimal)x;
        }

    }
}
