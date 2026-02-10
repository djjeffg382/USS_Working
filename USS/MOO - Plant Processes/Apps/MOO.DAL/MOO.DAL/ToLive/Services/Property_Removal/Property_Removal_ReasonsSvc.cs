using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Services
{
    public static class Property_Removal_ReasonsSvc
    {
        public const string TABLE_NAME = "TOLIVE.Property_Removal_Reasons";
        static Property_Removal_ReasonsSvc()
        {
            Util.RegisterOracle();
        }


        public static Property_Removal_Reasons Get(int reasonId)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE reason_id = :reason_id");


            Property_Removal_Reasons retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("reason_id", reasonId);
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
        /// Gets the full list of Property Removal Reasons
        /// </summary>
        /// <param name="InculdeInactive">Whether to include inactive in the list</param>
        /// <returns></returns>
        public static List<Property_Removal_Reasons> GetAll(bool InculdeInactive = false)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            if(!InculdeInactive)
                sql.AppendLine("WHERE Active = 'True'");

            List<Property_Removal_Reasons> elements = [];
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
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
            cols.AppendLine($"{ta}reason {ColPrefix}reason, {ta}active {ColPrefix}active, {ta}reason_id {ColPrefix}reason_id");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.property_removal_reasons");
            return sql.ToString();
        }


        public static int Insert(Property_Removal_Reasons obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Property_Removal_Reasons obj, OracleConnection conn)
        {
            if (obj.Reason_Id <= 0)
            {
                string sqlId = "SELECT NVL(MAX(reason_id),0) FROM tolive.property_removal_reasons";
                int nextId = Convert.ToInt32((decimal)MOO.Data.ExecuteScalar(sqlId, Data.MNODatabase.DMART)) + 1;
                obj.Reason_Id = nextId;
            }

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.property_removal_reasons(");
            sql.AppendLine("reason, active, reason_id)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":reason, :active, :reason_id)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("reason", obj.Reason);
            ins.Parameters.Add("active", obj.Active.ToString());
            ins.Parameters.Add("reason_id", obj.Reason_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Property_Removal_Reasons obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Property_Removal_Reasons obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.property_removal_reasons SET");
            sql.AppendLine("active = :active, ");
            sql.AppendLine("reason = :reason");
            sql.AppendLine("WHERE reason_id = :reason_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("active", obj.Active.ToString());
            upd.Parameters.Add("reason", obj.Reason);
            upd.Parameters.Add("reason_id", obj.Reason_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Property_Removal_Reasons obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Property_Removal_Reasons obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.property_removal_reasons");
            sql.AppendLine("WHERE reason_id = :reason_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("reason_id", obj.Reason_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Property_Removal_Reasons DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Property_Removal_Reasons RetVal = new();
            RetVal.Reason = (string)Util.GetRowVal(row, $"{ColPrefix}reason");
            RetVal.Active = bool.Parse((string)Util.GetRowVal(row, $"{ColPrefix}active"));
            RetVal.Reason_Id = Convert.ToInt32((decimal)Util.GetRowVal(row, $"{ColPrefix}reason_id"));
            return RetVal;
        }

    }
}
