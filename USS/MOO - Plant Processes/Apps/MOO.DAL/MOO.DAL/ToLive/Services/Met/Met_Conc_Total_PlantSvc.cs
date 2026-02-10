using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Services
{
    public static class Met_Conc_Total_PlantSvc
    {
        static Met_Conc_Total_PlantSvc()
        {
            Util.RegisterOracle();
        }

        public static Met_Conc_Total_Plant Get(DateTime datex)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE datex = :datex");
            sql.AppendLine("AND dmy = 1");


            Met_Conc_Total_Plant retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("datex", datex);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }


        public static List<Met_Conc_Total_Plant> GetByDateRange(DateTime StartDate, DateTime EndDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE datex BETWEEN :StartDate AND :EndDate");
            sql.AppendLine("ORDER BY Datex");

            List<Met_Conc_Total_Plant> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("StartDate", StartDate);
            cmd.Parameters.Add("EndDate", EndDate);
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
            cols.AppendLine($"{ta}datex {ColPrefix}datex, {ta}dmy {ColPrefix}dmy, ");
            cols.AppendLine($"ROUND({ta}rmf_tons_natural,4) {ColPrefix}rmf_tons_natural, ROUND({ta}rmf_moisture,4) {ColPrefix}rmf_moisture, ");
            cols.AppendLine($"ROUND({ta}rmf_tons_dry,4) {ColPrefix}rmf_tons_dry, ROUND({ta}rmf_avg_ltph,4) {ColPrefix}rmf_avg_ltph, ");
            cols.AppendLine($"ROUND({ta}rmf_pct_mag_fe ,4) {ColPrefix}rmf_pct_mag_fe, ");
            cols.AppendLine($"ROUND({ta}rmf_pct_mag_fe_coils,4) {ColPrefix}rmf_pct_mag_fe_coils, ");
            cols.AppendLine($"ROUND({ta}conc_tons_nat,4) {ColPrefix}conc_tons_nat, ROUND({ta}conc_tons_dry,4) {ColPrefix}conc_tons_dry, ");
            cols.AppendLine($"ROUND({ta}conc_nat_ltph,4) {ColPrefix}conc_nat_ltph, ROUND({ta}conc_pct_sio2,4) {ColPrefix}conc_pct_sio2, ");
            cols.AppendLine($"ROUND({ta}conc_pct_mag_fe,4) {ColPrefix}conc_pct_mag_fe, ROUND({ta}conc_pct_270m,4) {ColPrefix}conc_pct_270m, ");
            cols.AppendLine($"ROUND({ta}coarse_tails_tons_dry,4) {ColPrefix}coarse_tails_tons_dry, ");
            cols.AppendLine($"ROUND({ta}fine_tails_tons_dry,4) {ColPrefix}fine_tails_tons_dry, ");
            cols.AppendLine($"ROUND({ta}coarse_tails_pct_mag_fe,4) {ColPrefix}coarse_tails_pct_mag_fe, ");
            cols.AppendLine($"ROUND({ta}fine_tails_pct_mag_fe,4) {ColPrefix}fine_tails_pct_mag_fe, ");
            cols.AppendLine($"ROUND({ta}pct_recovery_natural,4) {ColPrefix}pct_recovery_natural, ");
            cols.AppendLine($"ROUND({ta}pct_recovery_dry,4) {ColPrefix}pct_recovery_dry, ");
            cols.AppendLine($"ROUND({ta}pct_mag_fe_recovery,4) {ColPrefix}pct_mag_fe_recovery, ");
            cols.AppendLine($"ROUND({ta}scheduled_hours,4) {ColPrefix}scheduled_hours, ROUND({ta}actual_hours,4) {ColPrefix}actual_hours, ");
            cols.AppendLine($"ROUND({ta}pct_operating_time,4) {ColPrefix}pct_operating_time, ");
            cols.AppendLine($"ROUND({ta}scheduled_maint_hours,4) {ColPrefix}scheduled_maint_hours, ");
            cols.AppendLine($"ROUND({ta}pct_scheduled_maint,4) {ColPrefix}pct_scheduled_maint, ");
            cols.AppendLine($"ROUND({ta}unscheduled_maint_hours,4) {ColPrefix}unscheduled_maint_hours, ");
            cols.AppendLine($"ROUND({ta}pct_unscheduled_maint,4) {ColPrefix}pct_unscheduled_maint, ");
            cols.AppendLine($"ROUND({ta}aggl_request_hours,4) {ColPrefix}aggl_request_hours, ");
            cols.AppendLine($"ROUND({ta}pct_aggl_request,4) {ColPrefix}pct_aggl_request, ");
            cols.AppendLine($"ROUND({ta}high_power_limit_hours,4) {ColPrefix}high_power_limit_hours, ");
            cols.AppendLine($"ROUND({ta}pct_high_power_limit,4) {ColPrefix}pct_high_power_limit, ROUND({ta}no_ore_hours,4) {ColPrefix}no_ore_hours, ");
            cols.AppendLine($"ROUND({ta}pct_no_ore,4) {ColPrefix}pct_no_ore, ROUND({ta}other_delay_hours,4) {ColPrefix}other_delay_hours, ");
            cols.AppendLine($"ROUND({ta}pct_other_delay,4) {ColPrefix}pct_other_delay, ROUND({ta}avail_hours,4) {ColPrefix}avail_hours, ");
            cols.AppendLine($"ROUND({ta}pct_avail,4) {ColPrefix}pct_avail, ROUND({ta}kwh_per_ton_rmf,4) {ColPrefix}kwh_per_ton_rmf, ");
            cols.AppendLine($"ROUND({ta}rod_mill_dav_tube_sio2,4) {ColPrefix}rod_mill_dav_tube_sio2, ");
            cols.AppendLine($"ROUND({ta}rmf_pct_34_inch,4) {ColPrefix}rmf_pct_34_inch, ROUND({ta}rmf_pct_12_inch,4) {ColPrefix}rmf_pct_12_inch, ");
            cols.AppendLine($"ROUND({ta}flux_conc_to_agglom,4) {ColPrefix}flux_conc_to_agglom, ");
            cols.AppendLine($"ROUND({ta}acid_conc_to_agglom,4) {ColPrefix}acid_conc_to_agglom, ");
            cols.AppendLine($"ROUND({ta}total_conc_to_agglom,4) {ColPrefix}total_conc_to_agglom");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.met_conc_total_plant");
            return sql.ToString();
        }


        internal static Met_Conc_Total_Plant DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Met_Conc_Total_Plant RetVal = new();
            RetVal.Datex = (DateTime)Util.GetRowVal(row, $"{ColPrefix}datex");
            RetVal.Dmy = (Met_DMY)(decimal)Util.GetRowVal(row, $"{ColPrefix}dmy");
            RetVal.Rmf_Tons_Natural = (decimal?)Util.GetRowVal(row, $"{ColPrefix}rmf_tons_natural");
            RetVal.Rmf_Moisture = (decimal?)Util.GetRowVal(row, $"{ColPrefix}rmf_moisture");
            RetVal.Rmf_Tons_Dry = (decimal?)Util.GetRowVal(row, $"{ColPrefix}rmf_tons_dry");
            RetVal.Rmf_Avg_Ltph = (decimal?)Util.GetRowVal(row, $"{ColPrefix}rmf_avg_ltph");
            RetVal.Rmf_Pct_Mag_Fe = (decimal?)Util.GetRowVal(row, $"{ColPrefix}rmf_pct_mag_fe");
            RetVal.Rmf_Pct_Mag_Fe_Coils = (decimal?)Util.GetRowVal(row, $"{ColPrefix}rmf_pct_mag_fe_coils");
            RetVal.Conc_Tons_Nat = (decimal?)Util.GetRowVal(row, $"{ColPrefix}conc_tons_nat");
            RetVal.Conc_Tons_Dry = (decimal?)Util.GetRowVal(row, $"{ColPrefix}conc_tons_dry");
            RetVal.Conc_Nat_Ltph = (decimal?)Util.GetRowVal(row, $"{ColPrefix}conc_nat_ltph");
            RetVal.Conc_Pct_Sio2 = (decimal?)Util.GetRowVal(row, $"{ColPrefix}conc_pct_sio2");
            RetVal.Conc_Pct_Mag_Fe = (decimal?)Util.GetRowVal(row, $"{ColPrefix}conc_pct_mag_fe");
            RetVal.Conc_Pct_270m = (decimal?)Util.GetRowVal(row, $"{ColPrefix}conc_pct_270m");
            RetVal.Coarse_Tails_Tons_Dry = (decimal?)Util.GetRowVal(row, $"{ColPrefix}coarse_tails_tons_dry");
            RetVal.Fine_Tails_Tons_Dry = (decimal?)Util.GetRowVal(row, $"{ColPrefix}fine_tails_tons_dry");
            RetVal.Coarse_Tails_Pct_Mag_Fe = (decimal?)Util.GetRowVal(row, $"{ColPrefix}coarse_tails_pct_mag_fe");
            RetVal.Fine_Tails_Pct_Mag_Fe = (decimal?)Util.GetRowVal(row, $"{ColPrefix}fine_tails_pct_mag_fe");
            RetVal.Pct_Recovery_Natural = (decimal?)Util.GetRowVal(row, $"{ColPrefix}pct_recovery_natural");
            RetVal.Pct_Recovery_Dry = (decimal?)Util.GetRowVal(row, $"{ColPrefix}pct_recovery_dry");
            RetVal.Pct_Mag_Fe_Recovery = (decimal?)Util.GetRowVal(row, $"{ColPrefix}pct_mag_fe_recovery");
            RetVal.Scheduled_Hours = (decimal?)Util.GetRowVal(row, $"{ColPrefix}scheduled_hours");
            RetVal.Actual_Hours = (decimal?)Util.GetRowVal(row, $"{ColPrefix}actual_hours");
            RetVal.Pct_Operating_Time = (decimal?)Util.GetRowVal(row, $"{ColPrefix}pct_operating_time");
            RetVal.Scheduled_Maint_Hours = (decimal?)Util.GetRowVal(row, $"{ColPrefix}scheduled_maint_hours");
            RetVal.Pct_Scheduled_Maint = (decimal?)Util.GetRowVal(row, $"{ColPrefix}pct_scheduled_maint");
            RetVal.Unscheduled_Maint_Hours = (decimal?)Util.GetRowVal(row, $"{ColPrefix}unscheduled_maint_hours");
            RetVal.Pct_Unscheduled_Maint = (decimal?)Util.GetRowVal(row, $"{ColPrefix}pct_unscheduled_maint");
            RetVal.Aggl_Request_Hours = (decimal?)Util.GetRowVal(row, $"{ColPrefix}aggl_request_hours");
            RetVal.Pct_Aggl_Request = (decimal?)Util.GetRowVal(row, $"{ColPrefix}pct_aggl_request");
            RetVal.High_Power_Limit_Hours = (decimal?)Util.GetRowVal(row, $"{ColPrefix}high_power_limit_hours");
            RetVal.Pct_High_Power_Limit = (decimal?)Util.GetRowVal(row, $"{ColPrefix}pct_high_power_limit");
            RetVal.No_Ore_Hours = (decimal?)Util.GetRowVal(row, $"{ColPrefix}no_ore_hours");
            RetVal.Pct_No_Ore = (decimal?)Util.GetRowVal(row, $"{ColPrefix}pct_no_ore");
            RetVal.Other_Delay_Hours = (decimal?)Util.GetRowVal(row, $"{ColPrefix}other_delay_hours");
            RetVal.Pct_Other_Delay = (decimal?)Util.GetRowVal(row, $"{ColPrefix}pct_other_delay");
            RetVal.Avail_Hours = (decimal?)Util.GetRowVal(row, $"{ColPrefix}avail_hours");
            RetVal.Pct_Avail = (decimal?)Util.GetRowVal(row, $"{ColPrefix}pct_avail");
            RetVal.Kwh_Per_Ton_Rmf = (decimal?)Util.GetRowVal(row, $"{ColPrefix}kwh_per_ton_rmf");
            RetVal.Rod_Mill_Dav_Tube_Sio2 = (decimal?)Util.GetRowVal(row, $"{ColPrefix}rod_mill_dav_tube_sio2");
            RetVal.Rmf_Pct_34_Inch = (decimal?)Util.GetRowVal(row, $"{ColPrefix}rmf_pct_34_inch");
            RetVal.Rmf_Pct_12_Inch = (decimal?)Util.GetRowVal(row, $"{ColPrefix}rmf_pct_12_inch");
            RetVal.Flux_Conc_To_Agglom = (decimal?)Util.GetRowVal(row, $"{ColPrefix}flux_conc_to_agglom");
            RetVal.Acid_Conc_To_Agglom = (decimal?)Util.GetRowVal(row, $"{ColPrefix}acid_conc_to_agglom");
            RetVal.Total_Conc_To_Agglom = (decimal?)Util.GetRowVal(row, $"{ColPrefix}total_conc_to_agglom");
            return RetVal;
        }

    }
}
