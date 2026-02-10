using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Services
{
    public static class FormsSessionIdSvc
    {
        static FormsSessionIdSvc()
        {
            Util.RegisterOracle();
        }
        internal const string TABLE_NAME = "tolive.FormsSessionId";


        /// <summary>
        /// Checks for an existing session and will create one if doesn't exist yet
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public static FormsSessionId ProcureSession(string UserName)
        {
            string usrNameLookup = UserName.ToUpper();
            if (UserName.Contains('\\'))
            {
                //the domain is being passed so split it to get the user
                //example hdq\usr123
                usrNameLookup = UserName.Split('\\')[1].ToUpper();
            }
                

            var sessId = GetByUsername(usrNameLookup);
            if (sessId == null)
            {
                sessId = new FormsSessionId()
                {
                    Session_Id = GenerateRandomCode(24),
                    WebLoginName = usrNameLookup,
                    NtUserName = usrNameLookup,
                    ChangedFromIp = "0.0.0.0",
                    Value = "0",
                    RecordDate = DateTime.Now
                };
                Insert(sessId);
            }
            else
            {
                sessId.RecordDate = DateTime.Now;
                Update(sessId);
            }
            return sessId;
        }


        public static FormsSessionId GetByUsername(string UserName)
        {
            StringBuilder sql = new();
            sql.Append(SqlBuilder.GetSelect(typeof(FormsSessionId), TABLE_NAME));
            sql.AppendLine($"WHERE upper(ntusername) = upper(:UserName)");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("UserName", UserName);
            conn.Open();
            var rdr = cmd.ExecuteReader();

            FormsSessionId retVal = null;
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }

            return retVal;
        }

        public static List<FormsSessionId> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(SqlBuilder.GetSelect(typeof(FormsSessionId), TABLE_NAME));
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            conn.Open();
            var rdr = cmd.ExecuteReader();
            List<FormsSessionId> retVal = [];

            while (rdr.Read())
            {
                var obj = DataRowToObject(rdr);
                retVal.Add(obj);
            }
            return retVal;
        }


        public static int Insert(FormsSessionId obj)
        {           
            return SqlBuilder.Insert(obj, Data.MNODatabase.DMART, TABLE_NAME);
        }


        public static int Insert(FormsSessionId obj, OracleConnection conn)
        {
            return SqlBuilder.Insert(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }



        public static int Update(FormsSessionId obj)
        {
            return SqlBuilder.Update(obj, Data.MNODatabase.DMART, TABLE_NAME);
        }

        public static int Update(FormsSessionId obj, OracleConnection conn)
        {
            return SqlBuilder.Update(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }

        public static int Delete(FormsSessionId obj)
        {
            return SqlBuilder.Delete(obj, Data.MNODatabase.DMART, TABLE_NAME);
        }


        public static int Delete(FormsSessionId obj, OracleConnection conn)
        {
            return SqlBuilder.Delete(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }



        internal static FormsSessionId DataRowToObject(DbDataReader row)
        {
            FormsSessionId RetVal = new();
            RetVal.Session_Id = (string)Util.GetRowVal(row,"Session_Id");
            RetVal.WebLoginName = (string)Util.GetRowVal(row, "WebLoginName");
            RetVal.NtUserName = (string)Util.GetRowVal(row, "NtUserName");
            RetVal.ChangedFromIp = (string)Util.GetRowVal(row, "ChangedFromIp");
            RetVal.Value = (string)Util.GetRowVal(row, "Value");
            RetVal.RecordDate = (DateTime)Util.GetRowVal(row, "RecordDate");

            return RetVal;
        }

        private static string GenerateRandomCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            byte[] randomBytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            StringBuilder result = new StringBuilder(length);
            foreach (byte b in randomBytes)
            {
                result.Append(chars[b % chars.Length]);
            }

            return result.ToString();
        }
    }
}
