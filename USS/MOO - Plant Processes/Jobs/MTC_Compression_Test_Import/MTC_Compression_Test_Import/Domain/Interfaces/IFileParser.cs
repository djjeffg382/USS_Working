using System.Threading;
using System.Threading.Tasks;
using MTC_Compression_Test_Import.Domain.Entities;

namespace MTC_Compression_Test_Import.Domain.Interfaces;

public interface IFileParser
{
    Task<CompressionParseResult> ParseAsync(string filePath, CancellationToken cancellationToken);
}
