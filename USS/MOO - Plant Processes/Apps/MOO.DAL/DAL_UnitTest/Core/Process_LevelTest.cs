using Microsoft.Data.SqlClient;
using MOO.DAL.Core.Enums;
using MOO.DAL.Core.Models;
using MOO.DAL.Core.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.Core;

namespace DAL_UnitTest.Core
{
    public class Process_LevelTest : BaseTestClass
    {
        [Fact]
        public void Process_Level_Test()
        {
            var getAll = Process_LevelSvc.GetAll();
            Assert.NotEmpty(getAll);
            var getId = Process_LevelSvc.GetAllWithChildren();
            Assert.NotNull(getId);

            //var pl = new Process_Level()
            //{
            //    Process_Level_Id = 242,
            //    Process_Level_Name = "test",
            //    Abbreviation = "test",
            //    Passport_Id = "test",
            //    Parent_Id = 23,
            //    Process_Type = 0
            //};
            //Process_LevelSvc.Insert(pl);
            //pl.Process_Level_Name = "Test123";
            //Process_LevelSvc.Update(pl);
            //Process_LevelSvc.Delete(pl);
        }
    }
}
