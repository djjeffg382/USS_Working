using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;

namespace MOO.DAL.ToLive.Services
{
    public class Email_Subscription_UserSvc
    {
        static Email_Subscription_UserSvc()
        {
            Util.RegisterOracle();
        }


        public static Email_Subscription_User Get(int Subscriber_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE subscriber_id = {Subscriber_id}");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }


        /// <summary>
        /// Gets the list of email subscription user by email subscription_id
        /// </summary>
        /// <param name="Email_Subscription_Id">Optional Filter by email subscription</param>
        /// <returns></returns>
        public static List<Email_Subscription_User> GetAll(int Email_Subscription_Id)
        {
            List<Email_Subscription_User> retVal = new();
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE email_subscription_id = {Email_Subscription_Id}");


            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                retVal.Add(DataRowToObject(dr));
            }
            return retVal;
        }

        /// <summary>
        /// Gets the list of email subscription user by email subscription_id
        /// </summary>
        /// <param name="Email_Subscription_Id">Optional Filter by email subscription</param>
        /// <returns></returns>
        public static Email_Subscription_User GetByUsername(int Email_Subscription_Id, string UserName)
        {
            Email_Subscription_User retVal;
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE email_subscription_id = {Email_Subscription_Id}");
            sql.AppendLine($"AND username = '{UserName.ToLower()}'");


            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count > 0)
            {
                retVal = DataRowToObject(ds.Tables[0].Rows[0]);
                return retVal;
            }
            else
                return null;
        }



        private static string GetSelect(string addField = "")
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine("subscriber_id, email_subscription_id, username, email_address, comments");
            if (!string.IsNullOrEmpty(addField))
            {
                sql.Append(", ");
                sql.AppendLine(addField);
            }
            sql.AppendLine("FROM tolive.email_subscription_user");
            return sql.ToString();
        }


        public static int Insert(Email_Subscription_User obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Email_Subscription_User obj, OracleConnection conn)
        {
            if (obj.Subscriber_Id <= 0)
                obj.Subscriber_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.seq_email_subscription"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.email_subscription_user(");
            sql.AppendLine("subscriber_id, email_subscription_id, username, email_address, comments)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":subscriber_id, :email_subscription_id, :username, :email_address, :comments)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("subscriber_id", obj.Subscriber_Id);
            ins.Parameters.Add("email_subscription_id", obj.Email_Subscription_Id);
            ins.Parameters.Add("username", obj.Username.ToLower());
            ins.Parameters.Add("email_address", obj.Email_Address);
            ins.Parameters.Add("comments", obj.Comments);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Email_Subscription_User obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Email_Subscription_User obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.email_subscription_user SET");
            sql.AppendLine("email_subscription_id = :email_subscription_id, ");
            sql.AppendLine("username = :username, ");
            sql.AppendLine("email_address = :email_address, ");
            sql.AppendLine("comments = :comments");
            sql.AppendLine("WHERE subscriber_id = :subscriber_id"); 
            OracleCommand upd = new(sql.ToString(), conn);
            upd.Parameters.Add("email_subscription_id", obj.Email_Subscription_Id);
            upd.Parameters.Add("username", obj.Username.ToLower());
            upd.Parameters.Add("email_address", obj.Email_Address);
            upd.Parameters.Add("comments", obj.Comments);
            upd.Parameters.Add("subscriber_id", obj.Subscriber_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Email_Subscription_User obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        /// <summary>
        /// changes the domain on the user from mno to hdq.  Used for changeover of users from mno -> hdq
        /// </summary>
        /// <param name="Username">the username without the domain</param>
        ///<returns>Returns number of rows updated</returns>
        public static int MoveDomainMnoToHdq(string Username)
        {
            if (Username.Contains("\\"))
                throw new Exception("Provide the username without the domain");
            var sql = @"UPDATE tolive.email_subscription_user
                        SET username = :newUser
                        WHERE LOWER(username) = :oldUser"; 
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand upd = new(sql, conn);
            upd.Parameters.Add("newUser", $"hdq\\{Username.ToLower()}");
            upd.Parameters.Add("oldUser", $"mno\\{Username.ToLower()}");
            int recsAffected = upd.ExecuteNonQuery();
            conn.Close();
            return recsAffected;
        }


        public static int Delete(Email_Subscription_User obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.email_subscription_user");
            sql.AppendLine("WHERE subscriber_id = :subscriber_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.Parameters.Add("subscriber_id", obj.Subscriber_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        private static Email_Subscription_User DataRowToObject(DataRow row)
        {
            Email_Subscription_User RetVal = new();
            RetVal.Subscriber_Id = Convert.ToInt32(row.Field<decimal>("subscriber_id"));
            RetVal.Email_Subscription_Id = Convert.ToInt32(row.Field<decimal>("email_subscription_id"));
            RetVal.Username = row.Field<string>("username");
            RetVal.Email_Address = row.Field<string>("email_address");
            RetVal.Comments = row.Field<string>("comments");
            return RetVal;
        }
    }
}
