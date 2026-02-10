using MOO.DAL.Blast.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;

namespace MOO.DAL.Blast.Services
{
    public static class CAT_PitsSvc
    {
        static CAT_PitsSvc()
        {
            Util.RegisterOracle();
        }

        public static CAT_Pits Get(int Pit_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE cp.id = :id");

            CAT_Pits retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("id", Pit_Id);
            cmd.BindByName = true;

            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr, "cp_");
            }
            conn.Close();
            return retVal;
        }

        public static CAT_Pits GetName(string Pit_Name)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE cp.pit_name = :pit_name");

            CAT_Pits retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("pit_name", Pit_Name);
            cmd.BindByName = true;

            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr, "cp_");
            }
            conn.Close();
            return retVal;
        }

        public static List<CAT_Pits> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            List<CAT_Pits> retVal = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;

            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    retVal.Add(DataRowToObject(rdr, "cp_"));
                }
            }
            conn.Close();
            return retVal;
        }


        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}id {ColPrefix}id, {ta}pit_name {ColPrefix}pit_name");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns("cp", "cp_"));
            sql.AppendLine("FROM blast.cat_pits cp");
            return sql.ToString();
        }

        internal static CAT_Pits DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            CAT_Pits RetVal = new();
            RetVal.Id = (int)(decimal)Util.GetRowVal(row, $"{ColPrefix}id");
            RetVal.Pit_Name = (string)Util.GetRowVal(row, $"{ColPrefix}pit_name");
            return RetVal;
        }

    }
}
