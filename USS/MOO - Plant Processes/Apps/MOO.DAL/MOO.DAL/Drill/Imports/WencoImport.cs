using MOO.DAL.Drill.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace MOO.DAL.Drill.Imports
{
    public class WencoImport : BaseImport
    {

        public WencoImport(Plant Plant) : base(Plant)
        {
            if (Plant == Plant.Minntac)
                throw new NotImplementedException("Minntac Wenco Drill not implemented");

        }

        public override List<Raw_Drilled_Hole> GetRawData(DateTime StartDate, DateTime EndDate)
        {
            List<Raw_Drilled_Hole> retVal = new();
            StringBuilder sql = new();


            sql.AppendLine("SELECT dt.equip_ident, db.BLAST_LOCATION_SNAME,");
            sql.AppendLine("	dt.badge_ident, b.FIRST_NAME, b.LAST_NAME,");
            sql.AppendLine("	dt.HOLE_CODE, dt.hole_depth planned_depth, dt.hole_depth drilled_depth,");
            sql.AppendLine("	dt.drill_start_timestamp, dt.end_timestamp,");
            sql.AppendLine("	h.design_northing, h.design_easting,");
            sql.AppendLine("	dt.hole_northing actual_northing, dt.hole_easting actual_easting,");
            sql.AppendLine("	dt.hole_toe_elevation design_bottom,");
            sql.AppendLine("	dt.hole_toe_elevation actual_bottom,");
            sql.AppendLine("	dt.hole_elevation, dt.drill_rec_ident");
            sql.AppendLine("FROM WencoReport.dbo.DRILL_TRANS dt");
            sql.AppendLine("INNER JOIN WencoReport.dbo.DRILL_BLAST db");
            sql.AppendLine("	ON db.DRILL_BLAST_IDENT = dt.DRILL_BLAST_IDENT");
            sql.AppendLine("INNER JOIN WencoReport.[dbo].[BADGE] b");
            sql.AppendLine("	ON dt.BADGE_IDENT = b.BADGE_IDENT");
            sql.AppendLine("INNER JOIN WencoReport.[dbo].[DRILL_HOLE] h");
            sql.AppendLine("	ON h.Drill_Blast_Ident = dt.DRILL_BLAST_IDENT AND h.hole_code = dt.HOLE_CODE");
            sql.AppendLine($"WHERE dt.End_Timestamp >= '{StartDate:MM/dd/yyyy HH:mm:ss}' AND dt.End_Timestamp <  '{EndDate:MM/dd/yyyy HH:mm:ss}'");
            sql.AppendLine("	ORDER BY dt.END_TIMESTAMP desc");



            //sql.AppendLine("SELECT dh.id hole_id, dj.Description pattern_number, dh.DrillHoleName hole_number, m.ExternalIdentifier DrillNbr, m.id machine_id,");
            //sql.AppendLine("	dh.drilltipcollarz - dh.drilltiptoez Drilled_Depth,");
            //sql.AppendLine("	ROUND(COALESCE(dh.DrillTipCollarZ - dh.PlannedToeZ,0),2) planned_depth,");
            //sql.AppendLine("	dh.StartTimestamp start_time, dh.EndTimestamp end_time, ");
            //sql.AppendLine("    op.ExternalIdentifier op_number, op.FirstName op_name_first, op.LastName op_name_last,");
            //sql.AppendLine("    PlannedCollarX, PlannedCollarY, PlannedToeZ,");
            //sql.AppendLine("    DrillTipCollarX, DrillTipCollarY, DrillTipCollarZ, DrillTipToeZ");
            //sql.AppendLine("");
            //sql.AppendLine("FROM mineAPS.dbo.DrillHole dh");
            //sql.AppendLine("INNER JOIN mineAPS.dbo.DrillJob dj");
            //sql.AppendLine("    ON dh.JobId = dj.id");
            //sql.AppendLine("INNER JOIN mineAPS.dbo.machine m");
            //sql.AppendLine("    ON dh.MachineId = m.id");
            //sql.AppendLine("LEFT OUTER JOIN mineAPS.dbo.RunningHistory rh");
            //sql.AppendLine("    ON dh.EndTimestamp >= rh.StartDatetime AND dh.EndTimestamp < rh.EndDatetime AND rh.machineid = dh.MachineId");
            //sql.AppendLine("LEFT OUTER JOIN mineAPS.dbo.Operator op");
            //sql.AppendLine("    ON rh.OperatorId = op.id");
            //sql.AppendLine($"WHERE dh.EndTimestamp >= '{StartDate:MM/dd/yyyy HH:mm:ss}' AND dh.EndTimestamp <  '{EndDate:MM/dd/yyyy HH:mm:ss}'");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), MOO.Data.MNODatabase.KtcWencoReport);

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Raw_Drilled_Hole newHole = DataRowToObj(dr);
                //SetDrillBit(newHole, dr["machine_id"].ToString());
                newHole.ROP_Ft_Per_Min = GetROP(dr["drill_rec_ident"].ToString());
                retVal.Add(newHole);

            }
            return retVal;
        }


        /// <summary>
        /// Gets the Rate of penetration from Wenco datbase
        /// </summary>
        /// <param name="HoleID"></param>
        /// <returns></returns>
        private static decimal? GetROP(string HoleID)
        {
            
            StringBuilder sql = new();

            sql.AppendLine("SELECT avg(rate_of_penetration)/60 ");
            sql.AppendLine("  FROM [WENCO].[dbo].[DRILL_HOLE_DETAIL_TRANS]");
            sql.AppendLine($"where DRILL_REC_IDENT = {HoleID}");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.KTC_Wenco);
            DataRow dr = ds.Tables[0].Rows[0];
            if (!dr.IsNull(0))
            {
                return (decimal)dr[0];
            }
            else
                return null;

        }

        //private void SetDrillBit(Raw_Drilled_Hole rdh, string MachineId)
        //{
        //    StringBuilder sql = new();

        //    sql.AppendLine("SELECT COALESCE(mcm.description, '') manufacturer, mc.description serial");
        //    sql.AppendLine("  FROM mineAPS.dbo.MachineConsumable mc");
        //    sql.AppendLine("INNER JOIN mineAPS.dbo.MachineConsumableHistory mh");
        //    sql.AppendLine("ON mc.id = mh.MachineConsumableId");
        //    sql.AppendLine("LEFT OUTER JOIN mineAPS.dbo.MachineConsumableManufacturer mcm");
        //    sql.AppendLine(" ON mc.MachineConsumableManufacturerId = mcm.id");
        //    sql.AppendLine($"WHERE mh.InstallDatetime < '{rdh.End_Date:MM/dd/yyyy HH:mm:ss}'");
        //    sql.AppendLine($"AND COALESCE(mh.RemoveDatetime, GETDATE()) > '{rdh.End_Date:MM/dd/yyyy HH:mm:ss}'");
        //    sql.AppendLine("AND mc.MachineConsumableTypeId = 'F837AB42-8573-2F90-A051-D3F12EDA9530'"); //Consumable type = Bit
        //    sql.AppendLine($"AND mh.MachineId = '{MachineId}'");
        //    sql.AppendLine("Order by mh.InstallDatetime Desc");  //if there is more than one record, then use the one with the latest install date

        //    DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), MOO.Data.MNODatabase.KTC_Wenco);
        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        rdh.Drill_Bit_Manufacturer = ds.Tables[0].Rows[0][0].ToString();
        //        rdh.Drill_Bit_Serial =  ds.Tables[0].Rows[0][1].ToString();
        //    }

        //}
        private Raw_Drilled_Hole DataRowToObj(DataRow dr)
        {
            Raw_Drilled_Hole retval = new();
            retval.Plant = Plant;
            retval.Equipment_Number = dr.Field<string>("equip_ident")!.Trim();
            retval.Pattern_Number = dr.Field<string>("blast_location_sname")!.Trim();
            if (dr.IsNull("badge_ident"))
            {
                retval.Operator_Number = "";
                retval.Operator_First_Name = "";
                retval.Operator_Last_Name = "";
            }
            else
            {
                retval.Operator_Number = dr.Field<string>("badge_ident")!.Trim().TrimStart('0');  //trim leading zeros
                retval.Operator_First_Name = dr.Field<string>("first_name")!.Trim();
                retval.Operator_Last_Name = dr.Field<string>("last_name")!.Trim();
            }


            retval.Hole_Number = dr.Field<string>("hole_code")!.Trim();
            retval.Planned_Depth = dr.Field<decimal>("planned_depth");
            retval.Drilled_Depth = dr.Field<decimal>("drilled_depth");
            retval.Start_Date = dr.Field<DateTime>("drill_start_timestamp")!;
            retval.End_Date = dr.Field<DateTime>("end_timestamp")!;
            retval.Design_Northing = dr.Field<decimal?>("design_northing");
            retval.Actual_Northing = dr.Field<decimal?>("actual_northing");
            retval.Design_Easting = dr.Field<decimal?>("design_easting");
            retval.Actual_Easting = dr.Field<decimal?>("actual_easting");
            retval.Design_Bottom = dr.Field<decimal?>("design_bottom");
            retval.Actual_Bottom = dr.Field<decimal?>("actual_bottom");
            retval.Collar = dr.Field<decimal?>("hole_elevation");
            retval.Reference_Key = dr["drill_rec_ident"].ToString()!;


            return retval;
        }

    }
}