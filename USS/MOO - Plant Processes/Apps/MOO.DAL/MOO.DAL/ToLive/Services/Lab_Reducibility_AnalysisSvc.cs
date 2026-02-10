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
    public static class Lab_Reducibility_AnalysisSvc
    {
        static Lab_Reducibility_AnalysisSvc()
        {
            Util.RegisterOracle();
        }


        public static Lab_Reducibility_Analysis Get(DateTime shiftDate, byte line)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE sdate = :sdate");
            sql.AppendLine("AND line = :line");


            Lab_Reducibility_Analysis retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("sdate", shiftDate);
            cmd.Parameters.Add("line", line);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }

        public static List<Lab_Reducibility_Analysis> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE sdate BETWEEN :startDate AND :endDate");

            List<Lab_Reducibility_Analysis> elements = [];
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
            cols.AppendLine($"{ta}sdate {ColPrefix}sdate, {ta}line {ColPrefix}line, {ta}si {ColPrefix}si, {ta}al {ColPrefix}al, ");
            cols.AppendLine($"{ta}ca {ColPrefix}ca, {ta}mg {ColPrefix}mg, {ta}mn {ColPrefix}mn, {ta}rdate {ColPrefix}rdate, ");
            cols.AppendLine($"{ta}ana_inits {ColPrefix}ana_inits ");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.lab_reducibility_analysis");
            return sql.ToString();
        }


        public static int Insert(Lab_Reducibility_Analysis obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Lab_Reducibility_Analysis obj, OracleConnection conn)
        {
            
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Lab_Reducibility_Analysis(");
            sql.AppendLine("sdate, line, si, al, ca, mg, mn, rdate, ana_inits)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":sdate, :line, :si, :al, :ca, :mg, :mn, :rdate, :ana_inits)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("sdate", obj.Sdate);
            ins.Parameters.Add("line", obj.Line);
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


        public static int Update(Lab_Reducibility_Analysis obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Lab_Reducibility_Analysis obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Lab_Reducibility_Analysis SET");
            sql.AppendLine("si = :si, ");
            sql.AppendLine("al = :al, ");
            sql.AppendLine("ca = :ca, ");
            sql.AppendLine("mg = :mg, ");
            sql.AppendLine("mn = :mn, ");
            sql.AppendLine("rdate = :rdate, ");
            sql.AppendLine("ana_inits = :ana_inits ");
            sql.AppendLine("WHERE sdate = :sdate");
            sql.AppendLine("AND line = :line");
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
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }




        /// <summary>
        /// updates a single component value of the table (SI, AL, CA, MG, MN)
        /// </summary>
        public static int UpdateComponent(DateTime shiftDate, byte line,
                                Lab_Reducibility_Analysis.ReducibilityComponent componentName, decimal? componentValue)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = UpdateComponent(shiftDate, line, componentName, componentValue, conn);
            conn.Close();
            return recsAffected;
        }


        /// <summary>
        /// updates a single component value of the table (SI, AL, CA, MG, MN)
        /// </summary>
        public static int UpdateComponent(DateTime shiftDate, byte line, 
                                Lab_Reducibility_Analysis.ReducibilityComponent componentName, decimal? componentValue, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.lab_reducibility_analysis SET");
            sql.AppendLine($"{componentName.ToString()} = :value ");
            sql.AppendLine("WHERE sdate = :sdate");
            sql.AppendLine("AND line = :line ");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("value", componentValue);

            upd.Parameters.Add("sdate", shiftDate);
            upd.Parameters.Add("line", line);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }



        public static int Delete(Lab_Reducibility_Analysis obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Lab_Reducibility_Analysis obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.Lab_Reducibility_Analysis");
            sql.AppendLine("WHERE sdate = :sdate");
            sql.AppendLine("AND line = :line");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("sdate", obj.Sdate);
            del.Parameters.Add("line", obj.Line);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Lab_Reducibility_Analysis DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Lab_Reducibility_Analysis RetVal = new();
            RetVal.Sdate = (DateTime)Util.GetRowVal(row, $"{ColPrefix}sdate");
            RetVal.Line = Convert.ToByte( (decimal)Util.GetRowVal(row, $"{ColPrefix}line"));
            RetVal.Si = (decimal?)Util.GetRowVal(row, $"{ColPrefix}si");
            RetVal.Al = (decimal?)Util.GetRowVal(row, $"{ColPrefix}al");
            RetVal.Ca = (decimal?)Util.GetRowVal(row, $"{ColPrefix}ca");
            RetVal.Mg = (decimal?)Util.GetRowVal(row, $"{ColPrefix}mg");
            RetVal.Mn = (decimal?)Util.GetRowVal(row, $"{ColPrefix}mn");
            RetVal.Rdate = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}rdate");
            RetVal.Ana_Inits = (string)Util.GetRowVal(row, $"{ColPrefix}ana_inits");
            return RetVal;
        }

    }
}
