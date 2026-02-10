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
    public class GCP_Test : BaseTestClass
    {
        [Fact]
        public void GCP_Deviation_Test()
        {
            var gcpList = GCP_DeviationsSvc.GetAll();
            Assert.NotEmpty(gcpList);
            var gcp = GCP_DeviationsSvc.Get(4, DateTime.Today);
            Assert.NotNull(gcp);
            gcp.Corrective_Action = DateTime.Now.ToLongTimeString();
            GCP_DeviationsSvc.Update(gcp);

            var insert = new GCP_Deviations() { 
                Line = 1,
                The_Date = DateTime.Today,
                Corrective_Action ="55", 
                Deviation_Type = "Test", 
                Num_Of_Bad_Reads = 420,   
                Num_Of_Good_Reads = 0,
                
            };
            GCP_DeviationsSvc.Insert(insert);
    }
    }
}
