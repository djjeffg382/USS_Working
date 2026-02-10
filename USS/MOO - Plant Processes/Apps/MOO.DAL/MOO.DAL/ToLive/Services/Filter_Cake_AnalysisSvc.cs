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
    public static class Filter_Cake_AnalysisSvc
    {
        static Filter_Cake_AnalysisSvc()
        {
            Util.RegisterOracle();
        }


        public static Filter_Cake_Analysis Get(DateTime shiftDate, byte stepNbr, byte interval)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE sdate = :sdate");
            sql.AppendLine("AND step = :stepNbr");
            sql.AppendLine("AND intv = :interval");


            Filter_Cake_Analysis retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("sdate", shiftDate);
            cmd.Parameters.Add("stepNbr", stepNbr);
            cmd.Parameters.Add("interval", interval);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }


        public static List<Filter_Cake_Analysis> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE sdate BETWEEN :startDate AND :endDate");

            List<Filter_Cake_Analysis> elements = [];
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
            cols.AppendLine($"{ta}sdate {ColPrefix}sdate, {ta}step {ColPrefix}step, {ta}intv {ColPrefix}intv, ");
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
            sql.AppendLine("FROM tolive.filter_cake_analysis");
            return sql.ToString();
        }


        public static int Insert(Filter_Cake_Analysis obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Filter_Cake_Analysis obj, OracleConnection conn)
        {

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Filter_Cake_Analysis(");
            sql.AppendLine("sdate, step, intv, si, al, ca, mg, mn, rdate, ana_inits)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":sdate, :step, :intv, :si, :al, :ca, :mg, :mn, :rdate, :ana_inits) ");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("sdate", obj.Sdate);
            ins.Parameters.Add("step", obj.Step);
            ins.Parameters.Add("intv", obj.Intv);
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


        public static int Update(Filter_Cake_Analysis obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Filter_Cake_Analysis obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Filter_Cake_Analysis SET");
            sql.AppendLine("step = :step, ");
            sql.AppendLine("intv = :intv, ");
            sql.AppendLine("si = :si, ");
            sql.AppendLine("al = :al, ");
            sql.AppendLine("ca = :ca, ");
            sql.AppendLine("mg = :mg, ");
            sql.AppendLine("mn = :mn, ");
            sql.AppendLine("rdate = :rdate, ");
            sql.AppendLine("ana_inits = :ana_inits ");
            sql.AppendLine("WHERE sdate = :sdate");
            sql.AppendLine("AND step = :step ");
            sql.AppendLine("AND intv = :intv ");
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
            upd.Parameters.Add("step", obj.Step);
            upd.Parameters.Add("intv", obj.Intv);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }

        /// <summary>
        /// updates a single component value of the table (SI, AL, CA, MG, MN)
        /// </summary>
        public static int UpdateComponent(DateTime shiftDate, byte stepNbr, byte interval, FilterCakeAnalysisComponent componentName, decimal? componentValue)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = UpdateComponent(shiftDate, stepNbr, interval, componentName, componentValue, conn);
            conn.Close();
            return recsAffected;
        }


        /// <summary>
        /// updates a single component value of the table (SI, AL, CA, MG, MN)
        /// </summary>
        public static int UpdateComponent(DateTime shiftDate, byte stepNbr, byte interval, FilterCakeAnalysisComponent componentName, decimal? componentValue, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Filter_Cake_Analysis SET");
            sql.AppendLine($"{componentName.ToString()} = :value ");
            sql.AppendLine("WHERE sdate = :sdate");
            sql.AppendLine("AND step = :step ");
            sql.AppendLine("AND intv = :intv ");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("value", componentValue);

            upd.Parameters.Add("sdate", shiftDate);
            upd.Parameters.Add("step", stepNbr);
            upd.Parameters.Add("intv", interval);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }

        public static int Delete(Filter_Cake_Analysis obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Filter_Cake_Analysis obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.Filter_Cake_Analysis");
            sql.AppendLine("WHERE sdate = :sdate");
            sql.AppendLine("AND step = :step ");
            sql.AppendLine("AND intv = :intv ");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("sdate", obj.Sdate);
            del.Parameters.Add("step", obj.Step);
            del.Parameters.Add("intv", obj.Intv);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Filter_Cake_Analysis DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Filter_Cake_Analysis RetVal = new();
            RetVal.Sdate = (DateTime)Util.GetRowVal(row, $"{ColPrefix}sdate");
            RetVal.Step = Convert.ToByte((decimal)Util.GetRowVal(row, $"{ColPrefix}step"));
            RetVal.Intv = Convert.ToByte((decimal)Util.GetRowVal(row, $"{ColPrefix}intv"));
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
