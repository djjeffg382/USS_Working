using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.DAL.Wenco.Models;

namespace MOO.DAL.Wenco.Services
{
    public static class HaulCycleQualitySummarySvc
    {
        static HaulCycleQualitySummarySvc()
        {
            Util.RegisterSqlServer();
        }


        public static HaulCycleQualitySummary GetByShift(MOO.Plant Plant, DateTime ShiftDate, byte Shift)
        {
            MOO.Data.MNODatabase wencoDB;
            if (Plant == Plant.Minntac)
                wencoDB = Data.MNODatabase.MTC_Wenco;
            else
                throw new NotImplementedException("Keetac not implemented yet");

            StringBuilder sql = new();
            sql.AppendLine(GetSelect());
            sql.AppendLine($"  WHERE dump_end_shift_date ='{ShiftDate}' AND DUMP_END_SHIFT_IDENT = {Shift}");
            sql.AppendLine("  AND Material_Ident IN ('O20', 'O25')");
            sql.AppendLine("  GROUP BY dump_end_shift_date, DUMP_END_SHIFT_IDENT, hcgt.QUALITY_CODE");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), wencoDB);
            HaulCycleQualitySummary retVal = new()
            {
                ShiftDate = ShiftDate,
                Shift = Shift
            };
            foreach(DataRow dr in ds.Tables[0].Rows)
            {
                retVal.QualityVals.Add(dr.Field<string>("quality_code").ToUpper(), dr.Field<decimal>("QualityVal"));
            }
            return retVal;
        }




        private static string GetSelect()
        {
            StringBuilder sql = new StringBuilder();

            sql.AppendLine("  SELECT dump_end_shift_date ShiftDate, DUMP_END_SHIFT_IDENT Shift, hcgt.QUALITY_CODE,");
            sql.AppendLine("	SUM(hcgt.quality_value * hct.QUANTITY_REPORTING)/ SUM(hct.QUANTITY_REPORTING) QualityVal");
            sql.AppendLine("  FROM HAUL_CYCLE_TRANS hct");
            sql.AppendLine("  INNER JOIN HAUL_CYCLE_GRADE_TRANS hcgt");
            sql.AppendLine("	ON hct.HAUL_CYCLE_REC_IDENT = hcgt.HAUL_CYCLE_REC_IDENT");


            return sql.ToString();
        }

    }
}
