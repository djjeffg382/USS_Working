using MOO.DAL.Core.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Core.Services
{
    public static class Forecast_MetricsSvc
    {
        static Forecast_MetricsSvc()
        {
            Util.RegisterOracle();
        }


        public static Forecast_Metrics Get(int CoreId)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE CoreId = :CoreId");


            Forecast_Metrics retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("CoreId", CoreId);
            var rdr = MOO.Data.ExecuteReader(cmd);
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }


        public static List<Forecast_Metrics> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            List<Forecast_Metrics> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            var rdr = MOO.Data.ExecuteReader(cmd);
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
            cols.AppendLine($"{ta}name {ColPrefix}name, {ta}coreid {ColPrefix}coreid, {ta}accessgroup {ColPrefix}accessgroup, ");
            cols.AppendLine($"{ta}username {ColPrefix}username, {ta}uom {ColPrefix}uom, {ta}plant {ColPrefix}plant, ");
            cols.AppendLine($"{ta}valuetype {ColPrefix}valuetype, {ta}sortorder {ColPrefix}sortorder, ");
            cols.AppendLine($"{ta}dailyvals {ColPrefix}dailyvals, {ta}bpval {ColPrefix}bpval, ");
            cols.AppendLine($"{ta}storebyshiftarea {ColPrefix}storebyshiftarea, {ta}active {ColPrefix}active, ");
            cols.AppendLine($"{ta}shiftcount {ColPrefix}shiftcount");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM core.forecast_metrics");
            return sql.ToString();
        }


        public static int Insert(Forecast_Metrics obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Forecast_Metrics obj, OracleConnection conn)
        {
            
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO core.Forecast_Metrics(");
            sql.AppendLine("name, coreid, accessgroup, username, uom, plant, valuetype, sortorder, dailyvals, ");
            sql.AppendLine("bpval, storebyshiftarea, active, shiftcount)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":name, :coreid, :accessgroup, :username, :uom, :plant, :valuetype, :sortorder, ");
            sql.AppendLine(":dailyvals, :bpval, :storebyshiftarea, :active, :shiftcount)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("name", obj.Name);
            ins.Parameters.Add("coreid", obj.CoreId);
            ins.Parameters.Add("accessgroup", obj.AccessGroup);            
            ins.Parameters.Add("username", string.Join(",", obj.Users));
            ins.Parameters.Add("uom", obj.Uom);
            ins.Parameters.Add("plant", obj.Plant.ToString());
            ins.Parameters.Add("valuetype", obj.ValueType);
            ins.Parameters.Add("sortorder", obj.SortOrder);
            ins.Parameters.Add("dailyvals", obj.DailyVals);
            ins.Parameters.Add("bpval", obj.BpVal ? 1: 0);
            ins.Parameters.Add("storebyshiftarea", obj.StoreByShiftArea.ToString());
            ins.Parameters.Add("active", obj.Active ? 1 : 0);
            ins.Parameters.Add("shiftcount", obj.ShiftCount);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Forecast_Metrics obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Forecast_Metrics obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE core.Forecast_Metrics SET");
            sql.AppendLine("name = :name, ");
            sql.AppendLine("accessgroup = :accessgroup, ");
            sql.AppendLine("username = :username, ");
            sql.AppendLine("uom = :uom, ");
            sql.AppendLine("plant = :plant, ");
            sql.AppendLine("valuetype = :valuetype, ");
            sql.AppendLine("sortorder = :sortorder, ");
            sql.AppendLine("dailyvals = :dailyvals, ");
            sql.AppendLine("bpval = :bpval, ");
            sql.AppendLine("storebyshiftarea = :storebyshiftarea, ");
            sql.AppendLine("active = :active, ");
            sql.AppendLine("shiftcount = :shiftcount");
            sql.AppendLine("WHERE coreid = :coreid");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("name", obj.Name);
            upd.Parameters.Add("accessgroup", obj.AccessGroup);
            upd.Parameters.Add("username", string.Join(",", obj.Users));
            upd.Parameters.Add("uom", obj.Uom);
            upd.Parameters.Add("plant", obj.Plant.ToString());
            upd.Parameters.Add("valuetype", obj.ValueType);
            upd.Parameters.Add("sortorder", obj.SortOrder);
            upd.Parameters.Add("dailyvals", obj.DailyVals);
            upd.Parameters.Add("bpval", obj.BpVal? 1:0);
            upd.Parameters.Add("storebyshiftarea", obj.StoreByShiftArea.ToString());
            upd.Parameters.Add("active", obj.Active ? 1 : 0);
            upd.Parameters.Add("shiftcount", obj.ShiftCount);
            upd.Parameters.Add("coreid", obj.CoreId);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);




            return recsAffected;
        }


        public static int Delete(Forecast_Metrics obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Forecast_Metrics obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM core.Forecast_Metrics");
            sql.AppendLine("WHERE coreid = :coreid");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("coreid", obj.CoreId);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Forecast_Metrics DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Forecast_Metrics RetVal = new();
            RetVal.Name = (string)Util.GetRowVal(row, $"{ColPrefix}name");
            RetVal.CoreId = (int)(decimal)Util.GetRowVal(row, $"{ColPrefix}coreid");
            RetVal.AccessGroup = (string)Util.GetRowVal(row, $"{ColPrefix}accessgroup");
            if (row.IsDBNull(row.GetOrdinal("username")))
            {
                RetVal.Users = new List<string>();
            }
            else
            {
                var users = ((string)Util.GetRowVal(row, $"{ColPrefix}username")).Trim();
                if (string.IsNullOrEmpty(users))
                    RetVal.Users = new List<string>();
                else
                    RetVal.Users = ((string)Util.GetRowVal(row, $"{ColPrefix}username")).Trim().Split(',').ToList();
            }
            


            RetVal.Uom = (string)Util.GetRowVal(row, $"{ColPrefix}uom");
            RetVal.Plant = Enum.Parse<MOO.Plant>((string)Util.GetRowVal(row, $"{ColPrefix}plant"));
            RetVal.ValueType = (decimal)Util.GetRowVal(row, $"{ColPrefix}valuetype");
            RetVal.SortOrder = (int?)(decimal?)Util.GetRowVal(row, $"{ColPrefix}sortorder");
            RetVal.DailyVals = (string)Util.GetRowVal(row, $"{ColPrefix}dailyvals");
            RetVal.BpVal = (decimal?)Util.GetRowVal(row, $"{ColPrefix}bpval") == 1;
            if (row.IsDBNull(row.GetOrdinal("storebyshiftarea")))
                RetVal.StoreByShiftArea = null;
            else
                RetVal.StoreByShiftArea = Enum.Parse<MOO.Area>((string)Util.GetRowVal(row, $"{ColPrefix}storebyshiftarea"));

            RetVal.Active = (short)Util.GetRowVal(row, $"{ColPrefix}active") == 1;
            RetVal.ShiftCount = (byte?)(short?)Util.GetRowVal(row, $"{ColPrefix}shiftcount");
            return RetVal;
        }

    }
}
