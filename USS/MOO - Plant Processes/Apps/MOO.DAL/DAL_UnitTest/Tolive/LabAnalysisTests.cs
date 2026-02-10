using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public class LabAnalysisTests
    {
        [Fact]
        public void LcaGetBySMID()
        {
            var lca = DAL.Services.Lab_Chem_AnalysisSvc.GetBySampleMgrId(521392);
            Assert.NotNull(lca);
        }
        [Fact]
        public void TestLabChemAnalysis()
        {
            var lct = DAL.Services.Lab_Chem_TypeSvc.Get(1);

            DAL.Models.Lab_Chem_Analysis lca = new()
            {
                Lab_Chem_Type = lct,
                Analysis_Date = DateTime.Now,
                Shift_Date12 = DateTime.Today,
                Shift_Date8 = DateTime.Today,
                Shift_Nbr12 = 2,
                Shift_Nbr8 = 1,
                Fe = 1.11
                
            };
            Assert.Equal(1, DAL.Services.Lab_Chem_AnalysisSvc.Insert(lca));


            var lst = DAL.Services.Lab_Chem_AnalysisSvc.GetByShiftDate(1, DateTime.Today, DateTime.Today, DAL.Enums.ShiftType.ShiftType12Hour);
            Assert.NotEmpty(lst);
            lca.Al2O3 = 11;
            Assert.Equal(1, DAL.Services.Lab_Chem_AnalysisSvc.Update(lca));
            Assert.Equal(1, DAL.Services.Lab_Chem_AnalysisSvc.Delete(lca)); 

        }

        [Fact]
        public void TestLabPhysAnalysis()
        {
            var lpt = DAL.Services.Lab_Phys_TypeSvc.Get(1);
            Assert.NotNull(lpt);

            DAL.Models.Lab_Phys_Analysis lpa = new()
            {
                Lab_Phys_Type = lpt,
                Analysis_Date = DateTime.Now,
                Shift_Date12 = DateTime.Today,
                Shift_Date8 = DateTime.Today,
                Shift_Nbr12 = 1,
                Shift_Nbr8 = 3,
                Start_Wgt = 51.3,
                Inch_5_8_Wgt = 12.4,
                Inch_9_16_Wgt = 20.2,
                Inch_1_4_Wgt = 27.9,
                Mesh_3_Wgt = 40.2,
                Mesh_8_Wgt = 45.6,
                Mesh_20_Wgt = 51.3
            };
            Assert.Equal(1, DAL.Services.Lab_Phys_AnalysisSvc.Insert(lpa));

            var lsct = DAL.Services.Lab_Phys_AnalysisSvc.GetByShiftDate(1, DateTime.Today.AddDays(-4), DateTime.Today.AddDays(-4), DAL.Enums.ShiftType.ShiftType12Hour, 1);
            Assert.NotEmpty(lsct);
            lpa.Inch_7_16_Wgt = (double)25.5;
            Assert.Equal(1, DAL.Services.Lab_Phys_AnalysisSvc.Update(lpa));
            Assert.Equal(1, DAL.Services.Lab_Phys_AnalysisSvc.Delete(lpa));
        }
    }
}
