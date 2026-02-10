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
    public class FS_WorkOrdersSvc
    {
        static FS_WorkOrdersSvc()
        {
            Util.RegisterOracle();
        }


        public static FS_WorkOrders Get(decimal workorders_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE workorders_id = :workorders_id");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("workorders_id", workorders_id);

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }


        public static List<FS_WorkOrders> GetAll(bool onlyActive = true, MOO.Plant? plant = null)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            if (onlyActive)
                sql.AppendLine($"WHERE Date_completed IS NULL");
            if (plant != null && onlyActive)
                sql.AppendLine($"AND plant = '{plant.ToString().Substring(0,1)}'");
            else if(plant != null && onlyActive == false)
                sql.AppendLine($"WHERE Plant = '{plant.ToString().Substring(0, 1)}'");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));

            DataSet ds = MOO.Data.ExecuteQuery(da);
            List<FS_WorkOrders> elements = new();
            if (ds.Tables[0].Rows.Count >= 1)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                     elements.Add(DataRowToObject(dr));
                }
            }
            return elements;
        }


        private static string GetSelect(string addField = "")
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine("workorders_id, workorder_nbr, date_open, dispatcher, requester, phone_nbr, location, ");
            sql.AppendLine("service_request, fire_chief_ntfy, firefighter_notified, time_notified, event_type, ");
            sql.AppendLine("comments, work_completed_by, date_completed, security_notified, plant, audited, ");
            sql.AppendLine("audited_by");
            if (!string.IsNullOrEmpty(addField))
            {
                sql.Append(", ");
                sql.AppendLine(addField);
            }
            sql.AppendLine("FROM tolive.fs_workorders");
            return sql.ToString();
        }


        public static int Insert(FS_WorkOrders obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(FS_WorkOrders obj, OracleConnection conn)
        {
            if (obj.Workorders_Id <= 0)
                obj.Workorders_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.seq_firesec"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.fs_workorders(");
            sql.AppendLine("workorders_id, workorder_nbr, date_open, dispatcher, requester, phone_nbr, location, ");
            sql.AppendLine("service_request, fire_chief_ntfy, firefighter_notified, time_notified, event_type, ");
            sql.AppendLine("comments, work_completed_by, date_completed, security_notified, plant, audited, ");
            sql.AppendLine("audited_by)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":workorders_id, :workorder_nbr, :date_open, :dispatcher, :requester, :phone_nbr, ");
            sql.AppendLine(":location, :service_request, :fire_chief_ntfy, :firefighter_notified, :time_notified, ");
            sql.AppendLine(":event_type, :comments, :work_completed_by, :date_completed, :security_notified, ");
            sql.AppendLine(":plant, :audited, :audited_by)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("workorders_id", obj.Workorders_Id);
            ins.Parameters.Add("workorder_nbr", obj.Workorder_Nbr);
            ins.Parameters.Add("date_open", obj.Date_Open);
            ins.Parameters.Add("dispatcher", obj.Dispatcher);
            ins.Parameters.Add("requester", obj.Requester);
            ins.Parameters.Add("phone_nbr", obj.Phone_Nbr);
            ins.Parameters.Add("location", obj.Location);
            ins.Parameters.Add("service_request", obj.Service_Request);
            ins.Parameters.Add("fire_chief_ntfy", obj.Fire_Chief_Ntfy.HasValue ? (obj.Fire_Chief_Ntfy.GetValueOrDefault(true) ? 1 : 0) : DBNull.Value);
            ins.Parameters.Add("firefighter_notified", obj.Firefighter_Notified);
            ins.Parameters.Add("time_notified", obj.Time_Notified);
            ins.Parameters.Add("event_type", obj.Event_Type);
            ins.Parameters.Add("comments", obj.Comments);
            ins.Parameters.Add("work_completed_by", obj.Work_Completed_By);
            ins.Parameters.Add("date_completed", obj.Date_Completed);
            ins.Parameters.Add("security_notified", obj.Security_Notified);
            ins.Parameters.Add("plant", obj.Plant.ToString()[..1]);
            ins.Parameters.Add("audited", obj.Audited.HasValue ? (obj.Audited.GetValueOrDefault(true) ? 1 : 0) : DBNull.Value);
            ins.Parameters.Add("audited_by", obj.Audited_By);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(FS_WorkOrders obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(FS_WorkOrders obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.fs_workorders SET");
            sql.AppendLine("workorder_nbr = :workorder_nbr, ");
            sql.AppendLine("date_open = :date_open, ");
            sql.AppendLine("dispatcher = :dispatcher, ");
            sql.AppendLine("requester = :requester, ");
            sql.AppendLine("phone_nbr = :phone_nbr, ");
            sql.AppendLine("location = :location, ");
            sql.AppendLine("service_request = :service_request, ");
            sql.AppendLine("fire_chief_ntfy = :fire_chief_ntfy, ");
            sql.AppendLine("firefighter_notified = :firefighter_notified, ");
            sql.AppendLine("time_notified = :time_notified, ");
            sql.AppendLine("event_type = :event_type, ");
            sql.AppendLine("comments = :comments, ");
            sql.AppendLine("work_completed_by = :work_completed_by, ");
            sql.AppendLine("date_completed = :date_completed, ");
            sql.AppendLine("security_notified = :security_notified, ");
            sql.AppendLine("plant = :plant, ");
            sql.AppendLine("audited = :audited, ");
            sql.AppendLine("audited_by = :audited_by");
            sql.AppendLine("WHERE workorders_id = :workorders_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.Parameters.Add("workorder_nbr", obj.Workorder_Nbr);
            upd.Parameters.Add("date_open", obj.Date_Open);
            upd.Parameters.Add("dispatcher", obj.Dispatcher);
            upd.Parameters.Add("requester", obj.Requester);
            upd.Parameters.Add("phone_nbr", obj.Phone_Nbr);
            upd.Parameters.Add("location", obj.Location);
            upd.Parameters.Add("service_request", obj.Service_Request);
            upd.Parameters.Add("fire_chief_ntfy", obj.Fire_Chief_Ntfy.HasValue ? (obj.Fire_Chief_Ntfy.GetValueOrDefault(true) ? 1 : 0) : DBNull.Value);
            upd.Parameters.Add("firefighter_notified", obj.Firefighter_Notified);
            upd.Parameters.Add("time_notified", obj.Time_Notified);
            upd.Parameters.Add("event_type", obj.Event_Type);
            upd.Parameters.Add("comments", obj.Comments);
            upd.Parameters.Add("work_completed_by", obj.Work_Completed_By);
            upd.Parameters.Add("date_completed", obj.Date_Completed);
            upd.Parameters.Add("security_notified", obj.Security_Notified);
            upd.Parameters.Add("plant", obj.Plant.ToString()[..1]);
            upd.Parameters.Add("audited", obj.Audited.HasValue ? (obj.Audited.GetValueOrDefault(true) ? 1 : 0) : DBNull.Value);
            upd.Parameters.Add("audited_by", obj.Audited_By);
            upd.Parameters.Add("workorders_id", obj.Workorders_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(FS_WorkOrders obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(FS_WorkOrders obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.fs_workorders");
            sql.AppendLine("WHERE workorders_id = :workorders_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.Parameters.Add("workorders_id", obj.Workorders_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        private static FS_WorkOrders DataRowToObject(DataRow row)
        {
            FS_WorkOrders RetVal = new();
            RetVal.Workorders_Id = row.Field<decimal>("workorders_id");
            RetVal.Workorder_Nbr = row.Field<decimal>("workorder_nbr");
            RetVal.Date_Open = row.Field<DateTime>("date_open");
            RetVal.Dispatcher = row.Field<string>("dispatcher");
            RetVal.Requester = row.Field<string>("requester");
            RetVal.Phone_Nbr = row.Field<string>("phone_nbr");
            RetVal.Location = row.Field<string>("location");
            RetVal.Service_Request = row.Field<string>("service_request");
            RetVal.Fire_Chief_Ntfy = Convert.ToBoolean(row.Field<decimal>("Fire_Chief_Ntfy") == 1);
            RetVal.Firefighter_Notified = row.Field<string>("firefighter_notified");
            RetVal.Time_Notified = row.Field<DateTime?>("time_notified");
            RetVal.Event_Type = row.Field<string>("event_type");
            RetVal.Comments = row.Field<string>("comments");
            RetVal.Work_Completed_By = row.Field<string>("work_completed_by");
            RetVal.Date_Completed = row.Field<DateTime?>("date_completed");
            RetVal.Security_Notified = row.Field<string>("security_notified");
            if (row["plant"].ToString().ToLower() == "m")
                RetVal.Plant = MOO.Plant.Minntac;
            else
                RetVal.Plant = MOO.Plant.Keetac;

            RetVal.Audited = Convert.ToBoolean(row.Field<decimal>("Audited") == 1);
            RetVal.Audited_By = row.Field<string>("audited_by");
            return RetVal;
        }

    }
}
