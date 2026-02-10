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

    /// <summary>
    /// Used to get Key/Value for Analytics from Analytics_Key_Value table
    /// </summary>
    public static class Analytics_Key_ValueSvc
    {
        static Analytics_Key_ValueSvc()
        {
            Util.RegisterOracle();
        }


        public static Analytics_Key_Value Get(string KeyName)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE key = :key");


            Analytics_Key_Value retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("key", KeyName);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }


        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT key, last_update, value");
            sql.AppendLine("FROM tolive.analytics_key_value");
            return sql.ToString();
        }


        internal static Analytics_Key_Value DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Analytics_Key_Value RetVal = new();
            RetVal.Key = (string)Util.GetRowVal(row, $"{ColPrefix}key");
            RetVal.Last_Update = (DateTime)Util.GetRowVal(row, $"{ColPrefix}last_update");
            RetVal.Value = (string)Util.GetRowVal(row, $"{ColPrefix}value");
            return RetVal;
        }

    }
}
