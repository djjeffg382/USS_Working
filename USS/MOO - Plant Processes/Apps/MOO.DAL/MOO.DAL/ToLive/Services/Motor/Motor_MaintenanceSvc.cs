using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;

namespace MOO.DAL.ToLive.Services
{
    public static class Motor_MaintenanceSvc
    {

        static Motor_MaintenanceSvc()
        {
            Util.RegisterOracle();
        }


        /// <summary>
        /// Gets the Motor_Maintenance by the specified ID
        /// </summary>
        /// <param name="motorMaintId"></param>
        /// <returns></returns>
        public static Motor_Maintenance Get(int motorMaintId)
        {
            StringBuilder sql = new();

            sql.Append(GetSelect());
            sql.AppendLine($"WHERE maint.motor_maintenance_id = {motorMaintId}");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;

        }

        /// <summary>
        /// Gets the Motor_Maintenance by the specified motor id
        /// </summary>
        /// <param name="motorEquipmentId"></param>
        /// <returns></returns>
        public static List<Motor_Maintenance> GetByMotor(int motorEquipmentId)
        {
            List<Motor_Maintenance> retVal = new();
            StringBuilder sql = new();

            sql.Append(GetSelect());
            sql.AppendLine($"WHERE maint.motor_equipment_id = {motorEquipmentId}");
            sql.AppendLine("ORDER BY maint.date_of_maint");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            foreach (DataRow row in ds.Tables[0].Rows)
                retVal.Add(DataRowToObject(row));

            return retVal;

        }



        /// <summary>
        /// Inserts the Motor_Maintenance into Oracle
        /// </summary>
        /// <param name="mt"></param>
        /// <returns></returns>
        public static int Insert(Motor_Maintenance mt)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                int recsAffected = Insert(mt, conn);
                return recsAffected;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Inserts the Motor_Maintenance into Oracle
        /// </summary>
        /// <param name="mt"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Insert(Motor_Maintenance mt, OracleConnection conn)
        {
            if (mt.Motor_Maintenance_Id <= 0)
                mt.Motor_Maintenance_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.motor_seq"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.motor_maintenance (motor_maintenance_id, motor_equipment_id, ");
            sql.AppendLine("    date_of_maint, notes, ");
            sql.AppendLine("    work_order, maint_done_by, ");
            sql.AppendLine(" last_modified_by, deleted_record, last_modified_date)");
            sql.AppendLine("VALUES(:motor_maintenance_id, :motor_equipment_id, ");
            sql.AppendLine("    :date_of_maint, :notes, ");
            sql.AppendLine("    :work_order, :maint_done_by, ");
            sql.AppendLine(" :last_modified_by, :deleted_record, :last_modified_date)");

            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("motor_maintenance_id", mt.Motor_Maintenance_Id);
            ins.Parameters.Add("motor_equipment_id", mt.Motor_Equipment.Motor_Equipment_Id);
            ins.Parameters.Add("date_of_maint", mt.Date_Of_Maint);
            ins.Parameters.Add("notes", mt.Notes);
            ins.Parameters.Add("work_order", mt.Work_Order);
            ins.Parameters.Add("maint_done_by", mt.Maint_Done_By);
            ins.Parameters.Add("last_modified_by", mt.Last_Modified_By);
            ins.Parameters.Add("deleted_record", mt.Deleted_Record ? 1 : 0);
            ins.Parameters.Add("last_modified_date", DateTime.Now);

            int recsAffected = ins.ExecuteNonQuery();

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
            sql.AppendLine("SELECT ");
            sql.AppendLine("    maint.motor_maintenance_id, maint.date_of_maint, maint.notes, ");
            sql.AppendLine("    maint.work_order, maint.maint_done_by,");
            sql.AppendLine("    maint.last_modified_by, maint.deleted_record,");
            sql.AppendLine("    maint.last_modified_date,");


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
            sql.AppendLine("    man.last_modified_date man_last_modified_date");

            if (!string.IsNullOrEmpty(addField))
            {
                sql.AppendLine($",{addField}");
            }

            sql.AppendLine("FROM tolive.motor_maintenance maint");
            sql.AppendLine("INNER JOIN tolive.motor_equipment me");
            sql.AppendLine("    ON maint.motor_equipment_id = me.motor_equipment_id");
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

            return sql.ToString();
        }



        private static Motor_Maintenance DataRowToObject(DataRow row, string fieldPrefix = "")
        {
            Motor_Maintenance RetVal = new();


            RetVal.Motor_Maintenance_Id = Convert.ToInt32(row.Field<decimal>($"{fieldPrefix}motor_maintenance_id"));
            RetVal.Date_Of_Maint = row.Field<DateTime?>($"{fieldPrefix}date_of_maint");
            RetVal.Notes = row.Field<string>($"{fieldPrefix}notes");
            RetVal.Work_Order = row.Field<string>($"{fieldPrefix}work_order");
            RetVal.Maint_Done_By = row.Field<string>($"{fieldPrefix}maint_done_by");

            RetVal.Last_Modified_By = row.Field<string>($"{fieldPrefix}last_modified_by");
            RetVal.Deleted_Record = Convert.ToInt32(row[$"{fieldPrefix}deleted_record"]) == 1;
            RetVal.Last_Modified_Date = row.Field<DateTime?>($"{fieldPrefix}last_modified_date");

            RetVal.Motor_Equipment = Motor_MotorsSvc.DataRowToObject(row, "mm_");

            return RetVal;
        }
    }
}
