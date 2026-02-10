using MOO.DAL.Drill.Models;
using System.Data.Common;
using System;
using System.Collections.Generic;

namespace MOO.DAL.Drill.Imports
{
    public abstract class BaseImport
    {
        public readonly MOO.Plant Plant;
        private static bool OLEDBIsRegistered = false;
        public BaseImport(MOO.Plant Plant)
        {
            this.Plant = Plant;
            if (!OLEDBIsRegistered)
            {
                DbProviderFactories.RegisterFactory(MOO.Data.DBType.SQLServer.ToString(), Microsoft.Data.SqlClient.SqlClientFactory.Instance);
                OLEDBIsRegistered = true;
            }
            
        }

        /// <summary>
        /// Gets the raw data from the source system and returns a list of the Raw Data objects.  This will then be transalted to be inserted into the Drilled_Hole table
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public abstract List<Raw_Drilled_Hole> GetRawData(DateTime StartDate, DateTime EndDate);




    }
}
