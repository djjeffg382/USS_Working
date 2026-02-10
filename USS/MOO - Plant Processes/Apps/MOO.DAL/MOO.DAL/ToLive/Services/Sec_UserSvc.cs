using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.DAL.ToLive.Models;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;

namespace MOO.DAL.ToLive.Services
{
    /// <summary>
    /// Class for getting sec_user information
    /// </summary>
    public static class Sec_UserSvc
    {
        public const string TABLE_NAME = "TOLIVE.sec_users";
        static Sec_UserSvc()
        {
            Util.RegisterOracle();
        }

        #region "Get Code"

        /// <summary>
        /// Gets the sec user from the user id
        /// </summary>
        /// <param name="user_Id"></param>
        /// <returns></returns>
        public static Sec_Users Get(int User_Id)
        {
            StringBuilder sql = new();
            sql.AppendLine(GetSelect());
            sql.AppendLine($"WHERE user_id = {User_Id}");


            Sec_Users retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            DbDataReader rdr = MOO.Data.ExecuteReader(cmd);
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }

        /// <summary>
        /// Gets the user based on the user with the domain included (example mno\user123
        /// </summary>
        /// <param name="user_With_Domain"></param>
        /// <returns></returns>
        public static Sec_Users Get(string user_With_Domain)
        {
            string[] usrDom = user_With_Domain.Split("\\");
            //array must contain 2 items or the passed parameter is invalid
            if (usrDom.Length == 2)
                return Get(usrDom[0], usrDom[1]);
            else
                return null;
        }

        /// <summary>
        /// Gets the sec_user object based on user and domain
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public static Sec_Users Get(string Domain, string Username)
        {
            StringBuilder sql = new();
            sql.AppendLine(GetSelect());
            sql.AppendLine($"WHERE LOWER(username) = :Username");
            sql.AppendLine($"AND LOWER(domain) = :Domain");


            Sec_Users retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("Username", Username.ToLower());
            cmd.Parameters.Add("Domain", Domain.ToLower());
            DbDataReader rdr = MOO.Data.ExecuteReader(cmd);
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }


        /// <summary>
        /// Gets the sec_user object based on user and domain
        /// </summary>
        /// <param name="IncludeInactive">Whether to include inactive Sec_Users</param>
        /// <returns></returns>
        public static List<Sec_Users> GetAll(bool IncludeInactive = true)
        {
            StringBuilder sql = new();
            sql.AppendLine(GetSelect());
            if (!IncludeInactive)
                sql.AppendLine("WHERE active = 1");
            sql.AppendLine("ORDER BY last_name");

            List<Sec_Users> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            DbDataReader rdr = MOO.Data.ExecuteReader(cmd);
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

        /// <summary>
        /// Gets a list of user for specified role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static List<Sec_Users> GetUsersInRole(Sec_Role role)
        {
            return GetUsersInRole(role.Role_Id);

        }

        /// <summary>
        /// Gets a list of user for specified role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static List<Sec_Users> GetUsersInRole(int Role_Id)
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT su.*");
            sql.AppendLine("FROM tolive.sec_users su");
            sql.AppendLine("INNER JOIN tolive.sec_access sa");
            sql.AppendLine("ON su.user_id = sa.user_id");
            sql.AppendLine($"WHERE sa.role_id = :Role_Id");
            sql.AppendLine("Order by Last_Name, first_name");


            List<Sec_Users> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("Role_Id", Role_Id);
            DbDataReader rdr = MOO.Data.ExecuteReader(cmd);
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

        #endregion
        #region "Get UserOptions only"

        /// <summary>
        /// Gets the User options of the specified user
        /// </summary>
        /// <param name="user_Id"></param>
        /// <returns></returns>
        public static string GetAllUserOptions(int user_Id)
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT useroptions");
            sql.AppendLine("FROM tolive.sec_users");
            sql.AppendLine($"WHERE user_id = {user_Id}");

            DataSet ds = Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count == 1)
            {
                return ds.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Gets the User options of the specified user
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="username">AD username</param>
        /// <returns></returns>
        public static string GetAllUserOptions(string domain, string username)
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT useroptions");
            sql.AppendLine("FROM tolive.sec_users");
            sql.AppendLine($"WHERE LOWER(username) = '{username.Replace("'", "''").ToLower()}'");
            sql.AppendLine($"AND LOWER(domain) = '{domain.Replace("'", "''").ToLower()}'");

            DataSet ds = Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count == 1)
            {
                return ds.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Gets the User options of the specified user
        /// </summary>
        /// <param name="user_With_Domain"></param>
        /// <returns></returns>
        public static string GetAllUserOptions(string user_With_Domain)
        {
            string[] usrDom = user_With_Domain.Split("\\");
            //array must contain 2 items or the passed parameter is invalid
            if (usrDom.Length == 2)
                return GetAllUserOptions(usrDom[0], usrDom[1]);
            else
                return null;
        }
        #endregion

        #region "Save User options"

        /// <summary>
        /// updates the sec user UserOptions
        /// </summary>
        public static int UpdateAllUserOptions(int userId, string userOptions)
        {

            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));

            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.sec_users");
            sql.AppendLine("SET  useroptions = :useroptions");
            sql.AppendLine("WHERE user_id = :user_id");

            OracleCommand upd = new(sql.ToString(), conn);
            upd.Parameters.Add("useroptions", userOptions);
            upd.Parameters.Add("user_id", userId);
            conn.Open();
            try
            {
                return upd.ExecuteNonQuery();
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
        /// updates the sec user UserOptions
        /// </summary>
        public static int UpdateAllUserOptions(string user_With_Domain, string userOptions)
        {
            string[] usrDom = user_With_Domain.Split("\\");
            //array must contain 2 items or the passed parameter is invalid
            if (usrDom.Length == 2)
                return UpdateAllUserOptions(usrDom[0], usrDom[1], userOptions);
            else
                return 0;
        }


        /// <summary>
        /// updates the sec user UserOptions
        /// </summary>
        public static int UpdateAllUserOptions(string domain, string username, string userOptions)
        {

            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));

            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.sec_users");
            sql.AppendLine("SET  useroptions = :useroptions");
            sql.AppendLine($"WHERE LOWER(username) = :username");
            sql.AppendLine($"AND LOWER(domain) = :domain");

            OracleCommand upd = new(sql.ToString(), conn);
            upd.Parameters.Add("useroptions", userOptions);
            upd.Parameters.Add("username", username.ToLower());
            upd.Parameters.Add("domain", domain.ToLower());
            conn.Open();
            try
            {
                return upd.ExecuteNonQuery();
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

        #endregion


        #region "GetSingleUserOption"

        /// <summary>
        /// Gets a specific user option for the specified user
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="user_Id">Sec User ID</param>
        /// <param name="name">Name of the setting</param>
        /// <param name="defaultVal">Default Value if not found</param>
        /// <returns></returns>
        public static T GetUserOption<T>(int user_Id, string name, object defaultVal)
        {
            string json = GetAllUserOptions(user_Id);
            return GetJSONValueByName<T>(json, name, defaultVal);
        }
        /// <summary>
        /// Gets a specific user option for the specified user
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="domain">AD Domain</param>
        /// <param name="username">AD Username</param>
        /// <param name="name">Name of the setting</param>
        /// <param name="defaultVal">Default Value if not found</param>
        /// <returns></returns>
        public static T GetUserOption<T>(string domain, string username, string name, object defaultVal)
        {
            string json = GetAllUserOptions(domain, username);
            return GetJSONValueByName<T>(json, name, defaultVal);
        }

        /// <summary>
        /// Gets a specific user option for the specified user
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="user_With_Domain">User with domain (example: mno\abc123)</param>
        /// <param name="name">Name of the setting</param>
        /// <param name="defaultVal">Default Value if not found</param>
        /// <returns></returns>
        public static T GetUserOption<T>(string user_With_Domain, string name, object defaultVal)
        {
            string json = GetAllUserOptions(user_With_Domain);
            return GetJSONValueByName<T>(json, name, defaultVal);
        }


        /// <summary>
        /// Gets the value of a JSON setting 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Name">The json name setting</param>
        /// <param name="jsonString">The total JSON String</param>
        /// <param name="DefaultVal">Default value if JSON String is not found</param>
        /// <returns></returns>
        public static T GetJSONValueByName<T>(string jsonString, string name, object defaultVal)
        {
            JToken jsonObj;
            if (!string.Equals(jsonString, string.Empty))
                jsonObj = JObject.Parse(jsonString);
            else
                return (T)Convert.ChangeType(defaultVal, typeof(T));


            JToken tok = jsonObj.SelectToken("$." + name);
            if (tok == null)
                return (T)Convert.ChangeType(defaultVal, typeof(T));
            else
            {
                var value = tok.Value<object>();
                return (T)Convert.ChangeType(value, typeof(T));
            }
        }

        #endregion


        #region "Update JSON setting"

        /// <summary>
        /// Gets a specific user option for the specified user
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="user_Id">Sec User ID</param>
        /// <param name="name">Name of the setting</param>
        /// <param name="defaultVal">Default Value if not found</param>
        /// <returns></returns>
        public static void UpdateUserOption(int user_Id, string name, JToken value)
        {
            string json = GetAllUserOptions(user_Id);
            json = UpdateJSonSettingInString(json, name, value);
            UpdateAllUserOptions(user_Id, json);
        }
        /// <summary>
        /// Gets a specific user option for the specified user
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="domain">AD Domain</param>
        /// <param name="username">AD Username</param>
        /// <param name="name">Name of the setting</param>
        /// <param name="defaultVal">Default Value if not found</param>
        /// <returns></returns>
        public static void UpdateUserOption(string domain, string username, string name, JToken value)
        {
            string json = GetAllUserOptions(domain, username);
            json = UpdateJSonSettingInString(json, name, value);
            UpdateAllUserOptions(domain, username, json);
        }

        /// <summary>
        /// Gets a specific user option for the specified user
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="user_With_Domain">User with domain (example: mno\abc123)</param>
        /// <param name="name">Name of the setting</param>
        /// <param name="defaultVal">Default Value if not found</param>
        /// <returns></returns>
        public static void UpdateUserOption(string user_With_Domain, string name, JToken value)
        {
            string json = GetAllUserOptions(user_With_Domain);
            json = UpdateJSonSettingInString(json, name, value);
            UpdateAllUserOptions(user_With_Domain, json);
        }






        /// <summary>
        /// updates a json setting in a json string and returns the new json string
        /// </summary>
        /// <param name="jsonString">json string to update</param>
        /// <param name="name">name of the setting to update</param>
        /// <param name="value">value to set it to</param>
        /// <returns></returns>
        public static string UpdateJSonSettingInString(string jsonString, string name, JToken value)
        {
            JObject jsonObj;
            if (!string.Equals(jsonString, string.Empty))
                jsonObj = JObject.Parse(jsonString);
            else
            {
                jsonObj = JObject.Parse("{}");
            }
            JToken tok = jsonObj.SelectToken("$." + name);
            if (tok == null)
            {
                jsonObj.Add(name, null);
            }
            jsonObj.Property(name).Value = value;

            return jsonObj.ToString();

        }

        #endregion


        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}user_id {ColPrefix}user_id, {ta}username {ColPrefix}username, {ta}domain {ColPrefix}domain, ");
            cols.AppendLine($"{ta}active {ColPrefix}active, {ta}created_date {ColPrefix}created_date, ");
            cols.AppendLine($"{ta}created_by {ColPrefix}created_by, {ta}email {ColPrefix}email, ");
            cols.AppendLine($"{ta}first_name {ColPrefix}first_name, {ta}last_name {ColPrefix}last_name, ");
            cols.AppendLine($"{ta}sy_user_id {ColPrefix}sy_user_id, {ta}modified_by {ColPrefix}modified_by, ");
            cols.AppendLine($"{ta}pw_expire_check {ColPrefix}pw_expire_check, {ta}last_edited_date {ColPrefix}last_edited_date, ");
            cols.AppendLine($"{ta}terminated {ColPrefix}terminated, {ta}useroptions {ColPrefix}useroptions ");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.sec_users");
            return sql.ToString();
        }




        /// <summary>
        /// inserts the user into the sec_user table
        /// </summary>
        /// <param name="usr"></param>
        /// <returns></returns>
        /// <remarks>User_id will be generated</remarks>
        public static void Insert(Sec_Users usr)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                Insert(usr, conn);
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
        /// inserts the user into the sec_user table
        /// </summary>
        /// <param name="usr"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        /// <remarks>User_id will be generated if zero or negative</remarks>
        public static void Insert(Sec_Users usr, OracleConnection conn)
        {
            //get the next id for the new user
            if (usr.User_Id <= 0)
                usr.User_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.seq_security"));
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.sec_users (user_id, username, domain, active, created_date,");
            sql.AppendLine("    created_by, email, first_name, last_name, sy_user_id, modified_by,");
            sql.AppendLine("    pw_expire_check, last_edited_date, terminated, useroptions)");
            sql.AppendLine("VALUES(:user_id, :username, :domain, :active, :created_date,");
            sql.AppendLine("    :created_by, :email, :first_name, :last_name, :sy_user_id, :modified_by,");
            sql.AppendLine("    :pw_expire_check, :last_edited_date, :terminated, :useroptions)");

            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("user_id", usr.User_Id);
            ins.Parameters.Add("username", usr.Username);
            ins.Parameters.Add("domain", usr.Domain);
            ins.Parameters.Add("active", usr.Active ? 1 : 0);
            ins.Parameters.Add("created_date", usr.Created_Date);
            if (string.IsNullOrEmpty(usr.Created_By))
                ins.Parameters.Add("created_by", "N/A");        //created by cannot be null or it causes issues with older system
            else
                ins.Parameters.Add("created_by", usr.Created_By);
            ins.Parameters.Add("email", usr.Email);
            ins.Parameters.Add("first_name", usr.First_Name);
            ins.Parameters.Add("last_name", string.IsNullOrEmpty(usr.Last_Name) ? " " : usr.Last_Name);  //DB table doesn't allow null and an empty string is null for oracle
            ins.Parameters.Add("sy_user_id", usr.Sy_User_Id);
            ins.Parameters.Add("modified_by", usr.Modified_By);
            ins.Parameters.Add("pw_expire_check", usr.Pw_Expire_Check);
            ins.Parameters.Add("last_edited_date", usr.Last_Edited_Date);
            ins.Parameters.Add("terminated", usr.Terminated ? 1 : 0);
            ins.Parameters.Add("useroptions", usr.UserOptions);

            ins.ExecuteNonQuery();
        }


        /// <summary>
        /// updates the sec user record
        /// </summary>
        /// <param name="usr"></param>
        public static int Update(Sec_Users usr)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                return Update(usr, conn);
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
        /// updates the sec user record
        /// </summary>
        /// <param name="usr"></param>
        /// <param name="conn"></param>
        public static int Update(Sec_Users usr, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.sec_users");
            sql.AppendLine("SET username = :username, domain = :domain, active = :active,");
            sql.AppendLine("    created_date = :created_date, created_by = :created_by, email = :email,");
            sql.AppendLine("    first_name = :first_name, last_name = :last_name, ");
            sql.AppendLine("    sy_user_id = :sy_user_id, modified_by = :modified_by,");
            sql.AppendLine("    pw_expire_check = :pw_expire_check, last_edited_date = :last_edited_date,");
            sql.AppendLine("    terminated = :terminated, useroptions = :useroptions");
            sql.AppendLine("WHERE user_id = :user_id");

            OracleCommand upd = new(sql.ToString(), conn);
            upd.Parameters.Add("username", usr.Username);
            upd.Parameters.Add("domain", usr.Domain);
            upd.Parameters.Add("active", usr.Active ? 1 : 0);
            upd.Parameters.Add("created_date", usr.Created_Date);
            if (string.IsNullOrEmpty(usr.Created_By))
                upd.Parameters.Add("created_by", "N/A");        //created by cannot be null or it causes issues with older system
            else
                upd.Parameters.Add("created_by", usr.Created_By);
            upd.Parameters.Add("email", usr.Email);
            upd.Parameters.Add("first_name", usr.First_Name);
            upd.Parameters.Add("last_name", string.IsNullOrEmpty(usr.Last_Name) ? " " : usr.Last_Name);  //DB table doesn't allow null and an empty string is null for oracle
            upd.Parameters.Add("sy_user_id", usr.Sy_User_Id);
            upd.Parameters.Add("modified_by", usr.Modified_By);
            upd.Parameters.Add("pw_expire_check", usr.Pw_Expire_Check);
            upd.Parameters.Add("last_edited_date", usr.Last_Edited_Date);
            upd.Parameters.Add("terminated", usr.Terminated ? 1 : 0);
            upd.Parameters.Add("useroptions", usr.UserOptions);
            upd.Parameters.Add("user_id", usr.User_Id);

            return upd.ExecuteNonQuery();

        }





        internal static Sec_Users DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Sec_Users RetVal = new();
            RetVal.User_Id = Convert.ToInt32((decimal)Util.GetRowVal(row, $"{ColPrefix}user_id"));
            RetVal.Username = (string)Util.GetRowVal(row, $"{ColPrefix}username");
            RetVal.Domain = (string)Util.GetRowVal(row, $"{ColPrefix}domain");
            RetVal.Active = (decimal)Util.GetRowVal(row, $"{ColPrefix}active") == 1;
            RetVal.Created_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}created_date");
            RetVal.Created_By = (string)Util.GetRowVal(row, $"{ColPrefix}created_by");
            RetVal.Email = (string)Util.GetRowVal(row, $"{ColPrefix}email");
            RetVal.First_Name = (string)Util.GetRowVal(row, $"{ColPrefix}first_name");
            RetVal.Last_Name = (string)Util.GetRowVal(row, $"{ColPrefix}last_name");
            RetVal.Sy_User_Id = Convert.ToInt32((decimal?)Util.GetRowVal(row, $"{ColPrefix}sy_user_id"));
            RetVal.Modified_By = (string)Util.GetRowVal(row, $"{ColPrefix}modified_by");
            RetVal.Pw_Expire_Check = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}pw_expire_check");
            RetVal.Last_Edited_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}last_edited_date");
            RetVal.Terminated = (decimal?)Util.GetRowVal(row, $"{ColPrefix}terminated") == 1;
            RetVal.UserOptions = (string)Util.GetRowVal(row, $"{ColPrefix}useroptions");
            return RetVal;
        }





    }
}
