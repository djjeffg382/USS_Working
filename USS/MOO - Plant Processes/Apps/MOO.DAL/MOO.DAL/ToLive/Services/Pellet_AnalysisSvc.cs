using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MOO.DAL.ToLive.Models.Filter_Cake_Analysis;

namespace MOO.DAL.ToLive.Services
{
    public static class Pellet_AnalysisSvc
    {
        static Pellet_AnalysisSvc()
        {
            Util.RegisterOracle();
        }


        public static Pellet_Analysis Get(DateTime sdate, byte line, byte shiftNbr)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE sdate = :sdate");
            sql.AppendLine("AND line = :line");
            sql.AppendLine("AND shift = :shiftNbr");


            Pellet_Analysis retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("sdate", sdate);
            cmd.Parameters.Add("line", line);
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


        
        public static List<Pellet_Analysis> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE sdate BETWEEN :startDate AND :endDate");

            List<Pellet_Analysis> elements = [];
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
            cols.AppendLine($"{ta}sdate {ColPrefix}sdate, {ta}line {ColPrefix}line, {ta}shift {ColPrefix}shift, ");
            cols.AppendLine($"{ta}si {ColPrefix}si, {ta}al {ColPrefix}al, {ta}ca {ColPrefix}ca, {ta}mg {ColPrefix}mg, ");
            cols.AppendLine($"{ta}mn {ColPrefix}mn, {ta}rdate {ColPrefix}rdate, {ta}ana_inits {ColPrefix}ana_inits, ");
            cols.AppendLine($"{ta}sample_date {ColPrefix}sample_date");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.pellet_analysis");
            return sql.ToString();
        }


        public static int Insert(Pellet_Analysis obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Pellet_Analysis obj, OracleConnection conn)
        {            
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Pellet_Analysis(");
            sql.AppendLine("sdate, line, shift, si, al, ca, mg, mn, rdate, ana_inits)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":sdate, :line, :shift, :si, :al, :ca, :mg, :mn, :rdate, :ana_inits)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("sdate", obj.Sdate);
            ins.Parameters.Add("line", obj.Line);
            ins.Parameters.Add("shift", obj.Shift);
            ins.Parameters.Add("si", obj.Si);
            ins.Parameters.Add("al", obj.Al);
            ins.Parameters.Add("ca", obj.Ca);
            ins.Parameters.Add("mg", obj.Mg);
            ins.Parameters.Add("mn", obj.Mn);
            ins.Parameters.Add("rdate", obj.Rdate);
            ins.Parameters.Add("ana_inits", obj.Ana_Inits);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Pellet_Analysis obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Pellet_Analysis obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Pellet_Analysis SET");
            sql.AppendLine("si = :si, ");
            sql.AppendLine("al = :al, ");
            sql.AppendLine("ca = :ca, ");
            sql.AppendLine("mg = :mg, ");
            sql.AppendLine("mn = :mn, ");
            sql.AppendLine("rdate = :rdate, ");
            sql.AppendLine("ana_inits = :ana_inits ");
            sql.AppendLine("WHERE sdate = :sdate");
            sql.AppendLine("AND line = :line");
            sql.AppendLine("AND shift = :shift");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("si", obj.Si);
            upd.Parameters.Add("al", obj.Al);
            upd.Parameters.Add("ca", obj.Ca);
            upd.Parameters.Add("mg", obj.Mg);
            upd.Parameters.Add("mn", obj.Mn);
            upd.Parameters.Add("rdate", obj.Rdate);
            upd.Parameters.Add("ana_inits", obj.Ana_Inits);

            upd.Parameters.Add("sdate", obj.Sdate);
            upd.Parameters.Add("line", obj.Line);
            upd.Parameters.Add("shift", obj.Shift);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        /// <summary>
        /// updates a single component value of the table (SI, AL, CA, MG, MN)
        /// </summary>
        public static int UpdateComponent(DateTime shiftDate, byte line, byte shiftNbr, Pellet_Analysis.PelletAnalysisComponent componentName, decimal? componentValue)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = UpdateComponent(shiftDate, line, shiftNbr, componentName, componentValue, conn);
            conn.Close();
            return recsAffected;
        }


        /// <summary>
        /// updates a single component value of the table (SI, AL, CA, MG, MN)
        /// </summary>
        public static int UpdateComponent(DateTime shiftDate, byte line, byte shiftNbr, Pellet_Analysis.PelletAnalysisComponent componentName, decimal? componentValue,
                        OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.pellet_analysis SET");
            sql.AppendLine($"{componentName.ToString()} = :value ");
            sql.AppendLine("WHERE sdate = :sdate");
            sql.AppendLine("AND line = :line ");
            sql.AppendLine("AND shift = :shiftNbr ");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("value", componentValue);

            upd.Parameters.Add("sdate", shiftDate);
            upd.Parameters.Add("line", line);
            upd.Parameters.Add("shiftNbr", shiftNbr);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Pellet_Analysis obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Pellet_Analysis obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.Pellet_Analysis");
            sql.AppendLine("WHERE sdate = :sdate");
            sql.AppendLine("AND line = :line");
            sql.AppendLine("AND shift = :shift");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("sdate", obj.Sdate);
            del.Parameters.Add("line", obj.Line);
            del.Parameters.Add("shift", obj.Shift);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Pellet_Analysis DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Pellet_Analysis RetVal = new();
            RetVal.Sdate = (DateTime)Util.GetRowVal(row, $"{ColPrefix}sdate");
            RetVal.Line = Convert.ToByte( (decimal)Util.GetRowVal(row, $"{ColPrefix}line"));
            RetVal.Shift =Convert.ToByte( (decimal)Util.GetRowVal(row, $"{ColPrefix}shift"));
            RetVal.Si = (decimal?)Util.GetRowVal(row, $"{ColPrefix}si");
            RetVal.Al = (decimal?)Util.GetRowVal(row, $"{ColPrefix}al");
            RetVal.Ca = (decimal?)Util.GetRowVal(row, $"{ColPrefix}ca");
            RetVal.Mg = (decimal?)Util.GetRowVal(row, $"{ColPrefix}mg");
            RetVal.Mn = (decimal?)Util.GetRowVal(row, $"{ColPrefix}mn");
            RetVal.Rdate = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}rdate");
            RetVal.Ana_Inits = (string)Util.GetRowVal(row, $"{ColPrefix}ana_inits");
            RetVal._sample_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}sample_date");
            return RetVal;
        }

    }
}
