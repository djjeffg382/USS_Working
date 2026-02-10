using MOO.DAL.ToLive.Enums;
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
    /// Service for obtaining Equipment_Unit_Types
    /// </summary>
    /// <remarks>We will eliminate Insert, Update, Delete as it will be EXTREMELY rare to add/remove any of these.  Any insert/update/deletes should be done directly on the database</remarks>
    public static class Equipment_Unit_TypesSvc
    {
        static Equipment_Unit_TypesSvc()
        {
            Util.RegisterOracle();
        }


        public static Equipment_Unit_Types Get(decimal unit_type)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE unit_type = :unit_type");


            Equipment_Unit_Types retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("unit_type", unit_type);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }

        public static List<Equipment_Unit_Types> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            List<Equipment_Unit_Types> elements = new();
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
            cols.AppendLine($"{ta}unit_type {ColPrefix}unit_type, {ta}unit_desc {ColPrefix}unit_desc, ");
            cols.AppendLine($"{ta}unit_comment {ColPrefix}unit_comment");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.equipment_unit_types");
            return sql.ToString();
        }




        internal static Equipment_Unit_Types DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Equipment_Unit_Types RetVal = new();
            RetVal.Unit_Type = Convert.ToInt16( (decimal)Util.GetRowVal(row, $"{ColPrefix}unit_type"));
            RetVal.Unit_Desc = (string)Util.GetRowVal(row, $"{ColPrefix}unit_desc");
            RetVal.Unit_Comment = (string)Util.GetRowVal(row, $"{ColPrefix}unit_comment");
            RetVal.UnitType = Enum.Parse<EqUnitTypes>(RetVal.Unit_Desc.Replace(' ', '_'), true);
            return RetVal;
        }

    }
}
