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
    public class CAD_DrawingTest : BaseTestClass
    {
        [Fact]
        public void DrawingTest()
        {
            var areas = DAL.Services.CAD_DrawingsSvc.GetDistinctAreas();

            var tt = DAL.Services.CAD_DrawingsSvc.GetAll(MOO.Plant.Minntac, "", "C-203");
            var workList = DAL.Services.CAD_DrawingsSvc.GetAll(MOO.Plant.Minntac, "partial floor", "C8350", "R296", "agg",
                            "296", "", "");
            Assert.NotEmpty(workList);
        }

        [Fact]
        public void DrawingPagedDataTest()
        {
            var pd = DAL.Services.CAD_DrawingsSvc.GetPagedData(1, 25, MOO.Plant.Minntac, "Agglo","","","","","","","","Area", "asc");
            Assert.NotNull(pd);
            Assert.NotEmpty(pd.Data);
        }

        [Fact]
        public void TestDistinct()
        {
            var a = DAL.Services.CAD_DrawingsSvc.GetDistinctAreas();
            Assert.NotEmpty(a);

            a = DAL.Services.CAD_DrawingsSvc.GetDistinctPlantLocations();
            Assert.NotEmpty(a);

            a = DAL.Services.CAD_DrawingsSvc.GetDistinctPrintLocations();
            Assert.NotEmpty(a);

            a = DAL.Services.CAD_DrawingsSvc.GetDistinctEquipNumbers();
            Assert.NotEmpty(a);


        }
    }
   
}
