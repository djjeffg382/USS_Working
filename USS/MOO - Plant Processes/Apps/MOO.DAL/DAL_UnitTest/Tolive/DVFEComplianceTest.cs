using MOO.DAL.ToLive.Models;
using MOO.DAL.ToLive.Services;
using System;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public class DVFEComplianceTest : BaseTestClass
    {
        [Fact]
        public void ComplianceTest()
        {
            DateTime startDate = DateTime.Now.AddYears(-3);
            DateTime endDate = DateTime.Now;

            var compliances = DVFE_ComplianceSvc.GetAll(startDate, endDate);
            Assert.NotEmpty(compliances);

            var compliance = DVFE_ComplianceSvc.Get(17940);
            Assert.NotNull(compliance);

            DVFE_Compliance dVFE_Compliance = new()
            {
                RegisterDate = DateTime.Now,
                Facility_Id = DVFE_Compliance.Facility.Minntac,
                Plant_Id = DVFE_Compliance.Plant.Agglomerator3,
                WebLoginName = "TEST",
                NTUserName = "Test",
                ChangedFromIp = "0.0.0.0",
                RegisterDateFull = DateTime.Now,
            };
            Assert.Equal(1,DVFE_ComplianceSvc.Insert(dVFE_Compliance));
        }

    }
}


