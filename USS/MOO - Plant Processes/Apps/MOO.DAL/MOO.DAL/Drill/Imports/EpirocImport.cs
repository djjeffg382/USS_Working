using MOO.DAL.Drill.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace MOO.DAL.Drill.Imports
{
    public class EpirocImport : BaseImport
    {
        private readonly List<MOO.DAL.ToLive.Models.People> UsedPeopleList = new();


        public EpirocImport(Plant Plant) : base(Plant)
        {
        }

        public override List<Raw_Drilled_Hole> GetRawData(DateTime StartDate, DateTime EndDate)
        {
            List<Raw_Drilled_Hole> retVal = new();
            StringBuilder sql = new();
            string surfaceMgrDb = "[SurfaceManager]";
            if (Plant == Plant.Keetac)
                surfaceMgrDb = "[KTC-SurfaceManager]";


            sql.AppendLine("SELECT dh.id, dp.Name as pattern_number, r.Name as eq_number, dh.HoleId as hole_number, dh.OperatorName as Operator_Name,");
            sql.AppendLine("	COALESCE((ph.StartPointZ - ph.EndPointZ) * 3.28084 , 0) as planned_depth, (dh.StartPointZ - dh.EndPointZ) * 3.28084 as drilled_depth,");
            sql.AppendLine("	dh.StartHoleTime as start_time, dh.EndHoleTime as end_time, dh.DrillBitId as bit_number,");
            sql.AppendLine("    dh.RawStartPointY * 3.28084 collar_easting, dh.RawStartPointX * 3.28084 collar_northing,");
            sql.AppendLine("    dh.RawStartPointZ * 3.28084 collar_elevation, dh.RawEndPointZ * 3.28084 actual_bottom, ");
            sql.AppendLine("    ph.RawStartPointY  * 3.28084 Design_Easting, ph.RawStartPointX  * 3.28084 Design_Northing,");
            sql.AppendLine("    ph.RawEndPointZ  * 3.28084 Design_Bottom, dh.AveragePenetrationRateInMetersPerMinute");
            sql.AppendLine($"FROM {surfaceMgrDb}.dbo.DrilledHole dh");
            sql.AppendLine($"Left Outer Join {surfaceMgrDb}.dbo.DrillPlan dp");
            sql.AppendLine("	on dh.DrillPlanId = dp.Id");
            sql.AppendLine($"Left Outer Join {surfaceMgrDb}.dbo.PlannedHole ph");
            sql.AppendLine("	on ph.DrillPlanId = dh.DrillPlanId AND dh.HoleId = ph.OriginalHoleId");
            sql.AppendLine($"Left Outer Join {surfaceMgrDb}.dbo.Rig r");
            sql.AppendLine("	on r.SerialNumber = dh.RigSerialNumber");
            sql.AppendLine($"WHERE dh.EndHoleTime >= '{StartDate:MM/dd/yyyy HH:mm:ss}' AND dh.EndHoleTime <  '{EndDate:MM/dd/yyyy HH:mm:ss}'");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), MOO.Data.MNODatabase.USSDrillData);

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Raw_Drilled_Hole newHole = DataRowToObj(dr);
                retVal.Add(newHole);

            }
            return retVal;
        }


        private Raw_Drilled_Hole DataRowToObj(DataRow dr)
        {
            Raw_Drilled_Hole retval = new();
            retval.Plant = Plant;
            retval.Equipment_Number = dr.Field<string>("eq_number")!.Trim();
            retval.Pattern_Number = dr.Field<string>("pattern_number")!.Trim();

            //Epiroc just has operator name field, should be in the format of Employee_Nbr<space>Employee_Last_Name
            string[] opName = dr.Field<string>("Operator_Name")!.Trim().Split(' ');
            if (opName.Length > 1)
            {
                retval.Operator_Number = opName[0].TrimStart('0');
                MOO.DAL.ToLive.Models.People p = GetPerson(retval.Operator_Number);
                if(p != null)
                {
                    retval.Operator_First_Name = p.First_Name;
                    retval.Operator_Last_Name = p.Last_Name;
                }
                else
                {
                    retval.Operator_First_Name = "";
                    retval.Operator_Last_Name = opName[1];
                }
            }

            retval.Hole_Number = dr.Field<string>("hole_number")!.Trim();
            retval.Planned_Depth = Convert.ToDecimal(dr.Field<double>("planned_depth"));
            retval.Drilled_Depth = Convert.ToDecimal(dr.Field<double>("drilled_depth"));
            retval.Start_Date = dr.Field<DateTime>("Start_Time")!;
            retval.End_Date = dr.Field<DateTime>("End_Time")!;

            retval.Design_Northing = (decimal?) dr.Field<double?>("Design_Northing");
            retval.Actual_Northing = Convert.ToDecimal(dr.Field<double>("collar_northing"));
            retval.Design_Easting = (decimal?)dr.Field<double?>("Design_Easting");
            retval.Actual_Easting = Convert.ToDecimal(dr.Field<double>("collar_easting"));
            retval.Design_Bottom = (decimal?)dr.Field<double?>("Design_Bottom");
            retval.Actual_Bottom = Convert.ToDecimal(dr.Field<double>("actual_bottom"));
            retval.Collar = Convert.ToDecimal(dr.Field<double>("collar_elevation"));
            if (!dr.IsNull("AveragePenetrationRateInMetersPerMinute"))
            {
                retval.ROP_Ft_Per_Min = (decimal)(dr.Field<double>("AveragePenetrationRateInMetersPerMinute") * 3.28084);
            }
            retval.Reference_Key = dr["id"].ToString()!;


            return retval;
        }

        private MOO.DAL.ToLive.Models.People GetPerson(string EmployeeNbr)
        {
            MOO.DAL.ToLive.Models.People retVal = null;
            if (int.TryParse(EmployeeNbr, out int OpNbr))
            {
                retVal = UsedPeopleList.FirstOrDefault(x => x.Employee_Number == EmployeeNbr);
                if (retVal == null)
                {
                    //not in the used people list search the database
                    List<MOO.DAL.ToLive.Models.People> pList = MOO.DAL.ToLive.Services.PeopleSvc.GetAll(empId: OpNbr);
                    if (pList.Count > 0)
                    {
                        UsedPeopleList.Add(pList[0]);
                        return pList[0];
                    }
                }

            }
            return retVal;
        }
    }
}
