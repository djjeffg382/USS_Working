using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL
{
    public static class Util
    {

        private static bool OracleIsRegistered = false;
        private static bool OLEDBIsRegistered = false;
        private static bool SqlIsRegistered = false;



        /// <summary>
        /// Register the Oracle client so MOO can use it
        /// </summary>
        public static void RegisterOracle()
        {
            if (!OracleIsRegistered)
            {
                DbProviderFactories.RegisterFactory(MOO.Data.DBType.Oracle.ToString(), Oracle.ManagedDataAccess.Client.OracleClientFactory.Instance);
                OracleIsRegistered = true;
            }

        }
        public static void RegisterOLEDB()
        {
            if (!OLEDBIsRegistered)
            {
                DbProviderFactories.RegisterFactory(MOO.Data.DBType.OLEDB.ToString(), System.Data.OleDb.OleDbFactory.Instance);
                OLEDBIsRegistered = true;
            }

        }


        public static void RegisterSqlServer()
        {
            if (!SqlIsRegistered)
            {
                DbProviderFactories.RegisterFactory(MOO.Data.DBType.SQLServer.ToString(), Microsoft.Data.SqlClient.SqlClientFactory.Instance);
                SqlIsRegistered = true;
            }

        }

        /// <summary>
        /// Gets the value of the column name
        /// </summary>
        /// <param name="rdr"></param>
        /// <param name="ColName"></param>
        /// <returns></returns>
        internal static object GetRowVal(DbDataReader rdr, string ColName)
        {
            //the reader object does not have a way to get value by column name so we must first get the ordinal
            int colNbr = rdr.GetOrdinal(ColName);

            //return null if the data is dbnull
            if (rdr.IsDBNull(colNbr))
                return null;
            else
                return rdr.GetValue(colNbr);
        }



    }
}
