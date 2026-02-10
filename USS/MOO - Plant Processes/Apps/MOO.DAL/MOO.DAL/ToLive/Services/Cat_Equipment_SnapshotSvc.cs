using MOO.DAL.Drill.Models;
using MOO.DAL.ToLive.Models;
using Newtonsoft.Json.Converters;
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
    /// <summary>
    /// Service for handling the data coming from VisionLink API
    /// </summary>
    /// <remarks>Should not need an update/delete function as we will always just be inserting.  The delete function will just be a purge function</remarks>
    public static class Cat_Equipment_SnapshotSvc
    {
        static Cat_Equipment_SnapshotSvc()
        {
            Util.RegisterOracle();
        }


        public static Cat_Equipment_Snapshot Get(long eq_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE eq_id = :eq_id");


            Cat_Equipment_Snapshot retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("eq_id", eq_id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }


        public static List<Cat_Equipment_Snapshot> GetBySnapshotDate(DateTime SnapDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE snapshot_date = :SnapDate");

            List<Cat_Equipment_Snapshot> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("SnapDate", SnapDate);
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


        public static List<Cat_Equipment_Snapshot> GetLatestSnapshot()
        {
            return GetLatestSnapshot(DateTime.Now);
        }

        /// <summary>
        /// gets the latest snapshot data prior to the specified Date
        /// </summary>
        /// <param name="priorToDate">The snapshot date to get prior to this </param>
        /// <returns></returns>
        public static List<Cat_Equipment_Snapshot> GetLatestSnapshot(DateTime priorToDate)
        {
            StringBuilder sql = new();

            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.cat_equipment_snapshot ces");
            sql.AppendLine("JOIN (");
            sql.AppendLine("        SELECT eqp_serial eqSerial, MAX(snapshot_date) SnapDate");
            sql.AppendLine("        FROM tolive.cat_equipment_snapshot");
            sql.AppendLine("        WHERE snapshot_date < :priorToDate");
            sql.AppendLine("        GROUP BY eqp_serial");
            sql.AppendLine(") latest");
            sql.AppendLine("ON ces.snapshot_date = latest.snapdate AND ces.eqp_serial = latest.eqSerial");

            List<Cat_Equipment_Snapshot> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("priorToDate", priorToDate);
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
            cols.AppendLine($"{ta}eq_id {ColPrefix}eq_id, {ta}snapshot_date {ColPrefix}snapshot_date, ");
            cols.AppendLine($"{ta}eqp_model {ColPrefix}eqp_model, {ta}eqp_id {ColPrefix}eqp_id, ");
            cols.AppendLine($"{ta}eqp_serial {ColPrefix}eqp_serial, {ta}eqp_load_count {ColPrefix}eqp_load_count, ");
            cols.AppendLine($"{ta}eqp_load_date {ColPrefix}eqp_load_date, {ta}eqp_payload_ttl {ColPrefix}eqp_payload_ttl, ");
            cols.AppendLine($"{ta}eqp_payload_date {ColPrefix}eqp_payload_date, {ta}loc_lat {ColPrefix}loc_lat, ");
            cols.AppendLine($"{ta}loc_long {ColPrefix}loc_long, {ta}loc_alt {ColPrefix}loc_alt, ");
            cols.AppendLine($"{ta}loc_datetime {ColPrefix}loc_datetime, {ta}hours_idle_time {ColPrefix}hours_idle_time, ");
            cols.AppendLine($"{ta}hours_idle_date {ColPrefix}hours_idle_date, {ta}hours_oper_time {ColPrefix}hours_oper_time, ");
            cols.AppendLine($"{ta}hours_oper_date {ColPrefix}hours_oper_date, ");
            cols.AppendLine($"{ta}dist_odometer_type {ColPrefix}dist_odometer_type, ");
            cols.AppendLine($"{ta}dist_odometer_total {ColPrefix}dist_odometer_total, ");
            cols.AppendLine($"{ta}dist_odometer_date {ColPrefix}dist_odometer_date, {ta}engine_number {ColPrefix}engine_number, ");
            cols.AppendLine($"{ta}engine_running {ColPrefix}engine_running, {ta}engine_date {ColPrefix}engine_date, ");
            cols.AppendLine($"{ta}fuel_units {ColPrefix}fuel_units, {ta}fuel_consumed {ColPrefix}fuel_consumed, ");
            cols.AppendLine($"{ta}fuel_remaining_pct {ColPrefix}fuel_remaining_pct, {ta}fuel_date {ColPrefix}fuel_date,");
            cols.AppendLine($"{ta}wenco_equip_ident {ColPrefix}wenco_equip_ident, {ta}plant {ColPrefix}plant");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.cat_equipment_snapshot");
            return sql.ToString();
        }


        public static int Insert(Cat_Equipment_Snapshot obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Cat_Equipment_Snapshot obj, OracleConnection conn)
        {
            if (obj.Eq_Id <= 0)
                obj.Eq_Id = Convert.ToInt32(MOO.Data.GetNextSequence("ToLive.SEQ_CAT_EQP_SNAPSHOT"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO ToLive.Cat_Equipment_Snapshot(");
            sql.AppendLine("eq_id, snapshot_date, eqp_model, eqp_id, eqp_serial, eqp_load_count, eqp_load_date, ");
            sql.AppendLine("eqp_payload_ttl, eqp_payload_date, loc_lat, loc_long, loc_alt, loc_datetime, ");
            sql.AppendLine("hours_idle_time, hours_idle_date, hours_oper_time, hours_oper_date, ");
            sql.AppendLine("dist_odometer_type, dist_odometer_total, dist_odometer_date, engine_number, ");
            sql.AppendLine("engine_running, engine_date, fuel_units, fuel_consumed, fuel_remaining_pct, ");
            sql.AppendLine("fuel_date, wenco_equip_ident, plant)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":eq_id, :snapshot_date, :eqp_model, :eqp_id, :eqp_serial, :eqp_load_count, ");
            sql.AppendLine(":eqp_load_date, :eqp_payload_ttl, :eqp_payload_date, :loc_lat, :loc_long, :loc_alt, ");
            sql.AppendLine(":loc_datetime, :hours_idle_time, :hours_idle_date, :hours_oper_time, :hours_oper_date, ");
            sql.AppendLine(":dist_odometer_type, :dist_odometer_total, :dist_odometer_date, :engine_number, ");
            sql.AppendLine(":engine_running, :engine_date, :fuel_units, :fuel_consumed, :fuel_remaining_pct, ");
            sql.AppendLine(":fuel_date, :wenco_equip_ident, :plant)");
            OracleCommand ins = new(sql.ToString(), conn) { BindByName = true };
            ins.Parameters.Add("eq_id", obj.Eq_Id);
            ins.Parameters.Add("snapshot_date", obj.Snapshot_Date);
            ins.Parameters.Add("eqp_model", obj.Eqp_Model);
            ins.Parameters.Add("eqp_id", obj.Eqp_Id);
            ins.Parameters.Add("eqp_serial", obj.Eqp_Serial);
            ins.Parameters.Add("eqp_load_count", obj.Eqp_Load_Count);
            ins.Parameters.Add("eqp_load_date", obj.Eqp_Load_Date);
            ins.Parameters.Add("eqp_payload_ttl", obj.Eqp_Payload_Ttl);
            ins.Parameters.Add("eqp_payload_date", obj.Eqp_Payload_Date);
            ins.Parameters.Add("loc_lat", obj.Loc_Lat);
            ins.Parameters.Add("loc_long", obj.Loc_Long);
            ins.Parameters.Add("loc_alt", obj.Loc_Alt);
            ins.Parameters.Add("loc_datetime", obj.Loc_Datetime);
            ins.Parameters.Add("hours_idle_time", obj.Hours_Idle_Time);
            ins.Parameters.Add("hours_idle_date", obj.Hours_Idle_Date);
            ins.Parameters.Add("hours_oper_time", obj.Hours_Oper_Time);
            ins.Parameters.Add("hours_oper_date", obj.Hours_Oper_Date);
            ins.Parameters.Add("dist_odometer_type", obj.Dist_Odometer_Type);
            ins.Parameters.Add("dist_odometer_total", obj.Dist_Odometer_Total);
            ins.Parameters.Add("dist_odometer_date", obj.Dist_Odometer_Date);
            ins.Parameters.Add("engine_number", obj.Engine_Number);
            if (obj.Engine_Running.HasValue)
                ins.Parameters.Add("engine_running", obj.Engine_Running.Value?1:0);
            else
                ins.Parameters.Add("engine_running", DBNull.Value);
            ins.Parameters.Add("engine_date", obj.Engine_Date);
            ins.Parameters.Add("fuel_units", obj.Fuel_Units);
            ins.Parameters.Add("fuel_consumed", obj.Fuel_Consumed);
            ins.Parameters.Add("fuel_remaining_pct", obj.Fuel_Remaining_Pct);
            ins.Parameters.Add("fuel_date", obj.Fuel_Date);
            ins.Parameters.Add("wenco_equip_ident", obj.Wenco_Equip_Ident);
            ins.Parameters.Add("plant", obj.Plant.HasValue ? obj.Plant.ToString(): DBNull.Value);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }




        public static int Purge(DateTime DeletePriorTo)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM ToLive.Cat_Equipment_Snapshot");
            sql.AppendLine("WHERE snapshot_date < :DeleteDate");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand del = new(sql.ToString(), conn) { BindByName = true };
            del.Parameters.Add("DeleteDate", DeletePriorTo);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            conn.Close();
            return recsAffected;
        }


        internal static Cat_Equipment_Snapshot DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Cat_Equipment_Snapshot RetVal = new();
            RetVal.Eq_Id = Convert.ToInt64(Util.GetRowVal(row, $"{ColPrefix}eq_id"));
            RetVal.Snapshot_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}snapshot_date");
            RetVal.Eqp_Model = (string)Util.GetRowVal(row, $"{ColPrefix}eqp_model");
            RetVal.Eqp_Id = (string)Util.GetRowVal(row, $"{ColPrefix}eqp_id");
            RetVal.Eqp_Serial = (string)Util.GetRowVal(row, $"{ColPrefix}eqp_serial");
            if(Enum.TryParse<MOO.Plant>((string)Util.GetRowVal(row, $"{ColPrefix}plant"), false, out MOO.Plant tmpPlant))
                RetVal.Plant = tmpPlant;
            else
                RetVal.Plant = null;

            RetVal.Wenco_Equip_Ident = (string)Util.GetRowVal(row, $"{ColPrefix}Wenco_Equip_Ident");
            RetVal.Eqp_Load_Count = (long?)Util.GetRowVal(row, $"{ColPrefix}eqp_load_count");
            RetVal.Eqp_Load_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}eqp_load_date");
            RetVal.Eqp_Payload_Ttl = (long?)(decimal?)Util.GetRowVal(row, $"{ColPrefix}eqp_payload_ttl");
            RetVal.Eqp_Payload_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}eqp_payload_date");
            RetVal.Loc_Lat = (string)Util.GetRowVal(row, $"{ColPrefix}loc_lat");
            RetVal.Loc_Long = (string)Util.GetRowVal(row, $"{ColPrefix}loc_long");
            RetVal.Loc_Alt = (string)Util.GetRowVal(row, $"{ColPrefix}loc_alt");
            RetVal.Loc_Datetime = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}loc_datetime");
            RetVal.Hours_Idle_Time = (double?)Util.GetRowVal(row, $"{ColPrefix}hours_idle_time");
            RetVal.Hours_Idle_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}hours_idle_date");
            RetVal.Hours_Oper_Time = (double?)Util.GetRowVal(row, $"{ColPrefix}hours_oper_time");
            RetVal.Hours_Oper_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}hours_oper_date");
            RetVal.Dist_Odometer_Type = (string)Util.GetRowVal(row, $"{ColPrefix}dist_odometer_type");
            RetVal.Dist_Odometer_Total = (double?)Util.GetRowVal(row, $"{ColPrefix}dist_odometer_total");
            RetVal.Dist_Odometer_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}dist_odometer_date");
            RetVal.Engine_Number = (string)Util.GetRowVal(row, $"{ColPrefix}engine_number");

            short? temp = (short?)Util.GetRowVal(row, $"{ColPrefix}engine_running");
            if (temp.HasValue)
                RetVal.Engine_Running = temp.Value == 1;
            else
                RetVal.Engine_Running = null;


            RetVal.Engine_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}engine_date");
            RetVal.Fuel_Units = (string)Util.GetRowVal(row, $"{ColPrefix}fuel_units");
            RetVal.Fuel_Consumed = (double?)Util.GetRowVal(row, $"{ColPrefix}fuel_consumed");
            RetVal.Fuel_Remaining_Pct = (short?)Util.GetRowVal(row, $"{ColPrefix}fuel_remaining_pct");
            RetVal.Fuel_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}fuel_date");
            return RetVal;
        }

    }
}
