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
    public static class Prop_Rmv_ReasonSvc
    {
        static Prop_Rmv_ReasonSvc()
        {
            Util.RegisterOracle();
        }
        internal const string TABLE_NAME = "TOLIVE.Prop_Rmv_Reason";

        public static async Task<Prop_Rmv_Reason> GetAsync(int Prop_Rmv_Reason_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Prop_Rmv_Reason_Id = :Prop_Rmv_Reason_Id");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("Prop_Rmv_Reason_Id", Prop_Rmv_Reason_Id);
            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();

            Prop_Rmv_Reason retVal = null;
            if (rdr.HasRows)
            {
                await rdr.ReadAsync();
                retVal = DataRowToObject(rdr);
            }

            return retVal;
        }


        public static async Task<List<Prop_Rmv_Reason>> GetAllAsync(bool IncludeInactive = false)
        {
            StringBuilder sql = new();
            sql.AppendLine(GetSelect());
            if (!IncludeInactive)
                sql.AppendLine("WHERE active = 1");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);

            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();
            List<Prop_Rmv_Reason> retVal = [];

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
            sql.AppendLine(SqlBuilder.GetSelect(typeof(Prop_Rmv_Reason), TABLE_NAME));

            return sql.ToString();
        }


        public static async Task<int> InsertAsync(Prop_Rmv_Reason obj)
        {
            if (obj.Prop_Rmv_Reason_Id <= 0)
                obj.Prop_Rmv_Reason_Id = Convert.ToInt32(await MOO.Data.GetNextSequenceAsync("TOLIVE.SEQ_PROP_RMV_CONFIG"));

            return await SqlBuilder.InsertAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);

        }


        public static async Task<int> InsertAsync(Prop_Rmv_Reason obj, OracleConnection conn)
        {
            if (obj.Prop_Rmv_Reason_Id <= 0)
                obj.Prop_Rmv_Reason_Id = Convert.ToInt32(await MOO.Data.GetNextSequenceAsync("TOLIVE.SEQ_PROP_RMV_CONFIG"));

            return await SqlBuilder.InsertAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }



        public static async Task<int> UpdateAsync(Prop_Rmv_Reason obj)
        {
            return await SqlBuilder.UpdateAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);
        }

        public static async Task<int> UpdateAsync(Prop_Rmv_Reason obj, OracleConnection conn)
        {
            return await SqlBuilder.UpdateAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }

        public static async Task<int> DeleteAsync(Prop_Rmv_Reason obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            using OracleTransaction transaction = conn.BeginTransaction();
            var retVal = await DeleteAsync(obj, conn);
            transaction.Commit();
            conn.Close();
            return retVal;
        }


        public static async Task<int> DeleteAsync(Prop_Rmv_Reason obj, OracleConnection conn)
        {
            //we must remove all approvers before deleting the Reason
            await Prop_Rmv_Reason_ApproverSvc.RemoveAllApproversAsync(obj, conn);
            return await SqlBuilder.DeleteAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0017:Simplify object initialization")]
        internal static Prop_Rmv_Reason DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Prop_Rmv_Reason RetVal = new();
            RetVal.Prop_Rmv_Reason_Id = (int)Util.GetRowVal(row, $"{ColPrefix}Prop_Rmv_reason_id");
            RetVal.Reason_Name = (string)Util.GetRowVal(row, $"{ColPrefix}reason_name");
            RetVal.Active = (short)Util.GetRowVal(row, $"{ColPrefix}active") == 1;
            RetVal.Req_Special_Approval = (short)Util.GetRowVal(row, $"{ColPrefix}Req_Special_Approval") == 1;
            return RetVal;
        }

    }
}
