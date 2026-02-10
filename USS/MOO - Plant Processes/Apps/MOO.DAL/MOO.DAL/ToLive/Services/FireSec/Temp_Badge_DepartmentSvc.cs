using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Services
{
    public static class Temp_Badge_DepartmentSvc
    {
        static Temp_Badge_DepartmentSvc()
        {
            Util.RegisterOracle();
        }


        public static Temp_Badge_Department Get(long department_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE department_id = :department_id");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("department_id", department_id);
            da.SelectCommand.BindByName = true;

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }

        public static List<Temp_Badge_Department> GetAll(MOO.Plant plantLoc)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            //We have to convert plant to 1 and 2. In the table is set to 1 for minntac and 2 for keetac
            var plantNumber = 1;

            //We only need to convert if the plant is keetac.
            if (plantLoc == MOO.Plant.Keetac)
                plantNumber = 2;

            sql.AppendLine($"WHERE plantloc = :plantVal");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("plantVal", plantNumber);

            da.SelectCommand.BindByName = true;
            DataSet ds = MOO.Data.ExecuteQuery(da);
            List<Temp_Badge_Department> elements = new();
            if (ds.Tables[0].Rows.Count > 1)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    elements.Add(DataRowToObject(dr));
                }
            }
            return elements;
        }


        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}department_id {ColPrefix}department_id, {ta}department {ColPrefix}department, ");
            cols.AppendLine($"{ta}email_address {ColPrefix}email_address, {ta}plantloc {ColPrefix}plantloc ");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.Temp_Badge_Department");
            return sql.ToString();
        }


        public static int Insert(Temp_Badge_Department obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Temp_Badge_Department obj, OracleConnection conn)
        {
            
            decimal maxDptId = (decimal)( MOO.Data.ExecuteScalar("SELECT MAX(department_id) FROM tolive.Temp_Badge_Department",Data.MNODatabase.DMART));
            obj.Department_Id = (int)maxDptId + 1;

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Temp_Badge_Department(");
            sql.AppendLine("department_id, department, email_address, plantloc)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":department_id, :department, :email_address, :plantloc)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("department_id", obj.Department_Id);
            ins.Parameters.Add("department", obj.Department);
            ins.Parameters.Add("email_address", obj.Email_Address);
            ins.Parameters.Add("plantloc", obj.Plantloc == MOO.Plant.Minntac ? 1 : 2);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Temp_Badge_Department obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Temp_Badge_Department obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Temp_Badge_Department SET");
            sql.AppendLine("department = :department, ");
            sql.AppendLine("email_address = :email_address, ");
            sql.AppendLine("plantloc = :plantloc");
            sql.AppendLine("WHERE department_id = :department_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("department", obj.Department);
            upd.Parameters.Add("email_address", obj.Email_Address);
            upd.Parameters.Add("plantloc", obj.Plantloc == MOO.Plant.Minntac ? 1 : 2);
            upd.Parameters.Add("department_id", obj.Department_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Temp_Badge_Department obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Temp_Badge_Department obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.Temp_Badge_Department");
            sql.AppendLine("WHERE department_id = :department_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("department_id", obj.Department_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Temp_Badge_Department DataRowToObject(DataRow row, string ColPrefix = "")
        {
            Temp_Badge_Department RetVal = new();
            RetVal.Department_Id = (long)row.Field<decimal>($"{ColPrefix}department_id");
            RetVal.Department = row.Field<string>($"{ColPrefix}department");
            RetVal.Email_Address = row.Field<string>($"{ColPrefix}email_address");
            RetVal.Plantloc = row.Field<short>($"{ColPrefix}plantloc") == 1 ? MOO.Plant.Minntac : MOO.Plant.Keetac;
            return RetVal;
        }

    }
}
