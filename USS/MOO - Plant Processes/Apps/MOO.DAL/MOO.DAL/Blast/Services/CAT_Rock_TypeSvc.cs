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
    public static class CAT_Rock_TypeSvc
    {
        static CAT_Rock_TypeSvc()
        {
            Util.RegisterOracle();
        }

        public static CAT_Rock_Type Get(int Rock_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE rt.id = :id");

            CAT_Rock_Type retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("id", Rock_Id);
            cmd.BindByName = true;

            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr, "rt_");
            }
            conn.Close();
            return retVal;
        }

        public static CAT_Rock_Type GetName(string RT_Name)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE rt.rt_name = :rt_name");

            CAT_Rock_Type retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("rt_name", RT_Name);
            cmd.BindByName = true;

            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr, "rt_");
            }
            conn.Close();
            return retVal;
        }

        public static List<CAT_Rock_Type> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            List<CAT_Rock_Type> retVal = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;

            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    retVal.Add(DataRowToObject(rdr, "rt_"));
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
            cols.AppendLine($"{ta}id {ColPrefix}id, {ta}rt_name {ColPrefix}rt_name");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns("rt", "rt_"));
            sql.AppendLine("FROM blast.cat_rock_type rt");
            return sql.ToString();
        }

        internal static CAT_Rock_Type DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            CAT_Rock_Type RetVal = new();
            RetVal.Id = (int)(decimal)Util.GetRowVal(row, $"{ColPrefix}id");
            RetVal.RT_Name = (string)Util.GetRowVal(row, $"{ColPrefix}rt_name");
            return RetVal;
        }

    }
}
