using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using MOO.Models;
using System.Text;
using System.Threading.Tasks;
//using Oracle.ManagedDataAccess.Client;

namespace MOO
{



    /// <summary>
    /// Class of abstract methods for dealing with MNO Databases
    /// </summary>
    public static class Data
    {
        /// <summary>
        /// Number of times to retry a query before giving up
        /// </summary>
        private const int RECURSIVE_SQL_RETRIES = 5;

        /// <summary>
        /// File location of the connection strings to databases
        /// </summary>
        private const string CONN_STRING_FILE = "C:\\MOOGlobalSettings\\Connections.json";

        private const string DEFAULT_TNSNAMES_FILE = @"C:\MOOGlobalSettings\tnsnames.ora";

        #region "Enums"
        /// <summary>
        /// Database Connection type
        /// </summary>
        public enum DBType
        {
            /// <summary>
            /// Oracle Database
            /// </summary>
            Oracle, 
            /// <summary>
            /// Microsoft SQL Server
            /// </summary>
            SQLServer,
            /// <summary>
            /// OLEDB Connection
            /// </summary>
            OLEDB
        }




        /// <summary>
        /// Common MOO Databases
        /// </summary>
        /// <remarks>To add a database, Add the enum and then add the connection to "C:\MOOGlobalSettings\Connections.json" using the enum name you created</remarks>
        public enum MNODatabase
        {
            /// <summary>
            /// Corporate delays database
            /// </summary>
            CorpDelays,
            /// <summary>
            /// Oracle DMART database
            /// </summary>
            DMART,
            /// <summary>
            /// Minntac Wenco database
            /// </summary>
            MTC_Wenco,
            /// <summary>
            /// Keetac Wenco database
            /// </summary>
            KTC_Wenco,
            /// <summary>
            /// Connection used for all drill databases on MNO-SQL02
            /// </summary>
            USSDrillData,
            /// <summary>
            /// Minntac MinVu database
            /// </summary>
            MTC_MinVu,
            /// <summary>
            /// Keetac MinVu database
            /// </summary>
            KTC_Minvu,
            /// <summary>
            /// Minntac PI Connection
            /// </summary>
            MTC_Pi,
            /// <summary>
            /// Minntac PI AF Server connection
            /// </summary>
            MTC_PI_AF,
            /// <summary>
            /// Minntac PI Connection using UTC Time
            /// </summary>
            MTC_PI_UTC,
            /// <summary>
            /// Connection to DMART as user BSCAdmin, used for admin type queries
            /// </summary>
            OraBscAdmin,
            /// <summary>
            /// Connection to the Minntac Mine reporting database
            /// </summary>
            [Obsolete("Use MtcWencoReport instead, this connection not used anymore")]
            MtcMineReport,
            /// <summary>
            /// Connection to the Minntac Mine reporting database
            /// </summary>
            [Obsolete("Use KtcWencoReport instead, this connection not used anymore")]
            KtcMineReport,
            /// <summary>
            /// Connection to the Sample manager database to read data
            /// </summary>
            LIMS_Read,
            /// <summary>
            /// Connection to the LIMS_Report ETL database
            /// </summary>
            LIMS_Report,
            /// <summary>
            /// Minntac Wenco reporting server
            /// </summary>
            MtcWencoReport,
            /// <summary>
            /// Keetac Wenco Reporting server
            /// </summary>
            KtcWencoReport,
            /// <summary>
            /// MOO Minestar Health Database server
            /// </summary>
            MineStar,
            /// <summary>
            /// Generic connection to MNO-SQL01 database
            /// </summary>
            MNO_SQL01
                

        }
        #endregion





        #region "DB Conn File Settings"
        private static readonly Dictionary<string, DBConnection> _DBConnections = [];

        /// <summary>
        /// Name value pair of Database Connections
        /// </summary>
        public static Dictionary<string, DBConnection> DBConnections
        {
            get
            {
                //added lock for threading.  Unit test was running asynch and was crashing here because of
                //multiple threads trying to modify _DBConnections at one time
                lock (_DBConnections)
                {
                    if (_DBConnections.Count==0)
                    {

                        string dbName;
                        DBConnection NewDB;
                        //create DB Connections Object, this is first read
                        //Parse the JSON file
                        string myJsonString = File.ReadAllText(CONN_STRING_FILE);
                        dynamic dynObj = Newtonsoft.Json.JsonConvert.DeserializeObject(myJsonString);

                        foreach (var conn in dynObj.Connections)
                        {
                            dbName = conn.Name;
                            NewDB = new DBConnection
                            {
                                ConnectionString = conn.ConnectionString,
                                DBType = conn.DBType
                            };
                            _DBConnections.Add(dbName, NewDB);

                        }
                    }
                }
                return _DBConnections;
            }
        }

        /// <summary>
        /// Used for storing information of the parsed connections json file
        /// </summary>
        public struct DBConnection
        {
            /// <summary>
            /// Connection String to the database
            /// </summary>
            public string ConnectionString;
            /// <summary>
            /// The database type
            /// </summary>
            public DBType DBType;

        }
        #endregion


        /// <summary>
        /// Executes the given SQL string and retries based on certain errors
        /// </summary>
        /// <param name="Sql"></param>
        /// <param name="Db"></param>
        /// <param name="Recursion"></param>
        private static async Task<int> ExecuteNonQueryAsync(string Sql, MNODatabase Db, int Recursion)
        {
            bool retry;
            MOO.Exceptions.MOOSqlException SQLEx;
            int RecsChanged = 0;
            DBConnection dbConn = DBConnections[Db.ToString()];
            String ConnString = dbConn.ConnectionString;

            System.Data.Common.DbConnection conn;
            System.Data.Common.DbCommand cmd;
            System.Data.Common.DbProviderFactory Factory = System.Data.Common.DbProviderFactories.GetFactory(dbConn.DBType.ToString());
            conn = Factory.CreateConnection();
            conn.ConnectionString = ConnString;
            cmd = Factory.CreateCommand();
            cmd.Connection = conn;
            cmd.CommandText = Sql;
            try
            {
                await conn.OpenAsync();
                RecsChanged = await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                retry = CheckIfShouldRetryQuery(ex);
                if (retry && Recursion < RECURSIVE_SQL_RETRIES)
                    await ExecuteNonQueryAsync(Sql, Db, Recursion + 1);
                else
                {
                    SQLEx = new Exceptions.MOOSqlException(ex, Sql);
                    throw (SQLEx);
                }

            }
            finally
            {
                await conn.CloseAsync();
            }
            return RecsChanged;
        }

        /// <summary>
        /// Executes the given SQL string and retries based on certain errors returning a data reader
        /// </summary>
        /// <param name="Cmd"></param>
        /// <param name="Recursion"></param>
        /// <returns></returns>
        private static async Task<int> ExecuteNonQueryAsync(System.Data.Common.DbCommand Cmd, int Recursion)
        {
            bool retry;
            MOO.Exceptions.MOOSqlException SQLEx;
            int RecsChanged = 0;
            try
            {
                RecsChanged = await Cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                retry = CheckIfShouldRetryQuery(ex);
                if (retry && Recursion < RECURSIVE_SQL_RETRIES)
                    await ExecuteNonQueryAsync(Cmd, Recursion + 1);
                else
                {
                    SQLEx = new Exceptions.MOOSqlException(ex, Cmd.CommandText);
                    throw (SQLEx);
                }
            }
            return RecsChanged;
        }

        /// <summary>
        /// Executes the given SQL string and retries based on certain errors
        /// </summary>
        /// <param name="Cmd"></param>
        /// <param name="Recursion"></param>
        /// <returns></returns>
        private static async Task<System.Data.Common.DbDataReader> ExecuteReaderAsync(System.Data.Common.DbCommand Cmd, int Recursion)
        {
            bool retry;
            MOO.Exceptions.MOOSqlException SQLEx;
            System.Data.Common.DbDataReader retVal = null;
            try
            {
                retVal = await Cmd.ExecuteReaderAsync();
            }
            catch (Exception ex)
            {
                retry = CheckIfShouldRetryQuery(ex);
                if (retry && Recursion < RECURSIVE_SQL_RETRIES)
                    retVal = await ExecuteReaderAsync(Cmd, Recursion + 1);
                else
                {
                    SQLEx = new Exceptions.MOOSqlException(ex, Cmd.CommandText);
                    throw (SQLEx);
                }
            }
            return retVal;
        }



        /// <summary>
        /// Fills the datatable given,
        /// </summary>
        /// <param name="da"></param>
        /// <param name="dt"></param>
        /// <param name="recursion"></param>
        /// <remarks>This is used to handle any normal SQL errors and perform a retry</remarks>
        private static void FillDataTable(System.Data.Common.DbDataAdapter da, System.Data.DataTable dt, int recursion)
        {
            Exceptions.MOOSqlException SQLEx;
            bool retry;
            string cmd;
            try
            {
                da.Fill(dt);
            }catch(Exception ex)
            {
                retry = CheckIfShouldRetryQuery(ex);
                if(retry && recursion < RECURSIVE_SQL_RETRIES)
                {
                    FillDataTable(da, dt, recursion + 1);
                }
                else
                {
                    cmd = da.SelectCommand.CommandText;
                    SQLEx = new Exceptions.MOOSqlException(ex, cmd);
                    throw SQLEx;
                }
            }
        }


        /// <summary>
        /// Executes an sql query and returns the result in a dataset
        /// </summary>
        /// <param name="Sql">The SQL to execute</param>
        /// <param name="DB">The database to execute the SQL</param>
        /// <param name="recursion">how many recursive times this has been called</param>
        /// <returns></returns>
        /// <remarks>This was created to handle when Oracle kills connections.  The recursion parameter is used so we don't
        ///get an infinite loop.  This should initially be called with zero for recursion and each time this function calls itself
        ///it will increment the recursion counter. This should be a private function as this should only be called within the class
        ///</remarks>
        private static DataSet ExecuteQuery(String Sql, MNODatabase DB, int recursion)
        {
            bool retry;
            MOO.Exceptions.MOOSqlException SQLEx;

            DataSet ds = new();
            DBConnection dbConn = DBConnections[DB.ToString()];
            String ConnString = dbConn.ConnectionString;

            System.Data.Common.DbConnection conn;
            System.Data.Common.DbDataAdapter da;
            System.Data.Common.DbProviderFactory Factory = System.Data.Common.DbProviderFactories.GetFactory(dbConn.DBType.ToString());
            conn = Factory.CreateConnection();
            conn.ConnectionString = ConnString;
            da = Factory.CreateDataAdapter();
            da.SelectCommand = Factory.CreateCommand();
            da.SelectCommand.Connection = conn;
            da.SelectCommand.CommandText = Sql;
            try
            {
                conn.Open();
                da.Fill(ds);
            }
            catch(Exception ex)
            {
                retry = CheckIfShouldRetryQuery(ex);
                if (retry && recursion < RECURSIVE_SQL_RETRIES)
                    return ExecuteQuery(Sql, DB, recursion + 1);
                else
                {
                    SQLEx = new Exceptions.MOOSqlException(ex, Sql);
                    throw (SQLEx);
                }

            }
            finally
            {
                conn.Close();
            }
            return ds;
        }


        /// <summary>
        /// Checks if we should retry the query based on the exception that occurs
        /// </summary>
        /// <param name="ex">The exception that occurred</param>
        /// <returns></returns>
        /// <remarks>Sometimes Oracle queries fail because of orphaned connections being disconnected, a simple requery will fix this</remarks>
        private static bool CheckIfShouldRetryQuery(Exception ex)
        {
            bool retry = false;

            /*
            *  ora-04068 is "existing state of packages has been discarded"  
            *  ORA-12571 TNS:packet writer failure - happens when Oracle resets from a shutdown
            *  ORA-01012 not logged on - This happens when Oracle kills the connection and vb.net does not know about it yet
            *  ORA-02396 Connection idle timeout - This happens when Oracle kills the connection and vb.net does not know about it yet
            *  ORA-03113 end-of-file on communication channel - This happens when a node on the RAC fails
            *  ORA-03135 connection lost contact - This happens when a node on the RAC fails
            *  deadlocked on lock resources - SQL Server occasionally sends this message
             * */
            if (ex.Message.Contains("ORA-04068") ||
                                ex.Message.Contains("ORA-12571") ||
                                ex.Message.Contains("ORA-01012") ||
                                ex.Message.Contains("ORA-02396") ||
                                ex.Message.Contains("ORA-03113") ||
                                ex.Message.Contains("ORA-03135") ||
                                ex.Message.Contains("Connection request timed out") ||
                                ex.Message.Contains("deadlocked on lock resources "))
            {
                System.Threading.Thread.Sleep(100);
                retry = true;
            }
                


            return retry;
        }




        #region "Public Functions"

        /// <summary>
        /// Fills the dataset and handles common Oracle/SQL Server errors
        /// </summary>
        /// <param name="da"></param>
        /// <param name="dt"></param>
        public static void FillDataTable(System.Data.Common.DbDataAdapter da, System.Data.DataTable dt)
        {
            FillDataTable(da, dt, 0);
        }

        /// <summary>
        /// Executes the dataadapter query to return a dataset
        /// </summary>
        /// <param name="da"></param>
        /// <returns></returns>
        public static DataSet ExecuteQuery(System.Data.Common.DbDataAdapter da)
        {
            DataSet ds = new();
            ds.Tables.Add(new DataTable());
            FillDataTable(da, ds.Tables[0]);
            return ds;
        }

        /// <summary>
        /// Executes the SQL Statement on the database and returns a dataset
        /// </summary>
        /// <param name="Sql">The SQL to be run on the database</param>
        /// <param name="DB">The database</param>
        /// <returns></returns>
        public static DataSet ExecuteQuery(String Sql, MNODatabase DB)
        {
            return ExecuteQuery(Sql, DB, 0);

        }

        /// <summary>
        /// Executes a SQL command that doesn't return any thing (example: Update or Delete command)
        /// </summary>
        /// <param name="Sql"></param>
        /// <param name="Db"></param>
        public static int ExecuteNonQuery(string Sql, MNODatabase Db)
        {
           return ExecuteNonQueryAsync(Sql, Db, 0).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Executes a SQL command that doesn't return any thing (example: Update or Delete command)
        /// </summary>
        /// <param name="Sql"></param>
        /// <param name="Db"></param>
        public static async Task<int> ExecuteNonQueryAsync(string Sql, MNODatabase Db)
        {
            return await ExecuteNonQueryAsync(Sql, Db, 0);
        }

        /// <summary>
        /// Executes the given Command and retries based on certain errors
        /// </summary>
        /// <param name="Cmd"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(System.Data.Common.DbCommand Cmd)
        {
           return ExecuteNonQueryAsync(Cmd, 0).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Executes the given Command and retries based on certain errors
        /// </summary>
        /// <param name="Cmd"></param>
        /// <returns></returns>
        public static async Task<int> ExecuteNonQueryAsync(System.Data.Common.DbCommand Cmd)
        {
            return await ExecuteNonQueryAsync(Cmd, 0);
        }


        /// <summary>
        /// Executes the given Command and retries based on certain errors retruning a datareader
        /// </summary>
        /// <param name="Cmd"></param>
        /// <returns></returns>
        public static System.Data.Common.DbDataReader ExecuteReader(System.Data.Common.DbCommand Cmd)
        {
            return ExecuteReaderAsync(Cmd, 0).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Executes the given Command and retries based on certain errors retruning a datareader
        /// </summary>
        /// <param name="Cmd"></param>
        /// <returns></returns>
        public static async Task<System.Data.Common.DbDataReader> ExecuteReaderAsync(System.Data.Common.DbCommand Cmd)
        {
            return await ExecuteReaderAsync(Cmd, 0);
        }

        /// <summary>
        /// Gets the connection string from the Connections.Json file
        /// </summary>
        /// <param name="DB">Name of the database in the file</param>
        /// <returns></returns>
        public static string GetConnectionString(MNODatabase DB)
        {
            DBConnection db = DBConnections[DB.ToString()];
            return db.ConnectionString;
        }

        /// <summary>
        /// Creates an Oracle EZConnect connection string from the tnsnames.ora file
        /// </summary>
        /// <param name="tnsEntry">TNS Entry Name</param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="tnsFile">Location of the TNS File</param>
        /// <returns></returns>
        public static string GetConnectionStringFromTNSNames(string tnsEntry, string userName, string password, string tnsFile = DEFAULT_TNSNAMES_FILE){
            var entries = TNSNamesOraParser.ParseTnsNamesFile(tnsFile);
            var found = entries.FirstOrDefault(x => string.Equals(x.Entry,tnsEntry,StringComparison.OrdinalIgnoreCase));
            if (found != null)
                return $"Data Source={found.Value};user id={userName};password={password}";
            else return null ;
        }


        /// <summary>
        /// Executes the query and returns the first row, first column value
        /// </summary>
        /// <param name="Sql"></param>
        /// <param name="Db"></param>
        /// <returns></returns>
        public static async Task<object> ExecuteScalarAsync(string Sql, MNODatabase Db)
        {
            MOO.Exceptions.MOOSqlException SQLEx;
            DBConnection dbConn = DBConnections[Db.ToString()];
            String ConnString = dbConn.ConnectionString;

            System.Data.Common.DbConnection conn;
            System.Data.Common.DbCommand cmd;
            System.Data.Common.DbProviderFactory Factory = System.Data.Common.DbProviderFactories.GetFactory(dbConn.DBType.ToString());
            conn = Factory.CreateConnection();
            conn.ConnectionString = ConnString;
            cmd = Factory.CreateCommand();
            cmd.Connection = conn;
            cmd.CommandText = Sql;
            try
            {
                await conn.OpenAsync();
                var rdr = await ExecuteReaderAsync(cmd);
                if (rdr.HasRows)
                {
                    await rdr.ReadAsync();
                    return rdr[0];
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                SQLEx = new Exceptions.MOOSqlException(ex, Sql);
                throw (SQLEx);
            }
            finally
            {
                await conn.CloseAsync();
            }
        }


        /// <summary>
        /// Executes the query and returns the first row, first column value
        /// </summary>
        /// <param name="Sql"></param>
        /// <param name="Db"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string Sql, MNODatabase Db)
        {
            return ExecuteScalarAsync(Sql, Db).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Executes the query and returns the first row, first column value
        /// </summary>
        /// <param name="Sql"></param>
        /// <param name="Db"></param>
        /// <param name="ValueIfNoRecords">The value to return if no records found</param>
        /// <returns></returns>
        public static async Task<object> ExecuteScalarAsync(string Sql, MNODatabase Db, object ValueIfNoRecords)
        {
            object result = await ExecuteScalarAsync(Sql, Db);
            if (result == null)
                return ValueIfNoRecords;
            else
                return result;
        }


        /// <summary>
        /// Executes the query and returns the first row, first column value
        /// </summary>
        /// <param name="Sql"></param>
        /// <param name="Db"></param>
        /// <param name="ValueIfNoRecords">The value to return if no records found</param>
        /// <returns></returns>
        public static object ExecuteScalar(string Sql, MNODatabase Db, object ValueIfNoRecords)
        {
            object result = ExecuteScalarAsync(Sql, Db).GetAwaiter().GetResult();
            if (result == null)
                return ValueIfNoRecords;
            else 
                return result; 
        }

        /// <summary>
        /// Gets the next Oracle Sequence value from the database
        /// </summary>
        /// <param name="SequenceName"></param>
        /// <returns></returns>
        public static decimal GetNextSequence(string SequenceName)
        {
            return GetNextSequenceAsync(SequenceName).GetAwaiter().GetResult();

        }


        /// <summary>
        /// Gets the next Oracle Sequence value from the database
        /// </summary>
        /// <param name="SequenceName"></param>
        /// <returns></returns>
        public static async Task<decimal> GetNextSequenceAsync(string SequenceName)
        {
            string Sql = "SELECT " + SequenceName + ".nextval FROM DUAL";
            var result = await ExecuteScalarAsync(Sql, MNODatabase.DMART, 0);
            return Convert.ToDecimal(result);

        }

        /// <summary>
        /// Gets the next sequence number from the specified database
        /// </summary>
        /// <param name="SequenceName">The name of the sequence</param>
        /// <param name="Db">The database connection to use</param>
        /// <returns></returns>
        /// <remarks>This only works for Oracle and SQL Server</remarks>
        /// <exception cref="Exception"></exception>
        public static decimal GetNextSequence(string SequenceName, MNODatabase Db)
        {
            return GetNextSequenceAsync(SequenceName, Db).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the next sequence number from the specified database
        /// </summary>
        /// <param name="SequenceName">The name of the sequence</param>
        /// <param name="Db">The database connection to use</param>
        /// <returns></returns>
        /// <remarks>This only works for Oracle and SQL Server</remarks>
        /// <exception cref="Exception"></exception>
        public static async Task<decimal> GetNextSequenceAsync(string SequenceName, MNODatabase Db)
        {
            if (Db == MNODatabase.DMART)
            {
                return await GetNextSequenceAsync(SequenceName);
            }
            else if (DBConnections[Db.ToString()].DBType == DBType.SQLServer)
            {
                string sql = $"SELECT NEXT VALUE FOR {SequenceName}";
                //SQL Server will return a value of type int64, we will convert this to a decimal
                var result = await ExecuteScalarAsync(sql, Db);
                return Convert.ToDecimal(result);
            }
            else
            {
                throw new Exception("Invalid Database Type for GetNextSequence");
            }
        }



        /// <summary>
        /// Reads the value from the Key/Value database table tolive.key_values
        /// </summary>
        /// <param name="KeyName">The name of the Key</param>
        /// <returns></returns>
        /// <remarks>This will error if key does not exist</remarks>
        public static string ReadDBKey(string KeyName)
        {
            return ReadDBKeyAsync(KeyName, null).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Reads the value from the Key/Value database table tolive.key_values
        /// </summary>
        /// <param name="KeyName">The name of the Key</param>
        /// <param name="DefaultVal">Value to return if key is not found</param>
        /// <returns></returns>
        public static string ReadDBKey(string KeyName, string DefaultVal)
        {
            return ReadDBKeyAsync(KeyName,DefaultVal).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Reads the value from the Key/Value database table tolive.key_values
        /// </summary>
        /// <param name="KeyName">The name of the Key</param>
        /// <param name="DefaultVal">Value to return if key is not found</param>
        /// <returns></returns>
        public static async Task<string> ReadDBKeyAsync(string KeyName, string DefaultVal)
        {
            string Sql = "SELECT key_value FROM tolive.key_values where key_name = :kname";
            MOO.Exceptions.MOOSqlException SQLEx;

            DBConnection dbConn = DBConnections["DMART"];  //Key_values table is in DMART
            String ConnString = dbConn.ConnectionString;

            System.Data.Common.DbConnection conn;
            //System.Data.Common.DbProviderFactory Factory = System.Data.Common.DbProviderFactories.GetFactory("Oracle.ManagedDataAccess.Core");
            System.Data.Common.DbProviderFactory Factory = System.Data.Common.DbProviderFactories.GetFactory(dbConn.DBType.ToString());
            conn = Factory.CreateConnection();
            conn.ConnectionString = ConnString;
            var cmd = Factory.CreateCommand();
            cmd.Connection = conn;
            cmd.CommandText = Sql;

            System.Data.Common.DbParameter param = Factory.CreateParameter();
            param.DbType = DbType.AnsiString;
            param.Value = KeyName;
            param.ParameterName = ":kname";
            cmd.Parameters.Add(param);
            try
            {
                await conn.OpenAsync();
                var rdr = await ExecuteReaderAsync(cmd);
                if (rdr.HasRows)
                {
                    await rdr.ReadAsync();
                    return rdr[0].ToString();
                }
                else
                {
                    if (DefaultVal == null)
                        throw new Exception("Key name '" + KeyName + "' is not in the key_values table");
                    else
                        return DefaultVal;
                }
            }
            catch (Exception ex)
            {
                SQLEx = new Exceptions.MOOSqlException(ex, Sql);
                throw (SQLEx);
            }
            finally
            {
                await conn.CloseAsync();
            }
        }


        /// <summary>
        /// Writes a value to the Key/Values Database table tolive.key_values in dmart
        /// </summary>
        /// <param name="KeyName">The name of the key</param>
        /// <param name="KeyValue">The new value of the key</param>
        /// <param name="Description">Description of the key value (only used on first use of key)</param>
        /// <param name="ProgramName">Program that is using the DB Key  (only used on first use of key)</param>
        /// <remarks>If the key does not exist it will be created.  The description and program name are only used on insert.  Update will not modify these</remarks>
        public static void WriteDBKey(string KeyName, string KeyValue, string Description = "", string ProgramName = "")
        {

            WriteDBKeyAsync(KeyName, KeyValue, Description, ProgramName).GetAwaiter().GetResult();
        }



        /// <summary>
        /// Writes a value to the Key/Values Database table tolive.key_values in dmart
        /// </summary>
        /// <param name="KeyName">The name of the key</param>
        /// <param name="KeyValue">The new value of the key</param>
        /// <param name="Description">Description of the key value (only used on first use of key)</param>
        /// <param name="ProgramName">Program that is using the DB Key  (only used on first use of key)</param>
        /// <remarks>If the key does not exist it will be created.  The description and program name are only used on insert.  Update will not modify these</remarks>
        public static async Task WriteDBKeyAsync(string KeyName, string KeyValue, string Description = "", string ProgramName = "")
        {            
            string Sql = "UPDATE tolive.key_values SET key_value = :kv, last_update = SYSDATE WHERE key_name = :kn";
            MOO.Exceptions.MOOSqlException SQLEx;
            DBConnection dbConn = DBConnections["DMART"];  //DB Keys are in DMART
            String ConnString = dbConn.ConnectionString;

            System.Data.Common.DbConnection conn;
            System.Data.Common.DbCommand cmd;
            System.Data.Common.DbProviderFactory Factory = System.Data.Common.DbProviderFactories.GetFactory(dbConn.DBType.ToString());
            conn = Factory.CreateConnection();
            conn.ConnectionString = ConnString;
            cmd = Factory.CreateCommand();
            cmd.Connection = conn;
            cmd.CommandText = Sql;

            System.Data.Common.DbParameter ParamKV = Factory.CreateParameter();
            ParamKV.DbType = DbType.AnsiString;
            ParamKV.Value = KeyValue;
            ParamKV.ParameterName = ":kv";
            cmd.Parameters.Add(ParamKV);


            System.Data.Common.DbParameter ParamKN = Factory.CreateParameter();
            ParamKN.DbType = DbType.AnsiString;
            ParamKN.Value = KeyName;
            ParamKN.ParameterName = ":kn";
            cmd.Parameters.Add(ParamKN);

            try
            {
                await conn.OpenAsync();
                var recsUpdated = await ExecuteNonQueryAsync(cmd);
                if (recsUpdated == 0)
                {
                    //update did not change a record, this means the key doesn't exist, execute an insert instead
                    //here we will provide the description and the program name
                    cmd.CommandText = "INSERT INTO tolive.key_values (key_value,key_name,key_description,program_name,last_update) VALUES(:kv,:kn,:kd,:pn,SYSDATE)";
                    System.Data.Common.DbParameter ParamKD = Factory.CreateParameter();
                    ParamKD.DbType = DbType.AnsiString;
                    ParamKD.Value = Description;
                    ParamKD.ParameterName = ":kd";
                    cmd.Parameters.Add(ParamKD);

                    System.Data.Common.DbParameter ParamPM = Factory.CreateParameter();
                    ParamPM.DbType = DbType.AnsiString;
                    ParamPM.Value = ProgramName;
                    ParamPM.ParameterName = ":pn";
                    cmd.Parameters.Add(ParamPM);
                    await ExecuteNonQueryAsync(cmd);
                }
            }
            catch (Exception ex)
            {
                SQLEx = new Exceptions.MOOSqlException(ex, Sql);
                throw (SQLEx);

            }
            finally
            {
                await conn.CloseAsync();
            }
        }


        /// <summary>
        /// converts a datatable to an object list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        public static List<T> DataTableToObjList<T>(DataTable dt)
        {
            Type temp = typeof(T);
            List<PropertyInfo> matchingProp = [];
            //first get a list of properties that match the column names
            foreach (DataColumn column in dt.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name.Equals(column.ColumnName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        matchingProp.Add(pro);
                    }
                }
            }

            List<T> data = [];
            foreach (DataRow row in dt.Rows)
            {

                T obj = Activator.CreateInstance<T>();
                foreach(PropertyInfo pro in matchingProp)
                {
                    if (pro.PropertyType == typeof(string) && row.IsNull(pro.Name))
                    {
                        pro.SetValue(obj, "", null);
                    }
                    else
                    {
                        pro.SetValue(obj, row[pro.Name], null);
                    }
                }
                data.Add(obj);
            }
            return data;
        }

        /// <summary>
        /// Converts the values in a datarow to an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static T DataRowToObj<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name.Equals(column.ColumnName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (pro.PropertyType == typeof(string) && dr.IsNull(column.ColumnName))
                        {
                            pro.SetValue(obj, "", null);
                        }
                        else
                        {
                            pro.SetValue(obj, dr[column.ColumnName], null);
                        }
                    }
                }
            }
            return obj;
        }

        /// <summary>
        /// Gets the comments for a given Oracle Tabls
        /// </summary>
        /// <param name="SchemaOwner"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public static List<OraTableColumnComment> GetOraTableComments(string SchemaOwner, string TableName)
        {
            return GetOraTableCommentsAsync(SchemaOwner,TableName).GetAwaiter().GetResult();
        }



        /// <summary>
        /// Gets the comments for a given Oracle Tabls
        /// </summary>
        /// <param name="SchemaOwner"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public static async Task<List<OraTableColumnComment>> GetOraTableCommentsAsync(string SchemaOwner, string TableName)
        {
            List<OraTableColumnComment> retVal = [];
            StringBuilder sql = new();
            sql.AppendLine("SELECT column_name, comments");
            sql.AppendLine("FROM all_col_comments");
            sql.AppendLine("WHERE UPPER(owner) = UPPER(:owner)");
            sql.AppendLine("AND UPPER(table_name) = UPPER(:tableName)");

            DBConnection dbConn = DBConnections["DMART"];  //DB Keys are in DMART
            String ConnString = dbConn.ConnectionString;

            System.Data.Common.DbProviderFactory Factory = System.Data.Common.DbProviderFactories.GetFactory(dbConn.DBType.ToString());
            using System.Data.Common.DbConnection conn = Factory.CreateConnection();
            conn.ConnectionString = ConnString;
            System.Data.Common.DbCommand cmd = Factory.CreateCommand();
            cmd.Connection = conn;
            cmd.CommandText = sql.ToString();

            System.Data.Common.DbParameter paramOwner = Factory.CreateParameter();
            paramOwner.DbType = DbType.AnsiString;
            paramOwner.Value = SchemaOwner;
            paramOwner.ParameterName = ":owner";
            cmd.Parameters.Add(paramOwner);


            System.Data.Common.DbParameter paramTable = Factory.CreateParameter();
            paramTable.DbType = DbType.AnsiString;
            paramTable.Value = TableName;
            paramTable.ParameterName = ":tableName";
            cmd.Parameters.Add(paramTable);

            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
            {
                OraTableColumnComment newRec = new()
                {
                    ColumnName = rdr[0].ToString(),
                    Comments = rdr[1].ToString()
                };
                retVal.Add(newRec);
            }

            await conn.CloseAsync();
            return retVal;
        }
        #endregion

    }
}
