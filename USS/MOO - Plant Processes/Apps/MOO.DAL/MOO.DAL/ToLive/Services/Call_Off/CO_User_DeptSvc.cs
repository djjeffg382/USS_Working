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
    public static class CO_User_DeptSvc
    {
        static CO_User_DeptSvc()
        {
            Util.RegisterOracle();
        }


        public static CO_User_Dept Get(CO_Dept Dept, Sec_Users SecUser)
        {
            return Get(Dept.Dept_Id, SecUser.User_Id);
        }

        public static CO_User_Dept Get(int DeptId, int SecUserId)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE cud.dept_id = :DeptId");
            sql.AppendLine("and cud.sec_user_id = :SecUserId");


            CO_User_Dept retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("DeptId", DeptId);
            cmd.Parameters.Add("SecUserId", SecUserId);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr,"cud_");
            }
            conn.Close();
            return retVal;
        }
        /// <summary>
        /// Gets the users in the specified department
        /// </summary>
        /// <param name="Dept"></param>
        /// <returns></returns>
        public static List<CO_User_Dept> GetUsersInDepartment(CO_Dept Dept)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE cud.dept_id = :Dept");

            List<CO_User_Dept> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("Dept", Dept.Dept_Id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    elements.Add(DataRowToObject(rdr,"cud_"));
                }
            }
            conn.Close();
            return elements;
        }

        /// <summary>
        /// Gets list of departments user has access to
        /// </summary>
        /// <param name="Dept"></param>
        /// <returns></returns>
        public static List<CO_User_Dept> GetUserDepartments(Sec_Users SecUsr)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE cud.sec_user_id = :userId");

            List<CO_User_Dept> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("userId", SecUsr.User_Id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    elements.Add(DataRowToObject(rdr, "cud_"));
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
            cols.AppendLine($"{ta}dept_id {ColPrefix}dept_id, {ta}sec_user_id {ColPrefix}sec_user_id, ");
            cols.AppendLine($"{ta}instant_email {ColPrefix}instant_email,{ta}freq_flyer_email {ColPrefix}freq_flyer_email, {ta}daily_email {ColPrefix}daily_email,");
            cols.AppendLine($"{ta}co_verified_email {ColPrefix}co_verified_email");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns("cud","cud_") + ",");
            sql.AppendLine(Sec_UserSvc.GetColumns("su", "su_") + ",");
            sql.AppendLine(CO_DeptSvc.GetColumns("cd", "cd_") );
            sql.AppendLine("FROM tolive.co_user_dept cud");
            sql.AppendLine("JOIN tolive.sec_users su ON cud.sec_user_id = su.user_id");
            sql.AppendLine("JOIN tolive.co_dept cd ON cud.dept_id = cd.dept_id");
            return sql.ToString();
        }


        public static int Insert(CO_User_Dept obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(CO_User_Dept obj, OracleConnection conn)
        {
           

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.CO_User_Dept(");
            sql.AppendLine("dept_id, sec_user_id, instant_email,freq_flyer_email, daily_email, co_verified_email)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":dept_id, :sec_user_id, :instant_email,:freq_flyer_email, :daily_email, :co_verified_email)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("dept_id", obj.Dept.Dept_Id);
            ins.Parameters.Add("sec_user_id", obj.Sec_User.User_Id);
            ins.Parameters.Add("instant_email", obj.Instant_Email ? 1 : 0);
            ins.Parameters.Add("freq_flyer_email", obj.Freq_Flyer_Email ? 1 : 0);
            ins.Parameters.Add("daily_email", obj.Daily_Email ? 1 : 0);
            ins.Parameters.Add("co_verified_email", obj.CO_Verified_Email ? 1 : 0);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(CO_User_Dept obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(CO_User_Dept obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.CO_User_Dept SET");
            sql.AppendLine("instant_email = :instant_email, ");
            sql.AppendLine("freq_flyer_email = :freq_flyer_email, ");
            sql.AppendLine("daily_email = :daily_email,");
            sql.AppendLine("co_verified_email = :co_verified_email");
            sql.AppendLine("WHERE dept_id = :dept_id");
            sql.AppendLine("AND sec_user_id = :sec_user_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("instant_email", obj.Instant_Email ? 1 : 0);
            upd.Parameters.Add("freq_flyer_email", obj.Freq_Flyer_Email ? 1 : 0);
            upd.Parameters.Add("daily_email", obj.Daily_Email ? 1 : 0);
            upd.Parameters.Add("co_verified_email", obj.CO_Verified_Email ? 1 : 0);
            upd.Parameters.Add("dept_id", obj.Dept.Dept_Id);
            upd.Parameters.Add("sec_user_id", obj.Sec_User.User_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(CO_User_Dept obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(CO_User_Dept obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.CO_User_Dept");
            sql.AppendLine("WHERE dept_id = :dept_id");
            sql.AppendLine("AND sec_user_id = :sec_user_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("dept_id", obj.Dept.Dept_Id);
            del.Parameters.Add("sec_user_id", obj.Sec_User.User_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static CO_User_Dept DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            CO_User_Dept RetVal = new();
            RetVal.Dept = CO_DeptSvc.DataRowToObject(row, "cd_");
            RetVal.Sec_User = Sec_UserSvc.DataRowToObject(row,"su_");
            RetVal.Instant_Email = (short)Util.GetRowVal(row, $"{ColPrefix}instant_email") == 1;
            RetVal.Freq_Flyer_Email = (short)Util.GetRowVal(row, $"{ColPrefix}freq_flyer_email") == 1;
            RetVal.Daily_Email = (short)Util.GetRowVal(row, $"{ColPrefix}daily_email") == 1;
            RetVal.CO_Verified_Email = (short)Util.GetRowVal(row, $"{ColPrefix}co_verified_email") == 1;
            return RetVal;
        }

    }
}
