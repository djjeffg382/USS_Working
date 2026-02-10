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
    public static class Equipment_Component_MasterSvc
    {
        static Equipment_Component_MasterSvc()
        {
            Util.RegisterOracle();
        }

        public static List<Equipment_Component_Master> GetEquipComponents(string equipId)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE ecm.equip_id = :equip_id");

            List<Equipment_Component_Master> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("equip_id", equipId);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    elements.Add(DataRowToObject(rdr,"ecm_"));
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
            cols.AppendLine($"{ta}equip_id {ColPrefix}equip_id, {ta}component_id {ColPrefix}component_id, ");
            cols.AppendLine($"{ta}equip_desc {ColPrefix}equip_desc, {ta}active_status {ColPrefix}active_status, ");
            cols.AppendLine($"{ta}erp_desc {ColPrefix}erp_desc, {ta}lims_samp_pnt {ColPrefix}lims_samp_pnt, ");
            cols.AppendLine($"{ta}lims_unit {ColPrefix}lims_unit");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns("ecm","ecm_") + ",");
            sql.AppendLine(Equipment_MasterSvc.GetColumns("em", "em_") + ",");
            sql.AppendLine(Equipment_Unit_TypesSvc.GetColumns("et", "et_"));
            sql.AppendLine("FROM tolive.equipment_component_master ecm");
            sql.AppendLine("JOIN tolive.equipment_master em ON em.equip_id = ecm.equip_id");
            sql.AppendLine("LEFT JOIN tolive.equipment_unit_types et ON ecm.lims_unit = et.unit_type");
            return sql.ToString();
        }


        public static int Insert(Equipment_Component_Master obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Equipment_Component_Master obj, OracleConnection conn)
        {
            
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Equipment_Component_Master(");
            sql.AppendLine("equip_id, component_id, equip_desc, active_status, erp_desc, lims_samp_pnt, ");
            sql.AppendLine("lims_unit)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":equip_id, :component_id, :equip_desc, :active_status, :erp_desc, :lims_samp_pnt, ");
            sql.AppendLine(":lims_unit)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("equip_id", obj.Equip.Equip_Id);
            ins.Parameters.Add("component_id", obj.Component_Id);
            ins.Parameters.Add("equip_desc", obj.Equip_Desc);
            ins.Parameters.Add("active_status", obj.Active_Status ? 1 : 0);
            ins.Parameters.Add("erp_desc", obj.Erp_Desc);
            ins.Parameters.Add("lims_samp_pnt", obj.Lims_Samp_Pnt);
            if(obj.Lims_Unit != null)
                ins.Parameters.Add("lims_unit", obj.Lims_Unit.Unit_Type);
            else
                ins.Parameters.Add("lims_unit", DBNull.Value);

            ins.BindByName = true;
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Equipment_Component_Master obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Equipment_Component_Master obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Equipment_Component_Master SET");
            sql.AppendLine("equip_desc = :equip_desc, ");
            sql.AppendLine("active_status = :active_status, ");
            sql.AppendLine("erp_desc = :erp_desc, ");
            sql.AppendLine("lims_samp_pnt = :lims_samp_pnt, ");
            sql.AppendLine("lims_unit = :lims_unit");
            sql.AppendLine("WHERE equip_id = :equip_id");
            sql.AppendLine("AND component_id = :component_id ");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.Parameters.Add("equip_desc", obj.Equip_Desc);
            upd.Parameters.Add("active_status", obj.Active_Status ? 1 : 0);
            upd.Parameters.Add("erp_desc", obj.Erp_Desc);
            upd.Parameters.Add("lims_samp_pnt", obj.Lims_Samp_Pnt);
            if (obj.Lims_Unit != null)
                upd.Parameters.Add("lims_unit", obj.Lims_Unit.Unit_Type);
            else
                upd.Parameters.Add("lims_unit", DBNull.Value);


            upd.Parameters.Add("equip_id", obj.Equip.Equip_Id);
            upd.Parameters.Add("component_id", obj.Component_Id);

            upd.BindByName = true;
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Equipment_Component_Master obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Equipment_Component_Master obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.Equipment_Component_Master");
            sql.AppendLine("WHERE equip_id = :equip_id");
            sql.AppendLine("AND component_id = :component_id ");
            OracleCommand del = new(sql.ToString(), conn);
            del.Parameters.Add("equip_id", obj.Equip.Equip_Id);
            del.Parameters.Add("component_id", obj.Component_Id);
            del.BindByName = true;
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Equipment_Component_Master DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Equipment_Component_Master RetVal = new();
            RetVal.Component_Id = Convert.ToInt16( (decimal)Util.GetRowVal(row, $"{ColPrefix}component_id"));
            RetVal.Equip_Desc = (string)Util.GetRowVal(row, $"{ColPrefix}equip_desc");
            RetVal.Active_Status = (short)Util.GetRowVal(row, $"{ColPrefix}active_status") == 1;
            RetVal.Erp_Desc = (string)Util.GetRowVal(row, $"{ColPrefix}erp_desc");
            RetVal.Lims_Samp_Pnt = (string)Util.GetRowVal(row, $"{ColPrefix}lims_samp_pnt");


            RetVal.Equip = Equipment_MasterSvc.DataRowToObject(row, "em_");
            if (row.IsDBNull(row.GetOrdinal($"{ColPrefix}lims_unit")))
                RetVal.Lims_Unit = null;
            else
                RetVal.Lims_Unit = Equipment_Unit_TypesSvc.DataRowToObject(row, "et_");
            return RetVal;
        }

    }
}
