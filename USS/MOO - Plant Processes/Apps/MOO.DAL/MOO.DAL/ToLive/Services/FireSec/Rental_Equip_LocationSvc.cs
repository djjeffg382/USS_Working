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
    public static class Rental_Equip_LocationSvc
    {
        static Rental_Equip_LocationSvc()
        {
            Util.RegisterOracle();
        }
        internal const string TABLE_NAME = "TOLIVE.Rental_Equip_Location";

        public static async Task<Rental_Equip_Location> GetAsync(MOO.Plant Plant, string Location)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE PlantLoc = :PlantLoc");
            sql.AppendLine($"AND Location = :Location");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("PlantLoc", Plant.ToString());
            cmd.Parameters.Add("Location", Location);
            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();

            Rental_Equip_Location retVal = null;
            if (rdr.HasRows)
            {
                await rdr.ReadAsync();
                retVal = DataRowToObject(rdr);
            }

            return retVal;
        }


        public static async Task<List<Rental_Equip_Location>> GetAllAsync()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);

            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();
            List<Rental_Equip_Location> retVal = [];

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
            sql.AppendLine(SqlBuilder.GetSelect(typeof(Rental_Equip_Location), TABLE_NAME));

            return sql.ToString();
        }

        //Insert and Delete were removed, users should not need to insert/delete any records in the application.  Any insert/deletes can be done manually


        public static async Task<int> UpdateAsync(Rental_Equip_Location obj)
        {
            return await SqlBuilder.UpdateAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);
        }

        public static async Task<int> UpdateAsync(Rental_Equip_Location obj, OracleConnection conn)
        {
            return await SqlBuilder.UpdateAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0017:Simplify object initialization")]
        internal static Rental_Equip_Location DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Rental_Equip_Location RetVal = new();
            RetVal.PlantLoc = Enum.Parse<MOO.Plant>((string)Util.GetRowVal(row, $"{ColPrefix}plantloc"));
            RetVal.Location = (string)Util.GetRowVal(row, $"{ColPrefix}location");
            RetVal.Email_List = (string)Util.GetRowVal(row, $"{ColPrefix}email_list");
            return RetVal;
        }

    }
}
