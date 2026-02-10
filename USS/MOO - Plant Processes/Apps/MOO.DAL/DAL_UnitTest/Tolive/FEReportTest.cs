using MOO.DAL.ToLive.Models;
using MOO.DAL.ToLive.Services;
using MOO.DAL.ToLive.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DAL_UnitTest.Tolive
{
    public class FEReportTest : BaseTestClass
    {
        [Fact] public void ReportTest()
        {
            var report = FE_ReportSvc.Get(2);
            Assert.NotNull(report);

            var reports = FE_ReportSvc.GetAll();
            Assert.NotEmpty(reports);


            var testUser = Sec_UserSvc.Get(20220);

            FE_Report newReport = new()
            {
                Fe_Type = FE_Type.FE_KTC_Tails_Basin,
                Record_Date = DateTime.Today,
                Sec_User_Entered_By = testUser,
                Temperature = 73.23,
                Wind_Direction = "NNW",
                Wind_Speed = 14.32,
                Observed_Weather = "Cloudy",
                Entered_Date = DateTime.Now,
            };

            Assert.Equal(1, FE_ReportSvc.Insert(newReport));

            reports = FE_ReportSvc.GetByDateRange(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1));
            Assert.NotEmpty(reports);

            newReport.Comments = "test";
            Assert.Equal(1, FE_ReportSvc.Update(newReport));
            Assert.Equal(1, FE_ReportSvc.Delete(newReport));
        }

        [Fact] public void KTCTailsBasinTest()
        {
            var detail = FE_K_Tails_BasinSvc.GetByReportId(8);
            Assert.NotNull(detail);

            var details = FE_K_Tails_BasinSvc.GetByReportId(2);
            Assert.NotNull(details);

            FE_K_Tails_Basin newDetail = new()
            {
                Fe_Report_Id = 2,
                Observation_Normal = false,
                Req_Attention = false,
                Semi_Truck_Hauling = true,
                Road_Req_Attention = true,
            };

            Assert.Equal(1, FE_K_Tails_BasinSvc.Insert(newDetail));
            newDetail.Req_Attention = true;
            Assert.Equal(1, FE_K_Tails_BasinSvc.Update(newDetail));
            Assert.Equal(1, FE_K_Tails_BasinSvc.Delete(newDetail));
        }

        [Fact] public void KTCMineTest()
        {
            var detail = FE_K_MineSvc.Get(15);
            Assert.NotNull(detail);

            var details = FE_K_MineSvc.GetAllByReportId(3);
            Assert.NotEmpty(details);

            FE_K_Mine newDetail = new()
            {
                Fe_Report_Id = 3,
                Source = "source",
                Equip = "eqiup1",
                Equip_Operating = false,
                Steam_Plume = Steam_Plume.Attached,
                Req_Attention = true,
                Emission_Code = "code",
                Sort_Order = -1,
            };

            Assert.Equal(1, FE_K_MineSvc.Insert(newDetail));
            newDetail.Comments = "hi";
            Assert.Equal(1, FE_K_MineSvc.Update(newDetail));
            Assert.Equal(1, FE_K_MineSvc.Delete(newDetail));
        }
    }
}
