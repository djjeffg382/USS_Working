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
    public class Met_Conc_LineSvc
    {
        static Met_Conc_LineSvc()
        {
            Util.RegisterOracle();
        }


        /// <summary>
        /// Gets the Met_Conc_Line records between the date range
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<Met_Conc_Line> Get(DateTime StartDate, DateTime EndDate, byte StartLineNbr, byte EndLineNbr)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE datex BETWEEN :StartDate AND :EndDate");
            sql.AppendLine("AND DMY = 1");
            sql.AppendLine("AND line BETWEEN :StartLine AND :EndLine");
            sql.AppendLine("ORDER BY Datex");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("StartDate", StartDate);
            da.SelectCommand.Parameters.Add("EndDate", EndDate);
            da.SelectCommand.Parameters.Add("StartLine", StartLineNbr);
            da.SelectCommand.Parameters.Add("EndLine", EndLineNbr);

            DataSet ds = MOO.Data.ExecuteQuery(da);

            List<Met_Conc_Line> retVal = new();
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
        public static List<Met_Conc_Line> GetMonthSummary(DateTime RollDate, byte StartLineNbr, byte EndLineNbr)
        {
            List<Met_Conc_Line> retVal = new();
            //limit for loop to only be 2-18 if start and line number are outside that range
            for (byte i = Math.Max(StartLineNbr, (byte)2); i <= Math.Min(EndLineNbr, (byte)18); i++)
            {
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
        public static Met_Conc_Line GetMonthSummary(DateTime RollDate, byte LineNumber)
        {
            StringBuilder sql = new();
            sql.AppendLine("DECLARE");
            sql.AppendLine("    RollDate Date;");
            sql.AppendLine("    Line Number;");
            sql.AppendLine("    mcl met_conc_line%rowtype;");
            sql.AppendLine("BEGIN");
            sql.AppendLine("    RollDate := :RollDate;");
            sql.AppendLine("    Line := :LineNumber;");
            sql.AppendLine("    tolive.met_roll.met_conc_line_m(RollDate, Line, mcl);");
            sql.AppendLine("    open :cursor FOR SELECT RollDate datex, Line line, 2 dmy, ");
            sql.AppendLine("        ROUND(mcl.rmf_tons,4) rmf_tons,ROUND(mcl.flux_tons,4) flux_tons, ROUND(mcl.rmf_hours,4) rmf_hours, ");
            sql.AppendLine("        ROUND(mcl.rgrnd_hours,4) rgrnd_hours, ROUND(mcl.flux_hours,4) flux_hours, ROUND(mcl.rmf_rm_power, 4) rmf_rm_power, ");
            sql.AppendLine("        ROUND(mcl.flux_rm_power,4) flux_rm_power, ROUND(mcl.rmf_pbm_power,4) rmf_pbm_power, ROUND(mcl.regrind_pbm_power,4) regrind_pbm_power, ");
            sql.AppendLine("        ROUND(mcl.flux_pbm_power,4) flux_pbm_power, ROUND(mcl.rmf_sbm_power,4) rmf_sbm_power, ROUND(mcl.regrind_sbm_power,4) regrind_sbm_power, ");
            sql.AppendLine("        ROUND(mcl.flux_sbm_power,4) flux_sbm_power, ROUND(mcl.coil_mag_fe,4) coil_mag_fe, ROUND(mcl.conc_sio2,4) conc_sio2, ROUND(mcl.conc_270,4) conc_270, ");
            sql.AppendLine("        ROUND(mcl.conc_tons,4) conc_tons, ROUND(mcl.ct_mag_fe,4) ct_mag_fe, ROUND(mcl.ft_mag_fe,4) ft_mag_fe,");
            sql.AppendLine("        ROUND(mcl.rmf_tons_adj,4) rmf_tons_adj, ROUND(mcl.schedhours,4) schedhours, ROUND(mcl.flux_minus_200,4) flux_minus_200");
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
            sql.AppendLine("datex, line, dmy, ROUND(rmf_tons,4) rmf_tons, ROUND(flux_tons,4) flux_tons, ROUND(rmf_hours,4) rmf_hours, ");
            sql.AppendLine("ROUND(rgrnd_hours,4) rgrnd_hours, ROUND(flux_hours,4) flux_hours, ROUND(rmf_rm_power, 4) rmf_rm_power, ");
            sql.AppendLine("ROUND(flux_rm_power,4) flux_rm_power, ROUND(rmf_pbm_power,4) rmf_pbm_power, ROUND(regrind_pbm_power,4) regrind_pbm_power, ");
            sql.AppendLine("ROUND(flux_pbm_power,4) flux_pbm_power, ROUND(rmf_sbm_power,4) rmf_sbm_power, ROUND(regrind_sbm_power,4) regrind_sbm_power, ");
            sql.AppendLine("ROUND(flux_sbm_power,4) flux_sbm_power, ROUND(coil_mag_fe,4) coil_mag_fe, ROUND(conc_sio2,4) conc_sio2, ROUND(conc_270,4) conc_270, ");
            sql.AppendLine("ROUND(conc_tons,4) conc_tons, ROUND(ct_mag_fe,4) ct_mag_fe, ROUND(ft_mag_fe,4) ft_mag_fe,");
            sql.AppendLine("ROUND(rmf_tons_adj,4) rmf_tons_adj, ROUND(schedhours,4) schedhours, ROUND(flux_minus_200,4) flux_minus_200");

            sql.AppendLine("FROM tolive.met_conc_line");
            return sql.ToString();
        }


        public static int Update(Met_Conc_Line obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Met_Conc_Line obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.met_conc_line SET");
            sql.AppendLine("rmf_tons = :rmf_tons, ");
            sql.AppendLine("flux_tons = :flux_tons, ");
            sql.AppendLine("rmf_hours = :rmf_hours, ");
            sql.AppendLine("rgrnd_hours = :rgrnd_hours, ");
            sql.AppendLine("flux_hours = :flux_hours, ");
            sql.AppendLine("rmf_rm_power = :rmf_rm_power, ");
            sql.AppendLine("flux_rm_power = :flux_rm_power, ");
            sql.AppendLine("rmf_pbm_power = :rmf_pbm_power, ");
            sql.AppendLine("regrind_pbm_power = :regrind_pbm_power, ");
            sql.AppendLine("flux_pbm_power = :flux_pbm_power, ");
            sql.AppendLine("rmf_sbm_power = :rmf_sbm_power, ");
            sql.AppendLine("regrind_sbm_power = :regrind_sbm_power, ");
            sql.AppendLine("flux_sbm_power = :flux_sbm_power, ");
            sql.AppendLine("coil_mag_fe = :coil_mag_fe, ");
            sql.AppendLine("conc_sio2 = :conc_sio2, ");
            sql.AppendLine("conc_270 = :conc_270, ");
            sql.AppendLine("conc_tons = :conc_tons, ");
            sql.AppendLine("ct_mag_fe = :ct_mag_fe, ");
            sql.AppendLine("ft_mag_fe = :ft_mag_fe, ");
            sql.AppendLine("rmf_tons_adj = :rmf_tons_adj, ");
            sql.AppendLine("schedhours = :schedhours, ");
            sql.AppendLine("flux_minus_200 = :flux_minus_200");
            sql.AppendLine("WHERE datex = :datex AND line = :line AND dmy = :dmy");


            OracleCommand upd = new(sql.ToString(), conn);
            upd.Parameters.Add("rmf_tons", obj.Rmf_Tons);
            upd.Parameters.Add("flux_tons", obj.Flux_Tons);
            upd.Parameters.Add("rmf_hours", obj.Rmf_Hours);
            upd.Parameters.Add("rgrnd_hours", obj.Rgrnd_Hours);
            upd.Parameters.Add("flux_hours", obj.Flux_Hours);
            upd.Parameters.Add("rmf_rm_power", obj.Rmf_Rm_Power);
            upd.Parameters.Add("flux_rm_power", obj.Flux_Rm_Power);
            upd.Parameters.Add("rmf_pbm_power", obj.Rmf_Pbm_Power);
            upd.Parameters.Add("regrind_pbm_power", obj.Regrind_Pbm_Power);
            upd.Parameters.Add("flux_pbm_power", obj.Flux_Pbm_Power);
            upd.Parameters.Add("rmf_sbm_power", obj.Rmf_Sbm_Power);
            upd.Parameters.Add("regrind_sbm_power", obj.Regrind_Sbm_Power);
            upd.Parameters.Add("flux_sbm_power", obj.Flux_Sbm_Power);
            upd.Parameters.Add("coil_mag_fe", obj.Coil_Mag_Fe);
            upd.Parameters.Add("conc_sio2", obj.Conc_Sio2);
            upd.Parameters.Add("conc_270", obj.Conc_270);
            upd.Parameters.Add("conc_tons", obj.Conc_Tons);
            upd.Parameters.Add("ct_mag_fe", obj.Ct_Mag_Fe);
            upd.Parameters.Add("ft_mag_fe", obj.Ft_Mag_Fe);
            upd.Parameters.Add("rmf_tons_adj", obj.Rmf_Tons_Adj);
            upd.Parameters.Add("schedhours", obj.Schedhours);
            upd.Parameters.Add("flux_minus_200", obj.Flux_Minus_200);

            //Where clause
            upd.Parameters.Add("datex", obj.Datex);
            upd.Parameters.Add("line", obj.Line);
            upd.Parameters.Add("dmy", (int)obj.Dmy);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }





        public static int Insert(Met_Conc_Line obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Met_Conc_Line obj, OracleConnection conn)
        {
            //verify if dmy = 2, then datex must be last day of month
            if (obj.Dmy == Met_DMY.Month && MOO.Dates.LastDayOfMonth(obj.Datex) != obj.Datex)
            {
                throw new Exception($"Cannot insert met_conc_line month end for date {obj.Datex:MM/dd/yyyy}, date is not last day of month");
            }

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Met_Conc_Line(");
            sql.AppendLine("datex, line, dmy, rmf_tons, flux_tons, rmf_hours, rgrnd_hours, flux_hours, ");
            sql.AppendLine("rmf_rm_power, flux_rm_power, rmf_pbm_power, regrind_pbm_power, flux_pbm_power, ");
            sql.AppendLine("rmf_sbm_power, regrind_sbm_power, flux_sbm_power, coil_mag_fe, conc_sio2, conc_270, ");
            sql.AppendLine("conc_tons, ct_mag_fe, ft_mag_fe, rmf_tons_adj, schedhours, flux_minus_200)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":datex, :line, :dmy, :rmf_tons, :flux_tons, :rmf_hours, :rgrnd_hours, :flux_hours, ");
            sql.AppendLine(":rmf_rm_power, :flux_rm_power, :rmf_pbm_power, :regrind_pbm_power, :flux_pbm_power, ");
            sql.AppendLine(":rmf_sbm_power, :regrind_sbm_power, :flux_sbm_power, :coil_mag_fe, :conc_sio2, ");
            sql.AppendLine(":conc_270, :conc_tons, :ct_mag_fe, :ft_mag_fe, :rmf_tons_adj, :schedhours, ");
            sql.AppendLine(":flux_minus_200)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("datex", obj.Datex);
            ins.Parameters.Add("line", obj.Line);
            ins.Parameters.Add("dmy", (int)obj.Dmy);
            ins.Parameters.Add("rmf_tons", obj.Rmf_Tons);
            ins.Parameters.Add("flux_tons", obj.Flux_Tons);
            ins.Parameters.Add("rmf_hours", obj.Rmf_Hours);
            ins.Parameters.Add("rgrnd_hours", obj.Rgrnd_Hours);
            ins.Parameters.Add("flux_hours", obj.Flux_Hours);
            ins.Parameters.Add("rmf_rm_power", obj.Rmf_Rm_Power);
            ins.Parameters.Add("flux_rm_power", obj.Flux_Rm_Power);
            ins.Parameters.Add("rmf_pbm_power", obj.Rmf_Pbm_Power);
            ins.Parameters.Add("regrind_pbm_power", obj.Regrind_Pbm_Power);
            ins.Parameters.Add("flux_pbm_power", obj.Flux_Pbm_Power);
            ins.Parameters.Add("rmf_sbm_power", obj.Rmf_Sbm_Power);
            ins.Parameters.Add("regrind_sbm_power", obj.Regrind_Sbm_Power);
            ins.Parameters.Add("flux_sbm_power", obj.Flux_Sbm_Power);
            ins.Parameters.Add("coil_mag_fe", obj.Coil_Mag_Fe);
            ins.Parameters.Add("conc_sio2", obj.Conc_Sio2);
            ins.Parameters.Add("conc_270", obj.Conc_270);
            ins.Parameters.Add("conc_tons", obj.Conc_Tons);
            ins.Parameters.Add("ct_mag_fe", obj.Ct_Mag_Fe);
            ins.Parameters.Add("ft_mag_fe", obj.Ft_Mag_Fe);
            ins.Parameters.Add("rmf_tons_adj", obj.Rmf_Tons_Adj);
            ins.Parameters.Add("schedhours", obj.Schedhours);
            ins.Parameters.Add("flux_minus_200", obj.Flux_Minus_200);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }



        private static Met_Conc_Line DataRowToObject(DataRow row)
        {
            Met_Conc_Line RetVal = new();
            RetVal.Datex = row.Field<DateTime>("datex");
            RetVal.Line = (byte)row.Field<decimal>("line");
            RetVal.Dmy = (Met_DMY)row.Field<decimal>("dmy");
            RetVal.Rmf_Tons = row.Field<decimal?>("rmf_tons");
            RetVal.Flux_Tons = row.Field<decimal?>("flux_tons");
            RetVal.Rmf_Hours = row.Field<decimal?>("rmf_hours");
            RetVal.Rgrnd_Hours = row.Field<decimal?>("rgrnd_hours");
            RetVal.Flux_Hours = row.Field<decimal?>("flux_hours");
            RetVal.Rmf_Rm_Power = row.Field<decimal?>("rmf_rm_power");
            RetVal.Flux_Rm_Power = row.Field<decimal?>("flux_rm_power");
            RetVal.Rmf_Pbm_Power = row.Field<decimal?>("rmf_pbm_power");
            RetVal.Regrind_Pbm_Power = row.Field<decimal?>("regrind_pbm_power");
            RetVal.Flux_Pbm_Power = row.Field<decimal?>("flux_pbm_power");
            RetVal.Rmf_Sbm_Power = row.Field<decimal?>("rmf_sbm_power");
            RetVal.Regrind_Sbm_Power = row.Field<decimal?>("regrind_sbm_power");
            RetVal.Flux_Sbm_Power = row.Field<decimal?>("flux_sbm_power");
            RetVal.Coil_Mag_Fe = row.Field<decimal?>("coil_mag_fe");
            RetVal.Conc_Sio2 = row.Field<decimal?>("conc_sio2");
            RetVal.Conc_270 = row.Field<decimal?>("conc_270");
            RetVal.Conc_Tons = row.Field<decimal?>("conc_tons");
            RetVal.Ct_Mag_Fe = row.Field<decimal?>("ct_mag_fe");
            RetVal.Ft_Mag_Fe = row.Field<decimal?>("ft_mag_fe");
            RetVal.Rmf_Tons_Adj = row.Field<decimal?>("rmf_tons_adj");
            RetVal.Schedhours = row.Field<decimal?>("schedhours");
            RetVal.Flux_Minus_200 = row.Field<decimal?>("flux_minus_200");
            return RetVal;
        }


    }
}
