using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;

namespace MOO.DAL.ToLive.Services
{


    public class ErrorSvc
    {

        static ErrorSvc()
        {
            Util.RegisterOracle();
        }

        public enum OraErrorLogType
        {
            Error = 0,
            Warn = 1,
            Info = 2
        }
        public static PagedData<List<Error>> GetPagedData(DateTime? startDate, DateTime? endDate,
                        OraErrorLogType[] error_Type, string error_Desc, string pName, string error_Sql,
                        int offset, int totalItems, string orderBy = "error_date", string orderDirection = "ASC")
        {
            PagedData<List<Error>> retObj = new();
            StringBuilder sql = new();
            OracleDataAdapter da = new("", MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            string filter = AddFilters(startDate, endDate, error_Type, error_Desc, pName, error_Sql, da.SelectCommand);

            sql.AppendLine("SELECT * FROM (");
            sql.AppendLine("SELECT error_date, error_num, pname, error_sql, error_type,");
            sql.AppendLine("    error_desc, error_stack,");
            sql.AppendLine($"    ROW_NUMBER() OVER(ORDER BY {orderBy} {orderDirection}) rn");
            sql.AppendLine("FROM tolive.error");
            sql.AppendLine(filter);
            sql.AppendLine(") tbl");
            sql.AppendLine($"WHERE rn BETWEEN {offset} AND {offset + totalItems}");
            da.SelectCommand.CommandText = sql.ToString();
            DataSet ds = MOO.Data.ExecuteQuery(da);
           
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                retObj.Data.Add(DataRowToObject(row));
            }
            retObj.TotalRows = GetOraErrLogsCount(startDate, endDate, error_Type, error_Desc, pName, error_Sql);
            
            return retObj;

        }


        private static int GetOraErrLogsCount(DateTime? startDate, DateTime? endDate,
                        OraErrorLogType[] error_Type, string error_Desc, string pName, string error_Sql)
        {
            StringBuilder sql = new();
            OracleDataAdapter da = new("", MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));

            string filter = AddFilters(startDate, endDate, error_Type, error_Desc, pName, error_Sql, da.SelectCommand);

            sql.AppendLine("SELECT COUNT(*) FROM tolive.error");
            sql.AppendLine(filter);
            da.SelectCommand.CommandText = sql.ToString();
            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            }
            else
                return 0;
        }



        private static string AddFilters(DateTime? startDate, DateTime? endDate,
                        OraErrorLogType[] error_Type, string error_Desc, string pName, string error_Sql,
                        OracleCommand cmd)
        {
            StringBuilder filter = new();

            /*****************Start/End Date Parameter*************/
            if (startDate.HasValue && endDate.HasValue)
            {
                filter.AppendLine("WHERE error_date BETWEEN :startDate AND :endDate");
                cmd.Parameters.Add("startDate", startDate.Value);
                cmd.Parameters.Add("endDate", endDate.Value);
            }
            /*****************Error Desc Parameter*************/
            if (!string.IsNullOrEmpty(error_Desc))
            {
                if (filter.Length == 0)
                    filter.Append("WHERE ");
                else
                    filter.Append("AND ");

                filter.AppendLine("error_desc LIKE :error_dec");
                cmd.Parameters.Add("error_desc", $"%{error_Desc}%");
            }
            /*************PName Parameter*******************/
            if (!string.IsNullOrEmpty(pName))
            {
                if (filter.Length == 0)
                    filter.Append("WHERE ");
                else
                    filter.Append("AND ");

                filter.AppendLine("pName LIKE :pName");
                cmd.Parameters.Add("pName", $"%{pName}%");
            }
            /*************error_Sql Parameter*******************/
            if (!string.IsNullOrEmpty(error_Sql))
            {
                if (filter.Length == 0)
                    filter.Append("WHERE ");
                else
                    filter.Append("AND ");

                filter.AppendLine("error_Sql LIKE :error_Sql");
                cmd.Parameters.Add("error_Sql", $"%{error_Sql}%");
            }
            if (error_Type != null && error_Type.Length > 0)
            {
                string errTypeList = string.Join(", ", error_Type.Select(e => $"{(int)e}"));

                if (filter.Length == 0)
                    filter.Append("WHERE ");
                else
                    filter.Append("AND ");

                filter.AppendLine($"error_type IN ({errTypeList})");
            }

            return filter.ToString();
        }


        private static Error DataRowToObject(DataRow row)
        {
            Error retVal = new()
            {
                Error_Date = (DateTime)row["Error_Date"],
                Error_Desc = row["Error_Desc"].ToString(),
                Error_Num = Convert.ToInt32(row["Error_Num"]),
                Error_Sql = row["Error_Sql"].ToString(),
                Error_Stack = row["Error_Stack"].ToString(),
                Error_Type = Convert.ToInt32(row["Error_Type"]),
                PName = row["PName"].ToString()
            };
            return retVal;
        }
    }
}
