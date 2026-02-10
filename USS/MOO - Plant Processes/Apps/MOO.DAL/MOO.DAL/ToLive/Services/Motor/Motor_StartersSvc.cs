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
    public class Motor_StartersSvc
    {
        static Motor_StartersSvc()
        {
            Util.RegisterOracle();
        }



        /// <summary>
        /// Gets the Motor_Starters record by the specified ID
        /// </summary>
        /// <param name="motorEquipId"></param>
        /// <returns></returns>
        public static Motor_Starters Get(int motorEquipId)
        {
            StringBuilder sql = new();

            sql.Append(GetSelect());
            sql.AppendLine($"WHERE me.motor_equipment_id = {motorEquipId}");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;

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
        public static PagedData<List<Motor_Starters>> GetPagedData(bool showDeleted, string motorSerialNbr, int? motorSiteId,
                        int? motorAreaId, string motorLocDesc,
                        int offset, int totalItems, string orderBy = "equip_serial_num", string orderDirection = "ASC")
        {
            PagedData<List<Motor_Starters>> retObj = new();
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

            string filter = AddFilters(showDeleted, motorSerialNbr, motorSiteId,
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
            sql.AppendLine("SELECT me.motor_equipment_id, me.motor_type_id,");
            sql.AppendLine("    me.motor_manufacturer_id, me.voltage_rating,");
            sql.AppendLine("    me.instruction_book, me. equip_serial_num, me.last_modified_by,");
            sql.AppendLine("    me.deleted_record, me.last_modified_date,");
            //motor_starters fields
            sql.AppendLine("    starter.notes, starter.starter_type, starter.current_rating,");
            //motor status fields
            sql.AppendLine("    stat.motor_status_id stat_motor_status_id,stat.motor_status_name stat_motor_status_name, ");
            sql.AppendLine("    stat.last_modified_by stat_last_modified_by,");
            sql.AppendLine("    stat.deleted_record stat_deleted_record, stat.last_modified_date stat_last_modified_date,");
            //motor location fields
            sql.AppendLine(Motor_LocationSvc.GetFields("ml", "loc_"));
            //Motor Area fields
            sql.AppendLine("    ma.motor_area_id ma_motor_area_id,motor_area_desc ma_motor_area_desc, ");
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
            sql.AppendLine("");
            if (!string.IsNullOrEmpty(addField))
            {
                sql.AppendLine($",{addField}");
            }


            sql.AppendLine("FROM tolive.motor_equipment me");
            sql.AppendLine("INNER JOIN tolive.motor_starters starter");
            sql.AppendLine("    ON starter.motor_equipment_id = me.motor_equipment_id");
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
        /// Inserts the starter  into Oracle
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static int Insert(Motor_Starters ms)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            OracleTransaction trans = conn.BeginTransaction();
            try
            {
                int recs = Insert(ms, conn);
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
        /// Inserts the Starter  into Oracle
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Insert(Motor_Starters ms, OracleConnection conn)
        {

            if (ms.Motor_Equipment_Id <= 0)
                ms.Motor_Equipment_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.motor_seq"));

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
            ins.Parameters.Add("motor_equipment_id", ms.Motor_Equipment_Id);
            ins.Parameters.Add("motor_type_id", (int)ms.EquipType);
            ins.Parameters.Add("motor_status_id", ms.Motor_Status.Motor_Status_Id);
            ins.Parameters.Add("motor_location_id", ms.Motor_Location.Motor_Location_Id);
            ins.Parameters.Add("motor_manufacturer_id", ms.Motor_Manufacturer.Motor_Manufacturer_Id);
            ins.Parameters.Add("voltage_rating", ms.Voltage_Rating);
            ins.Parameters.Add("instruction_book", ms.Instruction_Book);
            ins.Parameters.Add("equip_serial_num", ms.Equip_Serial_Num);
            ins.Parameters.Add("last_modified_by", ms.Last_Modified_By);
            ins.Parameters.Add("deleted_record", ms.Deleted_Record ? 1 : 0);
            ins.Parameters.Add("last_modified_date", DateTime.Now);

            ins.ExecuteNonQuery();

            sql.Clear();
            sql.AppendLine("INSERT INTO tolive.motor_starters (motor_equipment_id, notes, ");
            sql.AppendLine("    starter_type, current_rating,");
            sql.AppendLine("    last_modified_by, deleted_record, last_modified_date)");
            sql.AppendLine("VALUES(:motor_equipment_id, :notes, ");
            sql.AppendLine("    :starter_type, :current_rating, ");
            sql.AppendLine("    :last_modified_by, :deleted_record, :last_modified_date)");

            ins = new(sql.ToString(), conn);
            ins.Parameters.Add("motor_equipment_id", ms.Motor_Equipment_Id);
            ins.Parameters.Add("notes", ms.Notes);
            ins.Parameters.Add("starter_type", ms.Starter_Type.ToString().Replace("_"," "));
            ins.Parameters.Add("current_rating", ms.Current_Rating);
            ins.Parameters.Add("last_modified_by", ms.Last_Modified_By);
            ins.Parameters.Add("deleted_record", ms.Deleted_Record ? 1 : 0);
            ins.Parameters.Add("last_modified_date", DateTime.Now);

            return ins.ExecuteNonQuery();
        }


        /// <summary>
        /// Updates the starter  into Oracle
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static int Update(Motor_Starters ms)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            OracleTransaction trans = conn.BeginTransaction();
            try
            {
                int recs = Update(ms, conn);
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
        /// Updates the starter  into Oracle
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Update(Motor_Starters ms, OracleConnection conn)
        {

            if (ms.Motor_Equipment_Id <= 0)
                ms.Motor_Equipment_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.motor_seq"));

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
            upd.Parameters.Add("motor_type_id", (int)ms.EquipType);
            upd.Parameters.Add("motor_status_id", ms.Motor_Status.Motor_Status_Id);
            upd.Parameters.Add("motor_location_id", ms.Motor_Location.Motor_Location_Id);
            upd.Parameters.Add("motor_manufacturer_id", ms.Motor_Manufacturer.Motor_Manufacturer_Id);
            upd.Parameters.Add("voltage_rating", ms.Voltage_Rating);
            upd.Parameters.Add("instruction_book", ms.Instruction_Book);
            upd.Parameters.Add("equip_serial_num", ms.Equip_Serial_Num);
            upd.Parameters.Add("last_modified_by", ms.Last_Modified_By);
            upd.Parameters.Add("deleted_record", ms.Deleted_Record ? 1 : 0);
            upd.Parameters.Add("last_modified_date", DateTime.Now);
            upd.Parameters.Add("motor_equipment_id", ms.Motor_Equipment_Id);

            upd.ExecuteNonQuery();

            sql.Clear();
            sql.AppendLine("UPDATE tolive.motor_starters SET notes = :notes, starter_type = :starter_type, ");
            sql.AppendLine("    current_rating = :current_rating, ");
            sql.AppendLine("    last_modified_by = :last_modified_by,");
            sql.AppendLine("    deleted_record = :deleted_record,");
            sql.AppendLine("    last_modified_date = :last_modified_date");
            sql.AppendLine("WHERE motor_equipment_id = :motor_equipment_id");

            upd = new(sql.ToString(), conn);
            upd.Parameters.Add("notes", ms.Notes);
            upd.Parameters.Add("starter_type", ms.Starter_Type.ToString().Replace("_", " "));
            upd.Parameters.Add("current_rating", ms.Current_Rating);
            upd.Parameters.Add("last_modified_by", ms.Last_Modified_By);
            upd.Parameters.Add("deleted_record", ms.Deleted_Record ? 1 : 0);
            upd.Parameters.Add("last_modified_date", DateTime.Now);
            upd.Parameters.Add("motor_equipment_id", ms.Motor_Equipment_Id);

            return upd.ExecuteNonQuery();
        }


        /// <summary>
        /// Deletes the starter into Oracle
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static int Delete(Motor_Starters ms)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            OracleTransaction trans = conn.BeginTransaction();
            try
            {
                int recs = Delete(ms, conn);
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
        /// Deletes the Starter into Oracle
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static int Delete(Motor_Starters ms, OracleConnection conn)
        {
            ms.Deleted_Record = true;
            return Update(ms, conn);
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
        private static Motor_Starters DataRowToObject(DataRow row, string fieldPrefix = "", string motorAreaPrefix = "ma_",
                            string motorSitePrefix = "ms_", string motorLocationPrefix = "loc_", string manufacturerPrefix = "man_",
                            string statusPrefix = "stat_")
        {
            Motor_Starters RetVal = new();

            RetVal.Motor_Equipment_Id = Convert.ToInt32(row[$"{fieldPrefix}motor_equipment_id"]);
            if (row.Field<decimal?>($"{fieldPrefix}voltage_rating").HasValue)
                RetVal.Voltage_Rating = Convert.ToInt32(row.Field<decimal>($"{fieldPrefix}voltage_rating"));
            else
                RetVal.Voltage_Rating = null;



            RetVal.Instruction_Book = row.Field<string>($"{fieldPrefix}Instruction_Book");
            RetVal.Equip_Serial_Num = row.Field<string>($"{fieldPrefix}equip_serial_num");
            RetVal.Notes = row.Field<string>($"{fieldPrefix}notes");
            RetVal.Starter_Type = Enum.Parse<Motor_Starters.StarterType>(row.Field<string>($"{fieldPrefix}starter_type").Replace(" ", "_"));
            RetVal.Current_Rating = Convert.ToInt32(row.Field<decimal>($"{fieldPrefix}current_rating"));

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
