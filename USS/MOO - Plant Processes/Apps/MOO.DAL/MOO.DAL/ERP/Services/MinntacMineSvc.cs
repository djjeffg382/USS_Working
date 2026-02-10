using Microsoft.Data.SqlClient;
using MOO.DAL.ERP.Enums;
using MOO.DAL.ERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ERP.Services
{
    public static class MinntacMineSvc
    {


        /// <summary>
        /// Gets Crusher Production Forecast per day
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPValues> GetCrushedProductionForecastValues(DateTime StartDate, DateTime EndDate)
        {
            return ERPValues.GetMinVuPlansByShiftDate(Plant.Minntac, StartDate, EndDate, new int[] { 533, 690 });
        }

        /// <summary>
        /// Gets the ERP Messages for the Crusher Production Forecast per day
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPMessage> GetCrushedProductionForecastMessages(DateTime StartDate, DateTime EndDate)
        {
            var data = GetCrushedProductionForecastValues(StartDate, EndDate);

            return ERPMessage.DataValuesToERPMessages("Mine Crude Forecast",data, Plant.Minntac, TimePeriod.Daily, ProcessCode.XCR,
                                    Material.CrushRock, Activity.CRUSHING, 0, "LT", "CALC", "CRUSE");
        }


        /// <summary>
        /// Gets Crusher Production Forecast per day
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPValues> GetWasteForecastValues(DateTime StartDate, DateTime EndDate)
        {
            return ERPValues.GetMinVuPlansByShiftDate(Plant.Minntac, StartDate, EndDate, new int[] { 713, 717, 718 });
        }

        /// <summary>
        /// Gets the ERP Messages for the Crusher Production Forecast per day
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPMessage> GetWasteForecastMessages(DateTime StartDate, DateTime EndDate)
        {
            var data = GetWasteForecastValues(StartDate, EndDate);

            return ERPMessage.DataValuesToERPMessages("Mine Waste Forecast", data, Plant.Minntac, TimePeriod.Daily, ProcessCode.XCR,
                                    Material.CrushRock, Activity.CRUSHING, 2, "LT", "CALC", "WSTEE");
        }



        /// <summary>
        /// Gets the Values for the dumps into the crushers
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPValues> GetMineProductionValues(DateTime StartDate, DateTime EndDate)
        {
            List<ERPValues> retVal = new();
            StringBuilder sql = new();

            sql.AppendLine("select Convert(Datetime,cast(m.shiftyyyymmdd as char(8)),112) as prodDate, SUM(m.qty) weight");
            sql.AppendLine("FROM vwMaterial_Movement_By_Cycle m");
            sql.AppendLine("where m.RECORD_EXISTS='Y'");
            sql.AppendLine($"and m.shiftindex BETWEEN {StartDate:yyyyMMdd}1 AND {EndDate:yyyyMMdd}2");
            sql.AppendLine("AND m.MatMovement_CostCode IN ('OREDIRECT', 'OREDESTOCK')");
            sql.AppendLine("group by m.shiftyyyymmdd");
            sql.AppendLine("ORDER by m.shiftyyyymmdd");

            using SqlConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.MTC_MinVu));
            conn.Open();
            SqlCommand cmd = new(sql.ToString(), conn);
            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    retVal.Add(new ERPValues() { Shift_Date = rdr.GetDateTime(0), Value = rdr.GetDecimal(1), });
                }
            }
            conn.Close();
            return retVal;
        }

        /// <summary>
        /// Gets the ERP Messages for the dumps into the crushers
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPMessage> GetMineProductionMessages(DateTime StartDate, DateTime EndDate)
        {
            var data = GetMineProductionValues(StartDate, EndDate);

            return ERPMessage.DataValuesToERPMessages("Mine Crude Production", data, Plant.Minntac, TimePeriod.Daily, ProcessCode.XCR,
                                    Material.CrushRock, Activity.CRUSHING, 0, "LT", "CALC", "CRUSD");
        }



        /// <summary>
        /// Gets the Values for the dumps into the crushers
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPValues> GetMineWasteValues(DateTime StartDate, DateTime EndDate)
        {
            List<ERPValues> retVal = new();
            StringBuilder sql = new();

            sql.AppendLine("select Convert(Datetime,cast(m.shiftyyyymmdd as char(8)),112) as prodDate, SUM(m.qty) weight");
            sql.AppendLine("FROM Material_Movement_Loads m");
            sql.AppendLine("where m.RECORD_EXISTS='Y'");
            sql.AppendLine($"and m.shiftindex BETWEEN {StartDate:yyyyMMdd}1 AND {EndDate:yyyyMMdd}2");
            sql.AppendLine("AND m.MatMovement_abbrev IN ('WASTEDUMPED', 'SURFACEDUMPED', 'SURFACEWASTED')");
            sql.AppendLine("group by m.shiftyyyymmdd");
            sql.AppendLine("ORDER by m.shiftyyyymmdd");

            using SqlConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.MTC_MinVu));
            conn.Open();
            SqlCommand cmd = new(sql.ToString(), conn);
            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    retVal.Add(new ERPValues() { Shift_Date = rdr.GetDateTime(0), Value = rdr.GetDecimal(1), });
                }
            }
            conn.Close();
            return retVal;
        }



        /// <summary>
        /// Gets the ERP Messages for Mine Waste data
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPMessage> GetMineWasteMessages(DateTime StartDate, DateTime EndDate)
        {
            var data = GetMineWasteValues(StartDate, EndDate);

            return ERPMessage.DataValuesToERPMessages("Mine Waste Production", data, Plant.Minntac, TimePeriod.Daily, ProcessCode.XCR,
                                    Material.CrushRock, Activity.CRUSHING, 0, "LT", "CALC", "WSTED");
        }




        /// <summary>
        /// Gets values by lease and day for what was dumped into the crushers
        /// </summary>
        /// <param name="MonthDate">Can be any day in the month where we are getting the production month total</param>
        /// <returns></returns>
        public static List<ERPLeaseValues> GetCrusherConsumptionValues(DateTime MonthDate)
        {
            DateTime startOfMonth = MOO.Dates.FirstDayOfMonth(MonthDate);
            DateTime endOfMonth = MOO.Dates.LastDayOfMonth(MonthDate);

            List<ERPLeaseValues> retVal = new();
            StringBuilder sqlQuery = new();

            sqlQuery.AppendLine("WITH RawData AS");
            sqlQuery.AppendLine("(");
            sqlQuery.AppendLine("Select SurvAdjQty");
            sqlQuery.AppendLine(", shiftindex");
            sqlQuery.AppendLine(", MatMovement_enum AS MaterialMovementEnum");
            sqlQuery.AppendLine(", MatMovement_abbrev AS MaterialMovementAbbrev");
            sqlQuery.AppendLine(", PropertyCode");
            sqlQuery.AppendLine(", PropertyGrpCode");
            sqlQuery.AppendLine(" , Lease");
            sqlQuery.AppendLine(" , TimeEmpty");
            sqlQuery.AppendLine(", LeaseGrp");
            sqlQuery.AppendLine(", dumploc");
            sqlQuery.AppendLine(" FROM ODSMTCProd.dbo.vwMaterial_Movement_By_Cycle");
            sqlQuery.AppendLine($" WHERE(shiftindex >= {startOfMonth: yyyyMMdd}1 And shiftindex <= {endOfMonth:yyyyMMdd}2)");
            sqlQuery.AppendLine(" AND    MatMovement_enum in (1, 2) ");
            sqlQuery.AppendLine("AND    RECORD_EXISTS = 'Y'");
            sqlQuery.AppendLine("),");
            sqlQuery.AppendLine("ProjectedData AS");
            sqlQuery.AppendLine("(");
            sqlQuery.AppendLine("Select DATEADD(Day, DATEDIFF(Day, 0, TimeEmpty), 0) as prodDate, ");
            sqlQuery.AppendLine("mat.MaterialMovementEnum, mat.MaterialMovementAbbrev");
            sqlQuery.AppendLine(", CASE WHEN mat.PropertyCode = '-1' THEN 0 ELSE COALESCE(mat.PropertyCode, '0') END AS Line");
            sqlQuery.AppendLine(", COALESCE(mat.PropertyCode, '0') AS PropertyCode");
            sqlQuery.AppendLine(", COALESCE(mat.Lease, '01') AS LeaseCode");
            sqlQuery.AppendLine(", CASE WHEN mat.PropertyCode = '-1' THEN  'USS1 STOCK FEE' ELSE LeaseName END as LeaseName");
            sqlQuery.AppendLine(" , COALESCE(LessorGroupCode, '0') AS LessorGroupCode");
            sqlQuery.AppendLine(", COALESCE(LessorGroupname, 'USS1 STOCK') AS LessorGroupName");
            sqlQuery.AppendLine(", mat.SurvAdjQty as [Weight]");
            sqlQuery.AppendLine("FROM   RawData AS mat ");
            sqlQuery.AppendLine("LEFT OUTER JOIN ( ");
            sqlQuery.AppendLine(" SELECT l.LeaseCode AS LeaseCode, o.FromShiftindex, o.ToShiftindex, l.Name AS LeaseName");
            sqlQuery.AppendLine(", o.LessorGroupID AS LessorGroupCode, g.Name AS LessorGroupName ");
            sqlQuery.AppendLine("FROM   MTC_LeaseReferences.dbo.Leases AS l");
            sqlQuery.AppendLine("INNER JOIN MTC_LeaseReferences.dbo.Lease_Ownership AS o on l.LeaseCode = o.LeaseCode ");
            sqlQuery.AppendLine("INNER JOIN MTC_LeaseReferences.dbo.Lessor_Groups   AS g on o.LessorGroupID = g.LessorGroupID");
            sqlQuery.AppendLine(" WHERE  o.IsPrimaryGroupForLease = 'Y'");
            sqlQuery.AppendLine(") AS leases");
            sqlQuery.AppendLine("ON mat.Lease = leases.LeaseCode COLLATE DATABASE_DEFAULT AND mat.shiftindex >= leases.FromShiftindex AND mat.shiftindex <= leases.ToShiftindex");
            sqlQuery.AppendLine(")");

            sqlQuery.AppendLine("Select prodDate, ");
            sqlQuery.AppendLine(" PropertyCode, LeaseCode, LeaseName");
            sqlQuery.AppendLine(", LessorGroupCode, LessorGroupName");
            sqlQuery.AppendLine(", ROUND(SUM([Weight]), 2) as [Weight]");
            sqlQuery.AppendLine(" FROM ProjectedData");
            sqlQuery.AppendLine("GROUP BY MaterialMovementEnum, MaterialMovementAbbrev");
            sqlQuery.AppendLine(", Line, PropertyCode");
            sqlQuery.AppendLine(" , LeaseCode, LeaseName,prodDate, LessorGroupCode, LessorGroupName");

            using SqlConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.MTC_MinVu));
            conn.Open();
            SqlCommand cmd = new(sqlQuery.ToString(), conn);
            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    retVal.Add(new ERPLeaseValues()
                    {
                        Shift_Date = rdr.GetDateTime(0),
                        PropertyCode = rdr.GetString(1),
                        LeaseCode = rdr.GetString(2),
                        LeaseName = rdr.GetString(3),
                        LessorGroupCode = rdr.GetInt32(4),
                        LessorGroupName = rdr.GetString(5),
                        Value = (decimal)rdr.GetDouble(6)
                    });
                }
            }
            conn.Close();
            return retVal;
        }




        /// <summary>
        /// Gets the ERP Messages for the Crusher Month End Consumption
        /// </summary>
        /// <param name="MonthDate">Can be any day in the month where we are getting the production month total</param>
        /// <returns></returns>
        /// <remarks>This will include RAW Records for all of the leases.  Original design had ERP using this data but I don't believe this is used in ERP but I don't know if we can just not send it</remarks>
        public static ERPMessage GetMineMonthProductionMessage(DateTime MonthDate)
        {
            DateTime endOfMonth = MOO.Dates.LastDayOfMonth(MonthDate);

            ///data will be provided by date and lease
            var crushed = GetCrusherConsumptionValues(endOfMonth);
            var totTons = crushed.Sum(x => x.Value);

            MSTRec mst = new(MOO.Plant.Minntac, endOfMonth, endOfMonth, TimePeriod.Monthly);
            MPARec mpa = new("RMF", ProcessCode.XCR, Material.CrushRock, Activity.CRUSHING,
                            Math.Round(totTons, 2).ToString(), "LT", "ACT");

            var leaseGrp = (from crush in crushed
                            group crush by new { crush.PropertyCode, crush.LeaseCode, crush.LeaseName, crush.LessorGroupCode, crush.LessorGroupName } into grp
                            orderby grp.Sum(x => x.Value) descending
                            select new ERPLeaseValues()
                            {
                                Shift_Date = endOfMonth,
                                PropertyCode = grp.Key.PropertyCode,
                                LeaseCode = grp.Key.LeaseCode,
                                LeaseName = grp.Key.LeaseName,
                                LessorGroupCode = grp.Key.LessorGroupCode,
                                LessorGroupName = grp.Key.LessorGroupName,
                                Value = grp.Sum(x => x.Value)
                            }
                           ).ToList();
            List<RAWRec> rawRecs = new List<RAWRec>();
            foreach (var lse in leaseGrp)
            {
                RAWRec raw = new RAWRec(Material.Taconite, Math.Round(lse.Value, 2).ToString(), "LT", "ACT", lse.PropertyCode, lse.LeaseName, lse.LeaseCode, lse.LeaseName, lse.LessorGroupCode, lse.LessorGroupName);

                rawRecs.Add(raw);

            }

            ERPMessage retVal = new("Mine Month End Production/Consumption",mst, mpa, rawRecs);

            return retVal;
        }

    }
}
