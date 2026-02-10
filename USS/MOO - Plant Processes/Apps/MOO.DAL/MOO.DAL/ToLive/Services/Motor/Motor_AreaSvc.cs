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
    public class Motor_AreaSvc
    {
        static Motor_AreaSvc()
        {
            Util.RegisterOracle();
        }


        /// <summary>
        /// Gets the Motor_Area by the specified ID
        /// </summary>
        /// <param name="motorAreaId"></param>
        /// <returns></returns>
        public static Motor_Area Get(int motorAreaId)
        {
            StringBuilder sql = new();

            sql.AppendLine("SELECT motor_area_id ma_motor_area_id, motor_area_desc ma_motor_area_desc, ");
            sql.AppendLine(" ma.deleted_record ma_deleted_record, ma.last_modified_date ma_last_modified_date,");
            sql.AppendLine("ma.last_modified_by ma_last_modified_by,");
            //Motor site fields
            sql.AppendLine("ms.motor_site_id ms_motor_site_id,ms.motor_site_name ms_motor_site_name, ms.last_modified_by ms_last_modified_by,");
            sql.AppendLine(" ms.deleted_record ms_deleted_record, ms.last_modified_date ms_last_modified_date");
            sql.AppendLine("FROM tolive.motor_area ma");
            sql.AppendLine("INNER JOIN tolive.motor_site ms");
            sql.AppendLine("    ON ma.motor_site_id = ms.motor_site_id");
            sql.AppendLine($"WHERE ma.motor_area_id = {motorAreaId}");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0], "ma_");
            else
                return null;

        }

        /// <summary>
        /// Gets all motor_areas from Oracle
        /// </summary>
        /// <returns></returns>
        public static List<Motor_Area> GetAll(bool showDeleted)
        {
            List<Motor_Area> retVal = new();

            StringBuilder sql = new();

            sql.AppendLine("SELECT motor_area_id ma_motor_area_id, motor_area_desc ma_motor_area_desc, ");
            sql.AppendLine(" ma.deleted_record ma_deleted_record, ma.last_modified_date ma_last_modified_date,");
            sql.AppendLine("ma.last_modified_by ma_last_modified_by,");
            //Motor site fields
            sql.AppendLine("ms.motor_site_id ms_motor_site_id,ms.motor_site_name ms_motor_site_name, ms.last_modified_by ms_last_modified_by,");
            sql.AppendLine(" ms.deleted_record ms_deleted_record, ms.last_modified_date ms_last_modified_date");
            sql.AppendLine("FROM tolive.motor_area ma");
            sql.AppendLine("INNER JOIN tolive.motor_site ms");
            sql.AppendLine("    ON ma.motor_site_id = ms.motor_site_id");
            if (!showDeleted)
                sql.AppendLine("WHERE ma.deleted_record = 0");
            sql.AppendLine("ORDER BY ma.motor_area_desc");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            foreach (DataRow r in ds.Tables[0].Rows)
                retVal.Add(DataRowToObject(r, "ma_"));
            return retVal;
        }



        /// <summary>
        /// Inserts the motor_area into Oracle
        /// </summary>
        /// <param name="ma"></param>
        /// <returns></returns>
        public static int Insert(Motor_Area ma)
        {
            ValidateData(ma);
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                return Insert(ma, conn);
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
        /// Inserts the motor_areas into Oracle
        /// </summary>
        /// <param name="ma"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Insert(Motor_Area ma, OracleConnection conn)
        {
            ValidateData(ma);
            if (ma.Motor_Area_Id <= 0)
                ma.Motor_Area_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.motor_seq"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.motor_area (motor_area_id, motor_area_desc, ");
            sql.AppendLine("motor_site_id, last_modified_by,");
            sql.AppendLine(" deleted_record, last_modified_date)");
            sql.AppendLine("VALUES(:motor_area_id, :motor_area_desc,");
            sql.AppendLine(":motor_site_id, :last_modified_by,");
            sql.AppendLine(" :deleted_record, :last_modified_date)");

            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("motor_area_id", ma.Motor_Area_Id);
            ins.Parameters.Add("motor_area_desc", ma.Motor_Area_Desc);
            ins.Parameters.Add("motor_site_id", ma.Motor_Site.Motor_Site_Id);
            ins.Parameters.Add("last_modified_by", ma.Last_Modified_By);
            ins.Parameters.Add("deleted_record", ma.Deleted_Record ? 1 : 0);
            ins.Parameters.Add("last_modified_date", DateTime.Now);

            return ins.ExecuteNonQuery();

        }


        /// <summary>
        /// Updates the motor_area into Oracle
        /// </summary>
        /// <param name="ma"></param>
        /// <returns></returns>
        public static int Update(Motor_Area ma)
        {
            ValidateData(ma);
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                return Update(ma, conn);
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
        /// Update the motor_areas into Oracle
        /// </summary>
        /// <param name="ma"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Update(Motor_Area ma, OracleConnection conn)
        {
            ValidateData(ma);
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.motor_area ");
            sql.AppendLine("    SET motor_area_desc = :motor_area_desc,");
            sql.AppendLine("    motor_site_id = :motor_site_id,");
            sql.AppendLine("    last_modified_by = :last_modified_by,");
            sql.AppendLine("    deleted_record = :deleted_record,");
            sql.AppendLine("    last_modified_date = :last_modified_date");
            sql.AppendLine("WHERE motor_area_id = :motor_area_id");

            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("motor_area_desc", ma.Motor_Area_Desc);
            cmd.Parameters.Add("motor_site_id", ma.Motor_Site.Motor_Site_Id);
            cmd.Parameters.Add("last_modified_by", ma.Last_Modified_By);
            cmd.Parameters.Add("deleted_record", ma.Deleted_Record ? 1 : 0);
            cmd.Parameters.Add("last_modified_date", DateTime.Now);
            cmd.Parameters.Add("motor_area_id", ma.Motor_Area_Id);

            return cmd.ExecuteNonQuery();

        }

        /// <summary>
        /// Deletes the motor_area in Oracle
        /// </summary>
        /// <param name="ma"></param>
        /// <returns></returns>
        public static int Delete(Motor_Area ma)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                return Delete(ma, conn);
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
        /// Deletes the motor_site in Oracle
        /// </summary>
        /// <param name="ma"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Delete(Motor_Area ma, OracleConnection conn)
        {

            ma.Deleted_Record = true;
            return Update(ma, conn);

        }

        /// <summary>
        /// Validates
        /// </summary>
        private static void ValidateData(Motor_Area ma)
        {
            if (ma.Motor_Site == null)            
                throw new InvalidDataException("Missing data for site id", "Site_Id");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldPrefix">The string prefix for all of the motor location fields in the row</param>
        /// <param name="motorSitePrefix">prefix used for the Motor Site fields in the row</param>
        /// <returns></returns>
        internal static Motor_Area DataRowToObject(DataRow row, string fieldPrefix = "", string motorSitePrefix = "ms_")
        {
            Motor_Area RetVal = new();

            RetVal.Motor_Area_Id = Convert.ToInt32(row[$"{fieldPrefix}motor_area_id"]);
            RetVal.Motor_Area_Desc = row.Field<string>($"{fieldPrefix}motor_area_desc");
            RetVal.Last_Modified_By = row.Field<string>($"{fieldPrefix}last_modified_by");
            RetVal.Deleted_Record = Convert.ToInt32(row[$"{fieldPrefix}deleted_record"]) == 1;
            RetVal.Last_Modified_Date = row.Field<DateTime?>($"{fieldPrefix}last_modified_date");

            RetVal.Motor_Site = Motor_SiteSvc.DataRowToObject(row, motorSitePrefix);

            return RetVal;
        }
    }
}
