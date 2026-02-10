using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Services
{
    /// <summary>
    /// Class for showing Minntac Concentrator Recovery Analytics
    /// </summary>
    /// <remarks>As of this time we should not need Insert/Update/Delete as this data is published from Data Analytics group to our database. We should only need read</remarks>
    public static class Conc_Analytics_RecoverySvc
    {
        static Conc_Analytics_RecoverySvc()
        {
            Util.RegisterOracle();
        }


        public static Conc_Analytics_Recovery Get(DateTime datetime)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE datetime = :datetime");


            Conc_Analytics_Recovery retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("datetime", datetime);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }

        public static Conc_Analytics_Recovery GetLatest()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT * ");
            sql.AppendLine("FROM (");
            sql.AppendLine("SELECT ROW_NUMBER() OVER(ORDER BY datetime desc) rnum, ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.conc_analytics_recovery");
            sql.AppendLine(")");
            sql.AppendLine("WHERE rnum = 1");


            Conc_Analytics_Recovery retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }


        public static List<Conc_Analytics_Recovery> GetByDateRange(DateTime StartDate, DateTime EndDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE datetime BETWEEN :StartDate AND :EndDate");

            List<Conc_Analytics_Recovery> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("StartDate", StartDate);
            cmd.Parameters.Add("EndDate", EndDate);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    elements.Add(DataRowToObject(rdr));
                }
            }
            conn.Close();
            return elements;
        }

        /// <summary>
        /// Gets the Weighted average per day for Conc Analytics recovery
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="AbnormanPeriodFilter">Whether to filter out abnormal filter = 1 conditions</param>
        /// <returns></returns>
        /// <remarks>NOTE: The data is grouped by 23:00 - 23:00, this is to align more closer to the 22:30 start date for Minntac 3 shift schedule</remarks>
        public static List<Conc_Analytics_Recovery> GetByDateRangeWeightedAvg(DateTime StartDate, DateTime EndDate, bool AbnormanPeriodFilter = false)
        {
            //This sql got a bit complex, the DECODE in every column is to handle when we have nulls in the numerator to then not weight that into the average
            String sql = @"select trunc(DATETIME + 1/24) as DATETIME, 
                cast(sum(RMF_RATE_TOTAL_LT) as NUMBER(10,2)) as Rmf_Rate_Total_Lt,  
                cast(round(sum(S12_MAGFE_MEDIAN_PCT * RMF_RATE_TOTAL_LT) / nullif(sum(DECODE(S12_MAGFE_MEDIAN_PCT, null, null, RMF_RATE_TOTAL_LT)), 0), 4) as NUMBER(10,2)) S12_Magfe_Median_Pct,
                cast(round(sum(S3_Magfe_Median_Pct * RMF_RATE_TOTAL_LT) / nullif(sum(DECODE(S3_Magfe_Median_Pct, null, null, RMF_RATE_TOTAL_LT)), 0), 4) as NUMBER(10,2)) S3_Magfe_Median_Pct,
                cast(round(sum(Crse_Tail_Magfe_Pct * RMF_RATE_TOTAL_LT) / nullif(sum(DECODE(Crse_Tail_Magfe_Pct, null, null, RMF_RATE_TOTAL_LT)), 0), 4) as NUMBER(10,2)) Crse_Tail_Magfe_Pct,
                cast(round(sum(Fine_Tail_Magfe_Pct * RMF_RATE_TOTAL_LT) / nullif(sum(DECODE(Fine_Tail_Magfe_Pct, null, null, RMF_RATE_TOTAL_LT)), 0), 4) as NUMBER(10,2)) Fine_Tail_Magfe_Pct,
                cast(round(sum(Mag_Plnt_Conc_Sio2_Pct * RMF_RATE_TOTAL_LT) / nullif(sum(DECODE(Mag_Plnt_Conc_Sio2_Pct, null, null, RMF_RATE_TOTAL_LT)), 0), 4) as NUMBER(10,2)) Mag_Plnt_Conc_Sio2_Pct,
                cast(round(sum(Rougher_Fd_Sio2_Pct * RMF_RATE_TOTAL_LT) / nullif(sum(DECODE(Rougher_Fd_Sio2_Pct, null, null, RMF_RATE_TOTAL_LT)), 0), 4) as NUMBER(10,2)) Rougher_Fd_Sio2_Pct,
                cast(round(sum(Conc_Sio2_Pct * RMF_RATE_TOTAL_LT) / nullif(sum(DECODE(Conc_Sio2_Pct, null, null, RMF_RATE_TOTAL_LT)), 0), 4) as NUMBER(10,2)) Conc_Sio2_Pct,
                cast(round(sum(Float_Tail_Sio2_Pct * RMF_RATE_TOTAL_LT) / nullif(sum(DECODE(Float_Tail_Sio2_Pct, null, null, RMF_RATE_TOTAL_LT)), 0), 4) as NUMBER(10,2)) Float_Tail_Sio2_Pct,
                cast(round(sum(Mag_Plnt_Recov_Magfe_Pct * RMF_RATE_TOTAL_LT) / nullif(sum(DECODE(Mag_Plnt_Recov_Magfe_Pct, null, null, RMF_RATE_TOTAL_LT)), 0), 4) as NUMBER(10,2)) Mag_Plnt_Recov_Magfe_Pct,
                cast(round(sum(Flot_Recov_Magfe_Pct * RMF_RATE_TOTAL_LT) / nullif(sum(DECODE(Flot_Recov_Magfe_Pct, null, null, RMF_RATE_TOTAL_LT)), 0), 4) as NUMBER(10,2)) Flot_Recov_Magfe_Pct,
                cast(round(sum(Tot_Recov_Magfe_Pct * RMF_RATE_TOTAL_LT) / nullif(sum(DECODE(Tot_Recov_Magfe_Pct, null, null, RMF_RATE_TOTAL_LT)), 0), 4) as NUMBER(10,2)) Tot_Recov_Magfe_Pct,
                cast(round(sum(Mag_Plnt_Wgt_Magfe_Pct * RMF_RATE_TOTAL_LT) / nullif(sum(DECODE(Mag_Plnt_Wgt_Magfe_Pct, null, null, RMF_RATE_TOTAL_LT)), 0), 4) as NUMBER(10,2)) Mag_Plnt_Wgt_Magfe_Pct,
                cast(round(sum(Flot_Wgt_Magfe_Pct * RMF_RATE_TOTAL_LT) / nullif(sum(DECODE(Flot_Wgt_Magfe_Pct, null, null, RMF_RATE_TOTAL_LT)), 0), 4) as NUMBER(10,2)) Flot_Wgt_Magfe_Pct,
                cast(round(sum(Tot_Wgt_Magfe_Pct * RMF_RATE_TOTAL_LT) / nullif(sum(DECODE(Tot_Wgt_Magfe_Pct, null, null, RMF_RATE_TOTAL_LT)), 0), 4) as NUMBER(10,2)) Tot_Wgt_Magfe_Pct,
                cast(round(sum(Prev_Mag_Plnt_Recov_Magfe_Pct * RMF_RATE_TOTAL_LT) / nullif(sum(DECODE(Prev_Mag_Plnt_Recov_Magfe_Pct, null, null, RMF_RATE_TOTAL_LT)), 0), 4) as NUMBER(10,2)) Prev_Mag_Plnt_Recov_Magfe_Pct,
                cast(round(sum(Prev_Flot_Recov_Magfe_Pct * RMF_RATE_TOTAL_LT) / nullif(sum(DECODE(Prev_Flot_Recov_Magfe_Pct, null, null, RMF_RATE_TOTAL_LT)), 0), 4) as NUMBER(10,2)) Prev_Flot_Recov_Magfe_Pct,
                cast(round(sum(Prev_Tot_Recov_Mag_Fe_Pct * RMF_RATE_TOTAL_LT) / nullif(sum(DECODE(Prev_Tot_Recov_Mag_Fe_Pct, null, null, RMF_RATE_TOTAL_LT)), 0), 4) as NUMBER(10,2)) Prev_Tot_Recov_Mag_Fe_Pct,
                cast(round(sum(Prev_Mag_Plnt_Wgt_Magfe_Pct * RMF_RATE_TOTAL_LT) / nullif(sum(DECODE(Prev_Mag_Plnt_Wgt_Magfe_Pct, null, null, RMF_RATE_TOTAL_LT)), 0), 4) as NUMBER(10,2)) Prev_Mag_Plnt_Wgt_Magfe_Pct,
                cast(round(sum(Prev_Flot_Wgt_Magfe_Pct * RMF_RATE_TOTAL_LT) / nullif(sum(DECODE(Prev_Flot_Wgt_Magfe_Pct, null, null, RMF_RATE_TOTAL_LT)), 0), 4) as NUMBER(10,2)) Prev_Flot_Wgt_Magfe_Pct,
                cast(round(sum(Prev_Tot_Wgt_Magfe_Pct * RMF_RATE_TOTAL_LT) / nullif(sum(DECODE(Prev_Tot_Wgt_Magfe_Pct, null, null, RMF_RATE_TOTAL_LT)), 0), 4) as NUMBER(10,2)) Prev_Tot_Wgt_Magfe_Pct,
                cast(round(sum(Baseline_Norm_Mag_Recov * RMF_RATE_TOTAL_LT) / nullif(sum(DECODE(Baseline_Norm_Mag_Recov, null, null, RMF_RATE_TOTAL_LT)), 0), 4) as NUMBER(10,2)) Baseline_Norm_Mag_Recov,
                cast(round(sum(Baseline_Norm_Flot_Recov * RMF_RATE_TOTAL_LT) / nullif(sum(DECODE(Baseline_Norm_Flot_Recov, null, null, RMF_RATE_TOTAL_LT)), 0), 4) as NUMBER(10,2)) Baseline_Norm_Flot_Recov,
                cast(round(sum(Tot_Plnt_Magfe_Median_Pct * RMF_RATE_TOTAL_LT) / nullif(sum(DECODE(Tot_Plnt_Magfe_Median_Pct, null, null, RMF_RATE_TOTAL_LT)), 0), 4) as NUMBER(10,2)) Tot_Plnt_Magfe_Median_Pct,
                cast(MAX(ABNORMAL_PERIOD)as NUMBER (1,0)) Abnormal_Period,
                CAST(AVG(Num_Lines_Running) as NUMBER(10,2)) Num_Lines_Running, 
                CAST(SUM(AA_Conc_Tons) as NUMBER(10,2)) AA_Conc_Tons, 
                CAST(AVG(AA_Conc_Tons_Last_Day) as NUMBER(10,2)) AA_Conc_Tons_Last_Day

                FROM tolive.conc_analytics_recovery
                    WHERE datetime BETWEEN :StartDate -1/24 AND :EndDate - 1/24 - 1/60/24";


                if (AbnormanPeriodFilter)
                    sql += "\nAND abnormal_period = 0";

                sql+= @" group by trunc(Datetime + 1/24)
                ORDER BY 1";


            List<Conc_Analytics_Recovery> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql, conn);
            cmd.Parameters.Add("StartDate", StartDate);
            cmd.Parameters.Add("EndDate", EndDate);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    elements.Add(DataRowToObject(rdr));
                }
            }
            conn.Close();
            return elements;
        }


        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}datetime {ColPrefix}datetime, {ta}rmf_rate_total_lt {ColPrefix}rmf_rate_total_lt, ");
            cols.AppendLine($"{ta}s12_magfe_median_pct {ColPrefix}s12_magfe_median_pct, ");
            cols.AppendLine($"{ta}s3_magfe_median_pct {ColPrefix}s3_magfe_median_pct, ");
            cols.AppendLine($"{ta}crse_tail_magfe_pct {ColPrefix}crse_tail_magfe_pct, ");
            cols.AppendLine($"{ta}fine_tail_magfe_pct {ColPrefix}fine_tail_magfe_pct, ");
            cols.AppendLine($"{ta}mag_plnt_conc_sio2_pct {ColPrefix}mag_plnt_conc_sio2_pct, ");
            cols.AppendLine($"{ta}rougher_fd_sio2_pct {ColPrefix}rougher_fd_sio2_pct, {ta}conc_sio2_pct {ColPrefix}conc_sio2_pct, ");
            cols.AppendLine($"{ta}float_tail_sio2_pct {ColPrefix}float_tail_sio2_pct, ");
            cols.AppendLine($"{ta}mag_plnt_recov_magfe_pct {ColPrefix}mag_plnt_recov_magfe_pct, ");
            cols.AppendLine($"{ta}flot_recov_magfe_pct {ColPrefix}flot_recov_magfe_pct, ");
            cols.AppendLine($"{ta}tot_recov_magfe_pct {ColPrefix}tot_recov_magfe_pct, ");
            cols.AppendLine($"{ta}mag_plnt_wgt_magfe_pct {ColPrefix}mag_plnt_wgt_magfe_pct, ");
            cols.AppendLine($"{ta}flot_wgt_magfe_pct {ColPrefix}flot_wgt_magfe_pct, ");
            cols.AppendLine($"{ta}tot_wgt_magfe_pct {ColPrefix}tot_wgt_magfe_pct, ");
            cols.AppendLine($"{ta}prev_mag_plnt_recov_magfe_pct {ColPrefix}prev_mag_plnt_recov_magfe_pct, ");
            cols.AppendLine($"{ta}prev_flot_recov_magfe_pct {ColPrefix}prev_flot_recov_magfe_pct, ");
            cols.AppendLine($"{ta}prev_tot_recov_mag_fe_pct {ColPrefix}prev_tot_recov_mag_fe_pct, ");
            cols.AppendLine($"{ta}prev_mag_plnt_wgt_magfe_pct {ColPrefix}prev_mag_plnt_wgt_magfe_pct, ");
            cols.AppendLine($"{ta}prev_flot_wgt_magfe_pct {ColPrefix}prev_flot_wgt_magfe_pct, ");
            cols.AppendLine($"{ta}prev_tot_wgt_magfe_pct {ColPrefix}prev_tot_wgt_magfe_pct, ");
            cols.AppendLine($"{ta}baseline_norm_mag_recov {ColPrefix}baseline_norm_mag_recov, ");
            cols.AppendLine($"{ta}baseline_norm_flot_recov {ColPrefix}baseline_norm_flot_recov, ");
            cols.AppendLine($"{ta}tot_plnt_magfe_median_pct {ColPrefix}tot_plnt_magfe_median_pct, ");
            cols.AppendLine($"{ta}abnormal_period {ColPrefix}abnormal_period, ");
            //cast Num_Lines_Running to a double so that this will match the Object model which is a double.  This is a double as it can be used as an average if querying the average
            cols.AppendLine($"CAST({ta}Num_Lines_Running as NUMBER(10,2)) {ColPrefix}Num_Lines_Running, ");  
            cols.AppendLine($"{ta}AA_Conc_Tons {ColPrefix}AA_Conc_Tons, ");
            cols.AppendLine($"{ta}AA_Conc_Tons_Last_Day {ColPrefix}AA_Conc_Tons_Last_Day");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.conc_analytics_recovery");
            return sql.ToString();
        }



        internal static Conc_Analytics_Recovery DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Conc_Analytics_Recovery RetVal = new();
            RetVal.Datetime = (DateTime)Util.GetRowVal(row, $"{ColPrefix}datetime");
            RetVal.Rmf_Rate_Total_Lt = (double?)Util.GetRowVal(row, $"{ColPrefix}rmf_rate_total_lt");
            RetVal.S12_Magfe_Median_Pct = (double?)Util.GetRowVal(row, $"{ColPrefix}s12_magfe_median_pct");
            RetVal.S3_Magfe_Median_Pct = (double?)Util.GetRowVal(row, $"{ColPrefix}s3_magfe_median_pct");
            RetVal.Crse_Tail_Magfe_Pct = (double?)Util.GetRowVal(row, $"{ColPrefix}crse_tail_magfe_pct");
            RetVal.Fine_Tail_Magfe_Pct = (double?)Util.GetRowVal(row, $"{ColPrefix}fine_tail_magfe_pct");
            RetVal.Mag_Plnt_Conc_Sio2_Pct = (double?)Util.GetRowVal(row, $"{ColPrefix}mag_plnt_conc_sio2_pct");
            RetVal.Rougher_Fd_Sio2_Pct = (double?)Util.GetRowVal(row, $"{ColPrefix}rougher_fd_sio2_pct");
            RetVal.Conc_Sio2_Pct = (double?)Util.GetRowVal(row, $"{ColPrefix}conc_sio2_pct");
            RetVal.Float_Tail_Sio2_Pct = (double?)Util.GetRowVal(row, $"{ColPrefix}float_tail_sio2_pct");
            RetVal.Mag_Plnt_Recov_Magfe_Pct = (double?)Util.GetRowVal(row, $"{ColPrefix}mag_plnt_recov_magfe_pct");
            RetVal.Flot_Recov_Magfe_Pct = (double?)Util.GetRowVal(row, $"{ColPrefix}flot_recov_magfe_pct");
            RetVal.Tot_Recov_Magfe_Pct = (double?)Util.GetRowVal(row, $"{ColPrefix}tot_recov_magfe_pct");
            RetVal.Mag_Plnt_Wgt_Magfe_Pct = (double?)Util.GetRowVal(row, $"{ColPrefix}mag_plnt_wgt_magfe_pct");
            RetVal.Flot_Wgt_Magfe_Pct = (double?)Util.GetRowVal(row, $"{ColPrefix}flot_wgt_magfe_pct");
            RetVal.Tot_Wgt_Magfe_Pct = (double?)Util.GetRowVal(row, $"{ColPrefix}tot_wgt_magfe_pct");
            RetVal.Prev_Mag_Plnt_Recov_Magfe_Pct = (double?)Util.GetRowVal(row, $"{ColPrefix}prev_mag_plnt_recov_magfe_pct");
            RetVal.Prev_Flot_Recov_Magfe_Pct = (double?)Util.GetRowVal(row, $"{ColPrefix}prev_flot_recov_magfe_pct");
            RetVal.Prev_Tot_Recov_Mag_Fe_Pct = (double?)Util.GetRowVal(row, $"{ColPrefix}prev_tot_recov_mag_fe_pct");
            RetVal.Prev_Mag_Plnt_Wgt_Magfe_Pct = (double?)Util.GetRowVal(row, $"{ColPrefix}prev_mag_plnt_wgt_magfe_pct");
            RetVal.Prev_Flot_Wgt_Magfe_Pct = (double?)Util.GetRowVal(row, $"{ColPrefix}prev_flot_wgt_magfe_pct");
            RetVal.Prev_Tot_Wgt_Magfe_Pct = (double?)Util.GetRowVal(row, $"{ColPrefix}prev_tot_wgt_magfe_pct");
            RetVal.Baseline_Norm_Mag_Recov = (double?)Util.GetRowVal(row, $"{ColPrefix}baseline_norm_mag_recov");
            RetVal.Baseline_Norm_Flot_Recov = (double?)Util.GetRowVal(row, $"{ColPrefix}baseline_norm_flot_recov");
            RetVal.Tot_Plnt_Magfe_Median_Pct = (double?)Util.GetRowVal(row, $"{ColPrefix}tot_plnt_magfe_median_pct");
            RetVal.Abnormal_Period = (short)Util.GetRowVal(row, $"{ColPrefix}abnormal_period") == 1;
            RetVal.Num_Lines_Running = (double?)Util.GetRowVal(row, $"{ColPrefix}Num_Lines_Running");
            RetVal.AA_Conc_Tons = (double?)Util.GetRowVal(row, $"{ColPrefix}AA_Conc_Tons");
            RetVal.AA_Conc_Tons_Last_Day = (double?)Util.GetRowVal(row, $"{ColPrefix}AA_Conc_Tons_Last_Day");
            return RetVal;
        }

    }
}
