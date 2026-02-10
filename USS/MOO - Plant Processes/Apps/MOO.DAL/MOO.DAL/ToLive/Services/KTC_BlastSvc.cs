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
    public class KTC_BlastSvc
    {
        static KTC_BlastSvc()
        {
            Util.RegisterOracle();
        }

        
        public static KTC_Blast Get(string bench_number, string blast_number)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE bench_number = :bench_number AND blast_number = :blast_number");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("bench_number", bench_number);
            da.SelectCommand.Parameters.Add("blast_number", blast_number);
            da.SelectCommand.BindByName = true;

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }


        public static List<KTC_Blast> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"ORDER BY blasted_date desc");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.BindByName = true;

            DataSet ds = MOO.Data.ExecuteQuery(da);
            List<KTC_Blast> elements = new();
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
            cols.AppendLine($"{ta}blasted_date {ColPrefix}blasted_date, {ta}bench_number {ColPrefix}bench_number, ");
            cols.AppendLine($"{ta}blast_number {ColPrefix}blast_number, {ta}blasted_time {ColPrefix}blasted_time, ");
            cols.AppendLine($"{ta}material {ColPrefix}material, {ta}property_location {ColPrefix}property_location, ");
            cols.AppendLine($"{ta}total_ft {ColPrefix}total_ft, {ta}total_sub_ft {ColPrefix}total_sub_ft, ");
            cols.AppendLine($"{ta}holes {ColPrefix}holes, {ta}hole_size_in {ColPrefix}hole_size_in, {ta}drill {ColPrefix}drill, ");
            cols.AppendLine($"{ta}depth_deep_avg_ft {ColPrefix}depth_deep_avg_ft, ");
            cols.AppendLine($"{ta}depth_shallow_avg {ColPrefix}depth_shallow_avg, {ta}area_deep {ColPrefix}area_deep, ");
            cols.AppendLine($"{ta}area_shallow {ColPrefix}area_shallow, {ta}blasted_deep_gt {ColPrefix}blasted_deep_gt, ");
            cols.AppendLine($"{ta}blasted_shallow_gt {ColPrefix}blasted_shallow_gt, {ta}stemming {ColPrefix}stemming, ");
            cols.AppendLine($"{ta}full_column_load {ColPrefix}full_column_load, ");
            cols.AppendLine($"{ta}noise_north_ktc_db {ColPrefix}noise_north_ktc_db, ");
            cols.AppendLine($"{ta}noise_west_ktc_db {ColPrefix}noise_west_ktc_db, ");
            cols.AppendLine($"{ta}burden_and_spacing {ColPrefix}burden_and_spacing, {ta}subgrade_ft {ColPrefix}subgrade_ft, ");
            cols.AppendLine($"{ta}explosive1_lb {ColPrefix}explosive1_lb, {ta}explosive1_uc {ColPrefix}explosive1_uc, ");
            cols.AppendLine($"{ta}explosive2_lb {ColPrefix}explosive2_lb, {ta}explosive2_uc {ColPrefix}explosive2_uc, ");
            cols.AppendLine($"{ta}primer_1lb {ColPrefix}primer_1lb, {ta}primer_1lb_uc {ColPrefix}primer_1lb_uc, ");
            cols.AppendLine($"{ta}a_chord_ft {ColPrefix}a_chord_ft, {ta}a_chord_ft_uc {ColPrefix}a_chord_ft_uc, ");
            cols.AppendLine($"{ta}ez_trunkline_40ft {ColPrefix}ez_trunkline_40ft, ");
            cols.AppendLine($"{ta}ez_trunkline_40ft_uc {ColPrefix}ez_trunkline_40ft_uc, ");
            cols.AppendLine($"{ta}ez_trunkline_60ft {ColPrefix}ez_trunkline_60ft, ");
            cols.AppendLine($"{ta}ez_trunkline_60ft_uc {ColPrefix}ez_trunkline_60ft_uc, ");
            cols.AppendLine($"{ta}primadets_30ft {ColPrefix}primadets_30ft, {ta}primadets_30ft_uc {ColPrefix}primadets_30ft_uc, ");
            cols.AppendLine($"{ta}primadets_40ft {ColPrefix}primadets_40ft, {ta}primadets_40ft_uc {ColPrefix}primadets_40ft_uc, ");
            cols.AppendLine($"{ta}primadets_50ft {ColPrefix}primadets_50ft, {ta}primadets_50ft_uc {ColPrefix}primadets_50ft_uc, ");
            cols.AppendLine($"{ta}caps_6ft {ColPrefix}caps_6ft, {ta}caps_6ft_uc {ColPrefix}caps_6ft_uc, ");
            cols.AppendLine($"{ta}blasting_wire_ft {ColPrefix}blasting_wire_ft, ");
            cols.AppendLine($"{ta}blasting_wire_ft_uc {ColPrefix}blasting_wire_ft_uc, {ta}blasters {ColPrefix}blasters, ");
            cols.AppendLine($"{ta}forman {ColPrefix}forman, {ta}engineer {ColPrefix}engineer, {ta}survey {ColPrefix}survey, ");
            cols.AppendLine($"{ta}cc {ColPrefix}cc, {ta}additional_db_site {ColPrefix}additional_db_site, ");
            cols.AppendLine($"{ta}additional_db {ColPrefix}additional_db, {ta}electric_det_15_met {ColPrefix}electric_det_15_met, ");
            cols.AppendLine($"{ta}electric_det_15_met_uc {ColPrefix}electric_det_15_met_uc, ");
            cols.AppendLine($"{ta}electric_det_20_met {ColPrefix}electric_det_20_met, ");
            cols.AppendLine($"{ta}electric_det_20_met_uc {ColPrefix}electric_det_20_met_uc, ");
            cols.AppendLine($"{ta}m35_bus_line_ft {ColPrefix}m35_bus_line_ft, ");
            cols.AppendLine($"{ta}m35_bus_line_ft_uc {ColPrefix}m35_bus_line_ft_uc");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.ktc_blast");
            return sql.ToString();
        }


        public static int Insert(KTC_Blast obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(KTC_Blast obj, OracleConnection conn)
        {

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO ToLive.ktc_blast(");
            sql.AppendLine("blasted_date, bench_number, blast_number, blasted_time, material, property_location, ");
            sql.AppendLine("total_ft, total_sub_ft, holes, hole_size_in, drill, depth_deep_avg_ft, ");
            sql.AppendLine("depth_shallow_avg, area_deep, area_shallow, blasted_deep_gt, blasted_shallow_gt, ");
            sql.AppendLine("stemming, full_column_load, noise_north_ktc_db, noise_west_ktc_db, ");
            sql.AppendLine("burden_and_spacing, subgrade_ft, explosive1_lb, explosive1_uc, explosive2_lb, ");
            sql.AppendLine("explosive2_uc, primer_1lb, primer_1lb_uc, a_chord_ft, a_chord_ft_uc, ");
            sql.AppendLine("ez_trunkline_40ft, ez_trunkline_40ft_uc, ez_trunkline_60ft, ez_trunkline_60ft_uc, ");
            sql.AppendLine("primadets_30ft, primadets_30ft_uc, primadets_40ft, primadets_40ft_uc, primadets_50ft, ");
            sql.AppendLine("primadets_50ft_uc, caps_6ft, caps_6ft_uc, blasting_wire_ft, blasting_wire_ft_uc, ");
            sql.AppendLine("blasters, forman, engineer, survey, cc, additional_db_site, additional_db, ");
            sql.AppendLine("electric_det_15_met, electric_det_15_met_uc, electric_det_20_met, ");
            sql.AppendLine("electric_det_20_met_uc, m35_bus_line_ft, m35_bus_line_ft_uc)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":blasted_date, :bench_number, :blast_number, :blasted_time, :material, ");
            sql.AppendLine(":property_location, :total_ft, :total_sub_ft, :holes, :hole_size_in, :drill, ");
            sql.AppendLine(":depth_deep_avg_ft, :depth_shallow_avg, :area_deep, :area_shallow, :blasted_deep_gt, ");
            sql.AppendLine(":blasted_shallow_gt, :stemming, :full_column_load, :noise_north_ktc_db, ");
            sql.AppendLine(":noise_west_ktc_db, :burden_and_spacing, :subgrade_ft, :explosive1_lb, :explosive1_uc, ");
            sql.AppendLine(":explosive2_lb, :explosive2_uc, :primer_1lb, :primer_1lb_uc, :a_chord_ft, ");
            sql.AppendLine(":a_chord_ft_uc, :ez_trunkline_40ft, :ez_trunkline_40ft_uc, :ez_trunkline_60ft, ");
            sql.AppendLine(":ez_trunkline_60ft_uc, :primadets_30ft, :primadets_30ft_uc, :primadets_40ft, ");
            sql.AppendLine(":primadets_40ft_uc, :primadets_50ft, :primadets_50ft_uc, :caps_6ft, :caps_6ft_uc, ");
            sql.AppendLine(":blasting_wire_ft, :blasting_wire_ft_uc, :blasters, :forman, :engineer, :survey, :cc, ");
            sql.AppendLine(":additional_db_site, :additional_db, :electric_det_15_met, :electric_det_15_met_uc, ");
            sql.AppendLine(":electric_det_20_met, :electric_det_20_met_uc, :m35_bus_line_ft, :m35_bus_line_ft_uc)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("blasted_date", obj.Blasted_Date);
            ins.Parameters.Add("bench_number", obj.Bench_Number);
            ins.Parameters.Add("blast_number", obj.Blast_Number);
            ins.Parameters.Add("blasted_time", obj.Blasted_Time);
            ins.Parameters.Add("material", obj.Material);
            ins.Parameters.Add("property_location", obj.Property_Location);
            ins.Parameters.Add("total_ft", obj.Total_Ft);
            ins.Parameters.Add("total_sub_ft", obj.Total_Sub_Ft);
            ins.Parameters.Add("holes", obj.Holes);
            ins.Parameters.Add("hole_size_in", obj.Hole_Size_In);
            ins.Parameters.Add("drill", obj.Drill);
            ins.Parameters.Add("depth_deep_avg_ft", obj.Depth_Deep_Avg_Ft);
            ins.Parameters.Add("depth_shallow_avg", obj.Depth_Shallow_Avg);
            ins.Parameters.Add("area_deep", obj.Area_Deep);
            ins.Parameters.Add("area_shallow", obj.Area_Shallow);
            ins.Parameters.Add("blasted_deep_gt", obj.Blasted_Deep_Gt);
            ins.Parameters.Add("blasted_shallow_gt", obj.Blasted_Shallow_Gt);
            ins.Parameters.Add("stemming", obj.Stemming);
            ins.Parameters.Add("full_column_load", obj.Full_Column_Load);
            ins.Parameters.Add("noise_north_ktc_db", obj.Noise_North_Ktc_Db);
            ins.Parameters.Add("noise_west_ktc_db", obj.Noise_West_Ktc_Db);
            ins.Parameters.Add("burden_and_spacing", obj.Burden_And_Spacing);
            ins.Parameters.Add("subgrade_ft", obj.Subgrade_Ft);
            ins.Parameters.Add("explosive1_lb", obj.Explosive1_Lb);
            ins.Parameters.Add("explosive1_uc", obj.Explosive1_Uc);
            ins.Parameters.Add("explosive2_lb", obj.Explosive2_Lb);
            ins.Parameters.Add("explosive2_uc", obj.Explosive2_Uc);
            ins.Parameters.Add("primer_1lb", obj.Primer_1lb);
            ins.Parameters.Add("primer_1lb_uc", obj.Primer_1lb_Uc);
            ins.Parameters.Add("a_chord_ft", obj.A_Chord_Ft);
            ins.Parameters.Add("a_chord_ft_uc", obj.A_Chord_Ft_Uc);
            ins.Parameters.Add("ez_trunkline_40ft", obj.Ez_Trunkline_40ft);
            ins.Parameters.Add("ez_trunkline_40ft_uc", obj.Ez_Trunkline_40ft_Uc);
            ins.Parameters.Add("ez_trunkline_60ft", obj.Ez_Trunkline_60ft);
            ins.Parameters.Add("ez_trunkline_60ft_uc", obj.Ez_Trunkline_60ft_Uc);
            ins.Parameters.Add("primadets_30ft", obj.Primadets_30ft);
            ins.Parameters.Add("primadets_30ft_uc", obj.Primadets_30ft_Uc);
            ins.Parameters.Add("primadets_40ft", obj.Primadets_40ft);
            ins.Parameters.Add("primadets_40ft_uc", obj.Primadets_40ft_Uc);
            ins.Parameters.Add("primadets_50ft", obj.Primadets_50ft);
            ins.Parameters.Add("primadets_50ft_uc", obj.Primadets_50ft_Uc);
            ins.Parameters.Add("caps_6ft", obj.Caps_6ft);
            ins.Parameters.Add("caps_6ft_uc", obj.Caps_6ft_Uc);
            ins.Parameters.Add("blasting_wire_ft", obj.Blasting_Wire_Ft);
            ins.Parameters.Add("blasting_wire_ft_uc", obj.Blasting_Wire_Ft_Uc);
            ins.Parameters.Add("blasters", obj.Blasters);
            ins.Parameters.Add("forman", obj.Forman);
            ins.Parameters.Add("engineer", obj.Engineer);
            ins.Parameters.Add("survey", obj.Survey);
            ins.Parameters.Add("cc", obj.Cc);
            ins.Parameters.Add("additional_db_site", obj.Additional_Db_Site);
            ins.Parameters.Add("additional_db", obj.Additional_Db);
            ins.Parameters.Add("electric_det_15_met", obj.Electric_Det_15_Met);
            ins.Parameters.Add("electric_det_15_met_uc", obj.Electric_Det_15_Met_Uc);
            ins.Parameters.Add("electric_det_20_met", obj.Electric_Det_20_Met);
            ins.Parameters.Add("electric_det_20_met_uc", obj.Electric_Det_20_Met_Uc);
            ins.Parameters.Add("m35_bus_line_ft", obj.M35_Bus_Line_Ft);
            ins.Parameters.Add("m35_bus_line_ft_uc", obj.M35_Bus_Line_Ft_Uc);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            if (recsAffected > 0)
            {
                //update the old values as they are now updated in the DB
                obj.Old_Blast_Number = obj.Blast_Number;
                obj.Old_Bench_Number = obj.Bench_Number;
            }
            return recsAffected;
        }


        public static int Update(KTC_Blast obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(KTC_Blast obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE ToLive.ktc_blast SET");
            sql.AppendLine("bench_number = :bench_number, ");
            sql.AppendLine("blast_number = :blast_number, ");
            sql.AppendLine("blasted_time = :blasted_time, ");
            sql.AppendLine("material = :material, ");
            sql.AppendLine("property_location = :property_location, ");
            sql.AppendLine("total_ft = :total_ft, ");
            sql.AppendLine("total_sub_ft = :total_sub_ft, ");
            sql.AppendLine("holes = :holes, ");
            sql.AppendLine("hole_size_in = :hole_size_in, ");
            sql.AppendLine("drill = :drill, ");
            sql.AppendLine("depth_deep_avg_ft = :depth_deep_avg_ft, ");
            sql.AppendLine("depth_shallow_avg = :depth_shallow_avg, ");
            sql.AppendLine("area_deep = :area_deep, ");
            sql.AppendLine("area_shallow = :area_shallow, ");
            sql.AppendLine("blasted_deep_gt = :blasted_deep_gt, ");
            sql.AppendLine("blasted_shallow_gt = :blasted_shallow_gt, ");
            sql.AppendLine("stemming = :stemming, ");
            sql.AppendLine("full_column_load = :full_column_load, ");
            sql.AppendLine("noise_north_ktc_db = :noise_north_ktc_db, ");
            sql.AppendLine("noise_west_ktc_db = :noise_west_ktc_db, ");
            sql.AppendLine("burden_and_spacing = :burden_and_spacing, ");
            sql.AppendLine("subgrade_ft = :subgrade_ft, ");
            sql.AppendLine("explosive1_lb = :explosive1_lb, ");
            sql.AppendLine("explosive1_uc = :explosive1_uc, ");
            sql.AppendLine("explosive2_lb = :explosive2_lb, ");
            sql.AppendLine("explosive2_uc = :explosive2_uc, ");
            sql.AppendLine("primer_1lb = :primer_1lb, ");
            sql.AppendLine("primer_1lb_uc = :primer_1lb_uc, ");
            sql.AppendLine("a_chord_ft = :a_chord_ft, ");
            sql.AppendLine("a_chord_ft_uc = :a_chord_ft_uc, ");
            sql.AppendLine("ez_trunkline_40ft = :ez_trunkline_40ft, ");
            sql.AppendLine("ez_trunkline_40ft_uc = :ez_trunkline_40ft_uc, ");
            sql.AppendLine("ez_trunkline_60ft = :ez_trunkline_60ft, ");
            sql.AppendLine("ez_trunkline_60ft_uc = :ez_trunkline_60ft_uc, ");
            sql.AppendLine("primadets_30ft = :primadets_30ft, ");
            sql.AppendLine("primadets_30ft_uc = :primadets_30ft_uc, ");
            sql.AppendLine("primadets_40ft = :primadets_40ft, ");
            sql.AppendLine("primadets_40ft_uc = :primadets_40ft_uc, ");
            sql.AppendLine("primadets_50ft = :primadets_50ft, ");
            sql.AppendLine("primadets_50ft_uc = :primadets_50ft_uc, ");
            sql.AppendLine("caps_6ft = :caps_6ft, ");
            sql.AppendLine("caps_6ft_uc = :caps_6ft_uc, ");
            sql.AppendLine("blasting_wire_ft = :blasting_wire_ft, ");
            sql.AppendLine("blasting_wire_ft_uc = :blasting_wire_ft_uc, ");
            sql.AppendLine("blasters = :blasters, ");
            sql.AppendLine("forman = :forman, ");
            sql.AppendLine("engineer = :engineer, ");
            sql.AppendLine("survey = :survey, ");
            sql.AppendLine("cc = :cc, ");
            sql.AppendLine("additional_db_site = :additional_db_site, ");
            sql.AppendLine("additional_db = :additional_db, ");
            sql.AppendLine("electric_det_15_met = :electric_det_15_met, ");
            sql.AppendLine("electric_det_15_met_uc = :electric_det_15_met_uc, ");
            sql.AppendLine("electric_det_20_met = :electric_det_20_met, ");
            sql.AppendLine("electric_det_20_met_uc = :electric_det_20_met_uc, ");
            sql.AppendLine("m35_bus_line_ft = :m35_bus_line_ft, ");
            sql.AppendLine("m35_bus_line_ft_uc = :m35_bus_line_ft_uc,");
            sql.AppendLine("blasted_date = :blasted_date");

            sql.AppendLine("WHERE bench_number = :old_bench_number");
            sql.AppendLine("AND blast_number = :old_blast_number");

            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("bench_number", obj.Bench_Number);
            upd.Parameters.Add("blast_number", obj.Blast_Number);
            upd.Parameters.Add("blasted_time", obj.Blasted_Time);
            upd.Parameters.Add("material", obj.Material);
            upd.Parameters.Add("property_location", obj.Property_Location);
            upd.Parameters.Add("total_ft", obj.Total_Ft);
            upd.Parameters.Add("total_sub_ft", obj.Total_Sub_Ft);
            upd.Parameters.Add("holes", obj.Holes);
            upd.Parameters.Add("hole_size_in", obj.Hole_Size_In);
            upd.Parameters.Add("drill", obj.Drill);
            upd.Parameters.Add("depth_deep_avg_ft", obj.Depth_Deep_Avg_Ft);
            upd.Parameters.Add("depth_shallow_avg", obj.Depth_Shallow_Avg);
            upd.Parameters.Add("area_deep", obj.Area_Deep);
            upd.Parameters.Add("area_shallow", obj.Area_Shallow);
            upd.Parameters.Add("blasted_deep_gt", obj.Blasted_Deep_Gt);
            upd.Parameters.Add("blasted_shallow_gt", obj.Blasted_Shallow_Gt);
            upd.Parameters.Add("stemming", obj.Stemming);
            upd.Parameters.Add("full_column_load", obj.Full_Column_Load);
            upd.Parameters.Add("noise_north_ktc_db", obj.Noise_North_Ktc_Db);
            upd.Parameters.Add("noise_west_ktc_db", obj.Noise_West_Ktc_Db);
            upd.Parameters.Add("burden_and_spacing", obj.Burden_And_Spacing);
            upd.Parameters.Add("subgrade_ft", obj.Subgrade_Ft);
            upd.Parameters.Add("explosive1_lb", obj.Explosive1_Lb);
            upd.Parameters.Add("explosive1_uc", obj.Explosive1_Uc);
            upd.Parameters.Add("explosive2_lb", obj.Explosive2_Lb);
            upd.Parameters.Add("explosive2_uc", obj.Explosive2_Uc);
            upd.Parameters.Add("primer_1lb", obj.Primer_1lb);
            upd.Parameters.Add("primer_1lb_uc", obj.Primer_1lb_Uc);
            upd.Parameters.Add("a_chord_ft", obj.A_Chord_Ft);
            upd.Parameters.Add("a_chord_ft_uc", obj.A_Chord_Ft_Uc);
            upd.Parameters.Add("ez_trunkline_40ft", obj.Ez_Trunkline_40ft);
            upd.Parameters.Add("ez_trunkline_40ft_uc", obj.Ez_Trunkline_40ft_Uc);
            upd.Parameters.Add("ez_trunkline_60ft", obj.Ez_Trunkline_60ft);
            upd.Parameters.Add("ez_trunkline_60ft_uc", obj.Ez_Trunkline_60ft_Uc);
            upd.Parameters.Add("primadets_30ft", obj.Primadets_30ft);
            upd.Parameters.Add("primadets_30ft_uc", obj.Primadets_30ft_Uc);
            upd.Parameters.Add("primadets_40ft", obj.Primadets_40ft);
            upd.Parameters.Add("primadets_40ft_uc", obj.Primadets_40ft_Uc);
            upd.Parameters.Add("primadets_50ft", obj.Primadets_50ft);
            upd.Parameters.Add("primadets_50ft_uc", obj.Primadets_50ft_Uc);
            upd.Parameters.Add("caps_6ft", obj.Caps_6ft);
            upd.Parameters.Add("caps_6ft_uc", obj.Caps_6ft_Uc);
            upd.Parameters.Add("blasting_wire_ft", obj.Blasting_Wire_Ft);
            upd.Parameters.Add("blasting_wire_ft_uc", obj.Blasting_Wire_Ft_Uc);
            upd.Parameters.Add("blasters", obj.Blasters);
            upd.Parameters.Add("forman", obj.Forman);
            upd.Parameters.Add("engineer", obj.Engineer);
            upd.Parameters.Add("survey", obj.Survey);
            upd.Parameters.Add("cc", obj.Cc);
            upd.Parameters.Add("additional_db_site", obj.Additional_Db_Site);
            upd.Parameters.Add("additional_db", obj.Additional_Db);
            upd.Parameters.Add("electric_det_15_met", obj.Electric_Det_15_Met);
            upd.Parameters.Add("electric_det_15_met_uc", obj.Electric_Det_15_Met_Uc);
            upd.Parameters.Add("electric_det_20_met", obj.Electric_Det_20_Met);
            upd.Parameters.Add("electric_det_20_met_uc", obj.Electric_Det_20_Met_Uc);
            upd.Parameters.Add("m35_bus_line_ft", obj.M35_Bus_Line_Ft);
            upd.Parameters.Add("m35_bus_line_ft_uc", obj.M35_Bus_Line_Ft_Uc);
            upd.Parameters.Add("blasted_date", obj.Blasted_Date);

            upd.Parameters.Add("old_bench_number", obj.Old_Bench_Number);
            upd.Parameters.Add("old_blast_number", obj.Old_Blast_Number);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            if (recsAffected > 0)
            {
                //update the old values as they are now updated in the DB
                obj.Old_Blast_Number = obj.Blast_Number;
                obj.Old_Bench_Number = obj.Bench_Number;
            }
            return recsAffected;
        }


        public static int Delete(KTC_Blast obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(KTC_Blast obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM ToLive.ktc_blast");
            sql.AppendLine("WHERE bench_number = :bench_number AND blast_number = :blast_number");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("bench_number", obj.Old_Bench_Number);
            del.Parameters.Add("blast_number", obj.Old_Blast_Number);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static KTC_Blast DataRowToObject(DataRow row, string ColPrefix = "")
        {
            KTC_Blast RetVal = new();
            RetVal.Blasted_Date = row.Field<DateTime>($"{ColPrefix}blasted_date");
            RetVal.Bench_Number = row.Field<string>($"{ColPrefix}bench_number");
            RetVal.Blast_Number = row.Field<string>($"{ColPrefix}blast_number");
            RetVal.Blasted_Time = row.Field<decimal?>($"{ColPrefix}blasted_time");
            RetVal.Material = row.Field<string>($"{ColPrefix}material");
            RetVal.Property_Location = row.Field<string>($"{ColPrefix}property_location");
            RetVal.Total_Ft = row.Field<decimal?>($"{ColPrefix}total_ft");
            RetVal.Total_Sub_Ft = row.Field<decimal?>($"{ColPrefix}total_sub_ft");
            RetVal.Holes = row.Field<decimal?>($"{ColPrefix}holes");
            RetVal.Hole_Size_In = row.Field<decimal?>($"{ColPrefix}hole_size_in");
            RetVal.Drill = row.Field<string>($"{ColPrefix}drill");
            RetVal.Depth_Deep_Avg_Ft = row.Field<decimal?>($"{ColPrefix}depth_deep_avg_ft");
            RetVal.Depth_Shallow_Avg = row.Field<decimal?>($"{ColPrefix}depth_shallow_avg");
            RetVal.Area_Deep = row.Field<decimal?>($"{ColPrefix}area_deep");
            RetVal.Area_Shallow = row.Field<decimal?>($"{ColPrefix}area_shallow");
            RetVal.Blasted_Deep_Gt = row.Field<decimal?>($"{ColPrefix}blasted_deep_gt");
            RetVal.Blasted_Shallow_Gt = row.Field<decimal?>($"{ColPrefix}blasted_shallow_gt");
            RetVal.Stemming = row.Field<decimal?>($"{ColPrefix}stemming");
            RetVal.Full_Column_Load = row.Field<string>($"{ColPrefix}full_column_load");
            RetVal.Noise_North_Ktc_Db = row.Field<decimal?>($"{ColPrefix}noise_north_ktc_db");
            RetVal.Noise_West_Ktc_Db = row.Field<decimal?>($"{ColPrefix}noise_west_ktc_db");
            RetVal.Burden_And_Spacing = row.Field<string>($"{ColPrefix}burden_and_spacing");
            RetVal.Subgrade_Ft = row.Field<string>($"{ColPrefix}subgrade_ft");
            RetVal.Explosive1_Lb = row.Field<decimal?>($"{ColPrefix}explosive1_lb");
            RetVal.Explosive1_Uc = row.Field<decimal?>($"{ColPrefix}explosive1_uc");
            RetVal.Explosive2_Lb = row.Field<decimal?>($"{ColPrefix}explosive2_lb");
            RetVal.Explosive2_Uc = row.Field<decimal?>($"{ColPrefix}explosive2_uc");
            RetVal.Primer_1lb = row.Field<decimal?>($"{ColPrefix}primer_1lb");
            RetVal.Primer_1lb_Uc = row.Field<decimal?>($"{ColPrefix}primer_1lb_uc");
            RetVal.A_Chord_Ft = row.Field<decimal?>($"{ColPrefix}a_chord_ft");
            RetVal.A_Chord_Ft_Uc = row.Field<decimal?>($"{ColPrefix}a_chord_ft_uc");
            RetVal.Ez_Trunkline_40ft = row.Field<decimal?>($"{ColPrefix}ez_trunkline_40ft");
            RetVal.Ez_Trunkline_40ft_Uc = row.Field<decimal?>($"{ColPrefix}ez_trunkline_40ft_uc");
            RetVal.Ez_Trunkline_60ft = row.Field<decimal?>($"{ColPrefix}ez_trunkline_60ft");
            RetVal.Ez_Trunkline_60ft_Uc = row.Field<decimal?>($"{ColPrefix}ez_trunkline_60ft_uc");
            RetVal.Primadets_30ft = row.Field<decimal?>($"{ColPrefix}primadets_30ft");
            RetVal.Primadets_30ft_Uc = row.Field<decimal?>($"{ColPrefix}primadets_30ft_uc");
            RetVal.Primadets_40ft = row.Field<decimal?>($"{ColPrefix}primadets_40ft");
            RetVal.Primadets_40ft_Uc = row.Field<decimal?>($"{ColPrefix}primadets_40ft_uc");
            RetVal.Primadets_50ft = row.Field<decimal?>($"{ColPrefix}primadets_50ft");
            RetVal.Primadets_50ft_Uc = row.Field<decimal?>($"{ColPrefix}primadets_50ft_uc");
            RetVal.Caps_6ft = row.Field<decimal?>($"{ColPrefix}caps_6ft");
            RetVal.Caps_6ft_Uc = row.Field<decimal?>($"{ColPrefix}caps_6ft_uc");
            RetVal.Blasting_Wire_Ft = row.Field<decimal?>($"{ColPrefix}blasting_wire_ft");
            RetVal.Blasting_Wire_Ft_Uc = row.Field<decimal?>($"{ColPrefix}blasting_wire_ft_uc");
            RetVal.Blasters = row.Field<string>($"{ColPrefix}blasters");
            RetVal.Forman = row.Field<string>($"{ColPrefix}forman");
            RetVal.Engineer = row.Field<string>($"{ColPrefix}engineer");
            RetVal.Survey = row.Field<string>($"{ColPrefix}survey");
            RetVal.Cc = row.Field<string>($"{ColPrefix}cc");
            RetVal.Additional_Db_Site = row.Field<string>($"{ColPrefix}additional_db_site");
            RetVal.Additional_Db = row.Field<decimal?>($"{ColPrefix}additional_db");
            RetVal.Electric_Det_15_Met = row.Field<decimal>($"{ColPrefix}electric_det_15_met");
            RetVal.Electric_Det_15_Met_Uc = row.Field<decimal>($"{ColPrefix}electric_det_15_met_uc");
            RetVal.Electric_Det_20_Met = row.Field<decimal>($"{ColPrefix}electric_det_20_met");
            RetVal.Electric_Det_20_Met_Uc = row.Field<decimal>($"{ColPrefix}electric_det_20_met_uc");
            RetVal.M35_Bus_Line_Ft = row.Field<decimal>($"{ColPrefix}m35_bus_line_ft");
            RetVal.M35_Bus_Line_Ft_Uc = row.Field<decimal>($"{ColPrefix}m35_bus_line_ft_uc");

            RetVal.Old_Bench_Number = RetVal.Bench_Number;
            RetVal.Old_Blast_Number = RetVal.Blast_Number;

            return RetVal;
        }

    }
}
