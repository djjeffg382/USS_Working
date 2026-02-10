using Microsoft.AspNetCore.Mvc;
using System.Text;
using DAL = MOO.DAL.ToLive;

namespace OM_Lab.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WoodShipmentExport : ControllerBase
    {

        [HttpGet, Route("{StartDate}/{EndDate}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Get(DateTime StartDate, DateTime EndDate)
        {
            StringBuilder sb = new();

            decimal accum = 0;

            var wtsList = await Task.Run(() => DAL.Services.Wood_Truck_ShipmentsSvc.GetPagedData(0, int.MaxValue, "delivery_date, invoice_nbr", "asc", "", StartDate, EndDate));
            foreach (var wt in wtsList.Data)
            {
                sb.Append(wt.Invoice_Nbr);
                sb.Append(", " + wt.Delivery_Date!.Value.ToString("MMM-dd-yyyy") );
                sb.Append("," + wt.Inbound_Wt.GetValueOrDefault(0).ToString().PadLeft(8));
                sb.Append("," + wt.Outbound_Wt.GetValueOrDefault(0).ToString().PadLeft(8));
                decimal netWgt = wt.Inbound_Wt.GetValueOrDefault(0) - wt.Outbound_Wt.GetValueOrDefault(0);
                sb.Append("," + netWgt.ToString().PadLeft(8));
                sb.Append(", " + wt.Avg_Perc_Moist.GetValueOrDefault(0).ToString("F1").PadLeft(8));
                decimal dryNetWgt = netWgt * (1 - (wt.Avg_Perc_Moist.GetValueOrDefault(0) * .01M));
                sb.Append("," + dryNetWgt.ToString("F0").PadLeft(8));
                sb.Append("," + wt.Btu_Lb.GetValueOrDefault(0).ToString().PadLeft(11));
                decimal btuTruck = Math.Round(dryNetWgt * wt.Btu_Lb.GetValueOrDefault(0) / 1000000M,2,MidpointRounding.AwayFromZero);
                sb.Append("," + btuTruck.ToString().PadLeft(10));
                accum += btuTruck;
                sb.Append(",    " + accum.ToString());
                sb.AppendLine();

            }

            var buffer = Encoding.UTF8.GetBytes(sb.ToString());
            var stream = new MemoryStream(buffer);
            //var stream = new FileStream(filename);

            var result = new FileStreamResult(stream, "text/plain")
            {
                FileDownloadName = $"Export.txt"
            };
            return result;
        }
    }
}
