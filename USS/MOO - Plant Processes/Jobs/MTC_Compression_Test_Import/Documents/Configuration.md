# MTC_Compression_Test_Import Configuration Guide

This document describes all configuration options for the MTC_Compression_Test_Import application.

## Configuration Files

The application uses the standard .NET configuration system with JSON files:

| File | Purpose |
|------|---------|
| `appsettings.json` | Base configuration (always loaded) |
| `appsettings.{Environment}.json` | Environment-specific overrides (optional) |

The environment is determined by the `ASPNETCORE_ENVIRONMENT` environment variable. If not set, defaults to `Production`.

---

## Configuration Sections

### 1. FileProcessing

The `FileProcessing` section controls the core file processing behavior.

```json
{
  "FileProcessing": {
    "SourceDirectory": "C:\\Path\\To\\Source",
    "ArchiveDirectory": "C:\\Path\\To\\Archive",
    "ErrorDirectory": "C:\\Path\\To\\Error",
    "EnableDatabaseWrites": false,
    "PreviewDatabaseWrites": true,
    "FilePattern": "*.txt",
    "PelletAcceptanceThreshold": 60,
    "StrictEndOfOperation": false
  }
}
```

#### Configuration Properties

| Property | Type | Default | Required | Description |
|----------|------|---------|----------|-------------|
| `SourceDirectory` | string | `""` | **Yes** | Directory path where compression test report files are placed for processing. The job monitors this directory for files matching `FilePattern`. |
| `ArchiveDirectory` | string | `""` | **Yes** | Directory path where successfully processed files are moved after parsing and database insertion. Created automatically if it doesn't exist. |
| `ErrorDirectory` | string | `""` | **Yes** | Directory path where files are moved when processing fails (parsing errors, duplicate files, database errors). Created automatically if it doesn't exist. |
| `EnableDatabaseWrites` | bool | `false` | No | When `true`, the job writes parsed compression data to the Oracle database (LAB_COMPRESSION and LAB_COMPRESSION_DTL tables). **Important:** Defaults to `false` to prevent accidental writes on new deployments. |
| `PreviewDatabaseWrites` | bool | `true` | No | When `true` and `EnableDatabaseWrites` is `false`, logs what database operations would be executed without actually performing writes. Useful for testing and validation. |
| `FilePattern` | string | `"*.txt"` | No | File glob pattern for matching report files in the source directory. |
| `PelletAcceptanceThreshold` | double | `100.0` | No | Minimum pellet number required to accept an incomplete test (when `end_of_operation` flag is not set). If the last pellet number is below this threshold and the test is incomplete, the behavior depends on `StrictEndOfOperation`. |
| `StrictEndOfOperation` | bool | `false` | No | When `true`, rejects files where the `end_of_operation` flag is not set and the last pellet number is below `PelletAcceptanceThreshold`. When `false` (lenient mode), accepts such files with a warning. |

#### Database Write Modes

| EnableDatabaseWrites | PreviewDatabaseWrites | Behavior |
|---------------------|----------------------|----------|
| `false` | `false` | Files are parsed and validated only. No database operations or preview logs. |
| `false` | `true` | Files are parsed. Preview logs show what would be written to DB. No actual writes. |
| `true` | (ignored) | Files are parsed and written to the Oracle database. |

---

### 2. Serilog

The `Serilog` section configures structured logging using the Serilog library.

```json
{
  "Serilog": {
    "Using": [ "Serilog.Sinks.Graylog", "Serilog.Sinks.Console", "Serilog.Sinks.File" ],
    "MinimumLevel": "Information",
    "Override": {
      "Microsoft.AspNetCore": "Warning"
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "theme": "Serilog.Sinks.SystemConsole.Themes.SystemConsoleTheme::Literate, Serilog.Sinks.Console"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "C:\\Logs\\MTC_Compression_Test_Import-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 180,
          "shared": true
        }
      },
      {
        "Name": "Graylog",
        "Args": {
          "hostnameOrAddress": "mno-logs.mno.uss.com",
          "port": "12202",
          "transportType": "Tcp"
        }
      }
    ]
  }
}
```

#### Serilog Properties

| Property | Description |
|----------|-------------|
| `Using` | List of Serilog sink assemblies to load. |
| `MinimumLevel` | Minimum log level to capture. Options: `Verbose`, `Debug`, `Information`, `Warning`, `Error`, `Fatal`. |
| `Override` | Per-namespace log level overrides. |
| `WriteTo` | Array of sink configurations. |

#### Log Sinks

**Console Sink:**
- Outputs colored log messages to the console.
- Useful for development and debugging.

**File Sink:**
| Property | Description |
|----------|-------------|
| `path` | Log file path. Date suffix is appended based on `rollingInterval`. |
| `rollingInterval` | How often to roll to a new file: `Day`, `Hour`, `Month`, etc. |
| `retainedFileCountLimit` | Maximum number of log files to retain. Older files are deleted. |
| `shared` | When `true`, allows multiple processes to write to the same log file. |

**Graylog Sink:**
| Property | Description |
|----------|-------------|
| `hostnameOrAddress` | Graylog server hostname or IP address. |
| `port` | Graylog GELF input port. |
| `transportType` | Transport protocol: `Tcp` or `Udp`. |

---

## Environment Variables

| Variable | Description |
|----------|-------------|
| `ASPNETCORE_ENVIRONMENT` | Determines which `appsettings.{Environment}.json` file to load. Common values: `Development`, `Staging`, `Production`. Defaults to `Production` if not set. |

---

## Configuration Precedence

Configuration values are loaded in the following order (later sources override earlier):

1. `appsettings.json` (base configuration)
2. `appsettings.{Environment}.json` (environment-specific)
3. Environment variables
4. Command-line arguments

---

## Example Configurations

### Development Configuration

```json
{
  "FileProcessing": {
    "SourceDirectory": "C:\\Dev\\TestFiles\\Source",
    "ArchiveDirectory": "C:\\Dev\\TestFiles\\Archive",
    "ErrorDirectory": "C:\\Dev\\TestFiles\\Error",
    "EnableDatabaseWrites": false,
    "PreviewDatabaseWrites": true,
    "PelletAcceptanceThreshold": 60,
    "StrictEndOfOperation": false
  }
}
```

### Production Configuration

```json
{
  "FileProcessing": {
    "SourceDirectory": "\\\\server\\share\\CompTest\\Incoming",
    "ArchiveDirectory": "\\\\server\\share\\CompTest\\Archive",
    "ErrorDirectory": "\\\\server\\share\\CompTest\\Error",
    "EnableDatabaseWrites": true,
    "PreviewDatabaseWrites": false,
    "PelletAcceptanceThreshold": 100,
    "StrictEndOfOperation": false
  }
}
```

---

## Exit Codes

The application returns the following exit codes to indicate processing status. These codes can be used by schedulers and scripts to detect success or failure.

| Code | Name | Description |
|------|------|-------------|
| 0 | Success | All files processed successfully. |
| 1 | ConfigurationError | Missing or invalid configuration (e.g., `FileProcessing` section missing). |
| 2 | SourceDirectoryNotFound | The configured source directory does not exist. |
| 3 | ProcessingError | One or more files failed to process. Check logs and error directory for details. |
| 4 | Canceled | Processing was canceled (e.g., via CancellationToken). |
| 5 | UnhandledException | An unexpected error occurred. Check logs for exception details. |

The exit code constants are defined in `Domain/ExitCodes.cs`.

---

## Validation Rules

The application validates configuration at startup:

| Validation | Behavior |
|------------|----------|
| `SourceDirectory` is null/empty | Job throws `InvalidOperationException`, returns exit code 5. |
| `ArchiveDirectory` is null/empty | Job throws `InvalidOperationException`, returns exit code 5. |
| `ErrorDirectory` is null/empty | Job throws `InvalidOperationException`, returns exit code 5. |
| `SourceDirectory` does not exist | Job logs error and returns exit code 2. |
| `ArchiveDirectory` does not exist | Directory is created automatically. |
| `ErrorDirectory` does not exist | Directory is created automatically. |
| `FileProcessing` section is missing | Job logs error and returns exit code 1. |

---

## Related Documentation

- [Test Specifications](../MTC_Compression_Test_Import.Tests/TestSpecifications.md) - Unit test documentation
- Oracle database schema: `LAB_COMPRESSION`, `LAB_COMPRESSION_DTL` tables
