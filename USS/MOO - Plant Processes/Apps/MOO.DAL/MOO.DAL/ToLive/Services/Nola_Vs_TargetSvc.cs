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
    public static class Nola_Vs_TargetSvc
    {
        static Nola_Vs_TargetSvc()
        {
            Util.RegisterOracle();
        }
        internal const string TABLE_NAME = "TOLIVE.Nola_Vs_Target";

        public static async Task<Nola_Vs_Target> GetAsync(DateTime TheDate, short StepNumber)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE thedate = :TheDate");
            sql.AppendLine("AND step = :StepNumber");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("TheDate", TheDate);
            cmd.Parameters.Add("StepNumber", StepNumber);
            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();

            Nola_Vs_Target retVal = null;
            if (rdr.HasRows)
            {
                await rdr.ReadAsync();
                retVal = DataRowToObject(rdr);
            }

            return retVal;
        }


        public static async Task<List<Nola_Vs_Target>> GetAllAsync()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);

            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();
            List<Nola_Vs_Target> retVal = [];

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
            
            sql.AppendLine(SqlBuilder.GetSelect(typeof(Nola_Vs_Target), TABLE_NAME));

            return sql.ToString();
        }


        public static async Task<int> InsertAsync(Nola_Vs_Target obj)
        {

            return await SqlBuilder.InsertAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);

        }


        public static async Task<int> InsertAsync(Nola_Vs_Target obj, OracleConnection conn)
        {
            
            return await SqlBuilder.InsertAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }



        public static async Task<int> UpdateAsync(Nola_Vs_Target obj)
        {
            
            return await SqlBuilder.UpdateAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);
        }

        public static async Task<int> UpdateAsync(Nola_Vs_Target obj, OracleConnection conn)
        {
            return await SqlBuilder.UpdateAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }

        public static async Task<int> DeleteAsync(Nola_Vs_Target obj)
        {
            return await SqlBuilder.DeleteAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);
        }


        public static async Task<int> DeleteAsync(Nola_Vs_Target obj, OracleConnection conn)
        {
            return await SqlBuilder.DeleteAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0017:Simplify object initialization")]
        internal static Nola_Vs_Target DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Nola_Vs_Target RetVal = new();
            RetVal.TheDate = (DateTime)Util.GetRowVal(row, $"{ColPrefix}thedate");
            RetVal.Step =(short)(decimal)Util.GetRowVal(row, $"{ColPrefix}step");
            RetVal.Is_Out_Of_Control = (decimal)Util.GetRowVal(row, $"{ColPrefix}is_out_of_control") == 1;
            RetVal.Nola_Reliable = (decimal)Util.GetRowVal(row, $"{ColPrefix}nola_reliable") == 1;
            RetVal.Conc_Checked = (decimal)Util.GetRowVal(row, $"{ColPrefix}conc_checked") == 1;
            RetVal.Conc_Response_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}conc_response_date");
            RetVal.Conc_Responded_By = (string)Util.GetRowVal(row, $"{ColPrefix}conc_responded_by");
            RetVal.Conc_Comments = (string)Util.GetRowVal(row, $"{ColPrefix}conc_comments");
            RetVal.Solved_At_Conc = (decimal)Util.GetRowVal(row, $"{ColPrefix}solved_at_conc") == 1;
            RetVal.Sent_To_Mine_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}sent_to_mine_date");
            RetVal.Mine_Response_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}mine_response_date");
            RetVal.Mine_Responded_By = (string)Util.GetRowVal(row, $"{ColPrefix}mine_responded_by");
            RetVal.Mine_Actions = (string)Util.GetRowVal(row, $"{ColPrefix}mine_actions");
            RetVal.Solved_At_Mine = (decimal)Util.GetRowVal(row, $"{ColPrefix}solved_at_mine") == 1;
            RetVal.Mgmt_Review_By = (string)Util.GetRowVal(row, $"{ColPrefix}mgmt_review_by");
            RetVal.Mgmt_Review_Comments = (string)Util.GetRowVal(row, $"{ColPrefix}mgmt_review_comments");
            RetVal.Mgmt_Review_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}mgmt_review_date");
            return RetVal;
        }

    }
}
