using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Services
{
    public class TIMES_AbsenteeismSvc
    {
        static TIMES_AbsenteeismSvc()
        {
            Util.RegisterOracle();
        }


        public static TIMES_Absenteeism Get(decimal absent_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE absent_id = :absent_id");


            TIMES_Absenteeism retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("absent_id", absent_id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }


        public static TIMES_Absenteeism Get(DateTime absentDate, short shiftNbr, string payNumber)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE absent_date = :absentDate");
            sql.AppendLine("AND shift = :shiftNbr");
            sql.AppendLine("AND pay_Number = :payNumber");


            TIMES_Absenteeism retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("absentDate", absentDate);
            cmd.Parameters.Add("shiftNbr", shiftNbr);
            cmd.Parameters.Add("payNumber", payNumber);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }


        public static List<TIMES_Absenteeism> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            Console.WriteLine(sql.ToString());
            List<TIMES_Absenteeism> elements = new();
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

        public static List<TIMES_Absenteeism> GetAllByDate(DateTime startDate, DateTime endDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE ABSENT_DATE BETWEEN :startDate AND :endDate");

            List<TIMES_Absenteeism> elements = new();

            using (OracleConnection conn = new OracleConnection(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART)))
            {
                conn.Open();

                using (OracleCommand cmd = new OracleCommand(sql.ToString(), conn))
                {
                    cmd.Parameters.Add("startDate", OracleDbType.Date).Value = startDate;
                    cmd.Parameters.Add("endDate", OracleDbType.Date).Value = endDate;

                    using (OracleDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            elements.Add(DataRowToObject(rdr));
                        }
                    }
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
            cols.AppendLine($"{ta}absent_id {ColPrefix}absent_id, {ta}division {ColPrefix}division, ");
            cols.AppendLine($"{ta}department {ColPrefix}department, {ta}crew {ColPrefix}crew, ");
            cols.AppendLine($"{ta}absent_date {ColPrefix}absent_date, {ta}pay_number {ColPrefix}pay_number, ");
            cols.AppendLine($"{ta}employee_name {ColPrefix}employee_name, {ta}position {ColPrefix}position, ");
            cols.AppendLine($"{ta}shift {ColPrefix}shift, {ta}reason_code {ColPrefix}reason_code, ");
            cols.AppendLine($"{ta}reason_desc {ColPrefix}reason_desc, {ta}hours {ColPrefix}hours, ");
            cols.AppendLine($"{ta}imported_date {ColPrefix}imported_date, {ta}comments {ColPrefix}comments, ");
            cols.AppendLine($"{ta}location {ColPrefix}location, {ta}person_id {ColPrefix}person_id");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns("ta")+",");
            sql.AppendLine(MOO.DAL.ToLive.Services.PeopleSvc.GetColumns("ppl", "ppl_") + ",");
            sql.AppendLine(MOO.DAL.ToLive.Services.PeopleSvc.GetColumns("mgr", "mgr_"));

            sql.AppendLine("FROM tolive.times_absenteeism ta");

            //sql.AppendLine("LEFT OUTER JOIN tolive.people ppl");
            //sql.AppendLine("ON p.supervisor_person_id = supervisor.person_id");
            sql.AppendLine("INNER JOIN tolive.people ppl");
            sql.AppendLine("ON CAST(ta.pay_number as NUMBER) = NVL(CAST(ppl.employee_number AS NUMBER), ppl.person_id)");
            sql.AppendLine("LEFT JOIN tolive.people mgr");
            sql.AppendLine("ON ppl.supervisor_person_id = mgr.person_id");
            return sql.ToString();
        }


        public static int Insert(TIMES_Absenteeism obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(TIMES_Absenteeism obj, OracleConnection conn)
        {
            if (obj.Absent_Id <= 0)
                obj.Absent_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.SEQ_TIMES"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.times_absenteeism(");
            sql.AppendLine("absent_id, division, department, crew, absent_date, pay_number, employee_name, ");
            sql.AppendLine("position, shift, reason_code, reason_desc, hours, imported_date, comments, location, ");
            sql.AppendLine("person_id)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":absent_id, :division, :department, :crew, :absent_date, :pay_number, :employee_name, ");
            sql.AppendLine(":position, :shift, :reason_code, :reason_desc, :hours, :imported_date, :comments, ");
            sql.AppendLine(":location, :person_id)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("absent_id", obj.Absent_Id);
            ins.Parameters.Add("division", obj.Division);
            ins.Parameters.Add("department", obj.Department);
            ins.Parameters.Add("crew", obj.Crew);
            ins.Parameters.Add("absent_date", obj.Absent_Date);
            ins.Parameters.Add("pay_number", obj.Pay_Number);
            ins.Parameters.Add("employee_name", obj.Employee_Name);
            ins.Parameters.Add("position", obj.Position);
            ins.Parameters.Add("shift", obj.Shift);
            ins.Parameters.Add("reason_code", obj.Reason_Code);
            ins.Parameters.Add("reason_desc", obj.Reason_Desc);
            ins.Parameters.Add("hours", obj.Hours);
            ins.Parameters.Add("imported_date", obj.Imported_Date);
            ins.Parameters.Add("comments", obj.Comments);
            ins.Parameters.Add("location", obj.Location);
            ins.Parameters.Add("person_id", obj.Person.Person_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(TIMES_Absenteeism obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(TIMES_Absenteeism obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.times_absenteeism SET");
            sql.AppendLine("division = :division, ");
            sql.AppendLine("department = :department, ");
            sql.AppendLine("crew = :crew, ");
            sql.AppendLine("absent_date = :absent_date, ");
            sql.AppendLine("pay_number = :pay_number, ");
            sql.AppendLine("employee_name = :employee_name, ");
            sql.AppendLine("position = :position, ");
            sql.AppendLine("shift = :shift, ");
            sql.AppendLine("reason_code = :reason_code, ");
            sql.AppendLine("reason_desc = :reason_desc, ");
            sql.AppendLine("hours = :hours, ");
            sql.AppendLine("imported_date = :imported_date, ");
            sql.AppendLine("comments = :comments, ");
            sql.AppendLine("location = :location, ");
            sql.AppendLine("person_id = :person_id");
            sql.AppendLine("WHERE absent_id = :absent_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("division", obj.Division);
            upd.Parameters.Add("department", obj.Department);
            upd.Parameters.Add("crew", obj.Crew);
            upd.Parameters.Add("absent_date", obj.Absent_Date);
            upd.Parameters.Add("pay_number", obj.Pay_Number);
            upd.Parameters.Add("employee_name", obj.Employee_Name);
            upd.Parameters.Add("position", obj.Position);
            upd.Parameters.Add("shift", obj.Shift);
            upd.Parameters.Add("reason_code", obj.Reason_Code);
            upd.Parameters.Add("reason_desc", obj.Reason_Desc);
            upd.Parameters.Add("hours", obj.Hours);
            upd.Parameters.Add("imported_date", obj.Imported_Date);
            upd.Parameters.Add("comments", obj.Comments);
            upd.Parameters.Add("location", obj.Location);
            upd.Parameters.Add("person_id", obj.Person.Person_Id);
            upd.Parameters.Add("absent_id", obj.Absent_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(TIMES_Absenteeism obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(TIMES_Absenteeism obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.times_absenteeism");
            sql.AppendLine("WHERE absent_id = :absent_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("absent_id", obj.Absent_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static TIMES_Absenteeism DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            TIMES_Absenteeism RetVal = new();
            RetVal.Absent_Id = (decimal)Util.GetRowVal(row, $"{ColPrefix}absent_id");
            RetVal.Division = (string)Util.GetRowVal(row, $"{ColPrefix}division");
            RetVal.Department = (string)Util.GetRowVal(row, $"{ColPrefix}department");
            RetVal.Crew = (string)Util.GetRowVal(row, $"{ColPrefix}crew");
            RetVal.Absent_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}absent_date");
            RetVal.Pay_Number = (string)Util.GetRowVal(row, $"{ColPrefix}pay_number");
            RetVal.Employee_Name = (string)Util.GetRowVal(row, $"{ColPrefix}employee_name");
            RetVal.Position = (string)Util.GetRowVal(row, $"{ColPrefix}position");
            RetVal.Shift = (decimal)Util.GetRowVal(row, $"{ColPrefix}shift");
            RetVal.Reason_Code = (string)Util.GetRowVal(row, $"{ColPrefix}reason_code");
            RetVal.Reason_Desc = (string)Util.GetRowVal(row, $"{ColPrefix}reason_desc");
            RetVal.Hours = (decimal)Util.GetRowVal(row, $"{ColPrefix}hours");
            RetVal.Imported_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}imported_date");
            RetVal.Comments = (string)Util.GetRowVal(row, $"{ColPrefix}comments");
            RetVal.Location = (string)Util.GetRowVal(row, $"{ColPrefix}location");
            RetVal.Person = PeopleSvc.DataRowToObject(row,"ppl_");
            if (row.IsDBNull(row.GetOrdinal("mgr_person_id")))
                RetVal.Supervisor = null;
            else
                RetVal.Supervisor = PeopleSvc.DataRowToObject(row,"mgr_");
            return RetVal;
        }
    }
    }
