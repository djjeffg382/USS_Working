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
    public static class Agg3_ShiftSvc
    {

        static Agg3_ShiftSvc()
        {
            Util.RegisterOracle();
        }

        /// <summary>
        /// Gets the agg2 shift record by agg2 shift id
        /// </summary>
        /// <param name="agg3_Shift_Id"></param>
        /// <returns></returns>
        public static Agg3_Shift Get(int agg3_Shift_Id)
        {

            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE agg3_shift_id = {agg3_Shift_Id}");

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
        public static Agg3_Shift Get(DateTime report_Date, byte shift)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE report_date = {MOO.Dates.OraDate(report_Date)}");
            sql.AppendLine($"and shift = {shift}");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count == 0)
                return null;
            else
                return DataRowToObj(ds.Tables[0].Rows[0]);
        }

        /// <summary>
        /// Updates the given shift input record for shift input app (only updates pel tons, bin pct, and general_desc
        /// </summary>
        /// <param name="ag3Record"></param>
        /// <returns></returns>
        public static int UpdateFromShiftInput(Agg3_Shift ag3Record)
        {
            int recsUpdated = 0;
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                recsUpdated = UpdateFromShiftInput(conn, ag3Record);
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
        /// Updates the given shift input record for shift input app (only updates the reclaim in/out adjustments)
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="agRecord"></param>
        /// <returns></returns>
        public static int UpdateFromShiftInput(OracleConnection conn, Agg3_Shift agRecord)
        {


            StringBuilder sql = new();
            sql.AppendLine($"UPDATE tolive.agg3_shift");
            sql.AppendLine("SET recl_in_adj_ltons = :recl_in_adj_ltons,");  //set it null if it is zero
            sql.AppendLine("recl_out_adj_ltons = :recl_out_adj_ltons,");
            sql.AppendLine("recl_in_ltons = :recl_in_ltons,");
            sql.AppendLine("recl_out_ltons = :recl_out_ltons");
            sql.AppendLine($"WHERE agg3_shift_id = {agRecord.Agg3_Shift_Id}");

            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("recl_in_adj_ltons", agRecord.Recl_In_Adj_Ltons);
            cmd.Parameters.Add("recl_out_adj_ltons", agRecord.Recl_Out_Adj_Ltons);
            cmd.Parameters.Add("recl_in_ltons", agRecord.Recl_In_Ltons);
            cmd.Parameters.Add("recl_out_ltons", agRecord.Recl_Out_Ltons);

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
            sql.AppendLine($"SELECT agg3_shift_id, report_date, shift, ROUND(lines_running_avg,4) lines_running_avg,");
            sql.AppendLine("    ROUND(lines_running_sum,4) lines_running_sum, ");
            sql.AppendLine("    ROUND(tank_slurry_2_1_pct,4) tank_slurry_2_1_pct, ROUND(tank_slurry_2_2_pct,4) tank_slurry_2_2_pct,");
            sql.AppendLine("    ROUND(ball_hrs, 4) ball_hrs, ROUND(bent_lbs, 4) bent_lbs, ROUND(Recl_Bal_Ltons,4) Recl_Bal_Ltons,");
            sql.AppendLine("    ROUND(bent_lbs_lton, 4) bent_lbs_lton, ROUND(coal_stons, 4) coal_stons, ROUND(dd1_inh2o, 4) dd1_inh2O,");
            sql.AppendLine("    ROUND(fuel_oil_gals, 4) fuel_oil_gals, ROUND(fuel_oil_tank_lvl,4) fuel_oil_tank_lvl, ROUND(gas_mscf,4) gas_mscf,");
            sql.AppendLine("    ROUND(grate_hrs,4) grate_hrs, ROUND(mbtu_lton,4) mbtu_lton, ROUND(mbtus,4) mbtus, ROUND(pel_ltons,4) pel_ltons,");
            sql.AppendLine("    ROUND(recl_in_ltons,4) recl_in_ltons, ROUND(recl_out_ltons) recl_out_ltons, ROUND(wood_stons, 4) wood_stons,");
            sql.AppendLine("    ROUND(recl_in_act_ltons,4) recl_in_act_ltons, ROUND(recl_out_act_ltons,4) recl_out_act_ltons,");
            sql.AppendLine("    ROUND(recl_in_adj_ltons,4) recl_in_adj_ltons, ROUND(recl_out_adj_ltons,4) recl_out_adj_ltons");
            sql.AppendLine($"FROM Agg3_Shift a3s");
            return sql.ToString();
        }



        /// <summary>
        /// converts the datarow to an object
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private static Agg3_Shift DataRowToObj(DataRow row)
        {


            Agg3_Shift retObj = new();
            retObj.Agg3_Shift_Id = Convert.ToInt32(row["agg3_shift_id"]);
            retObj.Report_Date = row.Field<DateTime>("report_date");
            retObj.Shift = Convert.ToByte(row["shift"]);
            retObj.Lines_Running_Avg = row.Field<decimal?>("lines_running_avg");
            retObj.Lines_Running_Sum = row.Field<decimal?>("lines_running_sum");
            retObj.Tank_Slurry_2_1_Pct = row.Field<decimal?>("tank_slurry_2_1_pct");
            retObj.Tank_Slurry_2_2_Pct = row.Field<decimal?>("tank_slurry_2_2_pct");
            retObj.Recl_Bal_Ltons = row.Field<decimal?>("Recl_Bal_Ltons");

            retObj.Ball_Hrs = row.Field<decimal?>("ball_hrs");
            retObj.Bent_Lbs = row.Field<decimal?>("bent_lbs");
            retObj.Bent_Lbs_Lton = row.Field<decimal?>("bent_lbs_lton");
            retObj.Coal_Stons = row.Field<decimal?>("coal_stons");
            retObj.DD1_InH2O = row.Field<decimal?>("dd1_inh2o");
            retObj.Fuel_Oil_Gals = row.Field<decimal?>("fuel_oil_gals");
            retObj.Fuel_Oil_Tank_Lvl = row.Field<decimal?>("fuel_oil_tank_lvl");
            retObj.Gas_MSCF = row.Field<decimal?>("gas_mscf");
            retObj.Grate_Hrs = row.Field<decimal?>("grate_hrs");
            retObj.MBTU_Lton = row.Field<decimal?>("mbtu_lton");
            retObj.MBTUs = row.Field<decimal?>("mbtus");
            retObj.Pel_Ltons = row.Field<decimal?>("pel_ltons");
            retObj.Recl_In_Adj_Ltons = row.Field<decimal?>("recl_in_adj_ltons");
            retObj.Recl_Out_Adj_Ltons = row.Field<decimal?>("recl_out_adj_ltons");
            retObj.Wood_Stons = row.Field<decimal?>("wood_stons");
            retObj.Recl_In_Act_Ltons = row.Field<decimal?>("recl_in_act_ltons");
            retObj.Recl_Out_Act_Ltons = row.Field<decimal?>("recl_out_act_ltons");


            return retObj;
        }
    }
}
