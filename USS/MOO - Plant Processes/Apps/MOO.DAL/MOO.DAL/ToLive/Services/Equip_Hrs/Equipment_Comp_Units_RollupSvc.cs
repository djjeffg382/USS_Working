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
    /// Class used for obtaining data from Equipment_Comp_Units_Rollup
    /// </summary>
    /// <remarks>No insert/update/delete will be provided as this is handled through triggers on the Equipent_Unit_Daily_Totals</remarks>
    public static class Equipment_Comp_Units_RollupSvc
    {
        static Equipment_Comp_Units_RollupSvc()
        {
            Util.RegisterOracle();
        }

        public static List<Equipment_Comp_Units_Rollup> GetEquipCompUnit(string equipId, short componentId, EqUnitTypes unitType, DateTime startDate, DateTime endDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE ecr.equip_id = :equip_id");
            sql.AppendLine("AND ecr.component_id = :componentId");
            sql.AppendLine("AND ecr.unit_type = :unitType");
            sql.AppendLine("AND ecr.rollup_date BETWEEN :startDate AND :endDate");

            List<Equipment_Comp_Units_Rollup> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("equip_id", equipId);
            cmd.Parameters.Add("componentId", componentId);
            cmd.Parameters.Add("unitType", (short)unitType);
            cmd.Parameters.Add("startDate", startDate);
            cmd.Parameters.Add("endDate", endDate);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    elements.Add(DataRowToObject(rdr,"ecr_"));
                }
            }
            conn.Close();
            return elements;
        }

        /// <summary>
        /// Gets the last equipment_comp_units_rollup record prior to the specified date
        /// </summary>
        /// <param name="equipId">Equipment Id</param>
        /// <param name="componentId">Component Id</param>
        /// <param name="unitType">Equipment Unit Type</param>
        /// <param name="equipDate">The date we will want the record prior to this date</param>
        /// <returns></returns>
        public static Equipment_Comp_Units_Rollup GetLatestValue(string equipId, short componentId, EqUnitTypes unitType, DateTime equipDate)
        {
            StringBuilder sql = new();
            sql.AppendLine("WITH LastRec AS (SELECT equip_id, component_id, unit_type, rollup_date,");
            sql.AppendLine("                        ROW_NUMBER() OVER(ORDER BY rollup_date desc) rn");
            sql.AppendLine("                    FROM tolive.equipment_comp_units_rollup");
            sql.AppendLine("                    WHERE equip_id = :equip_id");
            sql.AppendLine("                    AND component_id = :componentId");
            sql.AppendLine("                    AND unit_type = :unitType");
            sql.AppendLine("                    AND rollup_date < :equipDate");
            sql.AppendLine(")");
            sql.Append(GetSelect());
            //join in the the LasRec sub query getting the top record which should be the latest
            sql.AppendLine("JOIN LastRec lr");
            sql.AppendLine("ON ecr.equip_id = lr.equip_id AND ecr.component_id = lr.component_id");
            sql.AppendLine("    AND ecr.unit_type = lr.unit_type AND ecr.rollup_date = lr.rollup_date");
            sql.AppendLine("    AND lr.rn = 1");




            Equipment_Comp_Units_Rollup retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("equip_id", equipId);
            cmd.Parameters.Add("componentId", componentId);
            cmd.Parameters.Add("unitType", (short)unitType);
            cmd.Parameters.Add("equipDate", equipDate);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr, "ecr_");
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
            cols.AppendLine($"{ta}equip_id {ColPrefix}equip_id, {ta}component_id {ColPrefix}component_id, ");
            cols.AppendLine($"{ta}unit_type {ColPrefix}unit_type, {ta}rollup_date {ColPrefix}rollup_date, ");
            cols.AppendLine($"ROUND({ta}daily_total,4) {ColPrefix}daily_total, ROUND({ta}weekly_total,4) {ColPrefix}weekly_total, ");
            cols.AppendLine($"ROUND({ta}monthly_total,4) {ColPrefix}monthly_total, ROUND({ta}ytd_units,4) {ColPrefix}ytd_units, ");
            cols.AppendLine($"ROUND({ta}ltd_units,4) {ColPrefix}ltd_units, ROUND({ta}inspection_units,4) {ColPrefix}inspection_units, ");
            cols.AppendLine($"{ta}active_status {ColPrefix}active_status");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns("ecr", "ecr_") + ",");
            sql.AppendLine(Equipment_MasterSvc.GetColumns("em", "em_") + ",");
            sql.AppendLine(Equipment_Component_MasterSvc.GetColumns("ecm", "ecm_") + ",");
            sql.AppendLine(Equipment_Unit_TypesSvc.GetColumns("et", "et_"));
            sql.AppendLine("FROM tolive.equipment_comp_units_rollup ecr");
            sql.AppendLine("JOIN tolive.equipment_master em ON em.equip_id = ecr.equip_id");
            sql.AppendLine("JOIN tolive.equipment_component_master ecm ON ecm.equip_id = ecr.equip_id AND ecm.component_id = ecr.component_id");
            sql.AppendLine("JOIN tolive.equipment_unit_types et ON ecr.unit_type = et.unit_type");
            return sql.ToString();
        }



        internal static Equipment_Comp_Units_Rollup DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Equipment_Comp_Units_Rollup RetVal = new();
            RetVal.Rollup_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}rollup_date");
            RetVal.Daily_Total = (decimal?)Util.GetRowVal(row, $"{ColPrefix}daily_total");
            RetVal.Weekly_Total = (decimal?)Util.GetRowVal(row, $"{ColPrefix}weekly_total");
            RetVal.Monthly_Total = (decimal?)Util.GetRowVal(row, $"{ColPrefix}monthly_total");
            RetVal.Ytd_Units = (decimal?)Util.GetRowVal(row, $"{ColPrefix}ytd_units");
            RetVal.Ltd_Units = (decimal?)Util.GetRowVal(row, $"{ColPrefix}ltd_units");
            RetVal.Inspection_Units = (decimal?)Util.GetRowVal(row, $"{ColPrefix}inspection_units");
            RetVal.Active_Status = (short)Util.GetRowVal(row, $"{ColPrefix}active_status") == 1;



            RetVal.EquipmentComponent = Equipment_Component_MasterSvc.DataRowToObject(row, "ecm_");
            RetVal.UnitType = Equipment_Unit_TypesSvc.DataRowToObject(row, "et_");


            return RetVal;
        }

    }
}
