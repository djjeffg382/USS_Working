using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Services
{
    public class Agg2_DaySvc
    {
        static Agg2_DaySvc()
        {
            Util.RegisterOracle();
        }


        public static Agg2_Day Get(DateTime ShiftDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Report_Date = :Report_Date");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("Report_Date", ShiftDate);
            da.SelectCommand.BindByName = true;

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }

        public static List<Agg2_Day> GetAll(DateTime StartDate, DateTime EndDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Report_Date BETWEEN :StartDate AND :EndDate");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("StartDate", StartDate);
            da.SelectCommand.Parameters.Add("EndDate", EndDate);
            da.SelectCommand.BindByName = true;

            DataSet ds = MOO.Data.ExecuteQuery(da);
            List<Agg2_Day> elements = new();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                elements.Add(DataRowToObject(dr));
            }
            return elements;
        }


        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}agg2_day_id {ColPrefix}agg2_day_id, {ta}report_date {ColPrefix}report_date, ");
            cols.AppendLine($"ROUND({ta}lines_running_avg,4) {ColPrefix}lines_running_avg, ");
            cols.AppendLine($"ROUND({ta}pday_lines_running_avg,4) {ColPrefix}pday_lines_running_avg, ");
            cols.AppendLine($"{ta}created_user {ColPrefix}created_user, {ta}created_date {ColPrefix}created_date, ");
            cols.AppendLine($"{ta}updated_user {ColPrefix}updated_user, {ta}updated_date {ColPrefix}updated_date, ");
            cols.AppendLine($"ROUND({ta}ball_hrs,4) {ColPrefix}ball_hrs, ROUND({ta}bent_lbs,4) {ColPrefix}bent_lbs, ");
            cols.AppendLine($"ROUND({ta}bent_lbs_lton,4) {ColPrefix}bent_lbs_lton, ROUND({ta}bin_3_pct,4) {ColPrefix}bin_3_pct, ");
            cols.AppendLine($"ROUND({ta}bin_4_pct,4) {ColPrefix}bin_4_pct, ROUND({ta}bin_5_pct,4) {ColPrefix}bin_5_pct, ");
            cols.AppendLine($"ROUND({ta}coal_stons,4) {ColPrefix}coal_stons, ROUND({ta}dd1_inh2o,4) {ColPrefix}dd1_inh2o, ");
            cols.AppendLine($"ROUND({ta}fuel_oil_gals,4) {ColPrefix}fuel_oil_gals, ROUND({ta}fuel_oil_tank_lvl,4) {ColPrefix}fuel_oil_tank_lvl, ");
            cols.AppendLine($"ROUND({ta}gas_mscf,4) {ColPrefix}gas_mscf, ROUND({ta}grate_hrs,4) {ColPrefix}grate_hrs, ");
            cols.AppendLine($"ROUND({ta}lines_running_sum,4) {ColPrefix}lines_running_sum, ROUND({ta}mbtu_lton,4) {ColPrefix}mbtu_lton, ");
            cols.AppendLine($"ROUND({ta}mbtus,4) {ColPrefix}mbtus, {ta}mo_to_day_pel_ltons {ColPrefix}mo_to_day_pel_ltons, ");
            cols.AppendLine($"ROUND({ta}mo_to_day_pel_frcst_ltons,4) {ColPrefix}mo_to_day_pel_frcst_ltons, ");
            cols.AppendLine($"ROUND({ta}pel_ltons,4) {ColPrefix}pel_ltons, ROUND({ta}recl_in_ltons,4) {ColPrefix}recl_in_ltons, ");
            cols.AppendLine($"ROUND({ta}recl_out_ltons,4) {ColPrefix}recl_out_ltons, ");
            cols.AppendLine($"ROUND({ta}recl_acid_bal_ltons,4) {ColPrefix}recl_acid_bal_ltons, ");
            cols.AppendLine($"ROUND({ta}tank_flux_1_pct,4) {ColPrefix}tank_flux_1_pct, ROUND({ta}tank_flux_2_pct,4) {ColPrefix}tank_flux_2_pct, ");
            cols.AppendLine($"ROUND({ta}tank_slurry_1_1_pct,4) {ColPrefix}tank_slurry_1_1_pct, ");
            cols.AppendLine($"ROUND({ta}tank_slurry_1_2_pct,4) {ColPrefix}tank_slurry_1_2_pct, ROUND({ta}wood_stons,4) {ColPrefix}wood_stons, ");
            cols.AppendLine($"ROUND({ta}drum_down_time_min,4) {ColPrefix}drum_down_time_min, ROUND({ta}down_time_min,4) {ColPrefix}down_time_min, ");
            cols.AppendLine($"ROUND({ta}recl_in_act_ltons,4) {ColPrefix}recl_in_act_ltons, ");
            cols.AppendLine($"ROUND({ta}recl_out_act_ltons,4) {ColPrefix}recl_out_act_ltons, ");
            cols.AppendLine($"ROUND({ta}recl_in_adj_ltons,4) {ColPrefix}recl_in_adj_ltons, ");
            cols.AppendLine($"ROUND({ta}recl_out_adj_ltons,4) {ColPrefix}recl_out_adj_ltons, ROUND({ta}recl_bal_ltons,4) {ColPrefix}recl_bal_ltons, ");
            cols.AppendLine($"ROUND({ta}nox_lbs_hr,4) {ColPrefix}nox_lbs_hr, ROUND({ta}so2_lbs_hr,4) {ColPrefix}so2_lbs_hr,");
            cols.AppendLine($"{ta}bin1_pct {ColPrefix}bin1_pct, {ta}bin2_pct {ColPrefix}bin2_pct");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.agg2_day");
            return sql.ToString();
        }




        internal static Agg2_Day DataRowToObject(DataRow row, string ColPrefix = "")
        {
            Agg2_Day RetVal = new();
            RetVal.Agg2_Day_Id = (int)row.Field<decimal>($"{ColPrefix}agg2_day_id");
            RetVal.Report_Date = row.Field<DateTime>($"{ColPrefix}report_date");
            RetVal.Lines_Running_Avg = row.Field<decimal?>($"{ColPrefix}lines_running_avg");
            RetVal.Pday_Lines_Running_Avg = row.Field<decimal?>($"{ColPrefix}pday_lines_running_avg");
            RetVal.Created_User = row.Field<string>($"{ColPrefix}created_user");
            RetVal.Created_Date = row.Field<DateTime>($"{ColPrefix}created_date");
            RetVal.Updated_User = row.Field<string>($"{ColPrefix}updated_user");
            RetVal.Updated_Date = row.Field<DateTime>($"{ColPrefix}updated_date");
            RetVal.Ball_Hrs = row.Field<decimal?>($"{ColPrefix}ball_hrs");
            RetVal.Bent_Lbs = row.Field<decimal?>($"{ColPrefix}bent_lbs");
            RetVal.Bent_Lbs_Lton = row.Field<decimal?>($"{ColPrefix}bent_lbs_lton");
            RetVal.Bin_3_Pct = row.Field<decimal?>($"{ColPrefix}bin_3_pct");
            RetVal.Bin_4_Pct = row.Field<decimal?>($"{ColPrefix}bin_4_pct");
            RetVal.Bin_5_Pct = row.Field<decimal?>($"{ColPrefix}bin_5_pct");
            RetVal.Coal_Stons = row.Field<decimal?>($"{ColPrefix}coal_stons");
            RetVal.Dd1_Inh2o = row.Field<decimal?>($"{ColPrefix}dd1_inh2o");
            RetVal.Fuel_Oil_Gals = row.Field<decimal?>($"{ColPrefix}fuel_oil_gals");
            RetVal.Fuel_Oil_Tank_Lvl = row.Field<decimal?>($"{ColPrefix}fuel_oil_tank_lvl");
            RetVal.Gas_Mscf = row.Field<decimal?>($"{ColPrefix}gas_mscf");
            RetVal.Grate_Hrs = row.Field<decimal?>($"{ColPrefix}grate_hrs");
            RetVal.Lines_Running_Sum = row.Field<decimal?>($"{ColPrefix}lines_running_sum");
            RetVal.Mbtu_Lton = row.Field<decimal?>($"{ColPrefix}mbtu_lton");
            RetVal.Mbtus = row.Field<decimal?>($"{ColPrefix}mbtus");
            RetVal.Mo_To_Day_Pel_Ltons = row.Field<decimal?>($"{ColPrefix}mo_to_day_pel_ltons");
            RetVal.Mo_To_Day_Pel_Frcst_Ltons = row.Field<decimal?>($"{ColPrefix}mo_to_day_pel_frcst_ltons");
            RetVal.Pel_Ltons = row.Field<decimal?>($"{ColPrefix}pel_ltons");
            RetVal.Recl_In_Ltons = row.Field<decimal?>($"{ColPrefix}recl_in_ltons");
            RetVal.Recl_Out_Ltons = row.Field<decimal?>($"{ColPrefix}recl_out_ltons");
            RetVal.Recl_Acid_Bal_Ltons = row.Field<decimal?>($"{ColPrefix}recl_acid_bal_ltons");
            RetVal.Tank_Flux_1_Pct = row.Field<decimal?>($"{ColPrefix}tank_flux_1_pct");
            RetVal.Tank_Flux_2_Pct = row.Field<decimal?>($"{ColPrefix}tank_flux_2_pct");
            RetVal.Tank_Slurry_1_1_Pct = row.Field<decimal?>($"{ColPrefix}tank_slurry_1_1_pct");
            RetVal.Tank_Slurry_1_2_Pct = row.Field<decimal?>($"{ColPrefix}tank_slurry_1_2_pct");
            RetVal.Wood_Stons = row.Field<decimal?>($"{ColPrefix}wood_stons");
            RetVal.Drum_Down_Time_Min = row.Field<decimal?>($"{ColPrefix}drum_down_time_min");
            RetVal.Down_Time_Min = row.Field<decimal?>($"{ColPrefix}down_time_min");
            RetVal.Recl_In_Act_Ltons = row.Field<decimal?>($"{ColPrefix}recl_in_act_ltons");
            RetVal.Recl_Out_Act_Ltons = row.Field<decimal?>($"{ColPrefix}recl_out_act_ltons");
            RetVal.Recl_In_Adj_Ltons = row.Field<decimal?>($"{ColPrefix}recl_in_adj_ltons");
            RetVal.Recl_Out_Adj_Ltons = row.Field<decimal?>($"{ColPrefix}recl_out_adj_ltons");
            RetVal.Recl_Bal_Ltons = row.Field<decimal?>($"{ColPrefix}recl_bal_ltons");
            RetVal.Nox_Lbs_Hr = row.Field<decimal?>($"{ColPrefix}nox_lbs_hr");
            RetVal.So2_Lbs_Hr = row.Field<decimal?>($"{ColPrefix}so2_lbs_hr");
            RetVal.Bin1_Pct = row.Field<double?>($"{ColPrefix}bin_1_pct");
            RetVal.Bin2_Pct = row.Field<double?>($"{ColPrefix}bin_2_pct");
            return RetVal;
        }

    }
}
