using System;
using System.Data.Common;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MTC_Compression_Test_Import.Domain;
using Serilog;

namespace MTC_Compression_Test_Import
{
    internal class Program
    {
        static int Main(string[] args)
        {
            int exitCode = ExitCodes.Success;
            string ServerType = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            //This line needed to register the Oracle client with the MOO library
            //Error logging goes to Oracle so this is needed on all apps using the MOO library
            DbProviderFactories.RegisterFactory(MOO.Data.DBType.Oracle.ToString(), Oracle.ManagedDataAccess.Client.OracleClientFactory.Instance);
            //Add the following line if SQL Server access is needed.  NuGet Package for SqlClient needs to be installed
            //DbProviderFactories.RegisterFactory(MOO.Data.DBType.SQLServer.ToString(), Microsoft.Data.SqlClient.SqlClientFactory.Instance);

            //Add this package if queries to PI will be used, Oledb NuGet package must be installed
            //DbProviderFactories.RegisterFactory(MOO.Data.DBType.OLEDB.ToString(), System.Data.OleDb.OleDbFactory.Instance);

            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ServerType", ServerType)
                .Enrich.WithProperty("Program", Util.PROGRAM_NAME)
                .CreateLogger();

            Log.Information("Starting {Program} with ServerType={ServerType}.", Util.PROGRAM_NAME, ServerType);

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<Main>();
                })
                .UseSerilog()
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.SetBasePath(AppContext.BaseDirectory);
                    configApp.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    configApp.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true);
                    configApp.AddEnvironmentVariables();
                    configApp.AddCommandLine(args);
                })
                .Build();

            var svc = ActivatorUtilities.CreateInstance<Main>(host.Services);

            if (ServerType == "Production")
            {
                try
                {
                    //This calls the run for the application and is the starting point for your code
                    exitCode = svc.Run();
                }
                catch (Exception ex)
                {
                    //If server type is set to production, log error to Error table
                    if (MOO.Settings.ServerType == MOO.Settings.ServerClass.Production)
                    {
                        MOO.Exceptions.ErrorLog.LogError(Util.PROGRAM_NAME, ex);
                    }
                    StringBuilder ErrMsg = new();
                    ErrMsg.AppendLine($"Error - {ex.Message}");
                    ErrMsg.AppendLine($"Stack - {ex.StackTrace}");
                    Log.Fatal(ErrMsg.ToString());
                    exitCode = ExitCodes.UnhandledException;
                }
                finally
                {
                    Log.CloseAndFlush();
                }
            }
            else
            {
                //don't run program with try..catch.  This will enusre program breaks on exception
                exitCode = svc.Run();
                //Wait one second to make sure Log service started to log anything, if program quits to quickly, no logs will be created as it didn't get fully connected
                System.Threading.Thread.Sleep(1000);
                Log.CloseAndFlush();
            }

            return exitCode;
        }

        static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables();
        }
    }
}
