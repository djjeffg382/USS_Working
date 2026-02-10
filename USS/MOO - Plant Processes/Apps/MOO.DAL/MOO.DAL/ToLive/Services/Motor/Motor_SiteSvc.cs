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
    public class Motor_SiteSvc
    {
        static Motor_SiteSvc()
        {
            Util.RegisterOracle();

        }

        /// <summary>
        /// Gets the Motor_Site by the specified ID
        /// </summary>
        /// <param name="motorSiteId"></param>
        /// <returns></returns>
        public static Motor_Site Get(int motorSiteId)
        {
            StringBuilder sql = new();

            sql.AppendLine("SELECT motor_site_id ms_motor_site_id, motor_site_name ms_motor_site_name, last_modified_by ms_last_modified_by,");
            sql.AppendLine(" deleted_record ms_deleted_record, last_modified_date ms_last_modified_date");
            sql.AppendLine("FROM tolive.motor_site");
            sql.AppendLine($"WHERE motor_site_id = {motorSiteId}");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0], "ms_");
            else
                return null;

        }

        /// <summary>
        /// Gets all motor_sites from Oracle
        /// </summary>
        /// <returns></returns>
        public static List<Motor_Site> GetAll(bool showDeleted)
        {
            List<Motor_Site> retVal = new();

            StringBuilder sql = new();

            sql.AppendLine("SELECT motor_site_id ms_motor_site_id, motor_site_name ms_motor_site_name, last_modified_by ms_last_modified_by,");
            sql.AppendLine(" deleted_record ms_deleted_record, last_modified_date ms_last_modified_date");
            sql.AppendLine("FROM tolive.motor_site");
            if (!showDeleted)
                sql.AppendLine("WHERE deleted_record = 0");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            foreach (DataRow r in ds.Tables[0].Rows)
                retVal.Add(DataRowToObject(r, "ms_"));
            return retVal;
        }



        /// <summary>
        /// Inserts the motor_site into Oracle
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static int Insert(Motor_Site ms)
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
        /// Inserts the motor_site into Oracle
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Insert(Motor_Site ms, OracleConnection conn)
        {
            if (ms.Motor_Site_Id <= 0)
                ms.Motor_Site_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.motor_seq"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.motor_site (motor_site_id, motor_site_name, last_modified_by,");
            sql.AppendLine(" deleted_record, last_modified_date)");
            sql.AppendLine("VALUES(:motor_site_id,: motor_site_name, :last_modified_by,");
            sql.AppendLine(" :deleted_record, :last_modified_date)");

            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("motor_site_id", ms.Motor_Site_Id);
            ins.Parameters.Add("motor_site_name", ms.Motor_Site_Name);
            ins.Parameters.Add("last_modified_by", ms.Last_Modified_By);
            ins.Parameters.Add("deleted_record", ms.Deleted_Record ? 1 : 0);
            ins.Parameters.Add("last_modified_date", ms.Last_Modified_Date);

            return ins.ExecuteNonQuery();

        }


        /// <summary>
        /// Update the motor_site in Oracle
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static int Update(Motor_Site ms)
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
        /// Update the motor_site in Oracle
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Update(Motor_Site ms, OracleConnection conn)
        {

            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.motor_site");
            sql.AppendLine("SET motor_site_name = :motor_site_name, last_modified_by = :last_modified_by,");
            sql.AppendLine("deleted_record = :deleted_record, last_modified_date = :last_modified_date");
            sql.AppendLine("WHERE motor_site_id = :motor_site_id");

            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("motor_site_name", ms.Motor_Site_Name);
            cmd.Parameters.Add("last_modified_by", ms.Last_Modified_By);
            cmd.Parameters.Add("deleted_record", ms.Deleted_Record ? 1 : 0);
            cmd.Parameters.Add("last_modified_date", ms.Last_Modified_Date);
            cmd.Parameters.Add("motor_site_id", ms.Motor_Site_Id);

            return cmd.ExecuteNonQuery();

        }

        /// <summary>
        /// Deletes the motor_site in Oracle
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static int Delete(Motor_Site ms)
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
        /// Deletes the motor_site in Oracle
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Delete(Motor_Site ms, OracleConnection conn)
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
        internal static Motor_Site DataRowToObject(DataRow row, string fieldPrefix = "")
        {
            Motor_Site RetVal = new();

            RetVal.Motor_Site_Id = Convert.ToInt32(row[$"{fieldPrefix}motor_site_id"]);
            RetVal.Motor_Site_Name = row.Field<string>($"{fieldPrefix}motor_site_name");
            RetVal.Last_Modified_By = row.Field<string>($"{fieldPrefix}last_modified_by");
            RetVal.Deleted_Record = Convert.ToInt32(row[$"{fieldPrefix}deleted_record"]) == 1;
            RetVal.Last_Modified_Date = row.Field<DateTime?>($"{fieldPrefix}last_modified_date");


            return RetVal;
        }

    }
}
