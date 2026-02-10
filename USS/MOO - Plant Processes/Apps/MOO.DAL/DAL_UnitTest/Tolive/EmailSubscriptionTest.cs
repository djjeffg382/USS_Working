using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;


namespace DAL_UnitTest.Tolive
{
    public class EmailSubscriptionTest : BaseTestClass
    {
        [Fact]
        public void TestEmailSubscription()
        {
            //test get
            DAL.Models.Email_Subscription es = DAL.Services.Email_SubscriptionSvc.Get(1);
            Assert.NotNull(es);

            List<DAL.Models.Email_Subscription> esList = DAL.Services.Email_SubscriptionSvc.GetAll();
            Assert.NotNull(esList);
            Assert.NotEmpty(esList);

            esList = DAL.Services.Email_SubscriptionSvc.GetByUsername("mno\\alt7747");
            Assert.NotNull(esList);
            Assert.NotEmpty(esList);

            esList = DAL.Services.Email_SubscriptionSvc.GetAll("MTC_AGG");
            Assert.NotNull(esList);
            Assert.NotEmpty(esList);

            //test insert
            es = new()
            {
                Application = "DAL Unit Test",
                Description = "DAL Unit Test",
                Hide_User_Emails = false,
                Subscription_Name = "DAL Unit Test",
                Tags = "ABC"
            };
            DAL.Services.Email_SubscriptionSvc.Insert(es);
            //test update
            es.Tags = "ABC123";
            DAL.Services.Email_SubscriptionSvc.Update(es);

            //test delete
            DAL.Services.Email_SubscriptionSvc.Delete(es);

        }

        [Fact]
        public void TestEmailSubscriptionUser()
        {
            //test get
            DAL.Models.Email_Subscription_User es = DAL.Services.Email_Subscription_UserSvc.Get(1);
            Assert.NotNull(es);

            List<DAL.Models.Email_Subscription_User> esList = DAL.Services.Email_Subscription_UserSvc.GetAll(9);
            Assert.NotNull(esList);
            Assert.NotEmpty(esList);

            es = DAL.Services.Email_Subscription_UserSvc.GetByUsername(9, "mno\\alt7747");
            Assert.NotNull(es);

            es = new()
            {
                Username = "mno\alt7747",
                Email_Address = "bpaltman@uss.com",
                Comments = "DAL Unit Test",
                Email_Subscription_Id = 1
            };
            DAL.Services.Email_Subscription_UserSvc.Insert(es);
            //test update
            es.Comments = "ABC123";
            DAL.Services.Email_Subscription_UserSvc.Update(es);

            //test delete
            DAL.Services.Email_Subscription_UserSvc.Delete(es);
        }
        [Fact]
        public void MoveDomainTest()
        {
            int aa = DAL.Services.Email_Subscription_UserSvc.MoveDomainMnoToHdq("alt7747");
        }
    }
}
