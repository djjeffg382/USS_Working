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
    public class Motor_ManufacturerSvc
    {
        static Motor_ManufacturerSvc()
        {
            Util.RegisterOracle();
        }



        /// <summary>
        /// Gets the Manufacturer by the specified ID
        /// </summary>
        /// <param name="motorManufacId"></param>
        /// <returns></returns>
        public static Motor_Manufacturer Get(int motorManufacId)
        {
            StringBuilder sql = new();

            sql.AppendLine("SELECT motor_manufacturer_id man_motor_manufacturer_id, motor_manufacturer_desc man_motor_manufacturer_desc,");
            sql.AppendLine("    last_modified_by man_last_modified_by, ");
            sql.AppendLine("    deleted_record man_deleted_record, ");
            sql.AppendLine("    last_modified_date man_last_modified_date");
            sql.AppendLine("FROM tolive.motor_manufacturer");
            sql.AppendLine($"WHERE motor_manufacturer_id = {motorManufacId}");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0], "man_");
            else
                return null;

        }


        /// <summary>
        /// Gets all motor_manufacturers from Oracle
        /// </summary>
        /// <returns></returns>
        public static List<Motor_Manufacturer> GetAll(bool showDeleted)
        {
            List<Motor_Manufacturer> retVal = new();

            StringBuilder sql = new();

            sql.AppendLine("SELECT motor_manufacturer_id man_motor_manufacturer_id, motor_manufacturer_desc man_motor_manufacturer_desc,");
            sql.AppendLine("    last_modified_by man_last_modified_by, ");
            sql.AppendLine("    deleted_record man_deleted_record, ");
            sql.AppendLine("    last_modified_date man_last_modified_date");
            sql.AppendLine("FROM tolive.motor_manufacturer");
            if (!showDeleted)
                sql.AppendLine("WHERE deleted_record = 0");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            foreach (DataRow r in ds.Tables[0].Rows)
                retVal.Add(DataRowToObject(r,"man_"));
            return retVal;
        }





        /// <summary>
        /// Inserts the motor_manufacturer into Oracle
        /// </summary>
        /// <param name="mm"></param>
        /// <returns></returns>
        public static int Insert(Motor_Manufacturer mm)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                return Insert(mm, conn);
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
        /// Inserts the Motor_Manufacturer into Oracle
        /// </summary>
        /// <param name="mm"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Insert(Motor_Manufacturer mm, OracleConnection conn)
        {
            if (mm.Motor_Manufacturer_Id <= 0)
                mm.Motor_Manufacturer_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.motor_seq"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.motor_manufacturer (motor_manufacturer_id, motor_manufacturer_desc, ");
            sql.AppendLine("last_modified_by,");
            sql.AppendLine(" deleted_record, last_modified_date)");
            sql.AppendLine("VALUES(:motor_manufacturer_id, :motor_manufacturer_desc,");
            sql.AppendLine(":last_modified_by,");
            sql.AppendLine(" :deleted_record, :last_modified_date)");

            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("motor_manufacturer_id", mm.Motor_Manufacturer_Id);
            ins.Parameters.Add("motor_manufacturer_desc", mm.Motor_Manufacturer_Desc);
            ins.Parameters.Add("last_modified_by", mm.Last_Modified_By);
            ins.Parameters.Add("deleted_record", mm.Deleted_Record ? 1 : 0);
            ins.Parameters.Add("last_modified_date", DateTime.Now);

            return ins.ExecuteNonQuery();

        }



        /// <summary>
        /// Updates the motor_manufacturer into Oracle
        /// </summary>
        /// <param name="mm"></param>
        /// <returns></returns>
        public static int Update(Motor_Manufacturer mm)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                return Update(mm, conn);
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
        /// Update the motor_manufacturer into Oracle
        /// </summary>
        /// <param name="mm"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Update(Motor_Manufacturer mm, OracleConnection conn)
        {


            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.motor_manufacturer ");
            sql.AppendLine("    SET motor_manufacturer_desc = :motor_manufacturer_desc,");
            sql.AppendLine("    last_modified_by = :last_modified_by,");
            sql.AppendLine("    deleted_record = :deleted_record,");
            sql.AppendLine("    last_modified_date = :last_modified_date");
            sql.AppendLine("WHERE motor_manufacturer_id = :motor_manufacturer_id");

            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("motor_manufacturer_desc", mm.Motor_Manufacturer_Desc);
            cmd.Parameters.Add("last_modified_by", mm.Last_Modified_By);
            cmd.Parameters.Add("deleted_record", mm.Deleted_Record ? 1 : 0);
            cmd.Parameters.Add("last_modified_date", DateTime.Now);
            cmd.Parameters.Add("motor_manufacturer_id", mm.Motor_Manufacturer_Id);

            return cmd.ExecuteNonQuery();

        }



        /// <summary>
        /// Deletes the motor_manufacturer in Oracle
        /// </summary>
        /// <param name="mm"></param>
        /// <returns></returns>
        public static int Delete(Motor_Manufacturer mm)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                return Delete(mm, conn);
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
        /// Deletes the motor_manufacturer in Oracle
        /// </summary>
        /// <param name="mm"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Delete(Motor_Manufacturer mm, OracleConnection conn)
        {

            mm.Deleted_Record = true;
            return Update(mm, conn);

        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldPrefix">The string prefix for all of the motor location fields in the row</param>
        /// <returns></returns>
        internal static Motor_Manufacturer DataRowToObject(DataRow row, string fieldPrefix = "")
        {
            Motor_Manufacturer RetVal = new();

            RetVal.Motor_Manufacturer_Id = Convert.ToInt32(row[$"{fieldPrefix}motor_manufacturer_id"]);
            RetVal.Motor_Manufacturer_Desc = row.Field<string>($"{fieldPrefix}motor_manufacturer_desc");
            RetVal.Last_Modified_By = row.Field<string>($"{fieldPrefix}last_modified_by");
            RetVal.Deleted_Record = Convert.ToInt32(row[$"{fieldPrefix}deleted_record"]) == 1;
            RetVal.Last_Modified_Date = row.Field<DateTime?>($"{fieldPrefix}last_modified_date");

            return RetVal;
        }
    }
}
