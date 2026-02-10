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
    public static class Dust_Coll_Epa_ReadingSvc
    {
        static Dust_Coll_Epa_ReadingSvc()
        {
            Util.RegisterOracle();
        }

        public static Dust_Coll_Epa_Reading Get(string equip_no)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE equip_no = :equip_no");


            Dust_Coll_Epa_Reading retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("equip_no", equip_no);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }

        //Returns the most recent reading for an equip no
        public static Dust_Coll_Epa_Reading GetLatest(string equip_no)
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT * FROM (");
            sql.AppendLine("SELECT ROW_NUMBER() OVER(ORDER BY virtual_read_date DESC) rn,");
            sql.AppendLine("equip_no equip_no, virtual_read_date virtual_read_date,");
            sql.AppendLine("pressure_date pressure_date, pressure_val pressure_val,");
            sql.AppendLine("flow_date flow_date, flow_val flow_val,");
            sql.AppendLine("reading_ind reading_ind, processed_ind processed_ind");
            sql.AppendLine("FROM tolive.dust_coll_epa_reading");
            sql.AppendLine("WHERE equip_no = :equip_no)");
            sql.AppendLine("WHERE rn = 1");

            Dust_Coll_Epa_Reading retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("equip_no", equip_no);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }

        public static List<Dust_Coll_Epa_Reading> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE virtual_read_date BETWEEN :startDate AND :endDate");

            List<Dust_Coll_Epa_Reading> elements = [];
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
            cols.AppendLine($"{ta}equip_no {ColPrefix}equip_no, {ta}virtual_read_date {ColPrefix}virtual_read_date, ");
            cols.AppendLine($"{ta}pressure_date {ColPrefix}pressure_date, {ta}pressure_val {ColPrefix}pressure_val, ");
            cols.AppendLine($"{ta}flow_date {ColPrefix}flow_date, {ta}flow_val {ColPrefix}flow_val, ");
            cols.AppendLine($"{ta}reading_ind {ColPrefix}reading_ind, {ta}processed_ind {ColPrefix}processed_ind");
            return cols.ToString();
        }

        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.dust_coll_epa_reading");
            return sql.ToString();
        }

        public static int Insert(Dust_Coll_Epa_Reading obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Dust_Coll_Epa_Reading obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.dust_coll_epa_reading(");
            sql.AppendLine("equip_no, virtual_read_date, pressure_date, pressure_val, flow_date, flow_val, ");
            sql.AppendLine("reading_ind, processed_ind)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":equip_no, :virtual_read_date, :pressure_date, :pressure_val, :flow_date, :flow_val, ");
            sql.AppendLine(":reading_ind, :processed_ind)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("equip_no", obj.Equip_No);
            ins.Parameters.Add("virtual_read_date", obj.Virtual_Read_Date);
            ins.Parameters.Add("pressure_date", obj.Pressure_Date);
            ins.Parameters.Add("pressure_val", obj.Pressure_Val);
            ins.Parameters.Add("flow_date", obj.Flow_Date);
            ins.Parameters.Add("flow_val", obj.Flow_Val);
            ins.Parameters.Add("reading_ind", obj.Reading_Ind);
            ins.Parameters.Add("processed_ind", obj.Processed_Ind);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }

        public static int Update(Dust_Coll_Epa_Reading obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Update(Dust_Coll_Epa_Reading obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.dust_coll_epa_reading SET");
            sql.AppendLine("virtual_read_date = :virtual_read_date, ");
            sql.AppendLine("pressure_date = :pressure_date, ");
            sql.AppendLine("pressure_val = :pressure_val, ");
            sql.AppendLine("flow_date = :flow_date, ");
            sql.AppendLine("flow_val = :flow_val, ");
            sql.AppendLine("reading_ind = :reading_ind, ");
            sql.AppendLine("processed_ind = :processed_ind");
            sql.AppendLine("WHERE equip_no = :equip_no");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("virtual_read_date", obj.Virtual_Read_Date);
            upd.Parameters.Add("pressure_date", obj.Pressure_Date);
            upd.Parameters.Add("pressure_val", obj.Pressure_Val);
            upd.Parameters.Add("flow_date", obj.Flow_Date);
            upd.Parameters.Add("flow_val", obj.Flow_Val);
            upd.Parameters.Add("reading_ind", obj.Reading_Ind);
            upd.Parameters.Add("processed_ind", obj.Processed_Ind);
            upd.Parameters.Add("equip_no", obj.Equip_No);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }

        internal static Dust_Coll_Epa_Reading DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Dust_Coll_Epa_Reading RetVal = new();
            RetVal.Equip_No = (string)Util.GetRowVal(row, $"{ColPrefix}equip_no");
            RetVal.Virtual_Read_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}virtual_read_date");
            RetVal.Pressure_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}pressure_date");
            RetVal.Pressure_Val = (double?)Util.GetRowVal(row, $"{ColPrefix}pressure_val");
            RetVal.Flow_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}flow_date");
            RetVal.Flow_Val = (double?)Util.GetRowVal(row, $"{ColPrefix}flow_val");
            RetVal.Reading_Ind = (string)Util.GetRowVal(row, $"{ColPrefix}reading_ind");
            RetVal.Processed_Ind = (string)Util.GetRowVal(row, $"{ColPrefix}processed_ind");
            return RetVal;
        }
    }
}
