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
    public static class Property_Removal_LocationSvc
    {
        static Property_Removal_LocationSvc()
        {
            Util.RegisterOracle();
        }
        internal const string TABLE_NAME = "TOLIVE.Property_Removal_Location";

        public static async Task<Property_Removal_Location> GetAsync(int Property_Removal_Location_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Property_Removal_Location_Id = :Property_Removal_Location_Id");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("Property_Removal_Location_Id", Property_Removal_Location_Id);
            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();

            Property_Removal_Location retVal = null;
            if (rdr.HasRows)
            {
                await rdr.ReadAsync();
                retVal = DataRowToObject(rdr);
            }

            return retVal;
        }


        public static async Task<List<Property_Removal_Location>> GetAllAsync()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);

            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();
            List<Property_Removal_Location> retVal = [];

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
            //sql.AppendLine(SqlBuilder.GetColumnsForSelect(typeof(Property_Removal_Location), "<TABLE_ALIAS>", "<TABLE_PREFIX>"));
            //sql.AppendLine($"FROM {TABLE_NAME} <TABLE_ALIAS>");
            //Add Join Here

            //If just selecting this table
            sql.AppendLine(SqlBuilder.GetSelect(typeof(Property_Removal_Location), TABLE_NAME));

            return sql.ToString();
        }


        public static async Task<int> InsertAsync(Property_Removal_Location obj)
        {            
            return await SqlBuilder.InsertAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);
        }


        public static async Task<int> InsertAsync(Property_Removal_Location obj, OracleConnection conn)
        {            
            return await SqlBuilder.InsertAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }

        //No Updates or Deletes as the primary key is the name.  If we update/delete, then all records need to be updated in property_removal_form

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0017:Simplify object initialization")]
        internal static Property_Removal_Location DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Property_Removal_Location RetVal = new();
            RetVal.Plant_Area = (string)Util.GetRowVal(row, $"{ColPrefix}plant_area");
            RetVal.Active = (string)Util.GetRowVal(row, $"{ColPrefix}active");
            return RetVal;
        }

    }
}
