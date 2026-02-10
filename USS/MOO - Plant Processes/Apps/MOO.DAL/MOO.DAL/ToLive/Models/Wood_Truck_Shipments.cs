using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class Wood_Truck_Shipments
    {
        /// <summary>
        /// The invoice number stored for updating
        /// </summary>
        ///<remarks>Invoice_Nbr is the primary key and we need to allow updating, therefore we need to store the old invoice number if the user chooses to change the invoice number
        ///This value will not be exposed publicly</remarks>
        internal string Old_Invoice_Nbr { get; set; }
        public string Invoice_Nbr { get; set; }
        public DateTime? Delivery_Date { get; set; }
        public decimal? Inbound_Wt { get; set; }
        public decimal? Outbound_Wt { get; set; }
        public decimal? Avg_Perc_Moist { get; set; }
        public decimal? Btu_Lb { get; set; }
        public decimal? Scale_Ticket_Number { get; set; }

    }
}
