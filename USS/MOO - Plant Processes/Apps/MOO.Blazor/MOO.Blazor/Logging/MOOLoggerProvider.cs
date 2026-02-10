using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.Blazor.Logging
{
    [ProviderAlias("MOOLogger")]
    public class MOOLoggerProvider : ILoggerProvider
    {
        public readonly MOOLoggerOptions Options;

        public MOOLoggerProvider(IOptions<MOOLoggerOptions> _options)
        {
            Options = _options.Value;

            
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new MOOLogger(this);
        }

        public void Dispose()           
        {
            GC.SuppressFinalize(this);
        }
    }
}
