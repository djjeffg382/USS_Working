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
    public static class Met_Agg_Plant3Svc
    {
        static Met_Agg_Plant3Svc()
        {
            Util.RegisterOracle();
        }


        /// <summary>
        /// Gets the Met data for the specified date
        /// </summary>
        /// <param name="MetRptDate"></param>
        /// <returns></returns>
        public static Met_Agg_Plant3 Get(DateTime MetRptDate, Met_Material Material)
        {
            List<Met_Agg_Plant3> val = Get(MetRptDate, MetRptDate, Material);
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
        public static List<Met_Agg_Plant3> Get(DateTime StartDate, DateTime EndDate, Met_Material Material)
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

            List<Met_Agg_Plant3> retVal = new();
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
        public static Met_Agg_Plant3 GetMonthSummary(DateTime RollDate, Met_Material Material)
        {
            StringBuilder sql = new();
            sql.AppendLine("DECLARE");
            sql.AppendLine("    RollDate Date;");
            sql.AppendLine("    ptype Number;");
            sql.AppendLine("    map met_agg_plant3%rowtype;");
            sql.AppendLine("BEGIN");
            sql.AppendLine("    RollDate := :RollDate;");
            sql.AppendLine("    ptype := :PelletType;");
            sql.AppendLine("    tolive.met_roll.met_agg_plant3_m(RollDate,  ptype, map);");
            sql.AppendLine("    open :cursor FOR SELECT RollDate datex, 2 dmy, ptype Material,");


            sql.AppendLine("ROUND(map.sched_hours,4) sched_hours, ROUND(map.sched_maint_hours,4) sched_maint_hours, ROUND(map.unsched_maint_hours,4) unsched_maint_hours, ");
            sql.AppendLine("ROUND(map.imposed_hours,4) imposed_hours, ROUND(map.east_to_stockpile,4) east_to_stockpile, ROUND(map.east_to_plant,4) east_to_plant, ");
            sql.AppendLine("ROUND(map.west_to_stockpile,4) west_to_stockpile, ROUND(map.west_to_plant,4) west_to_plant, ");
            sql.AppendLine("ROUND(map.truck_to_west,4) truck_to_west, ROUND(map.truck_from_west,4) truck_from_west, ROUND(map.truck_to_east,4) truck_to_east, ");
            sql.AppendLine("ROUND(map.truck_from_east,4) truck_from_east, ");
            sql.AppendLine("ROUND(map.bin7,4) bin7, ROUND(map.bin6,4) bin6, ROUND(map.tank6,4) tank6, ");
            sql.AppendLine("ROUND(map.tank7,4) tank7, ROUND(map.grp13_fines,4) grp13_fines,");
            sql.AppendLine("ROUND(map.filt_hrs_1,4) filt_hrs_1, ROUND(map.filt_hrs_2,4) filt_hrs_2, ROUND(map.filt_hrs_3,4) filt_hrs_3,");
            sql.AppendLine("ROUND(map.filt_hrs_4,4) filt_hrs_4, ROUND(map.filt_hrs_5,4) filt_hrs_5, ROUND(map.filt_hrs_6,4) filt_hrs_6,");
            sql.AppendLine("ROUND(map.filt_hrs_7,4) filt_hrs_7, ROUND(map.filt_hrs_8,4) filt_hrs_8, ROUND(map.filt_hrs_9,4) filt_hrs_9,");
            sql.AppendLine("ROUND(map.filt_hrs_10,4) filt_hrs_10, ROUND(map.filt_hrs_11,4) filt_hrs_11, ROUND(map.filt_hrs_12,4) filt_hrs_12, ");
            sql.AppendLine("ROUND(map.filt_tons_3,4) filt_tons_3, ROUND(map.filtsi,4) filtsi, ROUND(map.filtal,4) filtal, ");
            sql.AppendLine("ROUND(map.filtca,4) filtca, ROUND(map.filtmg,4) filtmg, ROUND(map.filtmn,4) filtmn, ");
            sql.AppendLine("ROUND(map.filt270,4) filt270, ROUND(map.filt500,4) filt500, ROUND(map.filth2o,4) filth2o,");
            sql.AppendLine("ROUND(map.reclaimh2o,4) reclaimh2o, ROUND(map.recl_bal_w,4) recl_bal_w, ");
            sql.AppendLine("ROUND(map.pellet_bal,4) pellet_bal, ROUND(map.trucked_recl_bal,4) trucked_recl_bal");

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
            sql.AppendLine("ROUND(truck_from_east,4) truck_from_east, ");
            sql.AppendLine("ROUND(bin7,4) bin7, ROUND(bin6,4) bin6, ROUND(tank6,4) tank6, ");
            sql.AppendLine("ROUND(tank7,4) tank7, ROUND(grp13_fines,4) grp13_fines,");
            sql.AppendLine("ROUND(filt_hrs_1,4) filt_hrs_1, ROUND(filt_hrs_2,4) filt_hrs_2, ROUND(filt_hrs_3,4) filt_hrs_3,");
            sql.AppendLine("ROUND(filt_hrs_4,4) filt_hrs_4, ROUND(filt_hrs_5,4) filt_hrs_5, ROUND(filt_hrs_6,4) filt_hrs_6,");
            sql.AppendLine("ROUND(filt_hrs_7,4) filt_hrs_7, ROUND(filt_hrs_8,4) filt_hrs_8, ROUND(filt_hrs_9,4) filt_hrs_9,");
            sql.AppendLine("ROUND(filt_hrs_10,4) filt_hrs_10, ROUND(filt_hrs_11,4) filt_hrs_11, ROUND(filt_hrs_12,4) filt_hrs_12, ");
            sql.AppendLine("ROUND(filt_tons_3,4) filt_tons_3, ROUND(filtsi,4) filtsi, ROUND(filtal,4) filtal, ");
            sql.AppendLine("ROUND(filtca,4) filtca, ROUND(filtmg,4) filtmg, ROUND(filtmn,4) filtmn, ");
            sql.AppendLine("ROUND(filt270,4) filt270, ROUND(filt500,4) filt500, ROUND(filth2o,4) filth2o,");
            sql.AppendLine("ROUND(reclaimh2o,4) reclaimh2o, ROUND(recl_bal_w,4) recl_bal_w, ");
            sql.AppendLine("ROUND(pellet_bal,4) pellet_bal, ROUND(trucked_recl_bal,4) trucked_recl_bal");
            sql.AppendLine("FROM tolive.met_agg_plant3");
            return sql.ToString();
        }


        public static int Insert(Met_Agg_Plant3 obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Met_Agg_Plant3 obj, OracleConnection conn)
        {
            

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Met_Agg_Plant3(");
            sql.AppendLine("material, datex, dmy, sched_hours, sched_maint_hours, unsched_maint_hours, ");
            sql.AppendLine("imposed_hours, east_to_stockpile, east_to_plant, west_to_stockpile, west_to_plant, ");
            sql.AppendLine("truck_to_west, truck_from_west, truck_to_east, truck_from_east, bin7, bin6, tank6, ");
            sql.AppendLine("tank7, grp13_fines, filt_hrs_1, filt_hrs_2, filt_hrs_3, filt_hrs_4, filt_hrs_5, ");
            sql.AppendLine("filt_hrs_6, filt_hrs_7, filt_hrs_8, filt_hrs_9, filt_hrs_10, filt_hrs_11, ");
            sql.AppendLine("filt_hrs_12, filt_tons_3, filtsi, filtal, filtca, filtmg, filtmn, filt270, filt500, ");
            sql.AppendLine("filth2o, reclaimh2o, recl_bal_w, pellet_bal, trucked_recl_bal)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":material, :datex, :dmy, :sched_hours, :sched_maint_hours, :unsched_maint_hours, ");
            sql.AppendLine(":imposed_hours, :east_to_stockpile, :east_to_plant, :west_to_stockpile, ");
            sql.AppendLine(":west_to_plant, :truck_to_west, :truck_from_west, :truck_to_east, :truck_from_east, ");
            sql.AppendLine(":bin7, :bin6, :tank6, :tank7, :grp13_fines, :filt_hrs_1, :filt_hrs_2, :filt_hrs_3, ");
            sql.AppendLine(":filt_hrs_4, :filt_hrs_5, :filt_hrs_6, :filt_hrs_7, :filt_hrs_8, :filt_hrs_9, ");
            sql.AppendLine(":filt_hrs_10, :filt_hrs_11, :filt_hrs_12, :filt_tons_3, :filtsi, :filtal, :filtca, ");
            sql.AppendLine(":filtmg, :filtmn, :filt270, :filt500, :filth2o, :reclaimh2o, :recl_bal_w, :pellet_bal, ");
            sql.AppendLine(":trucked_recl_bal)");
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
            ins.Parameters.Add("bin7", obj.Bin7);
            ins.Parameters.Add("bin6", obj.Bin6);
            ins.Parameters.Add("tank6", obj.Tank6);
            ins.Parameters.Add("tank7", obj.Tank7);
            ins.Parameters.Add("grp13_fines", obj.Grp13_Fines);
            ins.Parameters.Add("filt_hrs_1", obj.Filt_Hrs_1);
            ins.Parameters.Add("filt_hrs_2", obj.Filt_Hrs_2);
            ins.Parameters.Add("filt_hrs_3", obj.Filt_Hrs_3);
            ins.Parameters.Add("filt_hrs_4", obj.Filt_Hrs_4);
            ins.Parameters.Add("filt_hrs_5", obj.Filt_Hrs_5);
            ins.Parameters.Add("filt_hrs_6", obj.Filt_Hrs_6);
            ins.Parameters.Add("filt_hrs_7", obj.Filt_Hrs_7);
            ins.Parameters.Add("filt_hrs_8", obj.Filt_Hrs_8);
            ins.Parameters.Add("filt_hrs_9", obj.Filt_Hrs_9);
            ins.Parameters.Add("filt_hrs_10", obj.Filt_Hrs_10);
            ins.Parameters.Add("filt_hrs_11", obj.Filt_Hrs_11);
            ins.Parameters.Add("filt_hrs_12", obj.Filt_Hrs_12);
            ins.Parameters.Add("filt_tons_3", obj.Filt_Tons_3);
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
            ins.Parameters.Add("pellet_bal", obj.Pellet_Bal);
            ins.Parameters.Add("trucked_recl_bal", obj.Trucked_Recl_Bal);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Met_Agg_Plant3 obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Met_Agg_Plant3 obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Met_Agg_Plant3 SET");
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
            sql.AppendLine("bin7 = :bin7, ");
            sql.AppendLine("bin6 = :bin6, ");
            sql.AppendLine("tank6 = :tank6, ");
            sql.AppendLine("tank7 = :tank7, ");
            sql.AppendLine("grp13_fines = :grp13_fines, ");
            sql.AppendLine("filt_hrs_1 = :filt_hrs_1, ");
            sql.AppendLine("filt_hrs_2 = :filt_hrs_2, ");
            sql.AppendLine("filt_hrs_3 = :filt_hrs_3, ");
            sql.AppendLine("filt_hrs_4 = :filt_hrs_4, ");
            sql.AppendLine("filt_hrs_5 = :filt_hrs_5, ");
            sql.AppendLine("filt_hrs_6 = :filt_hrs_6, ");
            sql.AppendLine("filt_hrs_7 = :filt_hrs_7, ");
            sql.AppendLine("filt_hrs_8 = :filt_hrs_8, ");
            sql.AppendLine("filt_hrs_9 = :filt_hrs_9, ");
            sql.AppendLine("filt_hrs_10 = :filt_hrs_10, ");
            sql.AppendLine("filt_hrs_11 = :filt_hrs_11, ");
            sql.AppendLine("filt_hrs_12 = :filt_hrs_12, ");
            sql.AppendLine("filt_tons_3 = :filt_tons_3, ");
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
            upd.Parameters.Add("bin7", obj.Bin7);
            upd.Parameters.Add("bin6", obj.Bin6);
            upd.Parameters.Add("tank6", obj.Tank6);
            upd.Parameters.Add("tank7", obj.Tank7);
            upd.Parameters.Add("grp13_fines", obj.Grp13_Fines);
            upd.Parameters.Add("filt_hrs_1", obj.Filt_Hrs_1);
            upd.Parameters.Add("filt_hrs_2", obj.Filt_Hrs_2);
            upd.Parameters.Add("filt_hrs_3", obj.Filt_Hrs_3);
            upd.Parameters.Add("filt_hrs_4", obj.Filt_Hrs_4);
            upd.Parameters.Add("filt_hrs_5", obj.Filt_Hrs_5);
            upd.Parameters.Add("filt_hrs_6", obj.Filt_Hrs_6);
            upd.Parameters.Add("filt_hrs_7", obj.Filt_Hrs_7);
            upd.Parameters.Add("filt_hrs_8", obj.Filt_Hrs_8);
            upd.Parameters.Add("filt_hrs_9", obj.Filt_Hrs_9);
            upd.Parameters.Add("filt_hrs_10", obj.Filt_Hrs_10);
            upd.Parameters.Add("filt_hrs_11", obj.Filt_Hrs_11);
            upd.Parameters.Add("filt_hrs_12", obj.Filt_Hrs_12);
            upd.Parameters.Add("filt_tons_3", obj.Filt_Tons_3);
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
            upd.Parameters.Add("pellet_bal", obj.Pellet_Bal);
            upd.Parameters.Add("trucked_recl_bal", obj.Trucked_Recl_Bal);


            //Where clause
            upd.Parameters.Add("datex", obj.Datex);
            upd.Parameters.Add("dmy", (int)obj.Dmy);
            upd.Parameters.Add("material", (int)obj.Material);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }



        private static Met_Agg_Plant3 DataRowToObject(DataRow row)
        {
            Met_Agg_Plant3 RetVal = new();
            RetVal.Material = (Met_Material)row.Field<decimal>("material");
            RetVal.Datex =row.Field<DateTime>("datex");
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
            RetVal.Bin7 = row.Field<decimal?>("bin7");
            RetVal.Bin6 = row.Field<decimal?>("bin6");
            RetVal.Tank6 = row.Field<decimal?>("tank6");
            RetVal.Tank7 = row.Field<decimal?>("tank7");
            RetVal.Grp13_Fines = row.Field<decimal?>("grp13_fines");
            RetVal.Filt_Hrs_1 = row.Field<decimal?>("filt_hrs_1");
            RetVal.Filt_Hrs_2 = row.Field<decimal?>("filt_hrs_2");
            RetVal.Filt_Hrs_3 = row.Field<decimal?>("filt_hrs_3");
            RetVal.Filt_Hrs_4 = row.Field<decimal?>("filt_hrs_4");
            RetVal.Filt_Hrs_5 = row.Field<decimal?>("filt_hrs_5");
            RetVal.Filt_Hrs_6 = row.Field<decimal?>("filt_hrs_6");
            RetVal.Filt_Hrs_7 = row.Field<decimal?>("filt_hrs_7");
            RetVal.Filt_Hrs_8 = row.Field<decimal?>("filt_hrs_8");
            RetVal.Filt_Hrs_9 = row.Field<decimal?>("filt_hrs_9");
            RetVal.Filt_Hrs_10 = row.Field<decimal?>("filt_hrs_10");
            RetVal.Filt_Hrs_11 = row.Field<decimal?>("filt_hrs_11");
            RetVal.Filt_Hrs_12 = row.Field<decimal?>("filt_hrs_12");
            RetVal.Filt_Tons_3 = row.Field<decimal?>("filt_tons_3");
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
            RetVal.Pellet_Bal = row.Field<decimal?>("pellet_bal");
            RetVal.Trucked_Recl_Bal = row.Field<decimal?>("trucked_recl_bal");
            return RetVal;
        }

    }
}
