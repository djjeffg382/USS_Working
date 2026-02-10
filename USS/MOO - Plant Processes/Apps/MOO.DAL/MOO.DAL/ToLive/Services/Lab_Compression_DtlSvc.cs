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
    public static class Lab_Compression_DtlSvc
    {
        static Lab_Compression_DtlSvc()
        {
            Util.RegisterOracle();
        }
        internal const string TABLE_NAME = "TOLIVE.Lab_Compression_Dtl";
        public const string SEQUENCE_NAME = "TOLIVE.SEQ_LAB_COMPRESSION";

        public static async Task<Lab_Compression_Dtl> GetAsync(int PelletId)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Pellet_Id = :PelletId");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("PelletId", PelletId);
            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();

            Lab_Compression_Dtl retVal = null;
            if (rdr.HasRows)
            {
                await rdr.ReadAsync();
                retVal = DataRowToObject(rdr);
            }

            return retVal;
        }

        /// <summary>
        /// Gets the entire list of details from the header Lab_Compression key
        /// </summary>
        /// <returns></returns>
        public static async Task<List<Lab_Compression_Dtl>> GetByLabCompressionIdAsync(int LabCompressionId)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            //Add Filter here if needed
            sql.AppendLine($"WHERE Comp_Id = :LabCompressionId");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("LabCompressionId", LabCompressionId);

            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();
            List<Lab_Compression_Dtl> retVal = [];

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
            sql.AppendLine(SqlBuilder.GetSelect(typeof(Lab_Compression_Dtl), TABLE_NAME));

            return sql.ToString();
        }


        public static async Task<int> InsertAsync(Lab_Compression_Dtl obj)
        {
            if (obj.Pellet_Id <= 0)
                obj.Pellet_Id = Convert.ToInt32(await MOO.Data.GetNextSequenceAsync("TOLIVE.SEQ_LAB_COMPRESSION"));

            return await SqlBuilder.InsertAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);

        }


        public static async Task<int> InsertAsync(Lab_Compression_Dtl obj, OracleConnection conn)
        {
            if (obj.Pellet_Id <= 0)
                obj.Pellet_Id = Convert.ToInt32(await MOO.Data.GetNextSequenceAsync("TOLIVE.SEQ_LAB_COMPRESSION"));

            return await SqlBuilder.InsertAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }



        public static async Task<int> UpdateAsync(Lab_Compression_Dtl obj)
        {
            return await SqlBuilder.UpdateAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);
        }

        public static async Task<int> UpdateAsync(Lab_Compression_Dtl obj, OracleConnection conn)
        {
            return await SqlBuilder.UpdateAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }

        public static async Task<int> DeleteAsync(Lab_Compression_Dtl obj)
        {
            return await SqlBuilder.DeleteAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);
        }


        public static async Task<int> DeleteAsync(Lab_Compression_Dtl obj, OracleConnection conn)
        {
            return await SqlBuilder.DeleteAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0017:Simplify object initialization")]
        internal static Lab_Compression_Dtl DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Lab_Compression_Dtl RetVal = new();
            RetVal.Comp_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}comp_date");
            RetVal.Pellet_Id = (int)Util.GetRowVal(row, $"{ColPrefix}pellet_id");
            RetVal.Comp_Lbs = (double)Util.GetRowVal(row, $"{ColPrefix}comp_lbs");
            RetVal.Pellet_Nbr = (short)Util.GetRowVal(row, $"{ColPrefix}pellet_nbr");
            RetVal.Comp_Id = (int)Util.GetRowVal(row, $"{ColPrefix}comp_id");
            return RetVal;
        }

    }
}
