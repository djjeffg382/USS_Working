namespace MTC_Compression_Test_Import.Infrastructure.Parsing;

internal sealed class CompressionDataRow
{
    public string TimestampText { get; }
    public double Col2 { get; }
    public double Col3 { get; }
    public double Col4 { get; }
    public double Col5 { get; }
    public double Col6 { get; }

    public CompressionDataRow(
        string timestampText,
        double col2,
        double col3,
        double col4,
        double col5,
        double col6)
    {
        TimestampText = timestampText;
        Col2 = col2;
        Col3 = col3;
        Col4 = col4;
        Col5 = col5;
        Col6 = col6;
    }
}
