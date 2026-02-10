using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace MOO
{
    /// <summary>
    /// Class used for sending Email
    /// </summary>
    public static class Mail
    {

        /// <summary>
        /// Sends mail 
        /// </summary>
        /// <param name="ProgramName">The name of the program sending the email</param>
        /// <param name="Recipients">Comma seperated list of recipients</param>
        /// <param name="Subject">Subject of the email</param>
        /// <param name="Body">Body of the email</param>
        public static void SendMail(string ProgramName, string Recipients, string Subject, string Body)
        {
            SendMail(ProgramName, Settings.MailFromAddress, Recipients, "", "", Subject, Body, true, 
                        System.Net.Mail.MailPriority.Normal,"");
        }

        /// <summary>
        /// Sends mail 
        /// </summary>
        /// <param name="ProgramName">The name of the program sending the email</param>
        /// <param name="Recipients">Comma seperated list of recipients</param>
        /// <param name="Subject">Subject of the email</param>
        /// <param name="Body">Body of the email</param>
        /// <param name="Attachments">Comma seperated list of file attachments</param>
        public static void SendMail(string ProgramName, string Recipients, string Subject, string Body, string Attachments)
        {
            SendMail(ProgramName, Settings.MailFromAddress, Recipients, "", "", Subject, Body, true,
                        System.Net.Mail.MailPriority.Normal, Attachments);
        }

        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="ProgramName">The name of the program sending the email</param>
        /// <param name="From">The From Address</param>
        /// <param name="Recipients">Comma seperated list of recipients</param>
        /// <param name="CC">Comma seperated list of Carbon copyy recipients</param>
        /// <param name="BCC">Comma seperated list of BCC recipients</param>
        /// <param name="Subject">Subject of the email</param>
        /// <param name="Body">Body of the email</param>
        /// <param name="IsHTML">If the email is html or plain text</param>
        /// <param name="Priority">Email Priority</param>
        /// <param name="Attachments">Comma seperated list of file attachments</param>
        public static void SendMail(string ProgramName, string From, string Recipients, string CC,
                                string BCC, string Subject, string Body, bool IsHTML,
                                System.Net.Mail.MailPriority Priority, string Attachments)
        {

            //Validate parameters
            if (string.IsNullOrEmpty(From.Trim()))
                throw new ArgumentException("SendMail From paramater cannot be empty");

            if(string.IsNullOrEmpty(Recipients.Trim()))                        
                throw new ArgumentException("SendMail Recipients paramater cannot be empty");



            System.Net.Mail.MailMessage Msg = new();
            Msg.From = new System.Net.Mail.MailAddress(From);

            //add the to addresses
            foreach(string sendTo in Recipients.Split(","))
            {
                Msg.To.Add(new System.Net.Mail.MailAddress(sendTo));
            }

            //Add any cc addresses
            if(!string.Equals(CC.Trim(), string.Empty))
            {
                foreach (string sendTo in CC.Split(","))
                {
                    Msg.To.Add(new System.Net.Mail.MailAddress(sendTo));
                }
            }

            //Add any BCC addresses
            if (!string.Equals(BCC.Trim(), string.Empty))
            {
                foreach (string sendTo in BCC.Split(","))
                {
                    Msg.To.Add(new System.Net.Mail.MailAddress(sendTo));
                }
            }

            //Add any file attachments
            if (!string.Equals(Attachments.Trim(), string.Empty))
            {
                foreach (string File in Attachments.Split(","))
                {
                    Msg.Attachments.Add(new System.Net.Mail.Attachment(File));
                }
            }

            Msg.Subject = Subject;
            Msg.Body = Body;
            Msg.IsBodyHtml = IsHTML;
            Msg.Priority = Priority;

            System.Net.Mail.SmtpClient Smtp = new()
            {
                Host = Settings.SMTP_Server
            };
            Smtp.Send(Msg);
            RecordToMailLog(ProgramName, From, Recipients, CC, BCC, Subject,  IsHTML, Priority, Attachments);
        }

        /// <summary>
        /// Records the sent email to the tolive.mail_log table for troubleshooting
        /// </summary>
        /// <param name="ProgramName">The name of the program sending the email</param>
        /// <param name="From">The From Address</param>
        /// <param name="Recipients">Comma seperated list of recipients</param>
        /// <param name="CC">Comma seperated list of Carbon copyy recipients</param>
        /// <param name="BCC">Comma seperated list of BCC recipients</param>
        /// <param name="Subject">Subject of the email</param>
        /// <param name="IsHTML">If the email is html or plain text</param>
        /// <param name="Priority">Email Priority</param>
        /// <param name="Attachments">Comma seperated list of file attachments</param>
        internal static void RecordToMailLog(string ProgramName, string From, string Recipients, string CC,
                                string BCC, string Subject, bool IsHTML,
                                System.Net.Mail.MailPriority Priority, string Attachments)
        {
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.mail_log (mail_date, program, subject, msg_from, recipients2, cc, bcc, ishtml, priority, attachments)");
            sql.AppendLine("VALUES  (SYSDATE, :program, :subject, :msg_from, :recipients,:cc,:bcc,:ishtml, :priority,:attachments)");

            MOO.Data.DBConnection dbConn = MOO.Data.DBConnections["DMART"];
            String ConnString = dbConn.ConnectionString;

            System.Data.Common.DbConnection conn;
            System.Data.Common.DbCommand cmd;
            System.Data.Common.DbProviderFactory Factory = System.Data.Common.DbProviderFactories.GetFactory(dbConn.DBType.ToString());
            conn = Factory.CreateConnection();
            conn.ConnectionString = ConnString;
            cmd = Factory.CreateCommand();
            cmd.Connection = conn;
            cmd.CommandText = sql.ToString();

            //Parameter program
            System.Data.Common.DbParameter ProgParam = Factory.CreateParameter();
            ProgParam.DbType = System.Data.DbType.AnsiString;
            ProgParam.Value = Microsoft.VisualBasic.Strings.Left(ProgramName, 50);
            ProgParam.ParameterName = "program";
            cmd.Parameters.Add(ProgParam);

            //Parameter subject
            System.Data.Common.DbParameter SubjParam = Factory.CreateParameter();
            SubjParam.DbType = DbType.AnsiString;
            SubjParam.Value = Microsoft.VisualBasic.Strings.Left(Subject, 100);
            SubjParam.ParameterName = "subject";
            cmd.Parameters.Add(SubjParam);

            //Parameter msg_from
            System.Data.Common.DbParameter FromParam = Factory.CreateParameter();
            FromParam.DbType = DbType.AnsiString;
            FromParam.Value = Microsoft.VisualBasic.Strings.Left(From, 50);
            FromParam.ParameterName = "msg_from";
            cmd.Parameters.Add(FromParam);

            //Parameter recipients
            System.Data.Common.DbParameter RecipParam = Factory.CreateParameter();
            RecipParam.DbType = DbType.AnsiString;
            var recipVal = Recipients.Replace(",", "\n");
            RecipParam.Value = recipVal;
            RecipParam.ParameterName = "recipients";
            cmd.Parameters.Add(RecipParam);

            //Parameter cc
            System.Data.Common.DbParameter CCParam = Factory.CreateParameter();
            CCParam.DbType = DbType.AnsiString;
            CCParam.Value = Microsoft.VisualBasic.Strings.Left(CC, 500);
            CCParam.ParameterName = "cc";
            cmd.Parameters.Add(CCParam);

            //Parameter bcc
            System.Data.Common.DbParameter BccParam = Factory.CreateParameter();
            BccParam.DbType = DbType.AnsiString;
            BccParam.Value = Microsoft.VisualBasic.Strings.Left(BCC, 500);
            BccParam.ParameterName = "bcc";
            cmd.Parameters.Add(BccParam);

            //Parameter ishtml
            System.Data.Common.DbParameter HtmlParam = Factory.CreateParameter();
            HtmlParam.DbType = DbType.Int16;
            HtmlParam.Value = IsHTML?1:0;
            HtmlParam.ParameterName = "ishtml";
            cmd.Parameters.Add(HtmlParam);

            //Parameter priority
            System.Data.Common.DbParameter PriorityParam = Factory.CreateParameter();
            PriorityParam.DbType = DbType.AnsiString;
            PriorityParam.Value = Priority.ToString();
            PriorityParam.ParameterName = "priority";
            cmd.Parameters.Add(PriorityParam);

            //Parameter attachments
            System.Data.Common.DbParameter AttachParam = Factory.CreateParameter();
            AttachParam.DbType = DbType.AnsiString;
            AttachParam.Value = Microsoft.VisualBasic.Strings.Left(Attachments, 500);
            AttachParam.ParameterName = "attachments";
            cmd.Parameters.Add(AttachParam);


            try
            {
                conn.Open();
                Data.ExecuteNonQuery(cmd);
            }
            catch (Exception ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();

            }
            finally
            {
                conn.Close();
            }

        }
    }
}
