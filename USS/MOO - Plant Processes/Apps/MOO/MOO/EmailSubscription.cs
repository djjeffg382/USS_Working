using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO
{
    /// <summary>
    /// Class for handling automated email to groups set up in the oracle tolive.email_subscription and tolive.email_subscription_user tables
    /// </summary>
    public class EmailSubscription
    {

        private const string EMAIL_SUBSCRIPTION_LINK = "http://www.mno.uss.com/bzr/MOO_General/notifications";

        #region "Properties"
        private int _EmailSubscriptionID;
        /// <summary>
        /// The email subscriber ID (Primary Key) from the tolive.email_subscription table
        /// </summary>
        public int EmailSubscriptionID
        {
            get {
                return _EmailSubscriptionID;
            }
            set
            {
                _EmailSubscriptionID = value;
                FillProperties();
            }
        }

        private string _SubscriptionName;
        /// <summary>
        /// The email subscriber ID (Primary Key) from the tolive.email_subscription table
        /// </summary>
        public string SubscriptionName
        {
            get { return _SubscriptionName; }
        }


        private string _Description;
        /// <summary>
        /// Description of the Email Subscription
        /// </summary>
        public string Description
        {
            get { return _Description; }
        }

        private string _Application;
        /// <summary>
        /// The application that uses the email subscription
        /// </summary>
        public string Application
        {
            get { return _Application; }
        }

        private bool _Hide_User_Emails;
        /// <summary>
        /// Whether the email will be sent as one to all or sent individually to each user
        /// </summary>
        public bool Hide_User_Emails
        {
            get { return _Hide_User_Emails; }
        }

        /// <summary>
        /// The body of the message
        /// </summary>
        public string MessageBody;

        /// <summary>
        /// The email subject
        /// </summary>
        public string Subject;

        /// <summary>
        /// Attachments for the email.  Should be file locations to the files comma seperated
        /// </summary>
        public List<string> Attachments = new();

        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="EmailSubscriptionID">The email subscriber ID (Primary Key) from the tolive.email_subscription table</param>
        public EmailSubscription(int EmailSubscriptionID)
        {
            this.EmailSubscriptionID = EmailSubscriptionID;           
        }

        private void FillProperties()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT subscription_name, description, application, hide_user_emails");
            sql.AppendLine("FROM tolive.email_subscription");
            sql.AppendLine("WHERE email_subscription_id = " + EmailSubscriptionID.ToString());

            DataSet ds = Data.ExecuteQuery(sql.ToString(),Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count != 1)
                throw new Exception("Invalid Email Subscription ID");

            DataRow row = ds.Tables[0].Rows[0];
            _SubscriptionName = row["subscription_name"].ToString();
            _Description = row["description"].ToString();
            _Application = row["application"].ToString();
            _Hide_User_Emails = (row["hide_user_emails"].ToString() == "1");

        }
        /// <summary>
        /// Sends an email to the group defined in the database
        /// </summary>
        /// <remarks>Returns the number of users the email is sent to</remarks>
        public int SendMail()
        {
            int SentUserCount = 0;
            //Update the last used date on the email susbscription, will help in removing old items
            Data.ExecuteNonQuery("UPDATE tolive.email_subscription SET last_used = SYSDATE WHERE email_subscription_id = " + EmailSubscriptionID, Data.MNODatabase.DMART);

            StringBuilder Sql = new();
            StringBuilder Recipients = new();

            //Get a list of users on this email subscription
            Sql.AppendLine("SELECT esu.username, TRIM(NVL(esu.email_address, su.email)) EmailAddress");
            Sql.AppendLine("FROM tolive.email_subscription_user esu");
            Sql.AppendLine("LEFT OUTER JOIN tolive.sec_users su");
            Sql.AppendLine("    ON LOWER(esu.username) = LOWER(su.domain || '\\' || su.username)");
            Sql.AppendLine("INNER JOIN tolive.email_subscription es");
            Sql.AppendLine("    ON esu.email_subscription_id = es.email_subscription_id");
            Sql.AppendLine("WHERE esu.email_subscription_id = " + EmailSubscriptionID);

            DataSet ds = Data.ExecuteQuery(Sql.ToString(), Data.MNODatabase.DMART);
            //if no records found, return 0
            if (ds.Tables[0].Rows.Count == 0)
                return 0;

            foreach(DataRow row in ds.Tables[0].Rows)
            {
                if (row.IsNull("EmailAddress"))
                {
                    //no email address for this user, log in the error table
                    Exceptions.ErrorLog.LogError("MOO.DLL", "Cannot send mail to subscribed user " + row["username"] +
                                            " because an email address is not set up.  Please update record in email_subscription_user or the sec_user table corresponding to this user.",
                                            "Email Subscription ID = " + EmailSubscriptionID, "", Exceptions.ErrorLog.ErrorLogType.Crash);
                }
                else
                {
                    if (Recipients.Length > 0)
                        Recipients.Append(',');

                    Recipients.Append(row["EmailAddress"]);
                    SentUserCount++;
                }
            }
            //check one more time and make sure we actually have some recipients
            if (Recipients.Length == 0)
                return 0;

            //create the body of the email, this will start with the MessageBody given by the caller and then add in a footer
            //containing additional information on how to unsubscribe

            StringBuilder Body = new();
            Body.AppendLine(MessageBody);
            Body.AppendLine("<br/>");
            Body.AppendLine("<br/>");
            Body.AppendLine("<br/>");
            Body.AppendLine("<br/>");
            Body.AppendLine("<br/>");
            Body.AppendLine(String.Format("<span style='font-style:italic'>This automated email \"{0}\" was sent from {1}.", SubscriptionName, Application));
            Body.AppendLine("If you are a USS user you can unsubscribe to this message at ");
            Body.AppendLine(EMAIL_SUBSCRIPTION_LINK);
            Body.AppendLine("<br/>");
            Body.AppendLine("If you are not a USS user, please email " + Settings.PC_Level2_Email + " to unsubscribe to this email.</span>");

            StringBuilder Attach = new();
            
            foreach(string a in Attachments)
            {
                if (Attach.Length == 0)
                    Attach.Append(',');
                Attach.Append(a);
            }

            if(Hide_User_Emails)
            {
                //send email one at a time
                foreach(string SendTo in Recipients.ToString().Split(","))
                {
                    Mail.SendMail("Email subscription - " + SubscriptionName + "(" + EmailSubscriptionID + ")",
                                   Settings.MailFromAddress, SendTo, "", "", Subject, Body.ToString(), true,
                                   System.Net.Mail.MailPriority.Normal, Attach.ToString());
                }
            }
            else
            {
                //send one email to everyone
                Mail.SendMail("Email subscription - " + SubscriptionName + "(" + EmailSubscriptionID + ")",
                                   Settings.MailFromAddress, Recipients.ToString(), "", "", Subject, Body.ToString(), true,
                                   System.Net.Mail.MailPriority.Normal, Attach.ToString());
            }



            return SentUserCount;
        }
    }
}
