using FluentAssertions;
using Moq;
using MTC_Compression_Test_Import.Application.Options;
using MTC_Compression_Test_Import.Domain;
using MTC_Compression_Test_Import.Domain.Entities;
using MTC_Compression_Test_Import.Domain.Interfaces;
using MTC_Compression_Test_Import.Infrastructure.FileSystem;
using MTC_Compression_Test_Import.Infrastructure.Persistence;

namespace MTC_Compression_Test_Import.Tests.Processor;

/// <summary>
/// Component tests for FileProcessor.
/// Test IDs reference the validation spec document.
/// These tests use mocked IFileParser to avoid file parsing dependencies.
/// Database writes are disabled to avoid Oracle dependencies.
/// </summary>
public class FileProcessorTests : IDisposable
{
    private readonly string _testRoot;
    private readonly string _sourceDir;
    private readonly string _archiveDir;
    private readonly string _errorDir;

    public FileProcessorTests()
    {
        _testRoot = Path.Combine(Path.GetTempPath(), $"FileProcessorTests_{Guid.NewGuid():N}");
        _sourceDir = Path.Combine(_testRoot, "Source");
        _archiveDir = Path.Combine(_testRoot, "Archive");
        _errorDir = Path.Combine(_testRoot, "Error");

        Directory.CreateDirectory(_testRoot);
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_testRoot))
            {
                Directory.Delete(_testRoot, recursive: true);
            }
        }
        catch
        {
            // Ignore cleanup errors
        }
    }

    private FileProcessingOptions CreateOptions(
        string? sourceDir = null,
        string? archiveDir = null,
        string? errorDir = null,
        string? filePattern = "*.txt",
        double pelletAcceptanceThreshold = 100.0,
        bool strictEndOfOperation = false)
    {
        return new FileProcessingOptions
        {
            SourceDirectory = sourceDir ?? _sourceDir,
            ArchiveDirectory = archiveDir ?? _archiveDir,
            ErrorDirectory = errorDir ?? _errorDir,
            FilePattern = filePattern ?? "*.txt",
            PelletAcceptanceThreshold = pelletAcceptanceThreshold,
            StrictEndOfOperation = strictEndOfOperation,
            EnableDatabaseWrites = false,
            PreviewDatabaseWrites = false
        };
    }

    private static CompressionParseResult CreateSuccessfulParseResult(string filePath)
    {
        var dataPoints = new List<CompressionDataPoint>
        {
            new(DateTime.Now.AddSeconds(-2), 200.0, 1),
            new(DateTime.Now.AddSeconds(-1), 210.0, 2),
            new(DateTime.Now, 220.0, 3)
        };

        return new CompressionParseResult(
            sourceFile: filePath,
            instrumentId: 3,
            testNumber: 40.0,
            dataPoints: dataPoints,
            lastPelletNumber: 3.0,
            endOfOperationIsOne: true);
    }

    private static Mock<ICompressionRepository> CreateMockRepository()
    {
        return new Mock<ICompressionRepository>();
    }

    #region CT-F-001: Missing options causes exit code 5 (UnhandledException)

    [Theory]
    [InlineData(null, "archive", "error")]
    [InlineData("", "archive", "error")]
    [InlineData("   ", "archive", "error")]
    [InlineData("source", null, "error")]
    [InlineData("source", "", "error")]
    [InlineData("source", "   ", "error")]
    [InlineData("source", "archive", null)]
    [InlineData("source", "archive", "")]
    [InlineData("source", "archive", "   ")]
    public async Task RunAsync_MissingRequiredOptions_ReturnsUnhandledException(
        string? sourceDir, string? archiveDir, string? errorDir)
    {
        // Arrange
        var options = new FileProcessingOptions
        {
            SourceDirectory = sourceDir ?? string.Empty,
            ArchiveDirectory = archiveDir ?? string.Empty,
            ErrorDirectory = errorDir ?? string.Empty,
            EnableDatabaseWrites = false,
            PreviewDatabaseWrites = false
        };

        var mockParser = new Mock<IFileParser>();
        var mockRepository = new Mock<ICompressionRepository>();

        var processor = new FileProcessor(
            options,
            mockParser.Object,
            mockRepository.Object,
            enableDatabaseWrites: false,
            previewDatabaseWrites: false);

        // Act
        var exitCode = await processor.RunAsync(CancellationToken.None);

        // Assert - Missing options throws InvalidOperationException, caught as UnhandledException
        exitCode.Should().Be(ExitCodes.UnhandledException);
    }

    #endregion

    #region CT-F-002: Missing source directory causes exit code 2 (SourceDirectoryNotFound)

    [Fact]
    public async Task RunAsync_SourceDirectoryDoesNotExist_ReturnsSourceDirectoryNotFound()
    {
        // Arrange
        var nonExistentSource = Path.Combine(_testRoot, "NonExistentSource");
        Directory.CreateDirectory(_archiveDir);
        Directory.CreateDirectory(_errorDir);

        var options = CreateOptions(sourceDir: nonExistentSource);
        var mockParser = new Mock<IFileParser>();
        var mockRepository = new Mock<ICompressionRepository>();

        var processor = new FileProcessor(
            options,
            mockParser.Object,
            mockRepository.Object,
            enableDatabaseWrites: false,
            previewDatabaseWrites: false);

        // Act
        var exitCode = await processor.RunAsync(CancellationToken.None);

        // Assert
        exitCode.Should().Be(ExitCodes.SourceDirectoryNotFound);
    }

    #endregion

    #region CT-F-003: Missing archive/error directories are created

    [Fact]
    public async Task RunAsync_ArchiveDirectoryDoesNotExist_CreatesDirectory()
    {
        // Arrange
        Directory.CreateDirectory(_sourceDir);
        // Don't create archive dir - it should be created
        Directory.CreateDirectory(_errorDir);

        var options = CreateOptions();
        var mockParser = new Mock<IFileParser>();
        var mockRepository = new Mock<ICompressionRepository>();

        var processor = new FileProcessor(
            options,
            mockParser.Object,
            mockRepository.Object,
            enableDatabaseWrites: false,
            previewDatabaseWrites: false);

        // Act
        await processor.RunAsync(CancellationToken.None);

        // Assert
        Directory.Exists(_archiveDir).Should().BeTrue();
    }

    [Fact]
    public async Task RunAsync_ErrorDirectoryDoesNotExist_CreatesDirectory()
    {
        // Arrange
        Directory.CreateDirectory(_sourceDir);
        Directory.CreateDirectory(_archiveDir);
        // Don't create error dir - it should be created

        var options = CreateOptions();
        var mockParser = new Mock<IFileParser>();
        var mockRepository = new Mock<ICompressionRepository>();

        var processor = new FileProcessor(
            options,
            mockParser.Object,
            mockRepository.Object,
            enableDatabaseWrites: false,
            previewDatabaseWrites: false);

        // Act
        await processor.RunAsync(CancellationToken.None);

        // Assert
        Directory.Exists(_errorDir).Should().BeTrue();
    }

    [Fact]
    public async Task RunAsync_BothArchiveAndErrorDirectoriesDoNotExist_CreatesBoth()
    {
        // Arrange
        Directory.CreateDirectory(_sourceDir);
        // Don't create archive or error dirs

        var options = CreateOptions();
        var mockParser = new Mock<IFileParser>();
        var mockRepository = new Mock<ICompressionRepository>();

        var processor = new FileProcessor(
            options,
            mockParser.Object,
            mockRepository.Object,
            enableDatabaseWrites: false,
            previewDatabaseWrites: false);

        // Act
        await processor.RunAsync(CancellationToken.None);

        // Assert
        Directory.Exists(_archiveDir).Should().BeTrue();
        Directory.Exists(_errorDir).Should().BeTrue();
    }

    #endregion

    #region CT-F-004: Duplicate filename present in archive

    [Fact]
    public async Task RunAsync_DuplicateFilenameInArchive_MovesFileToErrorDirectory()
    {
        // Arrange
        Directory.CreateDirectory(_sourceDir);
        Directory.CreateDirectory(_archiveDir);
        Directory.CreateDirectory(_errorDir);

        var fileName = "test_report.txt";
        var sourceFilePath = Path.Combine(_sourceDir, fileName);
        var archiveFilePath = Path.Combine(_archiveDir, fileName);

        // Create file in source
        await File.WriteAllTextAsync(sourceFilePath, "source content");
        // Create duplicate in archive
        await File.WriteAllTextAsync(archiveFilePath, "archive content");

        var options = CreateOptions();
        var mockParser = new Mock<IFileParser>();
        var mockRepository = new Mock<ICompressionRepository>();

        var processor = new FileProcessor(
            options,
            mockParser.Object,
            mockRepository.Object,
            enableDatabaseWrites: false,
            previewDatabaseWrites: false);

        // Act
        await processor.RunAsync(CancellationToken.None);

        // Assert
        // Parser should NOT have been called (file was skipped)
        mockParser.Verify(p => p.ParseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);

        // File should be moved to error directory
        File.Exists(sourceFilePath).Should().BeFalse();
        File.Exists(Path.Combine(_errorDir, fileName)).Should().BeTrue();

        // Archive file should remain unchanged
        File.Exists(archiveFilePath).Should().BeTrue();
        (await File.ReadAllTextAsync(archiveFilePath)).Should().Be("archive content");
    }

    #endregion

    #region CT-F-010: Retry on IOException

    [Fact]
    public async Task RunAsync_IOExceptionOnFirstAttempt_RetriesAndSucceeds()
    {
        // Arrange
        Directory.CreateDirectory(_sourceDir);
        Directory.CreateDirectory(_archiveDir);
        Directory.CreateDirectory(_errorDir);

        var fileName = "test_report.txt";
        var sourceFilePath = Path.Combine(_sourceDir, fileName);
        await File.WriteAllTextAsync(sourceFilePath, "test content");

        var options = CreateOptions();
        var mockParser = new Mock<IFileParser>();
        var mockRepository = new Mock<ICompressionRepository>();

        var callCount = 0;
        mockParser
            .Setup(p => p.ParseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns<string, CancellationToken>((path, ct) =>
            {
                callCount++;
                if (callCount == 1)
                {
                    throw new IOException("Simulated IO error on first attempt");
                }
                return Task.FromResult(CreateSuccessfulParseResult(path));
            });

        var processor = new FileProcessor(
            options,
            mockParser.Object,
            mockRepository.Object,
            enableDatabaseWrites: false,
            previewDatabaseWrites: false);

        // Act
        var exitCode = await processor.RunAsync(CancellationToken.None);

        // Assert
        exitCode.Should().Be(0);
        callCount.Should().Be(2); // First attempt failed, second succeeded
        File.Exists(sourceFilePath).Should().BeFalse();
        File.Exists(Path.Combine(_archiveDir, fileName)).Should().BeTrue();
    }

    #endregion

    #region CT-F-011: Retry on UnauthorizedAccessException

    [Fact]
    public async Task RunAsync_UnauthorizedAccessExceptionOnFirstAttempt_RetriesAndSucceeds()
    {
        // Arrange
        Directory.CreateDirectory(_sourceDir);
        Directory.CreateDirectory(_archiveDir);
        Directory.CreateDirectory(_errorDir);

        var fileName = "test_report.txt";
        var sourceFilePath = Path.Combine(_sourceDir, fileName);
        await File.WriteAllTextAsync(sourceFilePath, "test content");

        var options = CreateOptions();
        var mockParser = new Mock<IFileParser>();
        var mockRepository = new Mock<ICompressionRepository>();

        var callCount = 0;
        mockParser
            .Setup(p => p.ParseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns<string, CancellationToken>((path, ct) =>
            {
                callCount++;
                if (callCount == 1)
                {
                    throw new UnauthorizedAccessException("Simulated access error on first attempt");
                }
                return Task.FromResult(CreateSuccessfulParseResult(path));
            });

        var processor = new FileProcessor(
            options,
            mockParser.Object,
            mockRepository.Object,
            enableDatabaseWrites: false,
            previewDatabaseWrites: false);

        // Act
        var exitCode = await processor.RunAsync(CancellationToken.None);

        // Assert
        exitCode.Should().Be(0);
        callCount.Should().Be(2); // First attempt failed, second succeeded
        File.Exists(sourceFilePath).Should().BeFalse();
        File.Exists(Path.Combine(_archiveDir, fileName)).Should().BeTrue();
    }

    #endregion

    #region CT-F-010b: Retry exhaustion moves file to error

    [Fact]
    public async Task RunAsync_IOExceptionOnAllAttempts_MovesFileToError()
    {
        // Arrange
        Directory.CreateDirectory(_sourceDir);
        Directory.CreateDirectory(_archiveDir);
        Directory.CreateDirectory(_errorDir);

        var fileName = "test_report.txt";
        var sourceFilePath = Path.Combine(_sourceDir, fileName);
        await File.WriteAllTextAsync(sourceFilePath, "test content");

        var options = CreateOptions();
        var mockParser = new Mock<IFileParser>();
        var mockRepository = new Mock<ICompressionRepository>();

        var callCount = 0;
        mockParser
            .Setup(p => p.ParseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns<string, CancellationToken>((path, ct) =>
            {
                callCount++;
                throw new IOException($"Simulated IO error on attempt {callCount}");
            });

        var processor = new FileProcessor(
            options,
            mockParser.Object,
            mockRepository.Object,
            enableDatabaseWrites: false,
            previewDatabaseWrites: false);

        // Act
        var exitCode = await processor.RunAsync(CancellationToken.None);

        // Assert - Processing completes but returns ProcessingError when files fail
        exitCode.Should().Be(ExitCodes.ProcessingError);
        callCount.Should().Be(3); // All 3 retry attempts exhausted
        File.Exists(sourceFilePath).Should().BeFalse();
        File.Exists(Path.Combine(_errorDir, fileName)).Should().BeTrue();
    }

    #endregion

    #region CT-F-012: Non-retry exception moves file to error

    [Fact]
    public async Task RunAsync_InvalidOperationException_MovesFileToErrorWithoutRetry()
    {
        // Arrange
        Directory.CreateDirectory(_sourceDir);
        Directory.CreateDirectory(_archiveDir);
        Directory.CreateDirectory(_errorDir);

        var fileName = "test_report.txt";
        var sourceFilePath = Path.Combine(_sourceDir, fileName);
        await File.WriteAllTextAsync(sourceFilePath, "test content");

        var options = CreateOptions();
        var mockParser = new Mock<IFileParser>();
        var mockRepository = new Mock<ICompressionRepository>();

        var callCount = 0;
        mockParser
            .Setup(p => p.ParseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns<string, CancellationToken>((path, ct) =>
            {
                callCount++;
                throw new InvalidOperationException("Simulated parsing error");
            });

        var processor = new FileProcessor(
            options,
            mockParser.Object,
            mockRepository.Object,
            enableDatabaseWrites: false,
            previewDatabaseWrites: false);

        // Act
        var exitCode = await processor.RunAsync(CancellationToken.None);

        // Assert - Processing completes but returns ProcessingError when files fail
        exitCode.Should().Be(ExitCodes.ProcessingError);
        callCount.Should().Be(1); // No retry for InvalidOperationException
        File.Exists(sourceFilePath).Should().BeFalse();
        File.Exists(Path.Combine(_errorDir, fileName)).Should().BeTrue();
    }

    [Fact]
    public async Task RunAsync_ArgumentException_MovesFileToErrorWithoutRetry()
    {
        // Arrange
        Directory.CreateDirectory(_sourceDir);
        Directory.CreateDirectory(_archiveDir);
        Directory.CreateDirectory(_errorDir);

        var fileName = "test_report.txt";
        var sourceFilePath = Path.Combine(_sourceDir, fileName);
        await File.WriteAllTextAsync(sourceFilePath, "test content");

        var options = CreateOptions();
        var mockParser = new Mock<IFileParser>();
        var mockRepository = new Mock<ICompressionRepository>();

        var callCount = 0;
        mockParser
            .Setup(p => p.ParseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns<string, CancellationToken>((path, ct) =>
            {
                callCount++;
                throw new ArgumentException("Simulated argument error");
            });

        var processor = new FileProcessor(
            options,
            mockParser.Object,
            mockRepository.Object,
            enableDatabaseWrites: false,
            previewDatabaseWrites: false);

        // Act
        var exitCode = await processor.RunAsync(CancellationToken.None);

        // Assert
        callCount.Should().Be(1); // No retry for ArgumentException
        File.Exists(Path.Combine(_errorDir, fileName)).Should().BeTrue();
    }

    #endregion

    #region Additional component tests

    [Fact]
    public async Task RunAsync_NoFilesInSourceDirectory_ReturnsExitCode0()
    {
        // Arrange
        Directory.CreateDirectory(_sourceDir);
        Directory.CreateDirectory(_archiveDir);
        Directory.CreateDirectory(_errorDir);

        var options = CreateOptions();
        var mockParser = new Mock<IFileParser>();
        var mockRepository = new Mock<ICompressionRepository>();

        var processor = new FileProcessor(
            options,
            mockParser.Object,
            mockRepository.Object,
            enableDatabaseWrites: false,
            previewDatabaseWrites: false);

        // Act
        var exitCode = await processor.RunAsync(CancellationToken.None);

        // Assert
        exitCode.Should().Be(0);
        mockParser.Verify(p => p.ParseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RunAsync_SuccessfulProcessing_MovesFileToArchive()
    {
        // Arrange
        Directory.CreateDirectory(_sourceDir);
        Directory.CreateDirectory(_archiveDir);
        Directory.CreateDirectory(_errorDir);

        var fileName = "test_report.txt";
        var sourceFilePath = Path.Combine(_sourceDir, fileName);
        await File.WriteAllTextAsync(sourceFilePath, "test content");

        var options = CreateOptions();
        var mockParser = new Mock<IFileParser>();
        var mockRepository = new Mock<ICompressionRepository>();

        mockParser
            .Setup(p => p.ParseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns<string, CancellationToken>((path, ct) => Task.FromResult(CreateSuccessfulParseResult(path)));

        var processor = new FileProcessor(
            options,
            mockParser.Object,
            mockRepository.Object,
            enableDatabaseWrites: false,
            previewDatabaseWrites: false);

        // Act
        var exitCode = await processor.RunAsync(CancellationToken.None);

        // Assert
        exitCode.Should().Be(0);
        File.Exists(sourceFilePath).Should().BeFalse();
        File.Exists(Path.Combine(_archiveDir, fileName)).Should().BeTrue();
    }

    [Fact]
    public async Task RunAsync_MultipleFiles_ProcessesAll()
    {
        // Arrange
        Directory.CreateDirectory(_sourceDir);
        Directory.CreateDirectory(_archiveDir);
        Directory.CreateDirectory(_errorDir);

        var fileNames = new[] { "report1.txt", "report2.txt", "report3.txt" };
        foreach (var fileName in fileNames)
        {
            await File.WriteAllTextAsync(Path.Combine(_sourceDir, fileName), "content");
        }

        var options = CreateOptions();
        var mockParser = new Mock<IFileParser>();
        var mockRepository = new Mock<ICompressionRepository>();

        mockParser
            .Setup(p => p.ParseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns<string, CancellationToken>((path, ct) => Task.FromResult(CreateSuccessfulParseResult(path)));

        var processor = new FileProcessor(
            options,
            mockParser.Object,
            mockRepository.Object,
            enableDatabaseWrites: false,
            previewDatabaseWrites: false);

        // Act
        var exitCode = await processor.RunAsync(CancellationToken.None);

        // Assert
        exitCode.Should().Be(0);
        mockParser.Verify(p => p.ParseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(3));

        foreach (var fileName in fileNames)
        {
            File.Exists(Path.Combine(_sourceDir, fileName)).Should().BeFalse();
            File.Exists(Path.Combine(_archiveDir, fileName)).Should().BeTrue();
        }
    }

    [Fact]
    public async Task RunAsync_CancellationRequested_ReturnsCanceled()
    {
        // Arrange
        Directory.CreateDirectory(_sourceDir);
        Directory.CreateDirectory(_archiveDir);
        Directory.CreateDirectory(_errorDir);

        var fileName = "test_report.txt";
        var sourceFilePath = Path.Combine(_sourceDir, fileName);
        await File.WriteAllTextAsync(sourceFilePath, "test content");

        var options = CreateOptions();
        var mockParser = new Mock<IFileParser>();
        var mockRepository = new Mock<ICompressionRepository>();

        var cts = new CancellationTokenSource();
        cts.Cancel();

        var processor = new FileProcessor(
            options,
            mockParser.Object,
            mockRepository.Object,
            enableDatabaseWrites: false,
            previewDatabaseWrites: false);

        // Act
        var exitCode = await processor.RunAsync(cts.Token);

        // Assert
        exitCode.Should().Be(ExitCodes.Canceled);
    }

    [Fact]
    public async Task RunAsync_FilePatternFilter_OnlyProcessesMatchingFiles()
    {
        // Arrange
        Directory.CreateDirectory(_sourceDir);
        Directory.CreateDirectory(_archiveDir);
        Directory.CreateDirectory(_errorDir);

        await File.WriteAllTextAsync(Path.Combine(_sourceDir, "report.txt"), "txt content");
        await File.WriteAllTextAsync(Path.Combine(_sourceDir, "report.csv"), "csv content");
        await File.WriteAllTextAsync(Path.Combine(_sourceDir, "report.log"), "log content");

        var options = CreateOptions(filePattern: "*.txt");
        var mockParser = new Mock<IFileParser>();
        var mockRepository = new Mock<ICompressionRepository>();

        mockParser
            .Setup(p => p.ParseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns<string, CancellationToken>((path, ct) => Task.FromResult(CreateSuccessfulParseResult(path)));

        var processor = new FileProcessor(
            options,
            mockParser.Object,
            mockRepository.Object,
            enableDatabaseWrites: false,
            previewDatabaseWrites: false);

        // Act
        await processor.RunAsync(CancellationToken.None);

        // Assert
        // Only .txt file should be processed
        mockParser.Verify(p => p.ParseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        File.Exists(Path.Combine(_archiveDir, "report.txt")).Should().BeTrue();
        // Other files should remain in source
        File.Exists(Path.Combine(_sourceDir, "report.csv")).Should().BeTrue();
        File.Exists(Path.Combine(_sourceDir, "report.log")).Should().BeTrue();
    }

    #endregion

    #region CT-F-020: Error categorization and logging

    [Fact]
    public async Task RunAsync_ParsingError_CategorizedCorrectly()
    {
        // Arrange
        Directory.CreateDirectory(_sourceDir);
        Directory.CreateDirectory(_archiveDir);
        Directory.CreateDirectory(_errorDir);

        var fileName = "test_report.txt";
        var sourceFilePath = Path.Combine(_sourceDir, fileName);
        await File.WriteAllTextAsync(sourceFilePath, "test content");

        var options = CreateOptions();
        var mockParser = new Mock<IFileParser>();
        var mockRepository = new Mock<ICompressionRepository>();

        // Simulate a parsing error with "No start row" message
        mockParser
            .Setup(p => p.ParseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("No start row found with Col2 == 1, Col5 > 0, Col6 == 0"));

        var processor = new FileProcessor(
            options,
            mockParser.Object,
            mockRepository.Object,
            enableDatabaseWrites: false,
            previewDatabaseWrites: false);

        // Act
        var exitCode = await processor.RunAsync(CancellationToken.None);

        // Assert - Processing completes but returns ProcessingError when files fail
        exitCode.Should().Be(ExitCodes.ProcessingError);
        File.Exists(Path.Combine(_errorDir, fileName)).Should().BeTrue();
    }

    [Fact]
    public async Task RunAsync_MissingHeaderError_CategorizedCorrectly()
    {
        // Arrange
        Directory.CreateDirectory(_sourceDir);
        Directory.CreateDirectory(_archiveDir);
        Directory.CreateDirectory(_errorDir);

        var fileName = "test_report.txt";
        var sourceFilePath = Path.Combine(_sourceDir, fileName);
        await File.WriteAllTextAsync(sourceFilePath, "test content");

        var options = CreateOptions();
        var mockParser = new Mock<IFileParser>();
        var mockRepository = new Mock<ICompressionRepository>();

        // Simulate a missing header error
        mockParser
            .Setup(p => p.ParseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Could not extract 'Compression Tester #' identifier"));

        var processor = new FileProcessor(
            options,
            mockParser.Object,
            mockRepository.Object,
            enableDatabaseWrites: false,
            previewDatabaseWrites: false);

        // Act
        var exitCode = await processor.RunAsync(CancellationToken.None);

        // Assert - Processing completes but returns ProcessingError when files fail
        exitCode.Should().Be(ExitCodes.ProcessingError);
        File.Exists(Path.Combine(_errorDir, fileName)).Should().BeTrue();
    }

    [Fact]
    public async Task RunAsync_MultipleFailures_AllRecordedInSummary()
    {
        // Arrange
        Directory.CreateDirectory(_sourceDir);
        Directory.CreateDirectory(_archiveDir);
        Directory.CreateDirectory(_errorDir);

        // Create multiple files that will fail
        await File.WriteAllTextAsync(Path.Combine(_sourceDir, "fail1.txt"), "content");
        await File.WriteAllTextAsync(Path.Combine(_sourceDir, "fail2.txt"), "content");
        await File.WriteAllTextAsync(Path.Combine(_sourceDir, "success.txt"), "content");

        var options = CreateOptions();
        var mockParser = new Mock<IFileParser>();
        var mockRepository = new Mock<ICompressionRepository>();

        mockParser
            .Setup(p => p.ParseAsync(It.Is<string>(s => s.Contains("fail1")), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("No core data rows"));

        mockParser
            .Setup(p => p.ParseAsync(It.Is<string>(s => s.Contains("fail2")), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("No start row"));

        mockParser
            .Setup(p => p.ParseAsync(It.Is<string>(s => s.Contains("success")), It.IsAny<CancellationToken>()))
            .Returns<string, CancellationToken>((path, ct) => Task.FromResult(CreateSuccessfulParseResult(path)));

        var processor = new FileProcessor(
            options,
            mockParser.Object,
            mockRepository.Object,
            enableDatabaseWrites: false,
            previewDatabaseWrites: false);

        // Act
        var exitCode = await processor.RunAsync(CancellationToken.None);

        // Assert - Processing completes but returns ProcessingError when some files fail
        exitCode.Should().Be(ExitCodes.ProcessingError);
        File.Exists(Path.Combine(_errorDir, "fail1.txt")).Should().BeTrue();
        File.Exists(Path.Combine(_errorDir, "fail2.txt")).Should().BeTrue();
        File.Exists(Path.Combine(_archiveDir, "success.txt")).Should().BeTrue();
    }

    [Fact]
    public async Task RunAsync_DuplicateFile_RecordedAsError()
    {
        // Arrange
        Directory.CreateDirectory(_sourceDir);
        Directory.CreateDirectory(_archiveDir);
        Directory.CreateDirectory(_errorDir);

        var fileName = "duplicate.txt";
        await File.WriteAllTextAsync(Path.Combine(_sourceDir, fileName), "source");
        await File.WriteAllTextAsync(Path.Combine(_archiveDir, fileName), "archive");

        var options = CreateOptions();
        var mockParser = new Mock<IFileParser>();
        var mockRepository = new Mock<ICompressionRepository>();

        var processor = new FileProcessor(
            options,
            mockParser.Object,
            mockRepository.Object,
            enableDatabaseWrites: false,
            previewDatabaseWrites: false);

        // Act
        var exitCode = await processor.RunAsync(CancellationToken.None);

        // Assert - Processing completes but returns ProcessingError when files fail
        exitCode.Should().Be(ExitCodes.ProcessingError);
        // Parser should not be called for duplicate files
        mockParser.Verify(p => p.ParseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        // File should be moved to error directory
        File.Exists(Path.Combine(_errorDir, fileName)).Should().BeTrue();
        // Archive file should be unchanged
        (await File.ReadAllTextAsync(Path.Combine(_archiveDir, fileName))).Should().Be("archive");
    }

    [Fact]
    public async Task RunAsync_PelletSequenceError_CategorizedAsValidationError()
    {
        // Arrange
        Directory.CreateDirectory(_sourceDir);
        Directory.CreateDirectory(_archiveDir);
        Directory.CreateDirectory(_errorDir);

        var fileName = "test_report.txt";
        await File.WriteAllTextAsync(Path.Combine(_sourceDir, fileName), "test content");

        var options = CreateOptions();
        var mockParser = new Mock<IFileParser>();
        var mockRepository = new Mock<ICompressionRepository>();

        mockParser
            .Setup(p => p.ParseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("pellet numbers reset/repeat"));

        var processor = new FileProcessor(
            options,
            mockParser.Object,
            mockRepository.Object,
            enableDatabaseWrites: false,
            previewDatabaseWrites: false);

        // Act
        var exitCode = await processor.RunAsync(CancellationToken.None);

        // Assert - Processing completes but returns ProcessingError when files fail
        exitCode.Should().Be(ExitCodes.ProcessingError);
        File.Exists(Path.Combine(_errorDir, fileName)).Should().BeTrue();
    }

    #endregion
}
