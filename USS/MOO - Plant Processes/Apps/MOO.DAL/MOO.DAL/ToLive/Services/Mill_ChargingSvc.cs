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
    public class Mill_ChargingSvc
    {
        static Mill_ChargingSvc()
        {
            Util.RegisterOracle();
        }


        public static Mill_Charging Get(DateTime Charge_Date, string Equip_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE charge_date = :Charge_Date");
            sql.AppendLine("AND equip_id = :Equip_Id");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("charge_date", Charge_Date);
            da.SelectCommand.Parameters.Add("Equip_Id", Equip_Id);
            da.SelectCommand.BindByName = true;

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }


        public static List<Mill_Charging> GetByDate(DateTime StartDate, DateTime EndDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE charge_date BETWEEN :StartDate AND :EndDate");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("StartDate", StartDate);
            da.SelectCommand.Parameters.Add("EndDate", EndDate);
            da.SelectCommand.BindByName = true;

            DataSet ds = MOO.Data.ExecuteQuery(da);
            List<Mill_Charging> elements = new();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                elements.Add(DataRowToObject(dr));
            }
            return elements;
        }

        /// <summary>
        /// Gets the last Lbs Per Charge value uses for a given date and equipment
        /// </summary>
        /// <param name="ChargeDate"></param>
        /// <param name="EqpSearch">Equipment search string (use 110, 120, or 130</param>
        /// <returns></returns>
        /// <remarks>This will be used in the application to default the lbs per charge field for the user</remarks>
        public static decimal GetLastLbPerCharge(DateTime ChargeDate, string EqpSearch)
        {
            StringBuilder sql = new();

            sql.AppendLine("SELECT lbs_per_charge");
            sql.AppendLine("FROM (");
            sql.AppendLine("        SELECT lbs_per_charge, charge_date,");
            sql.AppendLine("            ROW_NUMBER() OVER(ORDER BY charge_date desc) rn");
            sql.AppendLine("        FROM tolive.mill_charging");
            sql.AppendLine("        WHERE charge_date <= :ChargeDate");
            sql.AppendLine("        AND equip_id like :EqpSearch || '%') tbl");
            sql.AppendLine("WHERE rn = 1");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("ChargeDate", ChargeDate);
            da.SelectCommand.Parameters.Add("EqpSearch", EqpSearch);
            da.SelectCommand.BindByName = true;
            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count > 0)
                return (decimal)((Single)ds.Tables[0].Rows[0][0]);
            else
                return 0M;
        }


        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}charge_date {ColPrefix}charge_date, {ta}equip_id {ColPrefix}equip_id, ");
            cols.AppendLine($"{ta}charge_count {ColPrefix}charge_count, {ta}lbs_per_charge {ColPrefix}lbs_per_charge");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.mill_charging");
            return sql.ToString();
        }


        public static int Insert(Mill_Charging obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Mill_Charging obj, OracleConnection conn)
        {
           
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.mill_charging(");
            sql.AppendLine("charge_date, equip_id, charge_count, lbs_per_charge)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":charge_date, :equip_id, :charge_count, :lbs_per_charge)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("charge_date", obj.Charge_Date);
            ins.Parameters.Add("equip_id", obj.Equip_Id);
            ins.Parameters.Add("charge_count", obj.Charge_Count);
            ins.Parameters.Add("lbs_per_charge", obj.Lbs_Per_Charge);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Mill_Charging obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Mill_Charging obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.mill_charging SET");
            sql.AppendLine("charge_count = :charge_count, ");
            sql.AppendLine("lbs_per_charge = :lbs_per_charge");
            sql.AppendLine("WHERE charge_date = :charge_date");
            sql.AppendLine("AND equip_id = :equip_id ");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("charge_count", obj.Charge_Count);
            upd.Parameters.Add("lbs_per_charge", obj.Lbs_Per_Charge);
            //Where Clause
            upd.Parameters.Add("charge_date", obj.Charge_Date);
            upd.Parameters.Add("equip_id", obj.Equip_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Mill_Charging obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Mill_Charging obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.mill_charging");
            sql.AppendLine("WHERE charge_date = :charge_date");
            sql.AppendLine("AND equip_id = :equip_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("charge_date", obj.Charge_Date);
            del.Parameters.Add("equip_id", obj.Equip_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Mill_Charging DataRowToObject(DataRow row, string ColPrefix = "")
        {
            Mill_Charging RetVal = new();
            RetVal.Charge_Date = row.Field<DateTime>($"{ColPrefix}charge_date");
            RetVal.Equip_Id = row.Field<string>($"{ColPrefix}equip_id");
            RetVal.Charge_Count = row.Field<int>($"{ColPrefix}charge_count");
            RetVal.Lbs_Per_Charge = (decimal)row.Field<Single>($"{ColPrefix}lbs_per_charge");
            return RetVal;
        }

    }
}
