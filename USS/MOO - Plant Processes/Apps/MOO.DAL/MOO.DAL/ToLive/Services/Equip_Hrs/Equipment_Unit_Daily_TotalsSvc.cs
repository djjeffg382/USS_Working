using MOO.DAL.ToLive.Enums;
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
    public static class Equipment_Unit_Daily_TotalsSvc
    {
        static Equipment_Unit_Daily_TotalsSvc()
        {
            Util.RegisterOracle();
        }

        public static List<Equipment_Unit_Daily_Totals> GetByEquipDateRange(string equipId, EqUnitTypes unit_Type, DateTime startDate, DateTime endDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE equip_id = :equip_id");
            sql.AppendLine("AND unit_type = :unit_type");
            sql.AppendLine("AND unit_date BETWEEN :startDate and :endDate");

            List<Equipment_Unit_Daily_Totals> elements = [];
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("equip_id", equipId);
            cmd.Parameters.Add("unit_type", unit_Type);
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
            cols.AppendLine($"{ta}equip_id {ColPrefix}equip_id, {ta}unit_type {ColPrefix}unit_type, ");
            cols.AppendLine($"{ta}unit_date {ColPrefix}unit_date, ROUND({ta}nbr_of_units,4) {ColPrefix}nbr_of_units, ");
            cols.AppendLine($"{ta}proc_ind {ColPrefix}proc_ind");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns() + ",");
            sql.AppendLine(Equipment_MasterSvc.GetColumns("em", "em_") + ",");
            sql.AppendLine(Equipment_Unit_TypesSvc.GetColumns("et", "et_"));
            sql.AppendLine("FROM tolive.equipment_unit_daily_totals");
            sql.AppendLine("JOIN tolive.equipment_master em ON em.equip_id = eua.equip_id");
            sql.AppendLine("JOIN tolive.equipment_unit_types et ON eua.unit_type = et.unit_type");
            return sql.ToString();
        }


        public static int Insert(Equipment_Unit_Daily_Totals obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Equipment_Unit_Daily_Totals obj, OracleConnection conn)
        {
            
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Equipment_Unit_Daily_Totals(");
            sql.AppendLine("equip_id, unit_type, unit_date, nbr_of_units, proc_ind)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":equip_id, :unit_type, :unit_date, :nbr_of_units, :proc_ind)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("equip_id", obj.Equip.Equip_Id);
            ins.Parameters.Add("unit_type", obj.UnitType.Unit_Type);
            ins.Parameters.Add("unit_date", obj.Unit_Date.Date);
            ins.Parameters.Add("nbr_of_units", Math.Round(obj.Nbr_Of_Units.GetValueOrDefault(0),4,MidpointRounding.AwayFromZero));
            ins.Parameters.Add("proc_ind", "T");  //This is no longer used, we will default it to "T" though,  see comments on field in DB
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Equipment_Unit_Daily_Totals obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Equipment_Unit_Daily_Totals obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Equipment_Unit_Daily_Totals SET");
            sql.AppendLine("nbr_of_units = :nbr_of_units ");
            sql.AppendLine("WHERE equip_id = :equip_id");
            sql.AppendLine("AND unit_type = :unit_type ");
            sql.AppendLine("AND unit_date = :unit_date ");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("nbr_of_units", Math.Round(obj.Nbr_Of_Units.GetValueOrDefault(0), 4, MidpointRounding.AwayFromZero));
            upd.Parameters.Add("equip_id", obj.Equip.Equip_Id);
            upd.Parameters.Add("unit_type", obj.UnitType.Unit_Type);
            upd.Parameters.Add("unit_date", obj.Unit_Date.Date);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Equipment_Unit_Daily_Totals obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Equipment_Unit_Daily_Totals obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.Equipment_Unit_Daily_Totals");
            sql.AppendLine("WHERE equip_id = :equip_id");
            sql.AppendLine("AND unit_type = :unit_type ");
            sql.AppendLine("AND unit_date = :unit_date ");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("equip_id", obj.Equip.Equip_Id);
            del.Parameters.Add("unit_type", obj.UnitType.Unit_Type);
            del.Parameters.Add("unit_date", obj.Unit_Date.Date);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Equipment_Unit_Daily_Totals DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Equipment_Unit_Daily_Totals RetVal = new();
            RetVal.Unit_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}unit_date");
            RetVal.Nbr_Of_Units = (decimal?)Util.GetRowVal(row, $"{ColPrefix}nbr_of_units");


            RetVal.Equip = Equipment_MasterSvc.DataRowToObject(row, "em_");

            RetVal.UnitType = Equipment_Unit_TypesSvc.DataRowToObject(row, "et_");
            return RetVal;
        }

    }
}
