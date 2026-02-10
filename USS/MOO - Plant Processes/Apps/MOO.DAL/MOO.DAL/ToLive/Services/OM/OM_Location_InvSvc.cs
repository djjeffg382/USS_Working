using MOO.DAL.LIMS.Models;
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



    /// <summary>
    /// Class for obtaining inventory history of a location
    /// </summary>
    /// <remarks>Insert/Updates/Deletes are not done directly on this table.  They will be done through an OM_LocationTrans record</remarks>
    public static class OM_Location_InvSvc
    {
        public enum SourceOrDest
        {
            Source,
            Destination
        }

        public enum BeforeOrAfter
        {
            Before,
            After
        }

        static OM_Location_InvSvc()
        {
            Util.RegisterOracle();
        }
        internal const string TABLE_NAME = "TOLIVE.OM_Location_Inv";


        /// <summary>
        /// Gets the Location Inventory by the primary Key Location_Inv_Id
        /// </summary>
        /// <param name="LocationInvId"></param>
        /// <returns></returns>
        public static async Task<OM_Location_Inv> GetAsync(int LocationInvId)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Location_Inv_Id = :LocationInvId");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("LocationInvId", LocationInvId);
            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();

            OM_Location_Inv retVal = null;
            if (rdr.HasRows)
            {
                await rdr.ReadAsync();
                retVal = DataRowToObject(rdr);
            }

            return retVal;
        }

        /// <summary>
        /// Gets the corresponding inventory history record by the Location and the Transaction ID
        /// </summary>
        /// <param name="LocationTransactionId"></param>
        /// <param name="LocationId"></param>
        /// <returns></returns>
        public static async Task<OM_Location_Inv> GetByLocationTransactionAsync(int LocationTransactionId, int LocationId)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE location_id = :LocationId");
            sql.AppendLine($"AND location_trans_id = :LocationTransactionId");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("LocationId", LocationId);
            cmd.Parameters.Add("LocationTransactionId", LocationTransactionId);
            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();

            OM_Location_Inv retVal = null;
            if (rdr.HasRows)
            {
                await rdr.ReadAsync();
                retVal = DataRowToObject(rdr);
            }

            return retVal;
        }

        /// <summary>
        /// Gets location inventory recordds by a date range
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="LocationId"></param>
        /// <returns></returns>
        public static async Task<List<OM_Location_Inv>> GetByLocationAsync(DateTime StartDate, DateTime EndDate, int LocationId)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            //Add Filter here if needed
            sql.AppendLine($"WHERE inv.INVENTORY_DATE BETWEEN :StartDate AND :EndDate");
            sql.AppendLine($"AND inv.Location_Id = {LocationId}");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("StartDate", StartDate);
            cmd.Parameters.Add("EndDate", EndDate);

            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();
            List<OM_Location_Inv> retVal = [];

            while (await rdr.ReadAsync())
            {
                var obj = DataRowToObject(rdr);
                retVal.Add(obj);
            }
            return retVal;
        }

        /// <summary>
        /// Gets the inventory record Before or After the specified time
        /// </summary>
        /// <param name="PriorTo"></param>
        /// <param name="LocationId"></param>
        /// <param name="BeforeAfter">Determines if we are getting the record directly before or after the date</param>
        /// <returns></returns>
        public static async Task<OM_Location_Inv> GetInvRecordBeforeAfter(DateTime PriorTo, int LocationId, BeforeOrAfter BeforeAfter)
        {
            //the only difference between before and after are the order by and the equality expression
            string orderBy = BeforeAfter == BeforeOrAfter.Before?"desc" : "asc";
            string equality = BeforeAfter == BeforeOrAfter.Before ? "<" : ">";

            StringBuilder sql = new();
            sql.Append(GetSelect());
            //Add Filter here if needed
            sql.AppendLine("WHERE inv.location_inv_id = (");
            sql.AppendLine("            SELECT location_inv_id FROM");
            sql.AppendLine($"                (SELECT location_inv_id, ROW_NUMBER() OVER(ORDER BY inventory_date {orderBy}) rn");
            sql.AppendLine("                    FROM TOLIVE.OM_Location_Inv");
            sql.AppendLine($"                    WHERE location_id = {LocationId}");
            sql.AppendLine($"                    AND inventory_date {equality} :PriorTo)");
            sql.AppendLine("            WHERE rn = 1)");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("PriorTo", PriorTo);

            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();

            OM_Location_Inv retVal = null;
            if (rdr.HasRows)
            {
                await rdr.ReadAsync();
                retVal = DataRowToObject(rdr);
            }

            return retVal;
        }



        public static string GetSelect()
        {
            StringBuilder sql = new();
            sql.Append("SELECT ");
            sql.AppendLine(SqlBuilder.GetColumnsForSelect(typeof(OM_Location_Inv), "inv", "inv_") + ", " );
            sql.AppendLine(SqlBuilder.GetColumnsForSelect(typeof(OM_Location_Trans), "trans", "trans_") + ", ");
            sql.AppendLine(SqlBuilder.GetColumnsForSelect(typeof(OM_Location), "loc", "loc_") + ", ");
            sql.AppendLine(SqlBuilder.GetColumnsForSelect(typeof(OM_Site), "site", "site_"));
            sql.AppendLine($"FROM {TABLE_NAME} inv");
            sql.AppendLine($"JOIN {OM_Location_TransSvc.TABLE_NAME} trans");
            sql.AppendLine("    ON inv.location_trans_id = trans.location_trans_id");
            sql.AppendLine($"JOIN {OM_LocationSvc.TABLE_NAME} loc");
            sql.AppendLine("    ON inv.location_id = loc.location_id");
            //location object includes the site object so we need to include that in the query
            sql.AppendLine($"JOIN {OM_SiteSvc.TABLE_NAME} site");
            sql.AppendLine("    ON loc.site_id = site.site_id");

            return sql.ToString();
        }


        //public static async Task<int> InsertAsync(OM_Location_Inv obj)
        //{
        //    if (obj.Location_Inv_Id <= 0)
        //        obj.Location_Inv_Id = Convert.ToInt32(await MOO.Data.GetNextSequenceAsync("TOLIVE.SEQ_OM_INV"));

        //    return await SqlBuilder.InsertAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);

        //}


        internal static async Task<int> InsertFromTransactionAsync(OM_Location_Trans LocationTransaction, SourceOrDest UseSourceOrDest, OracleConnection conn)
        {
            int newInvId = Convert.ToInt32(await MOO.Data.GetNextSequenceAsync("TOLIVE.SEQ_OM_INV"));

            int locationId;
            double quantity;
            if (UseSourceOrDest == SourceOrDest.Source)
            {
                locationId = LocationTransaction.Source_Id.Value;
                //if this is the source, then we are taking from it so quantity is negative
                quantity = LocationTransaction.Quantity * -1;
            }
            else
            {
                locationId = LocationTransaction.Dest_Id.Value;
                //if this is the destination, then we are adding to the location
                quantity = LocationTransaction.Quantity;
            }

            OM_Location location = await OM_LocationSvc.GetAsync(locationId);

            //need to get the inventory value just before this transaction
            var invPrior = await GetInvRecordBeforeAfter(LocationTransaction.Transaction_Date, locationId, BeforeOrAfter.Before);
            OM_Location_Inv newInv = new()
            {
                Location_Inv_Id = newInvId,
                Location = location,
                Location_Transaction = LocationTransaction,
                Inventory_Date = LocationTransaction.Transaction_Date,
                Quantity = invPrior.Quantity + quantity
            };
            if (UseSourceOrDest == SourceOrDest.Destination)
            {
                //we are adding to location so we need to recalculate weighted average
                CalculateQualities(invPrior, newInv, LocationTransaction);
            }
            else
            {
                //we are removing quantity so keep qualities the same
                newInv.Fe = invPrior.Fe;
            }

            var result =  await SqlBuilder.InsertAsync(newInv, conn, Data.DBType.Oracle, TABLE_NAME);
            

            return result;
        }



        //public static async Task<int> UpdateAsync(OM_Location_Inv obj)
        //{
        //    return await SqlBuilder.UpdateAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);
        //}

        internal static async Task<int> UpdateFromTransactionAsync(OM_Location_Trans LocationTransaction, SourceOrDest UseSourceOrDest, OracleConnection conn)
        {
            int locationId;
            double quantity;
            if (UseSourceOrDest == SourceOrDest.Source)
            {
                locationId = LocationTransaction.Source_Id.Value;
                //if this is the source, then we are taking from it so quantity is negative
                quantity = LocationTransaction.Quantity * -1;
            }
            else
            {
                locationId = LocationTransaction.Dest_Id.Value;
                //if this is the destination, then we are adding to the location
                quantity = LocationTransaction.Quantity;
            }
            //need to get the inventory value just before this transaction
            var invPrior = await GetInvRecordBeforeAfter(LocationTransaction.Transaction_Date, locationId, BeforeOrAfter.Before);
            OM_Location_Inv updInv = await GetByLocationTransactionAsync(LocationTransaction.Location_Trans_Id, locationId);
            updInv.Quantity = invPrior.Quantity + quantity;

            if (UseSourceOrDest == SourceOrDest.Destination)
            {
                //we are adding to location so we need to recalculate weighted average
                CalculateQualities(invPrior, updInv, LocationTransaction);
            }
            else
            {
                //we are removing quantity so keep qualities the same
                updInv.Fe = invPrior.Fe;
            }

            var result = await SqlBuilder.UpdateAsync(updInv, conn, Data.DBType.Oracle, TABLE_NAME);


            return result;
        }


        //public static async Task<int> DeleteAsync(OM_Location_Inv obj)
        //{
        //    return await SqlBuilder.DeleteAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);
        //}


        internal static async Task<int> DeleteAsync(OM_Location_Inv obj, OracleConnection conn)
        {
            return await SqlBuilder.DeleteAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }


        /// <summary>
        /// Looks at prior inventory and the adjustment and calculates qualities
        /// </summary>
        /// <param name="InventoryPrior"></param>
        /// <param name="InventoryNew"></param>
        /// <param name="InventoryTransaction"></param>
        private static void CalculateQualities(OM_Location_Inv InventoryPrior, OM_Location_Inv InventoryNew, OM_Location_Trans InventoryTransaction)
        {
            if(InventoryTransaction.SourceType == OM_Location_Trans.LocTransSrcDestType.AdjustIn || InventoryTransaction.SourceType == OM_Location_Trans.LocTransSrcDestType.AdjustOut)
            {
                //adjustments will maintain the same quality
                InventoryNew.Fe = InventoryPrior.Fe;
            }
            else
            {
                InventoryNew.Fe = (float)((InventoryPrior.Fe * InventoryPrior.Quantity + InventoryTransaction.Fe * InventoryTransaction.Quantity) / (InventoryPrior.Quantity + InventoryTransaction.Quantity));
            }
                
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0017:Simplify object initialization")]
        internal static OM_Location_Inv DataRowToObject(DbDataReader row, string ColPrefix = "inv_")
        {
            OM_Location_Inv RetVal = new();
            RetVal.Location_Inv_Id = (int)Util.GetRowVal(row, $"{ColPrefix}location_inv_id");

            RetVal.Location = OM_LocationSvc.DataRowToObject(row,"loc_");
            RetVal.Location_Transaction = OM_Location_TransSvc.DataRowToObject(row, "trans_");
            RetVal.Inventory_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}inventory_date");
            RetVal.Quantity = (double)Util.GetRowVal(row, $"{ColPrefix}quantity");
            RetVal.Fe = (float)Util.GetRowVal(row, $"{ColPrefix}fe");
            return RetVal;
        }

    }
}
