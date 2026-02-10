using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO
{
    /// <summary>
    /// Mail Message Object for sending mail.  Alternative to MOO.Mail
    /// </summary>
    public class MailMessage
    {
        /// <summary>
        /// The name of the program sending the mail
        /// </summary>
        public string ProgramName { get; set; } = "";


        /// <summary>
        /// The subject of the email
        /// </summary>
        public string Subject { get; set; } = "";

        /// <summary>
        /// The body of the email
        /// </summary>
        public string Body { get; set; } = "";

        /// <summary>
        /// The From Address
        /// </summary>
        public string From { get; set; } = MOO.Settings.MailFromAddress;

        /// <summary>
        /// The priority of the email
        /// </summary>
        public System.Net.Mail.MailPriority Priority { get; set; }

        /// <summary>
        /// Indicates if the email should be sent as html encoded
        /// </summary>
        public bool IsHtml { get; set; } = false;

        #region "Recipients"
        private List<string> _recipients = [];

        /// <summary>
        /// List of recipients for the email
        /// </summary>
        public List<string> Recipients
        {
            get
            {
                return _recipients;
            }
            set
            {
                _recipients = value;
            }
        }

        /// <summary>
        /// Comma separated string of recipients
        /// </summary>
        public string RecipientsCsv
        {
            get
            {
                return string.Join(",", _recipients);
            }
            set
            {
                _recipients = value.Split(',').ToList();
            }
        }
        #endregion


        #region "Carbon Copy"
        private List<string> _cc = [];

        /// <summary>
        /// List of Carbon Copy recipients for the email
        /// </summary>
        public List<string> Cc
        {
            get
            {
                return _cc;
            }
            set
            {
                _cc = value;
            }
        }

        /// <summary>
        /// Comma separated string of Carbon Copy recipients
        /// </summary>
        public string CcCsv
        {
            get
            {
                return string.Join(",", _cc);
            }
            set
            {
                _cc = value.Split(',').ToList();
            }
        }
        #endregion


        #region "Blind Carbon Copy"
        private List<string> _bcc = [];

        /// <summary>
        /// List of Bcc Recipients for the email
        /// </summary>
        public List<string> Bcc
        {
            get
            {
                return _bcc;
            }
            set
            {
                _bcc = value;
            }
        }

        /// <summary>
        /// Comma separated string of Blind Carbon Copy recipients
        /// </summary>
        public string BccCsv
        {
            get
            {
                return string.Join(",", _bcc);
            }
            set
            {
                _bcc = value.Split(',').ToList();
            }
        }
        #endregion


        #region "Attachments"
        private List<string> _attachments = [];

        /// <summary>
        /// List of Attachments for the email
        /// </summary>
        public List<string> Attachments
        {
            get
            {
                return _attachments;
            }
            set
            {
                _attachments = value;
            }
        }

        /// <summary>
        /// Comma separated string of Attachments
        /// </summary>
        public string AttachmentsCsv
        {
            get
            {
                return string.Join(",", _attachments);
            }
            set
            {
                _attachments = value.Split(',').ToList();
            }
        }
        #endregion


        /// <summary>
        /// Sends the Mail Object through email
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void Send()
        {

            //Validate parameters
            if (string.IsNullOrEmpty(From.Trim()))
                throw new ArgumentException("Must Provide From Address for sending Email");

            if (Recipients.Count == 0)
                throw new ArgumentException("Must Provide at least one recipient for sending Email");

            if (string.IsNullOrEmpty(ProgramName.Trim()))
                throw new ArgumentException("Must Provide Program Name for sending Email");



            System.Net.Mail.MailMessage Msg = new();
            Msg.From = new System.Net.Mail.MailAddress(From);

            //add the to addresses
            foreach (string sendTo in Recipients)
            {
                Msg.To.Add(new System.Net.Mail.MailAddress(sendTo));
            }

            //Add any cc addresses
            if (Cc.Count > 0)
            {
                foreach (string sendTo in Cc)
                {
                    Msg.To.Add(new System.Net.Mail.MailAddress(sendTo));
                }
            }

            //Add any BCC addresses
            if (Bcc.Count > 0)
            {
                foreach (string sendTo in Bcc)
                {
                    Msg.To.Add(new System.Net.Mail.MailAddress(sendTo));
                }
            }

            //Add any file attachments
            if (Attachments.Count > 0)
            {
                foreach (string File in Attachments)
                {
                    Msg.Attachments.Add(new System.Net.Mail.Attachment(File));
                }
            }

            Msg.Subject = Subject;
            Msg.Body = Body;
            Msg.IsBodyHtml = IsHtml;
            Msg.Priority = Priority;

            System.Net.Mail.SmtpClient Smtp = new()
            {
                Host = Settings.SMTP_Server
            };
            Smtp.Send(Msg);
            MOO.Mail.RecordToMailLog(ProgramName, From, RecipientsCsv, CcCsv, BccCsv, Subject, IsHtml, Priority, AttachmentsCsv);
        }

    }
}
