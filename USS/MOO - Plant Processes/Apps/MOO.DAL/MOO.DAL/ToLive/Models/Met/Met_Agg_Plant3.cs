using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Met_Agg_Plant3
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
        public decimal? Bin7 { get; set; }
        public decimal? Bin6 { get; set; }
        public decimal? Tank6 { get; set; }
        public decimal? Tank7 { get; set; }
        public decimal? Grp13_Fines { get; set; }
        public decimal? Filt_Hrs_1 { get; set; }
        public decimal? Filt_Hrs_2 { get; set; }
        public decimal? Filt_Hrs_3 { get; set; }
        public decimal? Filt_Hrs_4 { get; set; }
        public decimal? Filt_Hrs_5 { get; set; }
        public decimal? Filt_Hrs_6 { get; set; }
        public decimal? Filt_Hrs_7 { get; set; }
        public decimal? Filt_Hrs_8 { get; set; }
        public decimal? Filt_Hrs_9 { get; set; }
        public decimal? Filt_Hrs_10 { get; set; }
        public decimal? Filt_Hrs_11 { get; set; }
        public decimal? Filt_Hrs_12 { get; set; }
        public decimal? Filt_Tons_3 { get; set; }
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
                    return Recl_Bal_W.GetValueOrDefault(0) -
                                Trucked_Recl_Bal.GetValueOrDefault(0) +
                                ((Bin6.GetValueOrDefault(0) + Bin7.GetValueOrDefault(0)) / 200 * 4500) +
                                (Tank6.GetValueOrDefault(0) + Tank7.GetValueOrDefault(0)) * 108;
                }
                else
                {
                    //new calculation starting October 2024 
                    return (Recl_Bal_W.GetValueOrDefault(0) -
                                Trucked_Recl_Bal.GetValueOrDefault(0)) +
                                ((.97993M * SquareDecimal(Bin6 / 100) + .00077M * (Bin6.GetValueOrDefault(0) / 100) + .00987M) * 2250) +
                                ((.97993M * SquareDecimal(Bin7 / 100) + .00077M * (Bin7.GetValueOrDefault(0) / 100) + .00987M) * 2250) +
                                ((Tank6.GetValueOrDefault(0) + Tank7.GetValueOrDefault(0)) * 108);

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
