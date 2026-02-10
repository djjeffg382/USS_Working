using Microsoft.Data.SqlClient;
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
    public static class IT_SystemSvc
    {
        static IT_SystemSvc()
        {
            Util.RegisterOracle();
        }
        internal const string TABLE_NAME = "tolive.IT_System";



        public static IT_System Get(int ITSystemId)
        {
            StringBuilder sql = new();
            sql.Append(SqlBuilder.GetSelect(typeof(IT_System), TABLE_NAME));
            sql.AppendLine($"WHERE IT_System_Id = :ITSystemId");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("ITSystemId", ITSystemId);
            conn.Open();
            var rdr = cmd.ExecuteReader();

            IT_System retVal = null;
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }

            return retVal;
        }

        public static List<IT_System> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(SqlBuilder.GetSelect(typeof(IT_System), TABLE_NAME));
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            conn.Open();
            var rdr = cmd.ExecuteReader();
            List<IT_System> retVal = [];

            while (rdr.Read())
            {
                var obj = DataRowToObject(rdr);
                retVal.Add(obj);
            }
            return retVal;
        }


        public static int Insert(IT_System obj)
        {
            if (obj.It_System_Id <= 0)
                obj.It_System_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.SEQ_IT_System"));

            return SqlBuilder.Insert(obj, Data.MNODatabase.DMART, TABLE_NAME);

        }


        public static int Insert(IT_System obj, OracleConnection conn)
        {
            if (obj.It_System_Id <= 0)
                obj.It_System_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.SEQ_IT_System"));

            return SqlBuilder.Insert(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }



        public static int Update(IT_System obj)
        {
            return SqlBuilder.Update(obj, Data.MNODatabase.DMART, TABLE_NAME);
        }

        public static int Update(IT_System obj, OracleConnection conn)
        {
            return SqlBuilder.Update(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }

        public static int Delete(IT_System obj)
        {
            return SqlBuilder.Delete(obj, Data.MNODatabase.DMART, TABLE_NAME);
        }


        public static int Delete(IT_System obj, OracleConnection conn)
        {
            return SqlBuilder.Delete(obj,conn,Data.DBType.Oracle,TABLE_NAME);
        }



        internal static IT_System DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            IT_System RetVal = new();
            RetVal.It_System_Id = (int)Util.GetRowVal(row, $"{ColPrefix}it_system_id");
            RetVal.System_Name = (string)Util.GetRowVal(row, $"{ColPrefix}system_name");
            RetVal.Description = (string)Util.GetRowVal(row, $"{ColPrefix}Description");

            if (Enum.TryParse<IT_System.ItSystemType>((string)Util.GetRowVal(row, $"{ColPrefix}system_type"), out IT_System.ItSystemType sysType))
                RetVal.System_Type = sysType;
            RetVal.Other_Documents = (string)Util.GetRowVal(row, $"{ColPrefix}other_documents");
            RetVal.Running_On = (string)Util.GetRowVal(row, $"{ColPrefix}running_on");
            RetVal.Graylog_App_Name = (string)Util.GetRowVal(row, $"{ColPrefix}Graylog_App_Name");
            RetVal.DotNetVersion = (string)Util.GetRowVal(row, $"{ColPrefix}DotNetVersion");
            RetVal.Notes = (string)Util.GetRowVal(row, $"{ColPrefix}Notes");
            RetVal.Active = (short)Util.GetRowVal(row, $"{ColPrefix}active")==1;
            return RetVal;
        }
    }
}
