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
    public class NodeScan_TblSvc
    {
        static NodeScan_TblSvc()
        {
            Util.RegisterOracle();
        }


        public static NodeScan_Tbl Get(string nodename)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE nodename = {nodename}");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }

        public static List<NodeScan_Tbl> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());


            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);

            List<NodeScan_Tbl> retVal = new();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                retVal.Add(DataRowToObject(dr));
            }

            return retVal;
        }



        /// <summary>
        /// Sets the active value field for the specified Nodename
        /// </summary>
        /// <param name="Node">The Node active</param>
        /// <param name="ActiveValue">The new Active Value</param>
        public static void SetActive(NodeScan_Tbl Node, bool ActiveValue)
        {
            SetActive(Node.NodeName, ActiveValue);
        }

        /// <summary>
        /// Sets the active value field for the specified Nodename
        /// </summary>
        /// <param name="NodeName">The nodename to set active</param>
        /// <param name="ActiveValue">The new Active Value</param>
        public static void SetActive(string NodeName, bool ActiveValue)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE TOLIVE.Nodescan_tbl SET");
            sql.AppendLine("isactive = :isactive");
            sql.AppendLine("WHERE nodename = :nodename");

            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                OracleCommand upd = new(sql.ToString(), conn);
                upd.Parameters.Add("isactive", ActiveValue ? "Y" : "N");
                upd.Parameters.Add("nodename", NodeName);
                int recsAffected = MOO.Data.ExecuteNonQuery(upd);
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
        /// Sets the active value field for the specified Nodename
        /// </summary>
        /// <param name="Node">The node to change</param>
        /// <param name="SchedDown">The new SchedDown Value</param>
        public static void SetScheduledDown(NodeScan_Tbl Node, bool SchedDown)
        {
            SetScheduledDown(Node.NodeName, SchedDown);
        }

        /// <summary>
        /// Sets the active value field for the specified Nodename
        /// </summary>
        /// <param name="NodeName">The nodename to change</param>
        /// <param name="SchedDown">The new SchedDown Value</param>
        public static void SetScheduledDown(string NodeName, bool SchedDown)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE TOLIVE.Nodescan_tbl SET");
            sql.AppendLine("isscheduleddown = :isscheduleddown");
            sql.AppendLine("WHERE nodename = :nodename");

            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                OracleCommand upd = new(sql.ToString(), conn);
                upd.Parameters.Add("isscheduleddown", SchedDown ? "Y" : "N");
                upd.Parameters.Add("nodename", NodeName);
                int recsAffected = MOO.Data.ExecuteNonQuery(upd);
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

        private static string GetSelect(string addField = "")
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine("nodename, isactive, datetime, description, isscheduleddown, email_list");
            if (!string.IsNullOrEmpty(addField))
            {
                sql.Append(", ");
                sql.AppendLine(addField);
            }
            sql.AppendLine("FROM tolive.nodescan_tbl");
            return sql.ToString();
        }


        private static NodeScan_Tbl DataRowToObject(DataRow row)
        {
            NodeScan_Tbl RetVal = new();
            RetVal.NodeName = row.Field<string>("nodename");
            RetVal.IsActive = row.Field<string>("isactive") == "Y";
            RetVal.Datetime = row.Field<DateTime>("datetime");
            RetVal.Description = row.Field<string>("description");
            RetVal.IsScheduledDown = row.Field<string>("isscheduleddown") == "Y";
            RetVal.Email_List = row.Field<string>("email_list");
            return RetVal;
        }

    }
}
