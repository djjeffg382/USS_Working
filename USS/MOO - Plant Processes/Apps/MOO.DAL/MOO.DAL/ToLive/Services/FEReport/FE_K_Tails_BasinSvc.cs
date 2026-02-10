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
    public static class FE_K_Tails_BasinSvc
    {
        static FE_K_Tails_BasinSvc()
        {
            Util.RegisterOracle();
        }

        public static FE_K_Tails_Basin Get(int fe_k_tb_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE fe_k_tb_id = :fe_k_tb_id");

            FE_K_Tails_Basin retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("fe_k_tb_id", fe_k_tb_id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }

        public static FE_K_Tails_Basin GetByReportId(int fe_report_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE fe_report_id = :fe_report_id");

            FE_K_Tails_Basin retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("fe_report_id", fe_report_id);
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
            cols.AppendLine($"{ta}fe_k_tb_id {ColPrefix}fe_k_tb_id, {ta}fe_report_id {ColPrefix}fe_report_id, ");
            cols.AppendLine($"{ta}observation_normal {ColPrefix}observation_normal, {ta}req_attention {ColPrefix}req_attention, ");
            cols.AppendLine($"{ta}semi_truck_hauling {ColPrefix}semi_truck_hauling, ");
            cols.AppendLine($"{ta}road_req_attention {ColPrefix}road_req_attention");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.fe_k_tails_basin" );
            
            return sql.ToString();
        }


        public static int Insert(FE_K_Tails_Basin obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(FE_K_Tails_Basin obj, OracleConnection conn)
        {
            if (obj.Fe_K_Tb_Id <= 0)
                obj.Fe_K_Tb_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.seq_fe_report"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.fe_k_tails_basin(");
            sql.AppendLine("fe_k_tb_id, fe_report_id, observation_normal, req_attention, semi_truck_hauling, ");
            sql.AppendLine("road_req_attention)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":fe_k_tb_id, :fe_report_id, :observation_normal, :req_attention, :semi_truck_hauling, ");
            sql.AppendLine(":road_req_attention)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("fe_k_tb_id", obj.Fe_K_Tb_Id);
            ins.Parameters.Add("fe_report_id", obj.Fe_Report_Id);
            ins.Parameters.Add("observation_normal", obj.Observation_Normal ? 1 : 0);
            ins.Parameters.Add("req_attention", obj.Req_Attention ? 1 : 0);
            ins.Parameters.Add("semi_truck_hauling", obj.Semi_Truck_Hauling ? 1 : 0);
            ins.Parameters.Add("road_req_attention", obj.Road_Req_Attention != null ? (obj.Road_Req_Attention ?? false ? 1 : 0): null);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(FE_K_Tails_Basin obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(FE_K_Tails_Basin obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.fe_k_tails_basin SET");
            sql.AppendLine("fe_report_id = :fe_report_id, ");
            sql.AppendLine("observation_normal = :observation_normal, ");
            sql.AppendLine("req_attention = :req_attention, ");
            sql.AppendLine("semi_truck_hauling = :semi_truck_hauling, ");
            sql.AppendLine("road_req_attention = :road_req_attention");
            sql.AppendLine("WHERE fe_k_tb_id = :fe_k_tb_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("fe_report_id", obj.Fe_Report_Id);
            upd.Parameters.Add("observation_normal", obj.Observation_Normal ? 1 : 0);
            upd.Parameters.Add("req_attention", obj.Req_Attention ? 1 : 0);
            upd.Parameters.Add("semi_truck_hauling", obj.Semi_Truck_Hauling ? 1 : 0);
            upd.Parameters.Add("road_req_attention", obj.Road_Req_Attention != null ? (obj.Road_Req_Attention ?? false ? 1 : 0): null);
            upd.Parameters.Add("fe_k_tb_id", obj.Fe_K_Tb_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(FE_K_Tails_Basin obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(FE_K_Tails_Basin obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.fe_k_tails_basin");
            sql.AppendLine("WHERE fe_k_tb_id = :fe_k_tb_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("fe_k_tb_id", obj.Fe_K_Tb_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }

        internal static FE_K_Tails_Basin DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            FE_K_Tails_Basin RetVal = new();
            RetVal.Fe_K_Tb_Id = (int)Util.GetRowVal(row, $"{ColPrefix}fe_k_tb_id");
            RetVal.Fe_Report_Id = (int)Util.GetRowVal(row, $"{ColPrefix}fe_report_id");
            RetVal.Observation_Normal = (short)Util.GetRowVal(row, $"{ColPrefix}observation_normal") == 1;
            RetVal.Req_Attention = (short)Util.GetRowVal(row, $"{ColPrefix}req_attention") == 1;
            RetVal.Semi_Truck_Hauling = (short)Util.GetRowVal(row, $"{ColPrefix}semi_truck_hauling") == 1;
            var roadAttention = Util.GetRowVal(row, $"{ColPrefix}road_req_attention"); 
            RetVal.Road_Req_Attention = (roadAttention != null ? (short)roadAttention == 1 : null);
            return RetVal;
        }

    }
}
