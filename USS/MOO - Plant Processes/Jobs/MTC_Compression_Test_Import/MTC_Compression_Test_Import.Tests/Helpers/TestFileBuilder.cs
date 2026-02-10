using System.Text;
using MTC_Compression_Test_Import.Domain.Entities;

namespace MTC_Compression_Test_Import.Tests.Helpers;

/// <summary>
/// Helper class to programmatically build compression test report content
/// for unit testing the parser.
/// </summary>
public class TestFileBuilder
{
    private readonly StringBuilder _content = new();
    private int? _testerId;
    private readonly List<DataRowSpec> _dataRows = new();

    /// <summary>
    /// Sets the Compression Tester identifier in the header.
    /// </summary>
    public TestFileBuilder WithTesterId(int testerId)
    {
        _testerId = testerId;
        return this;
    }

    /// <summary>
    /// Adds a custom header line.
    /// </summary>
    public TestFileBuilder WithHeaderLine(string line)
    {
        _content.AppendLine(line);
        return this;
    }

    /// <summary>
    /// Adds a standard header with the tester ID.
    /// </summary>
    public TestFileBuilder WithStandardHeader()
    {
        if (!_testerId.HasValue)
        {
            throw new InvalidOperationException("TesterId must be set before adding standard header.");
        }

        _content.AppendLine($"Report Header - Compression Tester # {_testerId}");
        _content.AppendLine("Some additional header info");
        _content.AppendLine();
        return this;
    }

    /// <summary>
    /// Adds a data row with the standard 6-column format.
    /// Columns are separated by 4+ spaces as expected by the parser.
    /// </summary>
    public TestFileBuilder WithDataRow(
        string timestamp,
        double statusFlag,
        double testNumber,
        double compressionLbs,
        double pelletNumber,
        double endOfOperation)
    {
        _dataRows.Add(new DataRowSpec(timestamp, statusFlag, testNumber, compressionLbs, pelletNumber, endOfOperation));
        return this;
    }

    /// <summary>
    /// Adds a standard start row (Col2=1, Col5>0, Col6=0).
    /// </summary>
    public TestFileBuilder WithStartRow(
        string timestamp,
        double testNumber,
        double compressionLbs,
        double pelletNumber)
    {
        return WithDataRow(timestamp, 1.0, testNumber, compressionLbs, pelletNumber, 0.0);
    }

    /// <summary>
    /// Adds a standard measurement row (Col2=1, Col6=0).
    /// </summary>
    public TestFileBuilder WithMeasurementRow(
        string timestamp,
        double testNumber,
        double compressionLbs,
        double pelletNumber)
    {
        return WithDataRow(timestamp, 1.0, testNumber, compressionLbs, pelletNumber, 0.0);
    }

    /// <summary>
    /// Adds an end-of-operation marker row (Col2=0, Col6=1).
    /// </summary>
    public TestFileBuilder WithEndOfOperationRow(
        string timestamp,
        double testNumber,
        double compressionLbs,
        double pelletNumber)
    {
        return WithDataRow(timestamp, 0.0, testNumber, compressionLbs, pelletNumber, 1.0);
    }

    /// <summary>
    /// Adds a raw line without formatting.
    /// </summary>
    public TestFileBuilder WithRawLine(string line)
    {
        _content.AppendLine(line);
        return this;
    }

    /// <summary>
    /// Builds the content string.
    /// </summary>
    public string Build()
    {
        var result = new StringBuilder(_content.ToString());

        foreach (var row in _dataRows)
        {
            // Format with 4+ spaces between columns as expected by the parser
            var line = $"{row.Timestamp}    {row.StatusFlag:F3}    {row.TestNumber:F3}    {row.CompressionLbs:F3}    {row.PelletNumber:F3}    {row.EndOfOperation:F3}";
            result.AppendLine(line);
        }

        return result.ToString();
    }

    /// <summary>
    /// Writes the content to a temporary file and returns the path.
    /// </summary>
    public string BuildToTempFile()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_report_{Guid.NewGuid():N}.txt");
        File.WriteAllText(tempPath, Build());
        return tempPath;
    }

    /// <summary>
    /// Creates a minimal valid report with the specified number of measurement rows.
    /// </summary>
    public static TestFileBuilder CreateValidReport(int testerId, double testNumber, int measurementCount)
    {
        var builder = new TestFileBuilder()
            .WithTesterId(testerId)
            .WithStandardHeader();

        var baseTime = new DateTime(2026, 1, 27, 9, 10, 0);

        for (var i = 0; i < measurementCount; i++)
        {
            var timestamp = baseTime.AddSeconds(i).ToString("MM/dd/yyyy HH:mm:ss");
            var compressionLbs = 200.0 + (i * 10); // Values > 90 for stats calculation
            var pelletNumber = i + 1;

            builder.WithMeasurementRow(timestamp, testNumber, compressionLbs, pelletNumber);
        }

        // Add end-of-operation marker
        var endTimestamp = baseTime.AddSeconds(measurementCount).ToString("MM/dd/yyyy HH:mm:ss");
        builder.WithEndOfOperationRow(endTimestamp, testNumber, 0.0, measurementCount);

        return builder;
    }

    private record DataRowSpec(
        string Timestamp,
        double StatusFlag,
        double TestNumber,
        double CompressionLbs,
        double PelletNumber,
        double EndOfOperation);
}
