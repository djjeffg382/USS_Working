using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MOO.DAL.ToLive.Services
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public static class OM_VesselSvc
    {
        static OM_VesselSvc()
        {
            Util.RegisterOracle();
        }
        internal const string TABLE_NAME = "TOLIVE.OM_Vessel";


        /// <summary>
        /// Gets the vessel by the Vessel ID
        /// </summary>
        /// <param name="Vessel_Id"></param>
        /// <returns></returns>
        public static async Task<OM_Vessel> GetAsync(int Vessel_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Vessel_Id = :Vessel_Id");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("Vessel_Id", Vessel_Id);
            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();

            OM_Vessel retVal = null;
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }

            return retVal;
        }


        public static async Task<List<OM_Vessel>> GetAllAsync()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("ORDER BY Vessel_Name");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);

            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();
            List<OM_Vessel> retVal = [];

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
            sql.AppendLine(SqlBuilder.GetSelect(typeof(OM_Vessel), TABLE_NAME));

            return sql.ToString();
        }


        public static async Task<int> InsertAsync(OM_Vessel obj)
        {
            if (obj.Vessel_Id <= 0)
                obj.Vessel_Id = Convert.ToInt32(await MOO.Data.GetNextSequenceAsync("TOLIVE.SEQ_OM_CONFIG"));

            return await SqlBuilder.InsertAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);

        }


        public static async Task<int> InsertAsync(OM_Vessel obj, OracleConnection conn)
        {
            if (obj.Vessel_Id <= 0)
                obj.Vessel_Id = Convert.ToInt32(await MOO.Data.GetNextSequenceAsync("TOLIVE.SEQ_OM_CONFIG"));

            return await SqlBuilder.InsertAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }



        public static async Task<int> UpdateAsync(OM_Vessel obj)
        {
            return await SqlBuilder.UpdateAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);
        }

        public static async Task<int> UpdateAsync(OM_Vessel obj, OracleConnection conn)
        {
            return await SqlBuilder.UpdateAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }

        public static async Task<int> DeleteAsync(OM_Vessel obj)
        {
            return await SqlBuilder.DeleteAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);
        }


        public static async Task<int> DeleteAsync(OM_Vessel obj, OracleConnection conn)
        {
            return await SqlBuilder.DeleteAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0017:Simplify object initialization")]
        internal static OM_Vessel DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            OM_Vessel RetVal = new();
            RetVal.Vessel_Id = (int)Util.GetRowVal(row, $"{ColPrefix}vessel_id");
            RetVal.Vessel_Name = (string)Util.GetRowVal(row, $"{ColPrefix}vessel_name");
            RetVal.Active = (short)Util.GetRowVal(row, $"{ColPrefix}active") == 1;
            return RetVal;
        }
    }
}
