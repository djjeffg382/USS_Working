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
    public static class Property_Removal_TrackedSvc
    {
        static Property_Removal_TrackedSvc()
        {
            Util.RegisterOracle();
        }


        public static Property_Removal_Tracked Get(int tracked_by)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE tracked_by = :tracked_by");


            Property_Removal_Tracked retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("tracked_by", tracked_by);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }


        public static List<Property_Removal_Tracked> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            List<Property_Removal_Tracked> elements = [];
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


        internal static string GetColumns(string TableAlias = "prt", string ColPrefix = "prt")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}tracked_by {ColPrefix}tracked_by, {ta}reason_id {ColPrefix}reason_id, ");
            cols.AppendLine($"{ta}plant {ColPrefix}plant");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns() + ",");
            sql.AppendLine(Property_Removal_ReasonsSvc.GetColumns("prr", "prr") + ",");
            sql.AppendLine(Sec_UserSvc.GetColumns("su", "su"));
            sql.AppendLine("FROM tolive.property_removal_tracked prt");
            sql.AppendLine("JOIN tolive.property_removal_reasons prr");
            sql.AppendLine("    ON prt.reason_id = prr.reason_id");
            sql.AppendLine("JOIN tolive.sec_users su");
            sql.AppendLine("    ON prt.tracked_by = su.user_id");
            return sql.ToString();
        }


        public static int Insert(Property_Removal_Tracked obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Property_Removal_Tracked obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.property_removal_tracked(");
            sql.AppendLine("tracked_by, reason_id, plant)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":tracked_by, :reason_id, :plant)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("tracked_by", obj.Tracked_By.User_Id);
            ins.Parameters.Add("reason_id", obj.Reason.Reason_Id);
            ins.Parameters.Add("plant", obj.Plant == MOO.Plant.Minntac ? 'M' : 'K');
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }

        public static int Update(Property_Removal_Tracked obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Property_Removal_Tracked obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.property_removal_tracked SET");
            sql.AppendLine("tracked_by = :tracked_by");
            sql.AppendLine("WHERE reason_id = :reason_id AND plant = :plant");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("reason_id", obj.Reason.Reason_Id);
            upd.Parameters.Add("plant", obj.Plant == MOO.Plant.Minntac ? 'M' : 'K');
            upd.Parameters.Add("tracked_by", obj.Tracked_By.User_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Property_Removal_Tracked obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Property_Removal_Tracked obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.property_removal_tracked");
            sql.AppendLine("WHERE tracked_by = :tracked_by AND reason_id = :reason_id AND plant = :plant");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("reason_id", obj.Reason.Reason_Id);
            del.Parameters.Add("plant", obj.Plant == MOO.Plant.Minntac ? 'M' : 'K');
            del.Parameters.Add("tracked_by", obj.Tracked_By.User_Id);

            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Property_Removal_Tracked DataRowToObject(DbDataReader row, string ColPrefix = "prt")
        {
            Property_Removal_Tracked RetVal = new();
            RetVal.Tracked_By = Sec_UserSvc.DataRowToObject(row,"su"); 
            RetVal.Reason = Property_Removal_ReasonsSvc.DataRowToObject(row,"prr");
            RetVal.Plant = (string)Util.GetRowVal(row, $"{ColPrefix}plant")=="M" ? MOO.Plant.Minntac : MOO.Plant.Keetac;
            return RetVal;
        }

    }
}
