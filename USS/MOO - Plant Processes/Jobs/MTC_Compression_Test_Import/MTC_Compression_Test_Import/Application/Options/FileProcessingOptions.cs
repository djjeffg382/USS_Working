namespace MTC_Compression_Test_Import.Application.Options;

public sealed class FileProcessingOptions
{
    public string SourceDirectory { get; set; } = string.Empty;

    public string ArchiveDirectory { get; set; } = string.Empty;

    public string ErrorDirectory { get; set; } = string.Empty;

    /// <summary>
    /// When true, the job will write parsed compression data to the database.
    /// Defaults to false so that new deployments do not write until explicitly enabled.
    /// </summary>
    public bool EnableDatabaseWrites { get; set; } = false;

    /// <summary>
    /// When true and EnableDatabaseWrites is false, logs what DB operations would be executed
    /// (tables, key fields, parameter values) without actually performing any writes.
    /// </summary>
    public bool PreviewDatabaseWrites { get; set; } = true;

    public string FilePattern { get; set; } = "*.txt";

    public double PelletAcceptanceThreshold { get; set; } = 100.0;

    public bool StrictEndOfOperation { get; set; } = false;
}
