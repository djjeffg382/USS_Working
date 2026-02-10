namespace MTC_Compression_Test_Import.Domain.Entities;

public sealed class CompressionStats
{
    public double Average { get; }
    public double StdDev { get; }
    public double Comp200 { get; }
    public double Comp300 { get; }

    public CompressionStats(double average, double stdDev, double comp200, double comp300)
    {
        Average = average;
        StdDev = stdDev;
        Comp200 = comp200;
        Comp300 = comp300;
    }
}
