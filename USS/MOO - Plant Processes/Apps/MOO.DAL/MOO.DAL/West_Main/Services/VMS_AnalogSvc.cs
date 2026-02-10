using MOO.DAL.West_Main.Enums;
using MOO.DAL.West_Main.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.West_Main.Services
{
    public static class VMS_AnalogSvc
    {
        static VMS_AnalogSvc()
        {
            Util.RegisterOracle();
        }


        public static VMS_Analog Get(WestMainPlants Plant, short Point_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE plant = :plant");
            sql.AppendLine($"AND Point_Id = :Point_Id");


            VMS_Analog retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("plant", Plant.ToString());
            cmd.Parameters.Add("Point_Id", Point_Id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }


        public static List<VMS_Analog> GetAll()
        {
            StringBuilder sql = new();
            sql.AppendLine(GetSelect());
            sql.AppendLine("ORDER BY point_id");

            List<VMS_Analog> elements = new();
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

        /// <summary>
        /// Retrieves a list of all analog points that are dependent on this point
        /// </summary>
        /// <returns></returns>
        public static List<VMS_Analog> GetDependencies(VMS_Analog AnaPoint)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE plant = :plant");
            sql.AppendLine("AND equation_type <> 1");
            sql.AppendLine($"AND (pnt1 = {AnaPoint.Point_Id}");
            sql.AppendLine($"OR pnt2 = {AnaPoint.Point_Id}");
            sql.AppendLine($"OR pnt3 = {AnaPoint.Point_Id}");
            sql.AppendLine($"OR pnt4 = {AnaPoint.Point_Id}");
            sql.AppendLine($"OR pnt5 = {AnaPoint.Point_Id}");
            sql.AppendLine($"OR pnt6 = {AnaPoint.Point_Id})");

            List<VMS_Analog> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn); ;
            cmd.Parameters.Add("plant", AnaPoint.Plant.ToString());
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

        /// <summary>
        /// Retrieves a list of all analog points that are dependent on this point
        /// </summary>
        /// <returns></returns>
        public static List<VMS_Analog> GetDependencies(VMS_Digital DigPoint)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE plant = :plant");
            sql.AppendLine($"AND (related_contact = {DigPoint.Point_Id}");
            sql.AppendLine("OR (equation_type = 1");
            sql.AppendLine($"AND (pnt1 = {DigPoint.Point_Id}");
            sql.AppendLine($"OR pnt2 = {DigPoint.Point_Id}");
            sql.AppendLine($"OR pnt3 = {DigPoint.Point_Id}");
            sql.AppendLine($"OR pnt4 = {DigPoint.Point_Id}");
            sql.AppendLine($"OR pnt5 = {DigPoint.Point_Id}");
            sql.AppendLine($"OR pnt6 = {DigPoint.Point_Id})))");

            List<VMS_Analog> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn); ;
            cmd.Parameters.Add("plant", DigPoint.Plant.ToString());
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    elements.Add(VMS_AnalogSvc.DataRowToObject(rdr));
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
            cols.AppendLine($"{ta}plant {ColPrefix}plant, {ta}point_id {ColPrefix}point_id, {ta}dmart_id {ColPrefix}dmart_id, ");
            cols.AppendLine($"{ta}point_name {ColPrefix}point_name, {ta}description {ColPrefix}description, ");
            cols.AppendLine($"{ta}eng_units {ColPrefix}eng_units, {ta}related_contact {ColPrefix}related_contact, ");
            cols.AppendLine($"{ta}point_type {ColPrefix}point_type, {ta}equation_type {ColPrefix}equation_type, ");
            cols.AppendLine($"{ta}scan_rate {ColPrefix}scan_rate, {ta}low_limit {ColPrefix}low_limit, ");
            cols.AppendLine($"{ta}high_limit {ColPrefix}high_limit, {ta}pnt1 {ColPrefix}pnt1, {ta}pnt2 {ColPrefix}pnt2, ");
            cols.AppendLine($"{ta}pnt3 {ColPrefix}pnt3, {ta}pnt4 {ColPrefix}pnt4, {ta}pnt5 {ColPrefix}pnt5, ");
            cols.AppendLine($"{ta}pnt6 {ColPrefix}pnt6, {ta}ic1 {ColPrefix}ic1, {ta}ic2 {ColPrefix}ic2, {ta}ic3 {ColPrefix}ic3, ");
            cols.AppendLine($"{ta}ic4 {ColPrefix}ic4, {ta}constant {ColPrefix}constant, {ta}last_update {ColPrefix}last_update");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM west_main.vms_analog");
            return sql.ToString();
        }


        public static int Insert(VMS_Analog obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(VMS_Analog obj, OracleConnection conn)
        {

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO west_main.VMS_Analog(");
            sql.AppendLine("plant, point_id, dmart_id, point_name, description, eng_units, related_contact, ");
            sql.AppendLine("point_type, equation_type, scan_rate, low_limit, high_limit, pnt1, pnt2, pnt3, pnt4, ");
            sql.AppendLine("pnt5, pnt6, ic1, ic2, ic3, ic4, constant)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":plant, :point_id, :dmart_id, :point_name, :description, :eng_units, :related_contact, ");
            sql.AppendLine(":point_type, :equation_type, :scan_rate, :low_limit, :high_limit, :pnt1, :pnt2, :pnt3, ");
            sql.AppendLine(":pnt4, :pnt5, :pnt6, :ic1, :ic2, :ic3, :ic4, :constant)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("plant", obj.Plant.ToString());
            ins.Parameters.Add("point_id", obj.Point_Id);
            ins.Parameters.Add("dmart_id", obj.Dmart_Id);
            ins.Parameters.Add("point_name", obj.Point_Name);
            ins.Parameters.Add("description", obj.Description);
            ins.Parameters.Add("eng_units", obj.Eng_Units);
            ins.Parameters.Add("related_contact", obj.Related_Contact);
            ins.Parameters.Add("point_type", (int)obj.Point_Type);
            ins.Parameters.Add("equation_type", (int)obj.Equation_Type);
            ins.Parameters.Add("scan_rate", obj.Scan_Rate);
            ins.Parameters.Add("low_limit", obj.Low_Limit);
            ins.Parameters.Add("high_limit", obj.High_Limit);
            ins.Parameters.Add("pnt1", obj.Pnt1);
            ins.Parameters.Add("pnt2", obj.Pnt2);
            ins.Parameters.Add("pnt3", obj.Pnt3);
            ins.Parameters.Add("pnt4", obj.Pnt4);
            ins.Parameters.Add("pnt5", obj.Pnt5);
            ins.Parameters.Add("pnt6", obj.Pnt6);
            ins.Parameters.Add("ic1", obj.Ic1);
            ins.Parameters.Add("ic2", obj.Ic2);
            ins.Parameters.Add("ic3", obj.Ic3);
            ins.Parameters.Add("ic4", obj.Ic4);
            ins.Parameters.Add("constant", obj.Constant);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(VMS_Analog obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(VMS_Analog obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE west_main.VMS_Analog SET");
            sql.AppendLine("dmart_id = :dmart_id, ");
            sql.AppendLine("point_name = :point_name, ");
            sql.AppendLine("description = :description, ");
            sql.AppendLine("eng_units = :eng_units, ");
            sql.AppendLine("related_contact = :related_contact, ");
            sql.AppendLine("point_type = :point_type, ");
            sql.AppendLine("equation_type = :equation_type, ");
            sql.AppendLine("scan_rate = :scan_rate, ");
            sql.AppendLine("low_limit = :low_limit, ");
            sql.AppendLine("high_limit = :high_limit, ");
            sql.AppendLine("pnt1 = :pnt1, ");
            sql.AppendLine("pnt2 = :pnt2, ");
            sql.AppendLine("pnt3 = :pnt3, ");
            sql.AppendLine("pnt4 = :pnt4, ");
            sql.AppendLine("pnt5 = :pnt5, ");
            sql.AppendLine("pnt6 = :pnt6, ");
            sql.AppendLine("ic1 = :ic1, ");
            sql.AppendLine("ic2 = :ic2, ");
            sql.AppendLine("ic3 = :ic3, ");
            sql.AppendLine("ic4 = :ic4, ");
            sql.AppendLine("constant = :constant,");
            sql.AppendLine("last_update = :last_update");
            sql.AppendLine("WHERE plant = :plant");
            sql.AppendLine("AND point_id = :point_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("dmart_id", obj.Dmart_Id);
            upd.Parameters.Add("point_name", obj.Point_Name);
            upd.Parameters.Add("description", obj.Description);
            upd.Parameters.Add("eng_units", obj.Eng_Units);
            upd.Parameters.Add("related_contact", obj.Related_Contact);
            upd.Parameters.Add("point_type", (int)obj.Point_Type);
            upd.Parameters.Add("equation_type", (int)obj.Equation_Type);
            upd.Parameters.Add("scan_rate", obj.Scan_Rate);
            upd.Parameters.Add("low_limit", obj.Low_Limit);
            upd.Parameters.Add("high_limit", obj.High_Limit);
            upd.Parameters.Add("pnt1", obj.Pnt1);
            upd.Parameters.Add("pnt2", obj.Pnt2);
            upd.Parameters.Add("pnt3", obj.Pnt3);
            upd.Parameters.Add("pnt4", obj.Pnt4);
            upd.Parameters.Add("pnt5", obj.Pnt5);
            upd.Parameters.Add("pnt6", obj.Pnt6);
            upd.Parameters.Add("ic1", obj.Ic1);
            upd.Parameters.Add("ic2", obj.Ic2);
            upd.Parameters.Add("ic3", obj.Ic3);
            upd.Parameters.Add("ic4", obj.Ic4);
            upd.Parameters.Add("constant", obj.Constant);
            upd.Parameters.Add("last_update", DateTime.Now);

            upd.Parameters.Add("plant", obj.Plant.ToString());
            upd.Parameters.Add("point_id", obj.Point_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(VMS_Analog obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(VMS_Analog obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM west_main.VMS_Analog");
            sql.AppendLine("WHERE plant = :plant");
            sql.AppendLine("AND point_id = :point_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("plant", obj.Plant.ToString());
            del.Parameters.Add("point_id", obj.Point_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        /// <summary>
        /// Deletes all the points for a given plant
        /// </summary>
        /// <param name="Plant"></param>
        /// <returns></returns>
        public static int DeletePlantRecords(WestMainPlants Plant)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = DeletePlantRecords(Plant, conn);
            conn.Close();
            return recsAffected;
        }

        /// <summary>
        /// Deletes all the points for a given plant
        /// </summary>
        /// <param name="Plant"></param>
        /// <returns></returns>
        public static int DeletePlantRecords(WestMainPlants Plant, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM west_main.VMS_Analog");
            sql.AppendLine("WHERE plant = :plant");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("plant", Plant.ToString());
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static VMS_Analog DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            VMS_Analog RetVal = new();
            RetVal.Plant = Enum.Parse<WestMainPlants>((string)Util.GetRowVal(row, $"{ColPrefix}plant"));
            RetVal.Point_Id = (short)Util.GetRowVal(row, $"{ColPrefix}point_id");
            RetVal.Dmart_Id = (decimal?)Util.GetRowVal(row, $"{ColPrefix}dmart_id");
            RetVal.Point_Name = (string)Util.GetRowVal(row, $"{ColPrefix}point_name");
            RetVal.Description = (string)Util.GetRowVal(row, $"{ColPrefix}description");
            RetVal.Eng_Units = (string)Util.GetRowVal(row, $"{ColPrefix}eng_units");
            RetVal.Related_Contact = (short?)Util.GetRowVal(row, $"{ColPrefix}related_contact");
            RetVal.Point_Type = (VMSAnaPointType)(short)Util.GetRowVal(row, $"{ColPrefix}point_type");
            RetVal.Equation_Type = (VMSAnaEqType)(short)Util.GetRowVal(row, $"{ColPrefix}equation_type");
            RetVal.Scan_Rate = (short?)Util.GetRowVal(row, $"{ColPrefix}scan_rate");
            RetVal.Low_Limit = (double?)Util.GetRowVal(row, $"{ColPrefix}low_limit");
            RetVal.High_Limit = (double?)Util.GetRowVal(row, $"{ColPrefix}high_limit");
            RetVal.Pnt1 = (short?)Util.GetRowVal(row, $"{ColPrefix}pnt1");
            RetVal.Pnt2 = (short?)Util.GetRowVal(row, $"{ColPrefix}pnt2");
            RetVal.Pnt3 = (short?)Util.GetRowVal(row, $"{ColPrefix}pnt3");
            RetVal.Pnt4 = (short?)Util.GetRowVal(row, $"{ColPrefix}pnt4");
            RetVal.Pnt5 = (short?)Util.GetRowVal(row, $"{ColPrefix}pnt5");
            RetVal.Pnt6 = (short?)Util.GetRowVal(row, $"{ColPrefix}pnt6");
            RetVal.Ic1 = (short?)Util.GetRowVal(row, $"{ColPrefix}ic1");
            RetVal.Ic2 = (short?)Util.GetRowVal(row, $"{ColPrefix}ic2");
            RetVal.Ic3 = (short?)Util.GetRowVal(row, $"{ColPrefix}ic3");
            RetVal.Ic4 = (short?)Util.GetRowVal(row, $"{ColPrefix}ic4");
            RetVal.Constant = (double?)Util.GetRowVal(row, $"{ColPrefix}constant");
            RetVal.Last_Update = (DateTime)Util.GetRowVal(row, $"{ColPrefix}last_update");
            return RetVal;
        }

    }
}
