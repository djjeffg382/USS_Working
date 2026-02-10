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
    public static class Tire_Maint_ReasonSvc
    {
        static Tire_Maint_ReasonSvc()
        {
            Util.RegisterOracle();
        }


        public static Tire_Maint_Reason Get(int tire_maint_reason_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE tire_maint_reason_id = :tire_maint_reason_id");


            Tire_Maint_Reason retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("tire_maint_reason_id", tire_maint_reason_id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }

        public static List<Tire_Maint_Reason> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            List<Tire_Maint_Reason> elements = [];
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
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
            cols.AppendLine($"{ta}tire_maint_reason_id {ColPrefix}tire_maint_reason_id, {ta}reason_name {ColPrefix}reason_name");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.tire_maint_reason");
            return sql.ToString();
        }

        internal static Tire_Maint_Reason DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Tire_Maint_Reason RetVal = new();
            RetVal.Tire_Maint_Reason_Id = (int)(short)Util.GetRowVal(row, $"{ColPrefix}tire_maint_reason_id");
            RetVal.Reason_Name = (Enums.TireMaintReason)Enum.Parse(typeof(Enums.TireMaintReason),(string)Util.GetRowVal(row, $"{ColPrefix}reason_name"));
            return RetVal;
        }

    }
}
