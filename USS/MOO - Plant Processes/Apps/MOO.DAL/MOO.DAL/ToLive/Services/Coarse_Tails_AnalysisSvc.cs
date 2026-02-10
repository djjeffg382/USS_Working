using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Services
{
    public static class Coarse_Tails_AnalysisSvc
    {
        static Coarse_Tails_AnalysisSvc()
        {
            Util.RegisterOracle();
        }


        public static Coarse_Tails_Analysis Get(DateTime sdate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE sdate = :sdate");


            Coarse_Tails_Analysis retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("sdate", sdate);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }

        public static List<Coarse_Tails_Analysis> GetByDate(DateTime StartDate, DateTime EndDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE sdate BETWEEN :StartDate AND :EndDate");

            List<Coarse_Tails_Analysis> elements = new();
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
            cols.AppendLine($"{ta}sdate {ColPrefix}sdate, {ta}line {ColPrefix}line, {ta}intvl {ColPrefix}intvl, ");
            cols.AppendLine($"{ta}fe {ColPrefix}fe, {ta}sample_date {ColPrefix}sample_date");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.coarse_tails_analysis");
            return sql.ToString();
        }


        public static int Insert(Coarse_Tails_Analysis obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Coarse_Tails_Analysis obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Coarse_Tails_Analysis(");
            sql.AppendLine("sdate, line, intvl, fe)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":sdate, :line, :intvl, :fe)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("sdate", obj.Sdate.Date);
            ins.Parameters.Add("line", obj.Line);
            ins.Parameters.Add("intvl", obj.Intvl);
            ins.Parameters.Add("fe", obj.Fe);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Coarse_Tails_Analysis obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Coarse_Tails_Analysis obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Coarse_Tails_Analysis SET");
            sql.AppendLine("fe = :fe ");
            sql.AppendLine("WHERE sdate = :sdate");
            sql.AppendLine("AND line = :line ");
            sql.AppendLine("AND intvl = :intvl ");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("fe", obj.Fe);

            upd.Parameters.Add("sdate", obj.Sdate.Date);
            upd.Parameters.Add("line", obj.Line);
            upd.Parameters.Add("intvl", obj.Intvl);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Coarse_Tails_Analysis obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Coarse_Tails_Analysis obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.Coarse_Tails_Analysis");
            sql.AppendLine("WHERE sdate = :sdate");
            sql.AppendLine("AND line = :line ");
            sql.AppendLine("AND intvl = :intvl ");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("sdate", obj.Sdate);
            del.Parameters.Add("line", obj.Line);
            del.Parameters.Add("intvl", obj.Intvl);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Coarse_Tails_Analysis DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Coarse_Tails_Analysis RetVal = new();
            RetVal.Sdate = (DateTime)Util.GetRowVal(row, $"{ColPrefix}sdate");
            RetVal.Line = (short)(decimal)Util.GetRowVal(row, $"{ColPrefix}line");
            RetVal.Intvl = (short)(decimal)Util.GetRowVal(row, $"{ColPrefix}intvl");
            RetVal.Fe = (decimal)Util.GetRowVal(row, $"{ColPrefix}fe");
            return RetVal;
        }

    }
}
