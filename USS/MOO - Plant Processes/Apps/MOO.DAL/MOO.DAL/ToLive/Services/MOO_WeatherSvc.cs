using MOO.DAL.ToLive.Enums;
using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.Enums;
using MOO.Enums.Extension;

namespace MOO.DAL.ToLive.Services
{
    public static class MOO_WeatherSvc
    {
        static MOO_WeatherSvc()
        {
            Util.RegisterOracle();
        }


        public static MOO_Weather GetLatest(MOO_Weather.Weather_City City)
        {
            StringBuilder sql = new();

            sql.AppendLine("SELECT * FROM (");
            sql.Append(GetSelect());
            sql.AppendLine("WHERE city = :city");
            sql.AppendLine(")");
            sql.AppendLine("WHERE rn = 1");


            MOO_Weather retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("city", City.ToString().ToUpper());
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }


        public static List<MOO_Weather> GetByDateRange(MOO_Weather.Weather_City City, DateTime StartDate, DateTime EndDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE thedate BETWEEN :startDate AND :endDate");
            sql.AppendLine("AND city = :city");

            List<MOO_Weather> elements = [];
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("startDate", StartDate);
            cmd.Parameters.Add("endDate", EndDate);
            cmd.Parameters.Add("city", City.ToString().ToUpper());
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
            cols.AppendLine($"{ta}weatherkey {ColPrefix}weatherkey, {ta}city {ColPrefix}city, {ta}thedate {ColPrefix}thedate, ");
            cols.AppendLine($"{ta}sky {ColPrefix}sky, {ta}temperature {ColPrefix}temperature, {ta}duepoint {ColPrefix}duepoint, ");
            cols.AppendLine($"{ta}relativehumidity {ColPrefix}relativehumidity, {ta}wind {ColPrefix}wind, ");
            cols.AppendLine($"{ta}pressure {ColPrefix}pressure, {ta}rawdata {ColPrefix}rawdata");
            cols.AppendLine(", ROW_NUMBER() OVER(ORDER BY thedate desc) rn");  //adding this so the get latest will work
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.moo_weather");
            return sql.ToString();
        }


        public static int Insert(MOO_Weather obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(MOO_Weather obj, OracleConnection conn)
        {

            long newKey = Convert.ToInt64((decimal)MOO.Data.ExecuteScalar("SELECT MAX(weatherkey) + 1 FROM tolive.moo_weather", Data.MNODatabase.DMART)) ;

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.MOO_Weather(");
            sql.AppendLine("weatherkey, city, thedate, sky, temperature, duepoint, relativehumidity, wind, ");
            sql.AppendLine("pressure, rawdata)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":weatherkey, :city, :thedate, :sky, :temperature, :duepoint, :relativehumidity, :wind, ");
            sql.AppendLine(":pressure, :rawdata)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("weatherkey", newKey);
            ins.Parameters.Add("city", obj.City.ToString().ToUpper());
            ins.Parameters.Add("thedate", obj.Thedate);
            ins.Parameters.Add("sky", obj.Sky);
            ins.Parameters.Add("temperature", obj.Temperature.ToString());
            ins.Parameters.Add("duepoint", obj.Dewpoint.ToString());
            ins.Parameters.Add("relativehumidity", obj.Relativehumidity.ToString());
            ins.Parameters.Add("wind", obj.Wind);
            ins.Parameters.Add("pressure", obj.Pressure.ToString());
            ins.Parameters.Add("rawdata", obj.Rawdata);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            obj.Weatherkey = newKey;
            return recsAffected;
        }


        public static int Update(MOO_Weather obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(MOO_Weather obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.MOO_Weather SET");
            sql.AppendLine("city = :city, ");
            sql.AppendLine("thedate = :thedate, ");
            sql.AppendLine("sky = :sky, ");
            sql.AppendLine("temperature = :temperature, ");
            sql.AppendLine("duepoint = :duepoint, ");
            sql.AppendLine("relativehumidity = :relativehumidity, ");
            sql.AppendLine("wind = :wind, ");
            sql.AppendLine("pressure = :pressure, ");
            sql.AppendLine("rawdata = :rawdata");
            sql.AppendLine("WHERE weatherkey = :weatherkey");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("city", obj.City.ToString().ToUpper());
            upd.Parameters.Add("thedate", obj.Thedate);
            upd.Parameters.Add("sky", obj.Sky);
            upd.Parameters.Add("temperature", obj.Temperature.ToString());
            upd.Parameters.Add("duepoint", obj.Dewpoint.ToString());
            upd.Parameters.Add("relativehumidity", obj.Relativehumidity.ToString());
            upd.Parameters.Add("wind", obj.Wind);
            upd.Parameters.Add("pressure", obj.Pressure.ToString());
            upd.Parameters.Add("rawdata", obj.Rawdata);
            upd.Parameters.Add("weatherkey", obj.Weatherkey);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(MOO_Weather obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(MOO_Weather obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.MOO_Weather");
            sql.AppendLine("WHERE weatherkey = :weatherkey");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("weatherkey", obj.Weatherkey);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static MOO_Weather DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            MOO_Weather RetVal = new();
            RetVal.Weatherkey = (long)Util.GetRowVal(row, $"{ColPrefix}weatherkey");
            RetVal.City = Enum.Parse<MOO_Weather.Weather_City>( (string)Util.GetRowVal(row, $"{ColPrefix}city"), true);
            RetVal.Thedate = (DateTime)Util.GetRowVal(row, $"{ColPrefix}thedate");
            RetVal.Sky = (string)Util.GetRowVal(row, $"{ColPrefix}sky");

            RetVal.Temperature = NullDoubleParse( (string)Util.GetRowVal(row, $"{ColPrefix}temperature"));
            RetVal.Dewpoint = NullDoubleParse((string)Util.GetRowVal(row, $"{ColPrefix}duepoint"));
            RetVal.Relativehumidity = NullDoubleParse((string)Util.GetRowVal(row, $"{ColPrefix}relativehumidity"));
            RetVal.Wind = (string)Util.GetRowVal(row, $"{ColPrefix}wind");
            string press = (string)Util.GetRowVal(row, $"{ColPrefix}pressure");
            //2016 and earlier data had a R - Rising and F - Falling.  Lets remove this.
            if(!string.IsNullOrEmpty( press ))
                RetVal.Pressure = NullDoubleParse(press.Replace("R","").Replace("F",""));
            RetVal.Rawdata = (string)Util.GetRowVal(row, $"{ColPrefix}rawdata");
            return RetVal;
        }

        /// <summary>
        /// parses the string to a double and returns null if it can't
        /// </summary>
        /// <param name="pareVal"></param>
        /// <returns></returns>
        private static double? NullDoubleParse(string pareVal)
        {
            if(double.TryParse(pareVal, out var retVal)) 
                return retVal;
            else 
                return null;
        }

    }
}
