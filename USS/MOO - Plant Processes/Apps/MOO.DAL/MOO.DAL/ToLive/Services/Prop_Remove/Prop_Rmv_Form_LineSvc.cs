using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data.Common;

namespace MOO.DAL.ToLive.Services
{
    public static class Prop_Rmv_Form_LineSvc
    {
        static Prop_Rmv_Form_LineSvc()
        {
            Util.RegisterOracle();
        }
        internal const string TABLE_NAME = "TOLIVE.Prop_Rmv_Form_Line";

        public static async Task<Prop_Rmv_Form_Line> GetAsync(int Prop_Rmv_Form_Line_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Prop_Rmv_Form_Line_Id = :Prop_Rmv_Form_Line_Id");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("Prop_Rmv_Form_Line_Id", Prop_Rmv_Form_Line_Id);
            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();

            Prop_Rmv_Form_Line retVal = null;
            if (rdr.HasRows)
            {
                await rdr.ReadAsync();
                retVal = DataRowToObject(rdr);
            }

            return retVal;
        }


        public static async Task<List<Prop_Rmv_Form_Line>> GetByFormIdAsync(int FormId)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Prop_Rmv_form_id = :FormId");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("FormId", FormId);

            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();
            List<Prop_Rmv_Form_Line> retVal = [];

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
            sql.AppendLine(SqlBuilder.GetSelect(typeof(Prop_Rmv_Form_Line), TABLE_NAME));

            return sql.ToString();
        }


        public static async Task<int> InsertAsync(Prop_Rmv_Form_Line obj)
        {
            if (obj.Prop_Rmv_Form_Line_Id <= 0)
                obj.Prop_Rmv_Form_Line_Id = Convert.ToInt32(await MOO.Data.GetNextSequenceAsync("TOLIVE.SEQ_PROP_RMV_CONFIG"));

            return await SqlBuilder.InsertAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);

        }


        public static async Task<int> InsertAsync(Prop_Rmv_Form_Line obj, OracleConnection conn)
        {
            if (obj.Prop_Rmv_Form_Line_Id <= 0)
                obj.Prop_Rmv_Form_Line_Id = Convert.ToInt32(await MOO.Data.GetNextSequenceAsync("TOLIVE.SEQ_PROP_RMV_CONFIG"));

            return await SqlBuilder.InsertAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }



        public static async Task<int> UpdateAsync(Prop_Rmv_Form_Line obj)
        {
            return await SqlBuilder.UpdateAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);
        }

        public static async Task<int> UpdateAsync(Prop_Rmv_Form_Line obj, OracleConnection conn)
        {
            return await SqlBuilder.UpdateAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }

        public static async Task<int> DeleteAsync(Prop_Rmv_Form_Line obj)
        {
            return await SqlBuilder.DeleteAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);
        }


        public static async Task<int> DeleteAsync(Prop_Rmv_Form_Line obj, OracleConnection conn)
        {
            return await SqlBuilder.DeleteAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0017:Simplify object initialization")]
        internal static Prop_Rmv_Form_Line DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Prop_Rmv_Form_Line RetVal = new();
            RetVal.Prop_Rmv_Form_Line_Id = (int)Util.GetRowVal(row, $"{ColPrefix}Prop_Rmv_form_line_id");
            RetVal.Prop_Rmv_Form_Id = (int)Util.GetRowVal(row, $"{ColPrefix}Prop_Rmv_form_id");
            RetVal.Item_Nbr = (short)Util.GetRowVal(row, $"{ColPrefix}item_nbr");
            RetVal.Quantity = (double)Util.GetRowVal(row, $"{ColPrefix}quantity");
            RetVal.Line_Description = (string)Util.GetRowVal(row, $"{ColPrefix}line_description");
            RetVal.Line_Comment = (string)Util.GetRowVal(row, $"{ColPrefix}line_comment");
            RetVal.Est_Return_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}est_return_date");
            RetVal.Mr_Pr_Pt_Rz = (string)Util.GetRowVal(row, $"{ColPrefix}mr_pr_pt_rz");
            RetVal.Po_Nbr = (string)Util.GetRowVal(row, $"{ColPrefix}po_nbr");
            RetVal.Line_Release = (string)Util.GetRowVal(row, $"{ColPrefix}line_release");
            return RetVal;
        }


    }
}
