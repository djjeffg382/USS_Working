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
    public static class Prop_Rmv_FormSvc
    {
        static Prop_Rmv_FormSvc()
        {
            Util.RegisterOracle();
        }
        internal const string TABLE_NAME = "TOLIVE.Prop_Rmv_Form";

        public static async Task<Prop_Rmv_Form> GetAsync(int Prop_Rmv_Form_Id)
        {
            List<Sec_Users> users = Sec_UserSvc.GetAll();
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Prop_Rmv_Form_Id = :Prop_Rmv_Form_Id");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("Prop_Rmv_Form_Id", Prop_Rmv_Form_Id);
            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();

            Prop_Rmv_Form retVal = null;
            if (rdr.HasRows)
            {
                await rdr.ReadAsync();
                retVal = DataRowToObject(rdr, users);
            }

            return retVal;
        }

        /// <summary>
        /// Gets all forms between supplied dates
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static async Task<List<Prop_Rmv_Form>> GetAllAsync(DateTime StartDate, DateTime EndDate)
        {
            List<Sec_Users> users = Sec_UserSvc.GetAll();
            StringBuilder sql = new();
            sql.Append(GetSelect());
            //Add Filter here if needed
            sql.AppendLine($"WHERE Created_Date BETWEEN :StartDate AND :EndDate");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("StartDate", StartDate);
            cmd.Parameters.Add("EndDate", EndDate);

            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();
            List<Prop_Rmv_Form> retVal = [];

            while (await rdr.ReadAsync())
            {
                var obj = DataRowToObject(rdr, users);
                retVal.Add(obj);
            }
            return retVal;
        }

        /// <summary>
        /// Gets a list of forms by creator
        /// </summary>
        /// <param name="Creator"></param>
        /// <returns></returns>
        public static async Task<List<Prop_Rmv_Form>> GetByCreatorAsync(Sec_Users Creator)
        {
            List<Sec_Users> users = Sec_UserSvc.GetAll();
            StringBuilder sql = new();
            sql.Append(GetSelect());
            //Add Filter here if needed
            sql.AppendLine($"WHERE Created_By_Id = :Creator");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("Creator", Creator.User_Id);

            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();
            List<Prop_Rmv_Form> retVal = [];

            while (await rdr.ReadAsync())
            {
                var obj = DataRowToObject(rdr, users);
                retVal.Add(obj);
            }
            return retVal;
        }


        /// <summary>
        /// Gets a list of forms that the user can/has approved
        /// </summary>
        /// <param name="Approver"></param>
        /// <returns></returns>
        public static async Task<List<Prop_Rmv_Form>> GetUserApproveAsync(Sec_Users Approver)
        {
            List<Sec_Users> users = Sec_UserSvc.GetAll();
            StringBuilder sql = new();
            sql.Append(GetSelect());
            //Add Filter here if needed
            sql.AppendLine($"WHERE prop_rmv_form_id IN (");
            sql.AppendLine("    SELECT prop_rmv_form_id");
            sql.AppendLine("FROM tolive.prop_rmv_form prf");
            sql.AppendLine("JOIN tolive.Prop_Rmv_reason prr");
            sql.AppendLine("    ON prf.Prop_Rmv_reason_id = prr.Prop_Rmv_reason_id");
            sql.AppendLine("LEFT JOIN tolive.Prop_Rmv_Reason_Approver prra");
            sql.AppendLine("    ON prr.prop_rmv_reason_id = prra.prop_rmv_reason_id");
            sql.AppendLine("    AND prr.req_special_approval = 1");
            sql.AppendLine("    AND prra.sec_user_id = :user_id");
            sql.AppendLine("JOIN tolive.Prop_Rmv_area pra");
            sql.AppendLine("    ON prf.Prop_Rmv_area_id = pra.Prop_Rmv_area_id");
            sql.AppendLine("LEFT JOIN tolive.Prop_Rmv_Area_Approver praa");
            sql.AppendLine("    ON pra.Prop_Rmv_Area_Id = praa.Prop_Rmv_Area_Id");
            sql.AppendLine("    AND prr.req_special_approval = 0");
            sql.AppendLine("    AND praa.sec_user_id = :user_id");
            sql.AppendLine(")");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("user_id", Approver.User_Id);

            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();
            List<Prop_Rmv_Form> retVal = [];

            while (await rdr.ReadAsync())
            {
                var obj = DataRowToObject(rdr, users);
                retVal.Add(obj);
            }
            return retVal;
        }



        public static string GetSelect()
        {
            StringBuilder sql = new();
            sql.Append("SELECT ");
            sql.AppendLine(SqlBuilder.GetColumnsForSelect(typeof(Prop_Rmv_Form), "prf", "prf_") + ",");
            sql.AppendLine(SqlBuilder.GetColumnsForSelect(typeof(Prop_Rmv_Area), "pra", "pra_") + ",");
            sql.AppendLine(SqlBuilder.GetColumnsForSelect(typeof(Prop_Rmv_Reason), "prr", "prr_") + ",");
            sql.AppendLine(SqlBuilder.GetColumnsForSelect(typeof(Prop_Rmv_Vendor), "prv", "prv_"));
            sql.AppendLine($"FROM {TABLE_NAME} prf");
            sql.AppendLine("JOIN tolive.Prop_Rmv_area pra");
            sql.AppendLine("    ON prf.Prop_Rmv_area_id = pra.Prop_Rmv_area_id");
            sql.AppendLine("JOIN tolive.Prop_Rmv_reason prr");
            sql.AppendLine("    ON prf.Prop_Rmv_reason_id = prr.Prop_Rmv_reason_id");
            sql.AppendLine("JOIN tolive.Prop_Rmv_vendor prv");
            sql.AppendLine("    ON prf.Prop_Rmv_vendor_id = prv.Prop_Rmv_vendor_id");
            


            return sql.ToString();
        }


        public static async Task<int> InsertAsync(Prop_Rmv_Form obj)
        {
            if (obj.Prop_Rmv_Form_Id <= 0)
                obj.Prop_Rmv_Form_Id = Convert.ToInt32(await MOO.Data.GetNextSequenceAsync("TOLIVE.SEQ_PROP_RMV_FORM"));

            return await SqlBuilder.InsertAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);

        }


        public static async Task<int> InsertAsync(Prop_Rmv_Form obj, OracleConnection conn)
        {
            if (obj.Prop_Rmv_Form_Id <= 0)
                obj.Prop_Rmv_Form_Id = Convert.ToInt32(await MOO.Data.GetNextSequenceAsync("TOLIVE.SEQ_PROP_RMV_FORM"));

            return await SqlBuilder.InsertAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }



        public static async Task<int> UpdateAsync(Prop_Rmv_Form obj)
        {
            return await SqlBuilder.UpdateAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);
        }

        public static async Task<int> UpdateAsync(Prop_Rmv_Form obj, OracleConnection conn)
        {
            return await SqlBuilder.UpdateAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }

        public static async Task<int> DeleteAsync(Prop_Rmv_Form obj)
        {
            return await SqlBuilder.DeleteAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);
        }


        public static async Task<int> DeleteAsync(Prop_Rmv_Form obj, OracleConnection conn)
        {
            return await SqlBuilder.DeleteAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="UserList">The full sec_users list so it can be used to fill the user objects</param>
        /// <param name="ColPrefix"></param>
        /// <returns></returns>
        /// <remarks>Rather than joining the sec_users table for each of the user we need, it is MUCH faster to get the full list and use that for everything
        /// Therefore, whoever calls this function must give the full sec_users list</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0017:Simplify object initialization")]
        internal static Prop_Rmv_Form DataRowToObject(DbDataReader row, List<Sec_Users> UserList, string ColPrefix = "prf_")
        {
            Prop_Rmv_Form RetVal = new();
            RetVal.Prop_Rmv_Form_Id = (int)Util.GetRowVal(row, $"{ColPrefix}Prop_Rmv_form_id");
            RetVal.Area = Prop_Rmv_AreaSvc.DataRowToObject(row, "pra_");
            RetVal.Reason = Prop_Rmv_ReasonSvc.DataRowToObject(row,"prr_");
            RetVal.Vendor = Prop_Rmv_VendorSvc.DataRowToObject(row, "prv_");

            int userId = (int)Util.GetRowVal(row, $"{ColPrefix}created_by_id");

            RetVal.Created_By = UserList.FirstOrDefault(x => x.User_Id == userId);
            RetVal.Created_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}created_date");
            RetVal.Closed_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}closed_date");
            RetVal.Status = Enum.Parse<Prop_Rmv_Form.PrStatus>((string)Util.GetRowVal(row, $"{ColPrefix}status"));
            RetVal.Vendor_Contact = (string)Util.GetRowVal(row, $"{ColPrefix}vendor_contact");
            RetVal.Explain_Other = (string)Util.GetRowVal(row, $"{ColPrefix}explain_other");
            RetVal.To_Be_Returned = (short)Util.GetRowVal(row, $"{ColPrefix}to_be_returned") == 1;
            return RetVal;
        }

    }
}
