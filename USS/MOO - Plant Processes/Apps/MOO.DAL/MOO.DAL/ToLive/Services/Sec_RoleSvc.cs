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
    /// <summary>
    /// Class used for obatining sec_role information
    /// </summary>
    public static class Sec_RoleSvc
    {

        static Sec_RoleSvc()
        {
            Util.RegisterOracle();
        }

        /// <summary>
        /// Gets the role object given the role_id
        /// </summary>
        /// <param name="role_Id"></param>
        /// <returns></returns>
        public static Sec_Role Get(int role_Id)
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT *");
            sql.AppendLine("FROM tolive.sec_role");
            sql.AppendLine($"WHERE role_id = {role_Id}");

            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                return DataRowToObject(rdr);
            }

            return null;

        }
        /// <summary>
        /// Gets the role given the role name
        /// </summary>
        /// <param name="role_Name"></param>
        /// <returns></returns>
        public static Sec_Role Get(string role_Name)
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT *");
            sql.AppendLine("FROM tolive.sec_role");
            sql.AppendLine($"WHERE LOWER(role_name) = '{role_Name.Replace("'", "''").ToLower()}'");



            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                return DataRowToObject(rdr);
            }

            return null;

        }

        public static List<Sec_Role> GetAll()
        {
            return GetAll(true);

        }


        /// <summary>
        /// Returns the list of all sec_roles in the database
        /// </summary>
        /// <param name="showInactive">If true, will show inactive roles in the list</param>
        /// <returns></returns>
        public static List<Sec_Role> GetAll(bool ShowInactive)
        {
            List<Sec_Role> RetVal = [];
            StringBuilder sql = new();
            sql.AppendLine("SELECT sr.* ");
            sql.AppendLine("FROM tolive.sec_role sr");
            if (!ShowInactive)
                sql.AppendLine("WHERE sr.active = 1");
            sql.AppendLine("ORDER BY role_name");

            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    RetVal.Add(DataRowToObject(rdr));
                }
            }
            conn.Close();
            return RetVal;
        }

        [Obsolete("Use GetAll(showInactive) instead")]
        /// <summary>
        /// Returns the list of all sec_roles in the database
        /// </summary>
        /// <param name="showInactive">If true, will show inactive roles in the list</param>
        /// <returns></returns>
        public static List<Sec_Role> GetAllRoles(bool showInactive)
        {
            return GetAll(showInactive);
        }




        /// <summary>
        /// Gets list of roles for a given user object
        /// </summary>
        /// <param name="usr"></param>
        /// <returns></returns>
        public static List<Sec_Role> GetRolesByUser(Sec_Users usr)
        {
            List<Sec_Role> RetVal = [];
            StringBuilder sql = new();
            sql.AppendLine("SELECT sr.* ");
            sql.AppendLine("FROM tolive.sec_role sr");
            sql.AppendLine("INNER JOIN sec_access sa");
            sql.AppendLine("    ON sr.role_id = sa.role_id");
            sql.AppendLine($"WHERE user_id = {usr.User_Id}");
            sql.AppendLine("AND sr.active = 1");

            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    RetVal.Add(DataRowToObject(rdr));
                }
            }
            conn.Close();
            return RetVal;
        }

        /// <summary>
        /// inserts a new role into the database (role_id will be generated if zero or negative)
        /// </summary>
        /// <param name="role"></param>
        /// <param name="conn"></param>
        public static void Insert(Sec_Role role)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                Insert(role, conn);
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
        /// inserts a new role into the database (role_id will be generated if zero or negative)
        /// </summary>
        /// <param name="role"></param>
        /// <param name="conn"></param>
        public static void Insert(Sec_Role role, OracleConnection conn)
        {
            //get the next id for the nrole
            if (role.Role_Id <= 0)
                role.Role_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.seq_security"));
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.sec_role (role_id, role_name, role_description,");
            sql.AppendLine("    created_date, created_by, application_id, modified_by,");
            sql.AppendLine("    role_notes, active)");
            sql.AppendLine("VALUES(:role_id, :role_name, :role_description, ");
            sql.AppendLine("    :created_date, :created_by, :application_id, :modified_by,");
            sql.AppendLine("    :role_notes, :active)");

            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("role_id", role.Role_Id);
            ins.Parameters.Add("role_name", role.Role_Name);
            ins.Parameters.Add("role_description", role.Role_Description);
            ins.Parameters.Add("created_date", DateTime.Now);
            ins.Parameters.Add("created_by", role.Created_By);
            ins.Parameters.Add("application_id", role.Application_Id);
            ins.Parameters.Add("modified_by", role.Modified_By);
            ins.Parameters.Add("role_notes", role.Role_Notes);
            ins.Parameters.Add("active", role.Active ? 1 : 0);

            MOO.Data.ExecuteNonQuery(ins);
        }
        /// <summary>
        /// updates the specified role
        /// </summary>
        /// <param name="role"></param>
        /// <param name="conn"></param>
        public static int Update(Sec_Role role)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                return Update(role, conn);
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
        /// updates the specified role
        /// </summary>
        /// <param name="role"></param>
        /// <param name="conn"></param>
        public static int Update(Sec_Role role, OracleConnection conn)
        {

            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.sec_role");
            sql.AppendLine("SET role_name = :role_name, role_description = :role_description,");
            sql.AppendLine("    created_date = :created_date, created_by = :created_by,");
            sql.AppendLine("    application_id = :application_id, modified_by = :modified_by,");
            sql.AppendLine("    role_notes = :role_notes, active = :active");
            sql.AppendLine("WHERE role_id = :role_id");

            OracleCommand upd = new(sql.ToString(), conn);
            upd.Parameters.Add("role_name", role.Role_Name);
            upd.Parameters.Add("role_description", role.Role_Description);
            upd.Parameters.Add("created_date", role.Created_Date);
            upd.Parameters.Add("created_by", role.Created_By);
            upd.Parameters.Add("application_id", role.Application_Id);
            upd.Parameters.Add("modified_by", role.Modified_By);
            upd.Parameters.Add("role_notes", role.Role_Notes);
            upd.Parameters.Add("active", role.Active ? 1 : 0);
            upd.Parameters.Add("role_id", role.Role_Id);

            return MOO.Data.ExecuteNonQuery(upd);
        }





        /// <summary>
        /// Adds the user to the specified role
        /// </summary>
        /// <param name="usr">the user to add to the role</param>
        /// <param name="role">the role to add the user to</param>
        /// <param name="GrantedBy">Who is granting the access</param>
        public static void AddUserToRole(Sec_Users usr, Sec_Role role, string GrantedBy)
        {
            AddUserToRole(usr.User_Id, role.Role_Id, GrantedBy);
        }

        /// <summary>
        /// Adds the user to the specified role
        /// </summary>
        /// <param name="user_Id">the id of the user</param>
        /// <param name="role_Id">the role id to add the user to</param>
        /// <param name="GrantedBy">Who is granting the access</param>
        public static void AddUserToRole(int user_Id, int role_Id, string GrantedBy)
        {
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.sec_access (access_id, user_id, role_id,");
            sql.AppendLine("created_date, created_by)");
            sql.AppendLine($"VALUES(seq_security.nextval,{user_Id},{role_Id},");
            sql.AppendLine("SYSDATE, :created_by)");

            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("created_by", GrantedBy);
            conn.Open();
            try
            {
                MOO.Data.ExecuteNonQuery(ins);
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
        /// removes access for a given user
        /// </summary>
        /// <param name="user_Id"></param>
        /// <param name="role_Id"></param>
        public static void RemoveUserFromRole(Sec_Users user, Sec_Role role)
        {
            RemoveUserFromRole(user.User_Id, role.Role_Id);
        }

        /// <summary>
        /// removes access for a given user
        /// </summary>
        /// <param name="user_Id"></param>
        /// <param name="role_Id"></param>
        public static void RemoveUserFromRole(int user_Id, int role_Id)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.sec_access");
            sql.AppendLine($"WHERE user_id = {user_Id} AND role_id ={role_Id}");

            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand del = new(sql.ToString(), conn);
            conn.Open();
            try
            {
                MOO.Data.ExecuteNonQuery(del);
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
        /// Gets a list of user for specified role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static List<Sec_Users> GetUsersInRole(Sec_Role role)
        {
            return Sec_UserSvc.GetUsersInRole(role.Role_Id);
        }

        /// <summary>
        /// Gets a list of user for specified role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static List<Sec_Users> GetUsersInRole(int role_id)
        {
            return Sec_UserSvc.GetUsersInRole(role_id);

        }



        private static Sec_Role DataRowToObject(DbDataReader row)
        {
            Sec_Role RetVal = new();

            RetVal.Role_Id = Convert.ToInt32(row["Role_Id"]);
            RetVal.Role_Name = row["Role_Name"].ToString();
            RetVal.Role_Description = row["Role_Description"].ToString();
            RetVal.Created_Date = Convert.ToDateTime(row["created_date"]);
            RetVal.Created_By = row.IsDBNull("Created_By") ? "" : row["Created_By"].ToString();
            RetVal.Application_Id = row.IsDBNull("Application_Id") ? null : Convert.ToInt32(row["Application_Id"]);
            RetVal.Modified_By = row.IsDBNull("Modified_By") ? "" : row["Modified_By"].ToString();
            RetVal.Role_Notes = row.IsDBNull("Role_Notes") ? "" : row["Role_Notes"].ToString();
            RetVal.Active = Convert.ToBoolean(row["Active"]);

            return RetVal;
        }







        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}role_id {ColPrefix}role_id, {ta}role_name {ColPrefix}role_name, ");
            cols.AppendLine($"{ta}role_description {ColPrefix}role_description, {ta}created_date {ColPrefix}created_date, ");
            cols.AppendLine($"{ta}created_by {ColPrefix}created_by, {ta}application_id {ColPrefix}application_id, ");
            cols.AppendLine($"{ta}modified_by {ColPrefix}modified_by, {ta}role_notes {ColPrefix}role_notes, ");
            cols.AppendLine($"{ta}active {ColPrefix}active");
            return cols.ToString();
        }



        internal static Sec_Role DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Sec_Role RetVal = new();
            RetVal.Role_Id = (int)(decimal)Util.GetRowVal(row, $"{ColPrefix}role_id");
            RetVal.Role_Name = (string)Util.GetRowVal(row, $"{ColPrefix}role_name");
            RetVal.Role_Description = (string)Util.GetRowVal(row, $"{ColPrefix}role_description");
            RetVal.Created_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}created_date");
            RetVal.Created_By = (string)Util.GetRowVal(row, $"{ColPrefix}created_by");
            RetVal.Application_Id = (int?)(decimal?)Util.GetRowVal(row, $"{ColPrefix}application_id");
            RetVal.Modified_By = (string)Util.GetRowVal(row, $"{ColPrefix}modified_by");
            RetVal.Role_Notes = (string)Util.GetRowVal(row, $"{ColPrefix}role_notes");
            RetVal.Active = ((decimal?)Util.GetRowVal(row, $"{ColPrefix}active") ?? 0) == 1 ? true: false;

            return RetVal;
        }
    }
}
