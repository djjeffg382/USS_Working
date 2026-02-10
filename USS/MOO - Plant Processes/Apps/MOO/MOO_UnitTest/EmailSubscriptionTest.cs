using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MOO_UnitTest
{
    public class EmailSubscriptionTest
    {
        private const int EMAIL_SUB_ID = -999;


        [Fact]
        public void TestSend123456()
        {
            Util.RegisterFactories();
            //just testing the 123456 subscription in the database
            MOO.EmailSubscription MyMail = new(123456);
            MyMail.Subject = "Mail Subscription Test from " + Util.PROGRAM_NAME;
            MyMail.MessageBody = "test";
            MyMail.SendMail();
        }


        [Fact]
        public void SendEmailPackageTest()
        {
            //We will have to add tests to the database and then delete afterwards.
            //run the delete first in case it crashed on last run
            Util.RegisterFactories();
            DeleteTests();

            

            //create a dummy email package first
            StringBuilder Sql = new();
            Sql.AppendLine("INSERT INTO tolive.email_subscription(email_subscription_id, subscription_name,");
            Sql.AppendLine("    description, application, hide_user_emails)");
            Sql.AppendLine("VALUES(" + EMAIL_SUB_ID.ToString() + ",'MOO UNIT TEST','Testing MOO Subscription class','");
            Sql.AppendLine(Util.PROGRAM_NAME + "',0)");
            MOO.Data.ExecuteNonQuery(Sql.ToString(), MOO.Data.MNODatabase.DMART);

            MOO.EmailSubscription MyMail = new(EMAIL_SUB_ID);
            MyMail.Subject = "Mail Subscription Test from " + Util.PROGRAM_NAME;

            //Test one user

            MyMail.MessageBody = "This email sent from MOO Unit Test and should contain 1 email address";
            Sql.Clear();
            Sql.AppendLine("INSERT INTO tolive.email_subscription_user(subscriber_id, email_subscription_id,username, email_address)");
            Sql.AppendLine("VALUES(-999," + EMAIL_SUB_ID.ToString() + ",'" + Util.TEST_USER_ID + "',null)");
            MOO.Data.ExecuteNonQuery(Sql.ToString(), MOO.Data.MNODatabase.DMART);            
            MyMail.SendMail();

            //Test 2 users
            MyMail.MessageBody = "This email sent from MOO Unit Test and should contain 2 email addresses";
            Sql.Clear();
            Sql.AppendLine("INSERT INTO tolive.email_subscription_user(subscriber_id, email_subscription_id,username, email_address)");
            Sql.AppendLine("VALUES(-9999," + EMAIL_SUB_ID.ToString() + ",'" + Util.TEST_USER_ID2 + "',null)");
            MOO.Data.ExecuteNonQuery(Sql.ToString(), MOO.Data.MNODatabase.DMART);
            MyMail.SendMail();

            //modify the subscription to hide users and resend
            MOO.Data.ExecuteQuery("UPDATE tolive.email_subscription SET hide_user_emails = 1 WHERE email_subscription_id = " + EMAIL_SUB_ID, MOO.Data.MNODatabase.DMART);
            //have to reinstantiate the subscription to get the new setting
            MyMail = new(EMAIL_SUB_ID);
            MyMail.Subject = "Mail Subscription Test from " + Util.PROGRAM_NAME;
            MyMail.MessageBody = "This email sent from MOO Unit Test with hide users set to true.  Should only see your email address in the from";
            MyMail.SendMail();


            //Test if email address is not found, this should log to the error log table
            Sql.Clear();
            MOO.Data.ExecuteQuery("UPDATE tolive.email_subscription_user set username = 'mno\\DummyUser' WHERE subscriber_id = -9999",MOO.Data.MNODatabase.DMART);


            MyMail.MessageBody = "This email sent from MOO Unit Test testing an invalid user in the database to ensure error is logged";
            MyMail.SendMail();
            //check error log to see if this is in the error table
            DataSet ds = MOO.Data.ExecuteQuery("SELECT error_date FROM tolive.error " +
                        "WHERE error_date > SYSDATE -10/24/60/60 " +
                        "AND error_desc like '%DummyUser%'", MOO.Data.MNODatabase.DMART);
            //check if there was an error record with the DummyUser within the last 10 seconds
            Assert.True(ds.Tables[0].Rows.Count > 0);

            DeleteTests();

        }

        private static void DeleteTests()
        {
            MOO.Data.ExecuteNonQuery("DELETE FROM tolive.email_subscription_user WHERE email_subscription_id = " + 
                        EMAIL_SUB_ID.ToString(),MOO.Data.MNODatabase.DMART);
            MOO.Data.ExecuteNonQuery("DELETE FROM tolive.email_subscription WHERE email_subscription_id = " +
                        EMAIL_SUB_ID.ToString(), MOO.Data.MNODatabase.DMART);
        }
    }
}
