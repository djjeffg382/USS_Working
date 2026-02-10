using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public class LIMSTest : BaseTestClass
    {
        [Fact]
        public void LimsBatchTest()
        {
            var lbList = DAL.Services.LIMS_BatchSvc.GetAll();
            Assert.NotEmpty(lbList);
            lbList = DAL.Services.LIMS_BatchSvc.GetAll(DAL.Enums.LIMS_Batch_Type.Other);
            Assert.NotEmpty(lbList);


            DAL.Models.LIMS_Batch b = new()
            {
                Batch_Name = "Test12345",
                Batch_Type = DAL.Enums.LIMS_Batch_Type.Pellet,
                Created_Date = DateTime.Now
            };
            DAL.Services.LIMS_BatchSvc.Insert(b);
            b = DAL.Services.LIMS_BatchSvc.Get(b.Batch_Id);
            Assert.NotNull(b);

            b.Last_Instrument_Export = DateTime.Now;
            //create some samples for this batch
            DAL.Models.LIMS_Batch_Samples s = new()
            {
                Batch_Id = b.Batch_Id,
                Batch_Seq = 1,
                Sample_Number = 295037
            };
            DAL.Services.LIMS_Batch_SamplesSvc.Insert(s);
            s = new()
            {
                Batch_Id = b.Batch_Id,
                Batch_Seq = 1,
                Sample_Number = 277745
            };
            DAL.Services.LIMS_Batch_SamplesSvc.Insert(s);

            var sList = DAL.Services.LIMS_Batch_SamplesSvc.Get(b.Batch_Id);
            Assert.NotEmpty(sList);
            

            DAL.Services.LIMS_BatchSvc.Update(b);

            //delete the children
            DAL.Services.LIMS_BatchSvc.DeleteChildren(b);
            DAL.Services.LIMS_BatchSvc.Delete(b);
        }
    }
}
