using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;
namespace DAL_UnitTest.Tolive
{
    public class Met_TablesTest : BaseTestClass
    {

        [Fact]
        public void TestConcLine()
        {
            var monthTest = DAL.Services.Met_Conc_LineSvc.GetMonthSummary(DateTime.Parse("11/29/2021"), 2, 12);
            Assert.NotEmpty(monthTest);
            List<DAL.Models.Met_Conc_Line> a = DAL.Services.Met_Conc_LineSvc.Get(DateTime.Parse("1/1/2022"), DateTime.Parse("1/1/2022"), 2, 12);
            Assert.NotNull(a);
            Assert.NotEmpty(a);
            a[0].Schedhours = 23.9M;
            Assert.Equal(1, DAL.Services.Met_Conc_LineSvc.Update(a[0]));

        }


        [Fact]
        public void TestConcPlant2()
        {
            var monthTest = DAL.Services.Met_Conc_Plant2Svc.GetMonthSummary(DateTime.Parse("11/29/2021"));
            Assert.NotNull(monthTest);

            List<DAL.Models.Met_Conc_Plant2> a = DAL.Services.Met_Conc_Plant2Svc.Get(DateTime.Parse("1/1/2022"), DateTime.Parse("1/1/2022"));
            Assert.NotNull(a);
            Assert.NotEmpty(a);
            a[0].Unsched_Maint_Hours = 23.9M;
            Assert.Equal(1, DAL.Services.Met_Conc_Plant2Svc.Update(a[0]));

        }


        [Fact]
        public void TestConcPlant3()
        {
            var monthTest = DAL.Services.Met_Conc_Plant3Svc.GetMonthSummary(DateTime.Parse("11/29/2021"));
            Assert.NotNull(monthTest);

            List<DAL.Models.Met_Conc_Plant3> a = DAL.Services.Met_Conc_Plant3Svc.Get(DateTime.Parse("1/1/2022"), DateTime.Parse("1/1/2022"));
            Assert.NotNull(a);
            Assert.NotEmpty(a);
            a[0].Unsched_Maint_Hours = 23.9M;
            Assert.Equal(1, DAL.Services.Met_Conc_Plant3Svc.Update(a[0]));

        }





        [Fact]
        public void TestAggLine()
        {
            var monthTest = DAL.Services.Met_Agg_LineSvc.GetMonthSummary(DateTime.Parse("11/29/2021"), 2, 12, DAL.Models.Met_Material.Flux);
            Assert.NotEmpty(monthTest);
            monthTest = DAL.Services.Met_Agg_LineSvc.GetMonthSummary(DateTime.Parse("11/30/2021"), 2, 12, DAL.Models.Met_Material.Flux);
            Assert.NotEmpty(monthTest);
            List<DAL.Models.Met_Agg_Line> a = DAL.Services.Met_Agg_LineSvc.Get(DateTime.Parse("1/1/2022"), DateTime.Parse("1/1/2022"), 2, 12, DAL.Models.Met_Material.Flux);
            Assert.NotNull(a);
            Assert.NotEmpty(a);
            a[0].SchedHours = 23.9M;
            Assert.Equal(1, DAL.Services.Met_Agg_LineSvc.Update(a[0]));

        }




        [Fact]
        public void TestAggPlant2()
        {
            var monthTest = DAL.Services.Met_Agg_Plant2Svc.GetMonthSummary(DateTime.Parse("11/29/2021"), DAL.Models.Met_Material.Flux);
            Assert.NotNull(monthTest);

            List<DAL.Models.Met_Agg_Plant2> a = DAL.Services.Met_Agg_Plant2Svc.Get(DateTime.Parse("1/1/2022"), DateTime.Parse("1/1/2022"), DAL.Models.Met_Material.Flux);
            Assert.NotNull(a);
            Assert.NotEmpty(a);
            a[0].Unsched_Maint_Hours = 23.9M;
            Assert.Equal(1, DAL.Services.Met_Agg_Plant2Svc.Update(a[0]));

        }



        [Fact]
        public void TestAggPlant3()
        {
            var monthTest = DAL.Services.Met_Agg_Plant3Svc.GetMonthSummary(DateTime.Parse("11/29/2021"), DAL.Models.Met_Material.Flux);
            Assert.NotNull(monthTest);

            List<DAL.Models.Met_Agg_Plant3> a = DAL.Services.Met_Agg_Plant3Svc.Get(DateTime.Parse("1/1/2022"), DateTime.Parse("1/1/2022"), DAL.Models.Met_Material.Flux);
            Assert.NotNull(a);
            Assert.NotEmpty(a);
            a[0].Unsched_Maint_Hours = 23.9M;
            Assert.Equal(1, DAL.Services.Met_Agg_Plant3Svc.Update(a[0]));

        }



        [Fact]
        public void TestCrushPrimary()
        {
            var monthTest = DAL.Services.Met_Crush_PrimarySvc.GetMonthSummary(DateTime.Parse("11/29/2021"), 1, 4);
            Assert.NotNull(monthTest);

            List<DAL.Models.Met_Crush_Primary> a = DAL.Services.Met_Crush_PrimarySvc.Get(DateTime.Parse("1/1/2022"), DateTime.Parse("1/1/2022"));
            Assert.NotNull(a);
            Assert.NotEmpty(a);
            a[0].Avail_Hours = 23.9M;
            Assert.Equal(1, DAL.Services.Met_Crush_PrimarySvc.Update(a[0]));

        }


        [Fact]
        public void TestCrushSecondary()
        {
            var monthTest = DAL.Services.Met_Crush_SecondarySvc.GetMonthSummary(DateTime.Parse("11/29/2021"), 1, 15);
            Assert.NotNull(monthTest);

            List<DAL.Models.Met_Crush_Secondary> a = DAL.Services.Met_Crush_SecondarySvc.Get(DateTime.Parse("1/1/2022"), DateTime.Parse("1/1/2022"), 1, 15);
            Assert.NotNull(a);
            Assert.NotEmpty(a);
            a[0].Sched_Hours = 23.9M;
            Assert.Equal(1, DAL.Services.Met_Crush_SecondarySvc.Update(a[0]));

        }


        [Fact]
        public void TestCrushTertiary()
        {
            var monthTest = DAL.Services.Met_Crush_TertiarySvc.GetMonthSummary(DateTime.Parse("11/29/2021"), 1, 15);
            Assert.NotNull(monthTest);

            List<DAL.Models.Met_Crush_Tertiary> a = DAL.Services.Met_Crush_TertiarySvc.Get(DateTime.Parse("1/1/2022"), DateTime.Parse("1/1/2022"), 1, 15);
            Assert.NotNull(a);
            Assert.NotEmpty(a);
            a[0].Sched_Hours = 23.9M;
            Assert.Equal(1, DAL.Services.Met_Crush_TertiarySvc.Update(a[0]));

        }
        [Fact]
        public void TestCrushPlant()
        {

            var yearTest = DAL.Services.Met_Crush_PlantSvc.GetYearRecord(DateTime.Parse("11/30/2021"));
            Assert.NotNull(yearTest);

            var monthTest = DAL.Services.Met_Crush_PlantSvc.GetMonthSummary(DateTime.Parse("11/29/2021"));
            Assert.NotNull(monthTest);

            List<DAL.Models.Met_Crush_Plant> a = DAL.Services.Met_Crush_PlantSvc.Get(DateTime.Parse("1/1/2022"), DateTime.Parse("1/1/2022"));
            Assert.NotNull(a);
            Assert.NotEmpty(a);
            a[0].Omh_Tons = 23.9M;
            Assert.Equal(1, DAL.Services.Met_Crush_PlantSvc.Update(a[0]));

        }



        [Fact]
        public void TestAggStandards()
        {
            DAL.Models.Met_Agg_Standards a = DAL.Services.Met_Agg_StandardsSvc.Get(DateTime.Parse("11/14/2021"));
            Assert.NotNull(a);
            a.New_Ltd_Reduce_Standard = 1;
            Assert.Equal(1, DAL.Services.Met_Agg_StandardsSvc.Update(a));

        }


        [Fact]
        public void TestAggInvCalc()
        {
            var z = DAL.Services.Met_Agg_Plant3Svc.GetMonthSummary(DateTime.Parse("10/27/2024"), DAL.Models.Met_Material.Flux);
            var y = z.Inventory;


            var a = DAL.Services.Met_Agg_Plant2Svc.GetMonthSummary(DateTime.Parse("1/31/2021"), DAL.Models.Met_Material.Flux);
            Assert.Equal(8432M, Math.Round(a.Inventory, 0, MidpointRounding.AwayFromZero));

            var b = DAL.Services.Met_Agg_Plant3Svc.GetMonthSummary(DateTime.Parse("1/31/2021"), DAL.Models.Met_Material.Flux);
            Assert.Equal(127343M, Math.Round(b.Inventory, 0, MidpointRounding.AwayFromZero));

        }


        [Fact]
        public void MaterialFactor()
        {
            string cmdText = "tolive.met_roll.material_factor";
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            OracleCommand cmd = new(cmdText, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.BindByName = true;
            cmd.Parameters.Add("roll_date", DateTime.Parse("1/1/2021"));
            cmd.Parameters.Add("mat", 1);
            cmd.Parameters.Add("step", 1);
            cmd.Parameters.Add("m_y", "m");
            cmd.Parameters.Add(new OracleParameter("retVal", OracleDbType.Decimal));
            //cmd.Parameters["retVal"].DbType = DbType.Decimal;
            cmd.Parameters["retVal"].Direction = ParameterDirection.ReturnValue;
            conn.Open();
            cmd.ExecuteNonQuery();

            object dbVal = cmd.Parameters["retVal"].Value;
            //dbVal should now be of type Oracle.ManagedDataAccess.Types.OracleDecimal
            //this has too many decimal points and results in Arithmetic overflow in .Net
            //Therefore we will convert to string and then just take the first 10 characters and then convert to decimal
            decimal retVal = decimal.Parse(dbVal.ToString()[..10]);
        }


        [Fact]
        public void MetFactorTests()
        {
            var mf = DAL.Services.Met_FactorSvc.GetAll();
            Assert.NotEmpty(mf);

            var mfh = DAL.Services.Met_Factor_HistorySvc.GetAll(3);  //wood mbtu per short ton
            Assert.NotEmpty(mfh);

            DAL.Models.Met_Factor_History mfhNew = new()
            {
                Met_Factor = mf[0],
                Effective_Date = DateTime.Today,
                Factor_Value = -1.1111M
            };

            DAL.Services.Met_Factor_HistorySvc.Insert(mfhNew);
            //check the record was inserted by calling get
            mfhNew = DAL.Services.Met_Factor_HistorySvc.Get(mfhNew.Met_Factor.Factor_Id, mfhNew.Effective_Date);
            Assert.NotNull(mfhNew);
            mfhNew.Factor_Value = 123M;
            Assert.Equal(1, DAL.Services.Met_Factor_HistorySvc.Update(mfhNew));
            //now delete the record
            Assert.Equal(1, DAL.Services.Met_Factor_HistorySvc.Delete(mfhNew));

        }

        [Fact]
        public void PelletTypeTests()
        {
            var pt = DAL.Services.Pellet_TypeSvc.GetAll();
            Assert.NotEmpty(pt);

            var pth = DAL.Services.Pellet_Type_HistorySvc.GetAll(2);  //get all step 2
            Assert.NotEmpty(pth);

            DAL.Models.Pellet_Type_History pthNew = new()
            {
                Pel_Type= pt[0],
                Ag_Step = 2,
                Start_Date = DateTime.Today
            };

            DAL.Services.Pellet_Type_HistorySvc.Insert(pthNew);
            //check the record was inserted by calling get
            pthNew = DAL.Services.Pellet_Type_HistorySvc.Get(pthNew.Ag_Step, pthNew.Start_Date);
            Assert.NotNull(pthNew);
            pthNew.Pel_Type = pt[1];
            //now delete the record
            Assert.Equal(1, DAL.Services.Pellet_Type_HistorySvc.Delete(pthNew));

            var pthLatest = DAL.Services.Pellet_Type_HistorySvc.GetLatest(2, DateTime.Today);
            Assert.NotNull(pthLatest);

        }

        [Fact]
        public void TestFeedToGrate()
        {
            DateTime testDate = DateTime.Parse("11/30/2021");
            var f = DAL.Services.Met_Feed_To_GrateSvc.Calculate(testDate, 2, DAL.Models.Met_Material.Flux);
            Assert.NotNull(f);

            int recsAffected = DAL.Services.Met_Feed_To_GrateSvc.Update(f);
            Assert.NotEqual(0, recsAffected);

            f = DAL.Services.Met_Feed_To_GrateSvc.Get(testDate, 2, DAL.Models.Met_Material.Flux);
            Assert.NotNull(f);

            //delete and then reinsert
            MOO.Data.ExecuteNonQuery($"DELETE FROM tolive.met_feed_to_grate WHERE datex = {MOO.Dates.OraDate(testDate, true)}" +
                                    " AND step = 2 AND Material = 1 AND DMY = 2", MOO.Data.MNODatabase.DMART);
            DAL.Services.Met_Feed_To_GrateSvc.Insert(f);
            f = DAL.Services.Met_Feed_To_GrateSvc.Get(testDate, 2, DAL.Models.Met_Material.Flux);
            Assert.NotNull(f);

            var fList = DAL.Services.Met_Feed_To_GrateSvc.GetAll(DateTime.Parse("1/1/2020"), DateTime.Today);
            Assert.NotEmpty(fList);

        }

        [Fact]
        public void TestGetMetConcTotalPlant()
        {
            var vals = DAL.Services.Met_Conc_Total_PlantSvc.GetByDateRange(DateTime.Parse("1/1/2020"), DateTime.Parse("1/20/2020"));
            Assert.NotEmpty(vals);
        }
    }
}
