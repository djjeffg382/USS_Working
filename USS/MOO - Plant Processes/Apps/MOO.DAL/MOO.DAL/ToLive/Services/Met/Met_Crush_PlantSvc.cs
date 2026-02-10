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
    public static class Met_Crush_PlantSvc
    {
        static Met_Crush_PlantSvc()
        {
            Util.RegisterOracle();
        }

        /// <summary>
        /// Gets the Met data for the specified date
        /// </summary>
        /// <param name="MetRptDate"></param>
        /// <returns></returns>
        public static Met_Crush_Plant Get(DateTime MetRptDate, Met_DMY Dmy = Met_DMY.Day)
        {
            List<Met_Crush_Plant> val = Get(MetRptDate, MetRptDate, Dmy);
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
        public static List<Met_Crush_Plant> Get(DateTime StartDate, DateTime EndDate, Met_DMY Dmy = Met_DMY.Day)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE datex BETWEEN :StartDate AND :EndDate");
            sql.AppendLine($"AND DMY = {(int)Dmy}");
            sql.AppendLine("ORDER BY Datex");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add(":StartDate", StartDate);
            da.SelectCommand.Parameters.Add(":EndDate", EndDate);

            DataSet ds = MOO.Data.ExecuteQuery(da);

            List<Met_Crush_Plant> retVal = new();
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
        public static Met_Crush_Plant GetMonthSummary(DateTime RollDate)
        {
            StringBuilder sql = new();
            sql.AppendLine("DECLARE");
            sql.AppendLine("    RollDate Date;");
            sql.AppendLine("    mcp met_crush_plant%rowtype;");
            sql.AppendLine("BEGIN");
            sql.AppendLine("    RollDate := :RollDate;");
            sql.AppendLine("    tolive.met_roll.met_crush_plant_m(RollDate, mcp);");
            sql.AppendLine("    open :cursor FOR SELECT RollDate datex, 2 dmy, ");

            sql.AppendLine("        ROUND(mcp.tons_009_1,4) tons_009_1, ROUND(mcp.tons_009_2,4) tons_009_2, ROUND(mcp.tons_009_3,4) tons_009_3,");
            sql.AppendLine("        ROUND(mcp.tons_009_4,4) tons_009_4, ROUND(mcp.tons_080_in_1,4) tons_080_in_1, ");
            sql.AppendLine("        ROUND(mcp.tons_080_out_1,4) tons_080_out_1, ROUND(mcp.tons_080_in_3,4) tons_080_in_3, ROUND(mcp.tons_080_out_3,4) tons_080_out_3,");
            sql.AppendLine("        ROUND(mcp.limestone_tons,4) limestone_tons, ROUND(mcp.limestone_hours,4) limestone_hours, ");
            sql.AppendLine("        ROUND(mcp.tons_005_05,4) tons_005_05, ROUND(mcp.hours_005_05,4) hours_005_05, ROUND(mcp.bal_080_1,4) bal_080_1,");
            sql.AppendLine("        ROUND(mcp.bal_080_3,4) bal_080_3, ROUND(mcp.crude_to_crusher_tons,4) crude_to_crusher_tons, ");
            sql.AppendLine("        ROUND(mcp.mine_indicated_sio2,4) mine_indicated_sio2, ROUND(mcp.mine_indicated_sio2_stdev,4) mine_indicated_sio2_stdev,");
            sql.AppendLine("        ROUND(mcp.lc1_and_lc2_pct,4) lc1_and_lc2_pct, ROUND(mcp.hisio2_pct,4) hisio2_pct, ");
            sql.AppendLine("        ROUND(mcp.surface_tons,4) surface_tons, ROUND(mcp.omr_tons,4) omr_tons, ROUND(mcp.omh_tons,4) omh_tons, ROUND(mcp.stocked_tons,4) stocked_tons,");
            sql.AppendLine("        ROUND(mcp.total_handled_tons,4) total_handled_tons, ROUND(mcp.demand_peak_mw,4) demand_peak_mw, ");
            sql.AppendLine("        ROUND(mcp.kwh_pellet_ton,4) kwh_pellet_ton");

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

        /// <summary>
        /// Gets the record from the table where YMD = 3  
        /// </summary>
        /// <param name="RollDate"></param>
        /// <returns></returns>
        public static Met_Crush_Plant GetYearRecord(DateTime RollDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE datex = :RollDate");
            sql.AppendLine("AND DMY = 3");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add(":RollDate", RollDate);
            Met_Crush_Plant retVal;
            DataSet ds = MOO.Data.ExecuteQuery(da);
            if(ds.Tables[0].Rows.Count == 0)
            {
                //didnt find a record so create one now
                retVal = new()
                {
                    Datex = RollDate,
                    Dmy = Met_DMY.Year
                };
            }
            else
                retVal = DataRowToObject(ds.Tables[0].Rows[0]);

            return retVal;
        }


        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine("datex, dmy, ");
            sql.AppendLine("ROUND(tons_009_1,4) tons_009_1, ROUND(tons_009_2,4) tons_009_2, ROUND(tons_009_3,4) tons_009_3,");
            sql.AppendLine("ROUND(tons_009_4,4) tons_009_4, ROUND(tons_080_in_1,4) tons_080_in_1, ");
            sql.AppendLine("ROUND(tons_080_out_1,4) tons_080_out_1, ROUND(tons_080_in_3,4) tons_080_in_3, ROUND(tons_080_out_3,4) tons_080_out_3,");
            sql.AppendLine("ROUND(limestone_tons,4) limestone_tons, ROUND(limestone_hours,4) limestone_hours, ");
            sql.AppendLine("ROUND(tons_005_05,4) tons_005_05, ROUND(hours_005_05,4) hours_005_05, ROUND(bal_080_1,4) bal_080_1,");
            sql.AppendLine("ROUND(bal_080_3,4) bal_080_3, ROUND(crude_to_crusher_tons,4) crude_to_crusher_tons, ");
            sql.AppendLine("ROUND(mine_indicated_sio2,4) mine_indicated_sio2, ROUND(mine_indicated_sio2_stdev,4) mine_indicated_sio2_stdev,");
            sql.AppendLine("ROUND(lc1_and_lc2_pct,4) lc1_and_lc2_pct, ROUND(hisio2_pct,4) hisio2_pct, ");
            sql.AppendLine("ROUND(surface_tons,4) surface_tons, ROUND(omr_tons,4) omr_tons, ROUND(omh_tons,4) omh_tons, ROUND(stocked_tons,4) stocked_tons,");
            sql.AppendLine("ROUND(total_handled_tons,4) total_handled_tons, ROUND(demand_peak_mw,4) demand_peak_mw, ");
            sql.AppendLine("ROUND(kwh_pellet_ton,4) kwh_pellet_ton");
            sql.AppendLine("FROM tolive.met_crush_plant");
            return sql.ToString();
        }


        public static int Insert(Met_Crush_Plant obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Met_Crush_Plant obj, OracleConnection conn)
        {
           
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Met_Crush_Plant(");
            sql.AppendLine("datex, dmy, tons_009_1, tons_009_2, tons_009_3, tons_009_4, tons_080_in_1, ");
            sql.AppendLine("tons_080_out_1, tons_080_in_3, tons_080_out_3, limestone_tons, limestone_hours, ");
            sql.AppendLine("tons_005_05, hours_005_05, bal_080_1, bal_080_3, crude_to_crusher_tons, ");
            sql.AppendLine("mine_indicated_sio2, mine_indicated_sio2_stdev, lc1_and_lc2_pct, hisio2_pct, ");
            sql.AppendLine("surface_tons, omr_tons, omh_tons, stocked_tons, total_handled_tons, demand_peak_mw, ");
            sql.AppendLine("kwh_pellet_ton)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":datex, :dmy, :tons_009_1, :tons_009_2, :tons_009_3, :tons_009_4, :tons_080_in_1, ");
            sql.AppendLine(":tons_080_out_1, :tons_080_in_3, :tons_080_out_3, :limestone_tons, :limestone_hours, ");
            sql.AppendLine(":tons_005_05, :hours_005_05, :bal_080_1, :bal_080_3, :crude_to_crusher_tons, ");
            sql.AppendLine(":mine_indicated_sio2, :mine_indicated_sio2_stdev, :lc1_and_lc2_pct, :hisio2_pct, ");
            sql.AppendLine(":surface_tons, :omr_tons, :omh_tons, :stocked_tons, :total_handled_tons, ");
            sql.AppendLine(":demand_peak_mw, :kwh_pellet_ton)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("datex", obj.Datex);
            ins.Parameters.Add("dmy", (int)obj.Dmy);
            ins.Parameters.Add("tons_009_1", obj.Tons_009_1);
            ins.Parameters.Add("tons_009_2", obj.Tons_009_2);
            ins.Parameters.Add("tons_009_3", obj.Tons_009_3);
            ins.Parameters.Add("tons_009_4", obj.Tons_009_4);
            ins.Parameters.Add("tons_080_in_1", obj.Tons_080_In_1);
            ins.Parameters.Add("tons_080_out_1", obj.Tons_080_Out_1);
            ins.Parameters.Add("tons_080_in_3", obj.Tons_080_In_3);
            ins.Parameters.Add("tons_080_out_3", obj.Tons_080_Out_3);
            ins.Parameters.Add("limestone_tons", obj.Limestone_Tons);
            ins.Parameters.Add("limestone_hours", obj.Limestone_Hours);
            ins.Parameters.Add("tons_005_05", obj.Tons_005_05);
            ins.Parameters.Add("hours_005_05", obj.Hours_005_05);
            ins.Parameters.Add("bal_080_1", obj.Bal_080_1);
            ins.Parameters.Add("bal_080_3", obj.Bal_080_3);
            ins.Parameters.Add("crude_to_crusher_tons", obj.Crude_To_Crusher_Tons);
            ins.Parameters.Add("mine_indicated_sio2", obj.Mine_Indicated_Sio2);
            ins.Parameters.Add("mine_indicated_sio2_stdev", obj.Mine_Indicated_Sio2_Stdev);
            ins.Parameters.Add("lc1_and_lc2_pct", obj.Lc1_And_Lc2_Pct);
            ins.Parameters.Add("hisio2_pct", obj.Hisio2_Pct);
            ins.Parameters.Add("surface_tons", obj.Surface_Tons);
            ins.Parameters.Add("omr_tons", obj.Omr_Tons);
            ins.Parameters.Add("omh_tons", obj.Omh_Tons);
            ins.Parameters.Add("stocked_tons", obj.Stocked_Tons);
            ins.Parameters.Add("total_handled_tons", obj.Total_Handled_Tons);
            ins.Parameters.Add("demand_peak_mw", obj.Demand_Peak_Mw);
            ins.Parameters.Add("kwh_pellet_ton", obj.Kwh_Pellet_Ton);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Met_Crush_Plant obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Met_Crush_Plant obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Met_Crush_Plant SET");
            sql.AppendLine("tons_009_1 = :tons_009_1, ");
            sql.AppendLine("tons_009_2 = :tons_009_2, ");
            sql.AppendLine("tons_009_3 = :tons_009_3, ");
            sql.AppendLine("tons_009_4 = :tons_009_4, ");
            sql.AppendLine("tons_080_in_1 = :tons_080_in_1, ");
            sql.AppendLine("tons_080_out_1 = :tons_080_out_1, ");
            sql.AppendLine("tons_080_in_3 = :tons_080_in_3, ");
            sql.AppendLine("tons_080_out_3 = :tons_080_out_3, ");
            sql.AppendLine("limestone_tons = :limestone_tons, ");
            sql.AppendLine("limestone_hours = :limestone_hours, ");
            sql.AppendLine("tons_005_05 = :tons_005_05, ");
            sql.AppendLine("hours_005_05 = :hours_005_05, ");
            sql.AppendLine("bal_080_1 = :bal_080_1, ");
            sql.AppendLine("bal_080_3 = :bal_080_3, ");
            sql.AppendLine("crude_to_crusher_tons = :crude_to_crusher_tons, ");
            sql.AppendLine("mine_indicated_sio2 = :mine_indicated_sio2, ");
            sql.AppendLine("mine_indicated_sio2_stdev = :mine_indicated_sio2_stdev, ");
            sql.AppendLine("lc1_and_lc2_pct = :lc1_and_lc2_pct, ");
            sql.AppendLine("hisio2_pct = :hisio2_pct, ");
            sql.AppendLine("surface_tons = :surface_tons, ");
            sql.AppendLine("omr_tons = :omr_tons, ");
            sql.AppendLine("omh_tons = :omh_tons, ");
            sql.AppendLine("stocked_tons = :stocked_tons, ");
            sql.AppendLine("total_handled_tons = :total_handled_tons, ");
            sql.AppendLine("demand_peak_mw = :demand_peak_mw, ");
            sql.AppendLine("kwh_pellet_ton = :kwh_pellet_ton");
            sql.AppendLine("WHERE datex = :datex AND dmy = :dmy");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.Parameters.Add("tons_009_1", obj.Tons_009_1);
            upd.Parameters.Add("tons_009_2", obj.Tons_009_2);
            upd.Parameters.Add("tons_009_3", obj.Tons_009_3);
            upd.Parameters.Add("tons_009_4", obj.Tons_009_4);
            upd.Parameters.Add("tons_080_in_1", obj.Tons_080_In_1);
            upd.Parameters.Add("tons_080_out_1", obj.Tons_080_Out_1);
            upd.Parameters.Add("tons_080_in_3", obj.Tons_080_In_3);
            upd.Parameters.Add("tons_080_out_3", obj.Tons_080_Out_3);
            upd.Parameters.Add("limestone_tons", obj.Limestone_Tons);
            upd.Parameters.Add("limestone_hours", obj.Limestone_Hours);
            upd.Parameters.Add("tons_005_05", obj.Tons_005_05);
            upd.Parameters.Add("hours_005_05", obj.Hours_005_05);
            upd.Parameters.Add("bal_080_1", obj.Bal_080_1);
            upd.Parameters.Add("bal_080_3", obj.Bal_080_3);
            upd.Parameters.Add("crude_to_crusher_tons", obj.Crude_To_Crusher_Tons);
            upd.Parameters.Add("mine_indicated_sio2", obj.Mine_Indicated_Sio2);
            upd.Parameters.Add("mine_indicated_sio2_stdev", obj.Mine_Indicated_Sio2_Stdev);
            upd.Parameters.Add("lc1_and_lc2_pct", obj.Lc1_And_Lc2_Pct);
            upd.Parameters.Add("hisio2_pct", obj.Hisio2_Pct);
            upd.Parameters.Add("surface_tons", obj.Surface_Tons);
            upd.Parameters.Add("omr_tons", obj.Omr_Tons);
            upd.Parameters.Add("omh_tons", obj.Omh_Tons);
            upd.Parameters.Add("stocked_tons", obj.Stocked_Tons);
            upd.Parameters.Add("total_handled_tons", obj.Total_Handled_Tons);
            upd.Parameters.Add("demand_peak_mw", obj.Demand_Peak_Mw);
            upd.Parameters.Add("kwh_pellet_ton", obj.Kwh_Pellet_Ton);

            //where clause
            upd.Parameters.Add("datex", obj.Datex);
            upd.Parameters.Add("dmy", (int)obj.Dmy);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }



        private static Met_Crush_Plant DataRowToObject(DataRow row)
        {
            Met_Crush_Plant RetVal = new();
            RetVal.Datex = row.Field<DateTime>("datex");
            RetVal.Dmy = (Met_DMY)row.Field<decimal>("dmy");
            RetVal.Tons_009_1 = row.Field<decimal?>("tons_009_1");
            RetVal.Tons_009_2 = row.Field<decimal?>("tons_009_2");
            RetVal.Tons_009_3 = row.Field<decimal?>("tons_009_3");
            RetVal.Tons_009_4 = row.Field<decimal?>("tons_009_4");
            RetVal.Tons_080_In_1 = row.Field<decimal?>("tons_080_in_1");
            RetVal.Tons_080_Out_1 = row.Field<decimal?>("tons_080_out_1");
            RetVal.Tons_080_In_3 = row.Field<decimal?>("tons_080_in_3");
            RetVal.Tons_080_Out_3 = row.Field<decimal?>("tons_080_out_3");
            RetVal.Limestone_Tons = row.Field<decimal?>("limestone_tons");
            RetVal.Limestone_Hours = row.Field<decimal?>("limestone_hours");
            RetVal.Tons_005_05 = row.Field<decimal?>("tons_005_05");
            RetVal.Hours_005_05 = row.Field<decimal?>("hours_005_05");
            RetVal.Bal_080_1 = row.Field<decimal?>("bal_080_1");
            RetVal.Bal_080_3 = row.Field<decimal?>("bal_080_3");
            RetVal.Crude_To_Crusher_Tons = row.Field<decimal?>("crude_to_crusher_tons");
            RetVal.Mine_Indicated_Sio2 = row.Field<decimal?>("mine_indicated_sio2");
            RetVal.Mine_Indicated_Sio2_Stdev = row.Field<decimal?>("mine_indicated_sio2_stdev");
            RetVal.Lc1_And_Lc2_Pct = row.Field<decimal?>("lc1_and_lc2_pct");
            RetVal.Hisio2_Pct = row.Field<decimal?>("hisio2_pct");
            RetVal.Surface_Tons = row.Field<decimal?>("surface_tons");
            RetVal.Omr_Tons = row.Field<decimal?>("omr_tons");
            RetVal.Omh_Tons = row.Field<decimal?>("omh_tons");
            RetVal.Stocked_Tons = row.Field<decimal?>("stocked_tons");
            RetVal.Total_Handled_Tons = row.Field<decimal?>("total_handled_tons");
            RetVal.Demand_Peak_Mw = row.Field<decimal?>("demand_peak_mw");
            RetVal.Kwh_Pellet_Ton = row.Field<decimal?>("kwh_pellet_ton");
            return RetVal;
        }

    }
}
