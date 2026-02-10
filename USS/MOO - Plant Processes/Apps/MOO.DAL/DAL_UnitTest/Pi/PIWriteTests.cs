using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.Pi;

namespace DAL_UnitTest.Pi
{
    public class PIWriteTests : BaseTestClass
    {
        [Fact]
        public void TestInsUpdDel()
        {
            DateTime testTime = DateTime.Now;
            //this should cause an insert to happen
            Assert.Equal(1, DAL.Services.PiComp2Svc.PutPIDataUnBuffered("Brian_Test", testTime, 123));


            //this should cause an update to happen
            Assert.Equal(1, DAL.Services.PiComp2Svc.PutPIDataUnBuffered("Brian_Test", testTime, 999));

            //delete statement will return a -1 for recsAffected if it runs
            Assert.Equal(-1, DAL.Services.PiComp2Svc.DeletePIDataUnbuffered("Brian_Test", testTime));
        }

        [Fact]
        public void RunBufferedWriteTest()
        {
            DAL.Services.PiComp2Svc.InsertPIDataBuffered("Brian_Test", DateTime.Now.AddSeconds(-5), 111);

            DAL.Services.PiComp2Svc.InsertPIStatusBuffered("Brian_Test", DateTime.Now, DAL.Enums.PISystemStates.Bad_Data);
        }


        //[Fact]
        //public void TestRunPIBuffer()
        //{

        //    DateTime testRun = DateTime.Now;
        //    DbProviderFactories.RegisterFactory(MOO.Data.DBType.Oracle.ToString(), Oracle.ManagedDataAccess.Client.OracleClientFactory.Instance);
        //    StringBuilder sql = new();
        //    sql.AppendLine("INSERT INTO tolive.pi_buffer(thedate, tag, pidate, value, s1_write, s2_write, type)");
        //    sql.AppendLine($"VALUES(SYSDATE, 'Brian_Test', {MOO.Dates.OraDate(testRun)}, 123, 1, 0, 'U')");
        //    MOO.Data.ExecuteNonQuery(sql.ToString(), MOO.Data.MNODatabase.DMART);
        //    DAL.Services.PiComp2Svc.RunOraPIBuffer();

        //    //test delete
        //    sql.Clear();
        //    sql.AppendLine("INSERT INTO tolive.pi_buffer(thedate, tag, pidate, value, s1_write, s2_write, type)");
        //    sql.AppendLine($"VALUES(SYSDATE, 'Brian_Test', {MOO.Dates.OraDate(testRun)}, 123, 1, 0, 'D')");
        //    MOO.Data.ExecuteNonQuery(sql.ToString(), MOO.Data.MNODatabase.DMART);
        //    DAL.Services.PiComp2Svc.RunOraPIBuffer();

        //}

    }
}
