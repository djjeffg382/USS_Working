using MOO.DAL.ToLive.Enums;
using MOO.DAL.ToLive.Models;
using MOO.Enums.Extension;
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
    public static class PLO_Pellet_Spec_ValuesSvc
    {
        static PLO_Pellet_Spec_ValuesSvc()
        {
            Util.RegisterOracle();
        }

        /// <summary>
        /// Gets the spec for a specific date and product type
        /// </summary>
        /// <param name="pelletSpecDate"></param>
        /// <param name="ploProductId"></param>
        /// <param name="spec"></param>
        /// <returns></returns>
        public static PLO_Pellet_Spec_Values Get(DateTime pelletSpecDate, int ploProductId, PLO_Spec_Name spec)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE ps.product_id = :ploProductId");
            sql.AppendLine("AND ps.Start_Date <= :pelletSpecDate");
            sql.AppendLine("AND (ps.End_Date > :pelletSpecDate OR ps.End_Date IS NULL)");
            sql.AppendLine("AND psv.SPEC_NAME > :spec");


            PLO_Pellet_Spec_Values retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);

            cmd.Parameters.Add("ploProductId", ploProductId);
            cmd.Parameters.Add("pelletSpecDate", pelletSpecDate);
            cmd.Parameters.Add("spec", spec.GetDisplay().Name);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }

        /// <summary>
        /// Gets the specs for a specific Pellet Spec Parent Record
        /// </summary>
        /// <param name="ploPelletSpecId">PLO Pellet Spec Id from PLO_PELLET_SPEC table</param>
        /// <returns></returns>
        public static List<PLO_Pellet_Spec_Values> GetByPelletSpec(long ploPelletSpecId)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE ps.plo_pellet_spec_id = :ploPelletSpecId");

            List<PLO_Pellet_Spec_Values> elements = [];
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("ploPelletSpecId", ploPelletSpecId);
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
        /// gets all specs for a specified date
        /// </summary>
        /// <param name="pelletSpecDate"></param>
        /// <param name="ploProductId"></param>
        /// <returns></returns>
        public static List<PLO_Pellet_Spec_Values> GetSpecsByDate(DateTime pelletSpecDate, int ploProductId)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE ps.product_id = :ploProductId");
            sql.AppendLine("AND ps.Start_Date <= :pelletSpecDate");
            sql.AppendLine("AND (ps.End_Date > :pelletSpecDate OR ps.End_Date IS NULL)");

            List<PLO_Pellet_Spec_Values> elements = [];
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("ploProductId", ploProductId);
            cmd.Parameters.Add("pelletSpecDate", pelletSpecDate);
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
        /// Gets a new list of specs for creating a new spec list
        /// </summary>
        public static List<PLO_Pellet_Spec_Values> GetNewSpecs(PLO_Pellet_Spec ploSpec)
        {
            List<PLO_Pellet_Spec_Values> retVal = [];
            var specs = Enum.GetValues<PLO_Spec_Name>();
            foreach(var spec in specs)
            {
                PLO_Pellet_Spec_Values val = new()
                {
                    Plo_Pellet_Spec = ploSpec,
                    Spec = spec
                };
                retVal.Add(val);
            }
            return retVal;
        }



        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}plo_pellet_spec_values_id {ColPrefix}plo_pellet_spec_values_id, ");
            cols.AppendLine($"{ta}plo_pellet_spec_id {ColPrefix}plo_pellet_spec_id, {ta}spec {ColPrefix}spec, ");
            cols.AppendLine($"{ta}sort_order {ColPrefix}sort_order, {ta}typical_value {ColPrefix}typical_value, ");
            cols.AppendLine($"{ta}min_value {ColPrefix}min_value, {ta}max_value {ColPrefix}max_value");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns("psv","psv_") + ",");
            sql.AppendLine(PLO_Pellet_SpecSvc.GetColumns("ps", "ps_") + ",");
            sql.AppendLine(PLO_ProductSvc.GetColumns("p", "p_"));
            sql.AppendLine("FROM tolive.plo_pellet_spec_values psv");
            sql.AppendLine("JOIN tolive.plo_pellet_spec ps ON ps.plo_pellet_spec_id = psv.plo_pellet_spec_id");
            sql.AppendLine("JOIN tolive.plo_product p ON ps.product_id = p.product_id");
            return sql.ToString();
        }


        public static int Insert(PLO_Pellet_Spec_Values obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(PLO_Pellet_Spec_Values obj, OracleConnection conn)
        {
            if (obj.Plo_Pellet_Spec_Values_Id <= 0)
                obj.Plo_Pellet_Spec_Values_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.PLO_SEQ"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.PLO_PELLET_SPEC_VALUES(");
            sql.AppendLine("plo_pellet_spec_values_id, plo_pellet_spec_id, spec, spec_name, sort_order, typical_value, ");
            sql.AppendLine("min_value, max_value)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":plo_pellet_spec_values_id, :plo_pellet_spec_id, :spec, :spec_name, :sort_order, ");
            sql.AppendLine(":typical_value, :min_value, :max_value)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("plo_pellet_spec_values_id", obj.Plo_Pellet_Spec_Values_Id);
            ins.Parameters.Add("plo_pellet_spec_id", obj.Plo_Pellet_Spec.Plo_Pellet_Spec_Id);
            ins.Parameters.Add("spec", obj.Spec.ToString());
            ins.Parameters.Add("spec_name", obj.Spec.GetDisplay().Name);
            ins.Parameters.Add("sort_order", obj.Spec.GetDisplay().Order);
            ins.Parameters.Add("typical_value", obj.Typical_Value);
            ins.Parameters.Add("min_value", obj.Min_Value);
            ins.Parameters.Add("max_value", obj.Max_Value);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(PLO_Pellet_Spec_Values obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(PLO_Pellet_Spec_Values obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.PLO_PELLET_SPEC_VALUES SET");
            sql.AppendLine("plo_pellet_spec_id = :plo_pellet_spec_id, ");
            sql.AppendLine("spec = :spec, ");
            sql.AppendLine("spec_name = :spec_name, ");
            sql.AppendLine("sort_order = :sort_order, ");
            sql.AppendLine("typical_value = :typical_value, ");
            sql.AppendLine("min_value = :min_value, ");
            sql.AppendLine("max_value = :max_value");
            sql.AppendLine("WHERE plo_pellet_spec_values_id = :plo_pellet_spec_values_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("plo_pellet_spec_id", obj.Plo_Pellet_Spec.Plo_Pellet_Spec_Id);
            upd.Parameters.Add("spec", obj.Spec.ToString());
            upd.Parameters.Add("spec_name", obj.Spec.GetDisplay().Name);
            upd.Parameters.Add("sort_order", obj.Spec.GetDisplay().Order);
            upd.Parameters.Add("typical_value", obj.Typical_Value);
            upd.Parameters.Add("min_value", obj.Min_Value);
            upd.Parameters.Add("max_value", obj.Max_Value);
            upd.Parameters.Add("plo_pellet_spec_values_id", obj.Plo_Pellet_Spec_Values_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(PLO_Pellet_Spec_Values obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(PLO_Pellet_Spec_Values obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.PLO_PELLET_SPEC_VALUES");
            sql.AppendLine("WHERE plo_pellet_spec_values_id = :plo_pellet_spec_values_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("plo_pellet_spec_values_id", obj.Plo_Pellet_Spec_Values_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static PLO_Pellet_Spec_Values DataRowToObject(DbDataReader row, string ColPrefix = "psv_")
        {
            PLO_Pellet_Spec_Values RetVal = new();
            RetVal.Plo_Pellet_Spec_Values_Id = (long)Util.GetRowVal(row, $"{ColPrefix}plo_pellet_spec_values_id");
            RetVal.Plo_Pellet_Spec = PLO_Pellet_SpecSvc.DataRowToObject(row, "ps_");
            RetVal.Spec = (Enum.Parse< PLO_Spec_Name>(Util.GetRowVal(row, $"{ColPrefix}spec").ToString()));
            RetVal.Typical_Value = (double?)Util.GetRowVal(row, $"{ColPrefix}typical_value");
            RetVal.Min_Value = (double?)Util.GetRowVal(row, $"{ColPrefix}min_value");
            RetVal.Max_Value = (double?)Util.GetRowVal(row, $"{ColPrefix}max_value");
            return RetVal;
        }

    }
}
