using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MOO_UnitTest
{
    public class MailTest
    {
        public MailTest()
        {
            //send mail records to tolive.mail_log so we need to register the DBFactories
            Util.RegisterFactories();
        }


        [Fact]
        public void SendMailTest()
        {

            //Test Empty Recipients, should throw exception
            Assert.Throws<ArgumentException>(() =>
                    MOO.Mail.SendMail("MOO.dll", MOO.Settings.MailFromAddress, "", "", "", "MOO.dll test",
                                "Test run from MOO Unit Test", false, System.Net.Mail.MailPriority.Normal, "")
            );

            //Test Empty From  parameter, should throw exception
            Assert.Throws<ArgumentException>(() =>
                    MOO.Mail.SendMail("MOO.dll", "", Util.TEST_EMAIL_ADDRESS, "", "", "MOO.dll test",
                                "Test run from MOO Unit Test", false, System.Net.Mail.MailPriority.Normal, "")
            );



            MOO.Mail.SendMail("MOO.dll", MOO.Settings.MailFromAddress, Util.TEST_EMAIL_ADDRESS, "", "", "MOO.dll test",
                                "Test run from MOO Unit Test", false, System.Net.Mail.MailPriority.Normal, "");

        }

        [Fact]
        public void SendMailTest2()
        {
            var mail = new MOO.MailMessage()
            {
                ProgramName = "MOO.Dll",
                RecipientsCsv = "bpaltman@uss.com",
                Subject = "Test",
                Body = "Test Body"
            };
            mail.Send();
        }
    }
}
