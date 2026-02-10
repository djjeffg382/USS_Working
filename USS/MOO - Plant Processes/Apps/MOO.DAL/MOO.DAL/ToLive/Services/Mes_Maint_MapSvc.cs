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
    public static class Mes_Maint_MapSvc
    {
        static Mes_Maint_MapSvc()
        {
            Util.RegisterOracle();
        }

        public static Mes_Maint_Map Get(string Grouping, Byte Sequence)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE Grouping = :Grouping");
            sql.AppendLine("AND Seq = :Sequence");

            Mes_Maint_Map retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("Grouping", Grouping);
            cmd.Parameters.Add("Seq", Sequence);
            OracleDataReader rdr = cmd.ExecuteReader();

            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }

        public static List<Mes_Maint_Map> GetAll()
        {
            StringBuilder sql = new();
            sql.AppendLine(GetSelect());
            sql.AppendLine("ORDER BY seq");

            List<Mes_Maint_Map> retVal = new();
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
            sb.AppendLine("SELECT Grouping, Seq, Value");
            sb.AppendLine("FROM tolive.Mes_Maint_Map");

            return sb.ToString();
        }

        public static int Update(Mes_Maint_Map obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Update(Mes_Maint_Map obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Mes_Maint_Map SET");
            sql.AppendLine("Value = :Value");

            sql.AppendLine("WHERE Grouping = :Grouping");
            sql.AppendLine("AND Seq = :Seq");

            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("Value", obj.Value);

            upd.Parameters.Add("Grouping", obj.Grouping);
            upd.Parameters.Add("Seq", obj.Seq);

            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }

        private static Mes_Maint_Map DataRowToObject(DbDataReader row)
        {
            Mes_Maint_Map retVal = new();
            retVal.Grouping = (string)Util.GetRowVal(row, "Grouping");
            retVal.Seq = (byte)(decimal)Util.GetRowVal(row, "Seq");
            retVal.Value = (string)Util.GetRowVal(row, "Value");

            return retVal;
        }
    }
}
