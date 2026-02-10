using System.Threading.Tasks;
using MTC_Compression_Test_Import.Domain.Entities;

namespace MTC_Compression_Test_Import.Domain.Interfaces;

/// <summary>
/// Repository interface for compression test data persistence.
/// </summary>
public interface ICompressionRepository
{
    /// <summary>
    /// Inserts a compression batch (header and detail rows) into the database.
    /// </summary>
    /// <param name="result">The parsed compression result to insert.</param>
    /// <returns>The generated comp_id for the inserted batch.</returns>
    Task<int> InsertCompressionBatchAsync(CompressionParseResult result);

    /// <summary>
    /// Computes and updates the standard deviation, average, and comp values for a compression batch.
    /// </summary>
    /// <param name="compId">The comp_id of the batch to update.</param>
    /// <returns>The computed statistics.</returns>
    Task<CompressionStats> UpdateStdDevAsync(int compId);
}
