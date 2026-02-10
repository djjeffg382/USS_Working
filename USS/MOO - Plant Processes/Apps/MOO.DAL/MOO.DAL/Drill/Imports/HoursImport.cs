using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Drill.Imports
{
    /// <summary>
    /// class that will be used for importing data from Wenco system into the drilled_hole_status table and the equip_hours table for reporting
    /// </summary>
    public class HoursImport
    {
        public readonly MOO.Plant Plant;
        private static bool OLEDBIsRegistered = false;
        public HoursImport(MOO.Plant Plant)
        {
            this.Plant = Plant;
            if (!OLEDBIsRegistered)
            {
                DbProviderFactories.RegisterFactory(MOO.Data.DBType.SQLServer.ToString(), Microsoft.Data.SqlClient.SqlClientFactory.Instance);
                OLEDBIsRegistered = true;
            }

        }

        /// <summary>
        /// Imports the data from Wenco status to the Drilled_Hole_Status table
        /// </summary>
        /// <param name="ShiftDate"></param>
        /// <param name="Shift"></param>
        /// <remarks>This loops through the data and applies a drilled hole to each of the statuses, this will give NOH, GOH per hole for reporting</remarks>
        public void ImportDrilledHoleStatus(DateTime ShiftDate, byte Shift)
        {
            Services.Drilled_Hole_StatusSvc.DeleteShift(Plant, ShiftDate, Shift);
            List<Models.Equip> eqList = Services.EquipSvc.GetAll(Plant);
            StringBuilder sql = new();

            sql.AppendLine("SELECT e.Equip_Ident,Start_Timestamp,End_Timestamp, status_code");
            sql.AppendLine("FROM equipment_status_trans est");
            sql.AppendLine("INNER JOIN EQUIP e");
            sql.AppendLine("	ON est.equip_ident = e.equip_ident");
            sql.AppendLine("WHERE e.EQP_TYPE='DRL'");
            sql.AppendLine($"AND Shift_Date = '{ShiftDate:MM/dd/yyyy}'");
            sql.AppendLine($"AND SHIFT_IDENT = {Shift}");
            sql.AppendLine("AND End_Timestamp IS NOT NULL");
            sql.AppendLine("ORDER BY e.equip_ident, start_timestamp");

            DataSet ds;
            MOO.Data.MNODatabase db = Data.MNODatabase.MTC_Wenco;
            if (Plant == Plant.Keetac)
                db = Data.MNODatabase.KTC_Wenco;

            ds = MOO.Data.ExecuteQuery(sql.ToString(), db);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Models.Equip eq = eqList.FirstOrDefault(x => x.Equip_Number == dr.Field<string>("equip_ident"));
                if (eq != null)
                {
                    Models.Drilled_Hole_Status dhs = new()
                    {
                        Plant = Plant,
                        Shift_Date = ShiftDate,
                        Shift = Shift,
                        Start_Time = dr.Field<DateTime>("start_timestamp"),
                        End_Time = dr.Field<DateTime>("end_timestamp"),
                        StatusCode = dr.Field<string>("status_code")
                    };

                    string statusCode = dr.Field<string>("status_code");
                    if (statusCode[..1] == "N")
                        dhs.StatusBucket = Models.Drilled_Hole_Status.StatusType.Operating;
                    else if (statusCode[..1] == "O")
                        dhs.StatusBucket = Models.Drilled_Hole_Status.StatusType.OpDelay;
                    else if (statusCode[..1] == "S")
                        dhs.StatusBucket = Models.Drilled_Hole_Status.StatusType.Standby;
                    else if (statusCode[..1] == "M" && statusCode != "M58" && statusCode != "M56")
                        dhs.StatusBucket = Models.Drilled_Hole_Status.StatusType.Maint;
                    else if (statusCode == "M58")
                        dhs.StatusBucket = Models.Drilled_Hole_Status.StatusType.MgmtDec;
                    else if (statusCode == "M56")
                        dhs.StatusBucket = Models.Drilled_Hole_Status.StatusType.SchedDown;
                    dhs.Drilled_Hole_Id = GetDrilledHoleIDForStatus(eq, dhs.Start_Time);
                    if(dhs.Drilled_Hole_Id >= 0)
                    {
                        //hole was found for this status,
                        try
                        {
                            Services.Drilled_Hole_StatusSvc.Insert(dhs);
                        }
                        catch (Exception)
                        {
                            //weird situation can occur in wenco where 2 statuses have same start date,
                            //Drilled_Hole_Status table doesn't allow this.
                            //add a half second to the time and insert again
                            //ideally we need to figure out which one is correct but this is difficult to 
                            //figure out in code so we will just import both as this is a rare situation
                            dhs.Start_Time = dhs.Start_Time.AddMilliseconds(500);
                            Services.Drilled_Hole_StatusSvc.Insert(dhs);
                        }
                        
                    }

                }
            }
        }

        /// <summary>
        /// finds the drilled hole id for the specified status
        /// </summary>
        /// <param name="Eq"></param>
        /// <param name="Status_Start_Time"></param>
        /// <returns></returns>
        private static int GetDrilledHoleIDForStatus(Models.Equip Eq, DateTime Status_Start_Time)
        {
            StringBuilder sql = new();

            sql.AppendLine("select top 1 drilled_hole_id");
            sql.AppendLine("FROM Drill.dbo.Drilled_Hole");
            sql.AppendLine("WHERE start_date < @Start_Time");
            sql.AppendLine("AND Equip_Id = @eq");
            sql.AppendLine("ORDER BY start_date desc");

            SqlDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.USSDrillData));
            da.SelectCommand.Parameters.AddWithValue("Start_Time", Status_Start_Time);
            da.SelectCommand.Parameters.AddWithValue("eq", Eq.Equip_Id);

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows[0].Field<int>("drilled_hole_id");
            }
            else
                return -1;
        }

        /// <summary>
        /// imports the Equipment hours per shift into the Equip_Hours table
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        public void ImportHours(DateTime StartDate, DateTime EndDate)
        {
            DateTime loopDate = StartDate;
            while(loopDate <= EndDate)
            {
                ImportHours(loopDate, 1);
                ImportHours(loopDate, 2);
                loopDate = loopDate.AddDays(1);
            }
        }

        /// <summary>
        /// imports the Equipment hours per shift into the Equip_Hours table
        /// </summary>
        /// <param name="ShiftDate"></param>
        /// <param name="Shift"></param>
        public void ImportHours(DateTime ShiftDate, byte Shift)
        {
            DateTime[] startEnd = MOO.Shifts.Shift.ShiftStartEndTime(Plant, Area.Drilling, ShiftDate, Shift);
            if (DateTime.Now < startEnd[1])
            {
                //shift has not finished yet so don't import
                return;
            }
            List <Models.Equip> eqList = Services.EquipSvc.GetAll(Plant);
            StringBuilder sql = new();
            Services.Equip_HoursSvc.DeleteShift(Plant, ShiftDate, Shift);


            sql.AppendLine("SELECT e.Equip_Ident,");
            sql.AppendLine("	COALESCE(CAST(SUM(CASE WHEN SUBSTRING(status_code,1,1) = 'N' THEN DATEDIFF(s,Start_Timestamp, End_Timestamp) ELSE 0 END) AS float)/3600, 0) Oper,");
            sql.AppendLine("	COALESCE(CAST(SUM(CASE WHEN SUBSTRING(status_code,1,1) = 'O' THEN DATEDIFF(s,Start_Timestamp, End_Timestamp) ELSE 0 END) AS float)/3600, 0) OperDelay,");
            sql.AppendLine("	COALESCE(CAST(SUM(CASE WHEN SUBSTRING(status_code,1,1) = 'S' THEN DATEDIFF(s,Start_Timestamp, End_Timestamp) ELSE 0 END) AS float)/3600, 0) Standby,");
            sql.AppendLine("	COALESCE(CAST(SUM(CASE WHEN (SUBSTRING(status_code,1,1) = 'M' AND status_code NOT IN ('M58', 'M56')) THEN DATEDIFF(s,Start_Timestamp, End_Timestamp) ELSE 0 END) AS float)/3600, 0) Maint,");
            sql.AppendLine("	COALESCE(CAST(SUM(CASE WHEN status_code = 'M58' THEN DATEDIFF(s,Start_Timestamp, End_Timestamp) ELSE 0 END) AS float)/3600, 0) MgmtDec,");
            sql.AppendLine("	COALESCE(CAST(SUM(CASE WHEN status_code = 'M56' THEN DATEDIFF(s,Start_Timestamp, End_Timestamp) ELSE 0 END) AS float)/3600, 0) SchedDown");
            sql.AppendLine("FROM equipment_status_trans est");
            sql.AppendLine("INNER JOIN EQUIP e");
            sql.AppendLine("	ON est.equip_ident = e.equip_ident");
            sql.AppendLine("WHERE e.EQP_TYPE='DRL'");
            sql.AppendLine($"AND Shift_Date = '{ShiftDate:MM/dd/yyyy}'");
            sql.AppendLine($"AND SHIFT_IDENT = {Shift}");
            sql.AppendLine("GROUP BY e.EQUIP_IDENT");

            DataSet ds;
            MOO.Data.MNODatabase db = Data.MNODatabase.MTC_Wenco;
            if (Plant == Plant.Keetac)
                db = Data.MNODatabase.KTC_Wenco;

            ds = MOO.Data.ExecuteQuery(sql.ToString(), db);
            foreach(DataRow dr in ds.Tables[0].Rows)
            {
                Models.Equip eq = eqList.FirstOrDefault(x => x.Equip_Number == dr.Field<string>("equip_ident"));
                if(eq != null)
                {
                    Models.Equip_Hours eh = new()
                    {
                        Equip = eq,
                        ShiftDate = ShiftDate,
                        Shift = Shift,
                        Maint = Math.Round(Convert.ToDecimal(dr["Maint"]),2,MidpointRounding.AwayFromZero),
                        Oper = Math.Round(Convert.ToDecimal(dr["Oper"]), 2, MidpointRounding.AwayFromZero),
                        OperDelay = Math.Round(Convert.ToDecimal(dr["OperDelay"]), 2, MidpointRounding.AwayFromZero),
                        Standby = Math.Round(Convert.ToDecimal(dr["Standby"]), 2, MidpointRounding.AwayFromZero),
                        Sched = Math.Round(Convert.ToDecimal(dr["SchedDown"]), 2, MidpointRounding.AwayFromZero),
                        MgmtDec = Math.Round(Convert.ToDecimal(dr["MgmtDec"]), 2, MidpointRounding.AwayFromZero)
                    };
                    //we get an occasional situation where we don't add up to 12 hours (not sure how this happens)
                    //We need to clean this up in that situation
                    decimal totHrs = eh.Maint + eh.Oper + eh.OperDelay + eh.Standby + eh.Sched + eh.MgmtDec;
                    decimal expectedHrs = 12;

                                      

                    if (startEnd[0].IsDaylightSavingTime() && !startEnd[1].IsDaylightSavingTime())
                    {
                        //Fall Back daylight saving, expect 13 hour
                        expectedHrs = 13;
                    }
                    else if (!startEnd[0].IsDaylightSavingTime() && startEnd[1].IsDaylightSavingTime())
                    {
                        //Spring ahead, expect 11 hours
                        expectedHrs = 11;
                    }



                    if (totHrs != expectedHrs && totHrs > 0)
                    {
                        //reallocate the hours so we add up to 12 based
                        if(Math.Abs(totHrs - expectedHrs) < .1M)
                        {
                            //assume this is a rounding issue and just apply it to the largets
                            FixRounding(eh, expectedHrs);
                        }
                        else
                        {
                            eh.OperDelay *=Math.Round((expectedHrs / totHrs), 2, MidpointRounding.AwayFromZero);
                            eh.Oper *= Math.Round((expectedHrs / totHrs), 2, MidpointRounding.AwayFromZero);
                            eh.Maint *= Math.Round((expectedHrs / totHrs), 2, MidpointRounding.AwayFromZero);
                            eh.Standby *= Math.Round((expectedHrs / totHrs), 2, MidpointRounding.AwayFromZero);
                            eh.Sched *= Math.Round((expectedHrs / totHrs), 2, MidpointRounding.AwayFromZero);
                            eh.MgmtDec *= Math.Round((expectedHrs / totHrs), 2, MidpointRounding.AwayFromZero);
                            //now just make sure rounding is good
                            FixRounding(eh, expectedHrs);
                        }
                    }
                    Services.Equip_HoursSvc.Insert(eh);
                }
            }
        }

        /// <summary>
        /// Fixes the rounding so hours add up exactly per shift
        /// </summary>
        /// <param name="Eh"></param>
        private static void FixRounding(Models.Equip_Hours Eh, decimal ExpectedHrs)
        {
            //first get the total hours
            decimal totHrs = Eh.Maint + Eh.Oper + Eh.OperDelay + Eh.Standby + Eh.Sched + Eh.MgmtDec;
            // calculate the differenect 
            decimal AddHrs = ExpectedHrs - totHrs;
            if (AddHrs == 0)
                return;
            //now apply that to whatever is the largest (Not really a good way to do this, We will first find what the largest value is,
            //then go back and find which item had that value
            decimal largest = Math.Max(Math.Max(Math.Max(Math.Max(Math.Max(Eh.Maint, Eh.Oper), Eh.OperDelay), Eh.Standby), Eh.Sched), Eh.MgmtDec);
            if (Eh.Maint == largest)
                Eh.Maint += AddHrs;
            else if (Eh.Oper == largest)
                Eh.Oper += AddHrs;
            else if (Eh.OperDelay == largest)
                Eh.OperDelay += AddHrs;
            else if (Eh.Standby == largest)
                Eh.Standby += AddHrs;
            else if (Eh.Sched == largest)
                Eh.Sched += AddHrs;
            else
                Eh.MgmtDec += AddHrs;
        }
    }
}
