using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public class AutoReportsTest : BaseTestClass
    {
        

        [Fact]
        public void TestRptItem()
        {
            var r = DAL.Services.Auto_Rpt_ItemSvc.GetSubscribed("mno\\alt7747");
            Assert.NotEmpty(r);


            DAL.Models.Auto_Rpt_Item itm = new()
            {
                Item_Description = "test",
                Item_Name = "test2",                
                Report_Type = DAL.Services.Auto_Rpt_TypeSvc.Get(1),
                Item_Value = "aaaaaaa"
            };

            DAL.Services.Auto_Rpt_ItemSvc.Insert(itm);
            var itmList = DAL.Services.Auto_Rpt_ItemSvc.GetAll();
            Assert.NotEmpty(itmList);
            itm.Item_Name = "test123";
            DAL.Services.Auto_Rpt_ItemSvc.Update(itm);

            DAL.Services.Auto_Rpt_ItemSvc.Delete(itm);
        }

        [Fact]
        public void TestRptParam()
        {
            var paramList = DAL.Services.Auto_Rpt_Run_Item_ParamSvc.GetAll(1);
            Assert.NotEmpty(paramList);
            DAL.Models.Auto_Rpt_Run_Item_Param newParam = new()
            {
                Item_Id = 1,
                Param_Name = "Testing123",
                Param_Value = "2"
            };

            DAL.Services.Auto_Rpt_Run_Item_ParamSvc.Insert(newParam);

            newParam.Dateformat = "MM/dd/yyyy";
            DAL.Services.Auto_Rpt_Run_Item_ParamSvc.Update(newParam);

            DAL.Services.Auto_Rpt_Run_Item_ParamSvc.Delete(newParam);
        }

        [Fact]
        public void TestRptRecipient()
        {
            var r = DAL.Services.Auto_Rpt_RcptSvc.Get(1452);
            Assert.NotNull(r);

            r = DAL.Services.Auto_Rpt_RcptSvc.Get(1, "MNO\\alt7747");
            Assert.NotNull(r);

            //DAL.Models.Auto_Rpt_Rcpt rcpt = new()
            //{
            //    Recipient = "mno\\test123",
            //    Item_Id = 1
            //};
            //DAL.Services.Auto_Rpt_RcptSvc.Insert(rcpt);

            //DAL.Services.Auto_Rpt_RcptSvc.Update(rcpt);
            //DAL.Services.Auto_Rpt_RcptSvc.Delete(rcpt);

        }
    }
}
