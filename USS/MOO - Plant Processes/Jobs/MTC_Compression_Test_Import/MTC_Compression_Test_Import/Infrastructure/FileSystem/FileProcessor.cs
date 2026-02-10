using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MTC_Compression_Test_Import.Application.Options;
using MTC_Compression_Test_Import.Domain;
using MTC_Compression_Test_Import.Domain.Entities;
using MTC_Compression_Test_Import.Domain.Interfaces;
using Serilog;

namespace MTC_Compression_Test_Import.Infrastructure.FileSystem;

public sealed class FileProcessor
{
    private readonly FileProcessingOptions _options;
    private readonly IFileParser _parser;
    private readonly ICompressionRepository _repository;
    private readonly bool _enableDatabaseWrites;
    private readonly bool _previewDatabaseWrites;
    private readonly List<FileProcessingError> _processingErrors = new();

    public FileProcessor(FileProcessingOptions options, IFileParser parser, ICompressionRepository repository, bool enableDatabaseWrites, bool previewDatabaseWrites)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _enableDatabaseWrites = enableDatabaseWrites;
        _previewDatabaseWrites = previewDatabaseWrites;
    }

    /// <summary>
    /// Runs a single processing pass over the source directory.
    /// Returns 0 for success, non-zero for error.
    /// </summary>
    public async Task<int> RunAsync(CancellationToken cancellationToken)
    {
        try
        {
            ValidateOptions();

            var sourceDirectory = _options.SourceDirectory!;
            var archiveDirectory = _options.ArchiveDirectory!;
            var errorDirectory = _options.ErrorDirectory!;
            var pattern = string.IsNullOrWhiteSpace(_options.FilePattern)
                ? "*.txt"
                : _options.FilePattern;

            if (!Directory.Exists(sourceDirectory))
            {
                Log.Error("Source directory does not exist: '{SourceDirectory}'.", sourceDirectory);
                return ExitCodes.SourceDirectoryNotFound;
            }

            if (!Directory.Exists(archiveDirectory))
            {
                Log.Information("Archive directory does not exist, creating: '{ArchiveDirectory}'.", archiveDirectory);
                Directory.CreateDirectory(archiveDirectory);
            }

            if (!Directory.Exists(errorDirectory))
            {
                Log.Information("Error directory does not exist, creating: '{ErrorDirectory}'.", errorDirectory);
                Directory.CreateDirectory(errorDirectory);
            }

            Log.Information("Looking for files in '{SourceDirectory}' matching pattern '{Pattern}'.", sourceDirectory, pattern);

            var files = Directory.GetFiles(sourceDirectory, pattern);
            Log.Information("Found {FileCount} file(s) to process.", files.Length);

            foreach (var file in files)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var fileName = Path.GetFileName(file);
                var archivedPath = Path.Combine(archiveDirectory, fileName);
                if (File.Exists(archivedPath))
                {
                    var errorPath = Path.Combine(errorDirectory, fileName);
                    Log.Warning(
                        "Duplicate file detected: '{SourceFile}' already exists in archive as '{ArchivePath}'. " +
                        "Moving source file to error directory: '{ErrorPath}'.",
                        file,
                        archivedPath,
                        errorPath);

                    var moved = TryMoveToDirectory(file, errorDirectory, "duplicate");
                    RecordError(
                        file,
                        moved ? errorPath : null,
                        "Duplicate",
                        $"File already exists in archive directory: {archivedPath}",
                        exception: null,
                        attemptNumber: 1,
                        maxAttempts: 1);
                    continue;
                }

                Log.Information("Processing file '{FilePath}'.", file);
                Console.WriteLine($"[Info] Processing file '{file}'.");

                var processed = await ProcessSingleFileAsync(file, archiveDirectory, errorDirectory, cancellationToken).ConfigureAwait(false);
                if (!processed)
                {
                    Log.Warning("Skipping file due to errors: '{FilePath}'.", file);
                }
            }

            LogProcessingSummary(files.Length);
            return _processingErrors.Count > 0 ? ExitCodes.ProcessingError : ExitCodes.Success;
        }
        catch (OperationCanceledException)
        {
            Log.Error("Processing was canceled.");
            return ExitCodes.Canceled;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unhandled exception during processing.");
            return ExitCodes.UnhandledException;
        }
    }

    private void ValidateOptions()
    {
        if (string.IsNullOrWhiteSpace(_options.SourceDirectory))
        {
            throw new InvalidOperationException("SourceDirectory must be configured in the FileProcessing section.");
        }

        if (string.IsNullOrWhiteSpace(_options.ArchiveDirectory))
        {
            throw new InvalidOperationException("ArchiveDirectory must be configured in the FileProcessing section.");
        }

        if (string.IsNullOrWhiteSpace(_options.ErrorDirectory))
        {
            throw new InvalidOperationException("ErrorDirectory must be configured in the FileProcessing section.");
        }
    }

    private async Task<bool> ProcessSingleFileAsync(string filePath, string archiveDirectory, string errorDirectory, CancellationToken cancellationToken)
    {
        const int maxAttempts = 3;
        const int delayMilliseconds = 500;
        Exception? lastException = null;
        var finalAttempt = 0;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            finalAttempt = attempt;
            try
            {
                Log.Information("Attempt {Attempt} to parse '{FilePath}'.", attempt, filePath);
                var parseResult = await _parser.ParseAsync(filePath, cancellationToken).ConfigureAwait(false);

                Log.Information(
                    "Parsed {RowCount} rows for test {TestNumber} on instrument {InstrumentId} from '{FilePath}'.",
                    parseResult.DataPoints.Count,
                    parseResult.TestNumber,
                    parseResult.InstrumentId,
                    filePath);
                if (!parseResult.EndOfOperationIsOne && parseResult.LastPelletNumber < _options.PelletAcceptanceThreshold)
                {
                    Log.Warning(
                        "File not completed: end_of_operation flag is not 1 and last pellet {LastPelletNumber} < threshold {Threshold} for '{FilePath}'.",
                        parseResult.LastPelletNumber,
                        _options.PelletAcceptanceThreshold,
                        filePath);
                }

                if (!_enableDatabaseWrites)
                {
                    if (_previewDatabaseWrites)
                    {
                        Log.Information("Database writes are disabled; previewing operations for '{FilePath}'.", filePath);
                        var compIdPreview = await _repository.InsertCompressionBatchAsync(parseResult).ConfigureAwait(false);
                        await _repository.UpdateStdDevAsync(compIdPreview).ConfigureAwait(false);
                    }
                    else
                    {
                        Log.Information("Database writes are disabled; skipping insert/stddev for '{FilePath}'.", filePath);
                    }
                }
                else
                {
                    Log.Information("Inserting compression data for '{FilePath}'.", filePath);
                    var compId = await _repository.InsertCompressionBatchAsync(parseResult).ConfigureAwait(false);
                    Log.Information("Inserted compression data with comp_id {CompId} for '{FilePath}'.", compId, filePath);

                    Log.Information("Computing STDDEV/AVG for comp_id {CompId}.", compId);
                    var stats = await _repository.UpdateStdDevAsync(compId).ConfigureAwait(false);
                    Log.Information(
                        "Updated stats for comp_id {CompId}: avg={Average}, stddev={StdDev}, comp200={Comp200}, comp300={Comp300}.",
                        compId,
                        stats.Average,
                        stats.StdDev,
                        stats.Comp200,
                        stats.Comp300);
                }

                MoveToArchive(filePath, archiveDirectory);
                return true;
            }
            catch (IOException ex) when (attempt < maxAttempts)
            {
                lastException = ex;
                Log.Warning(ex, "IO error processing '{FilePath}' (attempt {Attempt} of {MaxAttempts}). Will retry.", filePath, attempt, maxAttempts);
                await Task.Delay(delayMilliseconds, cancellationToken).ConfigureAwait(false);
            }
            catch (UnauthorizedAccessException ex) when (attempt < maxAttempts)
            {
                lastException = ex;
                Log.Warning(ex, "Access error processing '{FilePath}' (attempt {Attempt} of {MaxAttempts}). Will retry.", filePath, attempt, maxAttempts);
                await Task.Delay(delayMilliseconds, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                lastException = ex;
                var errorPath = Path.Combine(errorDirectory, Path.GetFileName(filePath));
                var category = ClassifyError(ex);

                LogDetailedError(filePath, errorPath, category, ex, attempt, maxAttempts);

                var moved = TryMoveToDirectory(filePath, errorDirectory, "error");
                RecordError(filePath, moved ? errorPath : null, category, ex.Message, ex, attempt, maxAttempts);
                return false;
            }
        }

        // All retry attempts exhausted
        var finalErrorPath = Path.Combine(errorDirectory, Path.GetFileName(filePath));
        var finalCategory = lastException != null ? ClassifyError(lastException) : "RetryExhausted";

        Log.Error(
            lastException,
            "Giving up on file after {MaxAttempts} attempts: '{FilePath}'. Last error: {ErrorMessage}",
            maxAttempts,
            filePath,
            lastException?.Message ?? "Unknown");
        Console.Error.WriteLine($"[Error] Giving up on file after {maxAttempts} attempts: '{filePath}'.");

        LogDetailedError(filePath, finalErrorPath, finalCategory, lastException, finalAttempt, maxAttempts);

        var movedFinal = TryMoveToDirectory(filePath, errorDirectory, "error");
        RecordError(filePath, movedFinal ? finalErrorPath : null, finalCategory, lastException?.Message ?? "Max retries exhausted", lastException, finalAttempt, maxAttempts);
        return false;
    }

    private static void MoveToArchive(string filePath, string archiveDirectory)
    {
        var fileName = Path.GetFileName(filePath);
        var destinationPath = Path.Combine(archiveDirectory, fileName);

        if (File.Exists(destinationPath))
        {
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var extension = Path.GetExtension(fileName);
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            var uniqueFileName = $"{nameWithoutExtension}_{timestamp}{extension}";
            destinationPath = Path.Combine(archiveDirectory, uniqueFileName);

            Log.Information("Archive file already exists. Using unique name '{UniqueFileName}'.", uniqueFileName);
        }

        File.Move(filePath, destinationPath);
        Log.Information("Moved '{FilePath}' to archive as '{DestinationPath}'.", filePath, destinationPath);
    }

    /// <summary>
    /// Attempts to move a file to the specified directory. Returns true if successful.
    /// </summary>
    private static bool TryMoveToDirectory(string filePath, string destinationDirectory, string reason)
    {
        try
        {
            MoveToArchive(filePath, destinationDirectory);
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(
                ex,
                "Failed to move '{FilePath}' to {Reason} directory '{DestinationDirectory}'.",
                filePath,
                reason,
                destinationDirectory);
            return false;
        }
    }

    /// <summary>
    /// Classifies an exception into a category for reporting.
    /// </summary>
    private static string ClassifyError(Exception ex) => ex switch
    {
        ArgumentException => "ValidationError",
        FileNotFoundException => "FileNotFound",
        InvalidOperationException ioe when ioe.Message.Contains("Compression Tester", StringComparison.OrdinalIgnoreCase) => "ParsingError_MissingHeader",
        InvalidOperationException ioe when ioe.Message.Contains("No core data rows", StringComparison.OrdinalIgnoreCase) => "ParsingError_NoData",
        InvalidOperationException ioe when ioe.Message.Contains("No start row", StringComparison.OrdinalIgnoreCase) => "ParsingError_NoStartRow",
        InvalidOperationException ioe when ioe.Message.Contains("No rows found for test_number", StringComparison.OrdinalIgnoreCase) => "ParsingError_NoTestRows",
        InvalidOperationException ioe when ioe.Message.Contains("pellet numbers reset", StringComparison.OrdinalIgnoreCase) => "ValidationError_PelletSequence",
        InvalidOperationException ioe when ioe.Message.Contains("End of operation flag", StringComparison.OrdinalIgnoreCase) => "ValidationError_IncompleteTest",
        InvalidOperationException ioe when ioe.Message.Contains("timestamp", StringComparison.OrdinalIgnoreCase) => "ParsingError_InvalidTimestamp",
        InvalidOperationException ioe when ioe.Message.Contains("comp_id", StringComparison.OrdinalIgnoreCase) => "DatabaseError_Stats",
        InvalidOperationException ioe when ioe.Message.Contains("Standard deviation", StringComparison.OrdinalIgnoreCase) => "DatabaseError_Stats",
        InvalidOperationException => "ValidationError",
        IOException => "IOError",
        UnauthorizedAccessException => "AccessError",
        _ when ex.GetType().Name.Contains("Oracle", StringComparison.OrdinalIgnoreCase) => "DatabaseError",
        _ => "UnknownError"
    };

    /// <summary>
    /// Logs detailed error information for troubleshooting.
    /// </summary>
    private static void LogDetailedError(
        string sourceFilePath,
        string errorFilePath,
        string category,
        Exception? ex,
        int attemptNumber,
        int maxAttempts)
    {
        Log.Error(
            ex,
            "File processing failed.\n" +
            "  Source: {SourceFilePath}\n" +
            "  Error Destination: {ErrorFilePath}\n" +
            "  Category: {ErrorCategory}\n" +
            "  Message: {ErrorMessage}\n" +
            "  Exception Type: {ExceptionType}\n" +
            "  Attempt: {AttemptNumber} of {MaxAttempts}\n" +
            "  Timestamp: {FailureTimestamp:O}",
            sourceFilePath,
            errorFilePath,
            category,
            ex?.Message ?? "Unknown error",
            ex?.GetType().Name ?? "None",
            attemptNumber,
            maxAttempts,
            DateTime.UtcNow);
    }

    /// <summary>
    /// Records an error for the processing summary.
    /// </summary>
    private void RecordError(
        string sourceFilePath,
        string? errorFilePath,
        string category,
        string message,
        Exception? exception,
        int attemptNumber,
        int maxAttempts)
    {
        _processingErrors.Add(new FileProcessingError(
            SourceFilePath: sourceFilePath,
            ErrorFilePath: errorFilePath,
            ErrorCategory: category,
            ErrorMessage: message,
            ExceptionType: exception?.GetType().Name,
            ExceptionDetails: exception?.ToString(),
            AttemptNumber: attemptNumber,
            MaxAttempts: maxAttempts,
            TesterIdExtracted: null,  // Could be enhanced to capture partial context
            TestNumberIdentified: null,
            RowsParsed: null,
            FailureTimestamp: DateTime.UtcNow));
    }

    /// <summary>
    /// Logs a summary of processing results.
    /// </summary>
    private void LogProcessingSummary(int totalFiles)
    {
        var successCount = totalFiles - _processingErrors.Count;
        var failedCount = _processingErrors.Count;

        if (failedCount == 0)
        {
            Log.Information(
                "Processing complete. Total files: {TotalFiles}, Successful: {SuccessCount}, Failed: {FailedCount}.",
                totalFiles,
                successCount,
                failedCount);
        }
        else
        {
            Log.Warning(
                "Processing complete with errors. Total files: {TotalFiles}, Successful: {SuccessCount}, Failed: {FailedCount}.",
                totalFiles,
                successCount,
                failedCount);

            Log.Warning("Failed files summary:");
            foreach (var error in _processingErrors)
            {
                var fileName = Path.GetFileName(error.SourceFilePath);
                Log.Warning(
                    "  - {FileName} ({ErrorCategory}): {ErrorMessage}",
                    fileName,
                    error.ErrorCategory,
                    error.ErrorMessage);
            }
        }
    }
}
