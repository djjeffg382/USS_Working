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
    public static class Rmf_ScreenSvc
    {
        static Rmf_ScreenSvc()
        {
            Util.RegisterOracle();
        }


        public static Rmf_Screen Get(DateTime Shift_Date, byte Shift, byte Step)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE shift_date = :shift_date");
            sql.AppendLine($"AND shift = :shift");
            sql.AppendLine($"AND Step = :step");


            Rmf_Screen retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("shift_date", Shift_Date.Date);
            cmd.Parameters.Add("Shift", Shift);
            cmd.Parameters.Add("Step", Step);
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
        /// Gets the Screen data for a given shift date
        /// </summary>
        /// <param name="Shift_Date"></param>
        /// <returns></returns>
        public static List<Rmf_Screen> GetShiftDate(DateTime Shift_Date, byte Step)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE shift_date = :shift_date");
            sql.AppendLine($"AND Step = :step");

            List<Rmf_Screen> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("shift_date", Shift_Date.Date);
            cmd.Parameters.Add("Step", Step);
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
            cols.AppendLine($"{ta}shift_date {ColPrefix}shift_date, {ta}shift {ColPrefix}shift, {ta}step {ColPrefix}step, ");
            cols.AppendLine($"{ta}start_wgt {ColPrefix}start_wgt, {ta}scn_1_inch {ColPrefix}scn_1_inch, ");
            cols.AppendLine($"{ta}scn_3_4_inch {ColPrefix}scn_3_4_inch, {ta}scn_1_2_inch {ColPrefix}scn_1_2_inch, ");
            cols.AppendLine($"{ta}scn_1_4_inch {ColPrefix}scn_1_4_inch, {ta}scn_6m {ColPrefix}scn_6m, ");
            cols.AppendLine($"{ta}scn_minus_6m {ColPrefix}scn_minus_6m");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.rmf_screen");
            return sql.ToString();
        }


        public static int Insert(Rmf_Screen obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Rmf_Screen obj, OracleConnection conn)
        {
            if (obj.Start_Wgt.GetValueOrDefault(0) != obj.CumulativeWeight)
                throw new InvalidDataEntered("Starting weight does not equal cumulative weight","Start Weight");  //This must be cahnaged to InvalidDataException but must be done at same time GUI is adjusted to catch this

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.rmf_screen(");
            sql.AppendLine("shift_date, shift, step, start_wgt, scn_1_inch, scn_3_4_inch, scn_1_2_inch, ");
            sql.AppendLine("scn_1_4_inch, scn_6m, scn_minus_6m)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":shift_date, :shift, :step, :start_wgt, :scn_1_inch, :scn_3_4_inch, :scn_1_2_inch, ");
            sql.AppendLine(":scn_1_4_inch, :scn_6m, :scn_minus_6m)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("shift_date", obj.Shift_Date.Date);
            ins.Parameters.Add("shift", obj.Shift);
            ins.Parameters.Add("step", obj.Step );
            ins.Parameters.Add("start_wgt", TrimValue(obj.Start_Wgt));
            ins.Parameters.Add("scn_1_inch", TrimValue(obj.Scn_1_Inch));
            ins.Parameters.Add("scn_3_4_inch", TrimValue(obj.Scn_3_4_Inch));
            ins.Parameters.Add("scn_1_2_inch", TrimValue(obj.Scn_1_2_Inch));
            ins.Parameters.Add("scn_1_4_inch", TrimValue(obj.Scn_1_4_Inch));
            ins.Parameters.Add("scn_6m", TrimValue(obj.Scn_6m));
            ins.Parameters.Add("scn_minus_6m", TrimValue(obj.Scn_Minus_6m));
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Rmf_Screen obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Rmf_Screen obj, OracleConnection conn)
        {
            if (obj.Start_Wgt.GetValueOrDefault(0) != obj.CumulativeWeight)
                throw new InvalidDataEntered("Starting weight does not equal cumulative weight", "Start Weight");  //This must be cahnaged to InvalidDataException but must be done at same time GUI is adjusted to catch this
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.rmf_screen SET");
            sql.AppendLine("start_wgt = :start_wgt, ");
            sql.AppendLine("scn_1_inch = :scn_1_inch, ");
            sql.AppendLine("scn_3_4_inch = :scn_3_4_inch, ");
            sql.AppendLine("scn_1_2_inch = :scn_1_2_inch, ");
            sql.AppendLine("scn_1_4_inch = :scn_1_4_inch, ");
            sql.AppendLine("scn_6m = :scn_6m, ");
            sql.AppendLine("scn_minus_6m = :scn_minus_6m");
            sql.AppendLine("WHERE shift_date = :shift_date");
            sql.AppendLine("AND shift = :shift ");
            sql.AppendLine("AND step = :step ");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("start_wgt", TrimValue(obj.Start_Wgt));
            upd.Parameters.Add("scn_1_inch", TrimValue(obj.Scn_1_Inch));
            upd.Parameters.Add("scn_3_4_inch", TrimValue(obj.Scn_3_4_Inch));
            upd.Parameters.Add("scn_1_2_inch", TrimValue(obj.Scn_1_2_Inch));
            upd.Parameters.Add("scn_1_4_inch", TrimValue(obj.Scn_1_4_Inch));
            upd.Parameters.Add("scn_6m", TrimValue(obj.Scn_6m));
            upd.Parameters.Add("scn_minus_6m", TrimValue(obj.Scn_Minus_6m));
            upd.Parameters.Add("shift_date", obj.Shift_Date.Date);
            upd.Parameters.Add("shift", obj.Shift);
            upd.Parameters.Add("step", obj.Step);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Rmf_Screen obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Rmf_Screen obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.rmf_screen");
            sql.AppendLine("WHERE shift_date = :shift_date");
            sql.AppendLine("AND shift = :shift ");
            sql.AppendLine("AND step = :step ");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("shift_date", obj.Shift_Date);
            del.Parameters.Add("shift", obj.Shift);
            del.Parameters.Add("step", obj.Step);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Rmf_Screen DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Rmf_Screen RetVal = new();
            RetVal.Shift_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}shift_date");
            RetVal.Shift = (short)Util.GetRowVal(row, $"{ColPrefix}shift");
            RetVal.Step = (short)Util.GetRowVal(row, $"{ColPrefix}step");
            RetVal.Start_Wgt = (decimal?)(float?)Util.GetRowVal(row, $"{ColPrefix}start_wgt");
            RetVal.Scn_1_Inch = (decimal?)(float?)Util.GetRowVal(row, $"{ColPrefix}scn_1_inch");
            RetVal.Scn_3_4_Inch = (decimal?)(float?)Util.GetRowVal(row, $"{ColPrefix}scn_3_4_inch");
            RetVal.Scn_1_2_Inch = (decimal?)(float?)Util.GetRowVal(row, $"{ColPrefix}scn_1_2_inch");
            RetVal.Scn_1_4_Inch = (decimal?)(float?)Util.GetRowVal(row, $"{ColPrefix}scn_1_4_inch");
            RetVal.Scn_6m = (decimal?)(float?)Util.GetRowVal(row, $"{ColPrefix}scn_6m");
            RetVal.Scn_Minus_6m = (decimal?)(float?)Util.GetRowVal(row, $"{ColPrefix}scn_minus_6m");
            return RetVal;
        }
        /// <summary>
        /// trims the weight values to one decimal before saving to the database
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private static decimal? TrimValue(decimal? val)
        {
            if (val.HasValue)
                return Math.Floor(val.Value * 10) / 10;
            else
                return null;
        }
    }
}
