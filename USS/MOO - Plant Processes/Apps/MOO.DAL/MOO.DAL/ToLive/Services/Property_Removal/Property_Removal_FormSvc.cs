using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Services
{
    public static class Property_Removal_FormSvc
    {
        static Property_Removal_FormSvc()
        {
            Util.RegisterOracle();
        }
        internal const string TABLE_NAME = "TOLIVE.Property_Removal_Form";

        /// <summary>
        /// sequence to use for the form number
        /// </summary>
        public const string SEQUENCE_NAME = "TOLIVE.PROPERTY_REMOVAL_SEQ";



        public static async Task<Property_Removal_Form> GetAsync(int FormNumber)
        {
            List<Sec_Users> users = Sec_UserSvc.GetAll();
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Form_Nbr = :FormNumber");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("Form_Nbr", FormNumber);
            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();

            Property_Removal_Form retVal = null;
            if (rdr.HasRows)
            {
                await rdr.ReadAsync();
                retVal = DataRowToObject(rdr, users);
            }

            return retVal;
        }


        public static async Task<List<Property_Removal_Form>> GetByCreatedDateAsync(DateTime StartDate, DateTime EndDate)
        {
            List<Sec_Users> users = Sec_UserSvc.GetAll();
            StringBuilder sql = new();
            sql.Append(GetSelect());
            //Add Filter here if needed
            sql.AppendLine($"WHERE prf.created_date BETWEEN :StartDate AND :EndDate");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("StartDate", StartDate);
            cmd.Parameters.Add("EndDate", EndDate);

            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();
            List<Property_Removal_Form> retVal = [];

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
            sql.AppendLine(SqlBuilder.GetColumnsForSelect(typeof(Property_Removal_Form), "prf", "prf_") + ",");
            sql.AppendLine(SqlBuilder.GetColumnsForSelect(typeof(Property_Removal_Vendors), "v", "v_") + ",");
            sql.AppendLine(SqlBuilder.GetColumnsForSelect(typeof(Property_Removal_Reasons), "prr", "prr_"));
            sql.AppendLine($"FROM {TABLE_NAME} prf");
            sql.AppendLine($"LEFT JOIN {Property_Removal_VendorsSvc.TABLE_NAME} v");
            sql.AppendLine("ON prf.company_name = v.company_id");
            sql.AppendLine($"LEFT JOIN {Property_Removal_ReasonsSvc.TABLE_NAME} prr");
            sql.AppendLine("ON prf.removal_reason = prr.reason_id");

            return sql.ToString();
        }

        /// <summary>
        /// Items to do prior to inserting the record.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static async Task PreInsert(Property_Removal_Form obj)
        {
            if (obj.Form_Nbr <= 0)
                obj.Form_Nbr = await GetNextFormNumber();

            if (!obj.Created_Date.HasValue)
                obj.Created_Date = DateTime.Now;

            if (!obj.Last_Updated_Date.HasValue)
            {
                obj.Last_Updated_Date = DateTime.Now;
            }
            obj.Updater ??= obj.Creator;

        }

        public static async Task<int> GetNextFormNumber()
        {
            int retVal = Convert.ToInt32(await MOO.Data.GetNextSequenceAsync(SEQUENCE_NAME));
            return retVal;
        }

        /// <summary>
        /// Inserts the Property removal form with the optional line items supplied
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="PropLineItems">Optional, line items to insert as well</param>
        /// <returns></returns>
        public static async Task<int> InsertAsync(Property_Removal_Form obj)
        {
            await PreInsert(obj);

            return await SqlBuilder.InsertAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);

        }


        public static async Task<int> InsertAsync(Property_Removal_Form obj, OracleConnection conn)
        {
            await PreInsert(obj);

            return await SqlBuilder.InsertAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }



        public static async Task<int> UpdateAsync(Property_Removal_Form obj)
        {
            obj.Last_Updated_Date = DateTime.Now;
            return await SqlBuilder.UpdateAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);
        }

        public static async Task<int> UpdateAsync(Property_Removal_Form obj, OracleConnection conn)
        {
            obj.Last_Updated_Date = DateTime.Now;
            return await SqlBuilder.UpdateAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }

        public static async Task<int> DeleteAsync(Property_Removal_Form obj)
        {
            return await SqlBuilder.DeleteAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);
        }


        public static async Task<int> DeleteAsync(Property_Removal_Form obj, OracleConnection conn)
        {
            return await SqlBuilder.DeleteAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="ColPrefix"></param>
        /// <param name="SecUserList">The full sec_users list so it can be used to fill the user objects</param>
        /// <returns></returns>
        /// <remarks>Rather than joining the sec_users table for each of the user we need, it is MUCH faster to get the full list and use that for everything
        /// Therefore, whoever calls this function must give the full sec_users list</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0017:Simplify object initialization")]
        internal static Property_Removal_Form DataRowToObject(DbDataReader row,  List<Sec_Users> SecUserList, string ColPrefix = "prf_")
        {
            Property_Removal_Form RetVal = new();
            RetVal.Form_Nbr = Convert.ToInt32((decimal)Util.GetRowVal(row, $"{ColPrefix}form_nbr"));
            RetVal.Plant_Initial = (string)Util.GetRowVal(row, $"{ColPrefix}plant_initial");
            RetVal.Plant_Area = (string)Util.GetRowVal(row, $"{ColPrefix}plant_area");
            RetVal.Date_Gate = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}date_gate");
            RetVal._status = (string)Util.GetRowVal(row, $"{ColPrefix}status");
            RetVal.Explain_Other = (string)Util.GetRowVal(row, $"{ColPrefix}explain_other");
            RetVal.Created_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}created_date");
            RetVal.Last_Updated_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}last_updated_date");
            RetVal.Vendor_Contact = (string)Util.GetRowVal(row, $"{ColPrefix}vendor_contact");
            RetVal.Close_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}close_date");
            RetVal.Open_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}open_date");
            RetVal.Void_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}void_date");
            RetVal.Printed = bool.Parse((string)Util.GetRowVal(row, $"{ColPrefix}was_printed"));
            RetVal.To_Be_Replaced = (decimal?)Util.GetRowVal(row, $"{ColPrefix}to_be_replaced") == 1;

            //Vendor (Company Name)
            if (!row.IsDBNull(row.GetOrdinal("v_company_id")))
                RetVal.Vendor = Property_Removal_VendorsSvc.DataRowToObject(row, "v_");
            else
                RetVal.Vendor = null;

            //Removal Reason
            if (!row.IsDBNull(row.GetOrdinal("prr_reason_id")))
                RetVal.Prop_Removal_Reason = Property_Removal_ReasonsSvc.DataRowToObject(row, "prr_");
            else
                RetVal.Prop_Removal_Reason = null;




            //Authorizer
            string user = (string)Util.GetRowVal(row, $"{ColPrefix}authorized_by");
            if (string.IsNullOrEmpty(user))
                RetVal.Authorizer = null;
            else
                RetVal.Authorizer = SecUserList.FirstOrDefault(x => x.User_Id == int.Parse(user));


            //Tracker
            user = (string)Util.GetRowVal(row, $"{ColPrefix}tracked_by");
            if (string.IsNullOrEmpty(user))
                RetVal.Tracker = null;
            else
                RetVal.Tracker = SecUserList.FirstOrDefault(x => x.User_Id == int.Parse(user));


            //Creator
            user = (string)Util.GetRowVal(row, $"{ColPrefix}created_by");
            if (string.IsNullOrEmpty(user))
                RetVal.Creator = null;
            else
                RetVal.Creator = SecUserList.FirstOrDefault(x => x.User_Id == int.Parse(user));

            //Updater
            user = (string)Util.GetRowVal(row, $"{ColPrefix}last_updated_by");
            if (string.IsNullOrEmpty(user))
                RetVal.Updater = null;
            else
                RetVal.Updater = SecUserList.FirstOrDefault(x => x.User_Id == int.Parse(user));

            //Closer
            user = (string)Util.GetRowVal(row, $"{ColPrefix}close_by");
            if (string.IsNullOrEmpty(user))
                RetVal.Closer = null;
            else
                RetVal.Closer = SecUserList.FirstOrDefault(x => x.User_Id == int.Parse(user));


            //Opener
            user = (string)Util.GetRowVal(row, $"{ColPrefix}open_by");
            if (string.IsNullOrEmpty(user))
                RetVal.Opener = null;
            else
                RetVal.Opener = SecUserList.FirstOrDefault(x => x.User_Id == int.Parse(user));


            //Voider
            user = (string)Util.GetRowVal(row, $"{ColPrefix}void_by");
            if (string.IsNullOrEmpty(user))
                RetVal.Voider = null;
            else
                RetVal.Voider = SecUserList.FirstOrDefault(x => x.User_Id == int.Parse(user));


























            ////Authorizer
            //if (!row.IsDBNull(row.GetOrdinal("auth_user_id")))
            //    RetVal.Authorizer = Sec_UserSvc.DataRowToObject(row, "auth_");
            //else
            //    RetVal.Authorizer = null;

            ////Tracker
            //if (!row.IsDBNull(row.GetOrdinal("track_user_id")))
            //    RetVal.Tracker = Sec_UserSvc.DataRowToObject(row, "track_");
            //else
            //    RetVal.Tracker = null;


            ////Creator
            //if (!row.IsDBNull(row.GetOrdinal("creator_user_id")))
            //    RetVal.Creator = Sec_UserSvc.DataRowToObject(row, "creator_");
            //else
            //    RetVal.Creator = null;

            ////Updater
            //if (!row.IsDBNull(row.GetOrdinal("updater_user_id")))
            //    RetVal.Updater = Sec_UserSvc.DataRowToObject(row, "updater_");
            //else
            //    RetVal.Updater = null;

            ////Closer
            //if (!row.IsDBNull(row.GetOrdinal("closer_user_id")))
            //    RetVal.Closer = Sec_UserSvc.DataRowToObject(row, "closer_");
            //else
            //    RetVal.Closer = null;


            ////Opener
            //if (!row.IsDBNull(row.GetOrdinal("opener_user_id")))
            //    RetVal.Opener = Sec_UserSvc.DataRowToObject(row, "opener_");
            //else
            //    RetVal.Opener = null;


            ////Voider
            //if (!row.IsDBNull(row.GetOrdinal("voider_user_id")))
            //    RetVal.Voider = Sec_UserSvc.DataRowToObject(row, "voider_");
            //else
            //    RetVal.Voider = null;



            return RetVal;
        }

    }
}
