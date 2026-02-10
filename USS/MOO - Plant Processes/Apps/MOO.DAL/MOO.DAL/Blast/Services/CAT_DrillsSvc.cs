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
    public static class CAT_DrillsSvc
    {
        static CAT_DrillsSvc()
        {
            Util.RegisterOracle();
        }

        public static CAT_Drills Get(int Drill_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE cd.id = :id");

            CAT_Drills retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("id", Drill_Id);
            cmd.BindByName = true;

            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr, "cd_");
            }
            conn.Close();
            return retVal;
        }

        public static List<CAT_Drills> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            List<CAT_Drills> retVal = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;

            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    retVal.Add(DataRowToObject(rdr, "cd_"));
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
            cols.AppendLine($"{ta}id {ColPrefix}id, {ta}num {ColPrefix}num, ");
            cols.AppendLine($"{ta}status {ColPrefix}status ");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns("cd", "cd_"));
            sql.AppendLine("FROM blast.cat_drills cd");
            return sql.ToString();
        }

        internal static CAT_Drills DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            CAT_Drills RetVal = new();
            RetVal.Id = (int)(decimal)Util.GetRowVal(row, $"{ColPrefix}id");
            RetVal.Num = (string)Util.GetRowVal(row, $"{ColPrefix}num");
            RetVal.Status = (decimal)Util.GetRowVal(row, $"{ColPrefix}status") == 1;
            return RetVal;
        }

    }
}
