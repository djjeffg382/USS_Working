using Microsoft.Data.SqlClient;
using MOO.DAL.Drill.Models;
using MOO.DAL.ToLive.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Drill.Imports
{
    public class TerrainImport : BaseImport
    {

        public TerrainImport(Plant Plant) : base(Plant)
        {
            //This was tested with the 65 drill at Minntac If Keetac is used, will probably need to adjust the database name used for query
            if (Plant == Plant.Keetac)
                throw new NotImplementedException("Keetac Wenco Drill not implemented");

        }
        public override List<Raw_Drilled_Hole> GetRawData(DateTime StartDate, DateTime EndDate)
        {
            List<Raw_Drilled_Hole> retVal = new();
            string sql = @"SELECT hole.hole_id, m.name Equipment_Number,
			Pattern.name PatternNumber,
			DrillBit1.consumable_supplier Drill_Bit_Manufacturer,
			DrillBit.serial Drill_Bit_Serial, 
			op.code Operator_Number,
			op.name Operator_Name,
			0 Material,
			hole.name Hole_Number,
			ROUND(hole.design_depth * 3.28084,1) Planned_Depth,
			ROUND(hole.depth * 3.28084, 2) Drilled_Depth,
			hole.time_start Start_Date,
			hole.time_end End_Date,
			hp.design_x * 3.28084 Design_Northing,
			hp.start_x * 3.28084  Actual_Northing,
			hp.design_y * 3.28084 Design_Easting,
			hp.start_y * 3.28084 Actual_Easting,
			hp.design_z * 3.28084 Design_Bottom,
			hp.end_z * 3.28084 Actual_Bottom,
			hp.collarelevation * 3.28084 Collar,
			hole.hole_id Reference_Key,
			0 ROP_Ft_Per_Min
FROM MTCTerrain.dbo.dr_p_hole_information hole
JOIN MTCTerrain.dbo.dr_c_machine m
	ON hole.machine_id = m.machine_id
JOIN MTCTerrain.dbo.dr_c_sub_area Pattern
	ON hole.sub_area_id = Pattern.sub_area_id
JOIN MTCTerrain.dbo.dr_p_hole_position hp
	ON hole.hole_id = hp.hole_id
LEFT JOIN MTCTerrain.dbo.dr_p_session sess
	ON hole.time_end BETWEEN sess.time_start AND sess.time_end
LEFT JOIN MTCTerrain.dbo.dr_c_operator op
	ON sess.operator_id = op.operator_id
LEFT JOIN MTCTerrain.dbo.dr_p_consumable_use consume
	ON hole.hole_id = consume.hole_id AND hole.machine_id = consume.machine_id AND hole.time_end BETWEEN consume.time_start AND COALESCE(consume.time_end,getdate())
LEFT JOIN MTCTerrain.dbo.dr_p_machine_consumable DrillBit1
	ON consume.consumable_id = DrillBit1.consumable_id
LEFT JOIN MTCTerrain.dbo.dr_c_consumable DrillBit
	ON consume.consumable_id = DrillBit.consumable_id AND DrillBit.consumable_type_id = 100
WHERE hole.time_end BETWEEN @StartDate AND @EndDate
ORDER BY hole.time_start";

            using SqlConnection conn = new SqlConnection(MOO.Data.GetConnectionString(Data.MNODatabase.MineStar));
            SqlCommand cmd = new(sql, conn);
            //Note Terrain is in UTC
            cmd.Parameters.AddWithValue("StartDate", StartDate.ToUniversalTime());
            cmd.Parameters.AddWithValue("EndDate", EndDate.ToUniversalTime());
            conn.Open();
            var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var drillObj = RdrToObject(rdr);
                retVal.Add(drillObj);
            }
            return retVal;
        }



        public Raw_Drilled_Hole RdrToObject(SqlDataReader rdr)
        {
            Raw_Drilled_Hole retVal = new();

            retVal.Plant = Plant;
            retVal.Equipment_Number = (string)Util.GetRowVal(rdr, "Equipment_Number");
            retVal.Pattern_Number = (string)Util.GetRowVal(rdr, "PatternNumber");
            retVal.Drill_Bit_Serial = (string)Util.GetRowVal(rdr, "Drill_Bit_Serial");
            retVal.Operator_Number = (string)Util.GetRowVal(rdr, "Operator_Number");
            var fullName = ((string)Util.GetRowVal(rdr, "Operator_Name"));
            if (fullName != null && fullName != "Guest")
            {
                string[] name = fullName.Split(' '); ;
                if (name.Length >= 1)
                    retVal.Operator_First_Name = name[0];
                if (name.Length >= 2)
                    retVal.Operator_Last_Name = name[1];
            }

            retVal.Hole_Number = (string)Util.GetRowVal(rdr, "Hole_Number");
            //for some reason Terrain puts a "V" at the end of hole number, let's remove that
            retVal.Hole_Number = retVal.Hole_Number.TrimEnd('V');
                

            retVal.Planned_Depth = (decimal)(double)Util.GetRowVal(rdr, "Planned_Depth");
            retVal.Drilled_Depth = (decimal)(double)Util.GetRowVal(rdr, "Drilled_Depth");
            //Note dates in Terrain database are in UTC, must convert
            retVal.Start_Date = ConvertTerrainTime((DateTime)Util.GetRowVal(rdr, "Start_Date"));            
            retVal.End_Date = ConvertTerrainTime((DateTime)Util.GetRowVal(rdr, "End_Date"));


            retVal.Design_Northing = (decimal?)(double?)Util.GetRowVal(rdr, "Design_Northing");
            retVal.Actual_Northing = (decimal?)(double?)Util.GetRowVal(rdr, "Actual_Northing");
            retVal.Design_Easting = (decimal?)(double?)Util.GetRowVal(rdr, "Design_Easting");
            retVal.Actual_Easting = (decimal?)(double?)Util.GetRowVal(rdr, "Actual_Easting");
            retVal.Design_Bottom = (decimal?)(double?)Util.GetRowVal(rdr, "Design_Bottom");
            retVal.Actual_Bottom = (decimal?)(double?)Util.GetRowVal(rdr, "Actual_Bottom");
            retVal.Collar = (decimal?)(double?)Util.GetRowVal(rdr, "Collar");
            retVal.Reference_Key = ((int)Util.GetRowVal(rdr, "Reference_Key")).ToString();

            var holeId = (int)Util.GetRowVal(rdr, "Hole_Id");
            retVal.ROP_Ft_Per_Min = GetROP(holeId);
            return retVal;
        }

        /// <summary>
        /// converts the Terrain (in UTC) time to local time
        /// </summary>
        /// <param name="dateVal"></param>
        /// <returns></returns>
        private static DateTime ConvertTerrainTime(DateTime dateVal)
        {
            DateTime ut = DateTime.SpecifyKind(dateVal, DateTimeKind.Utc);
            return ut.ToLocalTime();
        }

        /// <summary>
        /// gets the average ROP from terrain
        /// </summary>
        /// <param name="HoleNumber"></param>
        /// <returns></returns>
        private static decimal? GetROP(int HoleNumber)
        {
            //I am assuming the value in the DB is in Meters/Second so we will convert it to Feet/Minute
            string sql = @"SELECT AVG(rop) * 3.28084 * 60
  FROM MTCTerrain.dbo.dr_p_hole_profile
  WHERE hole_id = " + HoleNumber;

            using SqlConnection conn = new SqlConnection(MOO.Data.GetConnectionString(Data.MNODatabase.MineStar));
            SqlCommand cmd = new(sql, conn);
            //Note Terrain is in UTC
            conn.Open();
            var rdr = cmd.ExecuteReader();
            if (rdr.Read() && !rdr.IsDBNull(0))
            {
                return (decimal?)(double?)rdr.GetValue(0);
            }
            else
                return null;




        }
    }


}
