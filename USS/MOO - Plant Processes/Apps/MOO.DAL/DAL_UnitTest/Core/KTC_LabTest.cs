using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.Core;
namespace DAL_UnitTest.Core
{
    public class KTC_LabTest : BaseTestClass
    {
        [Fact]
        public void Test1()
        {
            var lab = DAL.Services.KTCLab.KTC_LabSvc.Get(DateTime.Parse("1/2/2019"));
            Assert.NotNull(lab);

            lab.ConcScreenStartWeight = 123;
            //DAL.Services.KTCLab.KTC_LabSvc.Save(lab);
            DAL.Services.ApprovalSvc.RemoveApproval(lab.Approval.Approval_Id);
            //DAL.Services.KTCLab.KTC_LabSvc.Approve(lab,"test",lab.Start_Date_No_DST);
        }

    }
}
