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
    public static class CO_DeptSvc
    {
        static CO_DeptSvc()
        {
            Util.RegisterOracle();
        }


        public static CO_Dept Get(int dept_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE cd.dept_id = :dept_id");


            CO_Dept retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("dept_id", dept_id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr, "cd_");
            }
            conn.Close();
            return retVal;
        }


        public static List<CO_Dept> GetAll(MOO.Plant Plant)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE cd.Plant = :Plant");

            List<CO_Dept> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("Plant", Plant.ToString());
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    elements.Add(DataRowToObject(rdr, "cd_"));
                }
            }
            conn.Close();
            return elements;
        }


        public static List<CO_Dept> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            List<CO_Dept> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    elements.Add(DataRowToObject(rdr, "cd_"));
                }
            }
            conn.Close();
            return elements;
        }



        /// <summary>
        /// Gets the departments of a specific user
        /// </summary>
        /// <param name="SecUser"></param>
        /// <returns></returns>
        public static List<CO_Dept> GetUserDepartments(Sec_Users SecUser)
        {
            return GetUserDepartments(SecUser.User_Id);
        }

        /// <summary>
        /// Gets the departments of a specific user
        /// </summary>
        /// <param name="SecUserId"></param>
        /// <returns></returns>
        public static List<CO_Dept> GetUserDepartments(int SecUserId)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("JOIN tolive.co_user_dept cud ON cd.dept_id = cud.dept_id");
            sql.AppendLine("WHERE cud.sec_user_id = :SecUserId");

            List<CO_Dept> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("SecUserId", SecUserId);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    elements.Add(DataRowToObject(rdr, "cd_"));
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
            cols.AppendLine($"{ta}dept_id {ColPrefix}dept_id, {ta}dept_name {ColPrefix}dept_name, {ta}plant {ColPrefix}plant, ");
            cols.AppendLine($"{ta}enabled {ColPrefix}enabled");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns("cd", "cd_"));
            sql.AppendLine("FROM tolive.co_dept cd");
            return sql.ToString();
        }


        public static int Insert(CO_Dept obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(CO_Dept obj, OracleConnection conn)
        {
            if (obj.Dept_Id <= 0)
                obj.Dept_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.seq_call_off"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.CO_Dept(");
            sql.AppendLine("dept_id, dept_name, plant, enabled)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":dept_id, :dept_name, :plant, :enabled)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("dept_id", obj.Dept_Id);
            ins.Parameters.Add("dept_name", obj.Dept_Name);
            ins.Parameters.Add("plant", obj.Plant.ToString());
            ins.Parameters.Add("enabled", obj.Enabled ? 1 : 0);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(CO_Dept obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(CO_Dept obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.CO_Dept SET");
            sql.AppendLine("dept_name = :dept_name, ");
            sql.AppendLine("plant = :plant, ");
            sql.AppendLine("enabled = :enabled");
            sql.AppendLine("WHERE dept_id = :dept_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("dept_name", obj.Dept_Name);
            upd.Parameters.Add("plant", obj.Plant.ToString());
            upd.Parameters.Add("enabled", obj.Enabled ? 1 : 0);
            upd.Parameters.Add("dept_id", obj.Dept_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(CO_Dept obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(CO_Dept obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.CO_Dept");
            sql.AppendLine("WHERE dept_id = :dept_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("dept_id", obj.Dept_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static CO_Dept DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            CO_Dept RetVal = new();
            RetVal.Dept_Id = Convert.ToInt32(Util.GetRowVal(row, $"{ColPrefix}dept_id"));
            RetVal.Dept_Name = (string)Util.GetRowVal(row, $"{ColPrefix}dept_name");
            RetVal.Plant = Enum.Parse<MOO.Plant>( (string)Util.GetRowVal(row, $"{ColPrefix}plant"));
            RetVal.Enabled = (short)Util.GetRowVal(row, $"{ColPrefix}enabled") == 1;
            return RetVal;
        }

    }
}
