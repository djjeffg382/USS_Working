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
    public static class Lineup_MeetingSvc
    {
        private static object enumVal;

        static Lineup_MeetingSvc()
        {
            Util.RegisterOracle();
        }


        public static Lineup_Meeting Get(DateTime LineupDate, string LineupName, string LineupKey)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            sql.AppendLine("WHERE lineup_date = :lineup_date");
            sql.AppendLine("AND lineup_name = :lineup_name ");
            sql.AppendLine("AND key = :lineup_key ");


            Lineup_Meeting retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("lineup_date", LineupDate);
            cmd.Parameters.Add("lineup_name", LineupName);
            cmd.Parameters.Add("lineup_key", LineupKey);
            cmd.BindByName = true;
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }


        /// <summary>
        /// Gets the lineup meeting list of items
        /// </summary>
        /// <param name="lineupDate"></param>
        /// <param name="lineupName"></param>
        /// <returns></returns>
        public static List<Lineup_Meeting> GetList<T>(DateTime lineupDate) where T : System.Enum
        {
            List<Lineup_Meeting> retVal = [];

            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE lineup_date = :lineup_date");
            sql.AppendLine("AND lineup_name = :lineup_name");


            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("lineup_date", lineupDate.Date);
            string lineupName = typeof(T).ToString();
            //get the lineup name enum without the dots
            lineupName = lineupName.Substring(lineupName.LastIndexOf('.') + 1);
            cmd.Parameters.Add("lineup_name", lineupName);
            cmd.BindByName = true;
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    if (Enum.TryParse(typeof(T), rdr["key"].ToString(), true, out enumVal))
                    {
                        var a = (T)enumVal;
                        retVal.Add(DataRowToObject(rdr));
                    }

                }
            }
            conn.Close();
            return retVal;
        }


        /// <summary>
        /// Gets the lineup meeting key/value for a given data and lineupname
        /// </summary>
        /// <param name="lineupDate"></param>
        /// <returns></returns>
        public static Dictionary<T, Lineup_Meeting> GetDictionary<T>(DateTime lineupDate) where T : System.Enum
        {
            Dictionary<T, Lineup_Meeting> retVal = [];

            List<Lineup_Meeting> listVals = GetList<T>(lineupDate);

            foreach (var val in listVals)
            {
                if (Enum.TryParse(typeof(T), val.Key, true, out enumVal))
                {
                    var a = (T)enumVal;
                    retVal.Add(a, val);
                }
            }

            string lineupName = typeof(T).ToString();
            //get the lineup name enum without the dots
            lineupName = lineupName.Substring(lineupName.LastIndexOf('.') + 1);
            //now loop through and add any keys that we don't have yet
            foreach (var val in Enum.GetValues(typeof(T)).Cast<T>())
            {
                if (!retVal.ContainsKey(val))
                    retVal.Add(val, new Lineup_Meeting()
                    {
                        Lineup_Date = lineupDate,
                        Key = val.ToString(),
                        Value = "",
                        Lineup_Name = lineupName
                    });
            }
            return retVal;
        }


        /// <summary>
        /// Gets an empty initialized Dictionary of possible LineUp Items
        /// </summary>
        /// <param name="lineupDate"></param>
        /// <returns>This can be used so so we don't have a null object before we are able to get data in the UI</returns>
        public static Dictionary<T, Lineup_Meeting> GetInitializedDictionary<T>(DateTime lineupDate) where T : System.Enum
        {
            Dictionary<T, Lineup_Meeting> retVal = [];


            string lineupName = typeof(T).ToString();
            //get the lineup name enum without the dots
            lineupName = lineupName.Substring(lineupName.LastIndexOf('.') + 1);
            //now loop through and add any keys that we don't have yet
            foreach (var val in Enum.GetValues(typeof(T)).Cast<T>())
            {
                if (!retVal.ContainsKey(val))
                    retVal.Add(val, new Lineup_Meeting()
                    {
                        Lineup_Date = lineupDate,
                        Key = val.ToString(),
                        Value = "",
                        Lineup_Name = lineupName
                    });
            }
            return retVal;
        }


        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT lineup_date, lineup_name, key, value ");
            sql.AppendLine("FROM tolive.lineup_meeting");
            return sql.ToString();
        }




        private static int Insert(Lineup_Meeting obj, OracleConnection conn)
        {
            

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Lineup_Meeting(");
            sql.AppendLine("lineup_date, lineup_name, key, value)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":lineup_date, :lineup_name, :lineup_key, :lineup_value)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("lineup_date", obj.Lineup_Date.Date);
            ins.Parameters.Add("lineup_name", obj.Lineup_Name);
            ins.Parameters.Add("lineup_key", obj.Key);
            ins.Parameters.Add("lineup_value", obj.Value);
            ins.BindByName = true;
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        /// <summary>
        /// Updates/Inserts the KeyValue in the database
        /// </summary>
        /// <param name="lineupDate"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Upsert(Lineup_Meeting obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Lineup_Meeting SET");
            sql.AppendLine("value = :lineup_value");
            sql.AppendLine("WHERE lineup_date = :lineup_date");
            sql.AppendLine("AND lineup_name = :lineup_name ");
            sql.AppendLine("AND key = :lineup_key ");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.Parameters.Add("lineup_value", obj.Value);
            upd.Parameters.Add("lineup_date", obj.Lineup_Date.Date);
            upd.Parameters.Add("lineup_name", obj.Lineup_Name);
            upd.Parameters.Add("lineup_key",obj.Key);

            upd.BindByName = true;
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            if (recsAffected== 0)
            {
                Insert(obj, conn);
            }
            conn.Close();
            return recsAffected;
        }




        internal static Lineup_Meeting DataRowToObject(DbDataReader row)
        {
            Lineup_Meeting RetVal = new();
            RetVal.Lineup_Date = (DateTime)Util.GetRowVal(row, "lineup_date");
            RetVal.Lineup_Name = (string)Util.GetRowVal(row, "lineup_name");
            RetVal.Key = (string)Util.GetRowVal(row, "key");
            RetVal.Value = (string)Util.GetRowVal(row, "value");
            return RetVal;
        }

    }
}
