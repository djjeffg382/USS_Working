using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;

namespace MOO.DAL.ToLive.Services
{
    public class Agg3_Shift_StepSvc
    {
        static Agg3_Shift_StepSvc()
        {
            Util.RegisterOracle();
        }

        /// <summary>
        /// Gets the agg2 shift record by agg shift id
        /// </summary>
        /// <param name="agg3_Shift_Id"></param>
        /// <returns></returns>
        public static Agg3_Shift_Step Get(int agg3_Shift_Id, byte step)
        {
            if (step != 3)
                throw new Exception($"Invalid Agg Step Number for Agg3_Shift Table - {step}");

            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE ass.agg3_shift_id = {agg3_Shift_Id}");
            sql.AppendLine($"AND ass.step = {step}");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count == 0)
                return null;
            else
                return DataRowToObj(ds.Tables[0].Rows[0]);
        }

        /// <summary>
        /// Gets the agg2 shift record by report date and shift
        /// </summary>
        /// <param name="report_Date"></param>
        /// <returns></returns>
        public static Agg3_Shift_Step Get(DateTime report_Date, byte shift, byte step)
        {
            if (step != 3)
                throw new Exception($"Invalid Agg Step Number for Agg3_Shift Table - {step}");
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE a3s.report_date = {MOO.Dates.OraDate(report_Date)}");
            sql.AppendLine($"and a3s.shift = {shift}");
            sql.AppendLine($"AND ass.step = {step}");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count == 0)
                return null;
            else
                return DataRowToObj(ds.Tables[0].Rows[0]);
        }

        /// <summary>
        /// Updates the given shift input record for shift input app (only updates filter_on, filter_avail, filtrate
        /// </summary>
        /// <param name="agRecord"></param>
        /// <returns></returns>
        public static int UpdateFromShiftInput(Agg3_Shift_Step agRecord)
        {
            int recsUpdated = 0;
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                recsUpdated = UpdateFromShiftInput(conn, agRecord);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                conn.Close();
            }
            return recsUpdated;
        }
        /// <summary>
        /// Updates the given shift input record for shift input app (only updates the filter_avail, filter_on and filtrate columns)
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="agRecord"></param>
        /// <returns></returns>
        public static int UpdateFromShiftInput(OracleConnection conn, Agg3_Shift_Step agRecord)
        {
           

            StringBuilder sql = new();
            sql.AppendLine($"UPDATE tolive.agg3_shift_step");
            sql.AppendLine("SET filter_avail = :filter_avail,");  //set it null if it is zero
            sql.AppendLine("filter_on = :filter_on,");
            sql.AppendLine("filtrate = :filtrate");
            sql.AppendLine($"WHERE agg3_shift_id = {agRecord.Agg3_Shift_Id}");
            sql.AppendLine($"AND step = {agRecord.Step}");

            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("filter_avail", agRecord.Filter_Avail);
            cmd.Parameters.Add("filter_on", agRecord.Filter_On);
            cmd.Parameters.Add("filtrate", agRecord.FiltRate);

            //assume the connection has been opened from the calling procedure
            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// returns the select statement without the where clause
        /// </summary>
        /// <returns></returns>
        private static string GetSelect()
        {
            

            StringBuilder sql = new();
            sql.AppendLine("SELECT a3s.report_date, a3s.shift, ass.agg3_shift_id, ass.step, ROUND(ass.ball_hrs, 4) ball_hrs, ROUND(ass.bent_lbs, 4) bent_lbs,");
            sql.AppendLine("    ROUND(ass.bent_lbs_lton, 4) bent_lbs_lton, ROUND(ass.coal_stons, 4) coal_stons, ROUND(ass.dd1_inh2o, 4) dd1_inh2O,");
            sql.AppendLine("    ROUND(ass.fuel_oil_gals, 4) fuel_oil_gals, ROUND(ass.gas_mscf,4) gas_mscf,");
            sql.AppendLine("    ROUND(ass.grate_hrs,4) grate_hrs, ROUND(ass.mbtu_lton,4) mbtu_lton, ROUND(ass.mbtus,4) mbtus, ROUND(ass.pel_ltons,4) pel_ltons,");
            sql.AppendLine("    ROUND(ass.wood_stons, 4) wood_stons, ass.filter_avail, ass.filter_on, ass.filtrate,");
            sql.AppendLine("    ROUND(ass.tot_203_ltons,4) tot_203_ltons, ROUND(ass.trip_moist,4) trip_moist, ass.vac_pump_on,");
            sql.AppendLine("    ROUND(ass.drum_down_time_min,4) drum_down_time_min, ROUND(ass.down_time_min, 4) down_time_min");
            sql.AppendLine($"FROM tolive.agg3_shift_step ass");
            sql.AppendLine($"INNER JOIN tolive.agg3_shift a3s");
            sql.AppendLine($"ON ass.agg3_shift_id = a3s.agg3_shift_id");
            return sql.ToString();
        }



        /// <summary>
        /// converts the datarow to an object
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private static Agg3_Shift_Step DataRowToObj(DataRow row)
        {


            Agg3_Shift_Step retObj = new();
            retObj.Agg3_Shift_Id = Convert.ToInt32(row["agg3_shift_id"]);
            retObj.Report_Date = row.Field<DateTime>("report_date");
            retObj.Shift = Convert.ToByte(row["shift"]);
            retObj.Step = Convert.ToByte(row["step"]);
            retObj.Ball_Hrs = row.Field<decimal?>("ball_hrs");
            retObj.Bent_Lbs = row.Field<decimal?>("bent_lbs");
            retObj.Bent_Lbs_Lton = row.Field<decimal?>("bent_lbs_lton");
            retObj.Coal_Stons = row.Field<decimal?>("coal_stons");
            retObj.DD1_InH2O = row.Field<decimal?>("dd1_inh2o");
            retObj.Fuel_Oil_Gals = row.Field<decimal?>("fuel_oil_gals");
            retObj.Gas_MSCF = row.Field<decimal?>("gas_mscf");
            retObj.Grate_Hrs = row.Field<decimal?>("grate_hrs");
            retObj.MBTU_Lton = row.Field<decimal?>("mbtu_lton");
            retObj.MBTUs = row.Field<decimal?>("mbtus");
            retObj.Pel_Ltons = row.Field<decimal?>("pel_ltons");
            retObj.Wood_Stons = row.Field<decimal?>("wood_stons");
            retObj.Drum_Down_Time_Min = row.Field<decimal?>("drum_down_time_min");
            retObj.Down_Time_Min = row.Field<decimal?>("down_time_min");
            retObj.Filter_Avail = row.Field<decimal?>("filter_avail");
            retObj.Filter_On = row.Field<decimal?>("filter_on");
            retObj.FiltRate = row.Field<decimal?>("filtrate");
            retObj.Tot_203_Ltons = row.Field <decimal?> ("tot_203_ltons");
            retObj.Trip_Moist = row.Field<decimal?>("trip_moist");
            retObj.Vac_Pump_On = row.Field<decimal?>("vac_pump_on");


            return retObj;
        }
    }
}
