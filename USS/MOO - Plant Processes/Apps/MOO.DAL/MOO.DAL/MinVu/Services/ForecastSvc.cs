using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.DAL.MinVu.Models;

namespace MOO.DAL.MinVu.Services
{
    public class ForecastSvc
    {
        static ForecastSvc()
        {
            Util.RegisterSqlServer();
        }
        public static Forecast GetByShift(MOO.Plant Plant, DateTime ShiftDate, byte Shift, int[] TagIds)
        {
            List<Forecast> vals = GetByDateRange(Plant, ShiftDate, Shift, ShiftDate, Shift, TagIds);
            if (vals.Count > 0)
                return vals[0];
            else
                return new Forecast()
                {
                    ShiftDate = ShiftDate,
                    Shift = Shift
                };
        }

        public static List<Forecast> GetByDateRange(MOO.Plant Plant, DateTime StartDate, byte StartShift, DateTime EndDate, byte EndShift, int[] TagIds)
        {
            List<Forecast> retVal = new();

            StringBuilder sql = new();

            string db;
            MOO.Data.MNODatabase MinVuDB;

            if (Plant == Plant.Keetac)
            {
                db = "ODSKTCProd";
                MinVuDB = Data.MNODatabase.KTC_Minvu;
            }
            else
            {
                db = "ODSMTCProd";
                MinVuDB = Data.MNODatabase.MTC_MinVu;
            }
                
            string startShiftIdx = StartDate.ToString("yyyyMMdd") + StartShift;
            string endShiftIdx = EndDate.ToString("yyyyMMdd") + EndShift;

            StringBuilder tags = new();
            foreach(int tag in TagIds)
            {
                if (tags.Length > 0)
                    tags.Append(',');
                tags.Append(tag);
            }


            sql.AppendLine("SELECT CAST(CONVERT(CHAR(10), CONVERT(datetime, SUBSTRING(CAST(pv.FromShiftIndex as VARCHAR(10)),1,8)), 120) AS DATE) ShiftDate, ");
            sql.AppendLine("		SUBSTRING(CAST(pv.FromShiftIndex as VARCHAR(10)),9,1) Shift,");
            sql.AppendLine("		ROUND(SUM(pv.TagValue), 2) ForecastValue");
            sql.AppendLine($"  FROM {db}.dbo.Plan_Values pv");
            sql.AppendLine($"INNER JOIN {db}.dbo.Plan_Plans pp");
            sql.AppendLine("    ON pv.PlanID = pp.planid");
            sql.AppendLine("WHERE pv.record_exists = 'Y'");
            sql.AppendLine($"   AND pv.tagid IN ({tags})");
            sql.AppendLine("    AND pp.PeriodIncrement_enum = 1");
            sql.AppendLine($"   AND pv.FromShiftIndex BETWEEN {startShiftIdx} AND {endShiftIdx}");
            sql.AppendLine("GROUP BY CAST(CONVERT(CHAR(10), CONVERT(datetime, SUBSTRING(CAST(pv.FromShiftIndex as VARCHAR(10)),1,8)), 120) AS DATE), ");
            sql.AppendLine("		SUBSTRING(CAST(pv.FromShiftIndex as VARCHAR(10)),9,1)");
            sql.AppendLine("ORDER BY 1,2");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), MinVuDB);
            foreach(DataRow row in ds.Tables[0].Rows)
            {
                retVal.Add(DataRowToObj(row));
            }
            return retVal; 
        }

        private static Forecast DataRowToObj(DataRow dr)
        {
            Forecast retVal = new();
            retVal.ShiftDate = dr.Field<DateTime>("ShiftDate");
            retVal.Shift = byte.Parse( dr.Field<string>("shift"));
            retVal.ForecastValue = dr.Field<double>("ForecastValue");
            return retVal;
        }
    }
}
