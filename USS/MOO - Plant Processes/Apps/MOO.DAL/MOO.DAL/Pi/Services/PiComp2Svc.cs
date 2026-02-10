using MOO.DAL.Pi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using Oracle.ManagedDataAccess.Client;
using MOO.DAL.Pi.Enums;

namespace MOO.DAL.Pi.Services
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    public static class PiComp2Svc
    {
        /*NOTES About writing to PI.  
         * The PutPIDataUnbuffered and DeletePIDataUnbuffered MUST have a unbuffered connection set up on the server.  This will be set up on the MNO-Web server.  The reason for this
         * is because a buffered connection will not allow delete/modify of the most recent value (known as the snapshot).  The buffer locks that value and won't allow edit.  To allow 
         * edit/delete of that value this data must be written individually to each server with an unbuffered connection.  Update/Delete will not be made for buffered connections as this will not 
         * affect the snapshot value.  The insert buffered should only be used for programs that will not need to update/delete data
         */

        public const string PRIMARY_SERVER_NAME = "MTC-PI";
        public const string SECONDARY_SERVER_NAME = "KTC-PI";
        static PiComp2Svc()
        {
            Util.RegisterOLEDB();
            Util.RegisterOracle();
        }

        /// <summary>
        /// Gets a list of PIComp2 Records from the PIComp2 Compression table
        /// </summary>
        /// <param name="StartDate">Start Time</param>
        /// <param name="EndDate">End Time</param>
        /// <param name="TagName">PI Tag Name</param>
        /// <param name="ExcludeNullValues">If true, Null Values will not be included in result set.</param>
        /// <param name="ExcludeBadStatus">If true, a pi comp status other than zero is not included</param>
        /// <returns></returns>
        /// <remarks> ExcludeNullValues and ExcludeBadStatus parameters were added because Radzen Charts crash with null values, these can be excluded for those or any other reason
        /// </remarks>
        public static List<PiComp2> Get(DateTime StartDate, DateTime EndDate, string TagName, bool ExcludeNullValues = false,bool ExcludeBadStatus = false)
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT tag, time, value, status, questionable, ");
            sql.AppendLine("substituted, annotated, annotations");
            sql.AppendLine("FROM [piarchive]..[picomp2]");
            sql.AppendLine("WHERE tag = ?");
            sql.AppendLine("AND time BETWEEN ? AND ?");
            if (ExcludeBadStatus)
                sql.AppendLine("AND status = 0");
            if (ExcludeNullValues)
                sql.AppendLine("AND value is not null");

            OleDbDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.MTC_Pi));
            da.SelectCommand.Parameters.AddWithValue("tag", TagName);
            da.SelectCommand.Parameters.AddWithValue("StartDate", StartDate);
            da.SelectCommand.Parameters.AddWithValue("EndDate", EndDate);

            DataSet ds = MOO.Data.ExecuteQuery(da);
            List<PiComp2> retval = new();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                retval.Add(DataRowToObj(dr));
            }
            return retval;


        }




        /// <summary>
        /// converts the datarow to an object
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private static PiComp2 DataRowToObj(DataRow row)
        {


            PiComp2 retObj = new();
            retObj.Tag = row.Field<string>("tag");
            retObj.Time = row.Field<DateTime>("time");
            //value can be a single, double, short or int so we will need to cast to string first and then parse to double to hold whatever type it is
            if (row.IsNull("value"))
                retObj.Value = null;
            else
                retObj.Value = double.Parse((row["value"].ToString()));
            retObj.Status = row.Field<int>("status");
            retObj.Questionable = row.Field<bool?>("Questionable");
            retObj.Substituted = row.Field<bool?>("Substituted");
            retObj.Annotated = row.Field<bool?>("Annotated");
            retObj.Annotations = row.Field<string>("Annotations");
            return retObj;
        }




        /// <summary>
        /// Inserts the PI Data on an buffered PI SDK Connection
        /// </summary>
        /// <param name="TagName"></param>
        /// <param name="TagDate"></param>
        /// <param name="Value"></param>
        /// <remarks>Server must have PI Buffering enabled. </remarks>
        public static int InsertPIDataBuffered(string TagName, DateTime TagDate, double Value)
        {

            StringBuilder sql = new();

            OleDbCommand cmd;
            int recsUpdated;

            sql.AppendLine("INSERT INTO [piarchive].[picomp2] (tag, value, time)");
            sql.AppendLine($"VALUES(?,?, '{TagDate:MM/dd/yyyy HH:mm:ss}')");
            //make 2 connections to connect to each pi server

            using OleDbConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.MTC_Pi));
            conn.Open();
            cmd = new OleDbCommand(sql.ToString(), conn);
            cmd.Parameters.Add(new OleDbParameter("tag", TagName));
            cmd.Parameters.Add(new OleDbParameter("value", Value));
            recsUpdated = cmd.ExecuteNonQuery();
            conn.Close();
            return recsUpdated;
        }


        /// <summary>
        /// Inserts a status value for a PI Tag buffered PI SDK Connection
        /// </summary>
        /// <param name="TagName"></param>
        /// <param name="TagDate"></param>
        /// <param name="Status"></param>
        /// <remarks>Server must have PI Buffering enabled. </remarks>
        public static int InsertPIStatusBuffered(string TagName, DateTime TagDate, PISystemStates Status)
        {

            StringBuilder sql = new();

            OleDbCommand cmd;
            int recsUpdated;

            sql.AppendLine("INSERT INTO [piarchive].[picomp2] (tag, status, time)");
            sql.AppendLine($"VALUES(?,?, '{TagDate:MM/dd/yyyy HH:mm:ss}')");
            //make 2 connections to connect to each pi server

            using OleDbConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.MTC_Pi));
            conn.Open();
            cmd = new OleDbCommand(sql.ToString(), conn);
            cmd.Parameters.Add(new OleDbParameter("tag", TagName));
            cmd.Parameters.Add(new OleDbParameter("status", (int)Status));
            recsUpdated = cmd.ExecuteNonQuery();
            conn.Close();
            return recsUpdated;
        }



        #region "Functions for dealing with unbuffered insert/update/delete and the PI_Buffer Oracle Table" 



        /// <summary>
        /// Deletes the data on an Unbuffered sdk connection 
        /// </summary>
        /// <param name="TagName"></param>
        /// <param name="TagDate"></param>
        /// <remarks>Server must not have PI Buffering enabled</remarks>
        public static int DeletePIDataUnbuffered(string TagName, DateTime TagDate)
        {
            return DeleteUnbufferedPIDataObj(TagName, TagDate);
        }

        /// <summary>
        /// Updates/Inserts the PI Data on an Unbuffered PI SDK Connection
        /// </summary>
        /// <param name="TagName"></param>
        /// <param name="TagDate"></param>
        /// <param name="Value"></param>
        /// <remarks>Server must not have PI Buffering enabled</remarks>
        public static int PutPIDataUnBuffered(string TagName, DateTime TagDate, double Value)
        {
            int recsUpdated;
            recsUpdated = UpdateUnbufferedPIDataObj(TagName, TagDate, Value);
            if (recsUpdated == 0)
                recsUpdated = InsertUnbufferedPIDataObj(TagName, TagDate, Value);
            return recsUpdated;
        }






        /// <summary>
        /// runs the PI command to the primary or the secondary server
        /// </summary>
        /// <param name="IsPrimaryServer">If true this will run the command on the primary PI server, else will run on secondary server</param>
        /// <returns></returns>
        /// <exception cref="FailedPIServerConnectionException"></exception>
        public static int RunUnbufferedPISQLCommand(bool IsPrimaryServer, OleDbCommand cmd)
        {
            int recsAffected = 0;
            OleDbConnection conn;
            string expectedServer;
            if (IsPrimaryServer)
                expectedServer = PRIMARY_SERVER_NAME;
            else
                expectedServer = SECONDARY_SERVER_NAME;
            if (IsPrimaryServer)
                conn = new(Data.GetConnectionString(Data.MNODatabase.MTC_Pi) + ";Connection Type=RequirePrimary");
            else
            {
                conn = new(Data.GetConnectionString(Data.MNODatabase.MTC_Pi));

            }
            //'run the command on connection 2
            conn.Open();
            try
            {
                OleDbCommand checkMemberNameQuery = new("SELECT value FROM pisystem.piconnection WHERE name = 'Server Path'", conn);
                string serverName = checkMemberNameQuery.ExecuteScalar().ToString();
                if (!serverName.ToUpper().Contains(expectedServer))
                {
                    //not connected to the right server
                    // query to switch connection to another collective member
                    OleDbCommand switchMemberQuery = new("SWITCH MEMBER", conn);
                    switchMemberQuery.ExecuteNonQuery();
                    serverName = checkMemberNameQuery.ExecuteScalar().ToString();
                    if (!serverName.ToUpper().Contains(expectedServer))
                    {
                        conn.Close();
                        //server switch failed
                        throw new FailedPIServerConnectionException("Failed to connect to requested server", expectedServer);
                    }
                }
                //if we get here we should now have a connection to the correct server
                cmd.Connection = conn;
                recsAffected = cmd.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }
            return recsAffected;
        }


        private static int DeleteUnbufferedPIDataObj(string TagName, DateTime TagDate)
        {
            StringBuilder sql = new();
            OleDbCommand cmd;
            sql.AppendLine("DELETE FROM [piarchive].[picomp2]");
            sql.AppendLine($"WHERE Tag = '{TagName}' AND time = ?");
            //make 2 connections to connect to each pi server

            cmd = new OleDbCommand(sql.ToString());
            cmd.Parameters.AddWithValue("time", TagDate);


            string Server1FailMsg = "";
            string Server2FailMsg = "";

            //delete statement will return a -1 for recsAffected if it runs
            int recsaffected1 = 0, recsaffected2 = 0;
            try
            {
                //attempt insert on primary
                recsaffected1 = RunUnbufferedPISQLCommand(true, cmd);
            }
            catch (Exception ex)
            {
                Server1FailMsg = ex.Message;
                InsertPIChangesBuffer(TagName, "", TagDate, "D", "MTC-PI");
            }

            try
            {
                //attempt insert on secondary
                recsaffected2 = RunUnbufferedPISQLCommand(false, cmd);
            }
            catch (Exception ex)
            {
                Server2FailMsg = ex.Message;
                InsertPIChangesBuffer(TagName, "", TagDate, "D", "KTC-PI");
            }
            if (!string.IsNullOrEmpty(Server1FailMsg) && !string.IsNullOrEmpty(Server2FailMsg))
            {
                //both failed, this is highly unlikely so record the error
                MOO.Exceptions.ErrorLog.LogError("MOO.DAL", "Error updating PI value on both servers, Both Servers failed",
                                 $"Exception on MTC-PI = {Server1FailMsg}\n" +
                                           $"Exception on KTC-PI = {Server2FailMsg}\n" +
                                           $"Tag:{TagName}\n" +
                                           $"Value:DELETE\n" +
                                           $"PIDate:{TagDate}", "", Exceptions.ErrorLog.ErrorLogType.Crash);
            }
            if (recsaffected1 != recsaffected2)
            {
                //we have an issue where one server updated and the other didn't
                MOO.Exceptions.ErrorLog.LogError("MOO.DAL", "Error updating PI value on both servers, Data updated on one but not the other",
                                 $"Recs Affected on MTC-PI = {recsaffected1}\n" +
                                           $"Recs Affected on KTC-PI = {recsaffected2}\n" +
                                           $"Tag:{TagName}\n" +
                                           $"Value:DELETE\n" +
                                           $"PIDate:{TagDate}", "", Exceptions.ErrorLog.ErrorLogType.Crash);
            }
            return Math.Min(recsaffected1, recsaffected2);
        }

        private static int UpdateUnbufferedPIDataObj(string TagName, DateTime TagDate, object Value)
        {
            StringBuilder sql = new();
            OleDbCommand cmd;
            sql.AppendLine("UPDATE [piarchive].[picomp2] SET Value = ?");
            sql.AppendLine($"WHERE Tag = ? AND time = '{TagDate:MM/dd/yyyy HH:mm:ss}'");
            //make 2 connections to connect to each pi server

            cmd = new OleDbCommand(sql.ToString());
            cmd.Parameters.Add(new OleDbParameter("value", Value));
            cmd.Parameters.Add(new OleDbParameter("tag", TagName));

            string Server1FailMsg = "";
            string Server2FailMsg = "";
            int recsaffected1 = 0, recsaffected2 = 0;
            try
            {
                //attempt update on primary
                recsaffected1 = RunUnbufferedPISQLCommand(true, cmd);
            }
            catch (Exception ex)
            {
                Server1FailMsg = ex.Message;
                InsertPIChangesBuffer(TagName, Value, TagDate, "U", "MTC-PI");
            }

            try
            {
                //attempt update on secondary
                recsaffected2 = RunUnbufferedPISQLCommand(false, cmd);
            }
            catch (Exception ex)
            {
                Server2FailMsg = ex.Message;
                InsertPIChangesBuffer(TagName, Value, TagDate, "U", "KTC-PI");
            }
            if (!string.IsNullOrEmpty(Server1FailMsg) && !string.IsNullOrEmpty(Server2FailMsg))
            {
                //both failed, this is highly unlikely so record the error
                MOO.Exceptions.ErrorLog.LogError("MOO.DAL", "Error updating PI value on both servers, Both Servers failed",
                                 $"Exception on MTC-PI = {Server1FailMsg}\n" +
                                           $"Exception on KTC-PI = {Server2FailMsg}\n" +
                                           $"Tag:{TagName}\n" +
                                           $"Value:{Value}\n" +
                                           $"PIDate:{TagDate}", "", Exceptions.ErrorLog.ErrorLogType.Crash);
            }
            if (recsaffected1 != recsaffected2)
            {
                //we have an issue where one server updated and the other didn't
                MOO.Exceptions.ErrorLog.LogError("MOO.DAL", "Error updating PI value on both servers, Data updated on one but not the other",
                                 $"Recs Affected on MTC-PI = {recsaffected1}\n" +
                                           $"Recs Affected on KTC-PI = {recsaffected2}\n" +
                                           $"Tag:{TagName}\n" +
                                           $"Value:{Value}\n" +
                                           $"PIDate:{TagDate}", "", Exceptions.ErrorLog.ErrorLogType.Crash);
            }
            return Math.Max(recsaffected1, recsaffected2);
        }

        private static int InsertUnbufferedPIDataObj(string TagName, DateTime TagDate, object Value)
        {
            StringBuilder sql = new();
            OleDbCommand cmd;
            sql.AppendLine("INSERT INTO [piarchive].[picomp2] (tag, value, time)");
            sql.AppendLine($"VALUES(?,?, '{TagDate:MM/dd/yyyy HH:mm:ss}')");
            //make 2 connections to connect to each pi server

            cmd = new OleDbCommand(sql.ToString());
            cmd.Parameters.Add(new OleDbParameter("tag", TagName));
            cmd.Parameters.Add(new OleDbParameter("value", Value));

            string Server1FailMsg = "";
            string Server2FailMsg = "";
            int recsaffected1 = 0, recsaffected2 = 0;
            try
            {
                //attempt insert on primary
                recsaffected1 = RunUnbufferedPISQLCommand(true, cmd);
            }
            catch (Exception ex)
            {
                Server1FailMsg = ex.Message;
                InsertPIChangesBuffer(TagName, Value, TagDate, "U", "MTC-PI");
            }

            try
            {
                //attempt insert on secondary
                recsaffected2 = RunUnbufferedPISQLCommand(false, cmd);
            }
            catch (Exception ex)
            {
                Server2FailMsg = ex.Message;
                InsertPIChangesBuffer(TagName, Value, TagDate, "U", "KTC-PI");
            }
            if (!string.IsNullOrEmpty(Server1FailMsg) && !string.IsNullOrEmpty(Server2FailMsg))
            {
                //both failed, this is highly unlikely so record the error
                MOO.Exceptions.ErrorLog.LogError("MOO.DAL", "Error updating PI value on both servers, Both Servers failed",
                                 $"Exception on MTC-PI = {Server1FailMsg}\n" +
                                           $"Exception on KTC-PI = {Server2FailMsg}\n" +
                                           $"Tag:{TagName}\n" +
                                           $"Value:{Value}\n" +
                                           $"PIDate:{TagDate}", "", Exceptions.ErrorLog.ErrorLogType.Crash);
            }
            if (recsaffected1 != recsaffected2)
            {
                //we have an issue where one server updated and the other didn't
                MOO.Exceptions.ErrorLog.LogError("MOO.DAL", "Error updating PI value on both servers, Data updated on one but not the other",
                                 $"Recs Affected on MTC-PI = {recsaffected1}\n" +
                                           $"Recs Affected on KTC-PI = {recsaffected2}\n" +
                                           $"Tag:{TagName}\n" +
                                           $"Value:{Value}\n" +
                                           $"PIDate:{TagDate}", "", Exceptions.ErrorLog.ErrorLogType.Crash);
            }
            return Math.Max(recsaffected1, recsaffected2);
        }

        private static void InsertPIChangesBuffer(string TagName, object Value, DateTime TagDate,
                                               string ChangeType, string Server)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.PI_BUFFER(THEDATE, TAG, PIDATE, VALUE, S1_WRITE, S2_WRITE, TYPE)");
            sql.AppendLine("VALUES(:THEDATE, :TAG, :PIDATE, :VALUE, :S1_WRITE, :S2_WRITE, :TYPE)");

            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("THEDATE", DateTime.Now);
            cmd.Parameters.Add("TAG", TagName);
            cmd.Parameters.Add("PIDATE", TagDate);
            cmd.Parameters.Add("VALUE", Value);
            cmd.Parameters.Add("S1_WRITE", Server.ToUpper().Contains(PRIMARY_SERVER_NAME) ? 1 : 0);
            cmd.Parameters.Add("S2_WRITE", Server.ToUpper().Contains(SECONDARY_SERVER_NAME) ? 1 : 0);
            cmd.Parameters.Add("TYPE", ChangeType);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        #endregion


    }
}
