using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO_UnitTest
{
    public static class Util
    {
        public const string PROGRAM_NAME = "MOO_UnitTest";
        //Change these test users to yourself to view the emails sent from testing
        public const string TEST_EMAIL_ADDRESS = "bpaltman@uss.com";
        /// <summary>
        /// Test user id, must contain the domain (example mno\abc1234)
        /// </summary>
        public const string TEST_USER_ID = "mno\\alt7747";


        public const string TEST_USER_ID2 = "mno\\ctz0621";

        /// <summary>
        /// Registers the DB Factories with the MOO Library
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "We only run windows")]        public static void RegisterFactories()
        {

            DbProviderFactories.RegisterFactory(MOO.Data.DBType.Oracle.ToString(), Oracle.ManagedDataAccess.Client.OracleClientFactory.Instance);
            DbProviderFactories.RegisterFactory(MOO.Data.DBType.SQLServer.ToString(), Microsoft.Data.SqlClient.SqlClientFactory.Instance);
            DbProviderFactories.RegisterFactory(MOO.Data.DBType.OLEDB.ToString(), System.Data.OleDb.OleDbFactory.Instance);

        }
    }

}
