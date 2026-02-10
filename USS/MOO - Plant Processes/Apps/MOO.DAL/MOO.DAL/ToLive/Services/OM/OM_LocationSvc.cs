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
    public static class OM_LocationSvc
    {
        static OM_LocationSvc()
        {
            Util.RegisterOracle();
        }
        internal const string TABLE_NAME = "TOLIVE.OM_Location";

        public static async Task<OM_Location> GetAsync(int LocationId)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Location_Id = :LocationId");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("LocationId", LocationId);
            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();

            OM_Location retVal = null;
            if (rdr.HasRows)
            {
                await rdr.ReadAsync();
                retVal = DataRowToObject(rdr,"loc_");
            }

            return retVal;
        }


        public static async Task<List<OM_Location>> GetAllAsync(OM_Location.LocationTypeEnum? LocationType = null, bool ShowInactive = true)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE 1=1");  //just adding this so I don't need to decide if we need AND for the next 2 filters
            if(LocationType != null) 
                sql.AppendLine($"AND Location_Type_Id = {(short)LocationType}");
            if (!ShowInactive)
                sql.AppendLine("AND active = 1");

            sql.AppendLine("ORDER BY Location_Name");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);

            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();
            List<OM_Location> retVal = [];

            while (await rdr.ReadAsync())
            {
                var obj = DataRowToObject(rdr,"loc_");
                retVal.Add(obj);
            }
            return retVal;
        }



        public static string GetSelect()
        {
            StringBuilder sql = new();
            //If building a join use this:
            sql.Append("SELECT ");
            sql.AppendLine(SqlBuilder.GetColumnsForSelect(typeof(OM_Location), "loc", "loc_") + ",");
            sql.AppendLine(SqlBuilder.GetColumnsForSelect(typeof(OM_Site), "site", "site_"));
            sql.AppendLine($"FROM {TABLE_NAME} loc");
            sql.AppendLine($"JOIN {OM_SiteSvc.TABLE_NAME} site");
            sql.AppendLine("    ON loc.site_id = site.site_id");
            

            return sql.ToString();
        }


        public static async Task<int> InsertAsync(OM_Location obj)
        {
            if (obj.Location_Id <= 0)
                obj.Location_Id = Convert.ToInt32(await MOO.Data.GetNextSequenceAsync("TOLIVE.SEQ_OM_CONFIG"));

            //Database Trigger OM_LOCATION_AR will insert a starting inventory record into the location_inv table upon inserting
            return await SqlBuilder.InsertAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);

        }


        public static async Task<int> InsertAsync(OM_Location obj, OracleConnection conn)
        {
            if (obj.Location_Id <= 0)
                obj.Location_Id = Convert.ToInt32(await MOO.Data.GetNextSequenceAsync("TOLIVE.SEQ_OM_CONFIG"));

            //Database Trigger OM_LOCATION_AR will insert a starting inventory record into the location_inv table upon inserting
            return await SqlBuilder.InsertAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }



        public static async Task<int> UpdateAsync(OM_Location obj)
        {
            return await SqlBuilder.UpdateAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);
        }

        public static async Task<int> UpdateAsync(OM_Location obj, OracleConnection conn)
        {
            return await SqlBuilder.UpdateAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }

        public static async Task<int> DeleteAsync(OM_Location obj)
        {
            return await SqlBuilder.DeleteAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);
        }


        public static async Task<int> DeleteAsync(OM_Location obj, OracleConnection conn)
        {
            return await SqlBuilder.DeleteAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0017:Simplify object initialization")]
        internal static OM_Location DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            OM_Location RetVal = new();
            RetVal.Location_Id = (int)Util.GetRowVal(row, $"{ColPrefix}location_id");
            RetVal.Location_Name = (string)Util.GetRowVal(row, $"{ColPrefix}location_name");
            RetVal.Location_Type_Id = (short)Util.GetRowVal(row, $"{ColPrefix}location_type_id");
            RetVal.Site = OM_SiteSvc.DataRowToObject(row, "site_");
            RetVal.Active = (short)Util.GetRowVal(row, $"{ColPrefix}active") == 1;
            return RetVal;
        }

    }
}
