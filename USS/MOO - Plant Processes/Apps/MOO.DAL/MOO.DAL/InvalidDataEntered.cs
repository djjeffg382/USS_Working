using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL
{
    /// <summary>
    /// Class used for throwing an exception if data is missing or invalid
    /// </summary>
    [Obsolete("Used MOO.DAL.InvalidDataException")]
    public class InvalidDataEntered : Exception
    {
        public string ErrMessage { get; set; }
        public string Field { get; set; }
        public InvalidDataEntered(string message, string fieldName)
        {
            ErrMessage = message;
            Field = fieldName;
        }
        public override string Message => ErrMessage;
    }
}
