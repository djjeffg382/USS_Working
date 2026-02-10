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
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Note there is no update/delete.  Use the LIMS_Batch.DeleteChildren and then re-insert to ensure the order is set correctly</remarks>
    public static class LIMS_Batch_SamplesSvc
    {
        static LIMS_Batch_SamplesSvc()
        {
            Util.RegisterOracle();
        }


        public static List<LIMS_Batch_Samples> Get(int Batch_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE batch_id = :batch_id");

            List<LIMS_Batch_Samples> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("batch_id", Batch_id);
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
            cols.AppendLine($"{ta}batch_id {ColPrefix}batch_id, {ta}batch_seq {ColPrefix}batch_seq, ");
            cols.AppendLine($"{ta}sample_number {ColPrefix}sample_number");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.lims_batch_samples");
            return sql.ToString();
        }


        public static int Insert(LIMS_Batch_Samples obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(LIMS_Batch_Samples obj, OracleConnection conn)
        {
           

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO TOLIVE.LIMS_Batch_Samples(");
            sql.AppendLine("batch_id, batch_seq, sample_number)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":batch_id, :batch_seq, :sample_number)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("batch_id", obj.Batch_Id);
            ins.Parameters.Add("batch_seq", obj.Batch_Seq);
            ins.Parameters.Add("sample_number", obj.Sample_Number);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        internal static LIMS_Batch_Samples DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            LIMS_Batch_Samples RetVal = new();
            RetVal.Batch_Id = (int)(decimal)Util.GetRowVal(row, $"{ColPrefix}batch_id");
            RetVal.Batch_Seq = (decimal)Util.GetRowVal(row, $"{ColPrefix}batch_seq");
            RetVal.Sample_Number = (int)(decimal)Util.GetRowVal(row, $"{ColPrefix}sample_number");
            return RetVal;
        }

    }
}
