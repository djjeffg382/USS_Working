using MOO.DAL.ToLive.Models;
using MOO.DAL.ToLive.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OM_Lab.Services
{
    public class CompTestService : ICompTestService
    {
        public async Task<List<Lab_Compression>> GetCompTestData(DateTime startDate, DateTime endDate)
        {
            // Calls into MOO.DAL Lab_CompressionSvc
            return await Lab_CompressionSvc.GetByDateAsync(startDate, endDate);
        }

        public async Task<List<Lab_Compression_Dtl>> GetCompTestDetails(int labCompressionId)
        {
            // Calls into MOO.DAL Lab_Compression_DtlSvc
            return await Lab_Compression_DtlSvc.GetByLabCompressionIdAsync(labCompressionId);
        }

        public async Task<int> UpdateCompTestAsync(Lab_Compression compTest)
        {
            // Calls into MOO.DAL Lab_CompressionSvc
            return await Lab_CompressionSvc.UpdateAsync(compTest);
        }
    }
}
