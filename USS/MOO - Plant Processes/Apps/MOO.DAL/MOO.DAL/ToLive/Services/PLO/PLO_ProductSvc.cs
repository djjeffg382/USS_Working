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
    public static class PLO_ProductSvc
    {
        static PLO_ProductSvc()
        {
            Util.RegisterOracle();
        }


        public static PLO_Product Get(int product_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE product_id = :product_id");


            PLO_Product retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("product_id", product_id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }


        public static List<PLO_Product> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            List<PLO_Product> elements = [];
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
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
            cols.AppendLine($"{ta}product_id {ColPrefix}product_id, {ta}product_number {ColPrefix}product_number, ");
            cols.AppendLine($"{ta}product_name {ColPrefix}product_name, {ta}edi_product_id {ColPrefix}edi_product_id");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.plo_product");
            return sql.ToString();
        }


        public static int Insert(PLO_Product obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(PLO_Product obj, OracleConnection conn)
        {
            if (obj.Product_Id <= 0)
                obj.Product_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.PLO_SEQ"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.plo_product(");
            sql.AppendLine("product_id, product_number, product_name, edi_product_id)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":product_id, :product_number, :product_name, :edi_product_id)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("product_id", obj.Product_Id);
            ins.Parameters.Add("product_number", obj.Product_Number);
            ins.Parameters.Add("product_name", obj.Product_Name);
            ins.Parameters.Add("edi_product_id", obj.Edi_Product_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(PLO_Product obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(PLO_Product obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.plo_product SET");
            sql.AppendLine("product_number = :product_number, ");
            sql.AppendLine("product_name = :product_name, ");
            sql.AppendLine("edi_product_id = :edi_product_id");
            sql.AppendLine("WHERE product_id = :product_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("product_number", obj.Product_Number);
            upd.Parameters.Add("product_name", obj.Product_Name);
            upd.Parameters.Add("edi_product_id", obj.Edi_Product_Id);
            upd.Parameters.Add("product_id", obj.Product_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(PLO_Product obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(PLO_Product obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.plo_product");
            sql.AppendLine("WHERE product_id = :product_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("product_id", obj.Product_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static PLO_Product DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            PLO_Product RetVal = new();
            RetVal.Product_Id = Convert.ToInt32( (decimal)Util.GetRowVal(row, $"{ColPrefix}product_id"));
            RetVal.Product_Number = (string)Util.GetRowVal(row, $"{ColPrefix}product_number");
            RetVal.Product_Name = (string)Util.GetRowVal(row, $"{ColPrefix}product_name");
            if (row.IsDBNull(row.GetOrdinal($"{ColPrefix}edi_product_id")))
                RetVal.Edi_Product_Id = null;
            else
                RetVal.Edi_Product_Id = Convert.ToInt32((decimal?)Util.GetRowVal(row, $"{ColPrefix}edi_product_id"));
            return RetVal;
        }

    }
}
