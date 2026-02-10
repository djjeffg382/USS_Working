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
    public static class Property_Removal_AuthorizedSvc
    {
        static Property_Removal_AuthorizedSvc()
        {
            Util.RegisterOracle();
        }

        public static Property_Removal_Authorized Get(int authorized_by)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE authorized_by = :authorized_by");

            Property_Removal_Authorized retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("authorized_by", authorized_by);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }

        public static List<Property_Removal_Authorized> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            List<Property_Removal_Authorized> elements = [];
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
            cols.AppendLine($"{ta}authorized_by {ColPrefix}authorized_by, {ta}active {ColPrefix}active");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.property_removal_authorized");
            return sql.ToString();
        }

        public static int Insert(Property_Removal_Authorized obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Insert(Property_Removal_Authorized obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Property_Removal_Authorized(");
            sql.AppendLine("authorized_by, active)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":authorized_by, :active)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("authorized_by", obj.Authorized_By.User_Id);
            ins.Parameters.Add("active", "True");
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }
        public static int Delete(Property_Removal_Authorized obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Property_Removal_Authorized obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.Property_Removal_Authorized");
            sql.AppendLine("WHERE authorized_by = :authorized_by");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("authorized_by", obj.Authorized_By.User_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Property_Removal_Authorized DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Property_Removal_Authorized RetVal = new();
            int authorizedBy = Int32.Parse((string)(Util.GetRowVal(row, $"{ColPrefix}authorized_by")));
            RetVal.Authorized_By = Sec_UserSvc.Get(authorizedBy);
            return RetVal;
        }
    }
}
