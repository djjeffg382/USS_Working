using Microsoft.Data.SqlClient;
using MOO.DAL.MineStar.Models;
using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.MineStar.Services
{
    public static class Fleet_Summary_SMUSvc
    {
        static Fleet_Summary_SMUSvc()
        {
            Util.RegisterSqlServer();
        }


        public static Fleet_Summary_SMU Get(int RowId)
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT fss.*");
            sql.AppendLine("FROM msvl.dbo.fleetsummary_smu fss");
            sql.AppendLine($"WHERE row_id = {RowId}");

            using SqlConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.MineStar));
            conn.Open();
            SqlCommand cmd = new(sql.ToString(), conn);
            SqlDataReader rdr = cmd.ExecuteReader();
            Fleet_Summary_SMU retVal = null;
            if (rdr.HasRows)
                retVal = DataRowToObject(rdr);
            conn.Close();
            return retVal;
        }

        /// <summary>
        /// gets the latest snapshot data
        /// </summary>
        /// <returns></returns>
        public static List<Fleet_Summary_SMU> GetLatestSnapshot()
        {
            StringBuilder sql = new();

            sql.AppendLine("SELECT fss.*");
            sql.AppendLine("FROM msvl.dbo.fleetsummary_smu fss");
            sql.AppendLine("JOIN (");
            sql.AppendLine("        SELECT eqp_serial eqSerial, MAX(snapshot_date) SnapDate");
            sql.AppendLine("        FROM msvl.dbo.fleetsummary_smu");
            sql.AppendLine("        GROUP BY eqp_serial");
            sql.AppendLine(") latest");
            sql.AppendLine("ON fss.snapshot_date = latest.snapdate AND fss.eqp_serial = latest.eqSerial");

            List<Fleet_Summary_SMU> elements = new();
            using SqlConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.MineStar));
            conn.Open();
            SqlCommand cmd = new(sql.ToString(), conn);
            SqlDataReader rdr = cmd.ExecuteReader();
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




        public static int Insert(Fleet_Summary_SMU obj)
        {
            using SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.MineStar));
            conn.Open();
            int retVal = Insert(obj, conn);
            conn.Close();
            return retVal;
        }

        public static int Insert(Fleet_Summary_SMU obj, SqlConnection conn)
        {

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO msvl.dbo.fleetsummary_smu(");
            sql.AppendLine("snapshot_date, eqp_model, eqp_id, eqp_serial, eqp_load_count, eqp_load_date, ");
            sql.AppendLine("eqp_payload_ttl, eqp_payload_date, loc_lat, loc_long, loc_alt, loc_datetime, ");
            sql.AppendLine("hours_idle_time, hours_idle_date, hours_oper_time, hours_oper_date, ");
            sql.AppendLine("dist_odometer_type, dist_odometer_total, dist_odometer_date, engine_number, ");
            sql.AppendLine("engine_running, engine_date, fuel_units, fuel_consumed, fuel_remaining_pct, ");
            sql.AppendLine("fuel_date)");
            sql.AppendLine("VALUES(");
            sql.AppendLine("@snapshot_date, @eqp_model, @eqp_id, @eqp_serial, @eqp_load_count, ");
            sql.AppendLine("@eqp_load_date, @eqp_payload_ttl, @eqp_payload_date, @loc_lat, @loc_long, @loc_alt, ");
            sql.AppendLine("@loc_datetime, @hours_idle_time, @hours_idle_date, @hours_oper_time, @hours_oper_date, ");
            sql.AppendLine("@dist_odometer_type, @dist_odometer_total, @dist_odometer_date, @engine_number, ");
            sql.AppendLine("@engine_running, @engine_date, @fuel_units, @fuel_consumed, @fuel_remaining_pct, ");
            sql.AppendLine("@fuel_date)");
            SqlCommand ins = new(sql.ToString(), conn);
            ins.Parameters.AddWithValue("snapshot_date", obj.Snapshot_Date);
            ins.Parameters.AddWithValue("eqp_model", obj.Eqp_Model);
            ins.Parameters.AddWithValue("eqp_id", obj.Eqp_Id == null ? "" : obj.Eqp_Id);
            ins.Parameters.AddWithValue("eqp_serial", obj.Eqp_Serial);
            ins.Parameters.AddWithValue("eqp_load_count", obj.Eqp_Load_Count == null ? DBNull.Value : obj.Eqp_Load_Count);
            ins.Parameters.AddWithValue("eqp_load_date", obj.Eqp_Load_Date == null ? DBNull.Value : obj.Eqp_Load_Date);
            ins.Parameters.AddWithValue("eqp_payload_ttl", obj.Eqp_Payload_Ttl == null ? DBNull.Value : obj.Eqp_Payload_Ttl);
            ins.Parameters.AddWithValue("eqp_payload_date", obj.Eqp_Payload_Date == null ? DBNull.Value : obj.Eqp_Payload_Date);
            ins.Parameters.AddWithValue("loc_lat", obj.Loc_Lat == null ? DBNull.Value : obj.Loc_Lat);
            ins.Parameters.AddWithValue("loc_long", obj.Loc_Long == null ? DBNull.Value : obj.Loc_Long);
            ins.Parameters.AddWithValue("loc_alt", obj.Loc_Alt == null ? DBNull.Value : obj.Loc_Alt);
            ins.Parameters.AddWithValue("loc_datetime", obj.Loc_Datetime == null ? DBNull.Value : obj.Loc_Datetime);
            ins.Parameters.AddWithValue("hours_idle_time", obj.Hours_Idle_Time == null ? DBNull.Value : obj.Hours_Idle_Time);
            ins.Parameters.AddWithValue("hours_idle_date", obj.Hours_Idle_Date == null ? DBNull.Value : obj.Hours_Idle_Date);
            ins.Parameters.AddWithValue("hours_oper_time", obj.Hours_Oper_Time == null ? DBNull.Value : obj.Hours_Oper_Time);
            ins.Parameters.AddWithValue("hours_oper_date", obj.Hours_Oper_Date == null ? DBNull.Value : obj.Hours_Oper_Date);
            ins.Parameters.AddWithValue("dist_odometer_type", obj.Dist_Odometer_Type == null ? DBNull.Value : obj.Dist_Odometer_Type);
            ins.Parameters.AddWithValue("dist_odometer_total", obj.Dist_Odometer_Total == null ? DBNull.Value : obj.Dist_Odometer_Total);
            ins.Parameters.AddWithValue("dist_odometer_date", obj.Dist_Odometer_Date == null ? DBNull.Value : obj.Dist_Odometer_Date);
            ins.Parameters.AddWithValue("engine_number", obj.Engine_Number == null ? DBNull.Value : obj.Engine_Number);
            ins.Parameters.AddWithValue("engine_running", obj.Engine_Running == null ? DBNull.Value : obj.Engine_Running);
            ins.Parameters.AddWithValue("engine_date", obj.Engine_Date == null ? DBNull.Value : obj.Engine_Date);
            ins.Parameters.AddWithValue("fuel_units", obj.Fuel_Units == null ? DBNull.Value : obj.Fuel_Units);
            ins.Parameters.AddWithValue("fuel_consumed", obj.Fuel_Consumed == null ? DBNull.Value : obj.Fuel_Consumed);
            ins.Parameters.AddWithValue("fuel_remaining_pct", obj.Fuel_Remaining_Pct == null ? DBNull.Value : obj.Fuel_Remaining_Pct);
            ins.Parameters.AddWithValue("fuel_date", obj.Fuel_Date == null ? DBNull.Value : obj.Fuel_Date);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;

        }



        /// <summary>
        /// Purges data in the fleetsummary_smu table older than the date provided
        /// </summary>
        /// <param name="DeletePriorTo"></param>
        /// <returns></returns>
        public static int Purge(DateTime DeletePriorTo)
        {
            using SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.MineStar));
            conn.Open();
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM msvl.dbo.fleetsummary_smu ");
            sql.AppendLine("WHERE snapshot_date < @SnapDate");

            SqlCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.AddWithValue("SnapDate", DeletePriorTo);
            int recsAffected = cmd.ExecuteNonQuery();
            conn.Close();
            return recsAffected;

        }

        /// <summary>
        /// Updates the Temp_SMU table with just the latest snapshot data
        /// </summary>
        /// <param name="SnapshotDate"></param>
        /// <returns></returns>
        public static int UpdateTempSMU(DateTime SnapshotDate)
        {
            using SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.MineStar));
            conn.Open();
            SqlCommand delCmd = new("DELETE FROM msvl.dbo.temp_smu", conn);
            delCmd.ExecuteNonQuery();
            SqlCommand inscmd = new("INSERT INTO msvl.dbo.temp_smu SELECT * FROM msvl.dbo.FLEETSUMMARY_SMU WHERE SNAPSHOT_DATE = @snaptime", conn);
            inscmd.Parameters.Add("snaptime", SqlDbType.DateTime);
            inscmd.Parameters["snaptime"].Value = SnapshotDate;
            int recsAffected = inscmd.ExecuteNonQuery();
            conn.Close();
            return recsAffected;
        }



        internal static Fleet_Summary_SMU DataRowToObject(SqlDataReader row)
        {
            Fleet_Summary_SMU RetVal = new();
            RetVal.Row_Id = Convert.ToInt32(Util.GetRowVal(row, "row_id"));
            RetVal.Snapshot_Date = (DateTime)Util.GetRowVal(row, "snapshot_date");
            RetVal.Eqp_Model = (string)Util.GetRowVal(row, "eqp_model");
            RetVal.Eqp_Id = (string)Util.GetRowVal(row, "eqp_id");
            RetVal.Eqp_Serial = (string)Util.GetRowVal(row, "eqp_serial");
            decimal? tempEqp_Load_Count = (decimal?)Util.GetRowVal(row, "eqp_load_count");
            if (tempEqp_Load_Count.HasValue)
                RetVal.Eqp_Load_Count = Convert.ToInt64(tempEqp_Load_Count.Value);
            else
                RetVal.Fuel_Remaining_Pct = null;


            RetVal.Eqp_Load_Date = (DateTime?)Util.GetRowVal(row, "eqp_load_date");
            RetVal.Eqp_Payload_Ttl = (long?)(decimal?)Util.GetRowVal(row, "eqp_payload_ttl");
            RetVal.Eqp_Payload_Date = (DateTime?)Util.GetRowVal(row, "eqp_payload_date");
            RetVal.Loc_Lat = (double?)(decimal?)Util.GetRowVal(row, "loc_lat");
            RetVal.Loc_Long = (double?)(decimal?)Util.GetRowVal(row, "loc_long");
            RetVal.Loc_Alt = (double?)(decimal?)Util.GetRowVal(row, "loc_alt");
            RetVal.Loc_Datetime = (DateTime?)Util.GetRowVal(row, "loc_datetime");
            RetVal.Hours_Idle_Time = (double?)(decimal?)Util.GetRowVal(row, "hours_idle_time");
            RetVal.Hours_Idle_Date = (DateTime?)Util.GetRowVal(row, "hours_idle_date");
            RetVal.Hours_Oper_Time = (double?)(decimal?)Util.GetRowVal(row, "hours_oper_time");
            RetVal.Hours_Oper_Date = (DateTime?)Util.GetRowVal(row, "hours_oper_date");
            RetVal.Dist_Odometer_Type = (string)Util.GetRowVal(row, "dist_odometer_type");
            RetVal.Dist_Odometer_Total = (double?)(decimal?)Util.GetRowVal(row, "dist_odometer_total");
            RetVal.Dist_Odometer_Date = (DateTime?)Util.GetRowVal(row, "dist_odometer_date");
            RetVal.Engine_Number = (string)Util.GetRowVal(row, "engine_number");
            RetVal.Engine_Running = (bool?)Util.GetRowVal(row, "engine_running");
            RetVal.Engine_Date = (DateTime?)Util.GetRowVal(row, "engine_date");
            RetVal.Fuel_Units = (string)Util.GetRowVal(row, "fuel_units");
            RetVal.Fuel_Consumed = (double?)(decimal?)Util.GetRowVal(row, "fuel_consumed");

            decimal? tempFuel = (decimal?)Util.GetRowVal(row, "fuel_remaining_pct");
            if (tempFuel.HasValue)
                RetVal.Fuel_Remaining_Pct = Convert.ToInt16(tempFuel.Value);
            else
                RetVal.Fuel_Remaining_Pct = null;

            RetVal.Fuel_Date = (DateTime?)Util.GetRowVal(row, "fuel_date");
            return RetVal;
        }
    }
}
