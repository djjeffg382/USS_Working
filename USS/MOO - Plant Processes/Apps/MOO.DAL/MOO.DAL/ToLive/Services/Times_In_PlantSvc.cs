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
    public static class Times_In_PlantSvc
    {
        static Times_In_PlantSvc()
        {
            Util.RegisterOracle();
        }


        public static Times_In_Plant Get(decimal in_plant_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE in_plant_id = :in_plant_id");


            Times_In_Plant retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("in_plant_id", in_plant_id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }


        public static List<Times_In_Plant> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE in_date BETWEEN :startDate AND :endDate");

            List<Times_In_Plant> elements = [];
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("startDate", startDate);
            cmd.Parameters.Add("endDate", endDate);
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
            cols.AppendLine($"{ta}in_plant_id {ColPrefix}in_plant_id, {ta}division {ColPrefix}division, ");
            cols.AppendLine($"{ta}department {ColPrefix}department, {ta}crew {ColPrefix}crew, ");
            cols.AppendLine($"{ta}pay_number {ColPrefix}pay_number, {ta}location {ColPrefix}location, ");
            cols.AppendLine($"{ta}employee_name {ColPrefix}employee_name, {ta}uss_id {ColPrefix}uss_id, ");
            cols.AppendLine($"{ta}in_date {ColPrefix}in_date, {ta}shift_date {ColPrefix}shift_date, {ta}shift {ColPrefix}shift");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.times_in_plant");
            return sql.ToString();
        }


        public static int Insert(Times_In_Plant obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Times_In_Plant obj, OracleConnection conn)
        {
            if (obj.In_Plant_Id <= 0)
                obj.In_Plant_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.SEQ_TIMES"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Times_In_Plant(");
            sql.AppendLine("in_plant_id, division, department, crew, pay_number, location, employee_name, uss_id, ");
            sql.AppendLine("in_date, shift_date, shift)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":in_plant_id, :division, :department, :crew, :pay_number, :location, :employee_name, ");
            sql.AppendLine(":uss_id, :in_date, :shift_date, :shift)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("in_plant_id", obj.In_Plant_Id);
            ins.Parameters.Add("division", obj.Division);
            ins.Parameters.Add("department", obj.Department);
            ins.Parameters.Add("crew", obj.Crew);
            ins.Parameters.Add("pay_number", obj.Pay_Number);
            ins.Parameters.Add("location", obj.Location);
            ins.Parameters.Add("employee_name", obj.Employee_Name);
            ins.Parameters.Add("uss_id", obj.Uss_Id);
            ins.Parameters.Add("in_date", obj.In_Date);
            ins.Parameters.Add("shift_date", obj.Shift_Date);
            ins.Parameters.Add("shift", obj.Shift);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Times_In_Plant obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Times_In_Plant obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Times_In_Plant SET");
            sql.AppendLine("division = :division, ");
            sql.AppendLine("department = :department, ");
            sql.AppendLine("crew = :crew, ");
            sql.AppendLine("pay_number = :pay_number, ");
            sql.AppendLine("location = :location, ");
            sql.AppendLine("employee_name = :employee_name, ");
            sql.AppendLine("uss_id = :uss_id, ");
            sql.AppendLine("in_date = :in_date, ");
            sql.AppendLine("shift_date = :shift_date, ");
            sql.AppendLine("shift = :shift");
            sql.AppendLine("WHERE in_plant_id = :in_plant_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("division", obj.Division);
            upd.Parameters.Add("department", obj.Department);
            upd.Parameters.Add("crew", obj.Crew);
            upd.Parameters.Add("pay_number", obj.Pay_Number);
            upd.Parameters.Add("location", obj.Location);
            upd.Parameters.Add("employee_name", obj.Employee_Name);
            upd.Parameters.Add("uss_id", obj.Uss_Id);
            upd.Parameters.Add("in_date", obj.In_Date);
            upd.Parameters.Add("shift_date", obj.Shift_Date);
            upd.Parameters.Add("shift", obj.Shift);
            upd.Parameters.Add("in_plant_id", obj.In_Plant_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Times_In_Plant obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Times_In_Plant obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.Times_In_Plant");
            sql.AppendLine("WHERE in_plant_id = :in_plant_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("in_plant_id", obj.In_Plant_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Times_In_Plant DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Times_In_Plant RetVal = new();
            RetVal.In_Plant_Id = Convert.ToInt32((decimal)Util.GetRowVal(row, $"{ColPrefix}in_plant_id"));
            RetVal.Division = (string)Util.GetRowVal(row, $"{ColPrefix}division");
            RetVal.Department = (string)Util.GetRowVal(row, $"{ColPrefix}department");
            RetVal.Crew = (string)Util.GetRowVal(row, $"{ColPrefix}crew");
            RetVal.Pay_Number = (string)Util.GetRowVal(row, $"{ColPrefix}pay_number");
            RetVal.Location = (string)Util.GetRowVal(row, $"{ColPrefix}location");
            RetVal.Employee_Name = (string)Util.GetRowVal(row, $"{ColPrefix}employee_name");
            RetVal.Uss_Id = (string)Util.GetRowVal(row, $"{ColPrefix}uss_id");
            RetVal.In_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}in_date");
            RetVal.Shift_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}shift_date");
            RetVal.Shift = Convert.ToInt16((decimal)Util.GetRowVal(row, $"{ColPrefix}shift"));
            return RetVal;
        }

    }
}
