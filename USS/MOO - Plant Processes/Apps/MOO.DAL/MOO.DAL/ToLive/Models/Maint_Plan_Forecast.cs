using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Maint_Plan_Forecast
    {
        public DateTime Forecast_Date { get; set; }
        public decimal? Crush_Tons { get; set; }
        public decimal? Conc_Lines { get; set; }
        public decimal? Aggl_Tons { get; set; }

    }
}
