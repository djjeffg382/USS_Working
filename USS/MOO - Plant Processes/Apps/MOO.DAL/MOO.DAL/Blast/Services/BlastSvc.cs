using MOO.DAL.Blast.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using MOO.DAL.ToLive.Enums;
using MOO.DAL.Blast.Enums;

namespace MOO.DAL.Blast.Services
{
    public static class BlastSvc
    {
        static BlastSvc()
        {

            Util.RegisterOracle();
        }

        public static Models.Blast Get(int id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE bt.id = :id");


            Models.Blast retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("id", id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr, "bt_");
            }
            conn.Close();
            return retVal;
        }

        //This is a function that grabs the drill number from cat_drills, through blast_drills, based on an entered blast id
        public static List<CAT_Drills> GetDrills(int id)
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(CAT_DrillsSvc.GetColumns("cd", "cd_"));
            sql.AppendLine("FROM blast.blast_drills bd");
            sql.AppendLine("JOIN blast.cat_drills cd ON bd.drill_id = cd.id");

            sql.AppendLine($"WHERE bd.blast_id = :blast_id");

            List<CAT_Drills> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("blast_id", id);
            cmd.BindByName = true;

            var rdr = MOO.Data.ExecuteReader(cmd);

            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    CAT_Drills item = CAT_DrillsSvc.DataRowToObject(rdr, "cd_");
                    elements.Add(item);
                }
            }
            conn.Close();
            return elements;
        }

        public static List<Models.Blast> GetByDateRange(DateTime StartDate, DateTime EndDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE blast_meeting_time BETWEEN :StartDate AND :EndDate");

            List<Models.Blast> elements = new();
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
                    elements.Add(DataRowToObject(rdr, "bt_"));
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
            cols.AppendLine($"{ta}id {ColPrefix}id, {ta}pattern_id {ColPrefix}pattern_id, ");
            cols.AppendLine($"{ta}blast_meeting_time {ColPrefix}blast_meeting_time, {ta}wind_direction {ColPrefix}wind_direction, ");
            cols.AppendLine($"{ta}wind_speed_mph {ColPrefix}wind_speed_mph, {ta}delay_lbs {ColPrefix}delay_lbs, ");
            cols.AppendLine($"{ta}blast_agent_lbs {ColPrefix}blast_agent_lbs, {ta}pit_id {ColPrefix}pit_id, ");
            cols.AppendLine($"{ta}rock_type_id {ColPrefix}rock_type_id, {ta}taconite_tons {ColPrefix}taconite_tons, ");
            cols.AppendLine($"{ta}waste_tons {ColPrefix}waste_tons, {ta}sky_cond {ColPrefix}sky_cond, ");
            cols.AppendLine($"{ta}temp_2200 {ColPrefix}temp_2200, {ta}temp_2700 {ColPrefix}temp_2700, ");
            cols.AppendLine($"{ta}temp_3200 {ColPrefix}temp_3200, {ta}temp_3700 {ColPrefix}temp_3700, ");
            cols.AppendLine($"{ta}temp_4200 {ColPrefix}temp_4200, {ta}temp_4700 {ColPrefix}temp_4700, ");
            cols.AppendLine($"{ta}temp_5200 {ColPrefix}temp_5200, {ta}temp_5700 {ColPrefix}temp_5700, ");
            cols.AppendLine($"{ta}temp_6200 {ColPrefix}temp_6200, {ta}temp_6700 {ColPrefix}temp_6700, ");
            cols.AppendLine($"{ta}open_sinking {ColPrefix}open_sinking, {ta}flying {ColPrefix}flying, ");
            cols.AppendLine($"{ta}position_code {ColPrefix}position_code, {ta}pattern_size_id {ColPrefix}pattern_size_id, ");
            cols.AppendLine($"{ta}northing {ColPrefix}northing, {ta}easting {ColPrefix}easting, ");
            cols.AppendLine($"{ta}blast_time {ColPrefix}blast_time, {ta}temp_surface {ColPrefix}temp_surface, ");
            cols.AppendLine($"{ta}test_lbs {ColPrefix}test_lbs, {ta}complaint {ColPrefix}complaint, ");
            cols.AppendLine($"{ta}comments {ColPrefix}comments, {ta}barometric_pressure {ColPrefix}barometric_pressure, ");
            cols.AppendLine($"{ta}subdrill {ColPrefix}subdrill");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns("bt", "bt_") + ",");
            sql.AppendLine(PatternSvc.GetColumns("pt", "pt_") + ",");
            sql.AppendLine(CAT_PitsSvc.GetColumns("cp", "cp_") + ",");
            sql.AppendLine(CAT_Rock_TypeSvc.GetColumns("rt", "rt_"));
            sql.AppendLine("FROM blast.blast bt");
            sql.AppendLine("JOIN blast.pattern pt ON bt.pattern_id = pt.id");
            sql.AppendLine("JOIN blast.cat_pits cp ON bt.pit_id = cp.id");
            sql.AppendLine("JOIN blast.cat_rock_type rt ON bt.rock_type_id = rt.id");
            return sql.ToString();
        }


        public static int Insert(Models.Blast obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Models.Blast obj, OracleConnection conn)
        {
            if (obj.Id <= 0)
                obj.Id = Convert.ToInt32(MOO.Data.GetNextSequence("Blast.SEQ_BLAST"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO Blast.Blast(");
            sql.AppendLine("id, pattern_id, blast_meeting_time, wind_direction, wind_speed_mph, delay_lbs, ");
            sql.AppendLine("blast_agent_lbs, pit_id, rock_type_id, taconite_tons, waste_tons, sky_cond, ");
            sql.AppendLine("temp_2200, temp_2700, temp_3200, temp_3700, temp_4200, temp_4700, temp_5200, ");
            sql.AppendLine("temp_5700, temp_6200, temp_6700, open_sinking, flying, position_code, ");
            sql.AppendLine("pattern_size_id, northing, easting, blast_time, temp_surface, test_lbs, complaint, ");
            sql.AppendLine("comments, barometric_pressure, subdrill)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":id, :pattern_id, :blast_meeting_time, :wind_direction, :wind_speed_mph, :delay_lbs, ");
            sql.AppendLine(":blast_agent_lbs, :pit_id, :rock_type_id, :taconite_tons, :waste_tons, :sky_cond, ");
            sql.AppendLine(":temp_2200, :temp_2700, :temp_3200, :temp_3700, :temp_4200, :temp_4700, :temp_5200, ");
            sql.AppendLine(":temp_5700, :temp_6200, :temp_6700, :open_sinking, :flying, :position_code, ");
            sql.AppendLine(":pattern_size_id, :northing, :easting, :blast_time, :temp_surface, :test_lbs, ");
            sql.AppendLine(":complaint, :comments, :barometric_pressure, :subdrill)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("id", obj.Id);
            ins.Parameters.Add("pattern_id", obj.Pattern.Id);
            ins.Parameters.Add("blast_meeting_time", obj.Blast_Meeting_Time);
            ins.Parameters.Add("wind_direction", obj.Wind_Direction);
            ins.Parameters.Add("wind_speed_mph", obj.Wind_Speed_Mph);
            ins.Parameters.Add("delay_lbs", obj.Delay_Lbs);
            ins.Parameters.Add("blast_agent_lbs", obj.Blast_Agent_Lbs);
            ins.Parameters.Add("pit_id", obj.Pit.Id);
            ins.Parameters.Add("rock_type_id", obj.Rock_Type.Id);
            ins.Parameters.Add("taconite_tons", obj.Taconite_Tons);
            ins.Parameters.Add("waste_tons", obj.Waste_Tons);
            ins.Parameters.Add("sky_cond", obj.Sky_Cond);
            ins.Parameters.Add("temp_2200", obj.Temp_2200);
            ins.Parameters.Add("temp_2700", obj.Temp_2700);
            ins.Parameters.Add("temp_3200", obj.Temp_3200);
            ins.Parameters.Add("temp_3700", obj.Temp_3700);
            ins.Parameters.Add("temp_4200", obj.Temp_4200);
            ins.Parameters.Add("temp_4700", obj.Temp_4700);
            ins.Parameters.Add("temp_5200", obj.Temp_5200);
            ins.Parameters.Add("temp_5700", obj.Temp_5700);
            ins.Parameters.Add("temp_6200", obj.Temp_6200);
            ins.Parameters.Add("temp_6700", obj.Temp_6700);
            ins.Parameters.Add("open_sinking", obj.Open_Sinking.ToString());
            ins.Parameters.Add("flying", obj.Flying);
            ins.Parameters.Add("position_code", obj.Position_Code);
            ins.Parameters.Add("pattern_size_id", obj.Pattern_Size_Id);
            ins.Parameters.Add("northing", obj.Northing);
            ins.Parameters.Add("easting", obj.Easting);
            ins.Parameters.Add("blast_time", obj.Blast_Time);
            ins.Parameters.Add("temp_surface", obj.Temp_Surface);
            ins.Parameters.Add("test_lbs", obj.Test_Lbs);
            ins.Parameters.Add("complaint", obj.Complaint);
            ins.Parameters.Add("comments", obj.Comments);
            ins.Parameters.Add("barometric_pressure", obj.Barometric_Pressure);
            ins.Parameters.Add("subdrill", obj.Subdrill);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Models.Blast obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Models.Blast obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE Blast.Blast SET");
            sql.AppendLine("pattern_id = :pattern_id, ");
            sql.AppendLine("blast_meeting_time = :blast_meeting_time, ");
            sql.AppendLine("wind_direction = :wind_direction, ");
            sql.AppendLine("wind_speed_mph = :wind_speed_mph, ");
            sql.AppendLine("delay_lbs = :delay_lbs, ");
            sql.AppendLine("blast_agent_lbs = :blast_agent_lbs, ");
            sql.AppendLine("pit_id = :pit_id, ");
            sql.AppendLine("rock_type_id = :rock_type_id, ");
            sql.AppendLine("taconite_tons = :taconite_tons, ");
            sql.AppendLine("waste_tons = :waste_tons, ");
            sql.AppendLine("sky_cond = :sky_cond, ");
            sql.AppendLine("temp_2200 = :temp_2200, ");
            sql.AppendLine("temp_2700 = :temp_2700, ");
            sql.AppendLine("temp_3200 = :temp_3200, ");
            sql.AppendLine("temp_3700 = :temp_3700, ");
            sql.AppendLine("temp_4200 = :temp_4200, ");
            sql.AppendLine("temp_4700 = :temp_4700, ");
            sql.AppendLine("temp_5200 = :temp_5200, ");
            sql.AppendLine("temp_5700 = :temp_5700, ");
            sql.AppendLine("temp_6200 = :temp_6200, ");
            sql.AppendLine("temp_6700 = :temp_6700, ");
            sql.AppendLine("open_sinking = :open_sinking, ");
            sql.AppendLine("flying = :flying, ");
            sql.AppendLine("position_code = :position_code, ");
            sql.AppendLine("pattern_size_id = :pattern_size_id, ");
            sql.AppendLine("northing = :northing, ");
            sql.AppendLine("easting = :easting, ");
            sql.AppendLine("blast_time = :blast_time, ");
            sql.AppendLine("temp_surface = :temp_surface, ");
            sql.AppendLine("test_lbs = :test_lbs, ");
            sql.AppendLine("complaint = :complaint, ");
            sql.AppendLine("comments = :comments, ");
            sql.AppendLine("barometric_pressure = :barometric_pressure, ");
            sql.AppendLine("subdrill = :subdrill");
            sql.AppendLine("WHERE id = :id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("blast_meeting_time", obj.Blast_Meeting_Time);
            upd.Parameters.Add("wind_direction", obj.Wind_Direction);
            upd.Parameters.Add("wind_speed_mph", obj.Wind_Speed_Mph);
            upd.Parameters.Add("delay_lbs", obj.Delay_Lbs);
            upd.Parameters.Add("blast_agent_lbs", obj.Blast_Agent_Lbs);
            upd.Parameters.Add("pit_id", obj.Pit.Id);
            upd.Parameters.Add("rock_type_id", obj.Rock_Type.Id);
            upd.Parameters.Add("taconite_tons", obj.Taconite_Tons);
            upd.Parameters.Add("waste_tons", obj.Waste_Tons);
            upd.Parameters.Add("sky_cond", obj.Sky_Cond);
            upd.Parameters.Add("temp_2200", obj.Temp_2200);
            upd.Parameters.Add("temp_2700", obj.Temp_2700);
            upd.Parameters.Add("temp_3200", obj.Temp_3200);
            upd.Parameters.Add("temp_3700", obj.Temp_3700);
            upd.Parameters.Add("temp_4200", obj.Temp_4200);
            upd.Parameters.Add("temp_4700", obj.Temp_4700);
            upd.Parameters.Add("temp_5200", obj.Temp_5200);
            upd.Parameters.Add("temp_5700", obj.Temp_5700);
            upd.Parameters.Add("temp_6200", obj.Temp_6200);
            upd.Parameters.Add("temp_6700", obj.Temp_6700);
            upd.Parameters.Add("open_sinking", obj.Open_Sinking.ToString());
            upd.Parameters.Add("flying", obj.Flying);
            upd.Parameters.Add("position_code", obj.Position_Code);
            upd.Parameters.Add("pattern_size_id", obj.Pattern_Size_Id);
            upd.Parameters.Add("northing", obj.Northing);
            upd.Parameters.Add("easting", obj.Easting);
            upd.Parameters.Add("blast_time", obj.Blast_Time);
            upd.Parameters.Add("temp_surface", obj.Temp_Surface);
            upd.Parameters.Add("test_lbs", obj.Test_Lbs);
            upd.Parameters.Add("complaint", obj.Complaint);
            upd.Parameters.Add("comments", obj.Comments);
            upd.Parameters.Add("barometric_pressure", obj.Barometric_Pressure);
            upd.Parameters.Add("subdrill", obj.Subdrill);
            upd.Parameters.Add("pattern_id", obj.Pattern.Id);
            upd.Parameters.Add("id", obj.Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Models.Blast obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Models.Blast obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM Blast.Blast");
            sql.AppendLine("WHERE id = :id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("id", obj.Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }

  
        internal static Models.Blast DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Models.Blast RetVal = new();
            RetVal.Id = (int)(decimal)Util.GetRowVal(row, $"{ColPrefix}id");
            RetVal.Pattern = PatternSvc.DataRowToObject(row, "pt_");
            RetVal.Blast_Meeting_Time = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}blast_meeting_time");
            RetVal.Wind_Direction = (decimal?)Util.GetRowVal(row, $"{ColPrefix}wind_direction");
            RetVal.Wind_Speed_Mph = (decimal?)Util.GetRowVal(row, $"{ColPrefix}wind_speed_mph");
            RetVal.Delay_Lbs = (decimal?)Util.GetRowVal(row, $"{ColPrefix}delay_lbs");
            RetVal.Blast_Agent_Lbs = (decimal?)Util.GetRowVal(row, $"{ColPrefix}blast_agent_lbs");
            RetVal.Pit = CAT_PitsSvc.DataRowToObject(row, $"cp_");
            RetVal.Rock_Type = CAT_Rock_TypeSvc.DataRowToObject(row, $"rt_");
            RetVal.Taconite_Tons = (decimal?)Util.GetRowVal(row, $"{ColPrefix}taconite_tons");
            RetVal.Waste_Tons = (decimal?)Util.GetRowVal(row, $"{ColPrefix}waste_tons");
            RetVal.Sky_Cond = (string)Util.GetRowVal(row, $"{ColPrefix}sky_cond");
            RetVal.Temp_2200 = (decimal?)Util.GetRowVal(row, $"{ColPrefix}temp_2200");
            RetVal.Temp_2700 = (decimal?)Util.GetRowVal(row, $"{ColPrefix}temp_2700");
            RetVal.Temp_3200 = (decimal?)Util.GetRowVal(row, $"{ColPrefix}temp_3200");
            RetVal.Temp_3700 = (decimal?)Util.GetRowVal(row, $"{ColPrefix}temp_3700");
            RetVal.Temp_4200 = (decimal?)Util.GetRowVal(row, $"{ColPrefix}temp_4200");
            RetVal.Temp_4700 = (decimal?)Util.GetRowVal(row, $"{ColPrefix}temp_4700");
            RetVal.Temp_5200 = (decimal?)Util.GetRowVal(row, $"{ColPrefix}temp_5200");
            RetVal.Temp_5700 = (decimal?)Util.GetRowVal(row, $"{ColPrefix}temp_5700");
            RetVal.Temp_6200 = (decimal?)Util.GetRowVal(row, $"{ColPrefix}temp_6200");
            RetVal.Temp_6700 = (decimal?)Util.GetRowVal(row, $"{ColPrefix}temp_6700");
            string OpenSinking = Util.GetRowVal(row, $"{ColPrefix}open_sinking").ToString();
            RetVal.Open_Sinking = Enum.Parse<Open_Sinking_Type>(OpenSinking);
            RetVal.Flying = (string)Util.GetRowVal(row, $"{ColPrefix}flying");
            RetVal.Position_Code = (string)Util.GetRowVal(row, $"{ColPrefix}position_code");
            RetVal.Pattern_Size_Id = (int?)(decimal?)Util.GetRowVal(row, $"{ColPrefix}pattern_size_id");
            RetVal.Northing = (long?)(decimal?)Util.GetRowVal(row, $"{ColPrefix}northing");
            RetVal.Easting = (long?)(decimal?)Util.GetRowVal(row, $"{ColPrefix}easting");
            RetVal.Blast_Time = (string)Util.GetRowVal(row, $"{ColPrefix}blast_time");
            RetVal.Temp_Surface = (decimal?)Util.GetRowVal(row, $"{ColPrefix}temp_surface");
            RetVal.Test_Lbs = (decimal?)Util.GetRowVal(row, $"{ColPrefix}test_lbs");
            RetVal.Complaint = (decimal?)Util.GetRowVal(row, $"{ColPrefix}complaint");
            RetVal.Comments = (string)Util.GetRowVal(row, $"{ColPrefix}comments");
            RetVal.Barometric_Pressure = (decimal?)Util.GetRowVal(row, $"{ColPrefix}barometric_pressure");
            RetVal.Subdrill = (short)Util.GetRowVal(row, $"{ColPrefix}subdrill");
            return RetVal;
        }
    }
}
