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
    public static class Met_Crush_TertiarySvc
    {
        static Met_Crush_TertiarySvc()
        {
            Util.RegisterOracle();
        }



        /// <summary>
        /// Gets the Met_Conc_Line records between the date range
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<Met_Crush_Tertiary> Get(DateTime StartDate, DateTime EndDate, byte StartCrusherNbr, byte EndCrusherNbr)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE datex BETWEEN :StartDate AND :EndDate");
            sql.AppendLine("AND DMY = 1");
            sql.AppendLine("AND unit BETWEEN :StartLine AND :EndLine");
            sql.AppendLine("ORDER BY Datex");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("StartDate", StartDate);
            da.SelectCommand.Parameters.Add("EndDate", EndDate);
            da.SelectCommand.Parameters.Add("StartLine", StartCrusherNbr);
            da.SelectCommand.Parameters.Add("EndLine", EndCrusherNbr);

            DataSet ds = MOO.Data.ExecuteQuery(da);

            List<Met_Crush_Tertiary> retVal = new();
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
        public static List<Met_Crush_Tertiary> GetMonthSummary(DateTime RollDate, byte StartCrusherNbr, byte EndCrusherNbr)
        {
            List<Met_Crush_Tertiary> retVal = new();
            //limit for loop to only be 1-15 if start and line number are outside that range
            for (byte i = Math.Max(StartCrusherNbr, (byte)1); i <= Math.Min(EndCrusherNbr, (byte)28); i++)
            {
                if (i != 17)  //there is no crusher 17
                    retVal.Add(GetMonthSummary(RollDate, i));
            }
            return retVal;
        }


        /// <summary>
        /// Gets the Month summary met data  
        /// </summary>
        /// <param name="RollDate"></param>
        /// <param name="LineNumber"></param>
        /// <returns></returns>
        public static Met_Crush_Tertiary GetMonthSummary(DateTime RollDate, byte LineNumber)
        {
            StringBuilder sql = new();
            sql.AppendLine("DECLARE");
            sql.AppendLine("    RollDate Date;");
            sql.AppendLine("    Line Number;");
            sql.AppendLine("    mct met_crush_tertiary%rowtype;");
            sql.AppendLine("BEGIN");
            sql.AppendLine("    RollDate := :RollDate;");
            sql.AppendLine("    Line := :LineNumber;");
            sql.AppendLine("    tolive.met_roll.met_crush_tertiary_m(RollDate, Line, mct);");
            sql.AppendLine("    open :cursor FOR SELECT RollDate datex, Line unit, 2 dmy, null step,");  //step is null on dmy records in old system so we will maintain this

            sql.AppendLine("            ROUND(mct.crush_tons,4) crush_tons, ROUND(mct.crush_hours,4) crush_hours,");
            sql.AppendLine("            ROUND(mct.crush_kwh,4) crush_kwh, ROUND(mct.metal_dly_hours,4) metal_dly_hours,");
            sql.AppendLine("            ROUND(mct.sched_hours,4) sched_hours");

            sql.AppendLine("FROM DUAL;");
            sql.AppendLine("END;");


            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.Parameters.Add("RollDate", RollDate);
            da.SelectCommand.Parameters.Add("LineNumber", LineNumber);
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
            sql.AppendLine("datex, unit, dmy, step, ");
            sql.AppendLine("ROUND(crush_tons,4) crush_tons, ROUND(crush_hours,4) crush_hours,");
            sql.AppendLine("ROUND(crush_kwh,4) crush_kwh, ROUND(metal_dly_hours,4) metal_dly_hours,");
            sql.AppendLine("ROUND(sched_hours,4) sched_hours");
            sql.AppendLine("FROM tolive.met_crush_tertiary");
            return sql.ToString();
        }


        public static int Insert(Met_Crush_Tertiary obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Met_Crush_Tertiary obj, OracleConnection conn)
        {


            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Met_Crush_Tertiary(");
            sql.AppendLine("datex, unit, dmy, step, crush_tons, crush_hours, crush_kwh, metal_dly_hours, ");
            sql.AppendLine("sched_hours)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":datex, :unit, :dmy, :step, :crush_tons, :crush_hours, :crush_kwh, :metal_dly_hours, ");
            sql.AppendLine(":sched_hours)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("datex", obj.Datex);
            ins.Parameters.Add("unit", obj.Unit);
            ins.Parameters.Add("dmy", (int)obj.Dmy);
            ins.Parameters.Add("step", obj.Step);
            ins.Parameters.Add("crush_tons", obj.Crush_Tons);
            ins.Parameters.Add("crush_hours", obj.Crush_Hours);
            ins.Parameters.Add("crush_kwh", obj.Crush_Kwh);
            ins.Parameters.Add("metal_dly_hours", obj.Metal_Dly_Hours);
            ins.Parameters.Add("sched_hours", obj.Sched_Hours);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Met_Crush_Tertiary obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Met_Crush_Tertiary obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Met_Crush_Tertiary SET");
            sql.AppendLine("step = :step, ");
            sql.AppendLine("crush_tons = :crush_tons, ");
            sql.AppendLine("crush_hours = :crush_hours, ");
            sql.AppendLine("crush_kwh = :crush_kwh, ");
            sql.AppendLine("metal_dly_hours = :metal_dly_hours, ");
            sql.AppendLine("sched_hours = :sched_hours");
            sql.AppendLine("WHERE datex = :datex AND unit = :unit AND dmy = :dmy");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.Parameters.Add("step", obj.Step);
            upd.Parameters.Add("crush_tons", obj.Crush_Tons);
            upd.Parameters.Add("crush_hours", obj.Crush_Hours);
            upd.Parameters.Add("crush_kwh", obj.Crush_Kwh);
            upd.Parameters.Add("metal_dly_hours", obj.Metal_Dly_Hours);
            upd.Parameters.Add("sched_hours", obj.Sched_Hours);

            //where clause
            upd.Parameters.Add("datex", obj.Datex);
            upd.Parameters.Add("unit", obj.Unit);
            upd.Parameters.Add("dmy", (int)obj.Dmy);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }



        private static Met_Crush_Tertiary DataRowToObject(DataRow row)
        {
            Met_Crush_Tertiary RetVal = new();
            RetVal.Datex = row.Field<DateTime>("datex");
            RetVal.Unit = (byte)row.Field<decimal>("unit");
            RetVal.Dmy = (Met_DMY)row.Field<decimal>("dmy");
            RetVal.Step = (byte?)row.Field<decimal?>("step");
            RetVal.Crush_Tons = row.Field<decimal?>("crush_tons");
            RetVal.Crush_Hours = row.Field<decimal?>("crush_hours");
            RetVal.Crush_Kwh = row.Field<decimal?>("crush_kwh");
            RetVal.Metal_Dly_Hours = row.Field<decimal?>("metal_dly_hours");
            RetVal.Sched_Hours = row.Field<decimal?>("sched_hours");
            return RetVal;
        }

    }
}
