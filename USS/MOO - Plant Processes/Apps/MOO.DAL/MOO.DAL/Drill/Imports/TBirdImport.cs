using MOO.DAL.Drill.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace MOO.DAL.Drill.Imports
{
    [Obsolete("Thunderbird Server Shut Down in 2024.  No longer used.")]
    public class TBirdImport : BaseImport
    {

        private readonly string TBirdDatabase;
        public TBirdImport(Plant Plant) : base(Plant)
        {
            if (Plant == Plant.Minntac)
                TBirdDatabase = "IMSC-MINNTAC";
            else
                TBirdDatabase = "IMSC-KEETAC";
        }

        public override List<Raw_Drilled_Hole> GetRawData(DateTime StartDate, DateTime EndDate)
        {
            List<Raw_Drilled_Hole> retVal = new();
            StringBuilder sql = new();

            sql.AppendLine("SELECT hd.hole_dat_id, hd.Pattern_Number, hd.Hole_Number, hd.Start_Time, hd.End_Time,");
            sql.AppendLine("		hd.design_northing, hd.design_easting, hd.planned_depth design_bottom, ");
            sql.AppendLine("		hd.collar_easting, hd.collar_northing, hd.collar_elevation,");
            sql.AppendLine("		hd.actual_depth, eq.eq_id, eq.EQ_Number, op.Op_Number, op.op_name_first,");
            sql.AppendLine("		op.op_name_last, (hd.collar_elevation - hd.planned_depth) planned_depth,");
            sql.AppendLine("		(hd.collar_elevation - hd.actual_depth) actual_bottom");
            sql.AppendLine($"FROM [{TBirdDatabase}].dbo.IMSC_Hole_Dat hd");
            sql.AppendLine($"INNER JOIN [{TBirdDatabase}].dbo.IMSC_EQ eq");
            sql.AppendLine("	ON eq.eq_id = hd.eq_id");
            sql.AppendLine($"INNER JOIN [{TBirdDatabase}].dbo.IMSC_Operator op");
            sql.AppendLine("	ON hd.OperatorID_Primary = op.op_id");
            sql.AppendLine($"WHERE hd.End_Time >= '{StartDate:MM/dd/yyyy HH:mm:ss}' AND hd.End_Time <  '{EndDate:MM/dd/yyyy HH:mm:ss}'");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), MOO.Data.MNODatabase.USSDrillData);

            foreach(DataRow dr in ds.Tables[0].Rows)
            {
                Raw_Drilled_Hole newHole = DataRowToObj(dr);
                SetBit(newHole,dr.Field<int>("eq_id"));
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
            retval.Operator_Number = dr.Field<string>("op_number")!.Trim();
            retval.Operator_First_Name = dr.Field<string>("op_name_first")!.Trim();
            retval.Operator_Last_Name = dr.Field<string>("op_name_last")!.Trim();
            retval.Hole_Number = dr.Field<string>("hole_number")!.Trim();
            retval.Planned_Depth = Convert.ToDecimal(dr.Field<double>("planned_depth"));
            retval.Drilled_Depth = Convert.ToDecimal(dr.Field<double>("actual_depth"));
            retval.Start_Date = dr.Field<DateTime>("Start_Time")!;
            retval.End_Date = dr.Field<DateTime>("End_Time")!;
            retval.Design_Northing = Convert.ToDecimal(dr.Field<double>("design_northing"));
            retval.Actual_Northing = Convert.ToDecimal(dr.Field<double>("collar_northing"));
            retval.Design_Easting = Convert.ToDecimal(dr.Field<double>("design_easting"));
            retval.Actual_Easting = Convert.ToDecimal(dr.Field<double>("collar_easting"));
            retval.Design_Bottom = Convert.ToDecimal(dr.Field<double>("design_bottom"));
            retval.Actual_Bottom = Convert.ToDecimal(dr.Field<double>("actual_bottom"));
            retval.Collar = Convert.ToDecimal(dr.Field<double>("collar_elevation"));
            retval.Reference_Key = dr.Field<int>("hole_dat_id").ToString();


            return retval;
        }

        /// <summary>
        /// Retrieves the bit that was being used for the specified drill at the specified time
        /// </summary>
        /// <param name="DrillId"></param>
        /// <param name="HoleStart"></param>
        /// <returns></returns>
        private void SetBit(Raw_Drilled_Hole rdh, int DrillId)
        {
            StringBuilder sql = new();

            sql.AppendLine("SELECT TOP 1 cons.cons_manuf, consumed_serial_number");
            sql.AppendLine($"FROM [{TBirdDatabase}].dbo.IMSC_Consumed c");
            sql.AppendLine($"LEFT OUTER JOIN [{TBirdDatabase}].dbo.IMSC_Consumables cons");
            sql.AppendLine($"ON c.cons_id = cons.cons_id");
            sql.AppendLine($"WHERE eq_id = {DrillId}");
            sql.AppendLine($"AND ((consumed_start <= '{rdh.End_Date:MM/dd/yyyy HH:mm:ss}'");
            sql.AppendLine($"AND consumed_stop >= '{rdh.End_Date:MM/dd/yyyy HH:mm:ss}')");
            sql.AppendLine($"OR (consumed_start = consumed_stop AND consumed_start <= '{rdh.End_Date:MM/dd/yyyy HH:mm:ss}'))");
            sql.AppendLine("order by consumed_start desc");


            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), MOO.Data.MNODatabase.USSDrillData);
            if(ds.Tables[0].Rows.Count > 0 && !string.IsNullOrEmpty(ds.Tables[0].Rows[0][1].ToString().Trim()))
            {
                rdh.Drill_Bit_Manufacturer = ds.Tables[0].Rows[0][0].ToString().Trim();
                rdh.Drill_Bit_Serial = ds.Tables[0].Rows[0][1].ToString().Trim();
            }
        }
    }
}
