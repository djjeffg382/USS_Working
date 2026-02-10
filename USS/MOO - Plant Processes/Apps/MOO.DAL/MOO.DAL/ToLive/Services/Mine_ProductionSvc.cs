using MOO.DAL.ToLive.Models;
using MOO.DAL.Pi.Services;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.DAL.Pi.Enums;
using Newtonsoft.Json.Linq;
using System.Data;

namespace MOO.DAL.ToLive.Services
{
    public static class Mine_ProductionSvc
    {
        static Mine_ProductionSvc()
        {
            Util.RegisterOracle();
        }

        /// <summary>
        /// Gets a PI Total listing
        /// </summary>
        /// <param name="StartTime">Start Time of the PI Total</param>
        /// <param name="EndTime">End Time of the PI Total</param>
        /// <param name="Tag">Pi Tag</param>
        /// <returns></returns>
        public static Mine_Production.Belt_Pi_Points GetCrusherPiPointTotalsPerBelt(DateTime StartTime, DateTime EndTime, Mine_Production.CrusherBelts Belt)
        {
            //CrusherBelts Belt
            var ret = new Mine_Production.Belt_Pi_Points();
            if (Belt == Mine_Production.CrusherBelts.Belt_01)
            {
                ret.Analog_Total = PiAggregateSvc.GetPiTotal(StartTime, EndTime, "AI202008")[0].Value * 24;
                ret.Pulse_Total = PiAggregateSvc.GetPiTotal(StartTime, EndTime, "PA221000")[0].Value * 2880;
            }

            if (Belt == Mine_Production.CrusherBelts.Belt_02)
            {
                ret.Analog_Total = PiAggregateSvc.GetPiTotal(StartTime, EndTime, "AI203008")[0].Value * 24;
                ret.Pulse_Total = PiAggregateSvc.GetPiTotal(StartTime, EndTime, "PA221030")[0].Value * 2880;
            }

            if (Belt == Mine_Production.CrusherBelts.Belt_03)
            {
                ret.Analog_Total = PiAggregateSvc.GetPiTotal(StartTime, EndTime, "AI201040")[0].Value * 24;
                ret.Pulse_Total = PiAggregateSvc.GetPiTotal(StartTime, EndTime, "PA222000")[0].Value * 2880;
            }
            return ret;

        }

        public static Mine_Production Get(DateTime shift_date, short shift)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE shift_date = :shift_date");
            sql.AppendLine("AND shift = :shift");


            Mine_Production retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("shift_date", shift_date);
            cmd.Parameters.Add("shift", shift);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }

        public static List<Mine_Production> GetBetweenDates(DateTime StartDate, DateTime EndDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE shift_date BETWEEN :StartDate AND :EndDate");

            List<Mine_Production> retList = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("StartDate", StartDate);
            cmd.Parameters.Add("EndDate", EndDate);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    retList.Add(DataRowToObject(rdr));
                }
            }
            conn.Close();
            return retList;
        }

            public static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}shift_date {ColPrefix}shift_date, {ta}shift {ColPrefix}shift, ");
            cols.AppendLine($"{ta}production_tons {ColPrefix}production_tons, {ta}modified_date {ColPrefix}modified_date, ");
            cols.AppendLine($"{ta}modified_by {ColPrefix}modified_by");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.mine_production");
            return sql.ToString();
        }


        public static int Insert(Mine_Production obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Mine_Production obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO Tolive.Mine_Production(");
            sql.AppendLine("shift_date, shift, production_tons, modified_date, modified_by)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":shift_date, :shift, :production_tons, :modified_date, :modified_by)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("shift_date", obj.Shift_Date);
            ins.Parameters.Add("shift", obj.Shift);
            ins.Parameters.Add("production_tons", obj.Production_Tons);
            ins.Parameters.Add("modified_date", obj.Modified_Date);
            ins.Parameters.Add("modified_by", obj.Modified_By);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Mine_Production obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Mine_Production obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE Tolive.Mine_Production SET");
            sql.AppendLine("production_tons = :production_tons, ");
            sql.AppendLine("modified_date = :modified_date, ");
            sql.AppendLine("modified_by = :modified_by");

            sql.AppendLine("WHERE shift_date = :shift_date");
            sql.AppendLine("AND shift = :shift");

            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("production_tons", obj.Production_Tons);
            upd.Parameters.Add("modified_date", obj.Modified_Date);
            upd.Parameters.Add("modified_by", obj.Modified_By);

            upd.Parameters.Add("shift_date", obj.Shift_Date);
            upd.Parameters.Add("shift", obj.Shift);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }

        internal static Mine_Production DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Mine_Production RetVal = new();
            RetVal.Shift_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}shift_date");
            RetVal.Shift = (short)Util.GetRowVal(row, $"{ColPrefix}shift");
            RetVal.Production_Tons = (decimal?)Util.GetRowVal(row, $"{ColPrefix}production_tons");
            RetVal.Modified_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}modified_date");
            RetVal.Modified_By = (string)Util.GetRowVal(row, $"{ColPrefix}modified_by");
            return RetVal;
        }

    }
}
