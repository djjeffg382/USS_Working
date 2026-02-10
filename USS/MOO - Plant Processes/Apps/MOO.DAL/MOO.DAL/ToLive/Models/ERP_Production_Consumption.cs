using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class ERP_Production_Consumption
    {
        public bool Sent_To_Erp { get; set; }
        public DateTime Month_Date { get; set; }
        public MOO.Plant Loc { get; set; }
        public DateTime Sent_Date { get; set; }
        /// <summary>
        /// Limestone Consumption
        /// </summary>
        public decimal? Lime_Cons { get; set; }

        /// <summary>
        /// Concentrate Production
        /// </summary>
        public decimal? Conc_Prod { get; set; }

        /// <summary>
        /// Agglomerator Flux Production
        /// </summary>
        public decimal? Aggf_Prod { get; set; }

        /// <summary>
        /// Agglomerator Acid Production
        /// </summary>
        public decimal? Agga_Prod { get; set; }

        /// <summary>
        /// Keetac K1 Pellet (Blast Furnace Pellet) production
        /// </summary>
        public decimal? Aggk1_Prod { get; set; }

        /// <summary>
        /// Crusher Production
        /// </summary>
        public decimal? Crush_Prod { get; set; }

        /// <summary>
        /// Concentrator Consumption
        /// </summary>
        public decimal? Conc_Cons { get; set; }

        /// <summary>
        /// Agglomerator Flux Consumption
        /// </summary>
        public decimal? Aggf_Cons { get; set; }

        /// <summary>
        /// Agglomerator Acid Consumption
        /// </summary>
        public decimal? Agga_Cons { get; set; }

        /// <summary>
        /// Keetac K1 Pellet (Blast Furnace Pellt) production
        /// </summary>
        public decimal? Aggk1_Cons { get; set; }

        /// <summary>
        /// Crusher Consumption
        /// </summary>
        public decimal? Crush_Cons { get; set; }

        /// <summary>
        /// Full ERP Message for the month
        /// </summary>
        public string ERP_Message { get; set; }

        /// <summary>
        /// DR Concentrate Consumption
        /// </summary>
        public long? DRCon_Cons { get; set; }

        /// <summary>
        /// DR Pellet Consumption
        /// </summary>
        public long? DRPell_Cons { get; set; }


        /// <summary>
        /// DR Concentrate Production
        /// </summary>
        public long? DRCon_Prod { get; set; }

        /// <summary>
        /// DR Pellet Production
        /// </summary>
        public long? DRPell_Prod { get; set; }


    }
}
