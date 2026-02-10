using Microsoft.Data.SqlClient;
using MOO.DAL.ToLive.Models;
using MOO.DAL.ToLive.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public class Web_Menu_LinkTest : BaseTestClass
    {
        [Fact]
        public void Menu_Link_Test()
        {
            var menuList = Sec_Web_Menu_LinksSvc.GetByWebMenuId(5006);
            Assert.NotEmpty(menuList);
            var web = Sec_Web_Menu_LinksSvc.Get(1202);
            Assert.NotNull(web);
            Sec_Web_Menu_LinksSvc.Update(web);

            var swml = new Sec_Web_Menu_Links()
            {
                Web_Menu_Id = 3071,
                Parent_Id = 8781,
                Display_Text = "Test2",
                Sort_Order = 0,
                Url = "apples",
                Open_New_Window = false,
                Roles = "apples",
                No_Access_Show = false,
                Tooltip = "apples",
                Is_Iis_Id_App = false,
                Active = false,
                Sy_Program_Code = "apples",
                Image_Url = "apples",
                Recurse_Folder = true,
                Menu_Type = 0,
                Modified_By = "apples",
                Include_In_Global_Menu = false
            };
            Sec_Web_Menu_LinksSvc.Insert(swml);
            swml.Url = "Candy";
            Sec_Web_Menu_LinksSvc.Update(swml);
            Sec_Web_Menu_LinksSvc.Delete(swml);
        }
    }
}
