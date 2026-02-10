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
    public static class IT_System_LogSvc
    {
        static IT_System_LogSvc()
        {
            Util.RegisterOracle();
        }
        internal const string TABLE_NAME = "tolive.IT_System_Log";

        public static IT_System_Log Get(int ITLogId)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE isl.IT_Log_Id = :ITLogId");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("ITLogId", ITLogId);
            conn.Open();
            var rdr = cmd.ExecuteReader();

            IT_System_Log retVal = null;
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr, "isl_");
            }

            return retVal;
        }


        public static List<IT_System_Log> GetBySystem(int ItSystemId)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE its.IT_System_Id = :ItSystemId");
            sql.AppendLine("ORDER BY isl.logdate desc, isl.entered_date desc");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("ItSystemId", ItSystemId);

            conn.Open();
            var rdr = cmd.ExecuteReader();
            List<IT_System_Log> retVal = [];

            while (rdr.Read())
            {
                var obj = DataRowToObject(rdr, "isl_");
                retVal.Add(obj);
            }
            return retVal;
        }



        public static string GetSelect()
        {
            StringBuilder sql = new();
            sql.Append("SELECT ");
            sql.AppendLine(SqlBuilder.GetColumnsForSelect(typeof(IT_System_Log), "isl", "isl_"));
            sql.AppendLine("," + SqlBuilder.GetColumnsForSelect(typeof(IT_System), "its", "its_"));
            sql.AppendLine($"FROM {TABLE_NAME} isl");
            sql.AppendLine($"JOIN {IT_SystemSvc.TABLE_NAME} its");
            sql.AppendLine("    ON isl.it_system_id = its.it_system_id");


            return sql.ToString();
        }


        public static int Insert(IT_System_Log obj)
        {
            if (obj.It_Log_Id <= 0)
                obj.It_Log_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.SEQ_IT_System"));

            return SqlBuilder.Insert(obj, Data.MNODatabase.DMART, TABLE_NAME);

        }


        public static int Insert(IT_System_Log obj, OracleConnection conn)
        {
            if (obj.It_Log_Id <= 0)
                obj.It_Log_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.SEQ_IT_System"));

            return SqlBuilder.Insert(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }



        public static int Update(IT_System_Log obj)
        {
            return SqlBuilder.Update(obj, Data.MNODatabase.DMART, TABLE_NAME);
        }

        public static int Update(IT_System_Log obj, OracleConnection conn)
        {
            return SqlBuilder.Update(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }

        public static int Delete(IT_System_Log obj)
        {
            return SqlBuilder.Delete(obj, Data.MNODatabase.DMART, TABLE_NAME);
        }


        public static int Delete(IT_System_Log obj, OracleConnection conn)
        {
            return SqlBuilder.Delete(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0017:Simplify object initialization")]
        internal static IT_System_Log DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            IT_System_Log RetVal = new();
            RetVal.It_Log_Id = (int)Util.GetRowVal(row, $"{ColPrefix}it_log_id");
            RetVal.It_System = IT_SystemSvc.DataRowToObject(row, "its_");
            RetVal.ChangedBy = (string)Util.GetRowVal(row, $"{ColPrefix}changedby");
            RetVal.LogDate = (DateTime)Util.GetRowVal(row, $"{ColPrefix}logdate");
            RetVal.LogChange = (string)Util.GetRowVal(row, $"{ColPrefix}logchange");
            RetVal.ChangeSet = (string)Util.GetRowVal(row, $"{ColPrefix}changeset");
            RetVal.RequestedBy = (string)Util.GetRowVal(row, $"{ColPrefix}RequestedBy");
            RetVal.Version = (string)Util.GetRowVal(row, $"{ColPrefix}Version");
            RetVal.Entered_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}Entered_Date");
            return RetVal;
        }

    }
}
