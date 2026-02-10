using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;

namespace MOO.DAL.ToLive.Services
{
    public static class PeopleSvc
    {
        public const string TABLE_NAME = "TOLIVE.People";
        static PeopleSvc()
        {
            Util.RegisterOracle();
        }

        /// <summary>
        /// search for user only by person Id
        /// </summary>
        /// <param name="Person_Id"></param>
        /// <returns></returns>
        public static People Get(int Person_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE person_id = :Person_Id ");

            People retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("Person_Id", Person_Id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }

        /// <summary>
        /// Get All users or search using any provided parameters.
        /// </summary>
        /// <param name="fName">Optional: First Name</param>
        /// <param name="lName">Optional: Last Name</param>
        /// <param name="personID">Primary Key</param>
        /// <param name="empId">Optional: Employee Number, is also a string employee numbers can have zeros leading (000543)</param>
        /// <param name="supervisorId">Optional: Supervisor ID</param>
        /// <param name="getOnlyActive">If true, only active people will be returned</param>
        /// <returns></returns>
        public static List<People> GetAll(string fName = "", string lName = "", int personID = 0, int empId = 0, int supervisorId = 0, bool getOnlyActive = true)
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine("person_id, last_name, first_name, middle_name, employee_number, work_location_code, ");
            sql.AppendLine("supervisor_person_id, low_level_group, base_level_group, cost_center, status_ind, ");
            sql.AppendLine("windows_ad_account, home_number, mobile_number, office_ext ");
            sql.AppendLine("FROM tolive.people");
            if(getOnlyActive)
                sql.AppendLine("WHERE (status_ind = 'A' or windows_ad_account is not null) ");

            if (!string.IsNullOrEmpty(fName))
                sql.AppendLine($"AND lower(first_name) LIKE '%{fName}%' ");

            if (!string.IsNullOrEmpty(lName))
                sql.AppendLine($"AND lower(last_name) LIKE '%{lName}%' ");

            if (personID > 0)
                sql.AppendLine($"AND PERSON_ID = {personID} ");

            if (empId > 0)
                sql.AppendLine($"AND LPAD(employee_number, 6, '0') = :empId ");

            if (supervisorId > 0)
                sql.AppendLine($"AND SUPERVISOR_PERSON_ID = {supervisorId} ");

            sql.AppendLine("ORDER BY last_name ");
            List<People> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);

            if (empId > 0)
                cmd.Parameters.Add("empId", empId.ToString().PadLeft(6, '0'));

            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    elements.Add(DataRowToObject(rdr));
                }
            }
            conn.Close();
            return elements;
        }

        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}person_id {ColPrefix}person_id, {ta}last_name {ColPrefix}last_name, {ta}first_name {ColPrefix}first_name, {ta}middle_name {ColPrefix}middle_name, ");
            cols.AppendLine($"{ta}employee_number {ColPrefix}employee_number, {ta}work_location_code {ColPrefix}work_location_code, ");
            cols.AppendLine($"{ta}supervisor_person_id {ColPrefix}supervisor_person_id, {ta}low_level_group {ColPrefix}low_level_group, ");
            cols.AppendLine($"{ta}base_level_group {ColPrefix}base_level_group, {ta}cost_center {ColPrefix}cost_center, {ta}status_ind {ColPrefix}status_ind, ");
            cols.AppendLine($"{ta}windows_ad_account {ColPrefix}windows_ad_account, {ta}home_number {ColPrefix}home_number,");
            cols.AppendLine($"{ta}mobile_number {ColPrefix}mobile_number, {ta}office_ext {ColPrefix}office_ext ");
            return cols.ToString();
        }

        private static string GetSelect(string addField = "")
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            if (!string.IsNullOrEmpty(addField))
            {
                sql.Append(", ");
                sql.AppendLine(addField);
            }
            sql.AppendLine("FROM tolive.people");
            return sql.ToString();
        }


        public static int Insert(People obj)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                int recsAffected = Insert(obj, conn);
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


        public static int Insert(People obj, OracleConnection conn)
        {

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.people(");
            sql.AppendLine("mobile_number, office_ext, person_id, last_name, first_name, middle_name, ");
            sql.AppendLine("employee_number, work_location_code, supervisor_person_id, low_level_group, ");
            sql.AppendLine("base_level_group, cost_center, status_ind, windows_ad_account, home_number)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":mobile_number, :office_ext, :person_id, :last_name, :first_name, :middle_name, ");
            sql.AppendLine(":employee_number, :work_location_code, :supervisor_person_id, :low_level_group, ");
            sql.AppendLine(":base_level_group, :cost_center, :status_ind, :windows_ad_account, :home_number)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("mobile_number", obj.Mobile_Number);
            ins.Parameters.Add("office_ext", obj.Office_Ext);
            ins.Parameters.Add("person_id", obj.Person_Id);
            ins.Parameters.Add("last_name", obj.Last_Name);
            ins.Parameters.Add("first_name", obj.First_Name);
            ins.Parameters.Add("middle_name", obj.Middle_Name);
            ins.Parameters.Add("employee_number", obj.Employee_Number);
            ins.Parameters.Add("work_location_code", obj.Work_Location_Code);
            ins.Parameters.Add("supervisor_person_id", obj.Supervisor_Person_Id);
            ins.Parameters.Add("low_level_group", obj.Low_Level_Group);
            ins.Parameters.Add("base_level_group", obj.Base_Level_Group);
            ins.Parameters.Add("cost_center", obj.Cost_Center);
            ins.Parameters.Add("status_ind", obj.Status_Ind);
            ins.Parameters.Add("windows_ad_account", obj.Windows_Ad_Account);
            ins.Parameters.Add("home_number", obj.Home_Number);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(People obj)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                int recsAffected = Update(obj, conn);
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


        public static int Update(People obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.people SET");
            sql.AppendLine("person_id = :person_id, ");
            sql.AppendLine("last_name = :last_name, ");
            sql.AppendLine("first_name = :first_name, ");
            sql.AppendLine("middle_name = :middle_name, ");
            sql.AppendLine("employee_number = :employee_number, ");
            sql.AppendLine("work_location_code = :work_location_code, ");
            sql.AppendLine("supervisor_person_id = :supervisor_person_id, ");
            sql.AppendLine("low_level_group = :low_level_group, ");
            sql.AppendLine("base_level_group = :base_level_group, ");
            sql.AppendLine("cost_center = :cost_center, ");
            sql.AppendLine("status_ind = :status_ind, ");
            sql.AppendLine("windows_ad_account = :windows_ad_account, ");
            sql.AppendLine("home_number = :home_number,");
            sql.AppendLine("mobile_number = :mobile_number,");
            sql.AppendLine("office_ext = :office_ext ");
            sql.AppendLine("WHERE person_id = :person_id"); OracleCommand upd = new(sql.ToString(), conn);
            upd.Parameters.Add("person_id", obj.Person_Id);
            upd.Parameters.Add("last_name", obj.Last_Name);
            upd.Parameters.Add("first_name", obj.First_Name);
            upd.Parameters.Add("middle_name", obj.Middle_Name);
            upd.Parameters.Add("employee_number", obj.Employee_Number);
            upd.Parameters.Add("work_location_code", obj.Work_Location_Code);
            upd.Parameters.Add("supervisor_person_id", obj.Supervisor_Person_Id);
            upd.Parameters.Add("low_level_group", obj.Low_Level_Group);
            upd.Parameters.Add("base_level_group", obj.Base_Level_Group);
            upd.Parameters.Add("cost_center", obj.Cost_Center);
            upd.Parameters.Add("status_ind", obj.Status_Ind);
            upd.Parameters.Add("windows_ad_account", obj.Windows_Ad_Account);
            upd.Parameters.Add("home_number", obj.Home_Number);
            upd.Parameters.Add("mobile_number", obj.Mobile_Number);
            upd.Parameters.Add("office_ext", obj.Office_Ext);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(People obj)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                int recsAffected = Delete(obj, conn);
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


        public static int Delete(People obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.people");
            sql.AppendLine("WHERE person_id = :person_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.Parameters.Add("person_id", obj.Person_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static People DataRowToObject(DataRow row, string ColPrefix = "")
        {
            People RetVal = new();
            RetVal.Person_Id = Convert.ToInt32(row.Field<decimal>($"{ColPrefix}person_id"));
            RetVal.Last_Name = row.Field<string>($"{ColPrefix}last_name");
            RetVal.First_Name = row.Field<string>($"{ColPrefix}first_name");
            RetVal.Middle_Name = row.Field<string>($"{ColPrefix}middle_name");
            RetVal.Employee_Number = row.Field<string>($"{ColPrefix}employee_number");
            RetVal.Work_Location_Code = Convert.ToInt32(row.Field<decimal?>($"{ColPrefix}work_location_code"));
            RetVal.Supervisor_Person_Id = Convert.ToInt32(row.Field<decimal?>($"{ColPrefix}supervisor_person_id"));
            RetVal.Low_Level_Group = row.Field<string>($"{ColPrefix}low_level_group");
            RetVal.Base_Level_Group = row.Field<string>($"{ColPrefix}base_level_group");
            RetVal.Cost_Center = row.Field<string>($"{ColPrefix}cost_center");
            RetVal.Status_Ind = row.Field<string>($"{ColPrefix}status_ind");
            RetVal.Windows_Ad_Account = row.Field<string>($"{ColPrefix}windows_ad_account");
            RetVal.Home_Number = row.Field<string>($"{ColPrefix}home_number");
            RetVal.Mobile_Number = row.Field<string>($"{ColPrefix}mobile_number");
            RetVal.Office_Ext = row.Field<string>($"{ColPrefix}office_ext");

            return RetVal;
        }

        internal static People DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            People RetVal = new();
            RetVal.Person_Id = Convert.ToInt32((decimal)Util.GetRowVal(row, $"{ColPrefix}person_id"));
            RetVal.Last_Name = (string)Util.GetRowVal(row, $"{ColPrefix}last_name");
            RetVal.First_Name = (string)Util.GetRowVal(row, $"{ColPrefix}first_name");
            RetVal.Middle_Name = (string)Util.GetRowVal(row, $"{ColPrefix}middle_name");
            RetVal.Employee_Number = (string)Util.GetRowVal(row, $"{ColPrefix}employee_number");

            if (row.IsDBNull(row.GetOrdinal($"{ColPrefix}work_location_code")))
                RetVal.Work_Location_Code = null;
            else
                RetVal.Work_Location_Code = Convert.ToInt32((decimal?)Util.GetRowVal(row, $"{ColPrefix}work_location_code"));

            if (row.IsDBNull(row.GetOrdinal($"{ColPrefix}supervisor_person_id")))
                RetVal.Supervisor_Person_Id = null;
            else
                RetVal.Supervisor_Person_Id = Convert.ToInt32((decimal?)Util.GetRowVal(row, $"{ColPrefix}supervisor_person_id"));

            RetVal.Low_Level_Group = (string)Util.GetRowVal(row, $"{ColPrefix}low_level_group");
            RetVal.Base_Level_Group = (string)Util.GetRowVal(row, $"{ColPrefix}base_level_group");
            RetVal.Cost_Center = (string)Util.GetRowVal(row, $"{ColPrefix}cost_center");
            RetVal.Status_Ind = (string)Util.GetRowVal(row, $"{ColPrefix}status_ind");
            RetVal.Windows_Ad_Account = (string)Util.GetRowVal(row, $"{ColPrefix}windows_ad_account");
            RetVal.Home_Number = (string)Util.GetRowVal(row, $"{ColPrefix}home_number");
            RetVal.Mobile_Number = (string)Util.GetRowVal(row, $"{ColPrefix}mobile_number");
            RetVal.Office_Ext = (string)Util.GetRowVal(row, $"{ColPrefix}office_ext");
            return RetVal;
        }

    }
}
