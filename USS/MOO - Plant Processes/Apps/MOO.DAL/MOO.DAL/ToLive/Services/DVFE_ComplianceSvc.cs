using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;

namespace MOO.DAL.ToLive.Services
{
    public static class DVFE_ComplianceSvc
    {
        static DVFE_ComplianceSvc()
        {
            Util.RegisterOracle();
        }

        public static DVFE_Compliance Get(long dvfe_key)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE dvfe_key = :dvfe_key");


            DVFE_Compliance retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("dvfe_key", dvfe_key);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }

        public static List<DVFE_Compliance> GetAll(DateTime startDate, DateTime endDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE registerdate BETWEEN :start_date AND :end_date");

            List<DVFE_Compliance> elements = [];
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("start_date", startDate);
            cmd.Parameters.Add("end_date", endDate);

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
            cols.AppendLine($"{ta}dvfe_key {ColPrefix}dvfe_key, {ta}registerdate {ColPrefix}registerdate, ");
            cols.AppendLine($"{ta}facility_id {ColPrefix}facility_id, {ta}plant_id {ColPrefix}plant_id, ");
            cols.AppendLine($"{ta}webloginname {ColPrefix}webloginname, {ta}ntusername {ColPrefix}ntusername, ");
            cols.AppendLine($"{ta}changedfromip {ColPrefix}changedfromip, {ta}registerdatefull {ColPrefix}registerdatefull");
            return cols.ToString();
        }

        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.dvfe_compliance");
            return sql.ToString();
        }


        public static int Insert(DVFE_Compliance obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(DVFE_Compliance obj, OracleConnection conn)
        {
            if (obj.DVFE_Key <= 0)
            {
                string sqlId = "SELECT NVL(MAX(dvfe_key),0) FROM tolive.dvfe_compliance";
                int nextId = Convert.ToInt32((decimal)MOO.Data.ExecuteScalar(sqlId, Data.MNODatabase.DMART)) + 1;
                obj.DVFE_Key = nextId;
            }

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.dvfe_compliance(");
            sql.AppendLine("dvfe_key, registerdate, facility_id, plant_id, webloginname, ntusername, ");
            sql.AppendLine("changedfromip, registerdatefull)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":dvfe_key, :registerdate, :facility_id, :plant_id, :webloginname, :ntusername, ");
            sql.AppendLine(":changedfromip, :registerdatefull)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("dvfe_key", obj.DVFE_Key);
            ins.Parameters.Add("registerdate", obj.RegisterDate);
            ins.Parameters.Add("facility_id", (int)obj.Facility_Id);
            ins.Parameters.Add("plant_id", (int)obj.Plant_Id);
            ins.Parameters.Add("webloginname", obj.WebLoginName);
            ins.Parameters.Add("ntusername", obj.NTUserName);
            ins.Parameters.Add("changedfromip", obj.ChangedFromIp);
            ins.Parameters.Add("registerdatefull", obj.RegisterDateFull);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }

        internal static DVFE_Compliance DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            DVFE_Compliance RetVal = new();
            RetVal.DVFE_Key = (long)Util.GetRowVal(row, $"{ColPrefix}dvfe_key");
            RetVal.RegisterDate = (DateTime)Util.GetRowVal(row, $"{ColPrefix}registerdate");
            RetVal.Facility_Id = (DVFE_Compliance.Facility)Enum.Parse(typeof(DVFE_Compliance.Facility),Util.GetRowVal(row, $"{ColPrefix}facility_id").ToString());
            RetVal.Plant_Id = (DVFE_Compliance.Plant)Enum.Parse(typeof(DVFE_Compliance.Plant),Util.GetRowVal(row, $"{ColPrefix}plant_id").ToString());
            RetVal.WebLoginName = (string)Util.GetRowVal(row, $"{ColPrefix}webloginname");
            RetVal.NTUserName = (string)Util.GetRowVal(row, $"{ColPrefix}ntusername");
            RetVal.ChangedFromIp = (string)Util.GetRowVal(row, $"{ColPrefix}changedfromip");
            RetVal.RegisterDateFull = (DateTime)Util.GetRowVal(row, $"{ColPrefix}registerdatefull");
            return RetVal;
        }
    }
}
