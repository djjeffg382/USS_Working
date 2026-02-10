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
    public class Sec_TablesTest : BaseTestClass
    {
        [Fact]
        public void TestGetUser()
        {

            DAL.Models.Sec_Users usr = DAL.Services.Sec_UserSvc.Get("mno\\alt7747");
            Assert.IsType<DAL.Models.Sec_Users>(usr);

            usr = DAL.Services.Sec_UserSvc.Get("mno", "alt7747");
            Assert.IsType<DAL.Models.Sec_Users>(usr);

            //Check the Roles property code
            Assert.IsType<List<DAL.Models.Sec_Role>>(usr.Roles);
            Assert.True(usr.Roles.Count > 0);

            usr = DAL.Services.Sec_UserSvc.Get(1);
            Assert.IsType<DAL.Models.Sec_Users>(usr);

            usr.Last_Edited_Date = DateTime.Now;
            DAL.Services.Sec_UserSvc.Update(usr);


            


        }
        [Fact]
        public void TestUpdateUser()
        {
            DAL.Models.Sec_Users usr;
            usr = DAL.Services.Sec_UserSvc.Get(1);
            Assert.IsType<DAL.Models.Sec_Users>(usr);

            usr.Last_Edited_Date = DateTime.Now;
            DAL.Services.Sec_UserSvc.Update(usr);
        }


        [Fact]
        public void TestInsertUser()
        {
            DAL.Models.Sec_Users usr;
            usr = new()
            {
                First_Name = "test",
                Last_Name = "Test2",
                Username = "test123",
                Domain = "MNO"
            };

            DAL.Services.Sec_UserSvc.Insert(usr);
            //no delete the record
            string sql = $"delete from sec_users where user_id ={usr.User_Id}";
            MOO.Data.ExecuteNonQuery(sql, MOO.Data.MNODatabase.DMART);
        }


        [Fact]
        public void TestGetRole()
        {
            DAL.Models.Sec_Role role = DAL.Services.Sec_RoleSvc.Get(841);
            Assert.NotNull(role);

            role = DAL.Services.Sec_RoleSvc.Get("K_PelEntry");
            Assert.NotNull(role);

            List<DAL.Models.Sec_Role> sr = DAL.Services.Sec_RoleSvc.GetAll(false);
            Assert.NotNull(sr);
        }

        [Fact]
        public void TestRoleInsertUpdate()
        {
            DAL.Models.Sec_Role role = new()
            {
                Role_Name = "Test",
                Role_Description = "Test Desc",
                Role_Notes = "Role notes go here",
                Created_By = "Me"
            };
            DAL.Services.Sec_RoleSvc.Insert(role);

            //no check the role is in the datbase
            role = DAL.Services.Sec_RoleSvc.Get(role.Role_Id);
            Assert.NotNull(role);

            role.Role_Description = "testing update";
            DAL.Services.Sec_RoleSvc.Update(role);

            //now delete the role

            MOO.Data.ExecuteNonQuery($"DELETE FROM tolive.sec_role WHERE role_id = {role.Role_Id}",
                    MOO.Data.MNODatabase.DMART);


        }

        [Fact]
        public void TestRoleFunctions()
        {
            List<DAL.Models.Sec_Users> u;
            DAL.Models.Sec_Role role = DAL.Services.Sec_RoleSvc.Get(841);
            u = DAL.Services.Sec_UserSvc.GetUsersInRole(role);
            Assert.True(u.Count > 0);


            DAL.Models.Sec_Users usr = DAL.Services.Sec_UserSvc.Get("mno\\alt7747");


            DAL.Services.Sec_RoleSvc.AddUserToRole(usr, role, "ME");

            DAL.Services.Sec_RoleSvc.RemoveUserFromRole(usr, role);

        }

        [Fact]
        public void TestSec_AppCode()
        {
            DAL.Models.Sec_Application app = DAL.Services.Sec_ApplicationSvc.Get(842);
            Assert.NotNull(app);

            app = new()
            {
                Application_Name = "test",
                Application_Description = "test123",
                Application_Notes = "Notes",
                Created_By = "ME",
                Created_Date = DateTime.Now
            };

            DAL.Services.Sec_ApplicationSvc.Insert(app);
            app = DAL.Services.Sec_ApplicationSvc.Get(app.Application_Id);
            Assert.NotNull(app);
            //now delete the app, we don't have a delete function as we don't want to allow that in the app at this time
            MOO.Data.ExecuteNonQuery($"DELETE FROM tolive.sec_application WHERE application_id = {app.Application_Id}", MOO.Data.MNODatabase.DMART);

        }

        [Fact]
        public void TestUserOptionsFunctions()
        {
            Random r = new();

            string newOpts = "{\"test\":" + r.Next(999999).ToString() + "}";
            DAL.Services.Sec_UserSvc.UpdateAllUserOptions(4, newOpts);

            string opts = DAL.Services.Sec_UserSvc.GetAllUserOptions(4);
            Assert.Equal(newOpts, opts);


            newOpts = "{\"test\":" + r.Next(999999).ToString() + "}";
            DAL.Services.Sec_UserSvc.UpdateAllUserOptions("mno","alt7747", newOpts);

            opts = DAL.Services.Sec_UserSvc.GetAllUserOptions("mno", "alt7747");
            Assert.Equal(newOpts, opts);

            //test updating a setting that already exists
            DAL.Services.Sec_UserSvc.UpdateUserOption(4, "test", r.Next());
            MOO.Data.ExecuteNonQuery("UPDATE tolive.sec_users set USEROPTIONS = '' WHERE user_id = 4",MOO.Data.MNODatabase.DMART);


            //test updating a setting that doesn't exist yet
            DAL.Services.Sec_UserSvc.UpdateUserOption(4, "test", r.Next());

        }


    }
}
