using MOO.DAL.Blast.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Runtime.Intrinsics.Arm;


namespace MOO.DAL.Blast.Services
{
    public static class Pattern_HolesSvc
    {
        static Pattern_HolesSvc()
        {
            Util.RegisterOracle();
        }


        public static Pattern_Holes Get(decimal pattern_hole_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE pattern_hole_id = :pattern_hole_id");

            Pattern_Holes retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("pattern_hole_id", pattern_hole_id);
            cmd.BindByName = true;

            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }


        public static Pattern_Holes Get(string Pattern_Number, string Hole_number)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE p.pattern_number = :pattern_number");
            sql.AppendLine($"AND ph.hole_number = :hole_number");

            Pattern_Holes retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("pattern_number", Pattern_Number);
            cmd.Parameters.Add("hole_number", Hole_number);
            cmd.BindByName = true;

            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }

        public static List<Pattern_Holes> GetByPatternNumber(string Pattern_Number)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE p.pattern_number = :pattern_number");

            List<Pattern_Holes> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("pattern_number", Pattern_Number);
            cmd.BindByName = true;

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
            cols.AppendLine($"{ta}pattern_hole_id {ColPrefix}pattern_hole_id, {ta}hole_number {ColPrefix}hole_number, ");
            cols.AppendLine($"{ta}northing {ColPrefix}northing, {ta}easting {ColPrefix}easting, ");
            cols.AppendLine($"{ta}planned_depth {ColPrefix}planned_depth, {ta}bottom_altitude {ColPrefix}bottom_altitude, ");
            cols.AppendLine($"{ta}pattern_id {ColPrefix}pattern_id");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns("ph") + ",");
            sql.AppendLine(PatternSvc.GetColumns("p", "p_") + ",");
            sql.AppendLine(DAL.ToLive.Services.Mine_SchedSvc.GetColumns("ms", "ms") );
            sql.AppendLine("FROM blast.pattern_holes ph");
            sql.AppendLine("INNER JOIN blast.pattern p");
            sql.AppendLine("ON ph.pattern_id = p.id");
            sql.AppendLine("LEFT JOIN tolive.Mine_Sched ms");
            sql.AppendLine("ON p.mine_sched_id = ms.mine_sched_id");
            return sql.ToString();
        }


        public static int Insert(Pattern_Holes obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Pattern_Holes obj, OracleConnection conn)
        {
            if (obj.Pattern_Hole_Id <= 0)
                obj.Pattern_Hole_Id = Convert.ToInt32(MOO.Data.GetNextSequence("Blast.seq_pattern_holes"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO Blast.Pattern_Holes(");
            sql.AppendLine("pattern_hole_id, hole_number, northing, easting, planned_depth, bottom_altitude, ");
            sql.AppendLine("pattern_id)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":pattern_hole_id, :hole_number, :northing, :easting, :planned_depth, :bottom_altitude, ");
            sql.AppendLine(":pattern_id)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("pattern_hole_id", obj.Pattern_Hole_Id);
            ins.Parameters.Add("hole_number", obj.Hole_Number);
            ins.Parameters.Add("northing", obj.Northing);
            ins.Parameters.Add("easting", obj.Easting);
            ins.Parameters.Add("planned_depth", obj.Planned_Depth);
            ins.Parameters.Add("bottom_altitude", obj.Bottom_Altitude);
            ins.Parameters.Add("pattern_id", obj.Pattern.Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Pattern_Holes obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Pattern_Holes obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE Blast.Pattern_Holes SET");
            sql.AppendLine("hole_number = :hole_number, ");
            sql.AppendLine("northing = :northing, ");
            sql.AppendLine("easting = :easting, ");
            sql.AppendLine("planned_depth = :planned_depth, ");
            sql.AppendLine("bottom_altitude = :bottom_altitude, ");
            sql.AppendLine("pattern_id = :pattern_id");
            sql.AppendLine("WHERE pattern_hole_id = :pattern_hole_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("hole_number", obj.Hole_Number);
            upd.Parameters.Add("northing", obj.Northing);
            upd.Parameters.Add("easting", obj.Easting);
            upd.Parameters.Add("planned_depth", obj.Planned_Depth);
            upd.Parameters.Add("bottom_altitude", obj.Bottom_Altitude);
            upd.Parameters.Add("pattern_id", obj.Pattern.Id);
            upd.Parameters.Add("pattern_hole_id", obj.Pattern_Hole_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Pattern_Holes obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Pattern_Holes obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM Blast.Pattern_Holes");
            sql.AppendLine("WHERE pattern_hole_id = :pattern_hole_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("pattern_hole_id", obj.Pattern_Hole_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }

        /// <summary>
        /// deletes all holes of the specified pattern number
        /// </summary>
        /// <param name="patternNumber"></param>
        /// <returns></returns>
        public static int DeletePatternHolesByPattern(string patternNumber)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = DeletePatternHolesByPattern(patternNumber, conn);
            conn.Close();
            return recsAffected;
        }


        /// <summary>
        /// deletes all holes of the specified pattern number
        /// </summary>
        /// <param name="patternNumber"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int DeletePatternHolesByPattern(string patternNumber, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM Blast.Pattern_Holes");
            sql.AppendLine("WHERE pattern_id = (SELECT id FROM blast.pattern WHERE pattern_number = :pattern)");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("pattern", patternNumber);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Pattern_Holes DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Pattern_Holes RetVal = new();
            RetVal.Pattern_Hole_Id = (int)(decimal)Util.GetRowVal(row, $"{ColPrefix}pattern_hole_id");
            RetVal.Hole_Number = (string)Util.GetRowVal(row, $"{ColPrefix}hole_number");
            RetVal.Northing = (decimal?)Util.GetRowVal(row, $"{ColPrefix}northing");
            RetVal.Easting = (decimal?)Util.GetRowVal(row, $"{ColPrefix}easting");
            RetVal.Planned_Depth = (decimal?)Util.GetRowVal(row, $"{ColPrefix}planned_depth");
            RetVal.Bottom_Altitude = (decimal?)Util.GetRowVal(row, $"{ColPrefix}bottom_altitude");
            RetVal.Pattern = PatternSvc.DataRowToObject(row, "p_");
            return RetVal;
        }

    }
}
