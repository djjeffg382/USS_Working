using MOO.DAL.ToLive.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public class SpeedLossTest
    {
        [Fact]
        public void TestSpeedLoss()
        {
            var sl = new Speed_Loss()
            {
                Plant = MOO.Plant.Minntac,
                Area = "Agglomerator",
                Line = "3-1",
                Start_Date = DateTime.Now,
                Avg_Ltph = 100,
                Loss_Tons = 50
            };
            DAL.Services.Speed_LossSvc.InsertAsync(sl).GetAwaiter().GetResult();

            sl = DAL.Services.Speed_LossSvc.GetAsync(sl.Speed_Loss_Id).GetAwaiter().GetResult();
            Assert.NotNull(sl);
            sl.End_Date = DateTime.Now;
            DAL.Services.Speed_LossSvc.UpdateAsync(sl).GetAwaiter().GetResult();

            var slList = DAL.Services.Speed_LossSvc.GetByArea(MOO.Plant.Minntac, "Agglomerator").GetAwaiter().GetResult();
            Assert.NotEmpty(slList);
            Assert.Equal(1, DAL.Services.Speed_LossSvc.DeleteAsync(sl).GetAwaiter().GetResult());
        }
    }
}
