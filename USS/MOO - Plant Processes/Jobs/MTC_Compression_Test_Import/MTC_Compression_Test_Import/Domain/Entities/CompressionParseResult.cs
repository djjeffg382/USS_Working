using System.Collections.Generic;

namespace MTC_Compression_Test_Import.Domain.Entities;

public sealed class CompressionParseResult
{
    public string SourceFile { get; }
    public int InstrumentId { get; }
    public double TestNumber { get; }
    public IReadOnlyList<CompressionDataPoint> DataPoints { get; }
    public double LastPelletNumber { get; }
    public bool EndOfOperationIsOne { get; }

    public CompressionParseResult(
        string sourceFile,
        int instrumentId,
        double testNumber,
        IReadOnlyList<CompressionDataPoint> dataPoints,
        double lastPelletNumber,
        bool endOfOperationIsOne)
    {
        SourceFile = sourceFile;
        InstrumentId = instrumentId;
        TestNumber = testNumber;
        DataPoints = dataPoints;
        LastPelletNumber = lastPelletNumber;
        EndOfOperationIsOne = endOfOperationIsOne;
    }
}
