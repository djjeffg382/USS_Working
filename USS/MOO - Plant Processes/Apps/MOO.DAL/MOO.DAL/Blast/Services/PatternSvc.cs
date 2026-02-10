using MOO.DAL.Blast.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;

namespace MOO.DAL.Blast.Services
{
    public static class PatternSvc
    {
        static PatternSvc()
        {
            Util.RegisterOracle();
        }

        /// <summary>
        /// gets the pattern by the Pattern ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Pattern Get(decimal Pattern_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE pt.id = :id");

            Pattern retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("id", Pattern_Id);
            cmd.BindByName = true;

            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr, "pt_");
            }
            conn.Close();
            return retVal;
        }

        /// <summary>
        /// gets the pattern by pattern number
        /// </summary>
        /// <param name="Pattern_Number"></param>
        /// <returns></returns>
        public static Pattern Get(string Pattern_Number)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE pt.pattern_number = :pattern_number");

            Pattern retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("pattern_number", Pattern_Number);
            cmd.BindByName = true;

            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr, "pt_");
            }
            conn.Close();
            return retVal;
        }
        public static List<Pattern> GetByUpdateDate(DateTime start, DateTime end)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE update_date BETWEEN :start_date AND :end_date");
            List<Pattern> retVal = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("start_date", start);
            cmd.Parameters.Add("end_date", end);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    retVal.Add(DataRowToObject(rdr, "pt_"));
                }
            }
            conn.Close();
            return retVal;
        }
        public static List<Pattern> GetByMineSchedDate(DateTime start, DateTime end)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE ms.sched_date BETWEEN :start_date AND :end_date");
            List<Pattern> retVal = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("start_date", start);
            cmd.Parameters.Add("end_date", end);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    retVal.Add(DataRowToObject(rdr, "pt_"));
                }
            }
            conn.Close();
            return retVal;
        }


        public static List<Pattern> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            List<Pattern> retVal = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;

            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    retVal.Add(DataRowToObject(rdr, "pt_"));
                }
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
            cols.AppendLine($"{ta}id {ColPrefix}id, {ta}pattern_number {ColPrefix}pattern_number, ");
            cols.AppendLine($"{ta}update_date {ColPrefix}update_date, ");
            cols.AppendLine($"{ta}mine_sched_id {ColPrefix}mine_sched_id");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns("pt", "pt_") + ",");
            sql.AppendLine(ToLive.Services.Mine_SchedSvc.GetColumns("ms", "ms"));
            sql.AppendLine("FROM blast.pattern pt");
            sql.AppendLine("LEFT JOIN tolive.mine_sched ms");
            sql.AppendLine("    ON pt.mine_sched_id = ms.mine_sched_id");
            return sql.ToString();
        }


        public static int Insert(Pattern obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Pattern obj, OracleConnection conn)
        {
            if (obj.Id <= 0)
                obj.Id = Convert.ToInt32(MOO.Data.GetNextSequence("blast.seq_pattern"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO blast.Pattern(");
            sql.AppendLine("id, pattern_number, update_date, mine_sched_id)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":id, :pattern_number, :update_date, :mine_sched_id)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("id", obj.Id);
            ins.Parameters.Add("pattern_number", obj.Pattern_Number);
            ins.Parameters.Add("update_date", obj.Update_Date);
            ins.Parameters.Add("mine_sched_id", obj.Mine_Sched == null ? null : obj.Mine_Sched.Mine_Sched_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Pattern obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Pattern obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE blast.Pattern SET");
            sql.AppendLine("pattern_number = :pattern_number, ");
            sql.AppendLine("update_date = :update_date, ");
            sql.AppendLine("mine_sched_id = :mine_sched_id");
            sql.AppendLine("WHERE id = :id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("pattern_number", obj.Pattern_Number);
            upd.Parameters.Add("update_date", obj.Update_Date);
            upd.Parameters.Add("id", obj.Id);
            upd.Parameters.Add("mine_sched_id", obj.Mine_Sched == null ? null : obj.Mine_Sched.Mine_Sched_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Pattern obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Pattern obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM blast.Pattern");
            sql.AppendLine("WHERE id = :id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("id", obj.Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Pattern DataRowToObject(DbDataReader row, string ColPrefix = "pt_")
        {
            Pattern RetVal = new();
            RetVal.Id = (int)(decimal)Util.GetRowVal(row, $"{ColPrefix}id");
            RetVal.Pattern_Number = (string)Util.GetRowVal(row, $"{ColPrefix}pattern_number");
            RetVal.Update_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}update_date");
            if (row.IsDBNull(row.GetOrdinal($"{ColPrefix}mine_sched_id")))
            {
                RetVal.Mine_Sched = null; 
            }
            else
            {
                RetVal.Mine_Sched = ToLive.Services.Mine_SchedSvc.DataRowToObject(row,"ms");
            }

            return RetVal;
        }

    }
}
