using System;

namespace MTC_Compression_Test_Import.Domain.Entities;

/// <summary>
/// Captures comprehensive details about a file processing failure.
/// </summary>
public sealed record FileProcessingError(
    /// <summary>The original source file path.</summary>
    string SourceFilePath,

    /// <summary>The destination path in the error directory (null if move failed).</summary>
    string? ErrorFilePath,

    /// <summary>Categorized error type for filtering and reporting.</summary>
    string ErrorCategory,

    /// <summary>Human-readable error message.</summary>
    string ErrorMessage,

    /// <summary>The .NET exception type name (e.g., "InvalidOperationException").</summary>
    string? ExceptionType,

    /// <summary>Full exception details including stack trace.</summary>
    string? ExceptionDetails,

    /// <summary>Which attempt failed (1-based).</summary>
    int AttemptNumber,

    /// <summary>Maximum attempts configured.</summary>
    int MaxAttempts,

    /// <summary>Compression Tester ID if extracted before failure.</summary>
    int? TesterIdExtracted,

    /// <summary>Test number if identified before failure.</summary>
    double? TestNumberIdentified,

    /// <summary>Number of data rows parsed before failure.</summary>
    int? RowsParsed,

    /// <summary>UTC timestamp when the failure occurred.</summary>
    DateTime FailureTimestamp);
