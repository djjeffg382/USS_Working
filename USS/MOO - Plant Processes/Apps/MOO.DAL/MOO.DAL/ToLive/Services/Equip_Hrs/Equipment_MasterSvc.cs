using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Services
{
    public class Equipment_MasterSvc
    {
        static Equipment_MasterSvc()
        {
            Util.RegisterOracle();
        }


        public static Equipment_Master Get(string Equip_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE equip_id = :equip_id");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("equip_id", Equip_Id);
            da.SelectCommand.BindByName = true;


            Equipment_Master retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("equip_id", Equip_Id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }

        public static List<Equipment_Master> GetAll(string Plant = "Minntac", string Area = "", string Equip_Group = "", string Equip_Desc = "")
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE plant = :plant");

            if (!string.IsNullOrEmpty(Area))
                sql.AppendLine("AND area = :Area");

            if (!string.IsNullOrEmpty(Equip_Group))
                sql.AppendLine("AND Equip_Group = :Equip_Group");

            if (!string.IsNullOrEmpty(Equip_Desc))
                sql.AppendLine("AND Equip_Desc = :Equip_Desc");

            sql.AppendLine("ORDER BY equip_id");

            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("Plant", Plant);

            if (!string.IsNullOrEmpty(Area))
                cmd.Parameters.Add("Area", Area);

            if (!string.IsNullOrEmpty(Equip_Group))
                cmd.Parameters.Add("Equip_Group", Equip_Group);

            if (!string.IsNullOrEmpty(Equip_Desc))
                cmd.Parameters.Add("Equip_Desc", Equip_Desc);

            cmd.BindByName = true;

            List<Equipment_Master> elements = [];
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
            cols.AppendLine($"{ta}equip_id {ColPrefix}equip_id, {ta}equip_desc {ColPrefix}equip_desc, {ta}plant {ColPrefix}plant, ");
            cols.AppendLine($"{ta}area {ColPrefix}area, {ta}equip_group {ColPrefix}equip_group, ");
            cols.AppendLine($"{ta}equip_type {ColPrefix}equip_type, {ta}source {ColPrefix}source, ");
            cols.AppendLine($"{ta}source_id {ColPrefix}source_id, {ta}passport_ind {ColPrefix}passport_ind, ");
            cols.AppendLine($"{ta}passport_id {ColPrefix}passport_id, {ta}initial_meter_reading {ColPrefix}initial_meter_reading");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.equipment_master");
            return sql.ToString();
        }




        internal static Equipment_Master DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Equipment_Master RetVal = new();
            RetVal.Equip_Id = (string)Util.GetRowVal(row, $"{ColPrefix}equip_id");
            RetVal.Equip_Desc = (string)Util.GetRowVal(row, $"{ColPrefix}equip_desc");
            RetVal.Plant = (string)Util.GetRowVal(row, $"{ColPrefix}plant");
            RetVal.Area = (string)Util.GetRowVal(row, $"{ColPrefix}area");
            RetVal.Equip_Group = (string)Util.GetRowVal(row, $"{ColPrefix}equip_group");
            RetVal.Equip_Type = (string)Util.GetRowVal(row, $"{ColPrefix}equip_type");
            RetVal.Initial_Meter_Reading = (decimal)Util.GetRowVal(row, $"{ColPrefix}initial_meter_reading");
            return RetVal;
        }

    }
}
