using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Pi
{
    /// <summary>
    /// used for capturing the exception when we are attempting to connect to a specific PI server (Primary/Secondary) at it fails
    /// </summary>
    internal class FailedPIServerConnectionException:Exception
    {
        public string ErrMessage { get; set; }
        public string Server { get; set; }
        public FailedPIServerConnectionException(string Message, string Server)
        {
            ErrMessage = Message;
            this.Server = Server;
        }
        public override string Message => ErrMessage;
    }
}
