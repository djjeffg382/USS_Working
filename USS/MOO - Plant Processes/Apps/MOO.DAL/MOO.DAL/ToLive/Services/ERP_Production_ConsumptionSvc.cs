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
    public static class ERP_Production_ConsumptionSvc
    {
        static ERP_Production_ConsumptionSvc()
        {
            Util.RegisterOracle();
        }


        public static ERP_Production_Consumption Get(MOO.Plant Plant, DateTime MonthDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE month_date = :MonthDate");
            sql.AppendLine("AND loc = :Plant");


            ERP_Production_Consumption retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("MonthDate",MOO.Dates.FirstDayOfMonth( MonthDate));
            cmd.Parameters.Add("Plant", Plant == Plant.Minntac ? "mtc":"ktc");
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }



        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}sent_to_erp {ColPrefix}sent_to_erp, {ta}month_date {ColPrefix}month_date, ");
            cols.AppendLine($"{ta}loc {ColPrefix}loc, {ta}sent_date {ColPrefix}sent_date, {ta}lime_cons {ColPrefix}lime_cons, ");
            cols.AppendLine($"{ta}conc_prod {ColPrefix}conc_prod, {ta}aggf_prod {ColPrefix}aggf_prod, ");
            cols.AppendLine($"{ta}agga_prod {ColPrefix}agga_prod, {ta}aggk1_prod {ColPrefix}aggk1_prod, ");
            cols.AppendLine($"{ta}crush_prod {ColPrefix}crush_prod, {ta}conc_cons {ColPrefix}conc_cons, ");
            cols.AppendLine($"{ta}aggf_cons {ColPrefix}aggf_cons, {ta}agga_cons {ColPrefix}agga_cons, ");
            cols.AppendLine($"{ta}aggk1_cons {ColPrefix}aggk1_cons, {ta}crush_cons {ColPrefix}crush_cons,");
            cols.AppendLine($"{ta}drcon_cons {ColPrefix}drcon_cons, {ta}drpell_cons {ColPrefix}drpell_cons,");
            cols.AppendLine($"{ta}drcon_prod {ColPrefix}drcon_prod, {ta}drpell_prod {ColPrefix}drpell_prod,");
            cols.AppendLine($"{ta}erp_message {ColPrefix}erp_message");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.erp_production_consumption");
            return sql.ToString();
        }


        public static int Insert(ERP_Production_Consumption obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(ERP_Production_Consumption obj, OracleConnection conn)
        {
            
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO TOLIVE.ERP_Production_Consumption(");
            sql.AppendLine("sent_to_erp, month_date, loc, sent_date, lime_cons, conc_prod, aggf_prod, agga_prod, ");
            sql.AppendLine("aggk1_prod, crush_prod, conc_cons, aggf_cons, agga_cons, aggk1_cons, crush_cons, ");
            sql.AppendLine("drcon_cons, drpell_cons, drcon_prod, drpell_prod, erp_message)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":sent_to_erp, :month_date, :loc, :sent_date, :lime_cons, :conc_prod, :aggf_prod, ");
            sql.AppendLine(":agga_prod, :aggk1_prod, :crush_prod, :conc_cons, :aggf_cons, :agga_cons, :aggk1_cons, ");
            sql.AppendLine(":crush_cons,:drcon_cons,:drpell_cons,:drcon_prod,:drpell_prod,:erp_message)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("sent_to_erp", obj.Sent_To_Erp ? 1 : 0);
            ins.Parameters.Add("month_date", MOO.Dates.FirstDayOfMonth(obj.Month_Date));
            ins.Parameters.Add("loc", obj.Loc == Plant.Minntac ? "mtc":"ktc");
            ins.Parameters.Add("sent_date", obj.Sent_Date);
            ins.Parameters.Add("lime_cons", obj.Lime_Cons);
            ins.Parameters.Add("conc_prod", obj.Conc_Prod);
            ins.Parameters.Add("aggf_prod", obj.Aggf_Prod);
            ins.Parameters.Add("agga_prod", obj.Agga_Prod);
            ins.Parameters.Add("aggk1_prod", obj.Aggk1_Prod);
            ins.Parameters.Add("crush_prod", obj.Crush_Prod);
            ins.Parameters.Add("conc_cons", obj.Conc_Cons);
            ins.Parameters.Add("aggf_cons", obj.Aggf_Cons);
            ins.Parameters.Add("agga_cons", obj.Agga_Cons);
            ins.Parameters.Add("aggk1_cons", obj.Aggk1_Cons);
            ins.Parameters.Add("crush_cons", obj.Crush_Cons);
            ins.Parameters.Add("drcon_cons", obj.DRCon_Cons);
            ins.Parameters.Add("drpell_cons", obj.DRPell_Cons);
            ins.Parameters.Add("drcon_prod", obj.DRCon_Prod);
            ins.Parameters.Add("drpell_prod", obj.DRPell_Prod);
            ins.Parameters.Add("erp_message", obj.ERP_Message);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(ERP_Production_Consumption obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(ERP_Production_Consumption obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE TOLIVE.ERP_Production_Consumption SET");
            sql.AppendLine("sent_to_erp = :sent_to_erp,");
            sql.AppendLine("sent_date = :sent_date, ");
            sql.AppendLine("lime_cons = :lime_cons, ");
            sql.AppendLine("conc_prod = :conc_prod, ");
            sql.AppendLine("aggf_prod = :aggf_prod, ");
            sql.AppendLine("agga_prod = :agga_prod, ");
            sql.AppendLine("aggk1_prod = :aggk1_prod, ");
            sql.AppendLine("crush_prod = :crush_prod, ");
            sql.AppendLine("conc_cons = :conc_cons, ");
            sql.AppendLine("aggf_cons = :aggf_cons, ");
            sql.AppendLine("agga_cons = :agga_cons, ");
            sql.AppendLine("aggk1_cons = :aggk1_cons, ");
            sql.AppendLine("crush_cons = :crush_cons,");
            sql.AppendLine("drcon_cons = :drcon_cons,");
            sql.AppendLine("drpell_cons = :drpell_cons,");
            sql.AppendLine("drcon_prod = :drcon_prod,");
            sql.AppendLine("drpell_prod = :drpell_prod,");
            sql.AppendLine("erp_message = :erp_message");

            sql.AppendLine("WHERE month_date = :month_date ");
            sql.AppendLine("AND loc = :loc ");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;

            upd.Parameters.Add("sent_to_erp", obj.Sent_To_Erp ? 1:0);
            upd.Parameters.Add("sent_date", obj.Sent_Date);
            upd.Parameters.Add("lime_cons", obj.Lime_Cons);
            upd.Parameters.Add("conc_prod", obj.Conc_Prod);
            upd.Parameters.Add("aggf_prod", obj.Aggf_Prod);
            upd.Parameters.Add("agga_prod", obj.Agga_Prod);
            upd.Parameters.Add("aggk1_prod", obj.Aggk1_Prod);
            upd.Parameters.Add("crush_prod", obj.Crush_Prod);
            upd.Parameters.Add("conc_cons", obj.Conc_Cons);
            upd.Parameters.Add("aggf_cons", obj.Aggf_Cons);
            upd.Parameters.Add("agga_cons", obj.Agga_Cons);
            upd.Parameters.Add("aggk1_cons", obj.Aggk1_Cons);
            upd.Parameters.Add("crush_cons", obj.Crush_Cons);
            upd.Parameters.Add("drcon_cons", obj.DRCon_Cons);
            upd.Parameters.Add("drpell_cons", obj.DRPell_Cons);
            upd.Parameters.Add("drcon_prod", obj.DRCon_Prod);
            upd.Parameters.Add("drpell_prod", obj.DRPell_Prod);
            upd.Parameters.Add("erp_message", obj.ERP_Message);

            upd.Parameters.Add("month_date", MOO.Dates.FirstDayOfMonth(obj.Month_Date));
            upd.Parameters.Add("loc", obj.Loc == Plant.Minntac ? "mtc" : "ktc");
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(ERP_Production_Consumption obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(ERP_Production_Consumption obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM TOLIVE.ERP_Production_Consumption");
            sql.AppendLine("WHERE month_date = :month_date ");
            sql.AppendLine("AND loc = :loc ");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("month_date", MOO.Dates.FirstDayOfMonth(obj.Month_Date));
            del.Parameters.Add("loc", obj.Loc == Plant.Minntac ? "mtc" : "ktc");
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static ERP_Production_Consumption DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            ERP_Production_Consumption RetVal = new();
            RetVal.Sent_To_Erp = (short)Util.GetRowVal(row, $"{ColPrefix}sent_to_erp") == 1;
            RetVal.Month_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}month_date");
            RetVal.Loc = (string)Util.GetRowVal(row, $"{ColPrefix}loc") == "mtc"? Plant.Minntac:Plant.Keetac;
            RetVal.Sent_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}sent_date");
            RetVal.Lime_Cons = (decimal?)Util.GetRowVal(row, $"{ColPrefix}lime_cons");
            RetVal.Conc_Prod = (decimal?)Util.GetRowVal(row, $"{ColPrefix}conc_prod");
            RetVal.Aggf_Prod = (decimal?)Util.GetRowVal(row, $"{ColPrefix}aggf_prod");
            RetVal.Agga_Prod = (decimal?)Util.GetRowVal(row, $"{ColPrefix}agga_prod");
            RetVal.Aggk1_Prod = (decimal?)Util.GetRowVal(row, $"{ColPrefix}aggk1_prod");
            RetVal.Crush_Prod = (decimal?)Util.GetRowVal(row, $"{ColPrefix}crush_prod");
            RetVal.Conc_Cons = (decimal?)Util.GetRowVal(row, $"{ColPrefix}conc_cons");
            RetVal.Aggf_Cons = (decimal?)Util.GetRowVal(row, $"{ColPrefix}aggf_cons");
            RetVal.Agga_Cons = (decimal?)Util.GetRowVal(row, $"{ColPrefix}agga_cons");
            RetVal.Aggk1_Cons = (decimal?)Util.GetRowVal(row, $"{ColPrefix}aggk1_cons");
            RetVal.Crush_Cons = (decimal?)Util.GetRowVal(row, $"{ColPrefix}crush_cons");
            RetVal.DRCon_Cons = (long?)Util.GetRowVal(row, $"{ColPrefix}drcon_cons");
            RetVal.DRPell_Cons = (long?)Util.GetRowVal(row, $"{ColPrefix}drpell_cons");
            RetVal.DRCon_Prod = (long?)Util.GetRowVal(row, $"{ColPrefix}drcon_prod");
            RetVal.DRPell_Prod = (long?)Util.GetRowVal(row, $"{ColPrefix}drpell_prod");

            RetVal.ERP_Message = (string?)Util.GetRowVal(row, $"{ColPrefix}erp_message");
            return RetVal;
        }

    }
}
