using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;
using DAL = MOO.DAL.ToLive;

namespace OM_Lab.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileIndexXlsx : ControllerBase
    {
        /// <summary>
        /// Gets the Lab Index Of Files 
        /// </summary>
        [HttpGet]
        public IActionResult DownloadExcel()
        {
            // Generate or load your Excel file here
            byte[] fileBytes = System.IO.File.ReadAllBytes(@"\\mno-fs.mno.uss.com\FS-ROOT\Departments\Lab\MOO_Library\Index\Index of files 2025.xlsx");
            string fileName = "Index of files 2025.xlsx";

            return File(fileBytes, "application/vnd.ms-excel", fileName);
        }

    }
}
