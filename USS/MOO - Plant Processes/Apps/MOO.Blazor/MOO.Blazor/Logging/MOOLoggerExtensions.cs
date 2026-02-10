using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.Blazor.Logging
{
    public static class MOOLoggerExtensions
    {
        public static ILoggingBuilder AddMOOFileLogger(this ILoggingBuilder builder, Action<MOOLoggerOptions> configure)
        {
            builder.Services.AddSingleton<ILoggerProvider, MOOLoggerProvider>();
            builder.Services.Configure(configure);
            return builder;
        }
    }
}
