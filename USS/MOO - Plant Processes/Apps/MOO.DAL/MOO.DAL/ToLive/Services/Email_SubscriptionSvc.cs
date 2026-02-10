using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System.Net;
using MOO.DAL.ToLive.Enums;

namespace MOO.DAL.ToLive.Services
{
    /// <summary>
    /// class used for insert/update/delete/select of the tolive.email_subscription table
    /// for use of this to send emails use MOO.EmailSubscription class
    /// </summary>
    public class Email_SubscriptionSvc
    {
        static Email_SubscriptionSvc()
        {
            Util.RegisterOracle();
        }

        /// <summary>
        /// Gets the email subscription from the primary key
        /// </summary>
        /// <param name="email_subscription_id"></param>
        /// <returns></returns>
        public static Email_Subscription Get(int email_subscription_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE email_subscription_id = {email_subscription_id}");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }

        /// <summary>
        /// Gets the list of email subscriptions
        /// </summary>
        /// <param name="Tag">Optional Filter by tag</param>
        /// <returns></returns>
        public static List<Email_Subscription> GetAll(string Tag = "")
        {
            List<Email_Subscription> retVal = new();
            StringBuilder sql = new();
            sql.Append(GetSelect());
            if (Tag != String.Empty)
            {
                sql.AppendLine($"WHERE tags LIKE '%{Tag}%'");
            }


            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                retVal.Add(DataRowToObject(dr));
            }
            return retVal;
        }

        /// <summary>
        /// Lists the email subscriptions that the specified user is subscribed to
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public static List<Email_Subscription> GetByUsername(string UserName)
        {
            List<Email_Subscription> retVal = new();
            StringBuilder sql = new();
            sql.Append(GetSelect());

            sql.AppendLine("INNER JOIN tolive.email_subscription_user ess");
            sql.AppendLine("    ON es.email_subscription_id = ess.email_subscription_id");
            sql.AppendLine($"WHERE username = '{UserName.ToLower()}'");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                retVal.Add(DataRowToObject(dr));
            }
            return retVal;
        }


        private static string GetSelect(string addField = "")
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine("es.email_subscription_id, es.subscription_name, es.description, es.application, es.hide_user_emails, ");
            sql.AppendLine("es.last_used, es.tags, es.manual_subscribe, es.directions, es.comments, es.subscription_type");
            if (!string.IsNullOrEmpty(addField))
            {
                sql.Append(", ");
                sql.AppendLine(addField);
            }
            sql.AppendLine("FROM tolive.email_subscription es");
            return sql.ToString();
        }


        public static int Insert(Email_Subscription obj)
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


        public static int Insert(Email_Subscription obj, OracleConnection conn)
        {
            if (obj.Subscription_Type == Email_Subscription_Type.PI_Notification)
                throw new Exception("Invalid subscription type, PI Notifications are pulled from PI AF directly and not stored in the database");
            if (obj.Email_Subscription_Id <= 0)
                obj.Email_Subscription_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.seq_email_subscription"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.email_subscription(");
            sql.AppendLine("email_subscription_id, subscription_name, description, application, hide_user_emails, ");
            sql.AppendLine("last_used, tags, manual_subscribe, directions, comments, subscription_type)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":email_subscription_id, :subscription_name, :description, :application, ");
            sql.AppendLine(":hide_user_emails, :last_used, :tags, :manual_subscribe, :directions, :comments, :subscription_type)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("email_subscription_id", obj.Email_Subscription_Id);
            ins.Parameters.Add("subscription_name", obj.Subscription_Name);
            ins.Parameters.Add("description", obj.Description);
            ins.Parameters.Add("application", obj.Application);
            ins.Parameters.Add("hide_user_emails", obj.Hide_User_Emails ? 1 : 0);
            ins.Parameters.Add("last_used", obj.Last_Used);
            ins.Parameters.Add("tags", obj.Tags);
            ins.Parameters.Add("manual_subscribe", obj.Subscription_Type == Email_Subscription_Type.Manual_Subscribe ? 1 : 0);
            ins.Parameters.Add("directions", obj.Directions);
            ins.Parameters.Add("comments", obj.Comments);
            ins.Parameters.Add("subscription_type", (short)obj.Subscription_Type);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Email_Subscription obj)
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


        public static int Update(Email_Subscription obj, OracleConnection conn)
        {
            if (obj.Subscription_Type == Email_Subscription_Type.PI_Notification)
                throw new Exception("Invalid subscription type, PI Notifications are pulled from PI AF directly and not stored in the database");
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.email_subscription SET");
            sql.AppendLine("subscription_name = :subscription_name, ");
            sql.AppendLine("description = :description, ");
            sql.AppendLine("application = :application, ");
            sql.AppendLine("hide_user_emails = :hide_user_emails, ");
            sql.AppendLine("last_used = :last_used, ");
            sql.AppendLine("tags = :tags,");
            sql.AppendLine("manual_subscribe = :manual_subscribe,");
            sql.AppendLine("directions = :directions,");
            sql.AppendLine("comments = :comments,");
            sql.AppendLine("subscription_type = :subscription_type");
            sql.AppendLine("WHERE email_subscription_id = :email_subscription_id"); OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("subscription_name", obj.Subscription_Name);
            upd.Parameters.Add("description", obj.Description);
            upd.Parameters.Add("application", obj.Application);
            upd.Parameters.Add("hide_user_emails", obj.Hide_User_Emails ? 1 : 0);
            upd.Parameters.Add("last_used", obj.Last_Used);
            upd.Parameters.Add("tags", obj.Tags);
            upd.Parameters.Add("manual_subscribe", obj.Subscription_Type == Email_Subscription_Type.Manual_Subscribe ? 1 : 0);
            upd.Parameters.Add("directions", obj.Directions);
            upd.Parameters.Add("comments", obj.Comments);
            upd.Parameters.Add("subscription_type", (short)obj.Subscription_Type);
            upd.Parameters.Add("email_subscription_id", obj.Email_Subscription_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Email_Subscription obj)
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


        public static int Delete(Email_Subscription obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.email_subscription");
            sql.AppendLine("WHERE email_subscription_id = :email_subscription_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.Parameters.Add("email_subscription_id", obj.Email_Subscription_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        private static Email_Subscription DataRowToObject(DataRow row)
        {
            Email_Subscription RetVal = new();
            RetVal.Email_Subscription_Id = Convert.ToInt32(row.Field<decimal>("email_subscription_id"));
            RetVal.Subscription_Name = row.Field<string>("subscription_name");
            RetVal.Description = row.Field<string>("description");
            RetVal.Application = row.Field<string>("application");
            RetVal.Hide_User_Emails = row.Field<short>("hide_user_emails") == 1;
            RetVal.Last_Used = row.Field<DateTime?>("last_used");
            RetVal.Tags = row.Field<string>("tags");
            RetVal.Directions = row.Field<string>("directions");
            RetVal.Comments = row.Field<string>("comments");
            RetVal.Subscription_Type = (Email_Subscription_Type)row.Field<short>("subscription_type");
            return RetVal;
        }

    }
}
