using MOO.DAL.ToLive.Models;
using MOO.DAL.ToLive.Services;
using System;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public class SecRequestTest : BaseTestClass
    {
        [Fact]
        public void RequestItemTest()
        {
            var requests = Sec_Request_ItemSvc.GetAll();
            Assert.NotEmpty(requests);


            var request = Sec_Request_ItemSvc.Get(6);
            Assert.NotNull(request);

            var role = Sec_RoleSvc.Get(294);

            Sec_Request_Item newRequestItem = new()
            {
                Item_Name = "Test Item",
                Action_Role = role,
                Active = false,
                Request_Comment_Header = "comment",
            };

            Assert.Equal(1, Sec_Request_ItemSvc.Insert(newRequestItem));
            newRequestItem.Item_Name = "Test Item2";
            Assert.Equal(1, Sec_Request_ItemSvc.Update(newRequestItem));
            Assert.Equal(1, Sec_Request_ItemSvc.Delete(newRequestItem));
        }

        [Fact]
        public void RequestTest()
        {
            var request = Sec_RequestSvc.Get(7);
            Assert.NotNull(request);

            var requests = Sec_RequestSvc.GetAllByItemId(1);
            Assert.NotEmpty(requests);

            requests = Sec_RequestSvc.GetAll();
            Assert.NotEmpty(requests);
            //requests = Sec_RquestSvc.GetByUserID(8574);
            //Assert.NotEmpty(requests);

            var user1 = Sec_UserSvc.Get(20220);            
            var user2 = Sec_UserSvc.Get(177);
            var requestItem = Sec_Request_ItemSvc.Get(6);
            var status = Sec_Request_StatusSvc.Get(5);

            DAL.Models.Sec_Request newRequest = new()
            {
                Request_First_Name = "1",
                Request_Last_Name = "1",
                Request_Username = "1",
                Manager = user2,
                Sec_Request_Item = requestItem,
                Request_Comments = "no u",
                Created_Date = DateTime.Now,
                User = user1,
            };

            Assert.Equal(1,Sec_RequestSvc.Insert(newRequest));
            newRequest.Request_First_Name = "Jeff";
            newRequest.Request_Last_Name = "Kayfes";
            Assert.Equal(1,Sec_RequestSvc.Update(newRequest));
            Assert.Equal(1,Sec_RequestSvc.Delete(newRequest));
        }
    }
}


