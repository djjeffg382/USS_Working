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
    public class Mine_ProductionTest : BaseTestClass
    {
        [Fact]
        public void Mine_Production_Test()
        {
            List<Mine_Production> list = Mine_ProductionSvc.GetBetweenDates(DateTime.Today.AddYears(-2).AddDays(-5), DateTime.Today.AddYears(-2));
            Console.WriteLine(list);

            //var piPoints = Mine_ProductionSvc.GetCrusherPiPointTotalsPerBelt(DateTime.Today.AddHours(-6), DateTime.Today.AddHours(6), Mine_Production.CrusherBelts.Belt_01);
            //Assert.NotNull(piPoints);
            //var mine = Mine_ProductionSvc.Get(DateTime.Parse("05/24/2021"), 2);
            //Assert.NotNull(mine);
            //Mine_ProductionSvc.Update(mine);

            //var mineP = new Mine_Production()
            //{
            //    Shift_Date = DateTime.Today,
            //    Shift = 2,
            //    Production_Tons = 94357,
            //    Modified_Date = DateTime.Today,
            //    Modified_By = "Test"
            //};
            //Mine_ProductionSvc.Insert(mineP);
        }
    }
}
