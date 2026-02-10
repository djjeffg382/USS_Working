using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Services
{
    public static class Lab_CompressionSvc
    {
        internal const string TABLE_NAME = "TOLIVE.Lab_Compression";
        public const string SEQUENCE_NAME = "TOLIVE.SEQ_LAB_COMPRESSION";
        static Lab_CompressionSvc()
        {
            Util.RegisterOracle();
        }

        public static async Task<Lab_Compression> GetAsync(int CompId)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Comp_Id = :CompId");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("CompId", CompId);
            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();

            Lab_Compression retVal = null;
            if (rdr.HasRows)
            {
                await rdr.ReadAsync();
                retVal = DataRowToObject(rdr);
            }

            return retVal;
        }

        /// <summary>
        /// Gets a list of Compression tests where created_date is within the date range
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static async Task<List<Lab_Compression>> GetByDateAsync(DateTime StartDate, DateTime EndDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            //Add Filter here if needed
            sql.AppendLine($"WHERE Created_Date BETWEEN :StartDate AND :EndDate");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("StartDate", StartDate);
            cmd.Parameters.Add("EndDate", EndDate);

            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();
            List<Lab_Compression> retVal = [];

            while (await rdr.ReadAsync())
            {
                var obj = DataRowToObject(rdr);
                retVal.Add(obj);
            }
            return retVal;
        }



        public static string GetSelect()
        {
            StringBuilder sql = new();
            
            sql.AppendLine(SqlBuilder.GetSelect(typeof(Lab_Compression), TABLE_NAME));

            return sql.ToString();
        }


        public static async Task<int> InsertAsync(Lab_Compression obj)
        {
            if (obj.Comp_Id <= 0)
                obj.Comp_Id = Convert.ToInt32(await MOO.Data.GetNextSequenceAsync(SEQUENCE_NAME));

            return await SqlBuilder.InsertAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);

        }


        public static async Task<int> InsertAsync(Lab_Compression obj, OracleConnection conn)
        {
            if (obj.Comp_Id <= 0)
                obj.Comp_Id = Convert.ToInt32(await MOO.Data.GetNextSequenceAsync(SEQUENCE_NAME));

            return await SqlBuilder.InsertAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }



        public static async Task<int> UpdateAsync(Lab_Compression obj)
        {
            return await SqlBuilder.UpdateAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);
        }

        public static async Task<int> UpdateAsync(Lab_Compression obj, OracleConnection conn)
        {
            return await SqlBuilder.UpdateAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }

        public static async Task<int> DeleteAsync(Lab_Compression obj)
        {
            return await SqlBuilder.DeleteAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);
        }


        public static async Task<int> DeleteAsync(Lab_Compression obj, OracleConnection conn)
        {
            return await SqlBuilder.DeleteAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0017:Simplify object initialization")]
        internal static Lab_Compression DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Lab_Compression RetVal = new();
            RetVal.Comp_Id = (int)Util.GetRowVal(row, $"{ColPrefix}comp_id");
            RetVal.Created_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}created_date");
            RetVal.Line_Nbr = (short?)Util.GetRowVal(row, $"{ColPrefix}line_nbr");
            RetVal.Test_Nbr = (short)Util.GetRowVal(row, $"{ColPrefix}test_nbr");
            RetVal.Shift_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}shift_date");
            RetVal.Shift = (short?)Util.GetRowVal(row, $"{ColPrefix}shift");
            RetVal.Instrument = (short)Util.GetRowVal(row, $"{ColPrefix}instrument");
            RetVal.Comp200 = (double?)Util.GetRowVal(row, $"{ColPrefix}comp200");
            RetVal.Comp300 = (double?)Util.GetRowVal(row, $"{ColPrefix}comp300");
            RetVal.Average = (double?)Util.GetRowVal(row, $"{ColPrefix}average");
            RetVal.Shift_Half = (short?)Util.GetRowVal(row, $"{ColPrefix}shift_half");
            return RetVal;
        }

    }
}
