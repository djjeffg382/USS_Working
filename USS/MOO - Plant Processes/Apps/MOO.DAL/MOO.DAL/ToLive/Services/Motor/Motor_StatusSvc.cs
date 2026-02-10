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
    public class Motor_StatusSvc
    {
        static Motor_StatusSvc()
        {
            Util.RegisterOracle();
        }



        /// <summary>
        /// Gets the Manufacturer by the specified ID
        /// </summary>
        /// <param name="motorStatusID"></param>
        /// <returns></returns>
        public static Motor_Status Get(int motorStatusID)
        {
            StringBuilder sql = new();

            sql.AppendLine("SELECT motor_status_id, motor_status_name,");
            sql.AppendLine("    last_modified_by, ");
            sql.AppendLine("    deleted_record,");
            sql.AppendLine("    last_modified_date");
            sql.AppendLine("FROM tolive.motor_status");
            sql.AppendLine($"WHERE motor_status_id = {motorStatusID}");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;

        }


        /// <summary>
        /// Gets all motor_statuses from Oracle
        /// </summary>
        /// <returns></returns>
        public static List<Motor_Status> GetAll(bool showDeleted)
        {
            List<Motor_Status> retVal = new();

            StringBuilder sql = new();

            sql.AppendLine("SELECT motor_status_id, motor_status_name,");
            sql.AppendLine("    last_modified_by, ");
            sql.AppendLine("    deleted_record,");
            sql.AppendLine("    last_modified_date");
            sql.AppendLine("FROM tolive.motor_status");
            if (!showDeleted)
                sql.AppendLine("WHERE deleted_record = 0");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            foreach (DataRow r in ds.Tables[0].Rows)
                retVal.Add(DataRowToObject(r));
            return retVal;
        }


        /// <summary>
        /// Inserts the motor_status into Oracle
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static int Insert(Motor_Status ms)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                return Insert(ms, conn);
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
        /// Inserts the Motor_Status into Oracle
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Insert(Motor_Status ms, OracleConnection conn)
        {
            if (ms.Motor_Status_Id <= 0)
                ms.Motor_Status_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.motor_seq"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.motor_status (motor_status_id, motor_status_name, ");
            sql.AppendLine("last_modified_by,");
            sql.AppendLine(" deleted_record, last_modified_date)");
            sql.AppendLine("VALUES(:motor_status_id, :motor_status_name,");
            sql.AppendLine(":last_modified_by,");
            sql.AppendLine(" :deleted_record, :last_modified_date)");

            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("motor_status_id", ms.Motor_Status_Id);
            ins.Parameters.Add("motor_status_name", ms.Motor_Status_Name);
            ins.Parameters.Add("last_modified_by", ms.Last_Modified_By);
            ins.Parameters.Add("deleted_record", ms.Deleted_Record ? 1 : 0);
            ins.Parameters.Add("last_modified_date", ms.Last_Modified_Date);

            return ins.ExecuteNonQuery();

        }



        /// <summary>
        /// Updates the Motor_Status into Oracle
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static int Update(Motor_Status ms)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                return Update(ms, conn);
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
        /// Update the Motor_Status into Oracle
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Update(Motor_Status ms, OracleConnection conn)
        {


            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.motor_status ");
            sql.AppendLine("    SET motor_status_name = :motor_status_name,");
            sql.AppendLine("    last_modified_by = :last_modified_by,");
            sql.AppendLine("    deleted_record = :deleted_record,");
            sql.AppendLine("    last_modified_date = :last_modified_date");
            sql.AppendLine("WHERE motor_status_id = :motor_status_id");

            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("motor_status_name", ms.Motor_Status_Name);
            cmd.Parameters.Add("last_modified_by", ms.Last_Modified_By);
            cmd.Parameters.Add("deleted_record", ms.Deleted_Record ? 1 : 0);
            cmd.Parameters.Add("last_modified_date", DateTime.Now);
            cmd.Parameters.Add("motor_status_id", ms.Motor_Status_Id);

            return cmd.ExecuteNonQuery();

        }


        /// <summary>
        /// Deletes the Motor_Status in Oracle
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static int Delete(Motor_Status ms)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                return Delete(ms, conn);
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
        /// Deletes the Motor_Status in Oracle
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Delete(Motor_Status ms, OracleConnection conn)
        {

            ms.Deleted_Record = true;
            return Update(ms);

        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldPrefix">The string prefix for all of the motor location fields in the row</param>
        /// <returns></returns>
        internal static Motor_Status DataRowToObject(DataRow row, string fieldPrefix = "")
        {
            Motor_Status RetVal = new();

            RetVal.Motor_Status_Id = Convert.ToInt32(row[$"{fieldPrefix}motor_status_id"]);
            RetVal.Motor_Status_Name = row.Field<string>($"{fieldPrefix}motor_status_name");
            RetVal.Last_Modified_By = row.Field<string>($"{fieldPrefix}last_modified_by");
            RetVal.Deleted_Record = Convert.ToInt32(row[$"{fieldPrefix}deleted_record"]) == 1;
            RetVal.Last_Modified_Date = row.Field<DateTime?>($"{fieldPrefix}last_modified_date");

            return RetVal;
        }
    }
}
