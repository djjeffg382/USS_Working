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
    public static class K_Pell_Ind_Line_HalfSvc
    {
        static K_Pell_Ind_Line_HalfSvc()
        {
            Util.RegisterOracle();
        }


        public static K_Pell_Ind_Line_Half Get(DateTime shift_date, byte HalfShiftNbr)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE shift_date = :shift_date");
            sql.AppendLine($"WHERE half = :half");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("half", HalfShiftNbr);
            da.SelectCommand.BindByName = true;

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }


        public static List<K_Pell_Ind_Line_Half> GetAll(DateTime StartShiftDate, DateTime EndShiftDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE shift_date BETWEEN :StartDate AND :EndDate");
            sql.AppendLine("ORDER BY shift_date, half");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("StartDate", StartShiftDate);
            da.SelectCommand.Parameters.Add("EndDate", EndShiftDate);
            da.SelectCommand.BindByName = true;

            DataSet ds = MOO.Data.ExecuteQuery(da);
            List<K_Pell_Ind_Line_Half> elements = new();
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
            cols.AppendLine($"{ta}shift_date {ColPrefix}shift_date, {ta}line {ColPrefix}line, {ta}shift {ColPrefix}shift, ");
            cols.AppendLine($"{ta}half {ColPrefix}half, {ta}coal_mmbtu {ColPrefix}coal_mmbtu, ");
            cols.AppendLine($"{ta}coal_usage_lbs {ColPrefix}coal_usage_lbs, {ta}kiln_gas_mmbtu {ColPrefix}kiln_gas_mmbtu, ");
            cols.AppendLine($"{ta}kiln_gas_usage_mscf {ColPrefix}kiln_gas_usage_mscf, ");
            cols.AppendLine($"{ta}pellet_tons_dry_gt {ColPrefix}pellet_tons_dry_gt, {ta}pellet_tons_gt {ColPrefix}pellet_tons_gt, ");
            cols.AppendLine($"{ta}pell_scn_plus_1_2_pct {ColPrefix}pell_scn_plus_1_2_pct, ");
            cols.AppendLine($"{ta}pell_scn_plus_1_4_pct {ColPrefix}pell_scn_plus_1_4_pct, ");
            cols.AppendLine($"{ta}pell_tumble_plus_1_4_pct {ColPrefix}pell_tumble_plus_1_4_pct, ");
            cols.AppendLine($"{ta}pell_tumble_plus_30_pct {ColPrefix}pell_tumble_plus_30_pct, ");
            cols.AppendLine($"{ta}pell_compression_lbs {ColPrefix}pell_compression_lbs, ");
            cols.AppendLine($"{ta}pell_less_300_pct {ColPrefix}pell_less_300_pct, ");
            cols.AppendLine($"{ta}pell_less_200_pct {ColPrefix}pell_less_200_pct, {ta}pell_fe_pct {ColPrefix}pell_fe_pct, ");
            cols.AppendLine($"{ta}pell_sio2_pct {ColPrefix}pell_sio2_pct, {ta}pell_cao_pct {ColPrefix}pell_cao_pct, ");
            cols.AppendLine($"{ta}pell_moisture_pct {ColPrefix}pell_moisture_pct, {ta}pell_mag_pct {ColPrefix}pell_mag_pct, ");
            cols.AppendLine($"{ta}start_date {ColPrefix}start_date, {ta}pell_mgo_pct {ColPrefix}pell_mgo_pct, ");
            cols.AppendLine($"{ta}pell_al2o3_pct {ColPrefix}pell_al2o3_pct, {ta}pell_mn_pct {ColPrefix}pell_mn_pct, ");
            cols.AppendLine($"{ta}pell_phos_pct {ColPrefix}pell_phos_pct, {ta}pell_sulfur_pct {ColPrefix}pell_sulfur_pct, ");
            cols.AppendLine($"{ta}pell_fe_plus_3_pct {ColPrefix}pell_fe_plus_3_pct, ");
            cols.AppendLine($"{ta}pell_scn_plus_7_16_pct {ColPrefix}pell_scn_plus_7_16_pct, ");
            cols.AppendLine($"{ta}pell_scn_plus_3_8_pct {ColPrefix}pell_scn_plus_3_8_pct, ");
            cols.AppendLine($"{ta}pell_tumble_plus_1_2_pct {ColPrefix}pell_tumble_plus_1_2_pct, ");
            cols.AppendLine($"{ta}pell_tumble_plus_7_16_pct {ColPrefix}pell_tumble_plus_7_16_pct, ");
            cols.AppendLine($"{ta}pell_tumble_plus_3_8_pct {ColPrefix}pell_tumble_plus_3_8_pct");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM warehouse.k_pell_ind_line_half");
            return sql.ToString();
        }


        internal static K_Pell_Ind_Line_Half DataRowToObject(DataRow row, string ColPrefix = "")
        {
            K_Pell_Ind_Line_Half RetVal = new();
            RetVal.Shift_Date = row.Field<DateTime>($"{ColPrefix}shift_date");
            RetVal.Line = (byte)row.Field<decimal>($"{ColPrefix}line");
            RetVal.Shift =(byte)row.Field<decimal?>($"{ColPrefix}shift");
            RetVal.Half = (byte)row.Field<decimal>($"{ColPrefix}half");
            RetVal.Coal_Mmbtu = row.Field<decimal?>($"{ColPrefix}coal_mmbtu");
            RetVal.Coal_Usage_Lbs = row.Field<decimal?>($"{ColPrefix}coal_usage_lbs");
            RetVal.Kiln_Gas_Mmbtu = row.Field<decimal?>($"{ColPrefix}kiln_gas_mmbtu");
            RetVal.Kiln_Gas_Usage_Mscf = row.Field<decimal?>($"{ColPrefix}kiln_gas_usage_mscf");
            RetVal.Pellet_Tons_Dry_Gt = row.Field<decimal?>($"{ColPrefix}pellet_tons_dry_gt");
            RetVal.Pellet_Tons_Gt = row.Field<decimal?>($"{ColPrefix}pellet_tons_gt");
            RetVal.Pell_Scn_Plus_1_2_Pct = row.Field<decimal?>($"{ColPrefix}pell_scn_plus_1_2_pct");
            RetVal.Pell_Scn_Plus_1_4_Pct = row.Field<decimal?>($"{ColPrefix}pell_scn_plus_1_4_pct");
            RetVal.Pell_Tumble_Plus_1_4_Pct = row.Field<decimal?>($"{ColPrefix}pell_tumble_plus_1_4_pct");
            RetVal.Pell_Tumble_Plus_30_Pct = row.Field<decimal?>($"{ColPrefix}pell_tumble_plus_30_pct");
            RetVal.Pell_Compression_Lbs = row.Field<decimal?>($"{ColPrefix}pell_compression_lbs");
            RetVal.Pell_Less_300_Pct = row.Field<decimal?>($"{ColPrefix}pell_less_300_pct");
            RetVal.Pell_Less_200_Pct = row.Field<decimal?>($"{ColPrefix}pell_less_200_pct");
            RetVal.Pell_Fe_Pct = row.Field<decimal?>($"{ColPrefix}pell_fe_pct");
            RetVal.Pell_Sio2_Pct = row.Field<decimal?>($"{ColPrefix}pell_sio2_pct");
            RetVal.Pell_Cao_Pct = row.Field<decimal?>($"{ColPrefix}pell_cao_pct");
            RetVal.Pell_Moisture_Pct = row.Field<decimal?>($"{ColPrefix}pell_moisture_pct");
            RetVal.Pell_Mag_Pct = row.Field<decimal?>($"{ColPrefix}pell_mag_pct");
            RetVal.Start_Date = row.Field<DateTime?>($"{ColPrefix}start_date");
            RetVal.Pell_Mgo_Pct = row.Field<decimal?>($"{ColPrefix}pell_mgo_pct");
            RetVal.Pell_Al2o3_Pct = row.Field<decimal?>($"{ColPrefix}pell_al2o3_pct");
            RetVal.Pell_Mn_Pct = row.Field<decimal?>($"{ColPrefix}pell_mn_pct");
            RetVal.Pell_Phos_Pct = row.Field<decimal?>($"{ColPrefix}pell_phos_pct");
            RetVal.Pell_Sulfur_Pct = row.Field<decimal?>($"{ColPrefix}pell_sulfur_pct");
            RetVal.Pell_Fe_Plus_3_Pct = row.Field<decimal?>($"{ColPrefix}pell_fe_plus_3_pct");
            RetVal.Pell_Scn_Plus_7_16_Pct = row.Field<decimal?>($"{ColPrefix}pell_scn_plus_7_16_pct");
            RetVal.Pell_Scn_Plus_3_8_Pct = row.Field<decimal?>($"{ColPrefix}pell_scn_plus_3_8_pct");
            RetVal.Pell_Tumble_Plus_1_2_Pct = row.Field<decimal?>($"{ColPrefix}pell_tumble_plus_1_2_pct");
            RetVal.Pell_Tumble_Plus_7_16_Pct = row.Field<decimal?>($"{ColPrefix}pell_tumble_plus_7_16_pct");
            RetVal.Pell_Tumble_Plus_3_8_Pct = row.Field<decimal?>($"{ColPrefix}pell_tumble_plus_3_8_pct");
            return RetVal;
        }

    }
}
