using MOO.DAL.ERP.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CA1822 // Mark members as static
namespace MOO.DAL.ERP.Models
{
    /// <summary>
    /// This is the header part of the message
    /// </summary>
    /// <remarks>See "Q:\Departments\BSC\Plant Processes\Systems\Applications\ERP\ERP Production\Mine Production Recording v1.0.xls" for defintions</remarks>
    public sealed class MSTRec
    {
        public MSTRec(MOO.Plant Plant, DateTime StartDate, DateTime EndDate, TimePeriod Period)
        {
            this.Plant = Plant;
            this.PeriodStartDate = StartDate;
            this.PeriodStopDate = EndDate;
            this.PeriodUnitOfMeasure = Period;
        }


        public MOO.Plant Plant { get; set; }

        public string StartOfRecord { get { return "MST:"; } }
        public string InterfaceType { get { return "MINE_PRODUCTION"; } }
        public string InterfaceLayout { get { return "STANDARD"; } }
        public string InterfaceVersion { get { return "0001"; } }
        public DateTime InterfaceTimestamp { get; set; } = DateTime.Now;
        public string InterfaceEnvironment { get { return "DEV"; } }  //the old program always sent dev so we will just send the same
        public string ActionCode { get { return "U"; } }

        /// <summary>
        /// Field8 will either be MINES_MTC or MINES_KTC based on the plant
        /// </summary>
        public string DataSource
        {
            get
            {
                return Plant == MOO.Plant.Minntac ? "MINES_MTC" : "MINES_KTC";
            }
        }

        public string ProgramId { get { return "MOO_PROD_RECORDS"; } }
        public string UserId { get { return "SYSTEM"; } }
        public string TransactionId { get { return ""; } }
        public string TransactionQualifier { get { return ""; } }

        /// <summary>
        /// Field13 will either be MTC or KTC based on the plant
        /// </summary>
        public string PlantId
        {
            get
            {
                return Plant == MOO.Plant.Minntac ? "MTC" : "KTC";
            }
        }
        public DateTime PeriodStartDate { get; set; }
        public DateTime PeriodStopDate { get; set; }
        public TimePeriod PeriodUnitOfMeasure { get; set; } //D for daily M for monthly
        public string Turn { get { return ""; } }
    }
}

#pragma warning restore CA1822 // Mark members as static
