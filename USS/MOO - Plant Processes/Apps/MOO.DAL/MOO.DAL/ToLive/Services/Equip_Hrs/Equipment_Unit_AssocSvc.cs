using MOO.DAL.ToLive.Enums;
using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Services
{
    public static class Equipment_Unit_AssocSvc
    {
        static Equipment_Unit_AssocSvc()
        {
            Util.RegisterOracle();
        }


        public static Equipment_Unit_Assoc Get(string Equip_Id, int Unit_Type)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE equip_id = :equip_id");
            sql.AppendLine("AND unit_type = :unit_type");


            Equipment_Unit_Assoc retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("equip_id", Equip_Id);
            cmd.Parameters.Add("unit_type", Unit_Type);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr,"eua_");
            }
            conn.Close();
            return retVal;
        }


        public static List<Equipment_Unit_Assoc> GetByEquipment(string Equip_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE equip_id = :equip_id");

            List<Equipment_Unit_Assoc> elements = [];
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("equip_id", Equip_Id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    elements.Add(DataRowToObject(rdr, "eua_"));
                }
            }
            conn.Close();
            return elements;
        }

        public static List<Equipment_Unit_Assoc> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            List<Equipment_Unit_Assoc> elements = [];
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    elements.Add(DataRowToObject(rdr,"eua_"));
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
            cols.AppendLine($"{ta}equip_id {ColPrefix}equip_id, {ta}unit_type {ColPrefix}unit_type, {ta}source {ColPrefix}source, ");
            cols.AppendLine($"{ta}source_id {ColPrefix}source_id, {ta}on_state_value {ColPrefix}on_state_value, ");
            cols.AppendLine($"{ta}multiplier {ColPrefix}multiplier, {ta}pi_sql {ColPrefix}pi_sql");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns("eua","eua_") + ",");
            sql.AppendLine(Equipment_MasterSvc.GetColumns("em", "em_") + ",");
            sql.AppendLine(Equipment_Unit_TypesSvc.GetColumns("et", "et_"));
            sql.AppendLine("FROM tolive.equipment_unit_assoc eua");
            sql.AppendLine("JOIN tolive.equipment_master em ON em.equip_id = eua.equip_id");
            sql.AppendLine("JOIN tolive.equipment_unit_types et ON eua.unit_type = et.unit_type");
            return sql.ToString();
        }


        public static int Insert(Equipment_Unit_Assoc obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Equipment_Unit_Assoc obj, OracleConnection conn)
        {
           

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Equipment_Unit_Assoc(");
            sql.AppendLine("equip_id, unit_type, source, source_id, on_state_value, multiplier, pi_sql)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":equip_id, :unit_type, :source, :source_id, :on_state_value, :multiplier, :pi_sql)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("equip_id", obj.Equip.Equip_Id);
            ins.Parameters.Add("unit_type", obj.UnitType.Unit_Type);
            ins.Parameters.Add("source", SourceToDBString(obj.Source));
            ins.Parameters.Add("source_id", obj.Source_Id);
            ins.Parameters.Add("on_state_value", obj.On_State_Value);
            ins.Parameters.Add("multiplier", obj.Multiplier);
            ins.Parameters.Add("pi_sql", obj.Pi_Sql);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }

        /// <summary>
        /// Converts the eunum of equipment unit assoc to the string needed to be put in the database
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        internal static string SourceToDBString(EqUnitAssocSource src)
        {
            switch (src)
            {
                case EqUnitAssocSource.MinVu_MPA:
                    return "MinVu MPA";
                case EqUnitAssocSource.Meter_Hours_Insert:
                    return "Meter Hours Insert";
                case EqUnitAssocSource.Data_Entry:
                    return "Data Entry/Modular";
                default:
                    return src.ToString();
            }
        }

        public static int Update(Equipment_Unit_Assoc obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Equipment_Unit_Assoc obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Equipment_Unit_Assoc SET");
            sql.AppendLine("unit_type = :unit_type, ");
            sql.AppendLine("source = :source, ");
            sql.AppendLine("source_id = :source_id, ");
            sql.AppendLine("on_state_value = :on_state_value, ");
            sql.AppendLine("multiplier = :multiplier, ");
            sql.AppendLine("pi_sql = :pi_sql");
            sql.AppendLine("WHERE equip_id = :equip_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("unit_type", obj.UnitType.Unit_Type);
            upd.Parameters.Add("source", SourceToDBString(obj.Source));
            upd.Parameters.Add("source_id", obj.Source_Id);
            upd.Parameters.Add("on_state_value", obj.On_State_Value);
            upd.Parameters.Add("multiplier", obj.Multiplier);
            upd.Parameters.Add("pi_sql", obj.Pi_Sql);
            upd.Parameters.Add("equip_id", obj.Equip.Equip_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }



        internal static Equipment_Unit_Assoc DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Equipment_Unit_Assoc RetVal = new();

            string src = (string)Util.GetRowVal(row, $"{ColPrefix}source");
            switch (src)
            {
                case "MinVu MPA":
                    RetVal.Source = EqUnitAssocSource.MinVu_MPA;
                    break;
                case "Meter Hours Insert":
                    RetVal.Source = EqUnitAssocSource.Meter_Hours_Insert;
                    break;
                case "Data Entry/Modular":
                    RetVal.Source = EqUnitAssocSource.Data_Entry;
                    break;
                default:
                    RetVal.Source = Enum.Parse<EqUnitAssocSource>(src);
                    break;
            }
            RetVal.Source_Id = (string)Util.GetRowVal(row, $"{ColPrefix}source_id");
            RetVal.On_State_Value = (string)Util.GetRowVal(row, $"{ColPrefix}on_state_value");
            RetVal.Multiplier = (decimal?)Util.GetRowVal(row, $"{ColPrefix}multiplier");
            RetVal.Pi_Sql = (string)Util.GetRowVal(row, $"{ColPrefix}pi_sql");


            RetVal.Equip = Equipment_MasterSvc.DataRowToObject(row, "em_");

            RetVal.UnitType = Equipment_Unit_TypesSvc.DataRowToObject(row, "et_");
            return RetVal;
        }

    }
}
