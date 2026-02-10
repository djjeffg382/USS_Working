using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//https://www.roundthecode.com/dotnet/create-your-own-logging-provider-to-log-to-text-files-in-net-core
namespace MOO.Blazor.Logging
{
    /// <summary>
    /// Class used for logging errors to the TOLIVE.ERROR table in Oracle
    /// </summary>
    public class MOOLogger : ILogger
    {
        protected readonly MOOLoggerProvider _MOOLoggerFileProvider;

        public MOOLogger([NotNull] MOOLoggerProvider mooLoggerFileProvider)
        {
            _MOOLoggerFileProvider = mooLoggerFileProvider;
        }

        IDisposable ILogger.BeginScope<TState>(TState state)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return null;
#pragma warning restore CS8603 // Possible null reference return.
        }

        bool ILogger.IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
#pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
        {
            if (!((ILogger)this).IsEnabled(logLevel))
            {
                return;
            }
            if((logLevel == LogLevel.Error || logLevel == LogLevel.Critical))
            {
                if (exception != null)
                {
                    MOO.Exceptions.ErrorLog.LogError(_MOOLoggerFileProvider.Options.ProgramName, exception);
                }                   
            }

        }
    }
}
