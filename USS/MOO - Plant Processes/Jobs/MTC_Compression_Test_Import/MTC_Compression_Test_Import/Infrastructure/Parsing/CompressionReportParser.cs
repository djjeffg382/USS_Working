using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MTC_Compression_Test_Import.Domain.Entities;
using MTC_Compression_Test_Import.Domain.Interfaces;
using Serilog;

namespace MTC_Compression_Test_Import.Infrastructure.Parsing;

public sealed class CompressionReportParser : IFileParser
{
    private static readonly Regex DataSplitRegex = new("\\s{4,}", RegexOptions.Compiled);
    private readonly double _pelletAcceptanceThreshold;
    private readonly bool _strictEndOfOperation;

    public CompressionReportParser(double pelletAcceptanceThreshold, bool strictEndOfOperation)
    {
        _pelletAcceptanceThreshold = pelletAcceptanceThreshold;
        _strictEndOfOperation = strictEndOfOperation;
    }

    public async Task<CompressionParseResult> ParseAsync(string filePath, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path must not be null or empty.", nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Report file not found.", filePath);
        }

        var lines = await File.ReadAllLinesAsync(filePath, cancellationToken).ConfigureAwait(false);

        // Retrieve compression tester value from header ("Compression Tester # ")
        var compressionTesterFromHeader = ExtractTesterId(lines, filePath);

        var rows = ParseDataRows(lines, filePath);

        if (rows.Count == 0)
        {
            throw new InvalidOperationException($"No core data rows were parsed from report '{filePath}'.");
        }

        // Determine the processing range based on start/stop conditions over columns 2, 5, and 6.
        var (startIndex, stopIndex) = FindProcessingRange(rows, filePath);

        // The first valid row defines the file-level test number (column 3).
        var firstRow = rows[startIndex];
        const double tolerance = 1e-6;
        double fileTestNumber = firstRow.Col3;

        bool IsApproximately(double value, double target) => Math.Abs(value - target) < tolerance;

        // Build the collection of valid rows (from start through stop, inclusive) for this test number only.
        var validRows = new List<CompressionDataRow>();
        for (var i = startIndex; i <= stopIndex; i++)
        {
            var row = rows[i];
            if (IsApproximately(row.Col3, fileTestNumber))
            {
                validRows.Add(row);
            }
        }

        if (validRows.Count == 0)
        {
            throw new InvalidOperationException(
                $"No rows found for test_number {fileTestNumber} in '{filePath}'.");
        }

        // Determine the last record for this test number and validate end_of_operation / pellet_number.
        var lastRowForTest = validRows[^1];
        double lastEndOfOperation = lastRowForTest.Col6;
        double lastPelletNumber = lastRowForTest.Col5;

        if (!IsApproximately(lastEndOfOperation, 1.0))
        {
            // If end_of_operation is never encountered, pellet numbers must remain strictly increasing.
            // If they reset/repeat, reject the file so no database records are created.
            if (validRows.Count > 1)
            {
                var previousRow = validRows[0];
                var previousPellet = Convert.ToInt32(previousRow.Col5);

                for (var i = 1; i < validRows.Count; i++)
                {
                    var row = validRows[i];
                    var pellet = Convert.ToInt32(row.Col5);

                    if (pellet <= previousPellet)
                    {
                        throw new InvalidOperationException(
                            $"Rejecting report '{filePath}' for test_number={fileTestNumber} because end_of_operation was not encountered and " +
                            $"pellet numbers reset/repeat ({previousPellet} at '{previousRow.TimestampText}' -> {pellet} at '{row.TimestampText}').");
                    }

                    previousPellet = pellet;
                    previousRow = row;
                }
            }

            if (lastPelletNumber >= _pelletAcceptanceThreshold)
            {
                Log.Warning(
                    "End of operation flag is not 1 for last record of test_number={TestNumber} in '{FilePath}', but pellet_number={PelletNumber} >= {Threshold}; accepting last record as valid.",
                    fileTestNumber,
                    filePath,
                    lastPelletNumber,
                    _pelletAcceptanceThreshold);
            }
            else
            {
                var message =
                    $"End of operation flag is not 1 and pellet_number={lastPelletNumber} < {_pelletAcceptanceThreshold} " +
                    $"for last record of test_number={fileTestNumber} in '{filePath}'.";

                if (_strictEndOfOperation)
                {
                    throw new InvalidOperationException(message);
                }

                Log.Warning("{Message} Continuing due to lenient configuration.", message);
            }
        }
        // Build the detail rows to insert. Production calculations exclude end-of-operation marker rows
        // (status=0 and end_of_operation=1) from AVG/STDDEV/comp200/comp300.
        var rowsForDataPoints = new List<CompressionDataRow>(validRows.Count);
        foreach (var row in validRows)
        {
            // End-of-operation marker rows are not actual measurements and must be excluded from stats.
            if (IsApproximately(row.Col2, 0.0) && IsApproximately(row.Col6, 1.0))
            {
                continue;
            }

            rowsForDataPoints.Add(row);
        }

        // In some reports, an incomplete test (no end marker encountered) may end with a saturated
        // trailing value (e.g., 1000.000) that production excludes from calculations.
        if (!IsApproximately(lastEndOfOperation, 1.0) && rowsForDataPoints.Count > 0)
        {
            var lastDataRow = rowsForDataPoints[^1];
            if (IsApproximately(lastDataRow.Col4, 1000.0))
            {
                Log.Information(
                    "Excluding trailing saturated compression value (1000.000) for incomplete test_number={TestNumber} in '{FilePath}'.",
                    fileTestNumber,
                    filePath);

                rowsForDataPoints.RemoveAt(rowsForDataPoints.Count - 1);
            }
        }

        var dataPoints = new List<CompressionDataPoint>(rowsForDataPoints.Count);
        foreach (var row in rowsForDataPoints)
        {
            if (!TryParseTimestamp(row.TimestampText, out var timestamp))
            {
                throw new InvalidOperationException($"Unable to parse timestamp '{row.TimestampText}' in '{filePath}'.");
            }

            dataPoints.Add(new CompressionDataPoint(
                timestamp,
                row.Col4,
                Convert.ToInt32(row.Col5)));
        }

        return new CompressionParseResult(
            filePath,
            Convert.ToInt32(compressionTesterFromHeader),
            fileTestNumber,
            dataPoints,
            lastPelletNumber,
            IsApproximately(lastEndOfOperation, 1.0));
    }

    private static int ExtractTesterId(IReadOnlyList<string> lines, string filePath)
    {
        const string marker = "Compression Tester #";

        foreach (var rawLine in lines)
        {
            if (string.IsNullOrWhiteSpace(rawLine))
            {
                continue;
            }

            var line = rawLine;

            var index = line.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (index < 0)
            {
                continue;
            }

            var start = index + marker.Length;
            if (start >= line.Length)
            {
                break;
            }

            // Skip whitespace after the marker
            while (start < line.Length && char.IsWhiteSpace(line[start]))
            {
                start++;
            }

            if (start >= line.Length)
            {
                break;
            }

            var end = start;
            while (end < line.Length && char.IsDigit(line[end]))
            {
                end++;
            }

            var idText = line[start..end];
            if (int.TryParse(idText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var testerId))
            {
                return testerId;
            }

            break;
        }

        throw new InvalidOperationException($"Could not extract 'Compression Tester #' identifier from report '{filePath}'.");
    }

    private static List<CompressionDataRow> ParseDataRows(IReadOnlyList<string> lines, string filePath)
    {
        var result = new List<CompressionDataRow>();

        foreach (var rawLine in lines)
        {
            var line = rawLine?.Trim();
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            // Remove leading form-feed or other control characters that may precede the timestamp
            line = line.TrimStart('\f', '\t');
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var parts = DataSplitRegex.Split(line);
            if (parts.Length != 6)
            {
                continue;
            }

            for (var i = 0; i < parts.Length; i++)
            {
                parts[i] = parts[i].Trim();
            }

            var timestampText = parts[0];

            if (!TryParseDouble(parts[1], out var col2) ||
                !TryParseDouble(parts[2], out var col3) ||
                !TryParseDouble(parts[3], out var col4) ||
                !TryParseDouble(parts[4], out var col5) ||
                !TryParseDouble(parts[5], out var col6))
            {
                continue;
            }

            result.Add(new CompressionDataRow(timestampText, col2, col3, col4, col5, col6));
        }

        return result;
    }

    private static (int startIndex, int stopIndex) FindProcessingRange(IReadOnlyList<CompressionDataRow> rows, string filePath)
    {
        const double tolerance = 1e-6;

        bool IsApproximately(double value, double target) => Math.Abs(value - target) < tolerance;

        // Find the start row: Col2 == 1, Col5 > 0, Col6 == 0.
        var startIndex = -1;
        for (var i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            if (IsApproximately(row.Col2, 1.0) &&
                row.Col5 > 0.0 &&
                IsApproximately(row.Col6, 0.0))
            {
                startIndex = i;
                break;
            }
        }

        if (startIndex < 0)
        {
            throw new InvalidOperationException(
                $"No start row found with Col2 == 1, Col5 > 0, Col6 == 0 in '{filePath}'.");
        }

        // Find the stop row after the start: Col2 == 0 and Col6 == 1.
        var stopIndex = -1;
        for (var i = startIndex + 1; i < rows.Count; i++)
        {
            var row = rows[i];
            if (IsApproximately(row.Col2, 0.0) && IsApproximately(row.Col6, 1.0))
            {
                stopIndex = i;
                break;
            }
        }

        if (stopIndex < 0)
        {
            // No explicit end marker row for this test range; fall back to the last row.
            stopIndex = rows.Count - 1;
        }

        return (startIndex, stopIndex);
    }

    private static bool TryParseDouble(string text, out double value)
    {
        return double.TryParse(text, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out value);
    }

    private static bool TryParseTimestamp(string text, out DateTime value)
    {
        if (DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out value))
        {
            return true;
        }

        return DateTime.TryParse(text, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out value);
    }
}
