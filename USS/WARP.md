# WARP.md

This file provides guidance to WARP (warp.dev) when working with code in this repository.

## Repository layout & key solutions

The main application code lives under `MOO - Plant Processes` and is organized into several .NET 8 solutions:

- `Apps/MOO.DAL/MOO.DAL.sln` – Core data access library (`MOO.DAL`) plus its xUnit test project (`DAL_UnitTest`). This layer encapsulates access to SQL Server, Oracle (`Oracle.ManagedDataAccess.Core`), WCF (`System.ServiceModel.*`), and other external systems. The DAL is split into domain-specific subfolders such as `Core`, `Blast`, `Drill`, `ERP`, `LIMS`, `LIMS_Report`, `MineStar`, `MinVu`, `Pi`, `ToLive`, `Warehouse`, `Wenco`, and `West_Main`.
- `Apps/MOO/MOO/MOO.csproj` – General-purpose shared library (`MOO`) packaged as a NuGet package and referenced by other projects (jobs, DAL, Blazor UI). Contains cross-cutting utilities and shared domain logic.
- `Apps/MOO_AD/MOO_AD/MOO_AD.csproj` – Active Directory integration library (`MOO.AD`) that wraps `System.DirectoryServices.AccountManagement` to expose user identity/role information. Packaged as `MOO.AD` and consumed by Blazor and job projects.
- `Apps/MOO.Blazor/MOO.Blazor.sln` – Blazor UI solution:
  - `MOO.Blazor/MOO.Blazor.csproj` – Razor component library targeting `net8.0` that depends on `MOO`, `MOO.AD`, `MOO.DAL`, `Radzen.Blazor`, and logging libraries.
  - `BlazorTest8/BlazorTest8.csproj` – Primary Blazor Server host application that wires up authentication, authorization, logging, and theme services, then hosts the `MOO.Blazor` components.
  - `BlazorTest/BlazorTest.csproj` – Additional/sample host used during development; `BlazorTest8` is the more complete reference.
- `Jobs/MTC_Compression_Test_Import/MTC_Compression_Test_Import.csproj` – Console job that parses compression test reports and writes to the back-end via `MOO` and `MOO.DAL`. Uses `Microsoft.Extensions.Hosting` and Serilog for structured logging.
- `Web/Bzr8/OM_Lab/OM_Lab.sln` – Legacy/ancillary web application for lab-related workflows. It follows a separate, older ASP.NET stack but relies on the same core libraries.

The `Nuget/` folder hosts third-party and internal NuGet packages used by the projects (for offline/controlled builds).

## .NET tooling prerequisites

All first-class projects target `net8.0`. Use the .NET 8 SDK (or later compatible SDK) for builds, tests, and running applications.

## Common commands

All commands below assume the working directory is the repo root (`C:\projects\cnb\uss` or equivalent).

### Build core libraries and tests

Build the data access layer and its tests:

```powershell path=null start=null
dotnet build "MOO - Plant Processes/Apps/MOO.DAL/MOO.DAL.sln" -c Debug
```

Build the shared libraries individually (useful when iterating on them in isolation):

```powershell path=null start=null
# General utilities library
dotnet build "MOO - Plant Processes/Apps/MOO/MOO/MOO.csproj" -c Debug

# Active Directory integration library
dotnet build "MOO - Plant Processes/Apps/MOO_AD/MOO_AD/MOO_AD.csproj" -c Debug
```

### Build and run the Blazor Server host

Build the Blazor UI solution (component library plus host apps):

```powershell path=null start=null
dotnet build "MOO - Plant Processes/Apps/MOO.Blazor/MOO.Blazor.sln" -c Debug
```

Run the `BlazorTest8` Blazor Server host (uses interactive server components, Windows/Negotiate auth, Radzen, and Serilog):

```powershell path=null start=null
dotnet run --project "MOO - Plant Processes/Apps/MOO.Blazor/BlazorTest8/BlazorTest8.csproj" -c Debug
```

This host:
- Builds configuration from `appsettings.json` and `appsettings.{Environment}.json`.
- Configures Serilog via appsettings, enriching logs with `ServerType` and `Program` (from `Util.PROGRAM_NAME`).
- Enables Negotiate (Windows) authentication and enforces authorization via the default/fallback policy.
- Registers Radzen components, theme cookie service, and `MOO.Blazor.Security.UserInfoClaims` to augment user claims from Active Directory.

### Run tests

The `DAL_UnitTest` project is an xUnit test project that references `MOO.DAL`.

Run all DAL tests:

```powershell path=null start=null
dotnet test "MOO - Plant Processes/Apps/MOO.DAL/DAL_UnitTest/DAL_UnitTest.csproj" -c Debug
```

Run a single test (or a subset) by name using xUnit filters, for example:

```powershell path=null start=null
# Filter by test method name
dotnet test "MOO - Plant Processes/Apps/MOO.DAL/DAL_UnitTest/DAL_UnitTest.csproj" `
  -c Debug --filter "Name=SomeTestMethodName"

# Filter by fully-qualified name pattern
dotnet test "MOO - Plant Processes/Apps/MOO.DAL/DAL_UnitTest/DAL_UnitTest.csproj" `
  -c Debug --filter "FullyQualifiedName~DAL_UnitTest.SomeTestClass"
```

Note: All DAL tests inherit from `BaseTestClass`, which calls `Util.CheckDMARTIsDev()` in the constructor. Ensure your environment points at a development/non-production DMART instance before running tests.

### Run the compression import job

Build and run the compression test import console job:

```powershell path=null start=null
dotnet run --project "MOO - Plant Processes/Jobs/MTC_Compression_Test_Import/MTC_Compression_Test_Import/MTC_Compression_Test_Import.csproj" -c Debug
```

The job reads configuration from `appsettings.json` / `appsettings.Development.json` (logging, connection details, and file locations) and uses `MOO` + `MOO.DAL` to persist parsed results.

### Linting and static analysis

There is no dedicated linting or analyzer configuration in this repository beyond what the C# compiler and referenced packages provide. Use `dotnet build` (or your IDE) to surface warnings and errors; any additional analyzers or formatting tools should be configured per-team and added explicitly to the projects.

## Architectural overview

At a high level, the solution follows a layered architecture built around reusable .NET 8 class libraries and a Blazor Server UI:

- **Shared libraries (MOO & MOO.AD)**
  - `MOO` is a general-purpose library used by the DAL, jobs, and UI. It centralizes common logic (e.g., configuration helpers, HTTP helpers, shared domain types).
  - `MOO.AD` encapsulates Active Directory access via `System.DirectoryServices.AccountManagement`, providing higher-level user and role information used for authorization and personalization.

- **Data access and integration layer (MOO.DAL)**
  - `MOO.DAL` is the central integration point for external systems and databases, exposing a stable API surface to the rest of the stack.
  - It depends on `Microsoft.Data.SqlClient`, `Oracle.ManagedDataAccess.Core`, `System.Data.OleDb`, and various `System.ServiceModel.*` packages to connect to SQL Server, Oracle, older OLE DB data sources, and WCF services.
  - The folder structure partitions responsibilities by upstream system or domain (e.g., `LIMS`, `Pi`, `ERP`, `MineStar`, `Wenco`). Each subfolder typically contains repositories, DTOs, and integration-specific logic.
  - Embedded XML resources under `LIMS/LIMSML_XML` define templates for communicating with LIMS; the DAL code loads these at runtime.
  - `DAL_UnitTest` mirrors this structure with corresponding tests, all sharing a `BaseTestClass` that enforces running against a development DMART environment.

- **Blazor UI layer (MOO.Blazor + BlazorTest8)**
  - `MOO.Blazor` is a Razor component library meant to be embedded into host applications. It:
    - Targets `net8.0` and `browser` (via `SupportedPlatform`), making it suitable for interactive Blazor components.
    - Uses `Radzen.Blazor` for rich UI controls and binds to back-end data via `MOO.DAL` abstractions and shared models from `MOO`.
    - Organizes UI into:
      - `Components/` – Reusable UI components (e.g., `AnomaliesView`, `DocumentListView`, `MetricValueEntry`, `MetricValueAdjust`, `EmailSubscription`, `PortalMenu`) plus dialog subcomponents.
      - `Pages/` – Full-page workflows (e.g., `Pulse_Analog_Check`).
      - `Security/` – Types like `UserInfoClaims` and a custom `MOOClaimsPrincipal` used to enrich and interpret user identities.
      - `UI/` – Higher-level layout, navigation, and shared presentation helpers (not exhaustively documented here; explore as needed).
  - `BlazorTest8` wires these components into a Blazor Server host:
    - Configures services in `Program.cs` using the minimal hosting model (`WebApplication.CreateBuilder`).
    - Registers `AddRazorComponents().AddInteractiveServerComponents()` and maps them via `app.MapRazorComponents<App>().AddInteractiveServerRenderMode()`.
    - Sets up Negotiate authentication (`AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate()`) and uses the default policy as a fallback for all routes.
    - Adds Radzen services (including `AddRadzenCookieThemeService`) and `AddHttpClient` for back-end calls.
    - Uses `MOO.Blazor.Security.UserInfoClaims` as an `IClaimsTransformation` to attach domain-specific claims to authenticated users.
    - Uses Serilog (configured from `appsettings*.json`) for structured logging including Graylog sinks.

- **Batch processing and legacy web entry points**
  - Batch jobs such as `MTC_Compression_Test_Import` are console apps that:
    - Use `Microsoft.Extensions.Hosting` to bootstrap a host, logging, and configuration.
    - Depend on `MOO` and `MOO.DAL` to implement domain-specific data ingestion and persistence.
  - Legacy or specialized web applications (e.g., `Web/Bzr8/OM_Lab`) consume the same core libraries, ensuring consistent business logic and data access across UI technologies.

This structure allows the core business logic and integrations to live in shared libraries (`MOO`, `MOO.AD`, `MOO.DAL`), while multiple front-ends (Blazor Server, console jobs, legacy web) consume those libraries via direct project references or NuGet packages.