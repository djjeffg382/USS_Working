using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL_UnitTest
{
    static class Util
    {
        /// <summary>
        /// checks the connection string to ensure our connection to dmart is the dev database
        /// Throws exception if not connected to dev, we don't want to modify production database
        /// </summary>
        public static void CheckDMARTIsDev()
        {
            string connString = MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART);
            if (!connString.Contains("dmartd"))
                throw new Exception("Connection string is set to Production database");
        }
    }
}
