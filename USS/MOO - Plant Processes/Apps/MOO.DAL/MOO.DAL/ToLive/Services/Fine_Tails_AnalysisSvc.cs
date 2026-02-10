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
    //The even lines will match the previous odd line,  Example line 3 and 4 are equal, line 5 and 6 are equal etc.
    //Therefore, insert updates and deletes will modify 2 records
    public static class Fine_Tails_AnalysisSvc
    {
        static Fine_Tails_AnalysisSvc()
        {
            Util.RegisterOracle();
        }


        public static Fine_Tails_Analysis Get(DateTime sdate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE sdate = :sdate");


            Fine_Tails_Analysis retVal = null;
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

        /// <summary>
        /// Gets the fine tails analysis by date range
        /// </summary>
        /// <param name="StartDate">Start Date</param>
        /// <param name="EndDate">End Date</param>
        /// <param name="ShowOnlySections">Whether to show just sections (3, 5, 7, 9....) or all lines.  Sections = true, All Lines = false</param>
        /// <returns></returns>
        public static List<Fine_Tails_Analysis> GetByDate(DateTime StartDate, DateTime EndDate, bool ShowOnlySections)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE sdate BETWEEN :StartDate AND :EndDate");

            List<Fine_Tails_Analysis> elements = new();
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
                    Fine_Tails_Analysis obj = DataRowToObject(rdr);
                    if(!ShowOnlySections || (obj.Line % 2 == 1))
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
            cols.AppendLine($"{ta}fe {ColPrefix}fe, {ta}sample_date {ColPrefix}sample_date");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.fine_tails_analysis");
            return sql.ToString();
        }


        public static int Insert(Fine_Tails_Analysis obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            var trans = conn.BeginTransaction();
            int recsAffected = Insert(obj, conn);
            trans.Commit();
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Fine_Tails_Analysis obj, OracleConnection conn)
        {
            
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Fine_Tails_Analysis(");
            sql.AppendLine("sdate, line, shift, fe)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":sdate, :line, :shift, :fe)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("sdate", obj.Sdate);
            ins.Parameters.Add("line", obj.Line);
            ins.Parameters.Add("shift", obj.Shift);
            ins.Parameters.Add("fe", obj.Fe);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            if(obj.Line % 2 == 1)
                ins.Parameters["line"].Value = obj.Line + 1;
            else
                ins.Parameters["line"].Value = obj.Line - 1;
            recsAffected += MOO.Data.ExecuteNonQuery(ins);  //now insert the even line also
            return recsAffected;
        }


        public static int Update(Fine_Tails_Analysis obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            var trans = conn.BeginTransaction();
            int recsAffected = Update(obj, conn);
            trans.Commit();
            conn.Close();
            return recsAffected;
        }


        public static int Update(Fine_Tails_Analysis obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Fine_Tails_Analysis SET");
            sql.AppendLine("fe = :fe ");
            sql.AppendLine("WHERE sdate = :sdate");
            sql.AppendLine("AND line = :line");
            sql.AppendLine("AND shift = :shift");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("fe", obj.Fe);
            upd.Parameters.Add("sdate", obj.Sdate.Date);
            upd.Parameters.Add("line", obj.Line);
            upd.Parameters.Add("shift", obj.Shift);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            if (obj.Line % 2 == 1)
                upd.Parameters["line"].Value = obj.Line + 1;
            else
                upd.Parameters["line"].Value = obj.Line - 1;
            recsAffected += MOO.Data.ExecuteNonQuery(upd);  //now update the even line also
            return recsAffected;
        }


        public static int Delete(Fine_Tails_Analysis obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            var trans = conn.BeginTransaction();
            int recsAffected = Delete(obj, conn);
            trans.Commit();
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Fine_Tails_Analysis obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.Fine_Tails_Analysis");
            sql.AppendLine("WHERE sdate = :sdate");
            sql.AppendLine("AND line = :line");
            sql.AppendLine("AND shift = :shift");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("sdate", obj.Sdate.Date);
            del.Parameters.Add("line", obj.Line);
            del.Parameters.Add("shift", obj.Shift);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            if (obj.Line % 2 == 1)
                del.Parameters["line"].Value = obj.Line + 1;
            else
                del.Parameters["line"].Value = obj.Line - 1;
            recsAffected += MOO.Data.ExecuteNonQuery(del);  //now Delete the even line also
            return recsAffected;
        }


        internal static Fine_Tails_Analysis DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Fine_Tails_Analysis RetVal = new();
            RetVal.Sdate = (DateTime)Util.GetRowVal(row, $"{ColPrefix}sdate");
            RetVal.Line = (short)(decimal)Util.GetRowVal(row, $"{ColPrefix}line");
            RetVal.Shift = (short)(decimal)Util.GetRowVal(row, $"{ColPrefix}shift");
            RetVal.Fe = (decimal)Util.GetRowVal(row, $"{ColPrefix}fe");
            return RetVal;
        }

    }
}
