using MOO.DAL.ToLive.Models;
using MOO.DAL.ToLive.Services;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public class Prop_Remove_Tests
    {
        [Fact]
        public void Misctest()
        {
            //var  a = DAL.Services.Prop_Rmv_Form_ApprovalSvc.GetRequiredApprovals(50000).GetAwaiter().GetResult();

            var usr = Sec_UserSvc.Get(4);
            var myAppvForms = DAL.Services.Prop_Rmv_FormSvc.GetUserApproveAsync(usr).GetAwaiter().GetResult();
            Assert.NotNull(myAppvForms);
        }


        [Fact]
        public void Test_Vendor()
        {
            //create a vendor
            DAL.Models.Prop_Rmv_Vendor v = new()
            {
                Vendor_Name = $"Test Vendor{DateTime.Now}",
                Active = true
            };
            //Test insert
            Assert.Equal(1,DAL.Services.Prop_Rmv_VendorSvc.InsertAsync(v).GetAwaiter().GetResult());

            //Test Update
            v.Active = false;
            Assert.Equal(1, DAL.Services.Prop_Rmv_VendorSvc.UpdateAsync(v).GetAwaiter().GetResult());

            //Test Get
            v = DAL.Services.Prop_Rmv_VendorSvc.GetAsync(v.Prop_Rmv_Vendor_Id).GetAwaiter().GetResult();
            Assert.NotNull(v);

            //Test GetAll
            var vList = DAL.Services.Prop_Rmv_VendorSvc.GetAllAsync(true).GetAwaiter().GetResult();
            Assert.NotEmpty(vList);

            // Test Delete
            Assert.Equal(1, DAL.Services.Prop_Rmv_VendorSvc.DeleteAsync(v).GetAwaiter().GetResult());

        }


        [Fact]
        public void Test_Reason()
        {
            //create a reason
            DAL.Models.Prop_Rmv_Reason r = new()
            {
                Reason_Name = $"Test Reason{DateTime.Now}",
                Active = true
            };
            //Test insert
            Assert.Equal(1, DAL.Services.Prop_Rmv_ReasonSvc.InsertAsync(r).GetAwaiter().GetResult());

            //Test Update
            r.Active = false;
            Assert.Equal(1, DAL.Services.Prop_Rmv_ReasonSvc.UpdateAsync(r).GetAwaiter().GetResult());

            //Test Get
            r = DAL.Services.Prop_Rmv_ReasonSvc.GetAsync(r.Prop_Rmv_Reason_Id).GetAwaiter().GetResult();
            Assert.NotNull(r);

            //Test GetAll
            var rList = DAL.Services.Prop_Rmv_ReasonSvc.GetAllAsync(true).GetAwaiter().GetResult();
            Assert.NotEmpty(rList);

            // Test Delete
            Assert.Equal(1, DAL.Services.Prop_Rmv_ReasonSvc.DeleteAsync(r).GetAwaiter().GetResult());

        }


        [Fact]
        public void Test_Area()
        {
            //create a area
            DAL.Models.Prop_Rmv_Area a = new()
            {
                Plant = MOO.Plant.Minntac,
                Area_Name = $"Test Area{DateTime.Now}",
                Active = true
            };
            //Test insert
            Assert.Equal(1, DAL.Services.Prop_Rmv_AreaSvc.InsertAsync(a).GetAwaiter().GetResult());

            //Test Update
            a.Active = false;
            Assert.Equal(1, DAL.Services.Prop_Rmv_AreaSvc.UpdateAsync(a).GetAwaiter().GetResult());

            //Test Get
            a = DAL.Services.Prop_Rmv_AreaSvc.GetAsync(a.Prop_Rmv_Area_Id).GetAwaiter().GetResult();
            Assert.NotNull(a);

            //Test GetAll
            var aList = DAL.Services.Prop_Rmv_AreaSvc.GetAllAsync(true).GetAwaiter().GetResult();
            Assert.NotEmpty(aList);

            // Test Delete
            Assert.Equal(1, DAL.Services.Prop_Rmv_AreaSvc.DeleteAsync(a).GetAwaiter().GetResult());

        }

        [Fact]
        public void TestProp_Rmv_Reason_Aprrover()
        {
            var usr = Sec_UserSvc.Get(4);

            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            var trans = conn.BeginTransaction();
            //create a reason
            DAL.Models.Prop_Rmv_Reason r = new()
            {
                Reason_Name = $"Test Reason{DateTime.Now}",
                Active = true
            };
            //Test insert
            Assert.Equal(1, DAL.Services.Prop_Rmv_ReasonSvc.InsertAsync(r,conn).GetAwaiter().GetResult());

            //add an approver
            Assert.Equal(1, DAL.Services.Prop_Rmv_Reason_ApproverSvc.AddApproverAsync(r,usr,conn).GetAwaiter().GetResult());
           
            trans.Commit();

            var approveList = DAL.Services.Prop_Rmv_Reason_ApproverSvc.GetByReason(r.Prop_Rmv_Reason_Id).GetAwaiter().GetResult();
            Assert.NotEmpty(approveList);
            
            Assert.Equal(1, DAL.Services.Prop_Rmv_Reason_ApproverSvc.RemoveApproverAsync(r,usr,conn).GetAwaiter().GetResult());

            DAL.Services.Prop_Rmv_Reason_ApproverSvc.RemoveAllApproversAsync(r, conn).GetAwaiter().GetResult();
            Assert.Equal(1, DAL.Services.Prop_Rmv_ReasonSvc.DeleteAsync(r, conn).GetAwaiter().GetResult());

            conn.Close();
            
        }


        [Fact]
        public void TestProp_Rmv_Area_Aprrover()
        {
            var usr = Sec_UserSvc.Get(4);

            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            var trans = conn.BeginTransaction();
            //create a area
            DAL.Models.Prop_Rmv_Area a = new()
            {
                Area_Name = $"Test Area{DateTime.Now}",
                Active = true
            };
            //Test insert
            Assert.Equal(1, DAL.Services.Prop_Rmv_AreaSvc.InsertAsync(a, conn).GetAwaiter().GetResult());

            //add an approver
            Assert.Equal(1, DAL.Services.Prop_Rmv_Area_ApproverSvc.AddApproverAsync(a, usr, conn).GetAwaiter().GetResult());

            trans.Commit();

            var approveList = DAL.Services.Prop_Rmv_Area_ApproverSvc.GetByArea(a.Prop_Rmv_Area_Id).GetAwaiter().GetResult();
            Assert.NotEmpty(approveList);

            Assert.Equal(1, DAL.Services.Prop_Rmv_Area_ApproverSvc.RemoveApproverAsync(a, usr, conn).GetAwaiter().GetResult());

            DAL.Services.Prop_Rmv_Area_ApproverSvc.RemoveAllApproversAsync(a, conn).GetAwaiter().GetResult();
            Assert.Equal(1, DAL.Services.Prop_Rmv_AreaSvc.DeleteAsync(a, conn).GetAwaiter().GetResult());

            conn.Close();

        }

        [Fact]
        public void PropRemoveFormTest()
        {
            var usr = Sec_UserSvc.Get(4);

            //we will need a reason, vendor, and area first
            //create a vendor
            DAL.Models.Prop_Rmv_Vendor vdr = new()
            {
                Vendor_Name = $"Test Vendor{DateTime.Now}",
                Active = true
            };
            Assert.Equal(1, DAL.Services.Prop_Rmv_VendorSvc.InsertAsync(vdr).GetAwaiter().GetResult());

            //create a reason
            DAL.Models.Prop_Rmv_Reason rsn = new()
            {
                Reason_Name = $"Test Reason{DateTime.Now}",
                Active = true
            };
            Assert.Equal(1, DAL.Services.Prop_Rmv_ReasonSvc.InsertAsync(rsn).GetAwaiter().GetResult());

            //create a area
            DAL.Models.Prop_Rmv_Area area = new()
            {
                Plant = MOO.Plant.Minntac,
                Area_Name = $"Test Area{DateTime.Now}",
                Active = true
            };
            Assert.Equal(1, DAL.Services.Prop_Rmv_AreaSvc.InsertAsync(area).GetAwaiter().GetResult());

            DAL.Models.Prop_Rmv_Form form = new()
            {
                Area= area,
                Reason = rsn,
                Vendor = vdr,
                Created_By = usr,
                Created_Date = DateTime.Now,
                Status = DAL.Models.Prop_Rmv_Form.PrStatus.Open,
                Vendor_Contact="Testing Form",
                To_Be_Returned = true
            };

            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            using OracleTransaction trans = conn.BeginTransaction();


            Assert.Equal(1, DAL.Services.Prop_Rmv_FormSvc.InsertAsync(form,conn).GetAwaiter().GetResult());
            //now add some line items

            DAL.Models.Prop_Rmv_Form_Line lineItm = new()
            {
                Item_Nbr = 1,
                Line_Description = "Test Line Item",
                Quantity = 1,
                Prop_Rmv_Form_Id = form.Prop_Rmv_Form_Id
            };
            Assert.Equal(1, DAL.Services.Prop_Rmv_Form_LineSvc.InsertAsync(lineItm, conn).GetAwaiter().GetResult());
            trans.Commit();
            conn.Close();

            var lines = DAL.Services.Prop_Rmv_Form_LineSvc.GetByFormIdAsync(form.Prop_Rmv_Form_Id).GetAwaiter().GetResult();
            Assert.NotEmpty(lines);

            form = DAL.Services.Prop_Rmv_FormSvc.GetAsync(form.Prop_Rmv_Form_Id).GetAwaiter().GetResult();
            Assert.NotNull(form);

            var formList = DAL.Services.Prop_Rmv_FormSvc.GetAllAsync(DateTime.Now.AddYears(-1), DateTime.Now.AddYears(1)).GetAwaiter().GetResult();
            Assert.NotEmpty(formList);

            form.Created_Date = DateTime.Now;
            Assert.Equal(1, DAL.Services.Prop_Rmv_FormSvc.UpdateAsync(form).GetAwaiter().GetResult());



            TestFormApprovals(form);


            Assert.Equal(1, DAL.Services.Prop_Rmv_Form_LineSvc.DeleteAsync(lineItm).GetAwaiter().GetResult());
            Assert.Equal(1, DAL.Services.Prop_Rmv_FormSvc.DeleteAsync(form).GetAwaiter().GetResult());

        }

        private void TestFormApprovals(Prop_Rmv_Form Form)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            var usr = Sec_UserSvc.Get(4);
            //add an approver to the area for this form
            var areaApprovers = DAL.Services.Prop_Rmv_Area_ApproverSvc.GetByArea(Form.Area.Prop_Rmv_Area_Id).GetAwaiter().GetResult();
            if(areaApprovers.Count == 0)
                DAL.Services.Prop_Rmv_Area_ApproverSvc.AddApproverAsync(Form.Area, usr, conn).GetAwaiter().GetResult();
            
            var approvals = DAL.Services.Prop_Rmv_Form_ApprovalSvc.GetRequiredApprovals(Form.Prop_Rmv_Form_Id).GetAwaiter().GetResult();
            Assert.NotNull(approvals);
            Assert.NotEmpty(approvals.ApprovalList);


            var myAppvForms = DAL.Services.Prop_Rmv_FormSvc.GetUserApproveAsync(usr).GetAwaiter().GetResult();
            Assert.NotNull(myAppvForms);
            



        }
    }
}
