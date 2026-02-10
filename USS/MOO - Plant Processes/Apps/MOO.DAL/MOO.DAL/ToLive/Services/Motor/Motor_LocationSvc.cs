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
    public class Motor_LocationSvc
    {
        static Motor_LocationSvc()
        {
            Util.RegisterOracle();
        }


        /// <summary>
        /// Gets the Motor_Location by the specified ID
        /// </summary>
        /// <param name="motorLocationId"></param>
        /// <returns></returns>
        public static Motor_Location Get(int motorLocationId)
        {
            StringBuilder sql = new();

            sql.Append(GetSelect());
            sql.AppendLine($"WHERE ml.motor_location_id = {motorLocationId}");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0], "loc_");
            else
                return null;

        }

        public static List<Motor_Location> GetAll(bool showDeleted = false)
        {
            List<Motor_Location> retObj = new();
            StringBuilder sql = new();
            sql.Append(GetSelect());
            if (!showDeleted)
                sql.AppendLine("WHERE ml.deleted_record = 0");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                retObj.Add(DataRowToObject(row, "loc_"));
            }
            return retObj;

        }

        /// <summary>
        /// Gets a paged list of data for use in a paged table
        /// </summary>
        /// <param name="motorAreaId"></param>
        /// <param name="motorLocDesc"></param>
        /// <param name="offset"></param>
        /// <param name="totalItems"></param>
        /// <param name="orderBy"></param>
        /// <param name="orderDirection"></param>
        /// <returns></returns>
        public static PagedData<List<Motor_Location>> GetPagedData(bool showDeleted, int? motorAreaId, string motorLocDesc,
                        int offset, int totalItems, string orderBy = "motor_location_desc", string orderDirection = "ASC")
        {
            PagedData<List<Motor_Location>> retObj = new();
            StringBuilder sql = new();
            OracleDataAdapter da = new("", MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            string filter = AddFilters(showDeleted, motorAreaId, motorLocDesc, da.SelectCommand);
            string ord;
            if (!string.IsNullOrEmpty(orderBy))
            {
                ord = orderBy;
                if (!string.IsNullOrEmpty(orderDirection))
                    ord = $"{ord} {orderDirection}";
            }
            else
            {
                ord = "ORDER BY motor_location_desc";
            }

            sql.AppendLine("SELECT * FROM (");
            sql.AppendLine(GetSelect($"ROW_NUMBER() OVER(ORDER BY {ord}) rn"));
            sql.AppendLine(filter);
            sql.AppendLine(") tbl");
            sql.AppendLine($"WHERE rn BETWEEN {offset} AND {offset + totalItems}");
            da.SelectCommand.CommandText = sql.ToString();
            DataSet ds = MOO.Data.ExecuteQuery(da);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                retObj.Data.Add(DataRowToObject(row, "loc_"));
            }
            retObj.TotalRows = GetMotorLocationCount(showDeleted, motorAreaId, motorLocDesc);

            return retObj;

        }


        private static int GetMotorLocationCount(bool showDeleted, int? motorAreaId, string motorLocDesc)
        {
            StringBuilder sql = new();
            OracleDataAdapter da = new("", MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));

            string filter = AddFilters(showDeleted, motorAreaId, motorLocDesc, da.SelectCommand);

            sql.AppendLine("SELECT COUNT(*) FROM tolive.motor_location ml");
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



        private static string AddFilters(bool showDeleted, int? motorAreaId, string motorLocDesc, OracleCommand cmd)
        {
            StringBuilder filter = new();

            if (!showDeleted)
            {
                filter.AppendLine("WHERE ml.deleted_record = 0");
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
        /// Gets the fields needed for thie motor location table
        /// </summary>
        /// <param name="TableAlias">The table alias in the query</param>
        /// <param name="AliasPrefix">how to prefix the fields aliases</param>
        /// <returns></returns>
        public static string GetFields(string TableAlias, string AliasPrefix)
        {
            StringBuilder retVal = new();
            retVal.AppendLine($"{TableAlias}.motor_location_id {AliasPrefix}motor_location_id, {TableAlias}.motor_location_desc {AliasPrefix}motor_location_desc, ");
            retVal.AppendLine($"{TableAlias}.last_modified_by {AliasPrefix}last_modified_by, {TableAlias}.deleted_record {AliasPrefix}deleted_record,");
            retVal.AppendLine($"{TableAlias}.last_modified_date {AliasPrefix}last_modified_date, {TableAlias}.is_asset_location {AliasPrefix}is_asset_location,");
            return retVal.ToString();
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
            sql.AppendLine(GetFields("ml", "loc_"));
            //Motor Area fields
            sql.AppendLine("    ma.motor_area_id ma_motor_area_id, ma.motor_area_desc ma_motor_area_desc, ");
            sql.AppendLine(" ma.deleted_record ma_deleted_record, ma.last_modified_date ma_last_modified_date,");
            sql.AppendLine("ma.last_modified_by ma_last_modified_by,");
            //motor site fields
            sql.AppendLine("ms.motor_site_id ms_motor_site_id,ms.motor_site_name ms_motor_site_name, ms.last_modified_by ms_last_modified_by,");
            sql.AppendLine(" ms.deleted_record ms_deleted_record, ms.last_modified_date ms_last_modified_date");
            if (!string.IsNullOrEmpty(addField))
            {
                sql.AppendLine($",{addField}");
            }

            sql.AppendLine("FROM tolive.motor_location ml");
            sql.AppendLine("INNER JOIN tolive.motor_area ma");
            sql.AppendLine("    ON ml.motor_area_id = ma.motor_area_id");
            sql.AppendLine("INNER JOIN tolive.motor_site ms");
            sql.AppendLine("    ON ma.motor_site_id = ms.motor_site_id");
            return sql.ToString();
        }




        /// <summary>
        /// Inserts the Motor_Location into Oracle
        /// </summary>
        /// <param name="ml"></param>
        /// <returns></returns>
        public static int Insert(Motor_Location ml)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            return Insert(ml, conn);

        }

        /// <summary>
        /// Inserts the Motor_Location into Oracle
        /// </summary>
        /// <param name="ml"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Insert(Motor_Location ml, OracleConnection conn)
        {
            if (ml.Motor_Location_Id <= 0)
                ml.Motor_Location_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.motor_seq"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.motor_location (motor_location_id, motor_location_desc, ");
            sql.AppendLine("motor_area_id, last_modified_by, is_asset_location,");
            sql.AppendLine(" deleted_record, last_modified_date)");
            sql.AppendLine("VALUES(:motor_location_id, :motor_location_desc,");
            sql.AppendLine(":motor_area_id, :last_modified_by,");
            sql.AppendLine(":is_asset_location,");
            sql.AppendLine(" :deleted_record, :last_modified_date)");

            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("motor_location_id", ml.Motor_Location_Id);
            ins.Parameters.Add("motor_location_desc", ml.Motor_Location_Desc);
            ins.Parameters.Add("motor_area_id", ml.Motor_Area.Motor_Area_Id);
            ins.Parameters.Add("last_modified_by", ml.Last_Modified_By);
            ins.Parameters.Add("is_asset_location", ml.Is_Asset_Location ? 1 : 0);
            ins.Parameters.Add("deleted_record", ml.Deleted_Record ? 1 : 0);
            ins.Parameters.Add("last_modified_date", DateTime.Now);

            return ins.ExecuteNonQuery();

        }


        /// <summary>
        /// Updates the Motor_Location into Oracle
        /// </summary>
        /// <param name="ml"></param>
        /// <returns></returns>
        public static int Update(Motor_Location ml)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            return Update(ml, conn);

        }

        /// <summary>
        /// Update the Motor_Location into Oracle
        /// </summary>
        /// <param name="ml"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Update(Motor_Location ml, OracleConnection conn)
        {


            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.motor_location ");
            sql.AppendLine("    SET motor_location_desc = :motor_location_desc,");
            sql.AppendLine("    motor_area_id = :motor_area_id,");
            sql.AppendLine("    last_modified_by = :last_modified_by,");
            sql.AppendLine("    deleted_record = :deleted_record,");
            sql.AppendLine("    last_modified_date = :last_modified_date,");
            sql.AppendLine("    is_asset_location = :is_asset_location");
            sql.AppendLine("WHERE motor_location_id = :motor_location_id");

            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("motor_location_desc", ml.Motor_Location_Desc);
            cmd.Parameters.Add("motor_area_id", ml.Motor_Area.Motor_Area_Id);
            cmd.Parameters.Add("last_modified_by", ml.Last_Modified_By);
            cmd.Parameters.Add("deleted_record", ml.Deleted_Record ? 1 : 0);
            cmd.Parameters.Add("last_modified_date", DateTime.Now);
            cmd.Parameters.Add("is_asset_location", ml.Is_Asset_Location ? 1 : 0);
            cmd.Parameters.Add("motor_location_id", ml.Motor_Location_Id);

            return cmd.ExecuteNonQuery();

        }


        /// <summary>
        /// Deletes the Motor_Location in Oracle
        /// </summary>
        /// <param name="ml"></param>
        /// <returns></returns>
        public static int Delete(Motor_Location ml)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                return Delete(ml, conn);
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
        /// Deletes the Motor_Location in Oracle
        /// </summary>
        /// <param name="ma"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Delete(Motor_Location ml, OracleConnection conn)
        {

            ml.Deleted_Record = true;
            return Update(ml);

        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fieldPrefix">The string prefix for all of the motor location fields in the row</param>
        /// <param name="motorSitePrefix">prefix used for the Motor Site fields in the row</param>
        /// <param name="motorAreaPrefix">prefix used for the Motor area fields in the row</param>
        /// <returns></returns>
        internal static Motor_Location DataRowToObject(DataRow row, string fieldPrefix = "", string motorAreaPrefix = "ma_", string motorSitePrefix = "ms_")
        {

            Motor_Location RetVal = new();

            RetVal.Motor_Location_Id = Convert.ToInt32(row[$"{fieldPrefix}motor_location_id"]);
            RetVal.Motor_Location_Desc = row.Field<string>($"{fieldPrefix}motor_location_desc");
            RetVal.Last_Modified_By = row.Field<string>($"{fieldPrefix}last_modified_by");
            RetVal.Deleted_Record = Convert.ToInt32(row[$"{fieldPrefix}deleted_record"]) == 1;
            RetVal.Last_Modified_Date = row.Field<DateTime?>($"{fieldPrefix}last_modified_date");
            RetVal.Is_Asset_Location = Convert.ToInt32(row[$"{fieldPrefix}is_asset_location"]) == 1;

            RetVal.Motor_Area = Motor_AreaSvc.DataRowToObject(row, motorAreaPrefix, motorSitePrefix);

            return RetVal;
        }
    }
}
