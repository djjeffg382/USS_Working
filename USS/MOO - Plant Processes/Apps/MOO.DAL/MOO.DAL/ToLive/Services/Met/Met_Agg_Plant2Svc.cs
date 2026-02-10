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
    public static class Met_Agg_Plant2Svc
    {
        static Met_Agg_Plant2Svc()
        {
            Util.RegisterOracle();
        }


        /// <summary>
        /// Gets the Met data for the specified date
        /// </summary>
        /// <param name="MetRptDate"></param>
        /// <returns></returns>
        public static Met_Agg_Plant2 Get(DateTime MetRptDate, Met_Material Material)
        {
            List<Met_Agg_Plant2> val = Get(MetRptDate, MetRptDate, Material);
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
        public static List<Met_Agg_Plant2> Get(DateTime StartDate, DateTime EndDate, Met_Material Material)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE datex BETWEEN :StartDate AND :EndDate");
            sql.AppendLine("AND Material = :Material");
            sql.AppendLine("AND DMY = 1");
            sql.AppendLine("ORDER BY Datex");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add(":StartDate", StartDate);
            da.SelectCommand.Parameters.Add(":EndDate", EndDate);
            da.SelectCommand.Parameters.Add(":Material", (int)Material);

            DataSet ds = MOO.Data.ExecuteQuery(da);

            List<Met_Agg_Plant2> retVal = new();
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
        public static Met_Agg_Plant2 GetMonthSummary(DateTime RollDate, Met_Material Material)
        {
            StringBuilder sql = new();
            sql.AppendLine("DECLARE");
            sql.AppendLine("    RollDate Date;");
            sql.AppendLine("    ptype Number;");
            sql.AppendLine("    map met_agg_plant2%rowtype;");
            sql.AppendLine("BEGIN");
            sql.AppendLine("    RollDate := :RollDate;");
            sql.AppendLine("    ptype := :PelletType;");
            sql.AppendLine("    tolive.met_roll.met_agg_plant2_m(RollDate, ptype, map);");
            sql.AppendLine("    open :cursor FOR SELECT RollDate datex, 2 dmy, ptype Material,");

            sql.AppendLine("        ROUND(map.sched_hours,4) sched_hours, ROUND(map.sched_maint_hours,4) sched_maint_hours, ROUND(map.unsched_maint_hours,4) unsched_maint_hours, ");
            sql.AppendLine("        ROUND(map.imposed_hours,4) imposed_hours, ROUND(map.east_to_stockpile,4) east_to_stockpile, ROUND(map.east_to_plant,4) east_to_plant, ");
            sql.AppendLine("        ROUND(map.west_to_stockpile,4) west_to_stockpile, ROUND(map.west_to_plant,4) west_to_plant, ");
            sql.AppendLine("        ROUND(map.truck_to_west,4) truck_to_west, ROUND(map.truck_from_west,4) truck_from_west, ROUND(map.truck_to_east,4) truck_to_east, ");
            sql.AppendLine("        ROUND(map.truck_from_east,4) truck_from_east, ROUND(map.bin1,4) bin1, ROUND(map.bin2,4) bin2, ROUND(map.bin3,4) bin3, ");
            sql.AppendLine("        ROUND(map.bin4,4) bin4, ROUND(map.bin5,4) bin5, ROUND(map.tank1,4) tank1, ROUND(map.tank2,4) tank2, ROUND(map.tank3,4) tank3, ROUND(map.tank4,4) tank4, ");
            sql.AppendLine("        ROUND(map.grp13_fines,4) grp13_fines, ROUND(map.filt_hrs_1_1,4) filt_hrs_1_1, ROUND(map.filt_hrs_1_2,4) filt_hrs_1_2, ");
            sql.AppendLine("        ROUND(map.filt_hrs_2_1,4) filt_hrs_2_1, ROUND(map.filt_hrs_2_2,4) filt_hrs_2_2, ROUND(map.filt_hrs_4_1,4) filt_hrs_4_1,");
            sql.AppendLine("        ROUND(map.filt_hrs_4_2, 4) filt_hrs_4_2, ROUND(map.filt_hrs_4_3,4) filt_hrs_4_3, ROUND(map.filt_hrs_4_4,4) filt_hrs_4_4, ");
            sql.AppendLine("        ROUND(map.filt_hrs_4_5,4) filt_hrs_4_5, ROUND(map.filt_hrs_5_1,4) filt_hrs_5_1, ROUND(map.filt_hrs_5_2,4) filt_hrs_5_2, ");
            sql.AppendLine("        ROUND(map.filt_hrs_5_3,4) filt_hrs_5_3, ROUND(map.filt_hrs_5_4,4) filt_hrs_5_4, ROUND(map.filt_hrs_5_5,4) filt_hrs_5_5, ");
            sql.AppendLine("        ROUND(map.filt_tons_1,4) filt_tons_1, ROUND(map.filt_tons_2,4) filt_tons_2, ROUND(map.filtsi,4) filtsi, ");
            sql.AppendLine("        ROUND(map.filtal,4) filtal, ROUND(map.filtca,4) filtca, ROUND(map.filtmg,4) filtmg,  ");
            sql.AppendLine("        ROUND(map.filtmn,4) filtmn, ROUND(map.filt270,4) filt270, ROUND(map.filt500,4) filt500, ");
            sql.AppendLine("        ROUND(map.filth2o,4) filth2o, ROUND(map.reclaimh2o,4) reclaimh2o, ROUND(map.recl_bal_w,4) recl_bal_w,");
            sql.AppendLine("        ROUND(map.recl_bal_e,4) recl_bal_e, ROUND(map.pellet_bal,4) pellet_bal, ROUND(map.trucked_recl_bal,4) trucked_recl_bal");

            sql.AppendLine("FROM DUAL;");
            sql.AppendLine("END;");


            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.Parameters.Add("RollDate", RollDate);
            da.SelectCommand.Parameters.Add("PelletType", (int)Material);
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
            sql.AppendLine("material, datex, dmy, ");
            sql.AppendLine("ROUND(sched_hours,4) sched_hours, ROUND(sched_maint_hours,4) sched_maint_hours, ROUND(unsched_maint_hours,4) unsched_maint_hours, ");
            sql.AppendLine("ROUND(imposed_hours,4) imposed_hours, ROUND(east_to_stockpile,4) east_to_stockpile, ROUND(east_to_plant,4) east_to_plant, ");
            sql.AppendLine("ROUND(west_to_stockpile,4) west_to_stockpile, ROUND(west_to_plant,4) west_to_plant, ");
            sql.AppendLine("ROUND(truck_to_west,4) truck_to_west, ROUND(truck_from_west,4) truck_from_west, ROUND(truck_to_east,4) truck_to_east, ");
            sql.AppendLine("ROUND(truck_from_east,4) truck_from_east, ROUND(bin1,4) bin1, ROUND(bin2,4) bin2, ROUND(bin3,4) bin3, ");
            sql.AppendLine("ROUND(bin4,4) bin4, ROUND(bin5,4) bin5, ROUND(tank1,4) tank1, ROUND(tank2,4) tank2, ROUND(tank3,4) tank3, ROUND(tank4,4) tank4, ");
            sql.AppendLine("ROUND(grp13_fines,4) grp13_fines, ROUND(filt_hrs_1_1,4) filt_hrs_1_1, ROUND(filt_hrs_1_2,4) filt_hrs_1_2, ");
            sql.AppendLine("ROUND(filt_hrs_2_1,4) filt_hrs_2_1, ROUND(filt_hrs_2_2,4) filt_hrs_2_2, ROUND(filt_hrs_4_1,4) filt_hrs_4_1,");
            sql.AppendLine("ROUND(filt_hrs_4_2, 4) filt_hrs_4_2, ROUND(filt_hrs_4_3,4) filt_hrs_4_3, ROUND(filt_hrs_4_4,4) filt_hrs_4_4, ");
            sql.AppendLine("ROUND(filt_hrs_4_5,4) filt_hrs_4_5, ROUND(filt_hrs_5_1,4) filt_hrs_5_1, ROUND(filt_hrs_5_2,4) filt_hrs_5_2, ");
            sql.AppendLine("ROUND(filt_hrs_5_3,4) filt_hrs_5_3, ROUND(filt_hrs_5_4,4) filt_hrs_5_4, ROUND(filt_hrs_5_5,4) filt_hrs_5_5, ");
            sql.AppendLine("ROUND(filt_tons_1,4) filt_tons_1, ROUND(filt_tons_2,4) filt_tons_2, ROUND(filtsi,4) filtsi, ");
            sql.AppendLine("ROUND(filtal,4) filtal, ROUND(filtca,4) filtca, ROUND(filtmg,4) filtmg,  ");
            sql.AppendLine("ROUND(filtmn,4) filtmn, ROUND(filt270,4) filt270, ROUND(filt500,4) filt500, ");
            sql.AppendLine("ROUND(filth2o,4) filth2o, ROUND(reclaimh2o,4) reclaimh2o, ROUND(recl_bal_w,4) recl_bal_w,");
            sql.AppendLine("ROUND(recl_bal_e,4) recl_bal_e, ROUND(pellet_bal,4) pellet_bal, ROUND(trucked_recl_bal,4) trucked_recl_bal");
            sql.AppendLine("FROM tolive.met_agg_plant2");
            return sql.ToString();
        }


        public static int Insert(Met_Agg_Plant2 obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Met_Agg_Plant2 obj, OracleConnection conn)
        {
            
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Met_Agg_Plant2(");
            sql.AppendLine("material, datex, dmy, sched_hours, sched_maint_hours, unsched_maint_hours, ");
            sql.AppendLine("imposed_hours, east_to_stockpile, east_to_plant, west_to_stockpile, west_to_plant, ");
            sql.AppendLine("truck_to_west, truck_from_west, truck_to_east, truck_from_east, bin1, bin2, bin3, ");
            sql.AppendLine("bin4, bin5, tank1, tank2, tank3, tank4, grp13_fines, filt_hrs_1_1, filt_hrs_1_2, ");
            sql.AppendLine("filt_hrs_2_1, filt_hrs_2_2, filt_hrs_4_1, filt_hrs_4_2, filt_hrs_4_3, filt_hrs_4_4, ");
            sql.AppendLine("filt_hrs_4_5, filt_hrs_5_1, filt_hrs_5_2, filt_hrs_5_3, filt_hrs_5_4, filt_hrs_5_5, ");
            sql.AppendLine("filt_tons_1, filt_tons_2, filtsi, filtal, filtca, filtmg, filtmn, filt270, filt500, ");
            sql.AppendLine("filth2o, reclaimh2o, recl_bal_w, recl_bal_e, pellet_bal, trucked_recl_bal)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":material, :datex, :dmy, :sched_hours, :sched_maint_hours, :unsched_maint_hours, ");
            sql.AppendLine(":imposed_hours, :east_to_stockpile, :east_to_plant, :west_to_stockpile, ");
            sql.AppendLine(":west_to_plant, :truck_to_west, :truck_from_west, :truck_to_east, :truck_from_east, ");
            sql.AppendLine(":bin1, :bin2, :bin3, :bin4, :bin5, :tank1, :tank2, :tank3, :tank4, :grp13_fines, ");
            sql.AppendLine(":filt_hrs_1_1, :filt_hrs_1_2, :filt_hrs_2_1, :filt_hrs_2_2, :filt_hrs_4_1, ");
            sql.AppendLine(":filt_hrs_4_2, :filt_hrs_4_3, :filt_hrs_4_4, :filt_hrs_4_5, :filt_hrs_5_1, ");
            sql.AppendLine(":filt_hrs_5_2, :filt_hrs_5_3, :filt_hrs_5_4, :filt_hrs_5_5, :filt_tons_1, ");
            sql.AppendLine(":filt_tons_2, :filtsi, :filtal, :filtca, :filtmg, :filtmn, :filt270, :filt500, ");
            sql.AppendLine(":filth2o, :reclaimh2o, :recl_bal_w, :recl_bal_e, :pellet_bal, :trucked_recl_bal)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("material", (int)obj.Material);
            ins.Parameters.Add("datex", obj.Datex);
            ins.Parameters.Add("dmy", (int)obj.Dmy);
            ins.Parameters.Add("sched_hours", obj.Sched_Hours);
            ins.Parameters.Add("sched_maint_hours", obj.Sched_Maint_Hours);
            ins.Parameters.Add("unsched_maint_hours", obj.Unsched_Maint_Hours);
            ins.Parameters.Add("imposed_hours", obj.Imposed_Hours);
            ins.Parameters.Add("east_to_stockpile", obj.East_To_Stockpile);
            ins.Parameters.Add("east_to_plant", obj.East_To_Plant);
            ins.Parameters.Add("west_to_stockpile", obj.West_To_Stockpile);
            ins.Parameters.Add("west_to_plant", obj.West_To_Plant);
            ins.Parameters.Add("truck_to_west", obj.Truck_To_West);
            ins.Parameters.Add("truck_from_west", obj.Truck_From_West);
            ins.Parameters.Add("truck_to_east", obj.Truck_To_East);
            ins.Parameters.Add("truck_from_east", obj.Truck_From_East);
            ins.Parameters.Add("bin1", obj.Bin1);
            ins.Parameters.Add("bin2", obj.Bin2);
            ins.Parameters.Add("bin3", obj.Bin3);
            ins.Parameters.Add("bin4", obj.Bin4);
            ins.Parameters.Add("bin5", obj.Bin5);
            ins.Parameters.Add("tank1", obj.Tank1);
            ins.Parameters.Add("tank2", obj.Tank2);
            ins.Parameters.Add("tank3", obj.Tank3);
            ins.Parameters.Add("tank4", obj.Tank4);
            ins.Parameters.Add("grp13_fines", obj.Grp13_Fines);
            ins.Parameters.Add("filt_hrs_1_1", obj.Filt_Hrs_1_1);
            ins.Parameters.Add("filt_hrs_1_2", obj.Filt_Hrs_1_2);
            ins.Parameters.Add("filt_hrs_2_1", obj.Filt_Hrs_2_1);
            ins.Parameters.Add("filt_hrs_2_2", obj.Filt_Hrs_2_2);
            ins.Parameters.Add("filt_hrs_4_1", obj.Filt_Hrs_4_1);
            ins.Parameters.Add("filt_hrs_4_2", obj.Filt_Hrs_4_2);
            ins.Parameters.Add("filt_hrs_4_3", obj.Filt_Hrs_4_3);
            ins.Parameters.Add("filt_hrs_4_4", obj.Filt_Hrs_4_4);
            ins.Parameters.Add("filt_hrs_4_5", obj.Filt_Hrs_4_5);
            ins.Parameters.Add("filt_hrs_5_1", obj.Filt_Hrs_5_1);
            ins.Parameters.Add("filt_hrs_5_2", obj.Filt_Hrs_5_2);
            ins.Parameters.Add("filt_hrs_5_3", obj.Filt_Hrs_5_3);
            ins.Parameters.Add("filt_hrs_5_4", obj.Filt_Hrs_5_4);
            ins.Parameters.Add("filt_hrs_5_5", obj.Filt_Hrs_5_5);
            ins.Parameters.Add("filt_tons_1", obj.Filt_Tons_1);
            ins.Parameters.Add("filt_tons_2", obj.Filt_Tons_2);
            ins.Parameters.Add("filtsi", obj.Filtsi);
            ins.Parameters.Add("filtal", obj.Filtal);
            ins.Parameters.Add("filtca", obj.Filtca);
            ins.Parameters.Add("filtmg", obj.Filtmg);
            ins.Parameters.Add("filtmn", obj.Filtmn);
            ins.Parameters.Add("filt270", obj.Filt270);
            ins.Parameters.Add("filt500", obj.Filt500);
            ins.Parameters.Add("filth2o", obj.Filth2o);
            ins.Parameters.Add("reclaimh2o", obj.Reclaimh2o);
            ins.Parameters.Add("recl_bal_w", obj.Recl_Bal_W);
            ins.Parameters.Add("recl_bal_e", obj.Recl_Bal_E);
            ins.Parameters.Add("pellet_bal", obj.Pellet_Bal);
            ins.Parameters.Add("trucked_recl_bal", obj.Trucked_Recl_Bal);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Met_Agg_Plant2 obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Met_Agg_Plant2 obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Met_Agg_Plant2 SET");
            sql.AppendLine("sched_hours = :sched_hours, ");
            sql.AppendLine("sched_maint_hours = :sched_maint_hours, ");
            sql.AppendLine("unsched_maint_hours = :unsched_maint_hours, ");
            sql.AppendLine("imposed_hours = :imposed_hours, ");
            sql.AppendLine("east_to_stockpile = :east_to_stockpile, ");
            sql.AppendLine("east_to_plant = :east_to_plant, ");
            sql.AppendLine("west_to_stockpile = :west_to_stockpile, ");
            sql.AppendLine("west_to_plant = :west_to_plant, ");
            sql.AppendLine("truck_to_west = :truck_to_west, ");
            sql.AppendLine("truck_from_west = :truck_from_west, ");
            sql.AppendLine("truck_to_east = :truck_to_east, ");
            sql.AppendLine("truck_from_east = :truck_from_east, ");
            sql.AppendLine("bin1 = :bin1, ");
            sql.AppendLine("bin2 = :bin2, ");
            sql.AppendLine("bin3 = :bin3, ");
            sql.AppendLine("bin4 = :bin4, ");
            sql.AppendLine("bin5 = :bin5, ");
            sql.AppendLine("tank1 = :tank1, ");
            sql.AppendLine("tank2 = :tank2, ");
            sql.AppendLine("tank3 = :tank3, ");
            sql.AppendLine("tank4 = :tank4, ");
            sql.AppendLine("grp13_fines = :grp13_fines, ");
            sql.AppendLine("filt_hrs_1_1 = :filt_hrs_1_1, ");
            sql.AppendLine("filt_hrs_1_2 = :filt_hrs_1_2, ");
            sql.AppendLine("filt_hrs_2_1 = :filt_hrs_2_1, ");
            sql.AppendLine("filt_hrs_2_2 = :filt_hrs_2_2, ");
            sql.AppendLine("filt_hrs_4_1 = :filt_hrs_4_1, ");
            sql.AppendLine("filt_hrs_4_2 = :filt_hrs_4_2, ");
            sql.AppendLine("filt_hrs_4_3 = :filt_hrs_4_3, ");
            sql.AppendLine("filt_hrs_4_4 = :filt_hrs_4_4, ");
            sql.AppendLine("filt_hrs_4_5 = :filt_hrs_4_5, ");
            sql.AppendLine("filt_hrs_5_1 = :filt_hrs_5_1, ");
            sql.AppendLine("filt_hrs_5_2 = :filt_hrs_5_2, ");
            sql.AppendLine("filt_hrs_5_3 = :filt_hrs_5_3, ");
            sql.AppendLine("filt_hrs_5_4 = :filt_hrs_5_4, ");
            sql.AppendLine("filt_hrs_5_5 = :filt_hrs_5_5, ");
            sql.AppendLine("filt_tons_1 = :filt_tons_1, ");
            sql.AppendLine("filt_tons_2 = :filt_tons_2, ");
            sql.AppendLine("filtsi = :filtsi, ");
            sql.AppendLine("filtal = :filtal, ");
            sql.AppendLine("filtca = :filtca, ");
            sql.AppendLine("filtmg = :filtmg, ");
            sql.AppendLine("filtmn = :filtmn, ");
            sql.AppendLine("filt270 = :filt270, ");
            sql.AppendLine("filt500 = :filt500, ");
            sql.AppendLine("filth2o = :filth2o, ");
            sql.AppendLine("reclaimh2o = :reclaimh2o, ");
            sql.AppendLine("recl_bal_w = :recl_bal_w, ");
            sql.AppendLine("recl_bal_e = :recl_bal_e, ");
            sql.AppendLine("pellet_bal = :pellet_bal, ");
            sql.AppendLine("trucked_recl_bal = :trucked_recl_bal");
            sql.AppendLine("WHERE datex = :datex AND dmy = :dmy AND material = :material");

            OracleCommand upd = new(sql.ToString(), conn);
            upd.Parameters.Add("sched_hours", obj.Sched_Hours);
            upd.Parameters.Add("sched_maint_hours", obj.Sched_Maint_Hours);
            upd.Parameters.Add("unsched_maint_hours", obj.Unsched_Maint_Hours);
            upd.Parameters.Add("imposed_hours", obj.Imposed_Hours);
            upd.Parameters.Add("east_to_stockpile", obj.East_To_Stockpile);
            upd.Parameters.Add("east_to_plant", obj.East_To_Plant);
            upd.Parameters.Add("west_to_stockpile", obj.West_To_Stockpile);
            upd.Parameters.Add("west_to_plant", obj.West_To_Plant);
            upd.Parameters.Add("truck_to_west", obj.Truck_To_West);
            upd.Parameters.Add("truck_from_west", obj.Truck_From_West);
            upd.Parameters.Add("truck_to_east", obj.Truck_To_East);
            upd.Parameters.Add("truck_from_east", obj.Truck_From_East);
            upd.Parameters.Add("bin1", obj.Bin1);
            upd.Parameters.Add("bin2", obj.Bin2);
            upd.Parameters.Add("bin3", obj.Bin3);
            upd.Parameters.Add("bin4", obj.Bin4);
            upd.Parameters.Add("bin5", obj.Bin5);
            upd.Parameters.Add("tank1", obj.Tank1);
            upd.Parameters.Add("tank2", obj.Tank2);
            upd.Parameters.Add("tank3", obj.Tank3);
            upd.Parameters.Add("tank4", obj.Tank4);
            upd.Parameters.Add("grp13_fines", obj.Grp13_Fines);
            upd.Parameters.Add("filt_hrs_1_1", obj.Filt_Hrs_1_1);
            upd.Parameters.Add("filt_hrs_1_2", obj.Filt_Hrs_1_2);
            upd.Parameters.Add("filt_hrs_2_1", obj.Filt_Hrs_2_1);
            upd.Parameters.Add("filt_hrs_2_2", obj.Filt_Hrs_2_2);
            upd.Parameters.Add("filt_hrs_4_1", obj.Filt_Hrs_4_1);
            upd.Parameters.Add("filt_hrs_4_2", obj.Filt_Hrs_4_2);
            upd.Parameters.Add("filt_hrs_4_3", obj.Filt_Hrs_4_3);
            upd.Parameters.Add("filt_hrs_4_4", obj.Filt_Hrs_4_4);
            upd.Parameters.Add("filt_hrs_4_5", obj.Filt_Hrs_4_5);
            upd.Parameters.Add("filt_hrs_5_1", obj.Filt_Hrs_5_1);
            upd.Parameters.Add("filt_hrs_5_2", obj.Filt_Hrs_5_2);
            upd.Parameters.Add("filt_hrs_5_3", obj.Filt_Hrs_5_3);
            upd.Parameters.Add("filt_hrs_5_4", obj.Filt_Hrs_5_4);
            upd.Parameters.Add("filt_hrs_5_5", obj.Filt_Hrs_5_5);
            upd.Parameters.Add("filt_tons_1", obj.Filt_Tons_1);
            upd.Parameters.Add("filt_tons_2", obj.Filt_Tons_2);
            upd.Parameters.Add("filtsi", obj.Filtsi);
            upd.Parameters.Add("filtal", obj.Filtal);
            upd.Parameters.Add("filtca", obj.Filtca);
            upd.Parameters.Add("filtmg", obj.Filtmg);
            upd.Parameters.Add("filtmn", obj.Filtmn);
            upd.Parameters.Add("filt270", obj.Filt270);
            upd.Parameters.Add("filt500", obj.Filt500);
            upd.Parameters.Add("filth2o", obj.Filth2o);
            upd.Parameters.Add("reclaimh2o", obj.Reclaimh2o);
            upd.Parameters.Add("recl_bal_w", obj.Recl_Bal_W);
            upd.Parameters.Add("recl_bal_e", obj.Recl_Bal_E);
            upd.Parameters.Add("pellet_bal", obj.Pellet_Bal);
            upd.Parameters.Add("trucked_recl_bal", obj.Trucked_Recl_Bal);

            //where clause params
            upd.Parameters.Add("datex", obj.Datex);
            upd.Parameters.Add("dmy", (int)obj.Dmy);
            upd.Parameters.Add("material", (int)obj.Material);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }



        private static Met_Agg_Plant2 DataRowToObject(DataRow row)
        {
            Met_Agg_Plant2 RetVal = new();
            RetVal.Material = (Met_Material)row.Field<decimal>("material");
            RetVal.Datex = row.Field<DateTime>("datex");
            RetVal.Dmy = (Met_DMY)row.Field<decimal>("dmy");
            RetVal.Sched_Hours = row.Field<decimal?>("sched_hours");
            RetVal.Sched_Maint_Hours = row.Field<decimal?>("sched_maint_hours");
            RetVal.Unsched_Maint_Hours = row.Field<decimal?>("unsched_maint_hours");
            RetVal.Imposed_Hours = row.Field<decimal?>("imposed_hours");
            RetVal.East_To_Stockpile = row.Field<decimal?>("east_to_stockpile");
            RetVal.East_To_Plant = row.Field<decimal?>("east_to_plant");
            RetVal.West_To_Stockpile = row.Field<decimal?>("west_to_stockpile");
            RetVal.West_To_Plant = row.Field<decimal?>("west_to_plant");
            RetVal.Truck_To_West = row.Field<decimal?>("truck_to_west");
            RetVal.Truck_From_West = row.Field<decimal?>("truck_from_west");
            RetVal.Truck_To_East = row.Field<decimal?>("truck_to_east");
            RetVal.Truck_From_East = row.Field<decimal?>("truck_from_east");
            RetVal.Bin1 = row.Field<decimal?>("bin1");
            RetVal.Bin2 = row.Field<decimal?>("bin2");
            RetVal.Bin3 = row.Field<decimal?>("bin3");
            RetVal.Bin4 = row.Field<decimal?>("bin4");
            RetVal.Bin5 = row.Field<decimal?>("bin5");
            RetVal.Tank1 = row.Field<decimal?>("tank1");
            RetVal.Tank2 = row.Field<decimal?>("tank2");
            RetVal.Tank3 = row.Field<decimal?>("tank3");
            RetVal.Tank4 = row.Field<decimal?>("tank4");
            RetVal.Grp13_Fines = row.Field<decimal?>("grp13_fines");
            RetVal.Filt_Hrs_1_1 = row.Field<decimal?>("filt_hrs_1_1");
            RetVal.Filt_Hrs_1_2 = row.Field<decimal?>("filt_hrs_1_2");
            RetVal.Filt_Hrs_2_1 = row.Field<decimal?>("filt_hrs_2_1");
            RetVal.Filt_Hrs_2_2 = row.Field<decimal?>("filt_hrs_2_2");
            RetVal.Filt_Hrs_4_1 = row.Field<decimal?>("filt_hrs_4_1");
            RetVal.Filt_Hrs_4_2 = row.Field<decimal?>("filt_hrs_4_2");
            RetVal.Filt_Hrs_4_3 = row.Field<decimal?>("filt_hrs_4_3");
            RetVal.Filt_Hrs_4_4 = row.Field<decimal?>("filt_hrs_4_4");
            RetVal.Filt_Hrs_4_5 = row.Field<decimal?>("filt_hrs_4_5");
            RetVal.Filt_Hrs_5_1 = row.Field<decimal?>("filt_hrs_5_1");
            RetVal.Filt_Hrs_5_2 = row.Field<decimal?>("filt_hrs_5_2");
            RetVal.Filt_Hrs_5_3 = row.Field<decimal?>("filt_hrs_5_3");
            RetVal.Filt_Hrs_5_4 = row.Field<decimal?>("filt_hrs_5_4");
            RetVal.Filt_Hrs_5_5 = row.Field<decimal?>("filt_hrs_5_5");
            RetVal.Filt_Tons_1 = row.Field<decimal?>("filt_tons_1");
            RetVal.Filt_Tons_2 = row.Field<decimal?>("filt_tons_2");
            RetVal.Filtsi = row.Field<decimal?>("filtsi");
            RetVal.Filtal = row.Field<decimal?>("filtal");
            RetVal.Filtca = row.Field<decimal?>("filtca");
            RetVal.Filtmg = row.Field<decimal?>("filtmg");
            RetVal.Filtmn = row.Field<decimal?>("filtmn");
            RetVal.Filt270 = row.Field<decimal?>("filt270");
            RetVal.Filt500 = row.Field<decimal?>("filt500");
            RetVal.Filth2o = row.Field<decimal?>("filth2o");
            RetVal.Reclaimh2o = row.Field<decimal?>("reclaimh2o");
            RetVal.Recl_Bal_W = row.Field<decimal?>("recl_bal_w");
            RetVal.Recl_Bal_E = row.Field<decimal?>("recl_bal_e");
            RetVal.Pellet_Bal = row.Field<decimal?>("pellet_bal");
            RetVal.Trucked_Recl_Bal = row.Field<decimal?>("trucked_recl_bal");
            return RetVal;
        }

    }
}
