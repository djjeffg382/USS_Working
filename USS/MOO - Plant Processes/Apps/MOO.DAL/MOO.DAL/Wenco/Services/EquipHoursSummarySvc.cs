using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.DAL.Wenco.Models;

namespace MOO.DAL.Wenco.Services
{
    public static class EquipHoursSummarySvc
    {
        static EquipHoursSummarySvc()
        {
            Util.RegisterSqlServer();
        }

        /// <summary>
        /// Equipment Type from the 
        /// </summary>
        /// <param name="EqpType">array of equip type strings see wenco Equip_Type.Eqtype_Ident table</param>
        /// <param name="ShiftDate"></param>
        /// <param name="Shift"></param>
        public static EquipHoursSummary GetByEquipTypeAndShift(MOO.Plant Plant, string[] EqpType, DateTime ShiftDate, byte Shift)
        {
            MOO.Data.MNODatabase wencoDB;
            if (Plant == Plant.Minntac)
                wencoDB = Data.MNODatabase.MTC_Wenco;
            else
                wencoDB = Data.MNODatabase.KTC_Wenco;

            StringBuilder sql = new();
            sql.AppendLine(GetSelect());
            StringBuilder eqType = new();
            foreach(string e in EqpType)
            {
                if (eqType.Length > 0)
                    eqType.Append(",");
                eqType.Append($"'{e}'");
            }

            sql.AppendLine($"WHERE e.EQP_TYPE IN ({eqType})");
            sql.AppendLine($"AND Shift_Date = '{ShiftDate:MM/dd/yyyy}'");
            sql.AppendLine($"AND SHIFT_IDENT = {Shift}");
            sql.AppendLine("GROUP BY Shift_Date, SHIFT_IDENT");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), wencoDB);
            if (ds.Tables[0].Rows.Count > 0)
                return DataRowToObj(ds.Tables[0].Rows[0]);
            else
                return new EquipHoursSummary()
                {
                    ShiftDate = ShiftDate,
                    Shift = Shift
                };
        }

        private static string GetSelect(string AdditionalFields = "")
        {
            StringBuilder sql = new();



            sql.AppendLine($"SELECT Shift_Date ShiftDate, SHIFT_IDENT shift,");
            sql.AppendLine("	COALESCE(CAST(SUM(CASE WHEN SUBSTRING(status_code,1,1) = 'N' THEN DATEDIFF(s,Start_Timestamp, End_Timestamp) ELSE 0 END) AS float)/3600, 0) Oper,");
            sql.AppendLine("	COALESCE(CAST(SUM(CASE WHEN SUBSTRING(status_code,1,1) = 'O' THEN DATEDIFF(s,Start_Timestamp, End_Timestamp) ELSE 0 END) AS float)/3600, 0) OperDelay,");
            sql.AppendLine("	COALESCE(CAST(SUM(CASE WHEN SUBSTRING(status_code,1,1) = 'S' THEN DATEDIFF(s,Start_Timestamp, End_Timestamp) ELSE 0 END) AS float)/3600, 0) Standby,");
            sql.AppendLine("	COALESCE(CAST(SUM(CASE WHEN (SUBSTRING(status_code,1,1) = 'M' AND status_code NOT IN ('M58', 'M56')) THEN DATEDIFF(s,Start_Timestamp, End_Timestamp) ELSE 0 END) AS float)/3600, 0) Maint,");
            sql.AppendLine("	COALESCE(CAST(SUM(CASE WHEN status_code = 'M58' THEN DATEDIFF(s,Start_Timestamp, End_Timestamp) ELSE 0 END) AS float)/3600, 0) MgmtDec,");
            sql.AppendLine("	COALESCE(CAST(SUM(CASE WHEN status_code = 'M56' THEN DATEDIFF(s,Start_Timestamp, End_Timestamp) ELSE 0 END) AS float)/3600, 0) SchedDown");
            if (!string.IsNullOrEmpty(AdditionalFields))
            {
                sql.Append(",");
                sql.AppendLine(AdditionalFields);
            }
            sql.AppendLine("FROM equipment_status_trans est");
            sql.AppendLine("INNER JOIN EQUIP e");
            sql.AppendLine("	ON est.equip_ident = e.equip_ident");


            return sql.ToString();
        }

        private static EquipHoursSummary DataRowToObj(DataRow dr)
        {
            EquipHoursSummary retVal = new();

            retVal.ShiftDate = dr.Field<DateTime>("ShiftDate");
            retVal.Shift = byte.Parse(dr.Field<string>("shift"));
            retVal.OperatingHrs = Math.Round(Convert.ToDecimal(dr.Field<double>("Oper")),2,MidpointRounding.AwayFromZero);
            retVal.OperatingDelayHrs = Math.Round(Convert.ToDecimal(dr.Field<double>("OperDelay")), 2, MidpointRounding.AwayFromZero);
            retVal.StandbyHrs = Math.Round(Convert.ToDecimal(dr.Field<double>("Standby")), 2, MidpointRounding.AwayFromZero);
            retVal.MaintenanceHrs = Math.Round(Convert.ToDecimal(dr.Field<double>("Maint")), 2, MidpointRounding.AwayFromZero);
            retVal.MgmtDecisionHrs = Math.Round(Convert.ToDecimal(dr.Field<double>("MgmtDec")), 2, MidpointRounding.AwayFromZero);
            retVal.SchedDownHrs = Math.Round(Convert.ToDecimal(dr.Field<double>("SchedDown")), 2, MidpointRounding.AwayFromZero);

            return retVal;

        }

    }
}
