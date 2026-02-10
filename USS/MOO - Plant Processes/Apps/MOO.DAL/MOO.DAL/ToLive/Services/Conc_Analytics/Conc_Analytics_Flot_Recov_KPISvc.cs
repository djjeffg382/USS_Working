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
    public static class Conc_Analytics_Flot_Recov_KPISvc
    {
        static Conc_Analytics_Flot_Recov_KPISvc()
        {
            Util.RegisterOracle();
        }


        public static Conc_Analytics_Flot_Recov_KPI GetLatest()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT * FROM (");
            sql.Append(GetSelect());
            sql.AppendLine(") WHERE rn = 1");


            Conc_Analytics_Flot_Recov_KPI retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }


        public static List<Conc_Analytics_Flot_Recov_KPI> GetByDate(DateTime StartDate, DateTime EndDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE datetime BETWEEN :StartDate AND :EndDate");

            List<Conc_Analytics_Flot_Recov_KPI> elements = [];
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("StartDate", StartDate);
            cmd.Parameters.Add("EndDate", EndDate);
            cmd.BindByName = true;
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


   
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ROW_NUMBER() OVER(ORDER BY datetime desc) rn,");  //include rownumber so we can get latests
            sql.AppendLine($" datetime, total_rmf, flot_rec, ff_sp, ");
            sql.AppendLine($"flot_base_cor, flot_base_norm, ");
            sql.AppendLine($"mag_act, coil, mag_base_cor, mag_base_norm");
            sql.AppendLine("FROM tolive.conc_analytics_flot_recov_kpi");
            return sql.ToString();
        }



        internal static Conc_Analytics_Flot_Recov_KPI DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Conc_Analytics_Flot_Recov_KPI RetVal = new();
            RetVal.Datetime = (DateTime)Util.GetRowVal(row, $"{ColPrefix}datetime");
            RetVal.Total_Rmf = (double?)Util.GetRowVal(row, $"{ColPrefix}total_rmf");
            RetVal.Flot_Rec = (float?)Util.GetRowVal(row, $"{ColPrefix}flot_rec");
            RetVal.Ff_Sp = (float?)Util.GetRowVal(row, $"{ColPrefix}ff_sp");
            RetVal.Flot_Base_Cor = (float?)Util.GetRowVal(row, $"{ColPrefix}flot_base_cor");
            RetVal.Flot_Base_Norm = (float?)Util.GetRowVal(row, $"{ColPrefix}flot_base_norm");
            RetVal.Mag_Act = (float?)Util.GetRowVal(row, $"{ColPrefix}mag_act");
            RetVal.Coil = (float?)Util.GetRowVal(row, $"{ColPrefix}coil");
            RetVal.Mag_Base_Cor = (float?)Util.GetRowVal(row, $"{ColPrefix}mag_base_cor");
            RetVal.Mag_Base_Norm = (float?)Util.GetRowVal(row, $"{ColPrefix}mag_base_norm");
            return RetVal;
        }

    }
}
