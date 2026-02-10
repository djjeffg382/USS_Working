using System;
using Xunit;
using System.Data.Common;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Runtime.ExceptionServices;
using System.Collections.Generic;
using MOO;

namespace MOO_UnitTest
{
    public class DataTest
    {


        [Fact]
        public void TestDBConnections()
        {
            Util.RegisterFactories();
            string dbConn = MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART);
            Assert.NotNull(dbConn);
            //Connect to each DB and run a simple query
            //exception will occur if there is a problem

            MOO.Data.ExecuteQuery("SELECT getdate()", MOO.Data.MNODatabase.MineStar);

            MOO.Data.ExecuteQuery("SELECT getdate()", MOO.Data.MNODatabase.MtcWencoReport);

            MOO.Data.ExecuteQuery("SELECT getdate()", MOO.Data.MNODatabase.LIMS_Read);
            MOO.Data.ExecuteQuery("SELECT getdate()", MOO.Data.MNODatabase.LIMS_Report);

            MOO.Data.ExecuteQuery("SELECT * FROM [piarchive]..[pisnapshot] WHERE tag = 'sinusoid'", MOO.Data.MNODatabase.MTC_Pi);
            MOO.Data.ExecuteQuery("SELECT * FROM [piarchive]..[pisnapshot] WHERE tag = 'sinusoid'", MOO.Data.MNODatabase.MTC_PI_UTC);
            //MOO.Data.ExecuteQuery("SELECT * FROM [Configuration].[Asset].[Element] WHERE DBReferenceTypeID IS NOT NULL", MOO.Data.MNODatabase.MTC_PI_AF);

            MOO.Data.ExecuteQuery("SELECT SYSDATE FROM DUAL", MOO.Data.MNODatabase.DMART);
            MOO.Data.ExecuteQuery("SELECT SYSDATE FROM DUAL", MOO.Data.MNODatabase.CorpDelays);
            MOO.Data.ExecuteQuery("SELECT getdate()", MOO.Data.MNODatabase.KTC_Minvu);
            MOO.Data.ExecuteQuery("SELECT getdate()", MOO.Data.MNODatabase.MTC_MinVu);

            MOO.Data.ExecuteQuery("SELECT getdate()", MOO.Data.MNODatabase.KTC_Wenco);
            MOO.Data.ExecuteQuery("SELECT getdate()", MOO.Data.MNODatabase.MTC_Wenco);

            MOO.Data.ExecuteQuery("SELECT getdate()", MOO.Data.MNODatabase.USSDrillData);

            //MOO.Data.ExecuteQuery("SELECT getdate()", MOO.Data.MNODatabase.KtcMineReport);
            //MOO.Data.ExecuteQuery("SELECT getdate()", MOO.Data.MNODatabase.MtcMineReport);

        }

        [Fact]
        public void TestFillDataTable()
        {
            Util.RegisterFactories();
            //we will test this using the Oracle connection
            string sql = "SELECT SYSDATE FROM DUAL";
            OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));

            OracleDataAdapter da = new(sql,conn);
            DataSet ds = new();
            DataTable dt = new();
            ds.Tables.Add(dt);
            dt.Columns.Add("SYSDATE", typeof(DateTime));
            MOO.Data.FillDataTable(da, dt);
            
        }

        [Fact]
        public void TestExecuteScalar()
        {
            Util.RegisterFactories();
            var dt = MOO.Data.ExecuteScalar("SELECT SYSDATE FROM DUAL", MOO.Data.MNODatabase.DMART);
            Assert.IsType<DateTime>(dt);
            //this query should return no records, test the valueifnorecord parameter
            var dt2 = MOO.Data.ExecuteScalar("select * from dual WHERE dummy is null", MOO.Data.MNODatabase.DMART,DateTime.Now);
            Assert.IsType<DateTime>(dt2);
        }
        [Fact]
        public void TestExecuteNonQuery()
        {
            Util.RegisterFactories();
            int RecsChanged;
            string sql = "UPDATE tolive.key_values SET key_value = 'aa' WHERE key_name = 'DUMMY'";
            RecsChanged = MOO.Data.ExecuteNonQuery(sql, MOO.Data.MNODatabase.DMART);
            Assert.Equal(0, RecsChanged);


            
            OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            OracleCommand Cmd = new(sql, conn);
            try
            {
                conn.Open();
                RecsChanged = MOO.Data.ExecuteNonQuery(Cmd);
                Assert.Equal(0, RecsChanged);
            }
            catch(Exception ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
            finally
            {
                conn.Close();
            }

        }


        [Fact]
        public void TestGetNextSequence()
        {
            Util.RegisterFactories();



            decimal seq;
            seq = MOO.Data.GetNextSequence("MOO_Rpt.de.seqLost_Hardware", MOO.Data.MNODatabase.MtcWencoReport);

            seq = MOO.Data.GetNextSequence("tolive.seq_generic");
            Assert.True(seq > 0);

            seq = MOO.Data.GetNextSequence("Drill.dbo.seq_Drill", MOO.Data.MNODatabase.USSDrillData);
            Assert.True(seq > 0);
        }


        [Fact]
        public void TestDBKeys()
        {
            Util.RegisterFactories();
            string DbKeyName = "MOO_Lib_Testing";
            string DbKeyVal = "test123";
            string DbKeyTest;
            //make sure the key does not exist first
            MOO.Data.ExecuteNonQuery("DELETE FROM tolive.key_values WHERE key_name = '" + DbKeyName + "'", MOO.Data.MNODatabase.DMART);
            //this test should fail as the key does not exist
            try
            {
                _ = MOO.Data.ReadDBKey(DbKeyName);
                Assert.True(false);  //shouldn't get to this line, should hit exception
            }catch(Exception ex)
            {
                //verify exception message is right 
                Assert.Contains("not in the key_values", ex.Message);
            }
            //this should return the same val, test ValueIfNot Found parameter
            DbKeyTest = MOO.Data.ReadDBKey(DbKeyName, DbKeyVal);
            Assert.Equal(DbKeyTest, DbKeyVal);

            //now write the value to the database
            MOO.Data.WriteDBKey(DbKeyName, DbKeyVal);

            //now read it back and ensure it is correct
            DbKeyTest = MOO.Data.ReadDBKey(DbKeyName);
            Assert.Equal(DbKeyTest, DbKeyVal);

            //finally, lets clear out our test db key
            MOO.Data.ExecuteNonQuery("DELETE FROM tolive.key_values WHERE key_name = '" + DbKeyName + "'", MOO.Data.MNODatabase.DMART);
        }

        class ObjTest
        {
            public decimal Role_ID { get; set; }
            public string Role_Name { get; set; }
        }
        [Fact]
        public void TestDataToObject()
        {
            Util.RegisterFactories();
            //create a dataset to test the conversion
            DataSet ds = MOO.Data.ExecuteQuery("SELECT * FROM tolive.sec_role", MOO.Data.MNODatabase.DMART);
            ObjTest sr = MOO.Data.DataRowToObj<ObjTest>(ds.Tables[0].Rows[0]);
            Assert.IsType<ObjTest>(sr);

            List<ObjTest> srList;
            srList= MOO.Data.DataTableToObjList<ObjTest>(ds.Tables[0]);
            Assert.IsType<List<ObjTest>>(srList);
            
        }

        [Fact]
        public void TestTNSNamesParser()
        {
            var tns = MOO.TNSNamesOraParser.ParseTnsNamesFile(@"C:\MOOGlobalSettings\tnsnames.ora");
            Assert.NotEmpty(tns);

        }

        [Fact]
        public void TestOraTableComments()
        {
            Util.RegisterFactories();
            var cmtList = MOO.Data.GetOraTableComments("tolive", "conc_analytics_recovery");
            Assert.NotEmpty(cmtList);

        }
    }
}
