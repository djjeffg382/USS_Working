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
    public static class Prop_Rmv_Area_ApproverSvc
    {
        static Prop_Rmv_Area_ApproverSvc()
        {
            Util.RegisterOracle();
        }
        internal const string TABLE_NAME = "TOLIVE.Prop_Rmv_Area_Approver";
               

        public static async Task<List<Prop_Rmv_Area_Approver>> GetByArea(int AreaId)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            //Add Filter here if needed
            sql.AppendLine($"WHERE praa.Prop_Rmv_area_id = :AreaId");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("AreaId", AreaId);

            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();
            List<Prop_Rmv_Area_Approver> retVal = [];

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
            sql.Append("SELECT ");
            sql.AppendLine(SqlBuilder.GetColumnsForSelect(typeof(Prop_Rmv_Area_Approver), "praa", "praa_") + ",");
            sql.AppendLine(SqlBuilder.GetColumnsForSelect(typeof(Prop_Rmv_Area), "pra", "pra_") + ",");
            sql.AppendLine(SqlBuilder.GetColumnsForSelect(typeof(Sec_Users), "su", "su_"));
            sql.AppendLine($"FROM {TABLE_NAME} praa");
            sql.AppendLine("JOIN tolive.Prop_Rmv_area pra");
            sql.AppendLine("    ON praa.Prop_Rmv_area_id = pra.Prop_Rmv_area_id");
            sql.AppendLine("JOIN tolive.sec_users su");
            sql.AppendLine("    ON praa.sec_user_id = su.user_id");

            return sql.ToString();
        }


        /// <summary>
        /// Adds an approver to the specified Area
        /// </summary>
        /// <param name="Area"></param>
        /// <returns></returns>
        public static async Task<int> AddApproverAsync(Prop_Rmv_Area Area, Sec_Users User, OracleConnection conn)
        {
            int newId = Convert.ToInt32(await MOO.Data.GetNextSequenceAsync("TOLIVE.SEQ_PROP_RMV_CONFIG"));
            Prop_Rmv_Area_Approver approver = new()
            {
                Prop_Rmv_Area_Apv_Id = newId,
                Area = Area,
                User = User
            };

            return await SqlBuilder.InsertAsync(approver, conn, Data.DBType.Oracle, TABLE_NAME);

        }


        /// <summary>
        /// Removes the user from the approval list
        /// </summary>
        /// <param name="Area"></param>
        /// <param name="User"></param>
        /// <returns></returns>
        public static async Task<int> RemoveApproverAsync(Prop_Rmv_Area Area, Sec_Users User, OracleConnection Conn)
        {
            StringBuilder sql = new();
            sql.AppendLine($"DELETE FROM {TABLE_NAME}");
            sql.AppendLine("WHERE Prop_Rmv_area_id = :AreaId");
            sql.AppendLine("AND sec_user_id = :UserId");

            OracleCommand cmd = new(sql.ToString(), Conn);
            cmd.Parameters.Add("AreaId", Area.Prop_Rmv_Area_Id);
            cmd.Parameters.Add("UserId", User.User_Id);
            var retVal = await cmd.ExecuteNonQueryAsync();
            return retVal;
        }


        /// <summary>
        /// Removes all users for a specified reason
        /// </summary>
        /// <param name="Reason"></param>
        /// <returns></returns>
        public static async Task<int> RemoveAllApproversAsync(Prop_Rmv_Area Area, OracleConnection Conn)
        {
            StringBuilder sql = new();
            sql.AppendLine($"DELETE FROM {TABLE_NAME}");
            sql.AppendLine("WHERE Prop_Rmv_area_id = :ReasonId");

            OracleCommand cmd = new(sql.ToString(), Conn);
            cmd.Parameters.Add("ReasonId", Area.Prop_Rmv_Area_Id);
            var retVal = await cmd.ExecuteNonQueryAsync();
            return retVal;
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0017:Simplify object initialization")]
        internal static Prop_Rmv_Area_Approver DataRowToObject(DbDataReader row, string ColPrefix = "praa_")
        {
            Prop_Rmv_Area_Approver RetVal = new();
            RetVal.Prop_Rmv_Area_Apv_Id = (int)Util.GetRowVal(row, $"{ColPrefix}Prop_Rmv_area_apv_id");
            RetVal.Area = Prop_Rmv_AreaSvc.DataRowToObject(row, "pra_");
            RetVal.User = Sec_UserSvc.DataRowToObject(row, "su_");
            return RetVal;
        }

    }
}
