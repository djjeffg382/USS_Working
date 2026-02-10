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
    public static class MACT_DC_15MinSvc
    {
        static MACT_DC_15MinSvc()
        {
            Util.RegisterOracle();
        }


        public static List<MACT_DC_15Min> GetByDateRange(string Equip_No, DateTime StartDate, DateTime EndDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Equip_No = :Equip_No");
            sql.AppendLine("AND Read_Date BETWEEN :StartDate AND :EndDate ");

            List<MACT_DC_15Min> elements = [];
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("Equip_No", Equip_No);
            cmd.Parameters.Add("StartDate", StartDate);
            cmd.Parameters.Add("EndDate", EndDate);

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
            cols.AppendLine($"{ta}equip_no {ColPrefix}equip_no, {ta}read_no {ColPrefix}read_no, ");
            cols.AppendLine($"{ta}read_date {ColPrefix}read_date, ROUND({ta}press_val,4) {ColPrefix}press_val, ");
            cols.AppendLine($"ROUND({ta}flow_val,4) {ColPrefix}flow_val, {ta}read_ind {ColPrefix}read_ind, ");
            cols.AppendLine($"{ta}read_date_trunc {ColPrefix}read_date_trunc");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.mact_dc_15min");
            return sql.ToString();
        }


        public static int Insert(MACT_DC_15Min obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(MACT_DC_15Min obj, OracleConnection conn)
        {
            
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.MACT_DC_15Min(");
            sql.AppendLine("equip_no, read_no, read_date, press_val, flow_val, read_ind)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":equip_no, :read_no, :read_date, :press_val, :flow_val, :read_ind)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("equip_no", obj.Equip_No);
            ins.Parameters.Add("read_no", obj.Read_No);
            ins.Parameters.Add("read_date", obj.Read_Date);
            ins.Parameters.Add("press_val", obj.Press_Val);
            ins.Parameters.Add("flow_val", obj.Flow_Val);
            ins.Parameters.Add("read_ind", obj.Read_Ind.ToString()[..1]);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }



        internal static MACT_DC_15Min DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            MACT_DC_15Min RetVal = new();
            RetVal.Equip_No = (string)Util.GetRowVal(row, $"{ColPrefix}equip_no");
            RetVal.Read_No = (decimal?)Util.GetRowVal(row, $"{ColPrefix}read_no");
            RetVal.Read_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}read_date");
            RetVal.Press_Val = (decimal?)Util.GetRowVal(row, $"{ColPrefix}press_val");
            RetVal.Flow_Val = (decimal?)Util.GetRowVal(row, $"{ColPrefix}flow_val");
            switch ((string)Util.GetRowVal(row, $"{ColPrefix}read_ind"))
            {
                case "B":
                    RetVal.Read_Ind = Enums.DustCollReadIndicator.Bad;
                    break;
                case "G":
                    RetVal.Read_Ind = Enums.DustCollReadIndicator.Good;
                    break;
                case "O":
                    RetVal.Read_Ind = Enums.DustCollReadIndicator.Off;
                    break;
            }
            return RetVal;
        }

    }
}
