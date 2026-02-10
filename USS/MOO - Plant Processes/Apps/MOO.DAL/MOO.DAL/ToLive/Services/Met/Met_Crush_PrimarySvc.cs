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
    public static class Met_Crush_PrimarySvc
    {
        static Met_Crush_PrimarySvc()
        {
            Util.RegisterOracle();
        }


        /// <summary>
        /// Gets the met data for the specified date range
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<Met_Crush_Primary> Get(DateTime StartDate, DateTime EndDate)
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

            List<Met_Crush_Primary> retVal = new();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                retVal.Add(DataRowToObject(dr));
            }

            return retVal;
        }


        /// <summary>
        /// Gets the month summary met data for list of line numbers
        /// </summary>
        /// <param name="RollDate"></param>
        /// <param name="StartLineNbr"></param>
        /// <param name="EndLineNbr"></param>
        /// <returns></returns>
        public static List<Met_Crush_Primary> GetMonthSummary(DateTime RollDate, byte StartLineNbr, byte EndLineNbr)
        {
            List<Met_Crush_Primary> retVal = new();
            //limit for loop to only be 1-4 if start and line number are outside that range
            for (byte i = Math.Max(StartLineNbr, (byte)1); i <= Math.Min(EndLineNbr, (byte)4); i++)
            {
                retVal.Add(GetMonthSummary(RollDate, i));
            }
            return retVal;
        }

        /// <summary>
        /// Gets the Month summary met data  
        /// </summary>
        /// <param name="RollDate"></param>
        /// <returns></returns>
        public static Met_Crush_Primary GetMonthSummary(DateTime RollDate, byte CrusherUnit)
        {           
            StringBuilder sql = new();
            sql.AppendLine("DECLARE");
            sql.AppendLine("    RollDate Date;");
            sql.AppendLine("    UnitNbr Number;");
            sql.AppendLine("    mcp met_crush_primary%rowtype;");
            sql.AppendLine("BEGIN");
            sql.AppendLine("    RollDate := :RollDate;");
            sql.AppendLine("    UnitNbr := :UnitNbr;");
            sql.AppendLine("    tolive.met_roll.met_crush_primary_m(RollDate, UnitNbr, mcp);");
            sql.AppendLine("    open :cursor FOR SELECT RollDate datex, 2 dmy, UnitNbr unit,  null step,");  //set step to null, this is how the old system showed month totals
            sql.AppendLine("            ROUND(mcp.tons,4) tons, ROUND(mcp.feed_time,4) feed_time, ROUND(mcp.down_time,4) down_time,");
            sql.AppendLine("            ROUND(mcp.crushing_time,4) crushing_time, ROUND(mcp.green_idle_time,4) green_idle_time, ");
            sql.AppendLine("            ROUND(mcp.red_idle_time,4) red_idle_time, ROUND(mcp.green_lite_hrs,4) green_lite_hrs, ROUND(mcp.no_ore_hrs,4) no_ore_hrs,");
            sql.AppendLine("            ROUND(mcp.hangup_hrs,4) hangup_hrs, ROUND(mcp.sched_hours,4) sched_hours, ROUND(mcp.avail_hours,4) avail_hours, ");
            sql.AppendLine("            ROUND(mcp.trucks_dumped,4) trucks_dumped");
            sql.AppendLine("FROM DUAL;");
            sql.AppendLine("END;");


            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.Parameters.Add("RollDate", RollDate);
            da.SelectCommand.Parameters.Add("UnitNbr", CrusherUnit);
            OracleParameter outTable = new("cursor", OracleDbType.RefCursor, ParameterDirection.Output);
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
            sql.AppendLine("datex, unit, dmy, step,");
            sql.AppendLine("ROUND(tons,4) tons, ROUND(feed_time,4) feed_time, ROUND(down_time,4) down_time,");
            sql.AppendLine("ROUND(crushing_time,4) crushing_time, ROUND(green_idle_time,4) green_idle_time, ");
            sql.AppendLine("ROUND(red_idle_time,4) red_idle_time, ROUND(green_lite_hrs,4) green_lite_hrs, ROUND(no_ore_hrs,4) no_ore_hrs,");
            sql.AppendLine("ROUND(hangup_hrs,4) hangup_hrs, ROUND(sched_hours,4) sched_hours, ROUND(avail_hours,4) avail_hours, ");
            sql.AppendLine("ROUND(trucks_dumped,4) trucks_dumped");
            sql.AppendLine("FROM tolive.met_crush_primary");
            return sql.ToString();
        }


        public static int Insert(Met_Crush_Primary obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Met_Crush_Primary obj, OracleConnection conn)
        {
            

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Met_Crush_Primary(");
            sql.AppendLine("datex, unit, dmy, step, tons, feed_time, down_time, crushing_time, green_idle_time, ");
            sql.AppendLine("red_idle_time, green_lite_hrs, no_ore_hrs, hangup_hrs, sched_hours, avail_hours, ");
            sql.AppendLine("trucks_dumped)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":datex, :unit, :dmy, :step, :tons, :feed_time, :down_time, :crushing_time, ");
            sql.AppendLine(":green_idle_time, :red_idle_time, :green_lite_hrs, :no_ore_hrs, :hangup_hrs, ");
            sql.AppendLine(":sched_hours, :avail_hours, :trucks_dumped)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("datex", obj.Datex);
            ins.Parameters.Add("unit", obj.Unit);
            ins.Parameters.Add("dmy", (int)obj.Dmy);
            ins.Parameters.Add("step", obj.Step);
            ins.Parameters.Add("tons", obj.Tons);
            ins.Parameters.Add("feed_time", obj.Feed_Time);
            ins.Parameters.Add("down_time", obj.Down_Time);
            ins.Parameters.Add("crushing_time", obj.Crushing_Time);
            ins.Parameters.Add("green_idle_time", obj.Green_Idle_Time);
            ins.Parameters.Add("red_idle_time", obj.Red_Idle_Time);
            ins.Parameters.Add("green_lite_hrs", obj.Green_Lite_Hrs);
            ins.Parameters.Add("no_ore_hrs", obj.No_Ore_Hrs);
            ins.Parameters.Add("hangup_hrs", obj.Hangup_Hrs);
            ins.Parameters.Add("sched_hours", obj.Sched_Hours);
            ins.Parameters.Add("avail_hours", obj.Avail_Hours);
            ins.Parameters.Add("trucks_dumped", obj.Trucks_Dumped);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Met_Crush_Primary obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Met_Crush_Primary obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Met_Crush_Primary SET");
            sql.AppendLine("step = :step, ");
            sql.AppendLine("tons = :tons, ");
            sql.AppendLine("feed_time = :feed_time, ");
            sql.AppendLine("down_time = :down_time, ");
            sql.AppendLine("crushing_time = :crushing_time, ");
            sql.AppendLine("green_idle_time = :green_idle_time, ");
            sql.AppendLine("red_idle_time = :red_idle_time, ");
            sql.AppendLine("green_lite_hrs = :green_lite_hrs, ");
            sql.AppendLine("no_ore_hrs = :no_ore_hrs, ");
            sql.AppendLine("hangup_hrs = :hangup_hrs, ");
            sql.AppendLine("sched_hours = :sched_hours, ");
            sql.AppendLine("avail_hours = :avail_hours, ");
            sql.AppendLine("trucks_dumped = :trucks_dumped");
            sql.AppendLine("WHERE datex = :datex AND dmy = :dmy AND unit = :unit");

            OracleCommand upd = new(sql.ToString(), conn);
            upd.Parameters.Add("step", obj.Step);
            upd.Parameters.Add("tons", obj.Tons);
            upd.Parameters.Add("feed_time", obj.Feed_Time);
            upd.Parameters.Add("down_time", obj.Down_Time);
            upd.Parameters.Add("crushing_time", obj.Crushing_Time);
            upd.Parameters.Add("green_idle_time", obj.Green_Idle_Time);
            upd.Parameters.Add("red_idle_time", obj.Red_Idle_Time);
            upd.Parameters.Add("green_lite_hrs", obj.Green_Lite_Hrs);
            upd.Parameters.Add("no_ore_hrs", obj.No_Ore_Hrs);
            upd.Parameters.Add("hangup_hrs", obj.Hangup_Hrs);
            upd.Parameters.Add("sched_hours", obj.Sched_Hours);
            upd.Parameters.Add("avail_hours", obj.Avail_Hours);
            upd.Parameters.Add("trucks_dumped", obj.Trucks_Dumped);

            //Where clause
            upd.Parameters.Add("datex", obj.Datex);
            upd.Parameters.Add("dmy", (int)obj.Dmy);
            upd.Parameters.Add("unit", obj.Unit);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        private static Met_Crush_Primary DataRowToObject(DataRow row)
        {
            Met_Crush_Primary RetVal = new();
            RetVal.Datex = row.Field<DateTime>("datex");
            RetVal.Unit = (byte)row.Field<decimal>("unit");
            RetVal.Dmy = (Met_DMY)row.Field<decimal>("dmy");
            RetVal.Step = (byte?)row.Field<decimal?>("step");
            RetVal.Tons = row.Field<decimal?>("tons");
            RetVal.Feed_Time = row.Field<decimal?>("feed_time");
            RetVal.Down_Time = row.Field<decimal?>("down_time");
            RetVal.Crushing_Time = row.Field<decimal?>("crushing_time");
            RetVal.Green_Idle_Time = row.Field<decimal?>("green_idle_time");
            RetVal.Red_Idle_Time = row.Field<decimal?>("red_idle_time");
            RetVal.Green_Lite_Hrs = row.Field<decimal?>("green_lite_hrs");
            RetVal.No_Ore_Hrs = row.Field<decimal?>("no_ore_hrs");
            RetVal.Hangup_Hrs = row.Field<decimal?>("hangup_hrs");
            RetVal.Sched_Hours = row.Field<decimal?>("sched_hours");
            RetVal.Avail_Hours = row.Field<decimal?>("avail_hours");
            RetVal.Trucks_Dumped = row.Field<decimal?>("trucks_dumped");
            return RetVal;
        }

    }
}
