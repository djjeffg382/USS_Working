using MOO.DAL.ToLive.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OM_Lab.Services
{
    public interface ICompTestService
    {
        Task<List<Lab_Compression>> GetCompTestData(DateTime startDate, DateTime endDate);
        Task<List<Lab_Compression_Dtl>> GetCompTestDetails(int labCompressionId);
        Task<int> UpdateCompTestAsync(Lab_Compression compTest);
    }
}
