using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.Exceptions 
{
    /// <summary>
    /// This will be thrown from the data.executsql method if it fails
    /// </summary>
    public class MOOSqlException : Exception
    {

        private readonly string _SQLString;
        private readonly Exception _ExceptionThrown;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ex">The exception that occurred on running the sql</param>
        /// <param name="Sql">The sql that was run</param>
        public MOOSqlException(Exception ex, string Sql)
        {
            this._SQLString = Sql;
            this._ExceptionThrown = ex;
            
        }

        /// <summary>
        /// Original exception error Message
        /// </summary>
        public override string Message
        {
            get { return _ExceptionThrown.Message; }
        }
        /// <summary>
        /// Exception Stack Trace
        /// </summary>
        public override string StackTrace
        {
            get { return _ExceptionThrown.StackTrace; }
        }
        /// <summary>
        /// The SQL String that errored
        /// </summary>
        public string SQLString
        {
            get { return _SQLString; }
        }

    }
}
