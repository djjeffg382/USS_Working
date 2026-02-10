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
    public static class Speed_LossSvc
    {
        static Speed_LossSvc()
        {
            Util.RegisterOracle();
        }
        internal const string TABLE_NAME = "TOLIVE.Speed_Loss";

        public static async Task<Speed_Loss> GetAsync(Guid Speed_Loss_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Speed_Loss_Id = :Speed_Loss_Id");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("Speed_Loss_Id", Speed_Loss_Id.ToByteArray());
            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();

            Speed_Loss retVal = null;
            if (rdr.HasRows)
            {
                await rdr.ReadAsync();
                retVal = DataRowToObject(rdr);
            }

            return retVal;
        }


        public static async Task<List<Speed_Loss>> GetByArea(MOO.Plant Plant, string Area)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            //Add Filter here if needed
            sql.AppendLine($"WHERE Plant = :Plant");
            sql.AppendLine($"AND Area = :Area");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("Plant", Plant.ToString());
            cmd.Parameters.Add("Area", Area);

            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();
            List<Speed_Loss> retVal = [];

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
            sql.AppendLine(SqlBuilder.GetSelect(typeof(Speed_Loss), TABLE_NAME));

            return sql.ToString();
        }


        public static async Task<int> InsertAsync(Speed_Loss obj)
        {
            if (obj.Speed_Loss_Id.Equals(Guid.Empty))
                obj.Speed_Loss_Id = Guid.NewGuid();

            return await SqlBuilder.InsertAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);

        }


        public static async Task<int> InsertAsync(Speed_Loss obj, OracleConnection conn)
        {
            if (obj.Speed_Loss_Id.Equals(Guid.Empty))
                obj.Speed_Loss_Id = Guid.NewGuid();

            return await SqlBuilder.InsertAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }



        public static async Task<int> UpdateAsync(Speed_Loss obj)
        {
            return await SqlBuilder.UpdateAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);
        }

        public static async Task<int> UpdateAsync(Speed_Loss obj, OracleConnection conn)
        {
            return await SqlBuilder.UpdateAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }

        public static async Task<int> DeleteAsync(Speed_Loss obj)
        {
            return await SqlBuilder.DeleteAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);
        }


        public static async Task<int> DeleteAsync(Speed_Loss obj, OracleConnection conn)
        {
            return await SqlBuilder.DeleteAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0017:Simplify object initialization")]
        internal static Speed_Loss DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Speed_Loss RetVal = new();
            RetVal.Speed_Loss_Id = new Guid((System.Byte[])Util.GetRowVal(row, $"{ColPrefix}speed_loss_id"));
            RetVal.Plant = Enum.Parse<MOO.Plant>( (string)Util.GetRowVal(row, $"{ColPrefix}plant"));
            RetVal.Area = (string)Util.GetRowVal(row, $"{ColPrefix}area");
            RetVal.Line = (string)Util.GetRowVal(row, $"{ColPrefix}Line");
            RetVal.Start_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}start_date");
            RetVal.End_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}end_date");
            RetVal.Avg_Ltph = (double?)Util.GetRowVal(row, $"{ColPrefix}Avg_Ltph");
            RetVal.Loss_Tons = (double?)Util.GetRowVal(row, $"{ColPrefix}Loss_Tons");
            RetVal.Reason = (string)Util.GetRowVal(row, $"{ColPrefix}reason");
            RetVal.Comments = (string)Util.GetRowVal(row, $"{ColPrefix}comments");
            return RetVal;
        }

    }
}
