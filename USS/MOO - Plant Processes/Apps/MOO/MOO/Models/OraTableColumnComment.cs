using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.Models
{
    /// <summary>
    /// Comments for Oracle Table Columns
    /// </summary>
    public sealed class OraTableColumnComment
    {
        /// <summary>
        /// Name of the column in the table
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// Comments for the given column
        /// </summary>
        public string Comments { get; set; }
    }
}
