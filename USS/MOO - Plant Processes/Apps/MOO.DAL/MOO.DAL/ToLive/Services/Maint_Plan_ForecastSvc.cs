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
    public static class Maint_Plan_ForecastSvc
    {
        static Maint_Plan_ForecastSvc()
        {
            Util.RegisterOracle();
        }


        public static Maint_Plan_Forecast Get(DateTime forecast_date)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE forecast_date = :forecast_date");


            Maint_Plan_Forecast retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("forecast_date", forecast_date);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }


        public static List<Maint_Plan_Forecast> GetAll(DateTime StartDate, DateTime EndDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE forecast_date BETWEEN :StartDate AND :EndDate");

            List<Maint_Plan_Forecast> elements = new();
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
            cols.AppendLine($"{ta}forecast_date {ColPrefix}forecast_date, {ta}crush_tons {ColPrefix}crush_tons, ");
            cols.AppendLine($"{ta}conc_lines {ColPrefix}conc_lines, {ta}aggl_tons {ColPrefix}aggl_tons");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.maint_plan_forecast");
            return sql.ToString();
        }


        public static int Insert(Maint_Plan_Forecast obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Maint_Plan_Forecast obj, OracleConnection conn)
        {
            
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Maint_Plan_Forecast(");
            sql.AppendLine("forecast_date, crush_tons, conc_lines, aggl_tons)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":forecast_date, :crush_tons, :conc_lines, :aggl_tons)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("forecast_date", obj.Forecast_Date);
            ins.Parameters.Add("crush_tons", obj.Crush_Tons);
            ins.Parameters.Add("conc_lines", obj.Conc_Lines);
            ins.Parameters.Add("aggl_tons", obj.Aggl_Tons);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Maint_Plan_Forecast obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Maint_Plan_Forecast obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Maint_Plan_Forecast SET");
            sql.AppendLine("crush_tons = :crush_tons, ");
            sql.AppendLine("conc_lines = :conc_lines, ");
            sql.AppendLine("aggl_tons = :aggl_tons");
            sql.AppendLine("WHERE forecast_date = :forecast_date");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("crush_tons", obj.Crush_Tons);
            upd.Parameters.Add("conc_lines", obj.Conc_Lines);
            upd.Parameters.Add("aggl_tons", obj.Aggl_Tons);
            upd.Parameters.Add("forecast_date", obj.Forecast_Date);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Maint_Plan_Forecast obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Maint_Plan_Forecast obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.Maint_Plan_Forecast");
            sql.AppendLine("WHERE forecast_date = :forecast_date");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("forecast_date", obj.Forecast_Date);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Maint_Plan_Forecast DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Maint_Plan_Forecast RetVal = new();
            RetVal.Forecast_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}forecast_date");
            RetVal.Crush_Tons = (decimal?)Util.GetRowVal(row, $"{ColPrefix}crush_tons");
            RetVal.Conc_Lines = (decimal?)Util.GetRowVal(row, $"{ColPrefix}conc_lines");
            RetVal.Aggl_Tons = (decimal?)Util.GetRowVal(row, $"{ColPrefix}aggl_tons");
            return RetVal;
        }

    }
}
