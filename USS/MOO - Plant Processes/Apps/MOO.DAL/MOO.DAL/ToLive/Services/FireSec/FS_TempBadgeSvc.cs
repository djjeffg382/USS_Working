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
    public class FS_TempBadgeSvc
    {
        static FS_TempBadgeSvc()
        {
            Util.RegisterOracle();
        }


        public static FS_TempBadge Get(long temp_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE tb.temp_id = :temp_id");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("temp_id", temp_id);
            da.SelectCommand.BindByName = true;

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }

        public static List<FS_TempBadge> GetAll(bool onlyActive = true, MOO.Plant? plant = null)
        {
            //plant values do not match Moo.Plant
            int? plantVal = null;

            if (plant != null)
            {
                if (plant == MOO.Plant.Keetac)
                    plantVal = 2;
                else if (plant == MOO.Plant.Minntac)
                    plantVal = 1;
            }


            StringBuilder sql = new();
            sql.Append(GetSelect());
            if (onlyActive)
                sql.AppendLine($"WHERE tb.temp_badge_return is NULL");

            if (plant != null && onlyActive)
                sql.AppendLine($"AND tb.plantloc = :plant");
            else if (plant != null && onlyActive == false)
                sql.AppendLine($"WHERE tb.Plantloc = :plant");


            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("plant", plantVal.ToString());
            DataSet ds = new();
            try
            {
                ds = MOO.Data.ExecuteQuery(da);
            }
            catch (Exception e)
            {
                var x = e.Message;
            }

            List<FS_TempBadge> elements = new();
            if (ds.Tables[0].Rows.Count >= 1)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    elements.Add(DataRowToObject(dr));
                }
            }
            return elements;
        }


        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns("tb") + ",");
            sql.AppendLine(PeopleSvc.GetColumns("e", "e_") + ",");
            sql.AppendLine(PeopleSvc.GetColumns("m", "m_") + ",");
            sql.AppendLine(Temp_Badge_DepartmentSvc.GetColumns("tbd", "tbd_"));

            sql.AppendLine("FROM tolive.temp_badge tb");
            sql.AppendLine("LEFT JOIN tolive.people e");
            sql.AppendLine("ON e.person_id = tb.employee_id");
            sql.AppendLine("LEFT JOIN tolive.people m");
            sql.AppendLine("ON m.person_id = tb.manager_id");
            sql.AppendLine("LEFT JOIN tolive.temp_badge_department tbd");
            sql.AppendLine("ON tb.department = tbd.department AND tb.plantloc = tbd.PLANTLOC");

            return sql.ToString();
        }




        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}temp_id {ColPrefix}temp_id, {ta}employee_id {ColPrefix}employee_id, ");
            cols.AppendLine($"{ta}manager_id {ColPrefix}manager_id, {ta}validbadge {ColPrefix}validbadge, ");
            cols.AppendLine($"{ta}c_name {ColPrefix}c_name, {ta}c_supername {ColPrefix}c_supername, ");
            cols.AppendLine($"{ta}c_companyname {ColPrefix}c_companyname, {ta}plantloc {ColPrefix}plantloc, ");
            cols.AppendLine($"{ta}msha_expdate {ColPrefix}msha_expdate, {ta}temp_badgenbr {ColPrefix}temp_badgenbr, ");
            cols.AppendLine($"{ta}temp_badge_issued {ColPrefix}temp_badge_issued, {ta}temp_badge_return {ColPrefix}temp_badge_return, ");
            cols.AppendLine($"{ta}sec_officer {ColPrefix}sec_officer, {ta}sec_officerreturn {ColPrefix}sec_officerreturn, ");
            cols.AppendLine($"{ta}reasonfor {ColPrefix}reasonfor, {ta}is_emp_conc {ColPrefix}is_emp_conc, ");
            cols.AppendLine($"{ta}filename {ColPrefix}filename, {ta}department {ColPrefix}department ");
            return cols.ToString();
        }


        public static int Insert(FS_TempBadge obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(FS_TempBadge obj, OracleConnection conn)
        {
            if (obj.Temp_Id <= 0)
                obj.Temp_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.seq_firesec"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.temp_badge(");
            sql.AppendLine("temp_id, employee_id, manager_id, validbadge, c_name, c_supername, c_companyname, ");
            sql.AppendLine("plantloc, msha_expdate, temp_badgenbr, temp_badge_issued, temp_badge_return, ");
            sql.AppendLine("sec_officer, sec_officerreturn, reasonfor, is_emp_conc, filename, ");
            sql.AppendLine("department)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":temp_id, :employee_id, :manager_id, :validbadge, :c_name, :c_supername, ");
            sql.AppendLine(":c_companyname, :plantloc, :msha_expdate, :temp_badgenbr, :temp_badge_issued, ");
            sql.AppendLine(":temp_badge_return, :sec_officer, :sec_officerreturn, :reasonfor, :is_emp_conc, ");
            sql.AppendLine(":filename, :department)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;

            //plant values do not match Moo.Plant
            int? plantVal = null;
            if (obj.Plantloc == MOO.Plant.Keetac)
                plantVal = 2;
            else if (obj.Plantloc == MOO.Plant.Minntac)
                plantVal = 1;

            ins.Parameters.Add("temp_id", obj.Temp_Id);

            if (obj.Department.Department == "Contractor")
            {
                //If contractor set to null value. Otherwise you get zero as value in DB
                ins.Parameters.Add("employee_id", null);
                ins.Parameters.Add("manager_id", null);
            }
            else
            {
                ins.Parameters.Add("employee_id", obj.Employee_Id.Person_Id);
                ins.Parameters.Add("manager_id", obj.Manager_Id.Person_Id);
            }

            ins.Parameters.Add("validbadge", obj.Validbadge);
            ins.Parameters.Add("c_name", obj.C_Name);
            ins.Parameters.Add("c_supername", obj.C_Supername);
            ins.Parameters.Add("c_companyname", obj.C_Companyname);
            ins.Parameters.Add("plantloc", plantVal);
            ins.Parameters.Add("msha_expdate", obj.Msha_Expdate);
            ins.Parameters.Add("temp_badgenbr", obj.Temp_Badgenbr);
            ins.Parameters.Add("temp_badge_issued", obj.Temp_Badge_Issued);
            ins.Parameters.Add("temp_badge_return", obj.Temp_Badge_Return);
            ins.Parameters.Add("sec_officer", obj.Sec_Officer);
            ins.Parameters.Add("sec_officerreturn", obj.Sec_Officerreturn);
            ins.Parameters.Add("reasonfor", obj.Reasonfor);
            ins.Parameters.Add("is_emp_conc", obj.Is_Emp_Conc);
            ins.Parameters.Add("filename", obj.Filename);
            ins.Parameters.Add("department", obj.Department.Department);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(FS_TempBadge obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(FS_TempBadge obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.temp_badge SET");
            sql.AppendLine("employee_id = :employee_id, ");
            sql.AppendLine("manager_id = :manager_id, ");
            sql.AppendLine("validbadge = :validbadge, ");
            sql.AppendLine("c_name = :c_name, ");
            sql.AppendLine("c_supername = :c_supername, ");
            sql.AppendLine("c_companyname = :c_companyname, ");
            sql.AppendLine("plantloc = :plantloc, ");
            sql.AppendLine("msha_expdate = :msha_expdate, ");
            sql.AppendLine("temp_badgenbr = :temp_badgenbr, ");
            sql.AppendLine("temp_badge_issued = :temp_badge_issued, ");
            sql.AppendLine("temp_badge_return = :temp_badge_return, ");
            sql.AppendLine("sec_officer = :sec_officer, ");
            sql.AppendLine("sec_officerreturn = :sec_officerreturn, ");
            sql.AppendLine("reasonfor = :reasonfor, ");
            sql.AppendLine("is_emp_conc = :is_emp_conc, ");
            sql.AppendLine("filename = :filename, ");
            sql.AppendLine("department = :department");
            sql.AppendLine("WHERE temp_id = :temp_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;

            //plant values do not match Moo.Plant
            int? plantVal = null;
            if (obj.Plantloc == MOO.Plant.Keetac)
                plantVal = 2;
            else if (obj.Plantloc == MOO.Plant.Minntac)
                plantVal = 1;

            if (obj.Department.Department == "Contractor")
            {
                //If contractor set to null value. Otherwise you get zero as value in DB
                upd.Parameters.Add("employee_id", null);
                upd.Parameters.Add("manager_id", null);
            }
            else
            {
                upd.Parameters.Add("employee_id", obj.Employee_Id.Person_Id);
                upd.Parameters.Add("manager_id", obj.Manager_Id.Person_Id);
            }

            upd.Parameters.Add("validbadge", obj.Validbadge);
            upd.Parameters.Add("c_name", obj.C_Name);
            upd.Parameters.Add("c_supername", obj.C_Supername);
            upd.Parameters.Add("c_companyname", obj.C_Companyname);
            upd.Parameters.Add("plantloc", plantVal);
            upd.Parameters.Add("msha_expdate", obj.Msha_Expdate);
            upd.Parameters.Add("temp_badgenbr", obj.Temp_Badgenbr);
            upd.Parameters.Add("temp_badge_issued", obj.Temp_Badge_Issued);
            upd.Parameters.Add("temp_badge_return", obj.Temp_Badge_Return);
            upd.Parameters.Add("sec_officer", obj.Sec_Officer);
            upd.Parameters.Add("sec_officerreturn", obj.Sec_Officerreturn);
            upd.Parameters.Add("reasonfor", obj.Reasonfor);
            upd.Parameters.Add("is_emp_conc", obj.Is_Emp_Conc);
            upd.Parameters.Add("filename", obj.Filename);
            upd.Parameters.Add("department", obj.Department.Department);
            upd.Parameters.Add("temp_id", obj.Temp_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(FS_TempBadge obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(FS_TempBadge obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.temp_badge");
            sql.AppendLine("WHERE temp_id = :temp_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("temp_id", obj.Temp_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        private static FS_TempBadge DataRowToObject(DataRow row)
        {
            FS_TempBadge RetVal = new();

            RetVal.Temp_Id = (long)row.Field<decimal>("temp_id");

            if (row.Field<decimal?>("Employee_Id") != null && row.Field<decimal?>("Employee_Id") != 0)
                RetVal.Employee_Id = PeopleSvc.DataRowToObject(row, "e_");
            else
                RetVal.Employee_Id = null;

            if (row.Field<decimal?>("Manager_Id") != null && row.Field<decimal?>("Manager_Id") != 0)
                RetVal.Manager_Id = PeopleSvc.DataRowToObject(row, "m_");
            else
                RetVal.Manager_Id = null;

            RetVal.Validbadge = row.Field<string>("validbadge");
            RetVal.C_Name = row.Field<string>("c_name");
            RetVal.C_Supername = row.Field<string>("c_supername");
            RetVal.C_Companyname = row.Field<string>("c_companyname");

            if (row.Field<string>("plantloc") == "1")
                RetVal.Plantloc = MOO.Plant.Minntac;
            else if (row.Field<string>("plantloc") == "2")
                RetVal.Plantloc = MOO.Plant.Keetac;

            RetVal.Msha_Expdate = row.Field<DateTime>("msha_expdate");
            RetVal.Temp_Badgenbr = (long)row.Field<decimal?>("temp_badgenbr");
            RetVal.Temp_Badge_Issued = row.Field<DateTime>("temp_badge_issued");
            RetVal.Temp_Badge_Return = row.Field<DateTime?>("temp_badge_return");
            RetVal.Sec_Officer = row.Field<string>("sec_officer");
            RetVal.Sec_Officerreturn = row.Field<string>("sec_officerreturn");
            RetVal.Reasonfor = row.Field<string>("reasonfor");
            RetVal.Is_Emp_Conc = row.Field<string>("is_emp_conc");
            RetVal.Filename = row.Field<string>("filename");
            //if we don't have a department id, then this is just a name like "Contractor"
            if (!row.IsNull("tbd_department_id"))
            {
                RetVal.Department = Temp_Badge_DepartmentSvc.DataRowToObject(row, "tbd_");
            }
            else
                RetVal.Department = null;

            return RetVal;
        }

    }
}
