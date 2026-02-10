using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO
{
    /// <summary>
    /// Class used for general functions that don't fit anywhere
    /// </summary>
    public static class General
    {
        /// <summary>
        /// Returns if the value is a number
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumber(object value)
        {
            return value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal;
        }
    }
}
