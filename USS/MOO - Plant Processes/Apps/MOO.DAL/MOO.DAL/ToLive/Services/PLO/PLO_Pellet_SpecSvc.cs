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
    public static class PLO_Pellet_SpecSvc
    {
        static PLO_Pellet_SpecSvc()
        {
            Util.RegisterOracle();
        }


        public static PLO_Pellet_Spec Get(long plo_pellet_spec_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE plo_pellet_spec_id = :plo_pellet_spec_id");


            PLO_Pellet_Spec retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("plo_pellet_spec_id", plo_pellet_spec_id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }


        /// <summary>
        /// Gets the spec for the corresponding pellet spec date
        /// </summary>
        /// <param name="pelletSpecDate"></param>
        /// <param name="ploProductId"></param>
        /// <returns></returns>
        public static PLO_Pellet_Spec GetBySpecDate(DateTime pelletSpecDate, int ploProductId)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE ps.product_id = :ploProductId");
            sql.AppendLine("AND ps.Start_Date <= :pelletSpecDate");
            sql.AppendLine("AND (ps.End_Date > :pelletSpecDate OR ps.End_Date IS NULL)");


            PLO_Pellet_Spec retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("ploProductId", ploProductId);
            cmd.Parameters.Add("pelletSpecDate", pelletSpecDate);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }


        public static List<PLO_Pellet_Spec> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            List<PLO_Pellet_Spec> elements = [];
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
            cols.AppendLine($"{ta}plo_pellet_spec_id {ColPrefix}plo_pellet_spec_id, {ta}start_date {ColPrefix}start_date, ");
            cols.AppendLine($"{ta}end_date {ColPrefix}end_date, {ta}modified_date {ColPrefix}modified_date, ");
            cols.AppendLine($"{ta}modified_by {ColPrefix}modified_by, {ta}product_id {ColPrefix}product_id");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns("ps", "ps_") + ",");
            sql.AppendLine(PLO_ProductSvc.GetColumns("p", "p_"));
            sql.AppendLine("FROM tolive.plo_pellet_spec ps");
            sql.AppendLine("JOIN tolive.plo_product p ON ps.product_id = p.product_id");
            return sql.ToString();
        }


        public static int Insert(PLO_Pellet_Spec obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(PLO_Pellet_Spec obj, OracleConnection conn)
        {
            if (obj.Plo_Pellet_Spec_Id <= 0)
                obj.Plo_Pellet_Spec_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.PLO_SEQ"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.plo_pellet_spec(");
            sql.AppendLine("plo_pellet_spec_id, start_date, end_date, modified_date, modified_by, product_id)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":plo_pellet_spec_id, :start_date, :end_date, :modified_date, :modified_by, ");
            sql.AppendLine(":product_id)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("plo_pellet_spec_id", obj.Plo_Pellet_Spec_Id);
            ins.Parameters.Add("start_date", obj.Start_Date);
            ins.Parameters.Add("end_date", obj.End_Date);
            ins.Parameters.Add("modified_date", obj.Modified_Date);
            ins.Parameters.Add("modified_by", obj.Modified_By);
            ins.Parameters.Add("product_id", obj.Product.Product_Id);

            ins.BindByName = true;
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(PLO_Pellet_Spec obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(PLO_Pellet_Spec obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.plo_pellet_spec SET");
            sql.AppendLine("start_date = :start_date, ");
            sql.AppendLine("end_date = :end_date, ");
            sql.AppendLine("modified_date = :modified_date, ");
            sql.AppendLine("modified_by = :modified_by, ");
            sql.AppendLine("product_id = :product_id");
            sql.AppendLine("WHERE plo_pellet_spec_id = :plo_pellet_spec_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("start_date", obj.Start_Date);
            upd.Parameters.Add("end_date", obj.End_Date);
            upd.Parameters.Add("modified_date", obj.Modified_Date);
            upd.Parameters.Add("modified_by", obj.Modified_By);
            upd.Parameters.Add("product_id", obj.Product.Product_Id);
            upd.Parameters.Add("plo_pellet_spec_id", obj.Plo_Pellet_Spec_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(PLO_Pellet_Spec obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(PLO_Pellet_Spec obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.plo_pellet_spec");
            sql.AppendLine("WHERE plo_pellet_spec_id = :plo_pellet_spec_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.Parameters.Add("plo_pellet_spec_id", obj.Plo_Pellet_Spec_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static PLO_Pellet_Spec DataRowToObject(DbDataReader row, string ColPrefix = "ps_")
        {
            PLO_Pellet_Spec RetVal = new();
            RetVal.Plo_Pellet_Spec_Id = (long)Util.GetRowVal(row, $"{ColPrefix}plo_pellet_spec_id");
            RetVal.Start_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}start_date");
            RetVal.End_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}end_date");
            RetVal.Modified_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}modified_date");
            RetVal.Modified_By = (string)Util.GetRowVal(row, $"{ColPrefix}modified_by");
            RetVal.Product = PLO_ProductSvc.DataRowToObject(row, "p_");
            return RetVal;
        }


    }
}
