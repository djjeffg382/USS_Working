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
    public static class LIMS_BatchSvc
    {
        static LIMS_BatchSvc()
        {
            Util.RegisterOracle();
        }


        public static LIMS_Batch Get(int batch_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE batch_id = :batch_id");


            LIMS_Batch retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("batch_id", batch_id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }

        public static List<LIMS_Batch> GetAll(Enums.LIMS_Batch_Type? BatchType = null)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            if (BatchType != null)
                sql.AppendLine($"WHERE batch_type = '{BatchType.ToString()}'");

            List<LIMS_Batch> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
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
            cols.AppendLine($"{ta}batch_id {ColPrefix}batch_id, {ta}created_date {ColPrefix}created_date, ");
            cols.AppendLine($"{ta}last_edit_date {ColPrefix}last_edit_date, ");
            cols.AppendLine($"{ta}last_instrument_export {ColPrefix}last_instrument_export, {ta}batch_name {ColPrefix}batch_name, ");
            cols.AppendLine($"{ta}batch_type {ColPrefix}batch_type");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.lims_batch");
            return sql.ToString();
        }


        public static int Insert(LIMS_Batch obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(LIMS_Batch obj, OracleConnection conn)
        {
            if (obj.Batch_Id <= 0)
                obj.Batch_Id = Convert.ToInt32(MOO.Data.GetNextSequence("TOLIVE.SEQ_LIMS_BATCH"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO TOLIVE.LIMS_Batch(");
            sql.AppendLine("batch_id, created_date, last_edit_date, last_instrument_export, batch_name, ");
            sql.AppendLine("batch_type)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":batch_id, :created_date, :last_edit_date, :last_instrument_export, :batch_name, ");
            sql.AppendLine(":batch_type)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("batch_id", obj.Batch_Id);
            ins.Parameters.Add("created_date", obj.Created_Date);
            ins.Parameters.Add("last_edit_date", obj.Last_Edit_Date);
            ins.Parameters.Add("last_instrument_export", obj.Last_Instrument_Export);
            ins.Parameters.Add("batch_name", obj.Batch_Name);
            ins.Parameters.Add("batch_type", obj.Batch_Type.ToString());
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(LIMS_Batch obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(LIMS_Batch obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE TOLIVE.LIMS_Batch SET");
            sql.AppendLine("created_date = :created_date, ");
            sql.AppendLine("last_edit_date = :last_edit_date, ");
            sql.AppendLine("last_instrument_export = :last_instrument_export, ");
            sql.AppendLine("batch_name = :batch_name, ");
            sql.AppendLine("batch_type = :batch_type");
            sql.AppendLine("WHERE batch_id = :batch_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("created_date", obj.Created_Date);
            upd.Parameters.Add("last_edit_date", obj.Last_Edit_Date);
            upd.Parameters.Add("last_instrument_export", obj.Last_Instrument_Export);
            upd.Parameters.Add("batch_name", obj.Batch_Name);
            upd.Parameters.Add("batch_type", obj.Batch_Type.ToString());
            upd.Parameters.Add("batch_id", obj.Batch_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(LIMS_Batch obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(LIMS_Batch obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM TOLIVE.LIMS_Batch");
            sql.AppendLine("WHERE batch_id = :batch_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("batch_id", obj.Batch_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }



        /// <summary>
        /// deletes all child records in the LIMS_BATCH_SAMPLES table
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int DeleteChildren(LIMS_Batch obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = DeleteChildren(obj, conn);
            conn.Close();
            return recsAffected;
        }

        /// <summary>
        /// deletes all child records in the LIMS_BATCH_SAMPLES table
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int DeleteChildren(LIMS_Batch obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM TOLIVE.LIMS_Batch_Samples");
            sql.AppendLine("WHERE batch_id = :batch_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("batch_id", obj.Batch_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static LIMS_Batch DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            LIMS_Batch RetVal = new();
            RetVal.Batch_Id = (int)(decimal)Util.GetRowVal(row, $"{ColPrefix}batch_id");
            RetVal.Created_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}created_date");
            RetVal.Last_Edit_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}last_edit_date");
            RetVal.Last_Instrument_Export = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}last_instrument_export");
            RetVal.Batch_Name = (string)Util.GetRowVal(row, $"{ColPrefix}batch_name");
            RetVal.Batch_Type = Enum.Parse<MOO.DAL.ToLive.Enums.LIMS_Batch_Type> ((string)Util.GetRowVal(row, $"{ColPrefix}batch_type"));
            return RetVal;
        }

    }
}
