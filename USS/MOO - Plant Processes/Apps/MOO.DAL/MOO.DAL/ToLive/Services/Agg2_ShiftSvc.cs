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
    public static class Agg2_ShiftSvc
    {

        static Agg2_ShiftSvc()
        {
            Util.RegisterOracle();
        }

        /// <summary>
        /// Gets the agg2 shift record by agg2 shift id
        /// </summary>
        /// <param name="agg2_Shift_Id"></param>
        /// <returns></returns>
        public static Agg2_Shift Get(int agg2_Shift_Id)
        {

            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE agg2_shift_id = {agg2_Shift_Id}");

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
        public static Agg2_Shift Get(DateTime report_Date, byte shift)
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
        /// <param name="ag2Record"></param>
        /// <returns></returns>
        public static int UpdateFromShiftInput(Agg2_Shift ag2Record)
        {
            int recsUpdated = 0;
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                recsUpdated = UpdateFromShiftInput(conn, ag2Record);
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
        public static int UpdateFromShiftInput(OracleConnection conn, Agg2_Shift agRecord)
        {


            StringBuilder sql = new();
            sql.AppendLine($"UPDATE tolive.agg2_shift");
            sql.AppendLine("SET recl_in_adj_ltons = :recl_in_adj_ltons,");  //set it null if it is zero
            sql.AppendLine("recl_out_adj_ltons = :recl_out_adj_ltons,");
            sql.AppendLine("recl_in_ltons = :recl_in_ltons,");
            sql.AppendLine("recl_out_ltons = :recl_out_ltons,");
            sql.AppendLine("bin1_pct = :bin1_pct,");
            sql.AppendLine("bin2_pct = :bin2_pct");
            sql.AppendLine($"WHERE agg2_shift_id = {agRecord.Agg2_Shift_Id}");

            OracleCommand cmd = new(sql.ToString(), conn) { BindByName = true } ;
            cmd.Parameters.Add("recl_in_adj_ltons", agRecord.Recl_In_Adj_Ltons);
            cmd.Parameters.Add("recl_out_adj_ltons", agRecord.Recl_Out_Adj_Ltons);
            cmd.Parameters.Add("recl_in_ltons", agRecord.Recl_In_Ltons);
            cmd.Parameters.Add("recl_out_ltons", agRecord.Recl_Out_Ltons);
            cmd.Parameters.Add("bin1_pct", agRecord.Bin1_Pct);
            cmd.Parameters.Add("bin2_pct", agRecord.Bin2_Pct);

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
            sql.AppendLine($"SELECT agg2_shift_id, report_date, shift, ROUND(lines_running_avg,4) lines_running_avg,");
            sql.AppendLine("    ROUND(lines_running_sum,4) lines_running_sum, ROUND(tank_flux_1_pct,4) tank_flux_1_pct, ");
            sql.AppendLine("    ROUND(tank_flux_2_pct, 4) tank_flux_2_pct, ");
            sql.AppendLine("ROUND(tank_slurry_1_1_pct,4) tank_slurry_1_1_pct, ROUND(tank_slurry_1_2_pct,4) tank_slurry_1_2_pct,");
            sql.AppendLine("    ROUND(ball_hrs, 4) ball_hrs, ROUND(bent_lbs, 4) bent_lbs,");
            sql.AppendLine("    ROUND(bent_lbs_lton, 4) bent_lbs_lton, ROUND(coal_stons, 4) coal_stons, ROUND(dd1_inh2o, 4) dd1_inh2O,");
            sql.AppendLine("    ROUND(fuel_oil_gals, 4) fuel_oil_gals, ROUND(fuel_oil_tank_lvl,4) fuel_oil_tank_lvl, ROUND(gas_mscf,4) gas_mscf,");
            sql.AppendLine("    ROUND(grate_hrs,4) grate_hrs, ROUND(mbtu_lton,4) mbtu_lton, ROUND(mbtus,4) mbtus, ROUND(pel_ltons,4) pel_ltons,");
            sql.AppendLine("    ROUND(recl_in_ltons,4) recl_in_ltons, ROUND(recl_out_ltons) recl_out_ltons, ROUND(wood_stons, 4) wood_stons,");
            sql.AppendLine("    ROUND(recl_in_act_ltons,4) recl_in_act_ltons, ROUND(recl_out_act_ltons,4) recl_out_act_ltons,");
            sql.AppendLine("    ROUND(recl_in_adj_ltons,4) recl_in_adj_ltons, ROUND(recl_out_adj_ltons,4) recl_out_adj_ltons,");
            sql.AppendLine("    bin1_pct, bin2_pct");
            sql.AppendLine($"FROM Agg2_Shift a2s");
            return sql.ToString();
        }



        /// <summary>
        /// converts the datarow to an object
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private static Agg2_Shift DataRowToObj(DataRow row)
        {


            Agg2_Shift retObj = new();
            retObj.Agg2_Shift_Id = Convert.ToInt32(row["agg2_shift_id"]);
            retObj.Report_Date = row.Field<DateTime>("report_date");
            retObj.Shift = Convert.ToByte(row["shift"]);
            retObj.Lines_Running_Avg = row.Field<decimal?>("lines_running_avg");
            retObj.Lines_Running_Sum = row.Field<decimal?>("lines_running_sum");
            retObj.Tank_Flux_1_Pct = row.Field<decimal?>("tank_flux_1_pct");
            retObj.Tank_Flux_2_Pct = row.Field<decimal?>("tank_flux_2_pct");
            retObj.Tank_Slurry_1_1_Pct = row.Field<decimal?>("tank_slurry_1_1_pct");
            retObj.Tank_Slurry_1_2_Pct = row.Field<decimal?>("tank_slurry_1_2_pct");

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


            retObj.Bin1_Pct = row.Field<float?>("bin1_pct");
            retObj.Bin2_Pct = row.Field<float?>("bin2_pct");

            return retObj;
        }
    }
}
