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
    public static class UomSvc
    {
        static UomSvc()
        {
            Util.RegisterOracle();
        }


        public static Uom Get(int Uom_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE uom_id = :Uom_id");


            Uom retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("uom_id", Uom_id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }

        public static List<Uom> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            List<Uom> elements = new();
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
            cols.AppendLine($"{ta}uom_id {ColPrefix}uom_id, {ta}uom_name {ColPrefix}uom_name, ");
            cols.AppendLine($"{ta}uom_abbreviation {ColPrefix}uom_abbreviation");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM core.uom");
            return sql.ToString();
        }



        internal static Uom DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Uom RetVal = new();
            RetVal.Uom_Id = (int)(decimal)Util.GetRowVal(row, $"{ColPrefix}uom_id");
            RetVal.Uom_Name = (string)Util.GetRowVal(row, $"{ColPrefix}uom_name");
            RetVal.Uom_Abbreviation = (string)Util.GetRowVal(row, $"{ColPrefix}uom_abbreviation");
            return RetVal;
        }

    }
}
