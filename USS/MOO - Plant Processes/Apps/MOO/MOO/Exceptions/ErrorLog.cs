using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Runtime.ExceptionServices;

namespace MOO.Exceptions
{
    /// <summary>
    /// Class used for logging data to the tolive.error table in DMART
    /// </summary>
    public static class ErrorLog
    {

        private const string MOO_UNIT_TEST_NAME = "MOO_UnitTest";
        /// <summary>
        /// The type of error
        /// </summary>
        public enum ErrorLogType
        {
            /// <summary>
            /// Program Crash
            /// </summary>
            Crash = 0,
            /// <summary>
            /// Warning, may need attention but did not cause crash
            /// </summary>
            Warning = 1,
            /// <summary>
            /// Informational, used for troubleshooting applications
            /// </summary>
            Info = 2,

            /// <summary>
            /// Used for logging when PI Points are added using the IT_Admin tool
            /// </summary>
            /// <remarks>I realize this is not an "ERROR" but this is the easiest spot to log this</remarks>
            PI_Point_Addition = 100


        }

        /// <summary>
        /// Logs an exception to the Oracle error table
        /// </summary>
        /// <param name="ProgramName">The name of the program</param>
        /// <param name="ex">The exception that occured</param>
        public static void LogError(string ProgramName, Exception ex)
        {
            string AdditionalInfo = "";
            if(ex is MOOSqlException SqlEx)
            {
                AdditionalInfo = SqlEx.SQLString;
            }
            string description;
            if(ex.InnerException != null)
                description = ex.Message + "\n\n" + "InnerException: " + ex.InnerException.Message;
            else
                description = ex.Message;

            LogError(ProgramName, description, ex.StackTrace, AdditionalInfo, ErrorLogType.Crash);
        }


        /// <summary>
        /// Records the error to the oracle table
        /// </summary>
        /// <param name="ProgramName">The name of the executing program</param>
        /// <param name="Message">The error message to record</param>
        /// <param name="StackTrace">The stack trace to record</param>
        /// <param name="AdditionalInfo">Any additional Information about the error</param>
        /// <param name="ErrorType">The type of error (Crash/Warning/Info)</param>
        public static void LogError(string ProgramName, string Message, string StackTrace, string AdditionalInfo, 
                            ErrorLogType ErrorType)
        {
            //Override Error type if this is coming from the MOO Unit test program
            if (String.Equals(MOO_UNIT_TEST_NAME, ProgramName))
                ErrorType = ErrorLogType.Info;

            MOO.Data.DBConnection dbConn = MOO.Data.DBConnections["DMART"];
            String ConnString = dbConn.ConnectionString;

            System.Data.Common.DbConnection conn;
            System.Data.Common.DbCommand cmd;
            System.Data.Common.DbProviderFactory Factory = System.Data.Common.DbProviderFactories.GetFactory(dbConn.DBType.ToString());
            conn = Factory.CreateConnection();
            conn.ConnectionString = ConnString;
            cmd = Factory.CreateCommand();
            cmd.Connection = conn;
            cmd.CommandText = "General.log_error";
            cmd.CommandType = CommandType.StoredProcedure;

            //Parameter error_date
            System.Data.Common.DbParameter ErrorDateParam = Factory.CreateParameter();
            ErrorDateParam.DbType = DbType.Date;
            ErrorDateParam.Value = DateTime.Now;
            ErrorDateParam.ParameterName = "in_error_date";
            cmd.Parameters.Add(ErrorDateParam);

            //Parameter error_num
            System.Data.Common.DbParameter ErrorNumParam = Factory.CreateParameter();
            ErrorNumParam.DbType = DbType.Int16;
            ErrorNumParam.Value = 0;
            ErrorNumParam.ParameterName = "in_error_num";
            cmd.Parameters.Add(ErrorNumParam);

            //Parameter in_error_desc
            System.Data.Common.DbParameter ErrorDescParam = Factory.CreateParameter();
            ErrorDescParam.DbType = DbType.AnsiString;
            ErrorDescParam.Value = Message;
            ErrorDescParam.ParameterName = "in_error_desc";
            cmd.Parameters.Add(ErrorDescParam);

            //Parameter program name
            System.Data.Common.DbParameter ErrorProgParam = Factory.CreateParameter();
            ErrorProgParam.DbType = DbType.AnsiString;
            ErrorProgParam.Value = Microsoft.VisualBasic.Strings.Left(ProgramName, 200);
            ErrorProgParam.ParameterName = "in_pname";
            cmd.Parameters.Add(ErrorProgParam);

            //Parameter Addition Info (sql)
            System.Data.Common.DbParameter ErrorInfoParam = Factory.CreateParameter();
            ErrorInfoParam.DbType = DbType.AnsiString;
            ErrorInfoParam.Value = Microsoft.VisualBasic.Strings.Left(AdditionalInfo, 500);
            ErrorInfoParam.ParameterName = "in_error_sql";
            cmd.Parameters.Add(ErrorInfoParam);

            //Parameter Stack
            System.Data.Common.DbParameter ErrorStackParam = Factory.CreateParameter();
            ErrorStackParam.DbType = DbType.AnsiString;
            ErrorStackParam.Value = StackTrace;
            ErrorStackParam.ParameterName = "in_error_stack";
            cmd.Parameters.Add(ErrorStackParam);

            //Parameter Error Type
            System.Data.Common.DbParameter ErrorTypeParam = Factory.CreateParameter();
            ErrorTypeParam.DbType = DbType.Int16;
            ErrorTypeParam.Value = (Int16)ErrorType;
            ErrorTypeParam.ParameterName = "in_error_type";
            cmd.Parameters.Add(ErrorTypeParam);


            try
            {
                conn.Open();
                Data.ExecuteNonQuery(cmd);
            }
            catch (Exception ex)
            {
                SendErrorLogEmail(ProgramName, ex, Message, StackTrace, AdditionalInfo, ErrorType);
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();

            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// email that will be sent if logging the error fails
        /// </summary>
        private static void SendErrorLogEmail(string ProgramName, Exception ex,string Message, string StackTrace, string AdditionalInfo, 
                                                    ErrorLogType ErrorType)
        {
            if(Settings.ServerType == Settings.ServerClass.Production)
            {
                StringBuilder ErrorEmail = new();
                ErrorEmail.AppendLine("An error occurred while trying to log an error to the tolive.error table.");
                ErrorEmail.AppendLine("Message: " + ex.Message);
                ErrorEmail.AppendLine("Stack: " + ex.StackTrace);
                ErrorEmail.AppendLine("");
                ErrorEmail.AppendLine("Error that could not be logged:");
                ErrorEmail.AppendLine("Program Name: " + ProgramName);
                ErrorEmail.AppendLine("Description: " + Message);
                ErrorEmail.AppendLine("Additional Info: " + AdditionalInfo);
                ErrorEmail.AppendLine("Stack: " + StackTrace);
                ErrorEmail.AppendLine("Error Type: "  + ErrorType.ToString());
                ErrorEmail.AppendLine("");
                ErrorEmail.AppendLine("");
                ErrorEmail.AppendLine("");

                Mail.SendMail(ProgramName, Settings.MailFromAddress, Settings.PC_Level2_Email, "", "",
                                "Cannot Log Error to Oracle Error table", ErrorEmail.ToString(), true, System.Net.Mail.MailPriority.High, "");
            }
        }


    }
}
