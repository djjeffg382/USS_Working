using MOO.DAL.Core.Models;
using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Services
{

    public static class Sec_RequestSvc {
        static Sec_RequestSvc()
        {
            Util.RegisterOracle();
        }

        public static Sec_Request Get(long sec_request_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE sr_sec_request_id = :sec_request_id");


            Sec_Request retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("sec_request_id", sec_request_id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }

        public static List<Sec_Request> GetByUserID(long userId)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE sr_additional_info_user =  :userID");
            sql.AppendLine($"OR sr_approval_by =  :userID");
            sql.AppendLine($"OR sr_closed_user =  :userID");

            List<Sec_Request> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("userID", userId);
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

        public static List<Sec_Request> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            List<Sec_Request> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
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

        public static List<Sec_Request> GetAllByItemId(long id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE sr_sec_request_item_id =  :itemId");
            List<Sec_Request> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName=true;
            cmd.Parameters.Add("itemId", id);
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


        private static string GetSelect()
        {
            //SQL select was a pain so I added a view
            StringBuilder sql = new();
            sql.AppendLine("SELECT * FROM tolive.SEC_REQUEST_V");
            return sql.ToString();
        }

        public static int Insert(Sec_Request obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Insert(Sec_Request obj, OracleConnection conn)
        {
            if (obj.Sec_Request_Id <= 0)
            {
                string sqlId = "SELECT NVL(MAX(sec_request_id),0) FROM tolive.sec_request";
                int nextId = Convert.ToInt32((decimal)MOO.Data.ExecuteScalar(sqlId, Data.MNODatabase.DMART)) + 1;
                obj.Sec_Request_Id = nextId;
            }

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.sec_request(");
            sql.AppendLine("sec_request_id, request_first_name, request_last_name, request_username, manager, ");
            sql.AppendLine("sec_request_item_id, request_comments, created_date, additional_info_user, ");
            sql.AppendLine("additional_info_comments, current_status, approval_by, approval_date, ");
            sql.AppendLine("approval_comments, closed_user, closed_comments, approved, user_id, email_flag)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":sec_request_id, :request_first_name, :request_last_name, :request_username, :manager, ");
            sql.AppendLine(":sec_request_item_id, :request_comments, :created_date, :additional_info_user, ");
            sql.AppendLine(":additional_info_comments, :current_status, :approval_by, :approval_date, ");
            sql.AppendLine(":approval_comments, :closed_user, :closed_comments, :approved, :user_id, :email_flag)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("sec_request_id", obj.Sec_Request_Id);
            ins.Parameters.Add("request_first_name", obj.Request_First_Name);
            ins.Parameters.Add("request_last_name", obj.Request_Last_Name);
            ins.Parameters.Add("request_username", obj.Request_Username);
            ins.Parameters.Add("manager", obj.Manager.User_Id);
            ins.Parameters.Add("sec_request_item_id", obj.Sec_Request_Item.Sec_Request_Item_Id);
            ins.Parameters.Add("request_comments", obj.Request_Comments);
            ins.Parameters.Add("created_date", obj.Created_Date);
            ins.Parameters.Add("additional_info_user", obj.Additional_Info_User?.User_Id);
            ins.Parameters.Add("additional_info_comments", obj.Additional_Info_Comments);
            ins.Parameters.Add("current_status", obj.Current_Status?.Status_Id);
            ins.Parameters.Add("approval_by", obj.Approval_By?.User_Id);
            ins.Parameters.Add("approval_date", obj.Approval_Date);
            ins.Parameters.Add("approval_comments", obj.Approval_Comments);
            ins.Parameters.Add("closed_user", obj.Closed_User?.User_Id);
            ins.Parameters.Add("closed_comments", obj.Closed_Comments);
            if (obj.Approved == null)
            {
                ins.Parameters.Add("approved", null);
            }
            else
            {
                ins.Parameters.Add("approved", obj.Approved==true? 'Y' : 'N');
            }
            ins.Parameters.Add("user_id", obj.User.User_Id);
            ins.Parameters.Add("email_flag", obj.Email_Flag ? 'Y' : 'N');
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }

        public static int Update(Sec_Request obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Update(Sec_Request obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.sec_request SET");
            sql.AppendLine("request_first_name = :request_first_name, ");
            sql.AppendLine("request_last_name = :request_last_name, ");
            sql.AppendLine("request_username = :request_username, ");
            sql.AppendLine("manager = :manager, ");
            sql.AppendLine("sec_request_item_id = :sec_request_item_id, ");
            sql.AppendLine("request_comments = :request_comments, ");
            sql.AppendLine("created_date = :created_date, ");
            sql.AppendLine("additional_info_user = :additional_info_user, ");
            sql.AppendLine("additional_info_comments = :additional_info_comments, ");
            sql.AppendLine("current_status = :current_status, ");
            sql.AppendLine("approval_by = :approval_by, ");
            sql.AppendLine("approval_date = :approval_date, ");
            sql.AppendLine("approval_comments = :approval_comments, ");
            sql.AppendLine("closed_user = :closed_user, ");
            sql.AppendLine("closed_comments = :closed_comments, ");
            sql.AppendLine("approved = :approved, ");
            sql.AppendLine("user_id = :user_id, ");
            sql.AppendLine("email_flag = :email_flag");
            sql.AppendLine("WHERE sec_request_id = :sec_request_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("sec_request_id", obj.Sec_Request_Id);
            upd.Parameters.Add("request_first_name", obj.Request_First_Name);
            upd.Parameters.Add("request_last_name", obj.Request_Last_Name);
            upd.Parameters.Add("request_username", obj.Request_Username);
            upd.Parameters.Add("manager", obj.Manager.User_Id);
            upd.Parameters.Add("sec_request_item_id", obj.Sec_Request_Item.Sec_Request_Item_Id);
            upd.Parameters.Add("request_comments", obj.Request_Comments);
            upd.Parameters.Add("created_date", obj.Created_Date);
            upd.Parameters.Add("additional_info_user", obj.Additional_Info_User?.User_Id);
            upd.Parameters.Add("additional_info_comments", obj.Additional_Info_Comments);
            upd.Parameters.Add("current_status", obj.Current_Status?.Status_Id);
            upd.Parameters.Add("approval_by", obj.Approval_By?.User_Id);
            upd.Parameters.Add("approval_date", obj.Approval_Date);
            upd.Parameters.Add("approval_comments", obj.Approval_Comments);
            upd.Parameters.Add("closed_user", obj.Closed_User?.User_Id);
            upd.Parameters.Add("closed_comments", obj.Closed_Comments);
            if (obj.Approved == null)
            {
                upd.Parameters.Add("approved", null);
            }
            else
            {
                upd.Parameters.Add("approved", obj.Approved == true ? 'Y' : 'N');
            }
            upd.Parameters.Add("user_id", obj.User.User_Id);
            upd.Parameters.Add("email_flag", obj.Email_Flag?'Y':'N');
            upd.Parameters.Add("sec_request_id", obj.Sec_Request_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }

        public static int Delete(Sec_Request obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Sec_Request obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.sec_request");
            sql.AppendLine("WHERE sec_request_id = :sec_request_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("sec_request_id", obj.Sec_Request_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Sec_Request DataRowToObject(DbDataReader row, string ColPrefix = "sr_")
        {
            Sec_Request RetVal = new();

            RetVal.Sec_Request_Id = (long)(decimal)(Util.GetRowVal(row, $"{ColPrefix}sec_request_id"));
            RetVal.Request_First_Name = (string)Util.GetRowVal(row, $"{ColPrefix}request_first_name");
            RetVal.Request_Last_Name = (string)Util.GetRowVal(row, $"{ColPrefix}request_last_name");
            RetVal.Request_Username = (string)Util.GetRowVal(row, $"{ColPrefix}request_username");
            RetVal.Request_Comments = (string)Util.GetRowVal(row, $"{ColPrefix}request_comments");
            RetVal.Created_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}created_date");
            RetVal.Additional_Info_Comments = (string)Util.GetRowVal(row, $"{ColPrefix}additional_info_comments");
            RetVal.Approval_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}approval_date");
            RetVal.Approval_Comments = (string)Util.GetRowVal(row, $"{ColPrefix}approval_comments");
            RetVal.Closed_Comments = (string)Util.GetRowVal(row, $"{ColPrefix}closed_comments");
            RetVal.Approved = ((string)Util.GetRowVal(row, $"{ColPrefix}approved")) == "Y";
            RetVal.Email_Flag = ((string)Util.GetRowVal(row, $"{ColPrefix}email_flag")) == "Y";

            RetVal.User = Sec_UserSvc.DataRowToObject(row, "suu_");
            RetVal.Current_Status = Sec_Request_StatusSvc.DataRowToObject(row, "srs_");
            RetVal.Manager = Sec_UserSvc.DataRowToObject(row, "sum_");
            RetVal.Sec_Request_Item = Sec_Request_ItemSvc.DataRowToObject(row,"sri_");

            if (!row.IsDBNull(row.GetOrdinal("sr_additional_info_user")))
            {
                RetVal.Additional_Info_User = Sec_UserSvc.DataRowToObject(row, "sui_");
            }

            if (!row.IsDBNull(row.GetOrdinal("sr_approval_by")))
            {
                RetVal.Approval_By = Sec_UserSvc.DataRowToObject(row, "sua_");
            }

            if (!row.IsDBNull(row.GetOrdinal("sr_closed_user")))
            {
                RetVal.Closed_User = Sec_UserSvc.DataRowToObject(row, "suc_");
            }

            return RetVal;
        }

    }
}
