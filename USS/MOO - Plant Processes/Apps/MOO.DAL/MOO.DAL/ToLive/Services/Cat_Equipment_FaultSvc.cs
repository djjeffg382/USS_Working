using MOO.DAL.Drill.Models;
using MOO.DAL.ToLive.Models;
using Newtonsoft.Json.Converters;
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
    /// Service for handling the data coming from VisionLink API
    /// </summary>
    /// <remarks>Should not need an update/delete function as we will always just be inserting.  The delete function will just be a purge function</remarks>
    public static class Cat_Equipment_FaultSvc
    {
        static Cat_Equipment_FaultSvc()
        {
            Util.RegisterOracle();
        }



        public static List<Cat_Equipment_Fault> GetByDate(string Eqp_Serial, DateTime StartDate, DateTime EndDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE eqp_serial = :eqp_serial");
            sql.AppendLine("AND fault_date BETWEEN :startdate AND :enddate");

            List<Cat_Equipment_Fault> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("eqp_serial", Eqp_Serial);
            cmd.Parameters.Add("startdate", StartDate);
            cmd.Parameters.Add("enddate", EndDate);
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
            cols.AppendLine($"{ta}fault_date {ColPrefix}fault_date, ");
            cols.AppendLine($"{ta}eqp_model {ColPrefix}eqp_model, {ta}eqp_id {ColPrefix}eqp_id, ");
            cols.AppendLine($"{ta}eqp_serial {ColPrefix}eqp_serial, {ta}wenco_equip_ident {ColPrefix}wenco_equip_ident, ");
            cols.AppendLine($"{ta}codeidentifier {ColPrefix}codeidentifier, {ta}codedescription {ColPrefix}codedescription, ");
            cols.AppendLine($"{ta}codeseverity {ColPrefix}codeseverity, {ta}codesource {ColPrefix}codesource,");
            cols.AppendLine($"{ta}plant {ColPrefix}plant");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.cat_equipment_fault");
            return sql.ToString();
        }


        public static int Insert(Cat_Equipment_Fault obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Cat_Equipment_Fault obj, OracleConnection conn)
        {
            

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO ToLive.Cat_Equipment_Fault(");
            sql.AppendLine("fault_date, eqp_model, eqp_id, eqp_serial, wenco_equip_ident, codeidentifier, ");
            sql.AppendLine("codedescription, codeseverity, codesource, plant)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":fault_date, :eqp_model, :eqp_id, :eqp_serial, :wenco_equip_ident, ");
            sql.AppendLine(":codeidentifier, :codedescription, :codeseverity, :codesource, :plant)");
            OracleCommand ins = new(sql.ToString(), conn) { BindByName = true };
            ins.Parameters.Add("fault_date", obj.Fault_Date);
            ins.Parameters.Add("eqp_model", obj.Eqp_Model);
            ins.Parameters.Add("eqp_id", obj.Eqp_Id);
            ins.Parameters.Add("eqp_serial", obj.Eqp_Serial);
            ins.Parameters.Add("wenco_equip_ident", obj.Wenco_Equip_Ident);
            ins.Parameters.Add("codeidentifier", obj.CodeIdentifier);
            ins.Parameters.Add("codedescription", obj.CodeDescription);
            ins.Parameters.Add("codeseverity", obj.CodeSeverity);
            ins.Parameters.Add("codesource", obj.CodeSource);
            ins.Parameters.Add("plant", obj.Plant.HasValue ? obj.Plant.ToString() : DBNull.Value);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }




        public static int Purge(DateTime DeletePriorTo)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM ToLive.Cat_Equipment_Fault");
            sql.AppendLine("WHERE fault_date < :DeleteDate");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand del = new(sql.ToString(), conn) { BindByName = true };
            del.Parameters.Add("DeleteDate", DeletePriorTo);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            conn.Close();
            return recsAffected;
        }


        internal static Cat_Equipment_Fault DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Cat_Equipment_Fault RetVal = new();
            RetVal.Eqp_Model = (string)Util.GetRowVal(row, $"{ColPrefix}eqp_model");
            RetVal.Eqp_Id = (string)Util.GetRowVal(row, $"{ColPrefix}eqp_id");
            RetVal.Eqp_Serial = (string)Util.GetRowVal(row, $"{ColPrefix}eqp_serial");

            if (Enum.TryParse<MOO.Plant>((string)Util.GetRowVal(row, $"{ColPrefix}plant"), false, out MOO.Plant tmpPlant))
                RetVal.Plant = tmpPlant;
            else
                RetVal.Plant = null;

            RetVal.Wenco_Equip_Ident = (string)Util.GetRowVal(row, $"{ColPrefix}wenco_equip_ident");

            RetVal.Fault_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}fault_date");
            RetVal.CodeIdentifier = RetVal.Eqp_Id = (string)Util.GetRowVal(row, $"{ColPrefix}CodeIdentifier");
            RetVal.CodeDescription = RetVal.Eqp_Id = (string)Util.GetRowVal(row, $"{ColPrefix}CodeDescription");
            RetVal.CodeSeverity = RetVal.Eqp_Id = (string)Util.GetRowVal(row, $"{ColPrefix}CodeSeverity");
            RetVal.CodeSource = RetVal.Eqp_Id = (string)Util.GetRowVal(row, $"{ColPrefix}CodeSource");
            return RetVal;
        }

    }
}
