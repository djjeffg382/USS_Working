using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using static MOO.DAL.ToLive.Models.Filter_Cake_Analysis;
using static MOO.DAL.ToLive.Models.Float_Analysis;

namespace MOO.DAL.ToLive.Services
{
    public static class Float_AnalysisSvc
    {
        static Float_AnalysisSvc()
        {
            Util.RegisterOracle();
        }


        public static Float_Analysis Get(DateTime shiftDate, byte shiftNbr, FloatAnalysisType sType)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE datex = :datex");
            sql.AppendLine("AND shift = :shiftNbr");
            sql.AppendLine("AND sType = :sType");


            Float_Analysis retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("datex", shiftDate);
            cmd.Parameters.Add("shiftNbr", shiftNbr);
            cmd.Parameters.Add("sType", sType.ToString());
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }


        public static List<Float_Analysis> GetByDateRange(DateTime startDate, DateTime endDate, FloatAnalysisType? sType = null)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE datex BETWEEN :startDate AND :endDate");
            if(sType != null)
                sql.AppendLine("AND sType = :sType");

            List<Float_Analysis> elements = [];
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("startDate", startDate);
            cmd.Parameters.Add("endDate", endDate);
            if (sType != null)
                cmd.Parameters.Add("sType", sType.ToString());

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
            cols.AppendLine($"{ta}datex {ColPrefix}datex, {ta}shift {ColPrefix}shift, {ta}stype {ColPrefix}stype, ");
            cols.AppendLine($"{ta}si {ColPrefix}si, {ta}al {ColPrefix}al, {ta}ca {ColPrefix}ca, {ta}mg {ColPrefix}mg, ");
            cols.AppendLine($"{ta}mn {ColPrefix}mn, {ta}ana_inits {ColPrefix}ana_inits, ");
            cols.AppendLine($"{ta}rdate {ColPrefix}rdate");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.float_analysis");
            return sql.ToString();
        }


        public static int Insert(Float_Analysis obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Float_Analysis obj, OracleConnection conn)
        {
           
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Float_Analysis(");
            sql.AppendLine("datex, shift, stype, si, al, ca, mg, mn, ana_inits, rdate)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":datex, :shift, :stype, :si, :al, :ca, :mg, :mn, :ana_inits, :rdate)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("datex", obj.Datex);
            ins.Parameters.Add("shift", obj.Shift);
            ins.Parameters.Add("stype", obj.SType.ToString());
            ins.Parameters.Add("si", obj.Si);
            ins.Parameters.Add("al", obj.Al);
            ins.Parameters.Add("ca", obj.Ca);
            ins.Parameters.Add("mg", obj.Mg);
            ins.Parameters.Add("mn", obj.Mn);
            ins.Parameters.Add("ana_inits", obj.Ana_Inits);
            ins.Parameters.Add("rdate", obj.Rdate);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Float_Analysis obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }

        /// <summary>
        /// updates a single component value of the table (SI, AL, CA, MG, MN)
        /// </summary>
        public static int UpdateComponent(DateTime shiftDate, byte shiftNbr, FloatAnalysisType sType, FloatAnalysisComponent componentName,
                                decimal? componentValue)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = UpdateComponent(shiftDate, shiftNbr, sType, componentName, componentValue, conn);
            conn.Close();
            return recsAffected;
        }


        /// <summary>
        /// updates a single component value of the table (SI, AL, CA, MG, MN)
        /// </summary>
        public static int UpdateComponent(DateTime shiftDate, byte shiftNbr, FloatAnalysisType sType, FloatAnalysisComponent componentName, 
                                decimal? componentValue, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.float_analysis SET");
            sql.AppendLine($"{componentName.ToString()} = :value ");
            sql.AppendLine("WHERE datex = :shiftDate");
            sql.AppendLine("AND shift = :shiftNbr ");
            sql.AppendLine("AND stype = :stype ");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("value", componentValue);

            upd.Parameters.Add("shiftDate", shiftDate);
            upd.Parameters.Add("shiftNbr", shiftNbr);
            upd.Parameters.Add("stype", sType.ToString());
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }



        public static int Update(Float_Analysis obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Float_Analysis SET");
            sql.AppendLine("si = :si, ");
            sql.AppendLine("al = :al, ");
            sql.AppendLine("ca = :ca, ");
            sql.AppendLine("mg = :mg, ");
            sql.AppendLine("mn = :mn, ");
            sql.AppendLine("ana_inits = :ana_inits, ");
            sql.AppendLine("rdate = :rdate ");

            sql.AppendLine("WHERE datex = :datex");
            sql.AppendLine("AND shift = :shift ");
            sql.AppendLine("AND stype = :stype ");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("si", obj.Si);
            upd.Parameters.Add("al", obj.Al);
            upd.Parameters.Add("ca", obj.Ca);
            upd.Parameters.Add("mg", obj.Mg);
            upd.Parameters.Add("mn", obj.Mn);
            upd.Parameters.Add("ana_inits", obj.Ana_Inits);
            upd.Parameters.Add("rdate", obj.Rdate);

            upd.Parameters.Add("datex", obj.Datex);
            upd.Parameters.Add("shift", obj.Shift);
            upd.Parameters.Add("stype", obj.SType.ToString());
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Float_Analysis obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Float_Analysis obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.Float_Analysis");
            sql.AppendLine("WHERE datex = :datex");
            sql.AppendLine("AND shift = :shift ");
            sql.AppendLine("AND stype = :stype ");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("datex", obj.Datex);
            del.Parameters.Add("shift", obj.Shift);
            del.Parameters.Add("stype", obj.SType.ToString());
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Float_Analysis DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Float_Analysis RetVal = new();
            RetVal.Datex = (DateTime)Util.GetRowVal(row, $"{ColPrefix}datex");
            RetVal.Shift = Convert.ToByte( (decimal)Util.GetRowVal(row, $"{ColPrefix}shift"));
            RetVal.SType =Enum.Parse<FloatAnalysisType>( (string)Util.GetRowVal(row, $"{ColPrefix}stype"));
            RetVal.Si = (decimal?)Util.GetRowVal(row, $"{ColPrefix}si");
            RetVal.Al = (decimal?)Util.GetRowVal(row, $"{ColPrefix}al");
            RetVal.Ca = (decimal?)Util.GetRowVal(row, $"{ColPrefix}ca");
            RetVal.Mg = (decimal?)Util.GetRowVal(row, $"{ColPrefix}mg");
            RetVal.Mn = (decimal?)Util.GetRowVal(row, $"{ColPrefix}mn");
            RetVal.Ana_Inits = (string)Util.GetRowVal(row, $"{ColPrefix}ana_inits");
            RetVal.Rdate = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}rdate");
            return RetVal;
        }

    }
}
