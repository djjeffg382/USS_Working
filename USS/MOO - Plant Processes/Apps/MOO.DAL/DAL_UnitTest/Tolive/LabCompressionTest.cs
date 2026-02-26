using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DAL = MOO.DAL.ToLive;
using Oracle.ManagedDataAccess.Client;

namespace DAL_UnitTest.Tolive
{
    public class LabCompressionTest : BaseTestClass
    {
        [Fact]
        public void TestLabCompression()
        {
            MOO.DAL.Util.RegisterOracle();  //Not needed in production but needed for testing as this is the first time we contact oracle


            var NewSeq =Convert.ToInt32( MOO.Data.GetNextSequenceAsync(DAL.Services.Lab_CompressionSvc.SEQUENCE_NAME).GetAwaiter().GetResult());
            //test insert
            DAL.Models.Lab_Compression comp = new()
            {
                Comp_Id = NewSeq,
                Created_Date = DateTime.Now,
                Line_Nbr = 3,
                Test_Nbr = 10,
                Shift_Date = DateTime.Today,
                Shift = 1,
                Shift_Half = 2,
                Instrument = 1,
                Comp200 = 1.234,
                Comp300 = 5.678,
                Average = 9.0123               
            };

            //create detail

            DAL.Models.Lab_Compression_Dtl compDtl = new()
            {
                Comp_Date = DateTime.Now,
                Comp_Lbs = 100,
                Pellet_Nbr = 1,
                Comp_Id = NewSeq
                //don't need to provide pellet_id, insert will get a new sequence for you
            };

            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            using OracleTransaction transaction = conn.BeginTransaction();

            int result = DAL.Services.Lab_CompressionSvc.InsertAsync(comp,conn).GetAwaiter().GetResult();
            Assert.Equal(1, result);  //should return 1 for records inserted
            result = DAL.Services.Lab_Compression_DtlSvc.InsertAsync(compDtl,conn).GetAwaiter().GetResult();
            Assert.Equal(1, result);  //should return 1 for records inserted

            transaction.Commit();
            conn.Close();


            //now test selecting
            var compList = DAL.Services.Lab_CompressionSvc.GetByDateAsync(DateTime.Today.AddYears(-5), DateTime.Today.AddYears(1)).GetAwaiter().GetResult();
            Assert.NotEmpty(compList);
            //test single get
            comp = DAL.Services.Lab_CompressionSvc.GetAsync(NewSeq).GetAwaiter().GetResult();
            Assert.NotNull(comp);

            //test selecting detail
            var dtlList = DAL.Services.Lab_Compression_DtlSvc.GetByLabCompressionIdAsync(comp.Comp_Id).GetAwaiter().GetResult();
            Assert.NotEmpty(dtlList);
            //test get single
            compDtl = DAL.Services.Lab_Compression_DtlSvc.GetAsync(dtlList[0].Pellet_Id).GetAwaiter().GetResult();
            Assert.NotNull(compDtl);

            //now test update
            compDtl.Comp_Lbs = 150;
            result = DAL.Services.Lab_Compression_DtlSvc.UpdateAsync(compDtl).GetAwaiter().GetResult();
            Assert.Equal(1, result);  //should return 1 for records updated

            comp.Shift = 3;
            comp.Compression_Type = DAL.Models.Lab_Compression.LabCompressionType.MtcPellet;
            result = DAL.Services.Lab_CompressionSvc.UpdateAsync(comp).GetAwaiter().GetResult();
            Assert.Equal(1, result);  //should return 1 for records updated

            //now test delete.  must delete detail first

            result = DAL.Services.Lab_Compression_DtlSvc.DeleteAsync(compDtl).GetAwaiter().GetResult();
            Assert.Equal(1, result);  //should return 1 for records deleted

            result = DAL.Services.Lab_CompressionSvc.DeleteAsync(comp).GetAwaiter().GetResult();
            Assert.Equal(1, result);  //should return 1 for records deleted



        }
    }
}
