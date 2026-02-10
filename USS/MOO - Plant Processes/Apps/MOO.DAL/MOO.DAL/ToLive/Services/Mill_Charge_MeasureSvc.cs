using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Services
{
    public static class Mill_Charge_MeasureSvc
    {
        static Mill_Charge_MeasureSvc()
        {
            Util.RegisterOracle();
        }


        public static Mill_Charge_Measure Get(string equip_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE equip_id = :equip_id");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("equip_id", equip_id);
            da.SelectCommand.BindByName = true;

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }

        /// <summary>
        /// Gets the mill charge measure records for the defined equipment search
        /// </summary>
        /// <param name="EquipSearch">example search (120, 130)</param>
        /// <returns></returns>
        public static List<Mill_Charge_Measure> GetAll(string EquipSearch)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE equip_id LIKE ('%' || :EquipSearch || '%')");
            sql.AppendLine("ORDER BY equip_id");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("EquipSearch", EquipSearch);
            da.SelectCommand.BindByName = true;

            DataSet ds = MOO.Data.ExecuteQuery(da);
            List<Mill_Charge_Measure> elements = new();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                elements.Add(DataRowToObject(dr));
            }
            return elements;
        }


        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}equip_id {ColPrefix}equip_id, {ta}measure_date {ColPrefix}measure_date");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.mill_charge_measure");
            return sql.ToString();
        }



        public static int Update(Mill_Charge_Measure obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Mill_Charge_Measure obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.mill_charge_measure SET");
            sql.AppendLine("measure_date = :measure_date");
            sql.AppendLine("WHERE equip_id = :equip_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("measure_date", obj.Measure_Date);
            upd.Parameters.Add("equip_id", obj.Equip_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }



        internal static Mill_Charge_Measure DataRowToObject(DataRow row, string ColPrefix = "")
        {
            Mill_Charge_Measure RetVal = new();
            RetVal.Equip_Id = row.Field<string>($"{ColPrefix}equip_id");
            RetVal.Measure_Date = row.Field<DateTime>($"{ColPrefix}measure_date");
            return RetVal;
        }

    }
}
