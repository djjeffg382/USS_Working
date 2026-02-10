using MOO.DAL.ToLive.Models;
using MOO.DAL.ToLive.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DAL_UnitTest.Tolive
{
    public class Web_MenusTest : BaseTestClass
    {
        [Fact]
        public void Web_Menus_Test()
        {
            var menuList = Sec_Web_MenusSvc.GetAll();
            Assert.NotEmpty(menuList);
            var web = Sec_Web_MenusSvc.Get(4452);
            Assert.NotNull(web);
            Sec_Web_MenusSvc.Update(web);

            var swm = new Sec_Web_Menus()
            {
                Web_Menu_Id = 0,
                Description = "test",
                Modified_By = "Alex",
                Where_Used = "Crusher",
            };
            Sec_Web_MenusSvc.Insert(swm);
            swm.Description = "Candy";
            Sec_Web_MenusSvc.Update(swm);
            Sec_Web_MenusSvc.Delete(swm);
        }
    }
}
