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
    public class Motor_MotorsSvc
    {
        static Motor_MotorsSvc()
        {
            Util.RegisterOracle();
        }

        /// <summary>
        /// Gets the Motor_Motors record by the specified ID
        /// </summary>
        /// <param name="motorEquipId"></param>
        /// <returns></returns>
        public static Motor_Motors Get(int motorEquipId)
        {
            StringBuilder sql = new();

            sql.Append(GetSelect());
            sql.AppendLine($"WHERE mm.motor_equipment_id = {motorEquipId}");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;

        }


        public static List<Motor_Motors> GetAll(bool showDeleted = false)
        {
            List<Motor_Motors> retObj = new();
            StringBuilder sql = new();

            sql.Append(GetSelect());
            if (!showDeleted)
                sql.AppendLine("WHERE mm.deleted_record = 0");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                retObj.Add(DataRowToObject(row));
            }
            return retObj;

        }


        /// <summary>
        /// Gets a paged list of data for use in a paged table
        /// </summary>
        /// <param name="showDeleted"></param>
        /// <param name="motorSerialNbr"></param>
        /// <param name="motorSiteId"></param>
        /// <param name="motorAreaId"></param>
        /// <param name="motorLocDesc"></param>
        /// <param name="totalItems"></param>
        /// <param name="orderBy"></param>
        /// <param name="orderDirection"></param>
        /// <returns></returns>
        public static PagedData<List<Motor_Motors>> GetPagedData(bool showDeleted, string motorSerialNbr, int? motorSiteId,
                        int? motorAreaId, string motorLocDesc, 
                        int offset, int totalItems, string orderBy = "equip_serial_num", string orderDirection = "ASC")
        {
            PagedData<List<Motor_Motors>> retObj = new();
            StringBuilder sql = new();
            OracleDataAdapter da = new("", MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            string filter = AddFilters(showDeleted, motorSerialNbr, motorSiteId,
                         motorAreaId, motorLocDesc, da.SelectCommand);

            sql.AppendLine("SELECT * FROM (");
            sql.AppendLine(GetSelect($"ROW_NUMBER() OVER(ORDER BY {orderBy} {orderDirection}) rn"));
            sql.AppendLine(filter);
            sql.AppendLine(") tbl");
            sql.AppendLine($"WHERE rn BETWEEN {offset} AND {offset + totalItems}");
            da.SelectCommand.CommandText = sql.ToString();
            DataSet ds = MOO.Data.ExecuteQuery(da);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                retObj.Data.Add(DataRowToObject(row));
            }
            retObj.TotalRows = GetMotorCount(showDeleted, motorSerialNbr, motorSiteId,
                                                        motorAreaId, motorLocDesc);

            return retObj;

        }


        private static int GetMotorCount(bool showDeleted, string motorSerialNbr, int? motorSiteId,
                        int? motorAreaId, string motorLocDesc)
        {
            StringBuilder sql = new();
            OracleDataAdapter da = new("", MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));

            string filter = AddFilters(showDeleted,motorSerialNbr, motorSiteId,
                                                    motorAreaId, motorLocDesc, da.SelectCommand);

            sql.AppendLine("SELECT COUNT(*) FROM tolive.motor_equipment me");
            sql.AppendLine("INNER JOIN tolive.motor_location ml");
            sql.AppendLine("    ON me.motor_location_id = ml.motor_location_id");
            sql.AppendLine("INNER JOIN tolive.motor_area ma");
            sql.AppendLine("    ON ml.motor_area_id = ma.motor_area_id");
            sql.AppendLine("INNER JOIN tolive.motor_site ms");
            sql.AppendLine("    ON ma.motor_site_id = ms.motor_site_id");
            sql.AppendLine(filter);
            da.SelectCommand.CommandText = sql.ToString();
            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            }
            else
                return 0;
        }



        private static string AddFilters(bool showDeleted, string motorSerialNbr, int? motorSiteId,
                        int? motorAreaId, string motorLocDesc, OracleCommand cmd)
        {
            StringBuilder filter = new();

            if (!showDeleted)
            {
                filter.AppendLine("WHERE me.deleted_record = 0");
            }

            /*****************motorAreaId Parameter*************/
            if (motorAreaId.HasValue)
            {
                if (filter.Length == 0)
                    filter.Append("WHERE ");
                else
                    filter.Append("AND ");

                filter.AppendLine("ml.motor_area_id = :motor_area_id");
                cmd.Parameters.Add("motor_area_id", motorAreaId.Value);
            }
            /*****************Motor Site ID Parameter*************/
            if (motorAreaId.HasValue)
            {
                if (filter.Length == 0)
                    filter.Append("WHERE ");
                else
                    filter.Append("AND ");

                filter.AppendLine("ms.motor_site_id = :motor_site_id");
                cmd.Parameters.Add("motor_site_id", motorSiteId.Value);
            }
            /*****************equip serial num Parameter*************/
            if (!string.IsNullOrEmpty(motorSerialNbr))
            {
                if (filter.Length == 0)
                    filter.Append("WHERE ");
                else
                    filter.Append("AND ");

                filter.AppendLine("UPPER(me.equip_serial_num) LIKE :equip_serial_num");
                cmd.Parameters.Add("equip_serial_num", $"%{motorSerialNbr.ToUpper()}%");
            }
            /*****************motorLocDesc Parameter*************/
            if (!string.IsNullOrEmpty(motorLocDesc))
            {
                if (filter.Length == 0)
                    filter.Append("WHERE ");
                else
                    filter.Append("AND ");

                filter.AppendLine("UPPER(ml.motor_location_desc) LIKE :motor_location_desc");
                cmd.Parameters.Add("motor_location_desc", $"%{motorLocDesc.ToUpper()}%");
            }
            return filter.ToString();
        }


        /// <summary>
        /// returns the select, from, and join portion of the query
        /// </summary>
        /// <param name="addField">any additional fields to add to the query</param>
        /// <returns></returns>
        private static string GetSelect(string addField = "")
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT me.motor_equipment_id, me.motor_type_id,me.motor_status_id,");
            sql.AppendLine("    me.voltage_rating,");
            sql.AppendLine("    me.instruction_book, me.equip_serial_num, me.last_modified_by,");
            sql.AppendLine("    me.deleted_record, me.last_modified_date,");
            //motor_motors fields
            sql.AppendLine("    mm.notes, mm.motor_type, mm.frame_size, mm.horse_power, mm.rpm, mm.inner_bearing,");
            sql.AppendLine("    mm.outer_bearing, mm.phase, mm.full_load_amps, mm.purchase_date,");
            sql.AppendLine("    mm.inverter_rated, mm.drive_configuration, mm.explosion_proof, mm.weight,");
            sql.AppendLine("    mm.j_box_setup, mm.rtd_type, mm.insulation_class, mm.service_factor,");
            sql.AppendLine("    mm.critical_motor, mm.catalog_id_number,");
            //motor status fields
            sql.AppendLine("    stat.motor_status_id stat_motor_status_id,stat.motor_status_name stat_motor_status_name, ");
            sql.AppendLine("    stat.last_modified_by stat_last_modified_by,");
            sql.AppendLine("    stat.deleted_record stat_deleted_record, stat.last_modified_date stat_last_modified_date,");
            //motor location fields
            sql.AppendLine(Motor_LocationSvc.GetFields("ml", "loc_"));
            //Motor Area fields
            sql.AppendLine("    ma.motor_area_id ma_motor_area_id,motor_area_desc ma_motor_area_desc,");
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


            sql.AppendLine("FROM tolive.motor_equipment me");
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


        /// <summary>
        /// Inserts the Motor  into Oracle
        /// </summary>
        /// <param name="mm"></param>
        /// <returns></returns>
        public static int Insert(Motor_Motors mm)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            OracleTransaction trans = conn.BeginTransaction();
            try
            {
                int recs = Insert(mm, conn);
                trans.Commit();
                return recs;
            }
            catch (Exception)
            {
                if (trans != null)
                    trans.Rollback();
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Inserts the Motor  into Oracle
        /// </summary>
        /// <param name="mm"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Insert(Motor_Motors mm, OracleConnection conn)
        {

            if (mm.Motor_Equipment_Id <= 0)
                mm.Motor_Equipment_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.motor_seq"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.motor_equipment (motor_equipment_id, motor_type_id, ");
            sql.AppendLine("    motor_status_id, motor_location_id, motor_manufacturer_id,");
            sql.AppendLine("    voltage_rating, instruction_book, equip_serial_num, ");
            sql.AppendLine("    last_modified_by, deleted_record, last_modified_date)");
            sql.AppendLine("VALUES(:motor_equipment_id, :motor_type_id, ");
            sql.AppendLine("    :motor_status_id, :motor_location_id, :motor_manufacturer_id,");
            sql.AppendLine("    :voltage_rating, :instruction_book, :equip_serial_num, ");
            sql.AppendLine("    :last_modified_by, :deleted_record, :last_modified_date)");

            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("motor_equipment_id", mm.Motor_Equipment_Id);
            ins.Parameters.Add("motor_type_id", (int)mm.EquipType);
            ins.Parameters.Add("motor_status_id", mm.Motor_Status.Motor_Status_Id);
            ins.Parameters.Add("motor_location_id", mm.Motor_Location.Motor_Location_Id);
            ins.Parameters.Add("motor_manufacturer_id", mm.Motor_Manufacturer.Motor_Manufacturer_Id);
            ins.Parameters.Add("voltage_rating", mm.Voltage_Rating);
            ins.Parameters.Add("instruction_book", mm.Instruction_Book);
            ins.Parameters.Add("equip_serial_num", mm.Equip_Serial_Num);
            ins.Parameters.Add("last_modified_by", mm.Last_Modified_By);
            ins.Parameters.Add("deleted_record", mm.Deleted_Record ? 1 : 0);
            ins.Parameters.Add("last_modified_date", DateTime.Now);

            ins.ExecuteNonQuery();

            sql.Clear();
            sql.AppendLine("INSERT INTO tolive.motor_motors (motor_equipment_id, notes, ");
            sql.AppendLine("    motor_type, frame_size, horse_power,");
            sql.AppendLine("    rpm, inner_bearing, outer_bearing, ");
            sql.AppendLine("    phase, full_load_amps, purchase_date, ");
            sql.AppendLine("    inverter_rated, drive_configuration, explosion_proof,");
            sql.AppendLine("    weight, j_box_setup, rtd_type, insulation_class, ");
            sql.AppendLine("    service_factor, critical_motor, catalog_id_number,");
            sql.AppendLine("    last_modified_by, deleted_record, last_modified_date)");
            sql.AppendLine("VALUES(:motor_equipment_id, :notes, ");
            sql.AppendLine("    :motor_type, :frame_size, :horse_power,");
            sql.AppendLine("    :rpm, :inner_bearing, :outer_bearing, ");
            sql.AppendLine("    :phase, :full_load_amps, :purchase_date, ");
            sql.AppendLine("    :inverter_rated, :drive_configuration, :explosion_proof,");
            sql.AppendLine("    :weight, :j_box_setup, :rtd_type, :insulation_class, ");
            sql.AppendLine("    :service_factor, :critical_motor, :catalog_id_number,");
            sql.AppendLine("    :last_modified_by, :deleted_record, :last_modified_date)");

            ins = new(sql.ToString(), conn);
            ins.Parameters.Add("motor_equipment_id", mm.Motor_Equipment_Id);
            ins.Parameters.Add("notes", mm.Notes);
            ins.Parameters.Add("motor_type", mm.Motor_Type);
            ins.Parameters.Add("frame_size", mm.Frame_Size);
            ins.Parameters.Add("horse_power", mm.Horse_Power);
            ins.Parameters.Add("rpm", mm.Rpm);
            ins.Parameters.Add("inner_bearing", mm.Inner_Bearing);
            ins.Parameters.Add("outer_bearing", mm.Outer_Bearing);
            ins.Parameters.Add("phase", mm.Phase);
            ins.Parameters.Add("full_load_amps", mm.Full_Load_Amps);
            ins.Parameters.Add("purchase_date", mm.Purchase_Date);
            ins.Parameters.Add("inverter_rated", mm.Inverter_Rated.HasValue ? mm.Inverter_Rated.Value.ToString() : null);
            ins.Parameters.Add("drive_configuration",
                        mm.Drive_Configuration == Motor_Motors.Drive_Config.Unknown ? null : mm.Drive_Configuration.ToString());

            ins.Parameters.Add("explosion_proof", mm.Explosion_Proof.HasValue ? mm.Explosion_Proof.ToString() : null);
            ins.Parameters.Add("weight", mm.Weight);
            switch (mm.J_Box_Setup)
            {
                case Motor_Motors.JBox.Unknown:
                    ins.Parameters.Add("j_box_setup", null);
                    break;
                case Motor_Motors.JBox.Dbl_Shaft:
                    ins.Parameters.Add("j_box_setup", "DBL SHAFT");
                    break;
                case Motor_Motors.JBox.F2_Or_Vertical:
                    ins.Parameters.Add("j_box_setup", "F2 OR VERT");
                    break;
                default:
                    ins.Parameters.Add("j_box_setup", mm.J_Box_Setup.ToString().ToUpper());
                    break;

            }

            ins.Parameters.Add("rtd_type", mm.RTD_Type);
            ins.Parameters.Add("insulation_class", mm.Insulation_Class);
            ins.Parameters.Add("service_factor", mm.Service_Factor);
            ins.Parameters.Add("critical_motor", mm.Critical_Motor.HasValue ? mm.Critical_Motor.ToString() : null);
            ins.Parameters.Add("catalog_id_number", mm.Catalog_Id_Number);
            ins.Parameters.Add("last_modified_by", mm.Last_Modified_By);
            ins.Parameters.Add("deleted_record", mm.Deleted_Record ? 1 : 0);
            ins.Parameters.Add("last_modified_date", DateTime.Now);

            return ins.ExecuteNonQuery();
        }


        /// <summary>
        /// Updates the Motor  into Oracle
        /// </summary>
        /// <param name="mm"></param>
        /// <returns></returns>
        public static int Update(Motor_Motors mm)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            OracleTransaction trans = conn.BeginTransaction();
            try
            {
                int recs = Update(mm, conn);
                trans.Commit();
                return recs;
            }
            catch (Exception)
            {
                if (trans != null)
                    trans.Rollback();
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Updates the Motor  into Oracle
        /// </summary>
        /// <param name="mm"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Update(Motor_Motors mm, OracleConnection conn)
        {

            if (mm.Motor_Equipment_Id <= 0)
                mm.Motor_Equipment_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.motor_seq"));

            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.motor_equipment ");
            sql.AppendLine("SET motor_type_id = :motor_type_id, motor_status_id = :motor_status_id,");
            sql.AppendLine("    motor_location_id = :motor_location_id, motor_manufacturer_id = :motor_manufacturer_id,");
            sql.AppendLine("    voltage_rating = :voltage_rating, instruction_book = :instruction_book,");
            sql.AppendLine("    equip_serial_num = :equip_serial_num, ");
            sql.AppendLine("    last_modified_by = :last_modified_by,");
            sql.AppendLine("    deleted_record = :deleted_record,");
            sql.AppendLine("    last_modified_date = :last_modified_date");
            sql.AppendLine("WHERE motor_equipment_id = :motor_equipment_id");

            OracleCommand upd = new(sql.ToString(), conn);
            upd.Parameters.Add("motor_type_id", (int)mm.EquipType);
            upd.Parameters.Add("motor_status_id", mm.Motor_Status.Motor_Status_Id);
            upd.Parameters.Add("motor_location_id", mm.Motor_Location.Motor_Location_Id);
            upd.Parameters.Add("motor_manufacturer_id", mm.Motor_Manufacturer.Motor_Manufacturer_Id);
            upd.Parameters.Add("voltage_rating", mm.Voltage_Rating);
            upd.Parameters.Add("instruction_book", mm.Instruction_Book);
            upd.Parameters.Add("equip_serial_num", mm.Equip_Serial_Num);
            upd.Parameters.Add("last_modified_by", mm.Last_Modified_By);
            upd.Parameters.Add("deleted_record", mm.Deleted_Record ? 1 : 0);
            upd.Parameters.Add("last_modified_date", DateTime.Now);
            upd.Parameters.Add("motor_equipment_id", mm.Motor_Equipment_Id);

            upd.ExecuteNonQuery();

            sql.Clear();
            sql.AppendLine("UPDATE tolive.motor_motors SET notes = :notes, motor_type = :motor_type, ");
            sql.AppendLine("    frame_size = :frame_size, horse_power = :horse_power, rpm = :rpm,");
            sql.AppendLine("    inner_bearing = :inner_bearing, outer_bearing = :outer_bearing,");
            sql.AppendLine("    phase = :phase, full_load_amps = :full_load_amps, purchase_date = :purchase_date,");
            sql.AppendLine("    inverter_rated = :inverter_rated, drive_configuration = :drive_configuration,");
            sql.AppendLine("    explosion_proof = :explosion_proof, weight = :weight, j_box_setup = :j_box_setup,");
            sql.AppendLine("    rtd_type = :rtd_type, insulation_class = :insulation_class,");
            sql.AppendLine("    service_factor = :service_factor, critical_motor = :critical_motor,");
            sql.AppendLine("    catalog_id_number = :catalog_id_number,");
            sql.AppendLine("    last_modified_by = :last_modified_by,");
            sql.AppendLine("    deleted_record = :deleted_record,");
            sql.AppendLine("    last_modified_date = :last_modified_date");
            sql.AppendLine("WHERE motor_equipment_id = :motor_equipment_id");

            upd = new(sql.ToString(), conn);
            upd.Parameters.Add("notes", mm.Notes);
            upd.Parameters.Add("motor_type", mm.Motor_Type);
            upd.Parameters.Add("frame_size", mm.Frame_Size);
            upd.Parameters.Add("horse_power", mm.Horse_Power);
            upd.Parameters.Add("rpm", mm.Rpm);
            upd.Parameters.Add("inner_bearing", mm.Inner_Bearing);
            upd.Parameters.Add("outer_bearing", mm.Outer_Bearing);
            upd.Parameters.Add("phase", mm.Phase);
            upd.Parameters.Add("full_load_amps", mm.Full_Load_Amps);
            upd.Parameters.Add("purchase_date", mm.Purchase_Date);
            upd.Parameters.Add("inverter_rated", mm.Inverter_Rated.HasValue ? mm.Inverter_Rated.Value.ToString() : null);
            upd.Parameters.Add("drive_configuration",
                        mm.Drive_Configuration == Motor_Motors.Drive_Config.Unknown ? null : mm.Drive_Configuration.ToString());

            upd.Parameters.Add("explosion_proof", mm.Explosion_Proof.HasValue ? mm.Explosion_Proof.ToString() : null);
            upd.Parameters.Add("weight", mm.Weight);
            switch (mm.J_Box_Setup)
            {
                case Motor_Motors.JBox.Unknown:
                    upd.Parameters.Add("j_box_setup", null);
                    break;
                case Motor_Motors.JBox.Dbl_Shaft:
                    upd.Parameters.Add("j_box_setup", "DBL SHAFT");
                    break;
                case Motor_Motors.JBox.F2_Or_Vertical:
                    upd.Parameters.Add("j_box_setup", "F2 OR VERT");
                    break;
                default:
                    upd.Parameters.Add("j_box_setup", mm.J_Box_Setup.ToString().ToUpper());
                    break;

            }

            upd.Parameters.Add("rtd_type", mm.RTD_Type);
            upd.Parameters.Add("insulation_class", mm.Insulation_Class);
            upd.Parameters.Add("service_factor", mm.Service_Factor);
            upd.Parameters.Add("critical_motor", mm.Critical_Motor.HasValue ? mm.Critical_Motor.ToString() : null);
            upd.Parameters.Add("catalog_id_number", mm.Catalog_Id_Number);
            upd.Parameters.Add("last_modified_by", mm.Last_Modified_By);
            upd.Parameters.Add("deleted_record", mm.Deleted_Record ? 1 : 0);
            upd.Parameters.Add("last_modified_date", DateTime.Now);
            upd.Parameters.Add("motor_equipment_id", mm.Motor_Equipment_Id);

            return upd.ExecuteNonQuery();
        }


        /// <summary>
        /// Deletes the Motor  into Oracle
        /// </summary>
        /// <param name="mm"></param>
        /// <returns></returns>
        public static int Delete(Motor_Motors mm)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            OracleTransaction trans = conn.BeginTransaction();
            try
            {
                int recs = Delete(mm, conn);
                trans.Commit();
                return recs;
            }
            catch (Exception)
            {
                if (trans != null)
                    trans.Rollback();
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Deletes the Motor  into Oracle
        /// </summary>
        /// <param name="mm"></param>
        /// <returns></returns>
        public static int Delete(Motor_Motors mm, OracleConnection conn)
        {
            mm.Deleted_Record = true;
            return Update(mm, conn);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldPrefix">The string prefix for all of the motor location fields in the row</param>
        /// <param name="motorSitePrefix">prefix used for the Motor Site fields in the row</param>
        /// <param name="motorAreaPrefix">prefix used for the Motor area fields in the row</param>
        /// <param name="manufacturerPrefix">prefix used for the Motor manufacturer fields in the row</param>
        /// <param name="motorLocationPrefix">prefix used for the location fields in the row</param>
        /// <param name="statusPrefix">prefix used for the status fields in the row</param>
        /// <returns></returns>
        internal static Motor_Motors DataRowToObject(DataRow row, string fieldPrefix = "", string motorAreaPrefix = "ma_", 
                            string motorSitePrefix = "ms_", string motorLocationPrefix = "loc_", string manufacturerPrefix = "man_",
                            string statusPrefix = "stat_")
        {
            Motor_Motors RetVal = new();

            RetVal.Motor_Equipment_Id = Convert.ToInt32(row[$"{fieldPrefix}motor_equipment_id"]);
            if (row.Field<decimal?>($"{fieldPrefix}voltage_rating").HasValue)
                RetVal.Voltage_Rating = Convert.ToInt32(row.Field<decimal>($"{fieldPrefix}voltage_rating"));
            else
                RetVal.Voltage_Rating = null;



            RetVal.Instruction_Book = row.Field<string>($"{fieldPrefix}Instruction_Book");
            RetVal.Equip_Serial_Num = row.Field<string>($"{fieldPrefix}equip_serial_num");
            RetVal.Notes = row.Field<string>($"{fieldPrefix}notes");
            RetVal.Motor_Type = row.Field<string>($"{fieldPrefix}motor_type");
            RetVal.Frame_Size = row.Field<string>($"{fieldPrefix}frame_size");
            RetVal.Horse_Power = row.Field<decimal?>($"{fieldPrefix}horse_power");
            if (row.Field<decimal?>($"{fieldPrefix}rpm").HasValue)
                RetVal.Rpm = Convert.ToInt32(row.Field<decimal>($"{fieldPrefix}rpm"));
            else
                RetVal.Rpm = null;
            RetVal.Inner_Bearing = row.Field<string>($"{fieldPrefix}inner_bearing");
            RetVal.Outer_Bearing = row.Field<string>($"{fieldPrefix}outer_bearing");
            if (row.Field<decimal?>($"{fieldPrefix}phase").HasValue)
                RetVal.Phase = Convert.ToInt32(row.Field<decimal>($"{fieldPrefix}phase"));
            else
                RetVal.Phase = null;

            RetVal.Full_Load_Amps = row.Field<decimal?>($"{fieldPrefix}full_load_amps");
            RetVal.Purchase_Date = row.Field<DateTime?>($"{fieldPrefix}purchase_date");

            if (!row.IsNull($"{fieldPrefix}inverter_rated"))
                RetVal.Inverter_Rated = bool.Parse(row.Field<string>($"{fieldPrefix}inverter_rated").ToLower());
            else
                RetVal.Inverter_Rated = null;
            if (!row.IsNull($"{fieldPrefix}drive_configuration"))
            {
                switch (row.Field<string>($"{fieldPrefix}drive_configuration"))
                {
                    case "Direct":
                        RetVal.Drive_Configuration = Motor_Motors.Drive_Config.Direct;
                        break;
                    case "Belt":
                        RetVal.Drive_Configuration = Motor_Motors.Drive_Config.Belt;
                        break;
                    case "Either":
                        RetVal.Drive_Configuration = Motor_Motors.Drive_Config.Either;
                        break;
                    case "Sump":
                        RetVal.Drive_Configuration = Motor_Motors.Drive_Config.Sump;
                        break;
                }
            }
            else
                RetVal.Drive_Configuration = Motor_Motors.Drive_Config.Unknown;


            if (!row.IsNull($"{fieldPrefix}explosion_proof"))
                RetVal.Explosion_Proof = bool.Parse(row.Field<string>($"{fieldPrefix}explosion_proof").ToLower());
            else
                RetVal.Explosion_Proof = null;

            if (row.Field<decimal?>($"{fieldPrefix}weight").HasValue)
                RetVal.Weight = Convert.ToInt32(row.Field<decimal>($"{fieldPrefix}weight"));
            else
                RetVal.Weight = null;

            if (!row.IsNull($"{fieldPrefix}j_box_setup"))
            {
                switch (row.Field<string>($"{fieldPrefix}j_box_setup").Trim())
                {
                    case "F1":
                        RetVal.J_Box_Setup = Motor_Motors.JBox.F1;
                        break;
                    case "F2":
                        RetVal.J_Box_Setup = Motor_Motors.JBox.F2;
                        break;
                    case "F3":
                        RetVal.J_Box_Setup = Motor_Motors.JBox.F3;
                        break;
                    case "F4":
                        RetVal.J_Box_Setup = Motor_Motors.JBox.F4;
                        break;
                    case "VERTICAL":
                        RetVal.J_Box_Setup = Motor_Motors.JBox.Vertical;
                        break;
                    case "F2 OR VERT":
                        RetVal.J_Box_Setup = Motor_Motors.JBox.F2_Or_Vertical;
                        break;
                    case "DBL SHAFT":
                        RetVal.J_Box_Setup = Motor_Motors.JBox.Dbl_Shaft;
                        break;
                }
            }
            else
                RetVal.J_Box_Setup = Motor_Motors.JBox.Unknown;


            RetVal.RTD_Type = row.Field<string>($"{fieldPrefix}rtd_type");
            RetVal.Insulation_Class = row.Field<string>($"{fieldPrefix}insulation_class");
            RetVal.Service_Factor = row.Field<double?>($"{fieldPrefix}service_factor");

            if (!row.IsNull($"{fieldPrefix}critical_motor"))
                RetVal.Critical_Motor = bool.Parse(row.Field<string>($"{fieldPrefix}critical_motor").ToLower());
            else
                RetVal.Critical_Motor = null;

            if (row.Field<decimal?>($"{fieldPrefix}catalog_id_number").HasValue)
                RetVal.Catalog_Id_Number = Convert.ToInt64(row.Field<decimal>($"{fieldPrefix}catalog_id_number"));
            else
                RetVal.Catalog_Id_Number = null;


            RetVal.Last_Modified_By = row.Field<string>($"{fieldPrefix}last_modified_by");
            RetVal.Deleted_Record = Convert.ToInt32(row[$"{fieldPrefix}deleted_record"]) == 1;
            RetVal.Last_Modified_Date = row.Field<DateTime?>($"{fieldPrefix}last_modified_date");


            RetVal.Motor_Status = Motor_StatusSvc.DataRowToObject(row, statusPrefix);
            RetVal.Motor_Location = Motor_LocationSvc.DataRowToObject(row, motorLocationPrefix, motorAreaPrefix, motorSitePrefix);
            RetVal.Motor_Manufacturer = Motor_ManufacturerSvc.DataRowToObject(row, manufacturerPrefix);

            return RetVal;
        }


    }
}
