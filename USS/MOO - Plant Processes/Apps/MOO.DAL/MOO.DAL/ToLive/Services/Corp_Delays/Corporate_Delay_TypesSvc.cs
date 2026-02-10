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
    /// Service for getting Corporate_Delay_Types records
    /// </summary>
    /// <remarks>Note: There should be no Insert/Update/Delete functions for this.  Records need to manually be added to this table as this is a definition table</remarks>
    public static class Corporate_Delay_TypesSvc
    {
        static Corporate_Delay_TypesSvc()
        {
            Util.RegisterOracle();
        }


        public static Corporate_Delay_Types Get(string delay_type_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE delay_type_id = :delay_type_id");


            Corporate_Delay_Types retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("delay_type_id", delay_type_id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }

        public static List<Corporate_Delay_Types> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            List<Corporate_Delay_Types> elements = new();
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
            cols.AppendLine($"{ta}delay_type_id {ColPrefix}delay_type_id, {ta}delay_description {ColPrefix}delay_description, ");
            cols.AppendLine($"{ta}delay_grouping {ColPrefix}delay_grouping, {ta}plant {ColPrefix}plant, {ta}area {ColPrefix}area, ");
            cols.AppendLine($"{ta}operating_unit {ColPrefix}operating_unit, {ta}eqp_category {ColPrefix}eqp_category, ");
            cols.AppendLine($"{ta}op_sys {ColPrefix}op_sys, {ta}equipment {ColPrefix}equipment, {ta}reason {ColPrefix}reason, ");
            cols.AppendLine($"{ta}detail {ColPrefix}detail, {ta}timed_delay_minutes {ColPrefix}timed_delay_minutes, ");
            cols.AppendLine($"{ta}facility {ColPrefix}facility, {ta}equipment_affected {ColPrefix}equipment_affected, ");
            cols.AppendLine($"{ta}component {ColPrefix}component, {ta}mark_complete {ColPrefix}mark_complete, ");
            cols.AppendLine($"{ta}mno_code {ColPrefix}mno_code, {ta}mno_equip_id {ColPrefix}mno_equip_id, ");
            cols.AppendLine($"{ta}proc_end_of_shift {ColPrefix}proc_end_of_shift, {ta}split_new_shift {ColPrefix}split_new_shift, ");
            cols.AppendLine($"{ta}delay_active {ColPrefix}delay_active");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.corporate_delay_types");
            return sql.ToString();
        }
        

        internal static Corporate_Delay_Types DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Corporate_Delay_Types RetVal = new();
            RetVal.Delay_Type_Id = (string)Util.GetRowVal(row, $"{ColPrefix}delay_type_id");
            RetVal.Delay_Description = (string)Util.GetRowVal(row, $"{ColPrefix}delay_description");
            RetVal.Delay_Grouping = (string)Util.GetRowVal(row, $"{ColPrefix}delay_grouping");
            RetVal.Plant = Enum.Parse<MOO.Plant>( ((string)Util.GetRowVal(row, $"{ColPrefix}plant")),true);
            RetVal.Area = (decimal?)Util.GetRowVal(row, $"{ColPrefix}area");
            RetVal.Operating_Unit = (decimal?)Util.GetRowVal(row, $"{ColPrefix}operating_unit");
            RetVal.Eqp_Category = (decimal?)Util.GetRowVal(row, $"{ColPrefix}eqp_category");
            RetVal.Op_Sys = (decimal?)Util.GetRowVal(row, $"{ColPrefix}op_sys");
            RetVal.Equipment = (decimal?)Util.GetRowVal(row, $"{ColPrefix}equipment");
            RetVal.Reason = (decimal?)Util.GetRowVal(row, $"{ColPrefix}reason");
            RetVal.Detail = (decimal?)Util.GetRowVal(row, $"{ColPrefix}detail");
            RetVal.Timed_Delay_Minutes = (decimal)Util.GetRowVal(row, $"{ColPrefix}timed_delay_minutes");
            RetVal.Facility = (decimal?)Util.GetRowVal(row, $"{ColPrefix}facility");
            RetVal.Equipment_Affected = (decimal?)Util.GetRowVal(row, $"{ColPrefix}equipment_affected");
            RetVal.Component = (decimal?)Util.GetRowVal(row, $"{ColPrefix}component");
            RetVal.Mark_Complete = (decimal)Util.GetRowVal(row, $"{ColPrefix}mark_complete") ==1;
            RetVal.Proc_End_Of_Shift = (decimal)Util.GetRowVal(row, $"{ColPrefix}proc_end_of_shift")==1;
            RetVal.Split_New_Shift = (decimal)Util.GetRowVal(row, $"{ColPrefix}split_new_shift") == 1;
            RetVal.Delay_Active = (decimal)Util.GetRowVal(row, $"{ColPrefix}delay_active") == 1;
            return RetVal;
        }

    }
}
