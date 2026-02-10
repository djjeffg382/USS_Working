using MOO.DAL.Core.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Core.Services
{
    public static class Collection_TimeSvc
    {
        static Collection_TimeSvc()
        {
            Util.RegisterOracle();
        }


        public static Collection_Time Get(decimal Coll_Time_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE coll_time_id = :coll_time_id");


            Collection_Time retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("coll_time_id", Coll_Time_Id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }

        public static List<Collection_Time> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            List<Collection_Time> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
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
            cols.AppendLine($"{ta}coll_time_id {ColPrefix}coll_time_id, {ta}coll_time_name {ColPrefix}coll_time_name, ");
            cols.AppendLine($"{ta}intervalminutes {ColPrefix}intervalminutes");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM core.collection_time");
            return sql.ToString();
        }



        internal static Collection_Time DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Collection_Time RetVal = new();
            RetVal.Coll_Time_Id = (int)(decimal)Util.GetRowVal(row, $"{ColPrefix}coll_time_id");
            RetVal.Coll_Time_Name = (string)Util.GetRowVal(row, $"{ColPrefix}coll_time_name");
            RetVal.Intervalminutes = (decimal?)Util.GetRowVal(row, $"{ColPrefix}intervalminutes");
            return RetVal;
        }

    }
}
