using MOO.DAL.ToLive.Enums;
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
    public static class FE_K_MineSvc
    {
        static FE_K_MineSvc()
        {
            Util.RegisterOracle();
        }


        public static FE_K_Mine Get(int fe_k_mine_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE fe_k_mine_id = :fe_k_mine_id");


            FE_K_Mine retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("fe_k_mine_id", fe_k_mine_id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }

        public static List<FE_K_Mine> GetAllByReportId(long fe_report_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE fe_report_id = :fe_report_id");

            List<FE_K_Mine> elements = [];
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("fe_report_id", fe_report_id);
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
            cols.AppendLine($"{ta}fe_k_mine_id {ColPrefix}fe_k_mine_id, {ta}fe_report_id {ColPrefix}fe_report_id, ");
            cols.AppendLine($"{ta}source {ColPrefix}source, {ta}equip {ColPrefix}equip, ");
            cols.AppendLine($"{ta}equip_operating {ColPrefix}equip_operating, {ta}steam_plume {ColPrefix}steam_plume, ");
            cols.AppendLine($"{ta}req_attention {ColPrefix}req_attention, {ta}comments {ColPrefix}comments, ");
            cols.AppendLine($"{ta}emission_code {ColPrefix}emission_code, {ta}sort_order {ColPrefix}sort_order, ");
            cols.AppendLine($"{ta}fe_category {ColPrefix}fe_category");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.fe_k_mine");
            return sql.ToString();
        }


        public static int Insert(FE_K_Mine obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(FE_K_Mine obj, OracleConnection conn)
        {
            if (obj.Fe_K_Mine_Id <= 0)
                obj.Fe_K_Mine_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.seq_fe_report"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.fe_k_mine(");
            sql.AppendLine("fe_k_mine_id, fe_report_id, source, equip, equip_operating, steam_plume, ");
            sql.AppendLine("req_attention, comments, emission_code, sort_order, fe_category)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":fe_k_mine_id, :fe_report_id, :source, :equip, :equip_operating, :steam_plume, ");
            sql.AppendLine(":req_attention, :comments, :emission_code, :sort_order, :fe_category)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("fe_k_mine_id", obj.Fe_K_Mine_Id);
            ins.Parameters.Add("fe_report_id", obj.Fe_Report_Id);
            ins.Parameters.Add("source", obj.Source);
            ins.Parameters.Add("equip", obj.Equip);
            ins.Parameters.Add("equip_operating", obj.Equip_Operating ? 1 : 0);
            ins.Parameters.Add("steam_plume", obj.Steam_Plume == null ? null : (int)obj.Steam_Plume);
            ins.Parameters.Add("req_attention", obj.Req_Attention ? 1 : 0);
            ins.Parameters.Add("comments", obj.Comments);
            ins.Parameters.Add("emission_code", obj.Emission_Code);
            ins.Parameters.Add("sort_order", obj.Sort_Order);
            ins.Parameters.Add("fe_category", obj.Fe_Category);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(FE_K_Mine obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(FE_K_Mine obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.fe_k_mine SET");
            sql.AppendLine("fe_report_id = :fe_report_id, ");
            sql.AppendLine("source = :source, ");
            sql.AppendLine("equip = :equip, ");
            sql.AppendLine("equip_operating = :equip_operating, ");
            sql.AppendLine("steam_plume = :steam_plume, ");
            sql.AppendLine("req_attention = :req_attention, ");
            sql.AppendLine("comments = :comments, ");
            sql.AppendLine("emission_code = :emission_code, ");
            sql.AppendLine("sort_order = :sort_order, ");
            sql.AppendLine("fe_category = :fe_category");
            sql.AppendLine("WHERE fe_k_mine_id = :fe_k_mine_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("fe_report_id", obj.Fe_Report_Id);
            upd.Parameters.Add("source", obj.Source);
            upd.Parameters.Add("equip", obj.Equip);
            upd.Parameters.Add("equip_operating", obj.Equip_Operating ? 1 : 0);
            upd.Parameters.Add("steam_plume", obj.Steam_Plume == null ? null : (int)obj.Steam_Plume);
            upd.Parameters.Add("req_attention", obj.Req_Attention ? 1 : 0);
            upd.Parameters.Add("comments", obj.Comments);
            upd.Parameters.Add("emission_code", obj.Emission_Code);
            upd.Parameters.Add("sort_order", obj.Sort_Order);
            upd.Parameters.Add("fe_category", obj.Fe_Category);
            upd.Parameters.Add("fe_k_mine_id", obj.Fe_K_Mine_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(FE_K_Mine obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(FE_K_Mine obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.fe_k_mine");
            sql.AppendLine("WHERE fe_k_mine_id = :fe_k_mine_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("fe_k_mine_id", obj.Fe_K_Mine_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static FE_K_Mine DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            FE_K_Mine RetVal = new();
            RetVal.Fe_K_Mine_Id = (int)Util.GetRowVal(row, $"{ColPrefix}fe_k_mine_id");
            RetVal.Fe_Report_Id = (int)Util.GetRowVal(row, $"{ColPrefix}fe_report_id");
            RetVal.Source = (string)Util.GetRowVal(row, $"{ColPrefix}source");
            RetVal.Equip = (string)Util.GetRowVal(row, $"{ColPrefix}equip");
            RetVal.Equip_Operating = (short)Util.GetRowVal(row, $"{ColPrefix}equip_operating") == 1;
            if (!row.IsDBNull(row.GetOrdinal($"{ColPrefix}steam_plume")))
            {
                RetVal.Steam_Plume = (Steam_Plume)Enum.Parse(typeof(Steam_Plume),((short)Util.GetRowVal(row, $"{ColPrefix}steam_plume")).ToString());
            }
            RetVal.Req_Attention = (short)Util.GetRowVal(row, $"{ColPrefix}req_attention") == 1;
            RetVal.Comments = (string)Util.GetRowVal(row, $"{ColPrefix}comments");
            RetVal.Emission_Code = (string)Util.GetRowVal(row, $"{ColPrefix}emission_code");
            RetVal.Sort_Order = (short)Util.GetRowVal(row, $"{ColPrefix}sort_order");
            RetVal.Fe_Category = (string)Util.GetRowVal(row, $"{ColPrefix}fe_category");
            return RetVal;
        }

    }
}
