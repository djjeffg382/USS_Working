using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using MOO.DAL.ToLive.Services;
using MOO.DAL.ToLive.Models;

namespace DAL_UnitTest.Tolive
{
    public class PLO_Tests : BaseTestClass
    {
        [Fact]
        public void testPLOProduct()
        {
            var pdctList = PLO_ProductSvc.GetAll();
            Assert.NotEmpty(pdctList);
        }

        [Fact]
        public void testPLOPelletSpec()
        {
            var pdctList = PLO_ProductSvc.GetAll();
            Assert.NotEmpty(pdctList);

            PLO_Pellet_Spec newSpec = new()
            {
                Start_Date = DateTime.Now,
                Modified_By = "Me",
                Modified_Date = DateTime.Now,
                Product = pdctList[0]
            };
            PLO_Pellet_SpecSvc.Insert(newSpec);
            var lstSpecVals = PLO_Pellet_Spec_ValuesSvc.GetNewSpecs(newSpec);
            foreach (var val in lstSpecVals)
            {
                PLO_Pellet_Spec_ValuesSvc.Insert(val);
            }

            var lstSpec = PLO_Pellet_SpecSvc.GetAll();
            Assert.NotEmpty(lstSpec);

            var specVals = PLO_Pellet_Spec_ValuesSvc.GetSpecsByDate(DateTime.Now.AddMinutes(1), newSpec.Product.Product_Id);
            Assert.NotEmpty(specVals);

            specVals = PLO_Pellet_Spec_ValuesSvc.GetByPelletSpec(newSpec.Plo_Pellet_Spec_Id);
        }

    }
}
