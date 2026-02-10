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
    public static class Mine_SchedSvc
    {
        static Mine_SchedSvc()
        {
            Util.RegisterOracle();
        }

        public static Mine_Sched Get(int mine_sched_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE mine_sched_id = :mine_sched_id");

            Mine_Sched retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("mine_sched_id", mine_sched_id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }


        public static List<Mine_Sched> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            List<Mine_Sched> elements = [];
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
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
        public static List<Mine_Sched> GetByMineSchedDate(DateTime start, DateTime end)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE sched_date BETWEEN :start_date AND :end_date");
            List<Mine_Sched> retVal = new();
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
                    retVal.Add(DataRowToObject(rdr));
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
            cols.AppendLine($"{ta}mine_sched_id {ColPrefix}mine_sched_id, {ta}plant {ColPrefix}plant, ");
            cols.AppendLine($"{ta}sched_date {ColPrefix}sched_date, {ta}blast_location {ColPrefix}blast_location, ");
            cols.AppendLine($"{ta}blast_delay {ColPrefix}blast_delay, {ta}maintenance {ColPrefix}maintenance, ");
            cols.AppendLine($"{ta}road_maint {ColPrefix}road_maint, {ta}dump_maint {ColPrefix}dump_maint");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.mine_sched");
            return sql.ToString();
        }


        public static int Insert(Mine_Sched obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Mine_Sched obj, OracleConnection conn)
        {
            if (obj.Mine_Sched_Id <= 0)
                obj.Mine_Sched_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.seq_mine_sched"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.mine_sched(");
            sql.AppendLine("mine_sched_id, plant, sched_date, blast_location, blast_delay, maintenance, ");
            sql.AppendLine("road_maint, dump_maint)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":mine_sched_id, :plant, :sched_date, :blast_location, :blast_delay, :maintenance, ");
            sql.AppendLine(":road_maint, :dump_maint)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("mine_sched_id", obj.Mine_Sched_Id);
            ins.Parameters.Add("plant", obj.Plant.ToString());
            ins.Parameters.Add("sched_date", obj.Sched_Date);
            ins.Parameters.Add("blast_location", obj.Blast_Location);
            ins.Parameters.Add("blast_delay", obj.Blast_Delay);
            ins.Parameters.Add("maintenance", obj.Maintenance);
            ins.Parameters.Add("road_maint", obj.Road_Maint);
            ins.Parameters.Add("dump_maint", obj.Dump_Maint);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Mine_Sched obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Mine_Sched obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.mine_sched SET");
            sql.AppendLine("plant = :plant, ");
            sql.AppendLine("sched_date = :sched_date, ");
            sql.AppendLine("blast_location = :blast_location, ");
            sql.AppendLine("blast_delay = :blast_delay, ");
            sql.AppendLine("maintenance = :maintenance, ");
            sql.AppendLine("road_maint = :road_maint, ");
            sql.AppendLine("dump_maint = :dump_maint");
            sql.AppendLine("WHERE mine_sched_id = :mine_sched_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("plant", obj.Plant.ToString());
            upd.Parameters.Add("sched_date", obj.Sched_Date);
            upd.Parameters.Add("blast_location", obj.Blast_Location);
            upd.Parameters.Add("blast_delay", obj.Blast_Delay);
            upd.Parameters.Add("maintenance", obj.Maintenance);
            upd.Parameters.Add("road_maint", obj.Road_Maint);
            upd.Parameters.Add("dump_maint", obj.Dump_Maint);
            upd.Parameters.Add("mine_sched_id", obj.Mine_Sched_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }

        internal static Mine_Sched DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Mine_Sched RetVal = new();
            RetVal.Mine_Sched_Id = (long)Util.GetRowVal(row, $"{ColPrefix}mine_sched_id");
            RetVal.Plant = (MOO.Plant)Enum.Parse(typeof(MOO.Plant),(string)Util.GetRowVal(row, $"{ColPrefix}plant"));
            RetVal.Sched_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}sched_date");
            RetVal.Blast_Location = (string)Util.GetRowVal(row, $"{ColPrefix}blast_location");
            RetVal.Blast_Delay = (string)Util.GetRowVal(row, $"{ColPrefix}blast_delay");
            RetVal.Maintenance = (string)Util.GetRowVal(row, $"{ColPrefix}maintenance");
            RetVal.Road_Maint = (string)Util.GetRowVal(row, $"{ColPrefix}road_maint");
            RetVal.Dump_Maint = (string)Util.GetRowVal(row, $"{ColPrefix}dump_maint");
            return RetVal;
        }

    }
}
