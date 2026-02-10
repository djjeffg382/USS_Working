using MOO.DAL.ToLive.Models;
using System;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public class PropertyRemovalTest:BaseTestClass
    {
        [Fact]
        public void ReasonsTest()
        {
            var reasons = DAL.Services.Property_Removal_ReasonsSvc.GetAll();
            Assert.NotEmpty(reasons);

            var obj = DAL.Services.Property_Removal_ReasonsSvc.Get(13);
            Assert.Equal("Donation", obj.Reason);

            Property_Removal_Reasons newReason = new Property_Removal_Reasons();
            newReason.Reason = "Test";
            newReason.Active = false;
            newReason.Reason_Id = 0;
            Assert.Equal(1,DAL.Services.Property_Removal_ReasonsSvc.Insert(newReason));

            newReason.Reason = "Updated";
            Assert.Equal(1,DAL.Services.Property_Removal_ReasonsSvc.Update(newReason));

            Assert.Equal(1,DAL.Services.Property_Removal_ReasonsSvc.Delete(newReason));
        }

        [Fact]
        public void VendorsTest()
        {
            var vendors = DAL.Services.Property_Removal_VendorsSvc.GetAll();
            Assert.NotEmpty(vendors);

            var obj = DAL.Services.Property_Removal_VendorsSvc.Get(246);
            Assert.Equal("CIBA", obj.Company_Name);

            Property_Removal_Vendors newVendor = new Property_Removal_Vendors();
            newVendor.Company_Name = "Test";
            newVendor.Active = false;
            newVendor.Company_Id = 0;
            Assert.Equal(1, DAL.Services.Property_Removal_VendorsSvc.Insert(newVendor));

            newVendor.Company_Name = "Updated";
            Assert.Equal(1, DAL.Services.Property_Removal_VendorsSvc.Update(newVendor));

            Assert.Equal(1, DAL.Services.Property_Removal_VendorsSvc.Delete(newVendor));
        }

        [Fact]
        public void TrackedTest()
        {
            var objects = DAL.Services.Property_Removal_TrackedSvc.GetAll();
            Assert.NotEmpty(objects);

            var obj = DAL.Services.Property_Removal_TrackedSvc.Get(140);
            Assert.Equal(140,obj.Tracked_By.User_Id);

            Property_Removal_Tracked newTracked = new Property_Removal_Tracked();
            newTracked.Tracked_By = DAL.Services.Sec_UserSvc.Get(140);
            newTracked.Plant = MOO.Plant.Minntac;
            newTracked.Reason = DAL.Services.Property_Removal_ReasonsSvc.Get(9);

            Assert.Equal(1, DAL.Services.Property_Removal_TrackedSvc.Delete(newTracked));

            Assert.Equal(1, DAL.Services.Property_Removal_TrackedSvc.Insert(newTracked));

            newTracked.Tracked_By = DAL.Services.Sec_UserSvc.Get(5474);
            Assert.Equal(1, DAL.Services.Property_Removal_TrackedSvc.Update(newTracked));

            newTracked.Tracked_By = DAL.Services.Sec_UserSvc.Get(140);
            Assert.Equal(1, DAL.Services.Property_Removal_TrackedSvc.Update(newTracked));
        }

        [Fact]
        public void AuthorizedTest()
        {
            var objects = DAL.Services.Property_Removal_AuthorizedSvc.GetAll();
            Assert.NotEmpty(objects);

            var obj = DAL.Services.Property_Removal_AuthorizedSvc.Get(30);
            Assert.NotNull(obj);

            obj.Authorized_By = DAL.Services.Sec_UserSvc.Get(20220);
            Assert.Equal(1, DAL.Services.Property_Removal_AuthorizedSvc.Insert(obj));

            Assert.Equal(1, DAL.Services.Property_Removal_AuthorizedSvc.Delete(obj));
        }


        [Fact]
        public void PropRemovalFormTests()
        {
            var forms = DAL.Services.Property_Removal_FormSvc.GetByCreatedDateAsync(DateTime.Today.AddYears(-20), DateTime.Today).GetAwaiter().GetResult();

            var person = DAL.Services.Sec_UserSvc.Get(4);  //This is me :)

            //Create a new one
            var newForm = new DAL.Models.Property_Removal_Form()
            {
                Created_Date = DateTime.Now,
                Creator = person,
                Voider = person,
                Tracker = person,
                Authorizer = person,
                Closer = person,
                Plant_Area = "Tire Shop"
            };
            DAL.Services.Property_Removal_FormSvc.InsertAsync(newForm).GetAwaiter().GetResult();
            //insert line item for this
            InsertLineItem(newForm.Form_Nbr);


            var form1 = DAL.Services.Property_Removal_FormSvc.GetAsync(newForm.Form_Nbr).GetAwaiter().GetResult();
            Assert.NotNull(form1);

            form1.Updater = person;
            Assert.Equal(1, DAL.Services.Property_Removal_FormSvc.UpdateAsync(form1).GetAwaiter().GetResult());

            Assert.Equal(1, DAL.Services.Property_Removal_FormSvc.DeleteAsync(form1).GetAwaiter().GetResult());

        }

        private void InsertLineItem(int FormNumber)
        {

            var item = new DAL.Models.Property_Removal_Line_Item()
            {
                Form_Nbr = FormNumber,
                Line_Item_Nbr = 1,
                Quantity = 5,
                Line_Desc = "Test"

            };

            Assert.Equal(1, DAL.Services.Property_Removal_Line_ItemSvc.InsertAsync(item).GetAwaiter().GetResult());

            var items = DAL.Services.Property_Removal_Line_ItemSvc.GetByFormNbrAsync(FormNumber).GetAwaiter().GetResult();
            Assert.NotEmpty(items);

            item.Est_Return_Date = DateTime.Now;
            Assert.Equal(1, DAL.Services.Property_Removal_Line_ItemSvc.UpdateAsync(item).GetAwaiter().GetResult());

            Assert.Equal(1, DAL.Services.Property_Removal_Line_ItemSvc.DeleteAsync(item).GetAwaiter().GetResult());
        }
    }
}
