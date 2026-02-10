using System;

namespace MTC_Compression_Test_Import.Domain.Entities;

public sealed class CompressionDataPoint
{
    public DateTime Timestamp { get; }
    public double CompressionLbs { get; }
    public int PelletNumber { get; }

    public CompressionDataPoint(DateTime timestamp, double compressionLbs, int pelletNumber)
    {
        Timestamp = timestamp;
        CompressionLbs = compressionLbs;
        PelletNumber = pelletNumber;
    }
}
