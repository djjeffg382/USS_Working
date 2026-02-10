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
    public static class Prop_Rmv_AreaSvc
    {
        static Prop_Rmv_AreaSvc()
        {
            Util.RegisterOracle();
        }
        internal const string TABLE_NAME = "TOLIVE.Prop_Rmv_Area";

        public static async Task<Prop_Rmv_Area> GetAsync(int Prop_Rmv_Area_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Prop_Rmv_Area_Id = :Prop_Rmv_Area_Id");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("Prop_Rmv_Area_Id", Prop_Rmv_Area_Id);
            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();

            Prop_Rmv_Area retVal = null;
            if (rdr.HasRows)
            {
                await rdr.ReadAsync();
                retVal = DataRowToObject(rdr);
            }

            return retVal;
        }


        public static async Task<List<Prop_Rmv_Area>> GetAllAsync(bool IncludeInactive = false)
        {
            StringBuilder sql = new();
            sql.AppendLine(GetSelect());
            if (!IncludeInactive)
                sql.AppendLine("WHERE active = 1");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);

            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();
            List<Prop_Rmv_Area> retVal = [];

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
            sql.AppendLine(SqlBuilder.GetSelect(typeof(Prop_Rmv_Area), TABLE_NAME));

            return sql.ToString();
        }


        public static async Task<int> InsertAsync(Prop_Rmv_Area obj)
        {
            if (obj.Prop_Rmv_Area_Id <= 0)
                obj.Prop_Rmv_Area_Id = Convert.ToInt32(await MOO.Data.GetNextSequenceAsync("TOLIVE.SEQ_PROP_RMV_CONFIG"));

            return await SqlBuilder.InsertAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);

        }


        public static async Task<int> InsertAsync(Prop_Rmv_Area obj, OracleConnection conn)
        {
            if (obj.Prop_Rmv_Area_Id <= 0)
                obj.Prop_Rmv_Area_Id = Convert.ToInt32(await MOO.Data.GetNextSequenceAsync("TOLIVE.SEQ_PROP_RMV_CONFIG"));

            return await SqlBuilder.InsertAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }



        public static async Task<int> UpdateAsync(Prop_Rmv_Area obj)
        {
            return await SqlBuilder.UpdateAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);
        }

        public static async Task<int> UpdateAsync(Prop_Rmv_Area obj, OracleConnection conn)
        {
            return await SqlBuilder.UpdateAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }

        public static async Task<int> DeleteAsync(Prop_Rmv_Area obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            using OracleTransaction transaction = conn.BeginTransaction();
            var retVal =  await DeleteAsync(obj, conn);
            transaction.Commit();
            conn.Close();
            return retVal;
        }


        public static async Task<int> DeleteAsync(Prop_Rmv_Area obj, OracleConnection conn)
        {
            //we must remove all approvers before deleting the area
            await Prop_Rmv_Area_ApproverSvc.RemoveAllApproversAsync(obj, conn);
            return await SqlBuilder.DeleteAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0017:Simplify object initialization")]
        internal static Prop_Rmv_Area DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Prop_Rmv_Area RetVal = new();
            RetVal.Prop_Rmv_Area_Id = (int)Util.GetRowVal(row, $"{ColPrefix}prop_rmv_area_id");
            RetVal.Plant = Enum.Parse<MOO.Plant>((string)Util.GetRowVal(row, $"{ColPrefix}plant"));
            RetVal.Area_Name = (string)Util.GetRowVal(row, $"{ColPrefix}area_name");
            RetVal.Active = (short)Util.GetRowVal(row, $"{ColPrefix}active") == 1;
            return RetVal;
        }

    }
}
