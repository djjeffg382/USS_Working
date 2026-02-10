using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;
using DAL = MOO.DAL.ToLive;

namespace OM_Lab.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BatchSampleRpt : ControllerBase
    {
        /// <summary>
        /// This controller will download the Sample Batch Report from SSRS and stream it back as an application/pdf type
        /// this will make it so that the report shows in the browser rather than as a download.
        /// </summary>
        /// <param name="BatchNbr"></param>
        /// <returns></returns>
        [HttpGet, Route("{BatchNbr}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> StreamReport(int BatchNbr)
        {

            string url = $"http://mno-reports.mno.uss.com/ReportServer?/Minntac/Lab/Sample%20Batch%20List&LimsBatchId={BatchNbr}&rs:Format=PDF";
            

            using HttpClient hc = new(new HttpClientHandler() { UseDefaultCredentials =true});

            var httpResult = await hc.GetAsync(url);

           

            using var resultStream = await httpResult.Content.ReadAsStreamAsync();

            var buffer = new byte[resultStream.Length];
            resultStream.Read(buffer, 0, buffer.Length);

            MemoryStream memStream = new(buffer);

            var result = new FileStreamResult(memStream, "application/pdf");

            return result;

        }
    }
}
