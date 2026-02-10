using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.DAL.ToLive.Models;
using MOO.DAL.ToLive.Enums;

namespace MOO.DAL.ToLive.Services
{
    public static class FE_ReportSvc
    {
        static FE_ReportSvc()
        {
            Util.RegisterOracle();
        }

        public static FE_Report Get(long fe_report_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE fer_.fe_report_id = :fe_report_id");


            FE_Report retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("fe_report_id", fe_report_id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }
        public static List<FE_Report> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE fer_.record_date BETWEEN :startDate and :endDate");


            List<FE_Report> elements = [];
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



        public static List<FE_Report> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            List<FE_Report> elements = [];
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


        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}fe_report_id {ColPrefix}fe_report_id, {ta}fe_type_id {ColPrefix}fe_type_id, ");
            cols.AppendLine($"{ta}record_date {ColPrefix}record_date, {ta}sec_user_entered_by {ColPrefix}sec_user_entered_by, ");
            cols.AppendLine($"{ta}temperature {ColPrefix}temperature, {ta}wind_direction {ColPrefix}wind_direction, ");
            cols.AppendLine($"{ta}wind_speed {ColPrefix}wind_speed, {ta}observed_weather {ColPrefix}observed_weather, ");
            cols.AppendLine($"{ta}comments {ColPrefix}comments, {ta}entered_date {ColPrefix}entered_date");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns("fer_","fer_") + ", ");
            sql.AppendLine(Sec_UserSvc.GetColumns("su_","su_"));
            sql.AppendLine("FROM tolive.fe_report fer_");
            sql.AppendLine("JOIN tolive.sec_users su_");
            sql.AppendLine("    ON fer_.sec_user_entered_by = su_.user_id");

            return sql.ToString();
        }


        public static int Insert(FE_Report obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(FE_Report obj, OracleConnection conn)
        {
            if (obj.Fe_Report_Id <= 0)
                obj.Fe_Report_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.seq_fe_report"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.fe_report(");
            sql.AppendLine("fe_report_id, fe_type_id, record_date, sec_user_entered_by, temperature, ");
            sql.AppendLine("wind_direction, wind_speed, observed_weather, comments, entered_date)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":fe_report_id, :fe_type_id, :record_date, :sec_user_entered_by, :temperature, ");
            sql.AppendLine(":wind_direction, :wind_speed, :observed_weather, :comments, :entered_date)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("fe_report_id", obj.Fe_Report_Id);
            ins.Parameters.Add("fe_type_id", (int)obj.Fe_Type);
            ins.Parameters.Add("record_date", obj.Record_Date);
            ins.Parameters.Add("sec_user_entered_by", obj.Sec_User_Entered_By.User_Id);
            ins.Parameters.Add("temperature", obj.Temperature);
            ins.Parameters.Add("wind_direction", obj.Wind_Direction);
            ins.Parameters.Add("wind_speed", obj.Wind_Speed);
            ins.Parameters.Add("observed_weather", obj.Observed_Weather);
            ins.Parameters.Add("comments", obj.Comments);
            ins.Parameters.Add("entered_date", obj.Entered_Date);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(FE_Report obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(FE_Report obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.fe_report SET");
            sql.AppendLine("fe_type_id = :fe_type_id, ");
            sql.AppendLine("record_date = :record_date, ");
            sql.AppendLine("sec_user_entered_by = :sec_user_entered_by, ");
            sql.AppendLine("temperature = :temperature, ");
            sql.AppendLine("wind_direction = :wind_direction, ");
            sql.AppendLine("wind_speed = :wind_speed, ");
            sql.AppendLine("observed_weather = :observed_weather, ");
            sql.AppendLine("comments = :comments, ");
            sql.AppendLine("entered_date = :entered_date");
            sql.AppendLine("WHERE fe_report_id = :fe_report_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("fe_type_id", (int)obj.Fe_Type);
            upd.Parameters.Add("record_date", obj.Record_Date);
            upd.Parameters.Add("sec_user_entered_by", obj.Sec_User_Entered_By.User_Id);
            upd.Parameters.Add("temperature", obj.Temperature);
            upd.Parameters.Add("wind_direction", obj.Wind_Direction);
            upd.Parameters.Add("wind_speed", obj.Wind_Speed);
            upd.Parameters.Add("observed_weather", obj.Observed_Weather);
            upd.Parameters.Add("comments", obj.Comments);
            upd.Parameters.Add("entered_date", obj.Entered_Date);
            upd.Parameters.Add("fe_report_id", obj.Fe_Report_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(FE_Report obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(FE_Report obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.fe_report");
            sql.AppendLine("WHERE fe_report_id = :fe_report_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("fe_report_id", obj.Fe_Report_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static FE_Report DataRowToObject(DbDataReader row, string ColPrefix = "fer_")
        {
            FE_Report RetVal = new();
            RetVal.Fe_Report_Id = (int)Util.GetRowVal(row, $"{ColPrefix}fe_report_id");
            RetVal.Fe_Type = (FE_Type)Enum.Parse(typeof(FE_Type),((short)Util.GetRowVal(row, $"{ColPrefix}fe_type_id")).ToString());
            RetVal.Record_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}record_date");
            RetVal.Sec_User_Entered_By = Sec_UserSvc.DataRowToObject(row, "su_");
            RetVal.Temperature = (double)(Single)Util.GetRowVal(row, $"{ColPrefix}temperature");
            RetVal.Wind_Direction = (string)Util.GetRowVal(row, $"{ColPrefix}wind_direction");
            RetVal.Wind_Speed = (double)(Single)Util.GetRowVal(row, $"{ColPrefix}wind_speed");
            RetVal.Observed_Weather = (string)Util.GetRowVal(row, $"{ColPrefix}observed_weather");
            RetVal.Comments = (string)Util.GetRowVal(row, $"{ColPrefix}comments");
            RetVal.Entered_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}entered_date");
            return RetVal;
        }

    }
}
