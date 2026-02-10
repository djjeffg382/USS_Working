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
    public static class OM_Location_TransSvc
    {
        static OM_Location_TransSvc()
        {
            Util.RegisterOracle();
        }
        internal const string TABLE_NAME = "TOLIVE.OM_Location_Trans";

        public static async Task<OM_Location_Trans> GetAsync(int TransactionId)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Location_Trans_Id = :TransactionId");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("TransactionId", TransactionId);
            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();

            OM_Location_Trans retVal = null;
            if (rdr.HasRows)
            {
                await rdr.ReadAsync();
                retVal = DataRowToObject(rdr);
            }

            return retVal;
        }

        /// <summary>
        /// Gets all transaction (in or out) for a given Location or Ship
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="SourceDestType"></param>
        /// <param name="LocationId">The Stockpile/Shiploader/Ship Id</param>
        /// <returns></returns>
        public static async Task<List<OM_Location_Trans>> GetTransactionsByLocationAsync(DateTime StartDate, DateTime EndDate, int LocationId)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            //Add Filter here if needed
            sql.AppendLine($"WHERE TRANSACTION_DATE BETWEEN :StartDate AND :EndDate");
            sql.AppendLine($"AND (SOURCE_ID = {LocationId}");
            sql.AppendLine($"       OR DEST_ID = {LocationId}) ");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("StartDate", StartDate);
            cmd.Parameters.Add("EndDate", EndDate);

            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();
            List<OM_Location_Trans> retVal = [];

            while (await rdr.ReadAsync())
            {
                var obj = DataRowToObject(rdr);
                retVal.Add(obj);
            }
            return retVal;
        }

        /// <summary>
        /// Gets all transaction (in or out) for a given Location or Ship
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="SourceDestType"></param>
        /// <param name="LocationId">The Stockpile/Shiploader/Ship Id</param>
        /// <returns></returns>
        public static async Task<List<OM_Location_Trans>> GetTransactionsByDateAsync(DateTime StartDate, DateTime EndDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            //Add Filter here if needed
            sql.AppendLine($"WHERE TRANSACTION_DATE BETWEEN :StartDate AND :EndDate");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("StartDate", StartDate);
            cmd.Parameters.Add("EndDate", EndDate);

            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();
            List<OM_Location_Trans> retVal = [];

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
            //If building a join use this:
            //sql.Append("SELECT ");
            //sql.AppendLine(SqlBuilder.GetColumnsForSelect(typeof(OM_Location_Trans), "<TABLE_ALIAS>", "<TABLE_PREFIX>"));
            //sql.AppendLine($"FROM {TABLE_NAME} <TABLE_ALIAS>");
            //Add Join Here

            //If just selecting this table
            sql.AppendLine(SqlBuilder.GetSelect(typeof(OM_Location_Trans), TABLE_NAME));

            return sql.ToString();
        }


        public static async Task<int> InsertAsync(OM_Location_Trans obj)
        {
            if (obj.Location_Trans_Id <= 0)
                obj.Location_Trans_Id = Convert.ToInt32(await MOO.Data.GetNextSequenceAsync("TOLIVE.SEQ_OM_TRANS"));

            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            using OracleTransaction trans = conn.BeginTransaction();
            var result = await InsertAsync(obj, conn);
            trans.Commit();
            return result;

        }


        public static async Task<int> InsertAsync(OM_Location_Trans obj, OracleConnection conn)
        {
            if (obj.Location_Trans_Id <= 0)
                obj.Location_Trans_Id = Convert.ToInt32(await MOO.Data.GetNextSequenceAsync("TOLIVE.SEQ_OM_TRANS"));

            var result = await SqlBuilder.InsertAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
            //upon insert we also need to update the inventory on both the source and the destination
            if(obj.SourceType == OM_Location_Trans.LocTransSrcDestType.Location)
                await OM_Location_InvSvc.InsertFromTransactionAsync(obj, OM_Location_InvSvc.SourceOrDest.Source, conn);
            if (obj.DestType == OM_Location_Trans.LocTransSrcDestType.Location)
                await OM_Location_InvSvc.InsertFromTransactionAsync(obj, OM_Location_InvSvc.SourceOrDest.Destination, conn);
            return result;
        }



        public static async Task<int> UpdateAsync(OM_Location_Trans obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            using OracleTransaction trans = conn.BeginTransaction();
            var result = await UpdateAsync(obj, conn);
            trans.Commit();
            return result;
        }

        public static async Task<int> UpdateAsync(OM_Location_Trans obj, OracleConnection conn)
        {
            var result =  await SqlBuilder.UpdateAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
            //upon update we also need to update the inventory on both the source and the destination
            if (obj.SourceType == OM_Location_Trans.LocTransSrcDestType.Location)
                await OM_Location_InvSvc.InsertFromTransactionAsync(obj, OM_Location_InvSvc.SourceOrDest.Source, conn);
            if (obj.DestType == OM_Location_Trans.LocTransSrcDestType.Location)
                await OM_Location_InvSvc.InsertFromTransactionAsync(obj, OM_Location_InvSvc.SourceOrDest.Destination, conn);
            return result;
        }

        public static async Task<int> DeleteAsync(OM_Location_Trans obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            using OracleTransaction trans = conn.BeginTransaction();
            var result = await DeleteAsync(obj, conn);
            trans.Commit();
            return result;

        }


        public static async Task<int> DeleteAsync(OM_Location_Trans obj, OracleConnection conn)
        {
            //need to delete the inventory record prior to deleting the transaction
            if(obj.SourceType == OM_Location_Trans.LocTransSrcDestType.Location)
            {
                var deleteInv = await OM_Location_InvSvc.GetByLocationTransactionAsync(obj.Location_Trans_Id, obj.Source_Id.Value);
                await OM_Location_InvSvc.DeleteAsync(deleteInv, conn);
            }
            if (obj.DestType == OM_Location_Trans.LocTransSrcDestType.Location)
            {
                var deleteInv = await OM_Location_InvSvc.GetByLocationTransactionAsync(obj.Location_Trans_Id, obj.Dest_Id.Value);
                await OM_Location_InvSvc.DeleteAsync(deleteInv, conn);
            }

            var result = await SqlBuilder.DeleteAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0017:Simplify object initialization")]
        internal static OM_Location_Trans DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            OM_Location_Trans RetVal = new();
            RetVal.Location_Trans_Id = (int)Util.GetRowVal(row, $"{ColPrefix}location_trans_id");
            RetVal.Transaction_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}transaction_date");
            RetVal.Source_Type_Id = (short)Util.GetRowVal(row, $"{ColPrefix}source_type_id");
            RetVal.Source_Id = (int?)Util.GetRowVal(row, $"{ColPrefix}source_id");
            RetVal.Dest_Type_Id = (short)Util.GetRowVal(row, $"{ColPrefix}dest_type_id");
            RetVal.Dest_Id = (int?)Util.GetRowVal(row, $"{ColPrefix}dest_id");
            RetVal.Quantity = (double)Util.GetRowVal(row, $"{ColPrefix}quantity");
            RetVal.Fe = (float?)Util.GetRowVal(row, $"{ColPrefix}fe");
            return RetVal;
        }

    }
}
