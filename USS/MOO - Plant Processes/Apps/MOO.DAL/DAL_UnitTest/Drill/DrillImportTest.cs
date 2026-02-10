using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using MOO.DAL.Drill.Imports;
using DAL = MOO.DAL.Drill;

namespace DAL_UnitTest.Drill
{
    public class DrillImportTest: BaseTestClass
    {

        

        [Fact]
        public void TestImportDrilledHoleStatus()
        {
            //October 2018 was last data in dev system for TBird and Epiroc

            HoursImport imp = new(MOO.Plant.Minntac);
            imp.ImportDrilledHoleStatus(DateTime.Parse("02/12/2021"), 2);

        }


        [Fact]
        public void TestImportMtc()
        {
            
            MainImport imp = new(MOO.Plant.Minntac);
            imp.RunImport(DateTime.Parse("1/18/2025"), 1);

           
        }

        [Fact]
        public void TestImportKTC()
        {

            //Keetac Wenco is not on dev so we will read from prod

            MainImport imp = new(MOO.Plant.Keetac);

            imp.RunImport(DateTime.Parse("6/27/2022"), 2);
        }



        [Fact]
        public void TestEpirocRead()
        {
            //October 2018 was last data in dev system

            EpirocImport e = new(MOO.Plant.Keetac);

            List<DAL.Models.Raw_Drilled_Hole> rdh = e.GetRawData(DateTime.Parse("5/12/2023 06:00"), DateTime.Parse("5/12/2023 18:00"));
            Assert.NotEmpty(rdh);
        }



        [Fact]
        public void TestWencoRead()
        {
            //October 2018 was last data in dev system

            WencoImport w = new(MOO.Plant.Keetac);

            List<DAL.Models.Raw_Drilled_Hole> rdh = w.GetRawData(DateTime.Parse("01/15/2022 06:00"), DateTime.Parse("01/15/2022 18:00"));
            Assert.NotEmpty(rdh);
        }

        [Fact]
        public void TestHoursImport()
        {
            HoursImport hours = new(MOO.Plant.Minntac);
            hours.ImportDrilledHoleStatus(DateTime.Parse("4/11/2022"), 1);
        }



        [Fact]
        public void TestTerrainImport()
        {
            TerrainImport e = new(MOO.Plant.Minntac);

            List<DAL.Models.Raw_Drilled_Hole> rdh = e.GetRawData(DateTime.Parse("1/13/2025 18:00"), DateTime.Parse("1/14/2025 06:00"));
            Assert.NotEmpty(rdh);
        }
    }
}
