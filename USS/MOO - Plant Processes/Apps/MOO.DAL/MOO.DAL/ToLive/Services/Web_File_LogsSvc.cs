using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Services
{
    public static class Web_File_LogsSvc
    {
        static Web_File_LogsSvc()
        {
            Util.RegisterOracle();
        }


        public static Web_File_Logs Get(DateTime webDate, string fileName)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE web_date = :webDate");
            sql.AppendLine("and filename = :fileName");


            Web_File_Logs retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("webDate", webDate);
            cmd.Parameters.Add("fileName", fileName);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }


        public static List<Web_File_Logs> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE web_date BETWEEN :startDate AND :endDate");

            List<Web_File_Logs> elements = [];
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("startDate", startDate);
            cmd.Parameters.Add("endDate", endDate);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    elements.Add(DataRowToObject(rdr));
                }
            }
            conn.Close();
            return elements;
        }


        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}web_date {ColPrefix}web_date, {ta}filename {ColPrefix}filename, ");
            cols.AppendLine($"{ta}view_count {ColPrefix}view_count, {ta}users {ColPrefix}users");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.web_file_logs");
            return sql.ToString();
        }


        public static int Insert(Web_File_Logs obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Web_File_Logs obj, OracleConnection conn)
        {


            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Web_File_Logs(");
            sql.AppendLine("web_date, filename, view_count, users)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":web_date, :filename, :view_count, :users)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("web_date", obj.Web_Date);
            ins.Parameters.Add("filename", obj.Filename);
            ins.Parameters.Add("view_count", obj.View_Count);
            ins.Parameters.Add("users", obj.Users);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }

        /// <summary>
        /// Deletes all logs for a specified date
        /// </summary>
        /// <param name="webDate">The date for which to delete the logs</param>
        /// <returns></returns>
        public static int DeleteAllByDate(DateTime webDate)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = DeleteAllByDate(webDate, conn);
            conn.Close();
            return recsAffected;
        }

        /// <summary>
        /// Deletes all logs for a specified date
        /// </summary>
        /// <param name="webDate">The date for which to delete the logs</param>
        /// <returns></returns>
        public static int DeleteAllByDate(DateTime webDate, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.Web_File_Logs");
            sql.AppendLine("WHERE web_date = :web_date");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("web_date", webDate);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }



        internal static Web_File_Logs DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Web_File_Logs RetVal = new();
            RetVal.Web_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}web_date");
            RetVal.Filename = (string)Util.GetRowVal(row, $"{ColPrefix}filename");
            RetVal.View_Count = Convert.ToInt32((decimal)Util.GetRowVal(row, $"{ColPrefix}view_count"));
            RetVal.Users = (string)Util.GetRowVal(row, $"{ColPrefix}users");
            return RetVal;
        }

    }
}
