using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.DAL.Wenco.Models;

namespace MOO.DAL.Wenco.Services
{
    public static class HaulCycleMaterialSummarySvc
    {
        static HaulCycleMaterialSummarySvc()
        {
            Util.RegisterSqlServer();
        }


        public static HaulCycleMaterialSummary GetByShift(MOO.Plant Plant, DateTime ShiftDate, byte Shift)
        {
            MOO.Data.MNODatabase wencoDB;
            if (Plant == Plant.Minntac)
                wencoDB = Data.MNODatabase.MTC_Wenco;
            else
                throw new NotImplementedException("Keetac not implemented yet");

            StringBuilder sql = new();
            sql.AppendLine(GetSelect());
            sql.AppendLine($"WHERE dump_end_shift_date = '{ShiftDate:MM/dd/yyyy}'");
            sql.AppendLine($"AND dump_end_shift_ident = {Shift}");
            sql.AppendLine("GROUP BY dump_end_shift_date, DUMP_END_SHIFT_IDENT");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), wencoDB);
            if (ds.Tables[0].Rows.Count > 0)
                return DataRowToObj(ds.Tables[0].Rows[0]);
            else
                return new HaulCycleMaterialSummary()
                {
                    ShiftDate = ShiftDate,
                    Shift = Shift
                };
        }

        public static List<HaulCycleMaterialSummary> GetByDateRange(MOO.Plant Plant, DateTime StartDate, DateTime EndDate)
        {
            MOO.Data.MNODatabase wencoDB;
            if (Plant == Plant.Minntac)
                wencoDB = Data.MNODatabase.MTC_Wenco;
            else
                throw new NotImplementedException("Keetac not implemented yet");

            List<HaulCycleMaterialSummary> retVal = new();

            StringBuilder sql = new();
            sql.AppendLine(GetSelect());
            sql.AppendLine($"WHERE dump_end_shift_date BETWEEN '{StartDate:MM/dd/yyyy}' AND '{EndDate:MM/dd/yyyy}'");
            sql.AppendLine("GROUP BY dump_end_shift_date, DUMP_END_SHIFT_IDENT");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), wencoDB);
            foreach(DataRow dr in ds.Tables[0].Rows)
            {
                retVal.Add(DataRowToObj(dr));
            }
            return retVal;
        }


        private static string GetSelect()
        {
            StringBuilder sql = new();

            sql.AppendLine("SELECT dump_end_shift_date ShiftDate, DUMP_END_SHIFT_IDENT Shift,");
            sql.AppendLine("	SUM(CASE WHEN Material_Ident = 'O21' THEN quantity_reporting ELSE 0 END) OreStocked,");
            sql.AppendLine("	SUM(CASE WHEN Material_Ident = 'O25' THEN quantity_reporting ELSE 0 END) OreDestocked,	");
            sql.AppendLine("	SUM(CASE WHEN Material_Ident = 'O20' THEN quantity_reporting ELSE 0 END) OreDirect,");
            sql.AppendLine("	SUM(CASE WHEN Material_Ident = 'W24' THEN quantity_reporting ELSE 0 END) OMH,");
            sql.AppendLine("	SUM(CASE WHEN Material_Ident = 'W50' THEN quantity_reporting ELSE 0 END) Surface,");
            sql.AppendLine("	SUM(CASE WHEN Material_Ident = 'W40' THEN quantity_reporting ELSE 0 END) Waste");
            sql.AppendLine("FROM HAUL_CYCLE_TRANS");

            return sql.ToString();
        }

        private static HaulCycleMaterialSummary DataRowToObj(DataRow dr)
        {
            HaulCycleMaterialSummary retVal = new();

            retVal.ShiftDate = dr.Field<DateTime>("ShiftDate");
            retVal.Shift = byte.Parse(dr.Field<string>("shift"));
            retVal.OreStocked = dr.Field<decimal>("OreStocked");
            retVal.OreDestocked = dr.Field<decimal>("OreDestocked");
            retVal.OreDirect = dr.Field<decimal>("OreDirect");
            retVal.Omh = dr.Field<decimal>("OMH");
            retVal.Surface = dr.Field<decimal>("Surface");
            retVal.Waste = dr.Field<decimal>("Waste");

            return retVal;
        }
    }
}
