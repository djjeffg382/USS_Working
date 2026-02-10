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
    public static class Flot_Feed_DT_AnalysisSvc
    {
        static Flot_Feed_DT_AnalysisSvc()
        {
            Util.RegisterOracle();
        }


        public static Flot_Feed_DT_Analysis Get(DateTime shiftDate, byte shiftNbr)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE datex = :datex");
            sql.AppendLine("AND shift = :shiftNbr");


            Flot_Feed_DT_Analysis retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("datex", shiftDate);
            cmd.Parameters.Add("shiftNbr", shiftNbr);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }

        public static List<Flot_Feed_DT_Analysis> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE datex BETWEEN :startDate AND :endDate");

            List<Flot_Feed_DT_Analysis> elements = [];
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("startDate", startDate);
            cmd.Parameters.Add("endDate", endDate);
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
            cols.AppendLine($"{ta}datex {ColPrefix}datex, {ta}shift {ColPrefix}shift, {ta}si {ColPrefix}si, ");
            cols.AppendLine($"{ta}ana_inits {ColPrefix}ana_inits, ");
            cols.AppendLine($"{ta}rdate {ColPrefix}rdate");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.flot_feed_dt_analysis");
            return sql.ToString();
        }


        public static int Insert(Flot_Feed_DT_Analysis obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Flot_Feed_DT_Analysis obj, OracleConnection conn)
        {
            
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Flot_Feed_DT_Analysis(");
            sql.AppendLine("datex, shift, si, ana_inits, rdate)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":datex, :shift, :si, :ana_inits, :rdate)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("datex", obj.Datex);
            ins.Parameters.Add("shift", obj.Shift);
            ins.Parameters.Add("si", obj.Si);
            ins.Parameters.Add("ana_inits", obj.Ana_Inits);
            ins.Parameters.Add("rdate", obj.Rdate);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Flot_Feed_DT_Analysis obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Flot_Feed_DT_Analysis obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Flot_Feed_DT_Analysis SET");
            sql.AppendLine("shift = :shift, ");
            sql.AppendLine("si = :si, ");
            sql.AppendLine("ana_inits = :ana_inits, ");
            sql.AppendLine("rdate = :rdate");
            sql.AppendLine("WHERE datex = :datex");
            sql.AppendLine("AND shift = :shift ");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("si", obj.Si);
            upd.Parameters.Add("ana_inits", obj.Ana_Inits);
            upd.Parameters.Add("rdate", obj.Rdate);

            upd.Parameters.Add("datex", obj.Datex);
            upd.Parameters.Add("shift", obj.Shift);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Flot_Feed_DT_Analysis obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Flot_Feed_DT_Analysis obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.Flot_Feed_DT_Analysis");
            sql.AppendLine("WHERE datex = :datex");
            sql.AppendLine("AND shift = :shift ");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("datex", obj.Datex);
            del.Parameters.Add("shift", obj.Shift);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Flot_Feed_DT_Analysis DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Flot_Feed_DT_Analysis RetVal = new();
            RetVal.Datex = (DateTime)Util.GetRowVal(row, $"{ColPrefix}datex");
            RetVal.Shift = Convert.ToByte((decimal)Util.GetRowVal(row, $"{ColPrefix}shift"));
            RetVal.Si = (decimal?)Util.GetRowVal(row, $"{ColPrefix}si");
            RetVal.Ana_Inits = (string)Util.GetRowVal(row, $"{ColPrefix}ana_inits");
            RetVal.Rdate = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}rdate");
            return RetVal;
        }

    }
}
