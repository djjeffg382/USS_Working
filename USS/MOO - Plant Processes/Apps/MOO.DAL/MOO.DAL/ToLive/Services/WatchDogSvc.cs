using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.DAL.ToLive.Models;
using System.Data;

namespace MOO.DAL.ToLive.Services
{
    public class WatchDogSvc
    {

        private const string ORA_LAST_RUN = "{OraLastRun}";
        private const string SQL_LAST_RUN = "{SQLLastRun}";
        static WatchDogSvc()
        {
            Util.RegisterOracle();
        }


        public static WatchDog Get(string wdname)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE wdname = {wdname}");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }

        public static List<WatchDog> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());


            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);

            List<WatchDog> retVal = new();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                retVal.Add(DataRowToObject(dr));
            }

            return retVal;
        }



        private static string GetSelect(string addField = "")
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine("wdname, active, connectionname, command, emailwebmaster, emaillist, runtimes, ");
            sql.AppendLine("lastrun, failures, emailnote");
            if (!string.IsNullOrEmpty(addField))
            {
                sql.Append(", ");
                sql.AppendLine(addField);
            }
            sql.AppendLine("FROM tolive.watchdog");
            return sql.ToString();
        }



        public static void SetActive(WatchDog Wdog, bool Active)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE TOLIVE.WatchDog SET");
            sql.AppendLine("active = :active");
            sql.AppendLine("WHERE wdname = :wdname");


            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand upd = new(sql.ToString(), conn);
            upd.Parameters.Add("active", Active ? 1 : 0);
            upd.Parameters.Add("wdname", Wdog.WDName);
            conn.Open();
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            conn.Close();
        }

        public static void UpdateLastRun(WatchDog Wdog, DateTime LastRun)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE TOLIVE.WatchDog SET");
            sql.AppendLine("LastRun = :LastRun");
            sql.AppendLine("WHERE wdname = :wdname");


            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand upd = new(sql.ToString(), conn);
            upd.Parameters.Add("LastRun", LastRun);
            upd.Parameters.Add("wdname", Wdog.WDName);
            conn.Open();
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            conn.Close();
        }

        /// <summary>
        /// runs the SQL Command in the Command field of the WatchDog
        /// </summary>
        /// <param name="Wdog">The watchdog object to run</param>
        /// <param name="SetLastRun">If true, will update the last run value after running the command</param>
        /// <returns></returns>
        public static DataSet RunCommand(WatchDog Wdog, bool SetLastRun = false)
        {
            string sql = Wdog.Command.Replace(SQL_LAST_RUN, Wdog.LastRun.GetValueOrDefault(DateTime.Now).ToString("MM/dd/yyyy HH:mm:ss"))
                        .Replace(ORA_LAST_RUN, MOO.Dates.OraDate(Wdog.LastRun.GetValueOrDefault(DateTime.Now)));
            if (Enum.TryParse<MOO.Data.MNODatabase>(Wdog.ConnectionName, out Data.MNODatabase db))
            {
                DateTime lastRun = DateTime.Now;
                DataSet ds = MOO.Data.ExecuteQuery(sql, db);
                if (SetLastRun)
                    UpdateLastRun(Wdog, lastRun);
                return ds;
            }
            else
                return null;

        }

        private static WatchDog DataRowToObject(DataRow row)
        {
            WatchDog RetVal = new();
            RetVal.WDName = row.Field<string>("wdname");
            RetVal.Active = row.Field<decimal>("active") == 1;
            RetVal.ConnectionName = row.Field<string>("connectionname");
            RetVal.Command = row.Field<string>("command");
            RetVal.EmailWebmaster = row.Field<decimal>("emailwebmaster") == 1;
            RetVal.EmailList = row.Field<string>("emaillist");
            RetVal.Runtimes = row.Field<string>("runtimes");
            RetVal.LastRun = row.Field<DateTime?>("lastrun");
            RetVal.Failures = row.Field<decimal?>("failures");
            RetVal.EmailNote = row.Field<string>("emailnote");
            return RetVal;
        }
    }
}
