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
    public static class Property_Removal_Line_ItemSvc
    {
        static Property_Removal_Line_ItemSvc()
        {
            Util.RegisterOracle();
        }
        internal const string TABLE_NAME = "TOLIVE.Property_Removal_Line_Item";



        public static async Task<List<Property_Removal_Line_Item>> GetByFormNbrAsync(int FormNumber)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            //Add Filter here if needed
            sql.AppendLine($"WHERE Form_Nbr = :FormNumber");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("FormNumber", FormNumber);

            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();
            List<Property_Removal_Line_Item> retVal = [];

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

            sql.AppendLine(SqlBuilder.GetSelect(typeof(Property_Removal_Line_Item), TABLE_NAME));

            return sql.ToString();
        }


        public static async Task<int> InsertAsync(Property_Removal_Line_Item obj)
        {
            return await SqlBuilder.InsertAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);
        }


        public static async Task<int> InsertAsync(Property_Removal_Line_Item obj, OracleConnection conn)
        {            
            return await SqlBuilder.InsertAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }



        public static async Task<int> UpdateAsync(Property_Removal_Line_Item obj)
        {
            return await SqlBuilder.UpdateAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);
        }

        public static async Task<int> UpdateAsync(Property_Removal_Line_Item obj, OracleConnection conn)
        {
            return await SqlBuilder.UpdateAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }

        public static async Task<int> DeleteAsync(Property_Removal_Line_Item obj)
        {
            return await SqlBuilder.DeleteAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);
        }


        public static async Task<int> DeleteAsync(Property_Removal_Line_Item obj, OracleConnection conn)
        {
            return await SqlBuilder.DeleteAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }

        /// <summary>
        /// Deletes all items for a form
        /// </summary>
        /// <param name="Form"></param>
        /// <param name="LineItemStarting"></param>
        /// <returns>Number of items deleted</returns>
        /// <remarks>Use this to delete prior to reinserting </remarks>
        public static async Task<int> DeleteAllFormItemsAsync(Property_Removal_Form Form, OracleConnection conn)
        {
            string sql = $"DELETE FROM {TABLE_NAME} WHERE form_nbr = {Form.Form_Nbr}";
            OracleCommand cmd = new OracleCommand(sql, conn);
            int retVal = await cmd.ExecuteNonQueryAsync();
            return retVal;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0017:Simplify object initialization")]
        internal static Property_Removal_Line_Item DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Property_Removal_Line_Item RetVal = new();
            RetVal.Form_Nbr = (decimal)Util.GetRowVal(row, $"{ColPrefix}form_nbr");
            RetVal.Line_Item_Nbr = Convert.ToInt16((decimal)Util.GetRowVal(row, $"{ColPrefix}line_item_nbr"));
            RetVal.Quantity = (decimal?)Util.GetRowVal(row, $"{ColPrefix}quantity");
            RetVal.Line_Desc = (string)Util.GetRowVal(row, $"{ColPrefix}line_desc");
            RetVal.Est_Return_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}est_return_date");
            RetVal.Po_Req_Mr_Pt_Rz = (string)Util.GetRowVal(row, $"{ColPrefix}po_req_mr_pt_rz");
            RetVal.Line_Comment = (string)Util.GetRowVal(row, $"{ColPrefix}line_comment");
            RetVal.Req_Nbr = (decimal?)Util.GetRowVal(row, $"{ColPrefix}req_nbr");
            RetVal.Line_Release = (string)Util.GetRowVal(row, $"{ColPrefix}line_release");
            return RetVal;
        }

    }
}
