using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Services
{
    public static class GCP_DeviationsSvc
    {
        static GCP_DeviationsSvc()
        {
            Util.RegisterOracle();
        }

        public static GCP_Deviations Get(byte Line, DateTime Date)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE Line = :Line");
            sql.AppendLine("AND The_Date = :TheDate");

            GCP_Deviations retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("Line", Line);
            cmd.Parameters.Add("TheDate", Date);
            OracleDataReader rdr = cmd.ExecuteReader();

            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }

        public static List<GCP_Deviations> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            List<GCP_Deviations> retVal = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            OracleDataReader rdr = cmd.ExecuteReader();

            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    retVal.Add(DataRowToObject(rdr));
                }
            }
            conn.Close();
            return retVal;
        }

        private static string GetSelect()
        {
            StringBuilder sb = new();
            sb.AppendLine("SELECT line, the_date, deviation_type,");
            sb.AppendLine("     num_of_good_reads, num_of_bad_reads, corrective_action");
            sb.AppendLine("FROM tolive.gcp_deviations");
            
            return sb.ToString();
        }

        public static int Insert(GCP_Deviations obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Insert(GCP_Deviations obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO TOLIVE.GCP_Deviations(");
            sql.AppendLine("Line, The_Date, Deviation_Type, Num_Of_Good_Reads, Num_Of_Bad_Reads, Corrective_Action)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":Line, :The_Date, :Deviation_Type, :Num_Of_Good_Reads, :Num_Of_Bad_Reads, :Corrective_Action)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("Line", obj.Line);
            ins.Parameters.Add("The_Date", obj.The_Date);
            ins.Parameters.Add("Deviation_Type", obj.Deviation_Type);
            ins.Parameters.Add("Num_Of_Good_Reads", obj.Num_Of_Good_Reads);
            ins.Parameters.Add("Num_Of_Bad_Reads", obj.Num_Of_Bad_Reads);
            ins.Parameters.Add("Corrective_Action", obj.Corrective_Action);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }

        public static int Update(GCP_Deviations obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Update(GCP_Deviations obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.gcp_deviations SET");
            sql.AppendLine("deviation_type = :deviation_type, ");
            sql.AppendLine("num_of_good_reads = :num_of_good_reads, ");
            sql.AppendLine("num_of_bad_reads = :num_of_bad_reads, ");
            sql.AppendLine("corrective_action = :corrective_action ");

            sql.AppendLine("WHERE line = :line");
            sql.AppendLine("AND the_date = :the_date");

            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("deviation_type", obj.Deviation_Type);
            upd.Parameters.Add("num_of_good_reads", obj.Num_Of_Good_Reads);
            upd.Parameters.Add("num_of_bad_reads", obj.Num_Of_Bad_Reads);
            upd.Parameters.Add("corrective_action", obj.Corrective_Action);

            upd.Parameters.Add("line", obj.Line);
            upd.Parameters.Add("the_date", obj.The_Date);

            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }
        private static GCP_Deviations DataRowToObject(DbDataReader row)
        {
            GCP_Deviations retVal = new();
            retVal.Line = (byte)(decimal)Util.GetRowVal(row, "Line");
            retVal.The_Date = (DateTime)Util.GetRowVal(row, "The_Date");
            retVal.Deviation_Type = (string)Util.GetRowVal(row, "Deviation_Type");
            retVal.Num_Of_Good_Reads = (short)(decimal)Util.GetRowVal(row, "Num_Of_Good_Reads");
            retVal.Num_Of_Bad_Reads = (short)(decimal)Util.GetRowVal(row, "Num_Of_Bad_Reads");
            retVal.Corrective_Action = (string)Util.GetRowVal(row, "Corrective_Action");

            return retVal;
        }
    }
}
