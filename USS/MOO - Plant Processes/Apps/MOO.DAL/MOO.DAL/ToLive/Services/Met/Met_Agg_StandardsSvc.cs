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
    public static class Met_Agg_StandardsSvc
    {
        static Met_Agg_StandardsSvc()
        {
            Util.RegisterOracle();
        }


        public static Met_Agg_Standards Get(DateTime datex)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE datex = :datex");
            sql.AppendLine("AND dmy = 1");
            sql.AppendLine("ORDER BY Datex");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("datex", datex);

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }


        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine("datex, dmy, ltd_standard, ltd_standard_furnace, reduce_standard, ");
            sql.AppendLine("reduce_standard_furnace, new_ltd_reduce_standard");
            sql.AppendLine("FROM tolive.met_agg_standards");
            return sql.ToString();
        }


        public static int Insert(Met_Agg_Standards obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Met_Agg_Standards obj, OracleConnection conn)
        {
            
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Met_Agg_Standards(");
            sql.AppendLine("datex, dmy, ltd_standard, ltd_standard_furnace, reduce_standard, ");
            sql.AppendLine("reduce_standard_furnace, new_ltd_reduce_standard)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":datex, :dmy, :ltd_standard, :ltd_standard_furnace, :reduce_standard, ");
            sql.AppendLine(":reduce_standard_furnace, :new_ltd_reduce_standard)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("datex", obj.Datex);
            ins.Parameters.Add("dmy", (int)obj.Dmy);
            ins.Parameters.Add("ltd_standard", obj.Ltd_Standard);
            ins.Parameters.Add("ltd_standard_furnace", obj.Ltd_Standard_Furnace);
            ins.Parameters.Add("reduce_standard", obj.Reduce_Standard);
            ins.Parameters.Add("reduce_standard_furnace", obj.Reduce_Standard_Furnace);
            ins.Parameters.Add("new_ltd_reduce_standard", obj.New_Ltd_Reduce_Standard);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Met_Agg_Standards obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Met_Agg_Standards obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Met_Agg_Standards SET");
            sql.AppendLine("ltd_standard = :ltd_standard, ");
            sql.AppendLine("ltd_standard_furnace = :ltd_standard_furnace, ");
            sql.AppendLine("reduce_standard = :reduce_standard, ");
            sql.AppendLine("reduce_standard_furnace = :reduce_standard_furnace, ");
            sql.AppendLine("new_ltd_reduce_standard = :new_ltd_reduce_standard");
            sql.AppendLine("WHERE datex = :datex AND dmy = :dmy");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.Parameters.Add("ltd_standard", obj.Ltd_Standard);
            upd.Parameters.Add("ltd_standard_furnace", obj.Ltd_Standard_Furnace);
            upd.Parameters.Add("reduce_standard", obj.Reduce_Standard);
            upd.Parameters.Add("reduce_standard_furnace", obj.Reduce_Standard_Furnace);
            upd.Parameters.Add("new_ltd_reduce_standard", obj.New_Ltd_Reduce_Standard);

            //where clause
            upd.Parameters.Add("datex", obj.Datex);
            upd.Parameters.Add("dmy", (int)obj.Dmy);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }



        private static Met_Agg_Standards DataRowToObject(DataRow row)
        {
            Met_Agg_Standards RetVal = new();
            RetVal.Datex = row.Field<DateTime>("datex");
            RetVal.Dmy = (Met_DMY)row.Field<decimal?>("dmy");
            RetVal.Ltd_Standard = row.Field<decimal?>("ltd_standard");
            RetVal.Ltd_Standard_Furnace = row.Field<decimal?>("ltd_standard_furnace");
            RetVal.Reduce_Standard = row.Field<decimal?>("reduce_standard");
            RetVal.Reduce_Standard_Furnace = row.Field<decimal?>("reduce_standard_furnace");
            RetVal.New_Ltd_Reduce_Standard = row.Field<decimal?>("new_ltd_reduce_standard");
            return RetVal;
        }

    }
}
