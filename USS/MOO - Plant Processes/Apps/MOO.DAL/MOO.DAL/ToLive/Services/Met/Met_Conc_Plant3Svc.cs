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
    public static class Met_Conc_Plant3Svc
    {
        static Met_Conc_Plant3Svc()
        {
            Util.RegisterOracle();
        }

        /// <summary>
        /// Gets the Met data for the specified date
        /// </summary>
        /// <param name="MetRptDate"></param>
        /// <returns></returns>
        public static Met_Conc_Plant3 Get(DateTime MetRptDate)
        {
            List<Met_Conc_Plant3> val = Get(MetRptDate, MetRptDate);
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
        public static List<Met_Conc_Plant3> Get(DateTime StartDate, DateTime EndDate)
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

            List<Met_Conc_Plant3> retVal = new();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                retVal.Add(DataRowToObject(dr));
            }

            return retVal;
        }


        private static string GetSelect(string addField = "")
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine("datex, dmy, ");
            sql.AppendLine("ROUND(sched_hours,4) sched_hours, ROUND(sched_maint_hours,4) sched_maint_hours, ");
            sql.AppendLine("ROUND(unsched_maint_hours,4) unsched_maint_hours, ROUND(aggl_request_hours,4) aggl_request_hours, ");
            sql.AppendLine("ROUND(hi_power_lim_hours,4) hi_power_lim_hours, ROUND(no_ore_hours,4) no_ore_hours, ");
            sql.AppendLine("ROUND(rmf_h2o,4) rmf_h2o, ROUND(rmf_mag_fe,4) rmf_mag_fe, ");
            sql.AppendLine("ROUND(plant_34_inch,4) plant_34_inch, ROUND(plant_12_inch,4) plant_12_inch, ");
            sql.AppendLine("ROUND(dav_tube_rm_sio2,4) dav_tube_rm_sio2, ROUND(step3_nola_sio2,4) step3_nola_sio2,");
            sql.AppendLine("ROUND(lab_flot_sio2,4) lab_flot_sio2");
            if (!string.IsNullOrEmpty(addField))
            {
                sql.Append(", ");
                sql.AppendLine(addField);
            }
            sql.AppendLine("FROM tolive.met_conc_plant3");
            return sql.ToString();
        }


        /// <summary>
        /// Gets the Month summary met data  
        /// </summary>
        /// <param name="RollDate"></param>
        /// <returns></returns>
        public static Met_Conc_Plant3 GetMonthSummary(DateTime RollDate)
        {
            StringBuilder sql = new();
            sql.AppendLine("DECLARE");
            sql.AppendLine("    RollDate Date;");
            sql.AppendLine("    mcp met_conc_plant3%rowtype;");
            sql.AppendLine("BEGIN");
            sql.AppendLine("    RollDate := :RollDate;");
            sql.AppendLine("    tolive.met_roll.met_conc_plant3_m(RollDate, mcp);");
            sql.AppendLine("    open :cursor FOR SELECT RollDate datex, 2 dmy, ");
            sql.AppendLine("        ROUND(mcp.sched_hours,4) sched_hours, ROUND(mcp.sched_maint_hours,4) sched_maint_hours, ");
            sql.AppendLine("        ROUND(mcp.unsched_maint_hours,4) unsched_maint_hours, ROUND(mcp.aggl_request_hours,4) aggl_request_hours, ");
            sql.AppendLine("        ROUND(mcp.hi_power_lim_hours,4) hi_power_lim_hours, ROUND(mcp.no_ore_hours,4) no_ore_hours, ");
            sql.AppendLine("        ROUND(mcp.rmf_h2o,4) rmf_h2o, ROUND(mcp.rmf_mag_fe,4) rmf_mag_fe, ");
            sql.AppendLine("        ROUND(mcp.plant_34_inch,4) plant_34_inch, ROUND(mcp.plant_12_inch,4) plant_12_inch, ");
            sql.AppendLine("        ROUND(mcp.dav_tube_rm_sio2,4) dav_tube_rm_sio2, ROUND(mcp.step3_nola_sio2,4) step3_nola_sio2,");
            sql.AppendLine("        ROUND(mcp.lab_flot_sio2,4) lab_flot_sio2");
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


        public static int Insert(Met_Conc_Plant3 obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Met_Conc_Plant3 obj, OracleConnection conn)
        {
            
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Met_Conc_Plant3(");
            sql.AppendLine("datex, dmy, sched_hours, sched_maint_hours, unsched_maint_hours, aggl_request_hours, ");
            sql.AppendLine("hi_power_lim_hours, no_ore_hours, rmf_h2o, rmf_mag_fe, plant_34_inch, plant_12_inch, ");
            sql.AppendLine("dav_tube_rm_sio2, step3_nola_sio2, lab_flot_sio2)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":datex, :dmy, :sched_hours, :sched_maint_hours, :unsched_maint_hours, ");
            sql.AppendLine(":aggl_request_hours, :hi_power_lim_hours, :no_ore_hours, :rmf_h2o, :rmf_mag_fe, ");
            sql.AppendLine(":plant_34_inch, :plant_12_inch, :dav_tube_rm_sio2, :step3_nola_sio2, :lab_flot_sio2)");
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
            ins.Parameters.Add("plant_34_inch", obj.Plant_34_Inch);
            ins.Parameters.Add("plant_12_inch", obj.Plant_12_Inch);
            ins.Parameters.Add("dav_tube_rm_sio2", obj.Dav_Tube_Rm_Sio2);
            ins.Parameters.Add("step3_nola_sio2", obj.Step3_Nola_Sio2);
            ins.Parameters.Add("lab_flot_sio2", obj.Lab_Flot_Sio2);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Met_Conc_Plant3 obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Met_Conc_Plant3 obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Met_Conc_Plant3 SET");
            sql.AppendLine("sched_hours = :sched_hours, ");
            sql.AppendLine("sched_maint_hours = :sched_maint_hours, ");
            sql.AppendLine("unsched_maint_hours = :unsched_maint_hours, ");
            sql.AppendLine("aggl_request_hours = :aggl_request_hours, ");
            sql.AppendLine("hi_power_lim_hours = :hi_power_lim_hours, ");
            sql.AppendLine("no_ore_hours = :no_ore_hours, ");
            sql.AppendLine("rmf_h2o = :rmf_h2o, ");
            sql.AppendLine("rmf_mag_fe = :rmf_mag_fe, ");
            sql.AppendLine("plant_34_inch = :plant_34_inch, ");
            sql.AppendLine("plant_12_inch = :plant_12_inch, ");
            sql.AppendLine("dav_tube_rm_sio2 = :dav_tube_rm_sio2, ");
            sql.AppendLine("step3_nola_sio2 = :step3_nola_sio2, ");
            sql.AppendLine("lab_flot_sio2 = :lab_flot_sio2");
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
            upd.Parameters.Add("plant_34_inch", obj.Plant_34_Inch);
            upd.Parameters.Add("plant_12_inch", obj.Plant_12_Inch);
            upd.Parameters.Add("dav_tube_rm_sio2", obj.Dav_Tube_Rm_Sio2);
            upd.Parameters.Add("step3_nola_sio2", obj.Step3_Nola_Sio2);
            upd.Parameters.Add("lab_flot_sio2", obj.Lab_Flot_Sio2);

            //where clause
            upd.Parameters.Add("datex", obj.Datex);
            upd.Parameters.Add("dmy", (int)obj.Dmy);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        private static Met_Conc_Plant3 DataRowToObject(DataRow row)
        {
            Met_Conc_Plant3 RetVal = new();
            RetVal.Datex = row.Field<DateTime>("datex");
            RetVal.Dmy =(Met_DMY)row.Field<decimal>("dmy");
            RetVal.Sched_Hours = row.Field<decimal?>("sched_hours");
            RetVal.Sched_Maint_Hours = row.Field<decimal?>("sched_maint_hours");
            RetVal.Unsched_Maint_Hours = row.Field<decimal?>("unsched_maint_hours");
            RetVal.Aggl_Request_Hours = row.Field<decimal?>("aggl_request_hours");
            RetVal.Hi_Power_Lim_Hours = row.Field<decimal?>("hi_power_lim_hours");
            RetVal.No_Ore_Hours = row.Field<decimal?>("no_ore_hours");
            RetVal.Rmf_H2o = row.Field<decimal?>("rmf_h2o");
            RetVal.Rmf_Mag_Fe = row.Field<decimal?>("rmf_mag_fe");
            RetVal.Plant_34_Inch = row.Field<decimal?>("plant_34_inch");
            RetVal.Plant_12_Inch = row.Field<decimal?>("plant_12_inch");
            RetVal.Dav_Tube_Rm_Sio2 = row.Field<decimal?>("dav_tube_rm_sio2");
            RetVal.Step3_Nola_Sio2 = row.Field<decimal?>("step3_nola_sio2");
            RetVal.Lab_Flot_Sio2 = row.Field<decimal?>("lab_flot_sio2");
            return RetVal;
        }

    }
}
