using MOO.DAL.ToLive.Enums;
using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Services
{
    public static class Lab_Phys_AnalysisSvc
    {
        static Lab_Phys_AnalysisSvc()
        {
            Util.RegisterOracle();
        }


        public static Lab_Phys_Analysis Get(int lab_phys_analysis_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE lab_phys_analysis_id = :lab_phys_analysis_id");


            Lab_Phys_Analysis retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("lab_phys_analysis_id", lab_phys_analysis_id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr, "lca_");
            }
            conn.Close();
            return retVal;
        }


        public static List<Lab_Phys_Analysis> GetByShiftDate(int labPhysTypeId, DateTime startDate, DateTime endDate, ShiftType shiftType, byte? lineNbr = null)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE lca.Lab_phys_type_id = :labPhysTypeId");
            if (shiftType == ShiftType.ShiftType8Hour)
            {
                sql.AppendLine("AND lca.shift_date8 BETWEEN :startDate AND :endDate");
            }
            else if (shiftType == ShiftType.ShiftType12Hour)
            {
                sql.AppendLine("AND lca.shift_date12 BETWEEN :startDate AND :endDate");
            }
            if (lineNbr != null)
                sql.AppendLine("AND lca.line_nbr = :lineNbr");


            List<Lab_Phys_Analysis> elements = [];
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("labPhysTypeId", labPhysTypeId);
            cmd.Parameters.Add("startDate", startDate);
            cmd.Parameters.Add("endDate", endDate);
            if (lineNbr != null)
                cmd.Parameters.Add("lineNbr", lineNbr);

            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    elements.Add(DataRowToObject(rdr, "lca_"));
                }
            }
            conn.Close();
            return elements;
        }

        public static List<Lab_Phys_Analysis> GetByAnalysisDate(int labPhysTypeId, DateTime startDate, DateTime endDate, byte? lineNbr = null)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE lca.Lab_phys_type_id = :labPhysTypeId");
            sql.AppendLine("AND lca.analysis_date BETWEEN :startDate AND :endDate");
            if (lineNbr != null)
                sql.AppendLine("AND lca.line_nbr = :lineNbr");


            List<Lab_Phys_Analysis> elements = [];
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("labPhysTypeId", labPhysTypeId);
            cmd.Parameters.Add("startDate", startDate);
            cmd.Parameters.Add("endDate", endDate);
            if (lineNbr != null)
                cmd.Parameters.Add("lineNbr", lineNbr);

            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    elements.Add(DataRowToObject(rdr, "lca_"));
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
            cols.AppendLine($"{ta}lab_phys_analysis_id {ColPrefix}lab_phys_analysis_id, ");
            cols.AppendLine($"{ta}lab_phys_type_id {ColPrefix}lab_phys_type_id, {ta}line_nbr {ColPrefix}line_nbr, ");
            cols.AppendLine($"{ta}analysis_date {ColPrefix}analysis_date, {ta}shift_date8 {ColPrefix}shift_date8, ");
            cols.AppendLine($"{ta}shift_half8 {ColPrefix}shift_half8, ");
            cols.AppendLine($"{ta}shift_nbr8 {ColPrefix}shift_nbr8, {ta}shift_date12 {ColPrefix}shift_date12, ");
            cols.AppendLine($"{ta}shift_nbr12 {ColPrefix}shift_nbr12, {ta}update_date {ColPrefix}update_date, ");
            cols.AppendLine($"{ta}last_update_by {ColPrefix}last_update_by, {ta}start_wgt {ColPrefix}start_wgt, ");
            cols.AppendLine($"{ta}inch_1_wgt {ColPrefix}inch_1_wgt, {ta}inch_1_pct {ColPrefix}inch_1_pct, ");
            cols.AppendLine($"{ta}inch_3_4_wgt {ColPrefix}inch_3_4_wgt, {ta}inch_3_4_pct {ColPrefix}inch_3_4_pct, ");
            cols.AppendLine($"{ta}inch_5_8_wgt {ColPrefix}inch_5_8_wgt, {ta}inch_5_8_pct {ColPrefix}inch_5_8_pct, ");
            cols.AppendLine($"{ta}inch_9_16_wgt {ColPrefix}inch_9_16_wgt, {ta}inch_9_16_pct {ColPrefix}inch_9_16_pct, ");
            cols.AppendLine($"{ta}inch_1_2_wgt {ColPrefix}inch_1_2_wgt, {ta}inch_1_2_pct {ColPrefix}inch_1_2_pct, ");
            cols.AppendLine($"{ta}inch_7_16_wgt {ColPrefix}inch_7_16_wgt, {ta}inch_7_16_pct {ColPrefix}inch_7_16_pct, ");
            cols.AppendLine($"{ta}inch_3_8_wgt {ColPrefix}inch_3_8_wgt, {ta}inch_3_8_pct {ColPrefix}inch_3_8_pct, ");
            cols.AppendLine($"{ta}inch_1_4_wgt {ColPrefix}inch_1_4_wgt, {ta}inch_1_4_pct {ColPrefix}inch_1_4_pct, ");
            cols.AppendLine($"{ta}mesh_3_wgt {ColPrefix}mesh_3_wgt, {ta}mesh_3_pct {ColPrefix}mesh_3_pct, ");
            cols.AppendLine($"{ta}mesh_4_wgt {ColPrefix}mesh_4_wgt, {ta}mesh_4_pct {ColPrefix}mesh_4_pct, ");
            cols.AppendLine($"{ta}mesh_6_wgt {ColPrefix}mesh_6_wgt, {ta}mesh_6_pct {ColPrefix}mesh_6_pct, ");
            cols.AppendLine($"{ta}mesh_8_wgt {ColPrefix}mesh_8_wgt, {ta}mesh_8_pct {ColPrefix}mesh_8_pct, ");
            cols.AppendLine($"{ta}mesh_10_12_wgt {ColPrefix}mesh_10_12_wgt, {ta}mesh_10_12_pct {ColPrefix}mesh_10_12_pct, ");
            cols.AppendLine($"{ta}mesh_14_16_wgt {ColPrefix}mesh_14_16_wgt, {ta}mesh_14_16_pct {ColPrefix}mesh_14_16_pct, ");
            cols.AppendLine($"{ta}mesh_20_wgt {ColPrefix}mesh_20_wgt, {ta}mesh_20_pct {ColPrefix}mesh_20_pct, ");
            cols.AppendLine($"{ta}mesh_28_30_wgt {ColPrefix}mesh_28_30_wgt, {ta}mesh_28_30_pct {ColPrefix}mesh_28_30_pct, ");
            cols.AppendLine($"{ta}mesh_35_40_wgt {ColPrefix}mesh_35_40_wgt, {ta}mesh_35_40_pct {ColPrefix}mesh_35_40_pct, ");
            cols.AppendLine($"{ta}mesh_48_50_wgt {ColPrefix}mesh_48_50_wgt, {ta}mesh_48_50_pct {ColPrefix}mesh_48_50_pct, ");
            cols.AppendLine($"{ta}mesh_65_70_wgt {ColPrefix}mesh_65_70_wgt, {ta}mesh_65_70_pct {ColPrefix}mesh_65_70_pct, ");
            cols.AppendLine($"{ta}mesh_100_wgt {ColPrefix}mesh_100_wgt, {ta}mesh_100_pct {ColPrefix}mesh_100_pct, ");
            cols.AppendLine($"{ta}mesh_150_140_wgt {ColPrefix}mesh_150_140_wgt, {ta}mesh_150_140_pct {ColPrefix}mesh_150_140_pct, ");
            cols.AppendLine($"{ta}mesh_200_wgt {ColPrefix}mesh_200_wgt, {ta}mesh_200_pct {ColPrefix}mesh_200_pct, ");
            cols.AppendLine($"{ta}mesh_270_wgt {ColPrefix}mesh_270_wgt, {ta}mesh_270_pct {ColPrefix}mesh_270_pct, ");
            cols.AppendLine($"{ta}mesh_325_wgt {ColPrefix}mesh_325_wgt, {ta}mesh_325_pct {ColPrefix}mesh_325_pct, ");
            cols.AppendLine($"{ta}mesh_400_wgt {ColPrefix}mesh_400_wgt, {ta}mesh_400_pct {ColPrefix}mesh_400_pct, ");
            cols.AppendLine($"{ta}mesh_500_wgt {ColPrefix}mesh_500_wgt, {ta}mesh_500_pct {ColPrefix}mesh_500_pct,");
            cols.AppendLine($"{ta}authorized_by {ColPrefix}authorized_by, {ta}defaults_used {ColPrefix}defaults_used");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns("lca", "lca_") + ",");
            sql.AppendLine(Lab_Phys_TypeSvc.GetColumns("lct", "lct_"));
            sql.AppendLine("FROM tolive.lab_phys_analysis lca");
            sql.AppendLine("JOIN tolive.lab_phys_type lct ON lca.lab_phys_type_id = lct.lab_phys_type_id");
            return sql.ToString();
        }


        public static int Insert(Lab_Phys_Analysis obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Lab_Phys_Analysis obj, OracleConnection conn)
        {
            if (obj.Lab_Phys_Analysis_Id <= 0)
                obj.Lab_Phys_Analysis_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.seq_lab_analysis"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Lab_Phys_Analysis(");
            sql.AppendLine("lab_phys_analysis_id, lab_phys_type_id, line_nbr, analysis_date, shift_date8, ");
            sql.AppendLine("shift_nbr8, shift_half8, shift_date12, shift_nbr12, update_date, last_update_by, start_wgt,");
            sql.AppendLine("inch_1_wgt, inch_3_4_wgt, inch_5_8_wgt,");
            sql.AppendLine("inch_9_16_wgt, inch_1_2_wgt, inch_7_16_wgt, inch_3_8_wgt, ");
            sql.AppendLine("inch_1_4_wgt, mesh_3_wgt, mesh_4_wgt, mesh_6_wgt, mesh_8_wgt, ");
            sql.AppendLine("mesh_10_12_wgt, mesh_14_16_wgt, mesh_20_wgt, mesh_28_30_wgt, ");
            sql.AppendLine("mesh_35_40_wgt, mesh_48_50_wgt, mesh_65_70_wgt, mesh_100_wgt, ");
            sql.AppendLine("mesh_150_140_wgt, mesh_200_wgt, mesh_270_wgt, mesh_325_wgt, ");
            sql.AppendLine("mesh_400_wgt, mesh_500_wgt, authorized_by, defaults_used)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":lab_phys_analysis_id, :lab_phys_type_id, :line_nbr, :analysis_date, :shift_date8, ");
            sql.AppendLine(":shift_nbr8, :shift_half8, :shift_date12, :shift_nbr12, :update_date, :last_update_by, :start_wgt, ");
            sql.AppendLine(":inch_1_wgt, :inch_3_4_wgt, :inch_5_8_wgt, ");
            sql.AppendLine(":inch_9_16_wgt, :inch_1_2_wgt, :inch_7_16_wgt, :inch_3_8_wgt, ");
            sql.AppendLine(":inch_1_4_wgt, :mesh_3_wgt, :mesh_4_wgt, :mesh_6_wgt, :mesh_8_wgt, ");
            sql.AppendLine(":mesh_10_12_wgt, :mesh_14_16_wgt, :mesh_20_wgt, :mesh_28_30_wgt, ");
            sql.AppendLine(":mesh_35_40_wgt, :mesh_48_50_wgt, :mesh_65_70_wgt, :mesh_100_wgt, ");
            sql.AppendLine(":mesh_150_140_wgt, :mesh_200_wgt, :mesh_270_wgt, :mesh_325_wgt, ");
            sql.AppendLine(":mesh_400_wgt, :mesh_500_wgt, :authorized_by, :defaults_used)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("lab_phys_analysis_id", obj.Lab_Phys_Analysis_Id);
            ins.Parameters.Add("lab_phys_type_id", obj.Lab_Phys_Type.Lab_Phys_Type_Id);
            ins.Parameters.Add("line_nbr", obj.Line_Nbr);
            ins.Parameters.Add("analysis_date", obj.Analysis_Date);
            ins.Parameters.Add("shift_date8", obj.Shift_Date8);
            ins.Parameters.Add("shift_nbr8", obj.Shift_Nbr8);
            ins.Parameters.Add("shift_half8", obj.Shift_Half8);
            ins.Parameters.Add("shift_date12", obj.Shift_Date12);
            ins.Parameters.Add("shift_nbr12", obj.Shift_Nbr12);
            ins.Parameters.Add("update_date", DateTime.Now);
            ins.Parameters.Add("last_update_by", obj.Last_Update_By);
            ins.Parameters.Add("start_wgt", obj.Start_Wgt);
            ins.Parameters.Add("inch_1_wgt", obj.Inch_1_Wgt);
            ins.Parameters.Add("inch_3_4_wgt", obj.Inch_3_4_Wgt);
            ins.Parameters.Add("inch_5_8_wgt", obj.Inch_5_8_Wgt);
            ins.Parameters.Add("inch_9_16_wgt", obj.Inch_9_16_Wgt);
            ins.Parameters.Add("inch_1_2_wgt", obj.Inch_1_2_Wgt);
            ins.Parameters.Add("inch_7_16_wgt", obj.Inch_7_16_Wgt);
            ins.Parameters.Add("inch_3_8_wgt", obj.Inch_3_8_Wgt);
            ins.Parameters.Add("inch_1_4_wgt", obj.Inch_1_4_Wgt);
            ins.Parameters.Add("mesh_3_wgt", obj.Mesh_3_Wgt);
            ins.Parameters.Add("mesh_4_wgt", obj.Mesh_4_Wgt);
            ins.Parameters.Add("mesh_6_wgt", obj.Mesh_6_Wgt);
            ins.Parameters.Add("mesh_8_wgt", obj.Mesh_8_Wgt);
            ins.Parameters.Add("mesh_10_12_wgt", obj.Mesh_10_12_Wgt);
            ins.Parameters.Add("mesh_14_16_wgt", obj.Mesh_14_16_Wgt);
            ins.Parameters.Add("mesh_20_wgt", obj.Mesh_20_Wgt);
            ins.Parameters.Add("mesh_28_30_wgt", obj.Mesh_28_30_Wgt);
            ins.Parameters.Add("mesh_35_40_wgt", obj.Mesh_35_40_Wgt);
            ins.Parameters.Add("mesh_48_50_wgt", obj.Mesh_48_50_Wgt);
            ins.Parameters.Add("mesh_65_70_wgt", obj.Mesh_65_70_Wgt);
            ins.Parameters.Add("mesh_100_wgt", obj.Mesh_100_Wgt);
            ins.Parameters.Add("mesh_150_140_wgt", obj.Mesh_150_140_Wgt);
            ins.Parameters.Add("mesh_200_wgt", obj.Mesh_200_Wgt);
            ins.Parameters.Add("mesh_270_wgt", obj.Mesh_270_Wgt);
            ins.Parameters.Add("mesh_325_wgt", obj.Mesh_325_Wgt);
            ins.Parameters.Add("mesh_400_wgt", obj.Mesh_400_Wgt);
            ins.Parameters.Add("mesh_500_wgt", obj.Mesh_500_Wgt);
            ins.Parameters.Add("authorized_by", obj.Authorized_By);
            ins.Parameters.Add("defaults_used", obj.Defaults_Used ? 1:0);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Lab_Phys_Analysis obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Lab_Phys_Analysis obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Lab_Phys_Analysis SET");
            sql.AppendLine("lab_phys_type_id = :lab_phys_type_id, ");
            sql.AppendLine("line_nbr = :line_nbr, ");
            sql.AppendLine("analysis_date = :analysis_date, ");
            sql.AppendLine("shift_date8 = :shift_date8, ");
            sql.AppendLine("shift_nbr8 = :shift_nbr8, ");
            sql.AppendLine("shift_half8 = :shift_half8, ");
            sql.AppendLine("shift_date12 = :shift_date12, ");
            sql.AppendLine("shift_nbr12 = :shift_nbr12, ");
            sql.AppendLine("update_date = :update_date, ");
            sql.AppendLine("last_update_by = :last_update_by, ");
            sql.AppendLine("start_wgt = :start_wgt, ");
            sql.AppendLine("inch_1_wgt = :inch_1_wgt, ");
            sql.AppendLine("inch_3_4_wgt = :inch_3_4_wgt, ");
            sql.AppendLine("inch_5_8_wgt = :inch_5_8_wgt, ");
            sql.AppendLine("inch_9_16_wgt = :inch_9_16_wgt, ");
            sql.AppendLine("inch_1_2_wgt = :inch_1_2_wgt, ");
            sql.AppendLine("inch_7_16_wgt = :inch_7_16_wgt, ");
            sql.AppendLine("inch_3_8_wgt = :inch_3_8_wgt, ");
            sql.AppendLine("inch_1_4_wgt = :inch_1_4_wgt, ");
            sql.AppendLine("mesh_3_wgt = :mesh_3_wgt, ");
            sql.AppendLine("mesh_4_wgt = :mesh_4_wgt, ");
            sql.AppendLine("mesh_6_wgt = :mesh_6_wgt, ");
            sql.AppendLine("mesh_8_wgt = :mesh_8_wgt, ");
            sql.AppendLine("mesh_10_12_wgt = :mesh_10_12_wgt, ");
            sql.AppendLine("mesh_14_16_wgt = :mesh_14_16_wgt, ");
            sql.AppendLine("mesh_20_wgt = :mesh_20_wgt, ");
            sql.AppendLine("mesh_28_30_wgt = :mesh_28_30_wgt, ");
            sql.AppendLine("mesh_35_40_wgt = :mesh_35_40_wgt, ");
            sql.AppendLine("mesh_48_50_wgt = :mesh_48_50_wgt, ");
            sql.AppendLine("mesh_65_70_wgt = :mesh_65_70_wgt, ");
            sql.AppendLine("mesh_100_wgt = :mesh_100_wgt, ");
            sql.AppendLine("mesh_150_140_wgt = :mesh_150_140_wgt, ");
            sql.AppendLine("mesh_200_wgt = :mesh_200_wgt, ");
            sql.AppendLine("mesh_270_wgt = :mesh_270_wgt, ");
            sql.AppendLine("mesh_325_wgt = :mesh_325_wgt, ");
            sql.AppendLine("mesh_400_wgt = :mesh_400_wgt, ");
            sql.AppendLine("mesh_500_wgt = :mesh_500_wgt,");
            sql.AppendLine("authorized_by = :authorized_by,");
            sql.AppendLine("defaults_used = :defaults_used");
            sql.AppendLine("WHERE lab_phys_analysis_id = :lab_phys_analysis_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("lab_phys_type_id", obj.Lab_Phys_Type.Lab_Phys_Type_Id);
            upd.Parameters.Add("line_nbr", obj.Line_Nbr);
            upd.Parameters.Add("analysis_date", obj.Analysis_Date);
            upd.Parameters.Add("shift_date8", obj.Shift_Date8);
            upd.Parameters.Add("shift_nbr8", obj.Shift_Nbr8);
            upd.Parameters.Add("shift_half8", obj.Shift_Half8);
            upd.Parameters.Add("shift_date12", obj.Shift_Date12);
            upd.Parameters.Add("shift_nbr12", obj.Shift_Nbr12);
            upd.Parameters.Add("update_date", DateTime.Now);
            upd.Parameters.Add("last_update_by", obj.Last_Update_By);
            upd.Parameters.Add("start_wgt", obj.Start_Wgt);
            upd.Parameters.Add("inch_1_wgt", obj.Inch_1_Wgt);
            upd.Parameters.Add("inch_3_4_wgt", obj.Inch_3_4_Wgt);
            upd.Parameters.Add("inch_5_8_wgt", obj.Inch_5_8_Wgt);
            upd.Parameters.Add("inch_9_16_wgt", obj.Inch_9_16_Wgt);
            upd.Parameters.Add("inch_1_2_wgt", obj.Inch_1_2_Wgt);
            upd.Parameters.Add("inch_7_16_wgt", obj.Inch_7_16_Wgt);
            upd.Parameters.Add("inch_3_8_wgt", obj.Inch_3_8_Wgt);
            upd.Parameters.Add("inch_1_4_wgt", obj.Inch_1_4_Wgt);
            upd.Parameters.Add("mesh_3_wgt", obj.Mesh_3_Wgt);
            upd.Parameters.Add("mesh_4_wgt", obj.Mesh_4_Wgt);
            upd.Parameters.Add("mesh_6_wgt", obj.Mesh_6_Wgt);
            upd.Parameters.Add("mesh_8_wgt", obj.Mesh_8_Wgt);
            upd.Parameters.Add("mesh_10_12_wgt", obj.Mesh_10_12_Wgt);
            upd.Parameters.Add("mesh_14_16_wgt", obj.Mesh_14_16_Wgt);
            upd.Parameters.Add("mesh_20_wgt", obj.Mesh_20_Wgt);
            upd.Parameters.Add("mesh_28_30_wgt", obj.Mesh_28_30_Wgt);
            upd.Parameters.Add("mesh_35_40_wgt", obj.Mesh_35_40_Wgt);
            upd.Parameters.Add("mesh_48_50_wgt", obj.Mesh_48_50_Wgt);
            upd.Parameters.Add("mesh_65_70_wgt", obj.Mesh_65_70_Wgt);
            upd.Parameters.Add("mesh_100_wgt", obj.Mesh_100_Wgt);
            upd.Parameters.Add("mesh_150_140_wgt", obj.Mesh_150_140_Wgt);
            upd.Parameters.Add("mesh_200_wgt", obj.Mesh_200_Wgt);
            upd.Parameters.Add("mesh_270_wgt", obj.Mesh_270_Wgt);
            upd.Parameters.Add("mesh_325_wgt", obj.Mesh_325_Wgt);
            upd.Parameters.Add("mesh_400_wgt", obj.Mesh_400_Wgt);
            upd.Parameters.Add("mesh_500_wgt", obj.Mesh_500_Wgt);
            upd.Parameters.Add("authorized_by", obj.Authorized_By);
            upd.Parameters.Add("defaults_used", obj.Defaults_Used ? 1 : 0);
            upd.Parameters.Add("lab_phys_analysis_id", obj.Lab_Phys_Analysis_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Lab_Phys_Analysis obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Lab_Phys_Analysis obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.Lab_Phys_Analysis");
            sql.AppendLine("WHERE lab_phys_analysis_id = :lab_phys_analysis_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("lab_phys_analysis_id", obj.Lab_Phys_Analysis_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Lab_Phys_Analysis DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Lab_Phys_Analysis RetVal = new();
            RetVal.Lab_Phys_Analysis_Id = (int)Util.GetRowVal(row, $"{ColPrefix}lab_phys_analysis_id");
            RetVal.Lab_Phys_Type = Lab_Phys_TypeSvc.DataRowToObject(row, "lct_");
            RetVal.Line_Nbr = (int?)(short?)Util.GetRowVal(row, $"{ColPrefix}line_nbr");
            RetVal.Analysis_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}analysis_date");
            RetVal.Shift_Date8 = (DateTime)Util.GetRowVal(row, $"{ColPrefix}shift_date8");
            RetVal.Shift_Nbr8 = (short)Util.GetRowVal(row, $"{ColPrefix}shift_nbr8");
            RetVal.Shift_Half8 = (short?)Util.GetRowVal(row, $"{ColPrefix}shift_half8");
            RetVal.Shift_Date12 = (DateTime)Util.GetRowVal(row, $"{ColPrefix}shift_date12");
            RetVal.Shift_Nbr12 = (short)Util.GetRowVal(row, $"{ColPrefix}shift_nbr12");
            RetVal.Update_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}update_date");
            RetVal.Last_Update_By = (string)Util.GetRowVal(row, $"{ColPrefix}last_update_by");
            RetVal.Authorized_By = (string)Util.GetRowVal(row, $"{ColPrefix}authorized_by");
            RetVal.Defaults_Used = (short)Util.GetRowVal(row, $"{ColPrefix}defaults_used")==1;

            RetVal.Start_Wgt = (double?)Util.GetRowVal(row, $"{ColPrefix}start_wgt");
            RetVal.Inch_1_Wgt = (double?)Util.GetRowVal(row, $"{ColPrefix}inch_1_wgt");
            RetVal.Inch_3_4_Wgt = (double?)Util.GetRowVal(row, $"{ColPrefix}inch_3_4_wgt");
            RetVal.Inch_5_8_Wgt = (double?)Util.GetRowVal(row, $"{ColPrefix}inch_5_8_wgt");
            RetVal.Inch_9_16_Wgt = (double?)Util.GetRowVal(row, $"{ColPrefix}inch_9_16_wgt");
            RetVal.Inch_1_2_Wgt = (double?)Util.GetRowVal(row, $"{ColPrefix}inch_1_2_wgt");
            RetVal.Inch_7_16_Wgt = (double?)Util.GetRowVal(row, $"{ColPrefix}inch_7_16_wgt");
            RetVal.Inch_3_8_Wgt = (double?)Util.GetRowVal(row, $"{ColPrefix}inch_3_8_wgt");
            RetVal.Inch_1_4_Wgt = (double?)Util.GetRowVal(row, $"{ColPrefix}inch_1_4_wgt");
            RetVal.Mesh_3_Wgt = (double?)Util.GetRowVal(row, $"{ColPrefix}mesh_3_wgt");
            RetVal.Mesh_4_Wgt = (double?)Util.GetRowVal(row, $"{ColPrefix}mesh_4_wgt");
            RetVal.Mesh_6_Wgt = (double?)Util.GetRowVal(row, $"{ColPrefix}mesh_6_wgt");
            RetVal.Mesh_8_Wgt = (double?)Util.GetRowVal(row, $"{ColPrefix}mesh_8_wgt");
            RetVal.Mesh_10_12_Wgt = (double?)Util.GetRowVal(row, $"{ColPrefix}mesh_10_12_wgt");
            RetVal.Mesh_14_16_Wgt = (double?)Util.GetRowVal(row, $"{ColPrefix}mesh_14_16_wgt");
            RetVal.Mesh_20_Wgt = (double?)Util.GetRowVal(row, $"{ColPrefix}mesh_20_wgt");
            RetVal.Mesh_28_30_Wgt = (double?)Util.GetRowVal(row, $"{ColPrefix}mesh_28_30_wgt");
            RetVal.Mesh_35_40_Wgt = (double?)Util.GetRowVal(row, $"{ColPrefix}mesh_35_40_wgt");
            RetVal.Mesh_48_50_Wgt = (double?)Util.GetRowVal(row, $"{ColPrefix}mesh_48_50_wgt");
            RetVal.Mesh_65_70_Wgt = (double?)Util.GetRowVal(row, $"{ColPrefix}mesh_65_70_wgt");
            RetVal.Mesh_100_Wgt = (double?)Util.GetRowVal(row, $"{ColPrefix}mesh_100_wgt");
            RetVal.Mesh_150_140_Wgt = (double?)Util.GetRowVal(row, $"{ColPrefix}mesh_150_140_wgt");
            RetVal.Mesh_200_Wgt = (double?)Util.GetRowVal(row, $"{ColPrefix}mesh_200_wgt");
            RetVal.Mesh_270_Wgt = (double?)Util.GetRowVal(row, $"{ColPrefix}mesh_270_wgt");
            RetVal.Mesh_325_Wgt = (double?)Util.GetRowVal(row, $"{ColPrefix}mesh_325_wgt");
            RetVal.Mesh_400_Wgt = (double?)Util.GetRowVal(row, $"{ColPrefix}mesh_400_wgt");
            RetVal.Mesh_500_Wgt = (double?)Util.GetRowVal(row, $"{ColPrefix}mesh_500_wgt");
            return RetVal;
        }
    }
}
