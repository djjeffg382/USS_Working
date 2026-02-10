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

  

    public class LabAnalysisTables : BaseTestClass
    {
        
        [Fact]
        public void TestFilterCakeAnalysis()
        {
            var fca = DAL.Services.Filter_Cake_AnalysisSvc.Get(DateTime.Parse("1/1/2020"), 2, 1);
            Assert.NotNull(fca);

            var fcaList = DAL.Services.Filter_Cake_AnalysisSvc.GetByDateRange(DateTime.Parse("1/1/2020"), DateTime.Parse("1/10/2020"));
            Assert.NotEmpty(fcaList);
            //test inserting
            fca.Sdate = DateTime.Today;
            Assert.Equal(1, DAL.Services.Filter_Cake_AnalysisSvc.Insert(fca));
            fca.Al = 11;
            Assert.Equal(1, DAL.Services.Filter_Cake_AnalysisSvc.Update(fca));


            Assert.Equal(1, DAL.Services.Filter_Cake_AnalysisSvc.UpdateComponent(fca.Sdate, fca.Step, fca.Intv,DAL.Models.Filter_Cake_Analysis.FilterCakeAnalysisComponent.Si,1.23M));

            Assert.Equal(1,DAL.Services.Filter_Cake_AnalysisSvc.Delete(fca));
        }

        [Fact]
        public void TestFilterCakeReclaimAnalysis()
        {
            var fca = DAL.Services.Filter_Cake_Recl_AnalysisSvc.Get(DateTime.Parse("1/1/2020"), 3, 1);
            Assert.NotNull(fca);

            var fcaList = DAL.Services.Filter_Cake_Recl_AnalysisSvc.GetByDateRange(DateTime.Parse("1/1/2020"), DateTime.Parse("1/10/2020"));
            Assert.NotEmpty(fcaList);
            //test inserting
            fca.Sdate = DateTime.Today;
            Assert.Equal(1, DAL.Services.Filter_Cake_Recl_AnalysisSvc.Insert(fca));
            fca.Al = 11;
            Assert.Equal(1, DAL.Services.Filter_Cake_Recl_AnalysisSvc.Update(fca));


            Assert.Equal(1, DAL.Services.Filter_Cake_Recl_AnalysisSvc.UpdateComponent(fca.Sdate, fca.Step, fca.Intv, DAL.Models.Filter_Cake_Recl_Analysis.FilterCakeReclaimComponent.Si, 1.23M));

            Assert.Equal(1, DAL.Services.Filter_Cake_Recl_AnalysisSvc.Delete(fca));
        }

        [Fact]
        public void TestFloatAnalysis()
        {
            var fa = DAL.Services.Float_AnalysisSvc.Get(DateTime.Parse("1/1/2020"), 2, DAL.Models.Float_Analysis.FloatAnalysisType.FC);
            Assert.NotNull(fa);

            var faList = DAL.Services.Float_AnalysisSvc.GetByDateRange(DateTime.Parse("1/1/2020"), DateTime.Parse("1/10/2020"));
            Assert.NotEmpty(faList);
            //test inserting
            fa.Datex = DateTime.Today;
            Assert.Equal(1, DAL.Services.Float_AnalysisSvc.Insert(fa));
            fa.Al = 11;
            Assert.Equal(1, DAL.Services.Float_AnalysisSvc.Update(fa));


            Assert.Equal(1, DAL.Services.Float_AnalysisSvc.UpdateComponent(fa.Datex, fa.Shift, fa.SType, DAL.Models.Float_Analysis.FloatAnalysisComponent.Si, 1.23M));

            Assert.Equal(1, DAL.Services.Float_AnalysisSvc.Delete(fa));
        }


        [Fact]
        public void TestColFlotAnalysis()
        {
            var cf = DAL.Services.Col_Flot_AnalysisSvc.Get(DateTime.Parse("1/1/2020"), 2,DAL.Models.Col_Flot_Analysis.SampleType.Froth);
            Assert.NotNull(cf);

            var fcList = DAL.Services.Col_Flot_AnalysisSvc.GetByDateRange(DateTime.Parse("1/1/2020"), DateTime.Parse("1/10/2020"));
            Assert.NotEmpty(fcList);
            //test inserting
            cf.Datex = DateTime.Today;
            Assert.Equal(1, DAL.Services.Col_Flot_AnalysisSvc.Insert(cf));
            cf.Si = 11;
            Assert.Equal(1, DAL.Services.Col_Flot_AnalysisSvc.Update(cf));


            Assert.Equal(1, DAL.Services.Col_Flot_AnalysisSvc.Delete(cf));
        }


        [Fact]
        public void TestPelletAnalysis()
        {
            var pa = DAL.Services.Pellet_AnalysisSvc.Get(DateTime.Parse("1/1/2020"), 4,2);
            Assert.NotNull(pa);

            var paList = DAL.Services.Pellet_AnalysisSvc.GetByDateRange(DateTime.Parse("1/1/2020"), DateTime.Parse("1/10/2020"));
            Assert.NotEmpty(paList);
            //test inserting
            pa.Sdate = DateTime.Today;
            Assert.Equal(1, DAL.Services.Pellet_AnalysisSvc.Insert(pa));
            pa.Si = 11;
            Assert.Equal(1, DAL.Services.Pellet_AnalysisSvc.Update(pa));


            Assert.Equal(1, DAL.Services.Pellet_AnalysisSvc.UpdateComponent(pa.Sdate, pa.Line, pa.Shift, Pellet_Analysis.PelletAnalysisComponent.Si, 1.23M));
            Assert.Equal(1, DAL.Services.Pellet_AnalysisSvc.Delete(pa));
        }


        [Fact]
        public void TestRMFDTAnalysis()
        {
            var rmf = DAL.Services.RMF_DT_AnalysisSvc.Get(DateTime.Parse("1/1/2020"), 2);
            Assert.NotNull(rmf);

            var rmfList = DAL.Services.RMF_DT_AnalysisSvc.GetByDateRange(DateTime.Parse("1/1/2020"), DateTime.Parse("1/10/2020"));
            Assert.NotEmpty(rmfList);
            //test inserting
            rmf.Datex = DateTime.Today;
            Assert.Equal(1, DAL.Services.RMF_DT_AnalysisSvc.Insert(rmf));
            rmf.Si = 11;
            Assert.Equal(1, DAL.Services.RMF_DT_AnalysisSvc.Update(rmf));


            Assert.Equal(1, DAL.Services.RMF_DT_AnalysisSvc.Delete(rmf));
        }



        [Fact]
        public void TestLabReduce()
        {
            var lr = DAL.Services.Lab_Reducibility_AnalysisSvc.Get(DateTime.Parse("1/12/2020"), 3);
            Assert.NotNull(lr);

            var lrList = DAL.Services.Lab_Reducibility_AnalysisSvc.GetByDateRange(DateTime.Parse("1/12/2020"), DateTime.Parse("1/31/2020"));
            Assert.NotEmpty(lrList);
            //test inserting
            lr.Sdate = DateTime.Today;
            Assert.Equal(1, DAL.Services.Lab_Reducibility_AnalysisSvc.Insert(lr));
            lr.Al = 11;
            Assert.Equal(1, DAL.Services.Lab_Reducibility_AnalysisSvc.Update(lr));


            Assert.Equal(1, DAL.Services.Lab_Reducibility_AnalysisSvc.UpdateComponent(lr.Sdate, lr.Line, DAL.Models.Lab_Reducibility_Analysis.ReducibilityComponent.Si, 1.23M));

            Assert.Equal(1, DAL.Services.Lab_Reducibility_AnalysisSvc.Delete(lr));
        }


        [Fact]
        public void TestFFDT()
        {
            var ff = DAL.Services.Flot_Feed_DT_AnalysisSvc.Get(DateTime.Parse("1/1/2020"), 2);
            Assert.NotNull(ff);

            var ffList = DAL.Services.Flot_Feed_DT_AnalysisSvc.GetByDateRange(DateTime.Parse("1/1/2020"), DateTime.Parse("1/10/2020"));
            Assert.NotEmpty(ffList);
            //test inserting
            ff.Datex = DateTime.Today;
            Assert.Equal(1, DAL.Services.Flot_Feed_DT_AnalysisSvc.Insert(ff));
            ff.Si = 11;
            Assert.Equal(1, DAL.Services.Flot_Feed_DT_AnalysisSvc.Update(ff));


            Assert.Equal(1, DAL.Services.Flot_Feed_DT_AnalysisSvc.Delete(ff));
        }
    }
}
