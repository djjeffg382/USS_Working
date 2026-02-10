using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Services
{
    public static class Met_Factor_HistorySvc
    {
        static Met_Factor_HistorySvc()
        {
            Util.RegisterOracle();
        }


        public static Met_Factor_History Get(int Factor_Id, DateTime EffectiveDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE mfh.factor_id = :factor_id");
            sql.AppendLine($"AND mfh.effective_date = :effective_date");
            sql.AppendLine("ORDER BY mfh.effective_date desc");
            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("factor_id", Factor_Id);
            da.SelectCommand.Parameters.Add("effective_date", EffectiveDate);

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }


        public static List<Met_Factor_History> GetAll(int Met_Factor_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE mfh.factor_id = :mf_id");
            sql.AppendLine("ORDER BY mfh.effective_date desc");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("mf_id", Met_Factor_Id);

            DataSet ds = MOO.Data.ExecuteQuery(da);
            List<Met_Factor_History> elements = new();
            if (ds.Tables[0].Rows.Count > 1)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    elements.Add(DataRowToObject(dr));
                }
            }
            return elements;
        }


        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine("mf.factor_id mf_factor_id, mf.name mf_name, mf.description mf_description,");
            sql.AppendLine("mfh.factor_id, mfh.effective_date, mfh.factor_value");
            sql.AppendLine("FROM tolive.met_factor_history mfh");
            sql.AppendLine("JOIN tolive.met_factor mf");
            sql.AppendLine("ON mfh.factor_id = mf.factor_id");
            return sql.ToString();
        }


        public static int Insert(Met_Factor_History obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Met_Factor_History obj, OracleConnection conn)
        {
            
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO Tolive.Met_Factor_History(");
            sql.AppendLine("factor_id, effective_date, factor_value)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":factor_id, :effective_date, :factor_value)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("factor_id", obj.Met_Factor.Factor_Id);
            ins.Parameters.Add("effective_date", obj.Effective_Date);
            ins.Parameters.Add("factor_value", obj.Factor_Value);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Met_Factor_History obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Met_Factor_History obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE Tolive.Met_Factor_History SET");
            sql.AppendLine("factor_value = :factor_value");
            sql.AppendLine("WHERE factor_id = :factor_id AND effective_date = :effective_date");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("factor_value", obj.Factor_Value);
            upd.Parameters.Add("factor_id", obj.Met_Factor.Factor_Id);
            upd.Parameters.Add("effective_date", obj.Effective_Date);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Met_Factor_History obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Met_Factor_History obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM Tolive.Met_Factor_History");
            sql.AppendLine("WHERE factor_id = :factor_id AND effective_date = :effective_date");
            OracleCommand del = new(sql.ToString(), conn);
            del.Parameters.Add("factor_id", obj.Met_Factor.Factor_Id);
            del.Parameters.Add("effective_date", obj.Effective_Date);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        private static Met_Factor_History DataRowToObject(DataRow row)
        {
            Met_Factor_History RetVal = new();
            RetVal.Met_Factor = Met_FactorSvc.DataRowToObject(row, "mf_");
            RetVal.Effective_Date = row.Field<DateTime>("effective_date");
            RetVal.Factor_Value = row.Field<decimal>("factor_value");
            return RetVal;
        }

    }
}
