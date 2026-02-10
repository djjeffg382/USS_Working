using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;

namespace MOO.DAL.ToLive.Services
{
    public static class Agg3_Shift_LineSvc
    {

        static Agg3_Shift_LineSvc()
        {
            Util.RegisterOracle();
        }
        /// <summary>
        /// Gets the agg shift 2 record by the agg shift id
        /// </summary>
        /// <param name="aggShiftID"></param>
        /// <returns></returns>
        public static Agg3_Shift_Line Get(int aggShiftID, byte line)
        {
            if (line < 6 || line > 7)
                throw new Exception($"Invalid line number given for Agg3_Shift_line - {line}");

            StringBuilder sql = new();

            sql.AppendLine(GetSelect());
            sql.AppendLine($"WHERE asl.agg3_shift_id = {aggShiftID}");
            sql.AppendLine($"AND asl.line = {line}");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count == 0)
                return null;
            else
                return DataRowToObj(ds.Tables[0].Rows[0]);
        }

        /// <summary>
        /// Gets the agg2 shift line record by date and shift
        /// </summary>
        /// <param name="reportDate"></param>
        /// <param name="shift"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public static Agg3_Shift_Line Get(DateTime reportDate, byte shift, byte line)
        {
            if (line < 6 || line > 7)
                throw new Exception($"Invalid line number given for Agg3_Shift_line - {line}");
            StringBuilder sql = new();

            sql.AppendLine(GetSelect());
            sql.AppendLine($"WHERE a3s.report_date = {MOO.Dates.OraDate(reportDate)}");
            sql.AppendLine($"AND a3s.shift = {shift}");
            sql.AppendLine($"AND asl.line = {line}");
            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count == 0)
                return null;
            else
                return DataRowToObj(ds.Tables[0].Rows[0]);
        }
        /// <summary>
        /// Updates the given shift input record for shift input app (only updates pel tons, bin pct, and general_desc
        /// </summary>
        /// <param name="ag3LineRecord"></param>
        /// <returns></returns>
        public static int UpdateFromShiftInput(Agg3_Shift_Line ag3LineRecord)
        {
            int recsUpdated = 0;
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                recsUpdated = UpdateFromShiftInput(conn, ag3LineRecord);
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
        /// Updates the given shift input record for shift input app (only updates pel tons, bin pct, and general_desc)
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="ag3LineRecord"></param>
        /// <returns></returns>
        public static int UpdateFromShiftInput(OracleConnection conn, Agg3_Shift_Line ag3LineRecord)
        {
           

            StringBuilder sql = new();
            sql.AppendLine($"UPDATE tolive.agg3_shift_line");
            sql.AppendLine("SET pel_adj_ltons = :pel_adj_ltons,");  //set it null if it is zero
            sql.AppendLine("pel_ltons = :pel_ltons,");
            sql.AppendLine("bin_pct = :bin_pct,");
            sql.AppendLine("general_desc = :general_desc");
            sql.AppendLine($"WHERE agg3_shift_id = {ag3LineRecord.Agg3_Shift_Id}");
            sql.AppendLine($"AND line = {ag3LineRecord.Line}");

            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("pel_adj_ltons", ag3LineRecord.Pel_Adj_Ltons == 0 ? null : ag3LineRecord.Pel_Adj_Ltons);  //put null if value is zero
            cmd.Parameters.Add("pel_ltons", ag3LineRecord.Pel_Ltons);
            cmd.Parameters.Add("bin_pct", ag3LineRecord.Bin_Pct);
            cmd.Parameters.Add("general_desc", ag3LineRecord.General_Desc);

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
            sql.AppendLine("SELECT asl.agg3_shift_id, asl.line, asl.status_on, ROUND(asl.ball_hrs,4) ball_hrs, ROUND(asl.bent_lbs,4) bent_lbs, ");
            sql.AppendLine("    ROUND(asl.bent_lbs_lton,4) bent_lbs_lton, ROUND(asl.coal_stons,4) coal_stons,");
            sql.AppendLine("    ROUND(asl.dd1_inh2o,4) dd1_inh2o, ROUND(asl.fuel_oil_gals,4) fuel_oil_gals, ROUND(asl.gas_mscf,4) gas_mscf, ");
            sql.AppendLine("    ROUND(asl.grate_hrs,4) grate_hrs, ROUND(asl.mbtu_lton,4) mbtu_lton, ROUND(asl.mbtus,4) mbtus, ROUND(asl.pel_ltons,4) pel_ltons,");
            sql.AppendLine("    ROUND(asl.wood_stons,4) wood_stons, asl.general_desc, ROUND(asl.at_14,4) at_14, ROUND(asl.bed_depth_in,4) bed_depth_in,");
            sql.AppendLine("    ROUND(asl.belt_037_ltph,4) belt_037_ltph, ROUND(asl.bin_pct,4) bin_pct, ");
            sql.AppendLine("    ROUND(asl.bt_14,4) bt_14, ROUND(asl.burn_zone_temp,4) burn_zone_temp, ROUND(asl.comp600,4) comp600, ");
            sql.AppendLine("    ROUND(asl.dd1_temp,4) dd1_temp, ROUND(asl.dd2_temp,4) dd2_temp, ROUND(asl.feed_grate_ltph,4) feed_grate_ltph,");
            sql.AppendLine("    ROUND(asl.grate_spd_IPM,4) grate_spd_IPM, ROUND(asl.kiln_exit_temp,4) kiln_exit_temp, ");
            sql.AppendLine("    ROUND(asl.comp600_m200,4) comp600_m200, ROUND(asl.pel_act_ltons,4) pel_act_ltons, ");
            sql.AppendLine("    ROUND(asl.pel_adj_ltons,4) pel_adj_ltons, ROUND(asl.u_grate_temp,4) u_grate_temp,a3s.report_date, a3s.shift");
            sql.AppendLine($"FROM tolive.agg3_shift_line asl");
            sql.AppendLine($"INNER JOIN agg3_shift a3s");
            sql.AppendLine($"ON a3s.agg3_shift_id = asl.agg3_shift_id");
            return sql.ToString();
        }

        /// <summary>
        /// converts the datarow to an object
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private static Agg3_Shift_Line DataRowToObj(DataRow row)
        {
            
            
            Agg3_Shift_Line retObj = new();
            retObj.Agg3_Shift_Id = Convert.ToInt32(row["agg3_shift_id"]);
            retObj.Line = Convert.ToByte(row["line"]);
            retObj.Report_Date = row.Field<DateTime>("report_date");
            retObj.Shift = Convert.ToByte(row["shift"]);
            retObj.Status_On = row.Field<decimal?>("status_on");
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
            retObj.Wood_Stons = row.Field<decimal?>("wood_stons");
            retObj.General_Desc = row["general_desc"].ToString() ;
            retObj.At_14 = row.Field<decimal?>("at_14");
            retObj.Bed_Depth_In = row.Field<decimal?>("bed_depth_in");
            retObj.Belt_037_LTPH = row.Field<decimal?>("belt_037_ltph");
            retObj.Bin_Pct = row.Field<decimal?>("bin_pct");
            retObj.Bt_14 = row.Field<decimal?>("bt_14");
            retObj.Burn_Zone_Temp = row.Field<decimal?>("burn_zone_temp");
            retObj.Comp600 = row.Field<decimal?>("comp600");
            retObj.DD1_Temp = row.Field<decimal?>("dd1_temp");
            retObj.DD2_Temp = row.Field<decimal?>("dd2_temp");
            retObj.Feed_Grate_LTPH = row.Field<decimal?>("feed_grate_ltph");
            retObj.Grate_Spd_IPM = row.Field<decimal?>("grate_spd_IPM");
            retObj.Kiln_Exit_Temp = row.Field<decimal?>("kiln_exit_temp");
            retObj.Comp600_M200 = row.Field<decimal?>("comp600_m200");
            retObj.Pel_Act_Ltons = row.Field<decimal?>("pel_act_ltons");
            retObj.Pel_Ltons = row.Field<decimal?>("pel_ltons");
            retObj.Pel_Adj_Ltons = row.Field<decimal?>("pel_adj_ltons");
            retObj.U_Grate_Temp = row.Field<decimal?>("u_grate_temp");

            
            return retObj;
        }

    }
}
