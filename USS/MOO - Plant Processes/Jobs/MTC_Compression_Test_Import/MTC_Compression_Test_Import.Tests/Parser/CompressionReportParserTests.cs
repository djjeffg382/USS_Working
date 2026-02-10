using FluentAssertions;
using MTC_Compression_Test_Import.Domain.Entities;
using MTC_Compression_Test_Import.Infrastructure.Parsing;
using MTC_Compression_Test_Import.Tests.Helpers;

namespace MTC_Compression_Test_Import.Tests.Parser;

/// <summary>
/// Unit tests for CompressionReportParser.ParseAsync.
/// Test IDs reference the validation spec document.
/// </summary>
public class CompressionReportParserTests : IDisposable
{
    private readonly List<string> _tempFiles = new();

    public void Dispose()
    {
        foreach (var file in _tempFiles)
        {
            if (File.Exists(file))
            {
                try { File.Delete(file); } catch { /* ignore cleanup errors */ }
            }
        }
    }

    private string CreateTempFile(string content)
    {
        var path = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid():N}.txt");
        File.WriteAllText(path, content);
        _tempFiles.Add(path);
        return path;
    }

    #region UT-P-001: ParseAsync rejects null/whitespace path

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    public async Task ParseAsync_NullOrWhitespacePath_ThrowsArgumentException(string? filePath)
    {
        // Arrange
        var parser = new CompressionReportParser(pelletAcceptanceThreshold: 100.0, strictEndOfOperation: false);

        // Act
        var act = () => parser.ParseAsync(filePath!, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("filePath");
    }

    #endregion

    #region UT-P-002: ParseAsync rejects missing file

    [Fact]
    public async Task ParseAsync_MissingFile_ThrowsFileNotFoundException()
    {
        // Arrange
        var parser = new CompressionReportParser(pelletAcceptanceThreshold: 100.0, strictEndOfOperation: false);
        var nonExistentPath = Path.Combine(Path.GetTempPath(), $"nonexistent_{Guid.NewGuid():N}.txt");

        // Act
        var act = () => parser.ParseAsync(nonExistentPath, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<FileNotFoundException>();
    }

    #endregion

    #region UT-P-003: Missing Compression Tester # in header

    [Fact]
    public async Task ParseAsync_MissingTesterIdInHeader_ThrowsInvalidOperationException()
    {
        // Arrange
        var parser = new CompressionReportParser(pelletAcceptanceThreshold: 100.0, strictEndOfOperation: false);
        var content = new TestFileBuilder()
            .WithHeaderLine("Some header without tester identifier")
            .WithHeaderLine("Another line")
            .WithDataRow("01/27/2026 09:10:11", 1.0, 40.0, 200.0, 1.0, 0.0)
            .WithDataRow("01/27/2026 09:10:12", 0.0, 40.0, 0.0, 1.0, 1.0)
            .Build();
        var filePath = CreateTempFile(content);

        // Act
        var act = () => parser.ParseAsync(filePath, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Compression Tester*");
    }

    #endregion

    #region UT-P-004: No core data rows parsed

    [Fact]
    public async Task ParseAsync_NoDataRows_ThrowsInvalidOperationException()
    {
        // Arrange
        var parser = new CompressionReportParser(pelletAcceptanceThreshold: 100.0, strictEndOfOperation: false);
        var content = new TestFileBuilder()
            .WithTesterId(3)
            .WithStandardHeader()
            .WithRawLine("This line does not have 6 columns")
            .WithRawLine("Neither does this one")
            .Build();
        var filePath = CreateTempFile(content);

        // Act
        var act = () => parser.ParseAsync(filePath, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*No core data rows*");
    }

    #endregion

    #region UT-P-005: No start row

    [Fact]
    public async Task ParseAsync_NoStartRow_ThrowsInvalidOperationException()
    {
        // Arrange
        // Data rows exist but none match start conditions: Col2==1, Col5>0, Col6==0
        var parser = new CompressionReportParser(pelletAcceptanceThreshold: 100.0, strictEndOfOperation: false);
        var content = new TestFileBuilder()
            .WithTesterId(3)
            .WithStandardHeader()
            // Col2=0, so not a start row
            .WithDataRow("01/27/2026 09:10:11", 0.0, 40.0, 200.0, 1.0, 0.0)
            // Col5=0, so not a start row
            .WithDataRow("01/27/2026 09:10:12", 1.0, 40.0, 200.0, 0.0, 0.0)
            // Col6=1, so not a start row
            .WithDataRow("01/27/2026 09:10:13", 1.0, 40.0, 200.0, 1.0, 1.0)
            .Build();
        var filePath = CreateTempFile(content);

        // Act
        var act = () => parser.ParseAsync(filePath, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*No start row*");
    }

    #endregion

    #region UT-P-007: Timestamp parse failure

    [Fact]
    public async Task ParseAsync_InvalidTimestamp_ThrowsInvalidOperationException()
    {
        // Arrange
        var parser = new CompressionReportParser(pelletAcceptanceThreshold: 100.0, strictEndOfOperation: false);
        var content = new TestFileBuilder()
            .WithTesterId(3)
            .WithStandardHeader()
            // Valid start row
            .WithDataRow("01/27/2026 09:10:11", 1.0, 40.0, 200.0, 1.0, 0.0)
            // Row with unparseable timestamp
            .WithDataRow("not-a-timestamp", 1.0, 40.0, 210.0, 2.0, 0.0)
            .WithDataRow("01/27/2026 09:10:13", 0.0, 40.0, 0.0, 2.0, 1.0)
            .Build();
        var filePath = CreateTempFile(content);

        // Act
        var act = () => parser.ParseAsync(filePath, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Unable to parse timestamp*");
    }

    #endregion

    #region UT-P-010: Incomplete test with pellet reset/repeat

    [Fact]
    public async Task ParseAsync_IncompleteTestWithPelletReset_ThrowsInvalidOperationException()
    {
        // Arrange
        // No end_of_operation marker (Col6 != 1 at end) AND pellet numbers reset/repeat
        var parser = new CompressionReportParser(pelletAcceptanceThreshold: 100.0, strictEndOfOperation: false);
        var content = new TestFileBuilder()
            .WithTesterId(3)
            .WithStandardHeader()
            .WithDataRow("01/27/2026 09:10:11", 1.0, 40.0, 200.0, 1.0, 0.0)
            .WithDataRow("01/27/2026 09:10:12", 1.0, 40.0, 210.0, 2.0, 0.0)
            .WithDataRow("01/27/2026 09:10:13", 1.0, 40.0, 220.0, 3.0, 0.0)
            // Pellet number resets from 3 back to 1
            .WithDataRow("01/27/2026 09:10:14", 1.0, 40.0, 230.0, 1.0, 0.0)
            // No end marker - last row still has Col6=0
            .Build();
        var filePath = CreateTempFile(content);

        // Act
        var act = () => parser.ParseAsync(filePath, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*pellet numbers reset/repeat*");
    }

    [Fact]
    public async Task ParseAsync_IncompleteTestWithPelletRepeat_ThrowsInvalidOperationException()
    {
        // Arrange
        // Pellet number stays the same (repeat, not strictly increasing)
        var parser = new CompressionReportParser(pelletAcceptanceThreshold: 100.0, strictEndOfOperation: false);
        var content = new TestFileBuilder()
            .WithTesterId(3)
            .WithStandardHeader()
            .WithDataRow("01/27/2026 09:10:11", 1.0, 40.0, 200.0, 1.0, 0.0)
            .WithDataRow("01/27/2026 09:10:12", 1.0, 40.0, 210.0, 2.0, 0.0)
            // Pellet number repeats (2 -> 2)
            .WithDataRow("01/27/2026 09:10:13", 1.0, 40.0, 220.0, 2.0, 0.0)
            .Build();
        var filePath = CreateTempFile(content);

        // Act
        var act = () => parser.ParseAsync(filePath, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*pellet numbers reset/repeat*");
    }

    #endregion

    #region UT-P-011: Incomplete test, last pellet < threshold, strict end-of-op enabled

    [Fact]
    public async Task ParseAsync_IncompleteTestBelowThreshold_StrictMode_ThrowsInvalidOperationException()
    {
        // Arrange
        // No end marker, pellet numbers increasing, but last pellet < threshold, strict mode ON
        var parser = new CompressionReportParser(pelletAcceptanceThreshold: 100.0, strictEndOfOperation: true);
        var content = new TestFileBuilder()
            .WithTesterId(3)
            .WithStandardHeader()
            .WithDataRow("01/27/2026 09:10:11", 1.0, 40.0, 200.0, 1.0, 0.0)
            .WithDataRow("01/27/2026 09:10:12", 1.0, 40.0, 210.0, 2.0, 0.0)
            .WithDataRow("01/27/2026 09:10:13", 1.0, 40.0, 220.0, 3.0, 0.0)
            // Last pellet = 3, which is < 100 threshold, no end marker
            .Build();
        var filePath = CreateTempFile(content);

        // Act
        var act = () => parser.ParseAsync(filePath, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*End of operation flag is not 1*");
    }

    #endregion

    #region UT-P-012: Incomplete test, last pellet < threshold, strict end-of-op disabled

    [Fact]
    public async Task ParseAsync_IncompleteTestBelowThreshold_LenientMode_Succeeds()
    {
        // Arrange
        // No end marker, pellet numbers increasing, last pellet < threshold, strict mode OFF
        var parser = new CompressionReportParser(pelletAcceptanceThreshold: 100.0, strictEndOfOperation: false);
        var content = new TestFileBuilder()
            .WithTesterId(3)
            .WithStandardHeader()
            .WithDataRow("01/27/2026 09:10:11", 1.0, 40.0, 200.0, 1.0, 0.0)
            .WithDataRow("01/27/2026 09:10:12", 1.0, 40.0, 210.0, 2.0, 0.0)
            .WithDataRow("01/27/2026 09:10:13", 1.0, 40.0, 220.0, 3.0, 0.0)
            .Build();
        var filePath = CreateTempFile(content);

        // Act
        var result = await parser.ParseAsync(filePath, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.EndOfOperationIsOne.Should().BeFalse();
        result.LastPelletNumber.Should().Be(3.0);
        result.DataPoints.Should().HaveCount(3);
    }

    [Fact]
    public async Task ParseAsync_IncompleteTestAboveThreshold_LenientMode_Succeeds()
    {
        // Arrange
        // No end marker, pellet numbers increasing, last pellet >= threshold
        var parser = new CompressionReportParser(pelletAcceptanceThreshold: 100.0, strictEndOfOperation: false);
        var content = new TestFileBuilder()
            .WithTesterId(3)
            .WithStandardHeader();

        // Add 105 rows with increasing pellet numbers
        var baseTime = new DateTime(2026, 1, 27, 9, 10, 0);
        for (var i = 1; i <= 105; i++)
        {
            var timestamp = baseTime.AddSeconds(i).ToString("MM/dd/yyyy HH:mm:ss");
            content.WithDataRow(timestamp, 1.0, 40.0, 200.0 + i, i, 0.0);
        }

        var filePath = CreateTempFile(content.Build());

        // Act
        var result = await parser.ParseAsync(filePath, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.EndOfOperationIsOne.Should().BeFalse();
        result.LastPelletNumber.Should().Be(105.0);
    }

    #endregion

    #region UT-P-020: End-of-operation marker row excluded from DataPoints

    [Fact]
    public async Task ParseAsync_EndOfOperationMarkerRow_ExcludedFromDataPoints()
    {
        // Arrange
        var parser = new CompressionReportParser(pelletAcceptanceThreshold: 100.0, strictEndOfOperation: false);
        var content = new TestFileBuilder()
            .WithTesterId(3)
            .WithStandardHeader()
            .WithDataRow("01/27/2026 09:10:11", 1.0, 40.0, 200.0, 1.0, 0.0)
            .WithDataRow("01/27/2026 09:10:12", 1.0, 40.0, 210.0, 2.0, 0.0)
            .WithDataRow("01/27/2026 09:10:13", 1.0, 40.0, 220.0, 3.0, 0.0)
            // End-of-operation marker row: Col2=0, Col6=1
            .WithDataRow("01/27/2026 09:10:14", 0.0, 40.0, 0.0, 3.0, 1.0)
            .Build();
        var filePath = CreateTempFile(content);

        // Act
        var result = await parser.ParseAsync(filePath, CancellationToken.None);

        // Assert
        // 3 measurement rows, marker row excluded
        result.DataPoints.Should().HaveCount(3);
        result.DataPoints.Should().AllSatisfy(dp => dp.CompressionLbs.Should().BeGreaterThan(0));
        // Verify end marker is properly detected
        result.EndOfOperationIsOne.Should().BeTrue();
    }

    #endregion

    #region UT-P-021: Trailing saturated 1000.000 excluded for incomplete tests

    [Fact]
    public async Task ParseAsync_IncompleteTestWithTrailingSaturatedValue_ExcludesTrailingRow()
    {
        // Arrange
        var parser = new CompressionReportParser(pelletAcceptanceThreshold: 100.0, strictEndOfOperation: false);
        var content = new TestFileBuilder()
            .WithTesterId(3)
            .WithStandardHeader()
            .WithDataRow("01/27/2026 09:10:11", 1.0, 40.0, 200.0, 1.0, 0.0)
            .WithDataRow("01/27/2026 09:10:12", 1.0, 40.0, 210.0, 2.0, 0.0)
            .WithDataRow("01/27/2026 09:10:13", 1.0, 40.0, 220.0, 3.0, 0.0)
            // Trailing saturated value (1000.000) - no end marker
            .WithDataRow("01/27/2026 09:10:14", 1.0, 40.0, 1000.0, 4.0, 0.0)
            .Build();
        var filePath = CreateTempFile(content);

        // Act
        var result = await parser.ParseAsync(filePath, CancellationToken.None);

        // Assert
        // Trailing 1000.000 should be excluded from DataPoints
        result.DataPoints.Should().HaveCount(3);
        result.DataPoints.Should().NotContain(dp => dp.CompressionLbs == 1000.0);
        result.EndOfOperationIsOne.Should().BeFalse();
    }

    [Fact]
    public async Task ParseAsync_CompleteTestWithTrailingSaturatedValue_IncludesTrailingRow()
    {
        // Arrange
        // When end_of_operation IS encountered, saturated values should NOT be excluded
        var parser = new CompressionReportParser(pelletAcceptanceThreshold: 100.0, strictEndOfOperation: false);
        var content = new TestFileBuilder()
            .WithTesterId(3)
            .WithStandardHeader()
            .WithDataRow("01/27/2026 09:10:11", 1.0, 40.0, 200.0, 1.0, 0.0)
            .WithDataRow("01/27/2026 09:10:12", 1.0, 40.0, 210.0, 2.0, 0.0)
            .WithDataRow("01/27/2026 09:10:13", 1.0, 40.0, 1000.0, 3.0, 0.0)
            // End-of-operation marker present
            .WithDataRow("01/27/2026 09:10:14", 0.0, 40.0, 0.0, 3.0, 1.0)
            .Build();
        var filePath = CreateTempFile(content);

        // Act
        var result = await parser.ParseAsync(filePath, CancellationToken.None);

        // Assert
        // Saturated value should be included since test is complete
        result.DataPoints.Should().HaveCount(3);
        result.DataPoints.Should().Contain(dp => dp.CompressionLbs == 1000.0);
        result.EndOfOperationIsOne.Should().BeTrue();
    }

    #endregion

    #region UT-P-022: LastPelletNumber/EndOfOperationIsOne derived from last parsed test row

    [Fact]
    public async Task ParseAsync_LastPelletAndEndFlag_DerivedFromLastTestRow_NotLastDataPoint()
    {
        // Arrange
        // The end-of-operation marker row defines LastPelletNumber and EndOfOperationIsOne
        // even though it's excluded from DataPoints
        var parser = new CompressionReportParser(pelletAcceptanceThreshold: 100.0, strictEndOfOperation: false);
        var content = new TestFileBuilder()
            .WithTesterId(3)
            .WithStandardHeader()
            .WithDataRow("01/27/2026 09:10:11", 1.0, 40.0, 200.0, 1.0, 0.0)
            .WithDataRow("01/27/2026 09:10:12", 1.0, 40.0, 210.0, 2.0, 0.0)
            .WithDataRow("01/27/2026 09:10:13", 1.0, 40.0, 220.0, 3.0, 0.0)
            // End-of-operation marker row with pellet_number=99 (different from last measurement)
            .WithDataRow("01/27/2026 09:10:14", 0.0, 40.0, 0.0, 99.0, 1.0)
            .Build();
        var filePath = CreateTempFile(content);

        // Act
        var result = await parser.ParseAsync(filePath, CancellationToken.None);

        // Assert
        // LastPelletNumber should be from the marker row (99), not from the last datapoint (3)
        result.LastPelletNumber.Should().Be(99.0);
        result.EndOfOperationIsOne.Should().BeTrue();
        // DataPoints should not include the marker row
        result.DataPoints.Should().HaveCount(3);
        result.DataPoints.Last().PelletNumber.Should().Be(3);
    }

    #endregion

    #region Additional validation tests for complete coverage

    [Fact]
    public async Task ParseAsync_ValidReport_ReturnsCorrectResult()
    {
        // Arrange
        var parser = new CompressionReportParser(pelletAcceptanceThreshold: 100.0, strictEndOfOperation: false);
        var content = TestFileBuilder.CreateValidReport(testerId: 3, testNumber: 40.0, measurementCount: 5).Build();
        var filePath = CreateTempFile(content);

        // Act
        var result = await parser.ParseAsync(filePath, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.SourceFile.Should().Be(filePath);
        result.InstrumentId.Should().Be(3);
        result.TestNumber.Should().Be(40.0);
        result.DataPoints.Should().HaveCount(5);
        result.EndOfOperationIsOne.Should().BeTrue();
    }

    [Fact]
    public async Task ParseAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var parser = new CompressionReportParser(pelletAcceptanceThreshold: 100.0, strictEndOfOperation: false);
        var content = TestFileBuilder.CreateValidReport(testerId: 3, testNumber: 40.0, measurementCount: 5).Build();
        var filePath = CreateTempFile(content);
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var act = () => parser.ParseAsync(filePath, cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task ParseAsync_RowsWithWrongColumnCount_Ignored()
    {
        // Arrange
        var parser = new CompressionReportParser(pelletAcceptanceThreshold: 100.0, strictEndOfOperation: false);
        var content = new TestFileBuilder()
            .WithTesterId(3)
            .WithStandardHeader()
            .WithRawLine("This has    only    three    columns")
            .WithDataRow("01/27/2026 09:10:11", 1.0, 40.0, 200.0, 1.0, 0.0)
            .WithRawLine("Another bad line with wrong columns")
            .WithDataRow("01/27/2026 09:10:12", 1.0, 40.0, 210.0, 2.0, 0.0)
            .WithDataRow("01/27/2026 09:10:13", 0.0, 40.0, 0.0, 2.0, 1.0)
            .Build();
        var filePath = CreateTempFile(content);

        // Act
        var result = await parser.ParseAsync(filePath, CancellationToken.None);

        // Assert
        result.DataPoints.Should().HaveCount(2);
    }

    [Fact]
    public async Task ParseAsync_DataPoints_HaveCorrectValues()
    {
        // Arrange
        var parser = new CompressionReportParser(pelletAcceptanceThreshold: 100.0, strictEndOfOperation: false);
        var content = new TestFileBuilder()
            .WithTesterId(3)
            .WithStandardHeader()
            .WithDataRow("01/27/2026 09:10:11", 1.0, 40.0, 200.5, 1.0, 0.0)
            .WithDataRow("01/27/2026 09:10:12", 1.0, 40.0, 210.75, 2.0, 0.0)
            .WithDataRow("01/27/2026 09:10:13", 0.0, 40.0, 0.0, 2.0, 1.0)
            .Build();
        var filePath = CreateTempFile(content);

        // Act
        var result = await parser.ParseAsync(filePath, CancellationToken.None);

        // Assert
        result.DataPoints.Should().HaveCount(2);

        result.DataPoints[0].CompressionLbs.Should().Be(200.5);
        result.DataPoints[0].PelletNumber.Should().Be(1);
        result.DataPoints[0].Timestamp.Should().Be(new DateTime(2026, 1, 27, 9, 10, 11));

        result.DataPoints[1].CompressionLbs.Should().Be(210.75);
        result.DataPoints[1].PelletNumber.Should().Be(2);
        result.DataPoints[1].Timestamp.Should().Be(new DateTime(2026, 1, 27, 9, 10, 12));
    }

    #endregion
}
