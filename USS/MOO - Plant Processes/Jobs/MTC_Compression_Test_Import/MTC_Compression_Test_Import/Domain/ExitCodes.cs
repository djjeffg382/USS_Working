namespace MTC_Compression_Test_Import.Domain;

/// <summary>
/// Exit codes returned by the application to indicate processing status.
/// </summary>
public static class ExitCodes
{
    /// <summary>All files processed successfully.</summary>
    public const int Success = 0;

    /// <summary>Missing or invalid configuration.</summary>
    public const int ConfigurationError = 1;

    /// <summary>Source directory does not exist.</summary>
    public const int SourceDirectoryNotFound = 2;

    /// <summary>One or more files failed to process.</summary>
    public const int ProcessingError = 3;

    /// <summary>Processing was canceled.</summary>
    public const int Canceled = 4;

    /// <summary>Unexpected error occurred.</summary>
    public const int UnhandledException = 5;
}
