using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Services
{
    public static class Met_Conc_Plant2Svc
    {
        static Met_Conc_Plant2Svc()
        {
            Util.RegisterOracle();
        }

        /// <summary>
        /// Gets the Met data for the specified date
        /// </summary>
        /// <param name="MetRptDate"></param>
        /// <returns></returns>
        public static Met_Conc_Plant2 Get(DateTime MetRptDate)
        {
            List<Met_Conc_Plant2> val = Get(MetRptDate, MetRptDate);
            if (val.Count > 0)
                return val[0];
            else
                return null; 
        }

        /// <summary>
        /// Gets the met data for the specified date range
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<Met_Conc_Plant2> Get(DateTime StartDate, DateTime EndDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE datex BETWEEN :StartDate AND :EndDate");
            sql.AppendLine("AND DMY = 1");
            sql.AppendLine("ORDER BY Datex");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add(":StartDate", StartDate);
            da.SelectCommand.Parameters.Add(":EndDate", EndDate);

            DataSet ds = MOO.Data.ExecuteQuery(da);

            List<Met_Conc_Plant2> retVal = new();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                retVal.Add(DataRowToObject(dr));
            }

            return retVal;
        }



        /// <summary>
        /// Gets the Month summary met data  
        /// </summary>
        /// <param name="RollDate"></param>
        /// <returns></returns>
        public static Met_Conc_Plant2 GetMonthSummary(DateTime RollDate)
        {
            StringBuilder sql = new();
            sql.AppendLine("DECLARE");
            sql.AppendLine("    RollDate Date;");
            sql.AppendLine("    mcp met_conc_plant2%rowtype;");
            sql.AppendLine("BEGIN");
            sql.AppendLine("    RollDate := :RollDate;");
            sql.AppendLine("    tolive.met_roll.met_conc_plant2_m(RollDate, mcp);");
            sql.AppendLine("    open :cursor FOR SELECT RollDate datex, 2 dmy, ");
            sql.AppendLine("            ROUND(mcp.sched_hours,4) sched_hours, ROUND(mcp.sched_maint_hours,4) sched_maint_hours, ");
            sql.AppendLine("            ROUND(mcp.unsched_maint_hours,4) unsched_maint_hours, ROUND(mcp.aggl_request_hours,4) aggl_request_hours, ");
            sql.AppendLine("            ROUND(mcp.hi_power_lim_hours,4) hi_power_lim_hours, ROUND(mcp.no_ore_hours,4) no_ore_hours, ");
            sql.AppendLine("            ROUND(mcp.rmf_h2o,4) rmf_h2o, ROUND(mcp.rmf_mag_fe,4) rmf_mag_fe, ROUND(mcp.flot_tail_sio2,4) flot_tail_sio2, ");
            sql.AppendLine("            ROUND(mcp.flot_dry_wt_rec,4) flot_dry_wt_rec, ROUND(mcp.flot_mag_fe_rec,4) flot_mag_fe_rec, ROUND(mcp.flot_conc_tons,4) flot_conc_tons,");
            sql.AppendLine("            ROUND(mcp.plant_34_inch,4) plant_34_inch, ROUND(mcp.plant_12_inch,4) plant_12_inch, ");
            sql.AppendLine("            ROUND(mcp.dav_tube_rec_flot,4) dav_tube_rec_flot, ROUND(mcp.dav_tube_sio2_flot,4) dav_tube_sio2_flot, ");
            sql.AppendLine("            ROUND(mcp.col_fd_sio2,4) col_fd_sio2, ROUND(mcp.col_froth_sio2,4) col_froth_sio2, ROUND(mcp.col_conc_sio2,4) col_conc_sio2, ");
            sql.AppendLine("            ROUND(mcp.col_conc_tph,4) col_conc_tph, ROUND(mcp.ln12_dredge_kwh,4) ln12_dredge_kwh, ROUND(mcp.step1_nola_sio2,4) step1_nola_sio2, ");
            sql.AppendLine("            ROUND(mcp.step2_nola_sio2,4) step2_nola_sio2, ROUND(mcp.step3_nola_sio2,4) step3_nola_sio2, ");
            sql.AppendLine("            ROUND(mcp.lab_flot_sio2,4) lab_flot_sio2, ROUND(mcp.defoamer,4) defoamer, ROUND(mcp.amine_lbs,4) amine_lbs, ");
            sql.AppendLine("            ROUND(mcp.frother_lbs,4) frother_lbs, ROUND(mcp.ln3_minus_200,4) ln3_minus_200, ROUND(mcp.flot_feed_tons,4) flot_feed_tons, ");
            sql.AppendLine("            ROUND(mcp.flot_tph,4) flot_tph, ROUND(mcp.flot_feed_sio2,4) flot_feed_sio2, ROUND(mcp.flot_minus_270,4) flot_minus_270, ");
            sql.AppendLine("            ROUND(mcp.bank1_conc_sio2,4) bank1_conc_sio2, ROUND(mcp.bank2_conc_sio2,4) bank2_conc_sio2, ");
            sql.AppendLine("            ROUND(mcp.bank3_conc_sio2,4) bank3_conc_sio2, ROUND(mcp.bank4_conc_sio2,4) bank4_conc_sio2, ");
            sql.AppendLine("            ROUND(mcp.regrind_froth_sio2,4) regrind_froth_sio2, ROUND(mcp.amine_lbs_hr,4) amine_lbs_hr, ROUND(mcp.frother_lbs_hr,4) frother_lbs_hr, ");
            sql.AppendLine("            ROUND(mcp.dav_tube_rm_sio2,4) dav_tube_rm_sio2, ROUND(mcp.conc_tons,4) conc_tons, ROUND(mcp.flot_bank_hrs,4) flot_bank_hrs");
            sql.AppendLine("FROM DUAL;");
            sql.AppendLine("END;");


            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.Parameters.Add("RollDate", RollDate);
            OracleParameter outTable = new OracleParameter("cursor", OracleDbType.RefCursor, ParameterDirection.Output);
            da.SelectCommand.Parameters.Add(outTable);
            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count > 0)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }

        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine("datex, dmy, ROUND(sched_hours,4) sched_hours, ROUND(sched_maint_hours,4) sched_maint_hours, ");
            sql.AppendLine("ROUND(unsched_maint_hours,4) unsched_maint_hours, ROUND(aggl_request_hours,4) aggl_request_hours, ");
            sql.AppendLine("ROUND(hi_power_lim_hours,4) hi_power_lim_hours, ROUND(no_ore_hours,4) no_ore_hours, ");
            sql.AppendLine("ROUND(rmf_h2o,4) rmf_h2o, ROUND(rmf_mag_fe,4) rmf_mag_fe, ROUND(flot_tail_sio2,4) flot_tail_sio2, ");
            sql.AppendLine("ROUND(flot_dry_wt_rec,4) flot_dry_wt_rec, ROUND(flot_mag_fe_rec,4) flot_mag_fe_rec, ROUND(flot_conc_tons,4) flot_conc_tons,");
            sql.AppendLine("ROUND(plant_34_inch,4) plant_34_inch, ROUND(plant_12_inch,4) plant_12_inch, ");
            sql.AppendLine("ROUND(dav_tube_rec_flot,4) dav_tube_rec_flot, ROUND(dav_tube_sio2_flot,4) dav_tube_sio2_flot, ");
            sql.AppendLine("ROUND(col_fd_sio2,4) col_fd_sio2, ROUND(col_froth_sio2,4) col_froth_sio2, ROUND(col_conc_sio2,4) col_conc_sio2, ");
            sql.AppendLine("ROUND(col_conc_tph,4) col_conc_tph, ROUND(ln12_dredge_kwh,4) ln12_dredge_kwh, ROUND(step1_nola_sio2,4) step1_nola_sio2, ");
            sql.AppendLine("ROUND(step2_nola_sio2,4) step2_nola_sio2, ROUND(step3_nola_sio2,4) step3_nola_sio2, ");
            sql.AppendLine("ROUND(lab_flot_sio2,4) lab_flot_sio2, ROUND(defoamer,4) defoamer, ROUND(amine_lbs,4) amine_lbs, ");
            sql.AppendLine("ROUND(frother_lbs,4) frother_lbs, ROUND(ln3_minus_200,4) ln3_minus_200, ROUND(flot_feed_tons,4) flot_feed_tons, ");
            sql.AppendLine("ROUND(flot_tph,4) flot_tph, ROUND(flot_feed_sio2,4) flot_feed_sio2, ROUND(flot_minus_270,4) flot_minus_270, ");
            sql.AppendLine("ROUND(bank1_conc_sio2,4) bank1_conc_sio2, ROUND(bank2_conc_sio2,4) bank2_conc_sio2, ");
            sql.AppendLine("ROUND(bank3_conc_sio2,4) bank3_conc_sio2, ROUND(bank4_conc_sio2,4) bank4_conc_sio2, ");
            sql.AppendLine("ROUND(regrind_froth_sio2,4) regrind_froth_sio2, ROUND(amine_lbs_hr,4) amine_lbs_hr, ROUND(frother_lbs_hr,4) frother_lbs_hr, ");
            sql.AppendLine("ROUND(dav_tube_rm_sio2,4) dav_tube_rm_sio2, ROUND(conc_tons,4) conc_tons, ROUND(flot_bank_hrs,4) flot_bank_hrs");

            sql.AppendLine("FROM tolive.met_conc_plant2");
            return sql.ToString();
        }



        public static int Update(Met_Conc_Plant2 obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Met_Conc_Plant2 obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Met_Conc_Plant2 SET");
            sql.AppendLine("sched_hours = :sched_hours, ");
            sql.AppendLine("sched_maint_hours = :sched_maint_hours, ");
            sql.AppendLine("unsched_maint_hours = :unsched_maint_hours, ");
            sql.AppendLine("aggl_request_hours = :aggl_request_hours, ");
            sql.AppendLine("hi_power_lim_hours = :hi_power_lim_hours, ");
            sql.AppendLine("no_ore_hours = :no_ore_hours, ");
            sql.AppendLine("rmf_h2o = :rmf_h2o, ");
            sql.AppendLine("rmf_mag_fe = :rmf_mag_fe, ");
            sql.AppendLine("flot_tail_sio2 = :flot_tail_sio2, ");
            sql.AppendLine("flot_dry_wt_rec = :flot_dry_wt_rec, ");
            sql.AppendLine("flot_mag_fe_rec = :flot_mag_fe_rec, ");
            sql.AppendLine("flot_conc_tons = :flot_conc_tons, ");
            sql.AppendLine("plant_34_inch = :plant_34_inch, ");
            sql.AppendLine("plant_12_inch = :plant_12_inch, ");
            sql.AppendLine("dav_tube_rec_flot = :dav_tube_rec_flot, ");
            sql.AppendLine("dav_tube_sio2_flot = :dav_tube_sio2_flot, ");
            sql.AppendLine("col_fd_sio2 = :col_fd_sio2, ");
            sql.AppendLine("col_froth_sio2 = :col_froth_sio2, ");
            sql.AppendLine("col_conc_sio2 = :col_conc_sio2, ");
            sql.AppendLine("col_conc_tph = :col_conc_tph, ");
            sql.AppendLine("ln12_dredge_kwh = :ln12_dredge_kwh, ");
            sql.AppendLine("step1_nola_sio2 = :step1_nola_sio2, ");
            sql.AppendLine("step2_nola_sio2 = :step2_nola_sio2, ");
            sql.AppendLine("step3_nola_sio2 = :step3_nola_sio2, ");
            sql.AppendLine("lab_flot_sio2 = :lab_flot_sio2, ");
            sql.AppendLine("defoamer = :defoamer, ");
            sql.AppendLine("amine_lbs = :amine_lbs, ");
            sql.AppendLine("frother_lbs = :frother_lbs, ");
            sql.AppendLine("ln3_minus_200 = :ln3_minus_200, ");
            sql.AppendLine("flot_feed_tons = :flot_feed_tons, ");
            sql.AppendLine("flot_tph = :flot_tph, ");
            sql.AppendLine("flot_feed_sio2 = :flot_feed_sio2, ");
            sql.AppendLine("flot_minus_270 = :flot_minus_270, ");
            sql.AppendLine("bank1_conc_sio2 = :bank1_conc_sio2, ");
            sql.AppendLine("bank2_conc_sio2 = :bank2_conc_sio2, ");
            sql.AppendLine("bank3_conc_sio2 = :bank3_conc_sio2, ");
            sql.AppendLine("bank4_conc_sio2 = :bank4_conc_sio2, ");
            sql.AppendLine("regrind_froth_sio2 = :regrind_froth_sio2, ");
            sql.AppendLine("amine_lbs_hr = :amine_lbs_hr, ");
            sql.AppendLine("frother_lbs_hr = :frother_lbs_hr, ");
            sql.AppendLine("dav_tube_rm_sio2 = :dav_tube_rm_sio2, ");
            sql.AppendLine("conc_tons = :conc_tons, ");
            sql.AppendLine("flot_bank_hrs = :flot_bank_hrs");
            sql.AppendLine("WHERE datex = :datex AND dmy = :dmy");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.Parameters.Add("sched_hours", obj.Sched_Hours);
            upd.Parameters.Add("sched_maint_hours", obj.Sched_Maint_Hours);
            upd.Parameters.Add("unsched_maint_hours", obj.Unsched_Maint_Hours);
            upd.Parameters.Add("aggl_request_hours", obj.Aggl_Request_Hours);
            upd.Parameters.Add("hi_power_lim_hours", obj.Hi_Power_Lim_Hours);
            upd.Parameters.Add("no_ore_hours", obj.No_Ore_Hours);
            upd.Parameters.Add("rmf_h2o", obj.Rmf_H2o);
            upd.Parameters.Add("rmf_mag_fe", obj.Rmf_Mag_Fe);
            upd.Parameters.Add("flot_tail_sio2", obj.Flot_Tail_Sio2);
            upd.Parameters.Add("flot_dry_wt_rec", obj.Flot_Dry_Wt_Rec);
            upd.Parameters.Add("flot_mag_fe_rec", obj.Flot_Mag_Fe_Rec);
            upd.Parameters.Add("flot_conc_tons", obj.Flot_Conc_Tons);
            upd.Parameters.Add("plant_34_inch", obj.Plant_34_Inch);
            upd.Parameters.Add("plant_12_inch", obj.Plant_12_Inch);
            upd.Parameters.Add("dav_tube_rec_flot", obj.Dav_Tube_Rec_Flot);
            upd.Parameters.Add("dav_tube_sio2_flot", obj.Dav_Tube_Sio2_Flot);
            upd.Parameters.Add("col_fd_sio2", obj.Col_Fd_Sio2);
            upd.Parameters.Add("col_froth_sio2", obj.Col_Froth_Sio2);
            upd.Parameters.Add("col_conc_sio2", obj.Col_Conc_Sio2);
            upd.Parameters.Add("col_conc_tph", obj.Col_Conc_Tph);
            upd.Parameters.Add("ln12_dredge_kwh", obj.Ln12_Dredge_Kwh);
            upd.Parameters.Add("step1_nola_sio2", obj.Step1_Nola_Sio2);
            upd.Parameters.Add("step2_nola_sio2", obj.Step2_Nola_Sio2);
            upd.Parameters.Add("step3_nola_sio2", obj.Step3_Nola_Sio2);
            upd.Parameters.Add("lab_flot_sio2", obj.Lab_Flot_Sio2);
            upd.Parameters.Add("defoamer", obj.Defoamer);
            upd.Parameters.Add("amine_lbs", obj.Amine_Lbs);
            upd.Parameters.Add("frother_lbs", obj.Frother_Lbs);
            upd.Parameters.Add("ln3_minus_200", obj.Ln3_Minus_200);
            upd.Parameters.Add("flot_feed_tons", obj.Flot_Feed_Tons);
            upd.Parameters.Add("flot_tph", obj.Flot_Tph);
            upd.Parameters.Add("flot_feed_sio2", obj.Flot_Feed_Sio2);
            upd.Parameters.Add("flot_minus_270", obj.Flot_Minus_270);
            upd.Parameters.Add("bank1_conc_sio2", obj.Bank1_Conc_Sio2);
            upd.Parameters.Add("bank2_conc_sio2", obj.Bank2_Conc_Sio2);
            upd.Parameters.Add("bank3_conc_sio2", obj.Bank3_Conc_Sio2);
            upd.Parameters.Add("bank4_conc_sio2", obj.Bank4_Conc_Sio2);
            upd.Parameters.Add("regrind_froth_sio2", obj.Regrind_Froth_Sio2);
            upd.Parameters.Add("amine_lbs_hr", obj.Amine_Lbs_Hr);
            upd.Parameters.Add("frother_lbs_hr", obj.Frother_Lbs_Hr);
            upd.Parameters.Add("dav_tube_rm_sio2", obj.Dav_Tube_Rm_Sio2);
            upd.Parameters.Add("conc_tons", obj.Conc_Tons);
            upd.Parameters.Add("flot_bank_hrs", obj.Flot_Bank_Hrs);

            //where clause
            upd.Parameters.Add("datex", obj.Datex);
            upd.Parameters.Add("dmy", (int)obj.Dmy);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }

        public static int Insert(Met_Conc_Plant2 obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Insert(Met_Conc_Plant2 obj, OracleConnection conn)
        {
            
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Met_Conc_Plant2(");
            sql.AppendLine("datex, dmy, sched_hours, sched_maint_hours, unsched_maint_hours, aggl_request_hours, ");
            sql.AppendLine("hi_power_lim_hours, no_ore_hours, rmf_h2o, rmf_mag_fe, flot_tail_sio2, ");
            sql.AppendLine("flot_dry_wt_rec, flot_mag_fe_rec, flot_conc_tons, plant_34_inch, plant_12_inch, ");
            sql.AppendLine("dav_tube_rec_flot, dav_tube_sio2_flot, col_fd_sio2, col_froth_sio2, col_conc_sio2, ");
            sql.AppendLine("col_conc_tph, ln12_dredge_kwh, step1_nola_sio2, step2_nola_sio2, step3_nola_sio2, ");
            sql.AppendLine("lab_flot_sio2, defoamer, amine_lbs, frother_lbs, ln3_minus_200, flot_feed_tons, ");
            sql.AppendLine("flot_tph, flot_feed_sio2, flot_minus_270, bank1_conc_sio2, bank2_conc_sio2, ");
            sql.AppendLine("bank3_conc_sio2, bank4_conc_sio2, regrind_froth_sio2, amine_lbs_hr, frother_lbs_hr, ");
            sql.AppendLine("dav_tube_rm_sio2, conc_tons, flot_bank_hrs)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":datex, :dmy, :sched_hours, :sched_maint_hours, :unsched_maint_hours, ");
            sql.AppendLine(":aggl_request_hours, :hi_power_lim_hours, :no_ore_hours, :rmf_h2o, :rmf_mag_fe, ");
            sql.AppendLine(":flot_tail_sio2, :flot_dry_wt_rec, :flot_mag_fe_rec, :flot_conc_tons, :plant_34_inch, ");
            sql.AppendLine(":plant_12_inch, :dav_tube_rec_flot, :dav_tube_sio2_flot, :col_fd_sio2, ");
            sql.AppendLine(":col_froth_sio2, :col_conc_sio2, :col_conc_tph, :ln12_dredge_kwh, :step1_nola_sio2, ");
            sql.AppendLine(":step2_nola_sio2, :step3_nola_sio2, :lab_flot_sio2, :defoamer, :amine_lbs, ");
            sql.AppendLine(":frother_lbs, :ln3_minus_200, :flot_feed_tons, :flot_tph, :flot_feed_sio2, ");
            sql.AppendLine(":flot_minus_270, :bank1_conc_sio2, :bank2_conc_sio2, :bank3_conc_sio2, ");
            sql.AppendLine(":bank4_conc_sio2, :regrind_froth_sio2, :amine_lbs_hr, :frother_lbs_hr, ");
            sql.AppendLine(":dav_tube_rm_sio2, :conc_tons, :flot_bank_hrs)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("datex", obj.Datex);
            ins.Parameters.Add("dmy", (int)obj.Dmy);
            ins.Parameters.Add("sched_hours", obj.Sched_Hours);
            ins.Parameters.Add("sched_maint_hours", obj.Sched_Maint_Hours);
            ins.Parameters.Add("unsched_maint_hours", obj.Unsched_Maint_Hours);
            ins.Parameters.Add("aggl_request_hours", obj.Aggl_Request_Hours);
            ins.Parameters.Add("hi_power_lim_hours", obj.Hi_Power_Lim_Hours);
            ins.Parameters.Add("no_ore_hours", obj.No_Ore_Hours);
            ins.Parameters.Add("rmf_h2o", obj.Rmf_H2o);
            ins.Parameters.Add("rmf_mag_fe", obj.Rmf_Mag_Fe);
            ins.Parameters.Add("flot_tail_sio2", obj.Flot_Tail_Sio2);
            ins.Parameters.Add("flot_dry_wt_rec", obj.Flot_Dry_Wt_Rec);
            ins.Parameters.Add("flot_mag_fe_rec", obj.Flot_Mag_Fe_Rec);
            ins.Parameters.Add("flot_conc_tons", obj.Flot_Conc_Tons);
            ins.Parameters.Add("plant_34_inch", obj.Plant_34_Inch);
            ins.Parameters.Add("plant_12_inch", obj.Plant_12_Inch);
            ins.Parameters.Add("dav_tube_rec_flot", obj.Dav_Tube_Rec_Flot);
            ins.Parameters.Add("dav_tube_sio2_flot", obj.Dav_Tube_Sio2_Flot);
            ins.Parameters.Add("col_fd_sio2", obj.Col_Fd_Sio2);
            ins.Parameters.Add("col_froth_sio2", obj.Col_Froth_Sio2);
            ins.Parameters.Add("col_conc_sio2", obj.Col_Conc_Sio2);
            ins.Parameters.Add("col_conc_tph", obj.Col_Conc_Tph);
            ins.Parameters.Add("ln12_dredge_kwh", obj.Ln12_Dredge_Kwh);
            ins.Parameters.Add("step1_nola_sio2", obj.Step1_Nola_Sio2);
            ins.Parameters.Add("step2_nola_sio2", obj.Step2_Nola_Sio2);
            ins.Parameters.Add("step3_nola_sio2", obj.Step3_Nola_Sio2);
            ins.Parameters.Add("lab_flot_sio2", obj.Lab_Flot_Sio2);
            ins.Parameters.Add("defoamer", obj.Defoamer);
            ins.Parameters.Add("amine_lbs", obj.Amine_Lbs);
            ins.Parameters.Add("frother_lbs", obj.Frother_Lbs);
            ins.Parameters.Add("ln3_minus_200", obj.Ln3_Minus_200);
            ins.Parameters.Add("flot_feed_tons", obj.Flot_Feed_Tons);
            ins.Parameters.Add("flot_tph", obj.Flot_Tph);
            ins.Parameters.Add("flot_feed_sio2", obj.Flot_Feed_Sio2);
            ins.Parameters.Add("flot_minus_270", obj.Flot_Minus_270);
            ins.Parameters.Add("bank1_conc_sio2", obj.Bank1_Conc_Sio2);
            ins.Parameters.Add("bank2_conc_sio2", obj.Bank2_Conc_Sio2);
            ins.Parameters.Add("bank3_conc_sio2", obj.Bank3_Conc_Sio2);
            ins.Parameters.Add("bank4_conc_sio2", obj.Bank4_Conc_Sio2);
            ins.Parameters.Add("regrind_froth_sio2", obj.Regrind_Froth_Sio2);
            ins.Parameters.Add("amine_lbs_hr", obj.Amine_Lbs_Hr);
            ins.Parameters.Add("frother_lbs_hr", obj.Frother_Lbs_Hr);
            ins.Parameters.Add("dav_tube_rm_sio2", obj.Dav_Tube_Rm_Sio2);
            ins.Parameters.Add("conc_tons", obj.Conc_Tons);
            ins.Parameters.Add("flot_bank_hrs", obj.Flot_Bank_Hrs);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }



        private static Met_Conc_Plant2 DataRowToObject(DataRow row)
        {
            Met_Conc_Plant2 RetVal = new();
            RetVal.Datex = row.Field<DateTime>("datex");
            RetVal.Dmy = (Met_DMY)row.Field<decimal>("dmy");
            RetVal.Sched_Hours = row.Field<decimal?>("sched_hours");
            RetVal.Sched_Maint_Hours = row.Field<decimal?>("sched_maint_hours");
            RetVal.Unsched_Maint_Hours = row.Field<decimal?>("unsched_maint_hours");
            RetVal.Aggl_Request_Hours = row.Field<decimal?>("aggl_request_hours");
            RetVal.Hi_Power_Lim_Hours = row.Field<decimal?>("hi_power_lim_hours");
            RetVal.No_Ore_Hours = row.Field<decimal?>("no_ore_hours");
            RetVal.Rmf_H2o = row.Field<decimal?>("rmf_h2o");
            RetVal.Rmf_Mag_Fe = row.Field<decimal?>("rmf_mag_fe");
            RetVal.Flot_Tail_Sio2 = row.Field<decimal?>("flot_tail_sio2");
            RetVal.Flot_Dry_Wt_Rec = row.Field<decimal?>("flot_dry_wt_rec");
            RetVal.Flot_Mag_Fe_Rec = row.Field<decimal?>("flot_mag_fe_rec");
            RetVal.Flot_Conc_Tons = row.Field<decimal?>("flot_conc_tons");
            RetVal.Plant_34_Inch = row.Field<decimal?>("plant_34_inch");
            RetVal.Plant_12_Inch = row.Field<decimal?>("plant_12_inch");
            RetVal.Dav_Tube_Rec_Flot = row.Field<decimal?>("dav_tube_rec_flot");
            RetVal.Dav_Tube_Sio2_Flot = row.Field<decimal?>("dav_tube_sio2_flot");
            RetVal.Col_Fd_Sio2 = row.Field<decimal?>("col_fd_sio2");
            RetVal.Col_Froth_Sio2 = row.Field<decimal?>("col_froth_sio2");
            RetVal.Col_Conc_Sio2 = row.Field<decimal?>("col_conc_sio2");
            RetVal.Col_Conc_Tph = row.Field<decimal?>("col_conc_tph");
            RetVal.Ln12_Dredge_Kwh = row.Field<decimal?>("ln12_dredge_kwh");
            RetVal.Step1_Nola_Sio2 = row.Field<decimal?>("step1_nola_sio2");
            RetVal.Step2_Nola_Sio2 = row.Field<decimal?>("step2_nola_sio2");
            RetVal.Step3_Nola_Sio2 = row.Field<decimal?>("step3_nola_sio2");
            RetVal.Lab_Flot_Sio2 = row.Field<decimal?>("lab_flot_sio2");
            RetVal.Defoamer = row.Field<decimal?>("defoamer");
            RetVal.Amine_Lbs = row.Field<decimal?>("amine_lbs");
            RetVal.Frother_Lbs = row.Field<decimal?>("frother_lbs");
            RetVal.Ln3_Minus_200 = row.Field<decimal?>("ln3_minus_200");
            RetVal.Flot_Feed_Tons = row.Field<decimal?>("flot_feed_tons");
            RetVal.Flot_Tph = row.Field<decimal?>("flot_tph");
            RetVal.Flot_Feed_Sio2 = row.Field<decimal?>("flot_feed_sio2");
            RetVal.Flot_Minus_270 = row.Field<decimal?>("flot_minus_270");
            RetVal.Bank1_Conc_Sio2 = row.Field<decimal?>("bank1_conc_sio2");
            RetVal.Bank2_Conc_Sio2 = row.Field<decimal?>("bank2_conc_sio2");
            RetVal.Bank3_Conc_Sio2 = row.Field<decimal?>("bank3_conc_sio2");
            RetVal.Bank4_Conc_Sio2 = row.Field<decimal?>("bank4_conc_sio2");
            RetVal.Regrind_Froth_Sio2 = row.Field<decimal?>("regrind_froth_sio2");
            RetVal.Amine_Lbs_Hr = row.Field<decimal?>("amine_lbs_hr");
            RetVal.Frother_Lbs_Hr = row.Field<decimal?>("frother_lbs_hr");
            RetVal.Dav_Tube_Rm_Sio2 = row.Field<decimal?>("dav_tube_rm_sio2");
            RetVal.Conc_Tons = row.Field<decimal?>("conc_tons");
            RetVal.Flot_Bank_Hrs = row.Field<decimal?>("flot_bank_hrs");
            return RetVal;
        }

    }
}
