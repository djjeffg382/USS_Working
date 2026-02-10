using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DAL_UnitTest.Tolive
{
    public class FireSecurityTests : BaseTestClass
    {
        [Fact]
        public void WorkOrderTest()
        {
            var workList = DAL.Services.FS_WorkOrdersSvc.GetAll(false, MOO.Plant.Minntac);
            var newWork = DAL.Services.FS_WorkOrdersSvc.Get(630);
            newWork.Workorders_Id = 0;
            DAL.Services.FS_WorkOrdersSvc.Insert(newWork);
            Assert.NotEmpty(workList);
        }
        [Fact]
        public void  TempBadgeTest()
        {
            //var tt = DAL.Services.FS_TempBadgeSvc.Get(50609);
            var test = DAL.Services.FS_TempBadgeSvc.GetAll(false, MOO.Plant.Keetac);
            Assert.NotNull(test);
            DAL.Models.FS_TempBadge tb = DAL.Services.FS_TempBadgeSvc.Get(12378);

            tb.Temp_Badge_Issued = DateTime.Now;
            tb.Temp_Id = 0;
            tb.Employee_Id = new DAL.Models.People();
            tb.Manager_Id = new DAL.Models.People();
            tb.Department.Department = "Contractor";

            DAL.Services.FS_TempBadgeSvc.Insert(tb);
            //Assert.NotNull(tb);
            //tb.Temp_Id = 0;
            //tb.C_Companyname = "Stark Inc";
            //DAL.Services.FS_TempBadgeSvc.Insert(tb);
            //tb.Reasonfor = "Test";
            //DAL.Services.FS_TempBadgeSvc.Update(tb);
            //DAL.Services.FS_TempBadgeSvc.Delete(tb);
            

            //var a = 0;


            //try
            //{
            //    DAL.Services.FS_TempBadgeSvc.Insert(addnew);

            //}
            //catch (Exception e)
            //{
            //    var msg = e.Message;
            //}


            //var tt = DAL.Services.FS_TempBadgeSvc.Get(50573);
            //tt.Plantloc = MOO.Plant.Minntac;

            ////tt.Temp_Id = 0;
            ////DAL.Services.FS_TempBadgeSvc.Insert(tt);
            //tt.Reasonfor = "Test";
            //DAL.Services.FS_TempBadgeSvc.Update(tt);
            //tt.C_Companyname = "US Steel";
            //tt.C_Name = "";
            //tt.C_Supername = "";
            //tt.Plantloc = MOO.Plant.Keetac;
            //DAL.Services.FS_TempBadgeSvc.Update(tt);
            //DAL.Services.FS_TempBadgeSvc.Delete(tt);
            //var test = DAL.Services.FS_TempBadgeSvc.GetAll(false);
            //var test1 = DAL.Services.FS_TempBadgeSvc.GetAll(true, MOO.Plant.Keetac);
            //Assert.NotEmpty(test);
            //Assert.NotEmpty(test1);
        }

        [Fact]
        public void RentalTest()
        {
            var rentalUpt = DAL.Services.FS_RentalEquipSvc.Get(160);
            rentalUpt.Comments += " Testing";
            rentalUpt.Rental_Equip_Id = 0;
            DAL.Services.FS_RentalEquipSvc.Insert(rentalUpt);
            rentalUpt.Plantloc = MOO.Plant.Keetac;
            var rentalList = DAL.Services.FS_RentalEquipSvc.GetAll();
            Assert.NotEmpty(rentalList);
        }

        [Fact]
        public void ConfineSpaceTest()
        {
            var test = DAL.Services.FS_ConfSpaceSvc.Get(50554);
            test.Confinedspace_Id = 0;
            test.Plant = MOO.Plant.Keetac;
            test.Date_Open = DateTime.Now;

            DAL.Services.FS_ConfSpaceSvc.Insert(test);
            var confSpaceList = DAL.Services.FS_ConfSpaceSvc.GetAll(true, MOO.Plant.Minntac);
            Assert.NotNull(confSpaceList);
            var test1 = DAL.Services.FS_ConfSpaceSvc.Get(50554);
            Assert.NotNull(test1);
            test.Audited = false;
            DAL.Services.FS_ConfSpaceSvc.Update(test);

        }

        [Fact]
        public void CritiqueTest()
        {

            DAL.Models.FS_Critique fs = DAL.Services.FS_CritiqueSvc.Get(50553);
            fs.Scenario_Desc = "Testing update 24";
            DAL.Services.FS_CritiqueSvc.Update(fs);
            fs.Post_ID = 50559;
            fs.Scenario_Desc = "Testing update 87";
            DAL.Services.FS_CritiqueSvc.Insert(fs);

            Assert.NotNull(fs);
        }

        [Fact]
        public void HotworksTest()
        {
            var hotworks = DAL.Services.FS_HotworksSvc.GetAll();
            var hotworks1 = DAL.Services.FS_HotworksSvc.GetAll(false, MOO.Plant.Minntac);
            Assert.NotEmpty(hotworks1);
            var hotworks2 = DAL.Services.FS_HotworksSvc.GetAll(false, MOO.Plant.Keetac);
            Assert.NotEmpty(hotworks2);
            Assert.NotNull(hotworks);
            //Enter new hotworks, update it, then delete it
            DAL.Models.FS_Hotworks fh = new()
            {
                Hotworks_Id = 0,
                Permit_Nbr = "0099999",
                Supervisor = "GENE SMITH",
                Employee_Nbr = "LAKEHEAD",
                Phone_Nbr = "218-260-3202",
                Location = "CRUSHER 0-11-02 GEARBOX",
                Date_Open = DateTime.Parse("2018-05-15 18:56:00"),
                Date_Close = DateTime.Parse("2018-05-19 18:56:00"),
                Badge_Nbr_Open = "308 ELJ",
                Badge_Nbr_Close = "308 ELJ",
                Comments = "",
                Audited = false,
                Audited_By = "Testing",
                Plant = MOO.Plant.Minntac

            };

            DAL.Services.FS_HotworksSvc.Insert(fh);

            //Test getall function with active/inactive. Set GetAll(false) to get all results
            List<DAL.Models.FS_Hotworks> listActive = DAL.Services.FS_HotworksSvc.GetAll();
            Assert.NotNull(listActive);
            List<DAL.Models.FS_Hotworks> listAll = DAL.Services.FS_HotworksSvc.GetAll(false);
            Assert.NotNull(listAll);

            fh.Supervisor = "Branden Stark";
            DAL.Services.FS_HotworksSvc.Update(fh);
            DAL.Services.FS_HotworksSvc.Delete(fh);
        }

        [Fact]
        public void Temp_Badge_DepartmentTest()
        {
            var person = DAL.Services.PeopleSvc.Get(70029308);
            var department = DAL.Services.Temp_Badge_DepartmentSvc.Get(45);
            var tbd = DAL.Services.Temp_Badge_DepartmentSvc.GetAll(MOO.Plant.Keetac);
            Assert.NotEmpty(tbd);

            var addnew = new DAL.Models.FS_TempBadge()
            {
                Employee_Id = person,
                Manager_Id = person,
                Validbadge = "No",
                Plantloc = MOO.Plant.Minntac,
                Msha_Expdate = DateTime.Now,
                Temp_Badgenbr = 22,
                Temp_Badge_Issued = DateTime.Now,
                Sec_Officer = person.First_Name,
                Is_Emp_Conc = "Employee",
                Department = department,
                Reasonfor = "Test"

            };
            //List<DAL.Models.Temp_Badge_Department> departmentAll = DAL.Services.Temp_Badge_DepartmentSvc.GetAll(MOO.Plant.Keetac);
            var list = DAL.Services.Temp_Badge_DepartmentSvc.GetAll(addnew.Department.Plantloc);
            var listM = DAL.Services.Temp_Badge_DepartmentSvc.GetAll(MOO.Plant.Keetac);

            Assert.NotEmpty(list);
            Assert.NotEmpty(listM);
            DAL.Models.Temp_Badge_Department tbdNew = new()
            {
                Department = "Test123",
                Email_Address = "abc@abc.com",
                Plantloc = MOO.Plant.Minntac
            };

            Assert.Equal(1, DAL.Services.Temp_Badge_DepartmentSvc.Insert(tbdNew));

            tbdNew.Department = "testChange";
            Assert.Equal(1, DAL.Services.Temp_Badge_DepartmentSvc.Update(tbdNew));

            Assert.Equal(1, DAL.Services.Temp_Badge_DepartmentSvc.Delete(tbdNew));

        }

        [Fact]
        public void TenantTest()
        {
            var tenantobj = DAL.Services.TenantSvc.Get(1);
            tenantobj.Tenant_Id = 0;
            tenantobj.Tenant_Issue = "Really Dusty";
            var newobj = DAL.Services.TenantSvc.Insert(tenantobj);
            DAL.Services.TenantSvc.Update(tenantobj);
            var tenantList = DAL.Services.TenantSvc.GetAll();
            Assert.NotEmpty(tenantList);
            DAL.Services.TenantSvc.Delete(tenantobj);
        }
    }
}
