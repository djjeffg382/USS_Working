using MOO.DAL.Warehouse.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Warehouse.Services
{
    public static class K_Conc_ShiftSvc
    {
        static K_Conc_ShiftSvc()
        {
            Util.RegisterOracle();
        }


        public static K_Conc_Shift Get(DateTime shift_date, byte ShiftNbr)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE shift_date = :shift_date");
            sql.AppendLine($"AND shift = :shift");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("shift_date", shift_date);
            da.SelectCommand.Parameters.Add("shift", ShiftNbr);
            da.SelectCommand.BindByName = true;

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }


        public static List<K_Conc_Shift> GetAll(DateTime StartShiftDate, DateTime EndShiftDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE shift_date BETWEEN :StartDate AND :EndDate");
            sql.AppendLine("ORDER BY shift_date, shift");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("StartDate", StartShiftDate);
            da.SelectCommand.Parameters.Add("EndDate", EndShiftDate);
            da.SelectCommand.BindByName = true;

            DataSet ds = MOO.Data.ExecuteQuery(da);
            List<K_Conc_Shift> elements = new();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    elements.Add(DataRowToObject(dr));
                }
            }
            return elements;
        }


        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}shift_date {ColPrefix}shift_date, {ta}shift {ColPrefix}shift, ");
            cols.AppendLine($"{ta}concentrate_slurry_gt {ColPrefix}concentrate_slurry_gt, ");
            cols.AppendLine($"{ta}tailings_flow_gpm {ColPrefix}tailings_flow_gpm, ");
            cols.AppendLine($"{ta}tails_density_pct {ColPrefix}tails_density_pct, ");
            cols.AppendLine($"{ta}pri_conc_feed_gt {ColPrefix}pri_conc_feed_gt, ");
            cols.AppendLine($"{ta}pri_power_consumption_kwh {ColPrefix}pri_power_consumption_kwh, ");
            cols.AppendLine($"{ta}sec_power_consumption_kwh {ColPrefix}sec_power_consumption_kwh, ");
            cols.AppendLine($"{ta}pri_conc_feed_rate_gtph {ColPrefix}pri_conc_feed_rate_gtph, ");
            cols.AppendLine($"{ta}tails_mag_fe_pct {ColPrefix}tails_mag_fe_pct, {ta}nola_sio2_pct {ColPrefix}nola_sio2_pct, ");
            cols.AppendLine($"{ta}conc_sio2_pct {ColPrefix}conc_sio2_pct, {ta}conc_fe_pct {ColPrefix}conc_fe_pct, ");
            cols.AppendLine($"{ta}blaine_surface_area_cm2_g {ColPrefix}blaine_surface_area_cm2_g, ");
            cols.AppendLine($"{ta}start_date {ColPrefix}start_date, ");
            cols.AppendLine($"{ta}coarse_tails_mag_fe_pct {ColPrefix}coarse_tails_mag_fe_pct, ");
            cols.AppendLine($"{ta}fine_tails_mag_fe_pct {ColPrefix}fine_tails_mag_fe_pct");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM warehouse.k_conc_shift");
            return sql.ToString();
        }


        internal static K_Conc_Shift DataRowToObject(DataRow row, string ColPrefix = "")
        {
            K_Conc_Shift RetVal = new();
            RetVal.Shift_Date = row.Field<DateTime>($"{ColPrefix}shift_date");
            RetVal.Shift = (byte)row.Field<decimal>($"{ColPrefix}shift");
            RetVal.Concentrate_Slurry_Gt = row.Field<decimal?>($"{ColPrefix}concentrate_slurry_gt");
            RetVal.Tailings_Flow_Gpm = row.Field<decimal?>($"{ColPrefix}tailings_flow_gpm");
            RetVal.Tails_Density_Pct = row.Field<decimal?>($"{ColPrefix}tails_density_pct");
            RetVal.Pri_Conc_Feed_Gt = row.Field<decimal?>($"{ColPrefix}pri_conc_feed_gt");
            RetVal.Pri_Power_Consumption_Kwh = row.Field<decimal?>($"{ColPrefix}pri_power_consumption_kwh");
            RetVal.Sec_Power_Consumption_Kwh = row.Field<decimal?>($"{ColPrefix}sec_power_consumption_kwh");
            RetVal.Pri_Conc_Feed_Rate_Gtph = row.Field<decimal?>($"{ColPrefix}pri_conc_feed_rate_gtph");
            RetVal.Tails_Mag_Fe_Pct = row.Field<decimal?>($"{ColPrefix}tails_mag_fe_pct");
            RetVal.Nola_Sio2_Pct = row.Field<decimal?>($"{ColPrefix}nola_sio2_pct");
            RetVal.Conc_Sio2_Pct = row.Field<decimal?>($"{ColPrefix}conc_sio2_pct");
            RetVal.Conc_Fe_Pct = row.Field<decimal?>($"{ColPrefix}conc_fe_pct");
            RetVal.Blaine_Surface_Area_Cm2_G = row.Field<decimal?>($"{ColPrefix}blaine_surface_area_cm2_g");
            RetVal.Start_Date = row.Field<DateTime?>($"{ColPrefix}start_date");
            RetVal.Coarse_Tails_Mag_Fe_Pct = row.Field<decimal?>($"{ColPrefix}coarse_tails_mag_fe_pct");
            RetVal.Fine_Tails_Mag_Fe_Pct = row.Field<decimal?>($"{ColPrefix}fine_tails_mag_fe_pct");
            return RetVal;
        }

    }
}
