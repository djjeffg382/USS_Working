using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;

namespace MOO.DAL.ToLive.Services
{
    public static class Motor_ChangeSvc
    {
        static Motor_ChangeSvc()
        {
            Util.RegisterOracle();
        }


        /// <summary>
        /// Gets the Motor_Change by the specified ID
        /// </summary>
        /// <param name="motorChangeId"></param>
        /// <returns></returns>
        public static Motor_Change Get(int motorChangeId)
        {
            StringBuilder sql = new();

            sql.Append(GetSelect());
            sql.AppendLine($"WHERE mc.motor_change_id = {motorChangeId}");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;

        }


        /// <summary>
        /// Gets the Motor_Changes that require ground check AND have not had a ground check
        /// </summary>
        /// <returns></returns>
        public static List<Motor_Change> GetReqGrdChk()
        {
            List<Motor_Change> retVal = new();
            StringBuilder sql = new();

            sql.Append(GetSelect());
            sql.AppendLine($"WHERE mc.motor_change_id IN (");
            sql.AppendLine("    SELECT mc.motor_change_id");
            sql.AppendLine("        FROM tolive.motor_change mc");
            sql.AppendLine("    LEFT JOIN tolive.motor_test mt");
            sql.AppendLine("        ON mt.motor_change_id = mc.motor_change_id");
            sql.AppendLine("    WHERE mc.req_grd_chk = 1");
            sql.AppendLine("    AND mt.motor_test_id IS NULL)");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);

            foreach (DataRow dr in ds.Tables[0].Rows)
                retVal.Add(DataRowToObject(dr));

            return retVal;

        }

        /// <summary>
        /// Gets the Motor_Changes by the specified motor id
        /// </summary>
        /// <param name="motorEquipmentId"></param>
        /// <returns></returns>
        public static List<Motor_Change> GetByMotor(int motorEquipmentId)
        {
            List<Motor_Change> retVal = new();
            StringBuilder sql = new();

            sql.Append(GetSelect());
            sql.AppendLine($"WHERE mc.motor_equipment_id = {motorEquipmentId}");
            sql.AppendLine("ORDER BY mc.date_of_change");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            foreach (DataRow row in ds.Tables[0].Rows)
                retVal.Add(DataRowToObject(row));

            return retVal;

        }




        /// <summary>
        /// Inserts the motor_change into Oracle
        /// </summary>
        /// <param name="mc"></param>
        /// <returns></returns>
        public static int Insert(Motor_Change mc)
        {
            ValidateData(mc);
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();

            int recsAffected = Insert(mc, conn);
            return recsAffected;
        }

        /// <summary>
        /// Inserts the motor_test into Oracle
        /// </summary>
        /// <param name="mc"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Insert(Motor_Change mc, OracleConnection conn)
        {
            ValidateData(mc);
            if (mc.Motor_Change_Id <= 0)
                mc.Motor_Change_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.motor_seq"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.motor_change (motor_change_id, motor_equipment_id, ");
            sql.AppendLine("    date_of_change, work_order, change_done_by, notes, ");
            sql.AppendLine("    old_motor_status_id, new_motor_status_id, ");
            sql.AppendLine("    old_motor_location_id, new_motor_location_id, req_grd_chk,");
            sql.AppendLine(" last_modified_by, deleted_record, last_modified_date)");
            sql.AppendLine("VALUES(:motor_change_id, :motor_equipment_id, ");
            sql.AppendLine("    :date_of_change, :work_order, :change_done_by, :notes, ");
            sql.AppendLine("    :old_motor_status_id, :new_motor_status_id, ");
            sql.AppendLine("    :old_motor_location, :new_motor_location, :req_grd_chk,");
            sql.AppendLine(" :last_modified_by, :deleted_record, :last_modified_date)");

            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("motor_change_id", mc.Motor_Change_Id);
            ins.Parameters.Add("motor_equipment_id", mc.Motor_Equipment.Motor_Equipment_Id);
            ins.Parameters.Add("date_of_change", mc.Date_Of_Change);
            ins.Parameters.Add("work_order", mc.Work_Order);
            ins.Parameters.Add("change_done_by", mc.Change_Done_By);
            ins.Parameters.Add("notes", mc.Notes);
            ins.Parameters.Add("old_motor_status_id", mc.Old_Motor_Status.Motor_Status_Id);
            ins.Parameters.Add("new_motor_status_id", mc.New_Motor_Status.Motor_Status_Id);
            ins.Parameters.Add("old_motor_location", mc.Old_Motor_Location.Motor_Location_Id);
            ins.Parameters.Add("new_motor_location", mc.New_Motor_Location.Motor_Location_Id);
            ins.Parameters.Add("req_grd_chk", mc.Req_Grd_Chk ? 1 : 0);
            ins.Parameters.Add("last_modified_by", mc.Last_Modified_By);
            ins.Parameters.Add("deleted_record", mc.Deleted_Record ? 1 : 0);
            ins.Parameters.Add("last_modified_date", DateTime.Now);

            int recsAffected = ins.ExecuteNonQuery();

            //next update the motor equipment table with the new status and location
            var mm = Motor_MotorsSvc.Get(mc.Motor_Equipment.Motor_Equipment_Id);
            mm.Motor_Location = mc.New_Motor_Location;
            mm.Motor_Status = mc.New_Motor_Status;
            Motor_MotorsSvc.Update(mm, conn);

            return recsAffected;
        }




        /// <summary>
        /// returns the select, from, and join portion of the query
        /// </summary>
        /// <param name="addField">any additional fields to add to the query</param>
        /// <returns></returns>
        private static string GetSelect(string addField = "")
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT mc.motor_change_id, mc.date_of_change, mc.work_order, mc.change_done_by,");
            sql.AppendLine("    mc.notes, mc.last_modified_by, mc.deleted_record, mc.last_modified_date,");
            sql.AppendLine("    mc.req_grd_chk,");


            //Motor_Equipment fields (will be prefixed with mm for motor_motors
            sql.AppendLine(" me.motor_equipment_id mm_motor_equipment_id, me.motor_type_id mm_motor_type_id,");
            sql.AppendLine("    me.voltage_rating mm_voltage_rating,");
            sql.AppendLine("    me.instruction_book mm_instruction_book, me.equip_serial_num mm_equip_serial_num, me.last_modified_by mm_last_modified_by,");
            sql.AppendLine("    me.deleted_record mm_deleted_record, me.last_modified_date mm_last_modified_date,");
            //motor_motors fields
            sql.AppendLine("    mm.notes mm_notes, mm.motor_type mm_motor_type, mm.frame_size mm_frame_size, ");
            sql.AppendLine("    mm.horse_power mm_horse_power, mm.rpm mm_rpm, mm.inner_bearing mm_inner_bearing,");
            sql.AppendLine("    mm.outer_bearing mm_outer_bearing, mm.phase mm_phase, mm.full_load_amps mm_full_load_amps, ");
            sql.AppendLine("    mm.purchase_date mm_purchase_date, mm.inverter_rated mm_inverter_rated, ");
            sql.AppendLine("    mm.drive_configuration mm_drive_configuration, mm.explosion_proof mm_explosion_proof, mm.weight mm_weight,");
            sql.AppendLine("    mm.j_box_setup mm_j_box_setup, mm.rtd_type mm_rtd_type, mm.insulation_class mm_insulation_class, mm.service_factor mm_service_factor,");
            sql.AppendLine("    mm.critical_motor mm_critical_motor, mm.catalog_id_number mm_catalog_id_number,");
            //motor status fields
            sql.AppendLine("    stat.motor_status_id stat_motor_status_id,stat.motor_status_name stat_motor_status_name, ");
            sql.AppendLine("    stat.last_modified_by stat_last_modified_by,");
            sql.AppendLine("    stat.deleted_record stat_deleted_record, stat.last_modified_date stat_last_modified_date,");
            //motor location fields
            sql.AppendLine(Motor_LocationSvc.GetFields("ml", "loc_"));
            //Motor Area fields
            sql.AppendLine("    ma.motor_area_id ma_motor_area_id,ma.motor_area_desc ma_motor_area_desc,");
            sql.AppendLine("    ma.deleted_record ma_deleted_record, ma.last_modified_date ma_last_modified_date,");
            sql.AppendLine("    ma.last_modified_by ma_last_modified_by,");
            //motor site fields
            sql.AppendLine("    ms.motor_site_id ms_motor_site_id,ms.motor_site_name ms_motor_site_name, ms.last_modified_by ms_last_modified_by,");
            sql.AppendLine("    ms.deleted_record ms_deleted_record, ms.last_modified_date ms_last_modified_date,");
            //motor_manufacturer fields   
            sql.AppendLine("    man.motor_manufacturer_id man_motor_manufacturer_id,man.motor_manufacturer_desc man_motor_manufacturer_desc,");
            sql.AppendLine("    man.last_modified_by man_last_modified_by, ");
            sql.AppendLine("    man.deleted_record man_deleted_record, ");
            sql.AppendLine("    man.last_modified_date man_last_modified_date,");


            //Old status fields
            sql.AppendLine("    oldstat.motor_status_id oldstat_motor_status_id,oldstat.motor_status_name oldstat_motor_status_name, ");
            sql.AppendLine("    oldstat.last_modified_by oldstat_last_modified_by,");
            sql.AppendLine("    oldstat.deleted_record oldstat_deleted_record, oldstat.last_modified_date oldstat_last_modified_date,");

            //New status fields
            sql.AppendLine("    newstat.motor_status_id newstat_motor_status_id,newstat.motor_status_name newstat_motor_status_name, ");
            sql.AppendLine("    newstat.last_modified_by newstat_last_modified_by,");
            sql.AppendLine("    newstat.deleted_record newstat_deleted_record, newstat.last_modified_date newstat_last_modified_date,");



            //old motor location fields
            sql.AppendLine(Motor_LocationSvc.GetFields("oldml", "oldml_"));
            //old Motor Area fields
            sql.AppendLine("    oldma.motor_area_id oldma_motor_area_id,oldma.motor_area_desc oldma_motor_area_desc,");
            sql.AppendLine("    oldma.deleted_record oldma_deleted_record, oldma.last_modified_date oldma_last_modified_date,");
            sql.AppendLine("    oldma.last_modified_by oldma_last_modified_by,");
            //old motor site fields
            sql.AppendLine("    oldms.motor_site_id oldms_motor_site_id,oldms.motor_site_name oldms_motor_site_name, oldms.last_modified_by oldms_last_modified_by,");
            sql.AppendLine("    oldms.deleted_record oldms_deleted_record, oldms.last_modified_date oldms_last_modified_date,");



            //new motor location fields
            sql.AppendLine(Motor_LocationSvc.GetFields("newml", "newml_"));
            //new Motor Area fields
            sql.AppendLine("    newma.motor_area_id newma_motor_area_id,newma.motor_area_desc newma_motor_area_desc,");
            sql.AppendLine("    newma.deleted_record newma_deleted_record, newma.last_modified_date newma_last_modified_date,");
            sql.AppendLine("    newma.last_modified_by newma_last_modified_by,");
            //new motor site fields
            sql.AppendLine("    newms.motor_site_id newms_motor_site_id,newms.motor_site_name newms_motor_site_name, newms.last_modified_by newms_last_modified_by,");
            sql.AppendLine("    newms.deleted_record newms_deleted_record, newms.last_modified_date newms_last_modified_date");


            if (!string.IsNullOrEmpty(addField))
            {
                sql.AppendLine($",{addField}");
            }

            sql.AppendLine("FROM tolive.motor_change mc");
            sql.AppendLine("INNER JOIN tolive.motor_equipment me");
            sql.AppendLine("    ON mc.motor_equipment_id = me.motor_equipment_id");
            sql.AppendLine("INNER JOIN tolive.motor_motors mm");
            sql.AppendLine("    ON mm.motor_equipment_id = me.motor_equipment_id");
            sql.AppendLine("INNER JOIN tolive.motor_status stat");
            sql.AppendLine("    ON me.motor_status_id = stat.motor_status_id");
            sql.AppendLine("INNER JOIN tolive.motor_location ml");
            sql.AppendLine("    ON me.motor_location_id = ml.motor_location_id");
            sql.AppendLine("INNER JOIN tolive.motor_area ma");
            sql.AppendLine("    ON ml.motor_area_id = ma.motor_area_id");
            sql.AppendLine("INNER JOIN tolive.motor_site ms");
            sql.AppendLine("    ON ma.motor_site_id = ms.motor_site_id");
            sql.AppendLine("INNER JOIN tolive.motor_manufacturer man");
            sql.AppendLine("    ON me.motor_manufacturer_id = man.motor_manufacturer_id");
            sql.AppendLine("LEFT JOIN tolive.motor_status oldstat");
            sql.AppendLine("    ON mc.old_motor_status_id = oldstat.motor_status_id");
            sql.AppendLine("LEFT JOIN tolive.motor_status newstat");
            sql.AppendLine("    ON mc.new_motor_status_id = newstat.motor_status_id");
            //old motor location
            sql.AppendLine("LEFT JOIN tolive.motor_location oldml");
            sql.AppendLine("    ON mc.old_motor_location_id = oldml.motor_location_id");
            sql.AppendLine("LEFT JOIN tolive.motor_area oldma");
            sql.AppendLine("    ON oldml.motor_area_id = oldma.motor_area_id");
            sql.AppendLine("LEFT JOIN tolive.motor_site oldms");
            sql.AppendLine("    ON oldma.motor_site_id = oldms.motor_site_id");
            //new motor location
            sql.AppendLine("LEFT JOIN tolive.motor_location newml");
            sql.AppendLine("    ON mc.new_motor_location_id = newml.motor_location_id");
            sql.AppendLine("LEFT JOIN tolive.motor_area newma");
            sql.AppendLine("    ON newml.motor_area_id = newma.motor_area_id");
            sql.AppendLine("LEFT JOIN tolive.motor_site newms");
            sql.AppendLine("    ON newma.motor_site_id = newms.motor_site_id");

            return sql.ToString();
        }



        private static bool ValidateData(Motor_Change mc)
        {
            if (mc.New_Motor_Status == null)
                throw new MOO.DAL.InvalidDataException("Missing New Motor Status", "New_Motor_Status_id");
            if (mc.Old_Motor_Status == null)
                throw new MOO.DAL.InvalidDataException("Missing Old Motor Status", "Old_Motor_Status_id");
            if (mc.New_Motor_Location == null)
                throw new MOO.DAL.InvalidDataException("Missing New Motor Location", "New_Motor_Location_id");
            if (mc.Old_Motor_Location == null)
                throw new MOO.DAL.InvalidDataException("Missing Old Motor Location", "Old_Motor_Location_id");

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldPrefix">The string prefix for all of the motor location fields in the row</param>
        /// <returns></returns>
        private static Motor_Change DataRowToObject(DataRow row, string fieldPrefix = "")
        {
            Motor_Change RetVal = new();

            RetVal.Date_Of_Change = row.Field<DateTime>("date_of_change");
            RetVal.Work_Order = row.Field<string>("work_order");
            RetVal.Change_Done_By = row.Field<string>("change_done_by");
            RetVal.Notes = row.Field<string>("notes");


            RetVal.Last_Modified_By = row.Field<string>($"{fieldPrefix}last_modified_by");
            RetVal.Deleted_Record = Convert.ToInt32(row[$"{fieldPrefix}deleted_record"]) == 1;
            RetVal.Req_Grd_Chk = Convert.ToInt32(row[$"{fieldPrefix}req_grd_chk"]) == 1;
            RetVal.Last_Modified_Date = row.Field<DateTime?>($"{fieldPrefix}last_modified_date");

            RetVal.Motor_Equipment = Motor_MotorsSvc.DataRowToObject(row, "mm_");

            if (!row.IsNull("oldstat_motor_status_id"))
                RetVal.Old_Motor_Status = Motor_StatusSvc.DataRowToObject(row, "oldstat_");

            if (!row.IsNull("newstat_motor_status_id"))
                RetVal.New_Motor_Status = Motor_StatusSvc.DataRowToObject(row, "newstat_");

            if (!row.IsNull("oldml_motor_location_id"))
                RetVal.Old_Motor_Location = Motor_LocationSvc.DataRowToObject(row, "oldml_", "oldma_", "oldms_");


            if (!row.IsNull("newml_motor_location_id"))
                RetVal.New_Motor_Location = Motor_LocationSvc.DataRowToObject(row, "newml_", "newma_", "newms_");

            return RetVal;
        }

    }
}
