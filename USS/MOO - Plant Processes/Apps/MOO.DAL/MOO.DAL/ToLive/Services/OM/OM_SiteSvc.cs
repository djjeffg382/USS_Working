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
    public static class OM_SiteSvc
    {
        static OM_SiteSvc()
        {
            Util.RegisterOracle();
        }
        internal const string TABLE_NAME = "TOLIVE.OM_Site";

        public static async Task<OM_Site> GetAsync(int SiteId)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Site_Id = :SiteId");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("SiteId", SiteId);
            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();

            OM_Site retVal = null;
            if (rdr.HasRows)
            {
                await rdr.ReadAsync();
                retVal = DataRowToObject(rdr);
            }

            return retVal;
        }


        public static async Task<List<OM_Site>> GetAllAsync(OM_Site.SiteTypeEnum? LocationType = null, bool ShowInactive = true)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE 1=1");  //just adding this so I don't need to decide if we need AND for the next 2 filters
            //Add Filter here if needed
            if (LocationType != null)
                sql.AppendLine($"AND Site_type_id = {(short)LocationType}");
            if (!ShowInactive)
                sql.AppendLine("AND active = 1");

            sql.AppendLine("ORDER BY Site_Name");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);

            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();
            List<OM_Site> retVal = [];

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
            sql.AppendLine(SqlBuilder.GetSelect(typeof(OM_Site), TABLE_NAME));

            return sql.ToString();
        }


        public static async Task<int> InsertAsync(OM_Site obj)
        {
            if (obj.Site_Id <= 0)
                obj.Site_Id = Convert.ToInt32(await MOO.Data.GetNextSequenceAsync("TOLIVE.SEQ_OM_CONFIG"));

            return await SqlBuilder.InsertAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);

        }


        public static async Task<int> InsertAsync(OM_Site obj, OracleConnection conn)
        {
            if (obj.Site_Id <= 0)
                obj.Site_Id = Convert.ToInt32(await MOO.Data.GetNextSequenceAsync("TOLIVE.SEQ_OM_CONFIG"));

            return await SqlBuilder.InsertAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }



        public static async Task<int> UpdateAsync(OM_Site obj)
        {
            return await SqlBuilder.UpdateAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);
        }

        public static async Task<int> UpdateAsync(OM_Site obj, OracleConnection conn)
        {
            return await SqlBuilder.UpdateAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }

        public static async Task<int> DeleteAsync(OM_Site obj)
        {
            return await SqlBuilder.DeleteAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);
        }


        public static async Task<int> DeleteAsync(OM_Site obj, OracleConnection conn)
        {
            return await SqlBuilder.DeleteAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0017:Simplify object initialization")]
        internal static OM_Site DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            OM_Site RetVal = new();
            RetVal.Site_Id = (int)Util.GetRowVal(row, $"{ColPrefix}site_id");
            RetVal.Site_Name = (string)Util.GetRowVal(row, $"{ColPrefix}site_name");
            var locType = (short)Util.GetRowVal(row, $"{ColPrefix}site_type_id");
            RetVal.SiteType = (OM_Site.SiteTypeEnum)locType;
            RetVal.Active = (short)Util.GetRowVal(row, $"{ColPrefix}active") == 1;
            return RetVal;
        }

    }
}
