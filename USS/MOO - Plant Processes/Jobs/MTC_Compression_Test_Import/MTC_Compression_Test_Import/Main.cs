using System;
using Microsoft.Extensions.Configuration;
using MTC_Compression_Test_Import.Application.Options;
using MTC_Compression_Test_Import.Domain;
using MTC_Compression_Test_Import.Infrastructure.FileSystem;
using MTC_Compression_Test_Import.Infrastructure.Parsing;
using MTC_Compression_Test_Import.Infrastructure.Persistence;
using Serilog;

namespace MTC_Compression_Test_Import
{
    public class Main
    {
        private readonly IConfiguration _config;

        public Main(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Main Point of Program run
        /// </summary>
        /// <returns>Exit code: 0 for success, non-zero for errors.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Class instantiated by ActivatorUtilities.CreateInstance")]
        public int Run()
        {
            var options = _config.GetSection("FileProcessing").Get<FileProcessingOptions>();
            if (options is null)
            {
                Log.Error("Configuration section 'FileProcessing' is missing.");
                return ExitCodes.ConfigurationError;
            }

            var parser = new CompressionReportParser(
                options.PelletAcceptanceThreshold,
                options.StrictEndOfOperation);

            var previewDatabaseWrites = options.PreviewDatabaseWrites && !options.EnableDatabaseWrites;
            var repository = new OracleCompressionRepository(previewDatabaseWrites);
            var enableDatabaseWrites = options.EnableDatabaseWrites;
            var processor = new FileProcessor(options, parser, repository, enableDatabaseWrites, options.PreviewDatabaseWrites);

            using var cts = new System.Threading.CancellationTokenSource();
            var exitCode = processor.RunAsync(cts.Token).GetAwaiter().GetResult();

            if (exitCode != ExitCodes.Success)
            {
                Log.Error("File processing completed with non-zero exit code: {ExitCode}", exitCode);
            }

            return exitCode;
        }
    }
}
