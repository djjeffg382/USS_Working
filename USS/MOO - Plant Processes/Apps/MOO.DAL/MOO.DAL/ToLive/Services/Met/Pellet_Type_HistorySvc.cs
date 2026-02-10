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
    public static class Pellet_Type_HistorySvc
    {
        static Pellet_Type_HistorySvc()
        {
            Util.RegisterOracle();
        }



        public static Pellet_Type_History GetLatest(byte Ag_Step, DateTime EffectiveDate)
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT * FROM (");
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE ag_step = :ag_step");
            sql.AppendLine($"AND start_date <= :start_date");
            sql.AppendLine(") WHERE rownbr = 1");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("ag_step", Ag_Step);
            da.SelectCommand.Parameters.Add("start_date", EffectiveDate);

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }

        public static Pellet_Type_History Get(byte Ag_Step, DateTime Start_Date)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE ag_step = :ag_step");
            sql.AppendLine($"AND start_date = :start_date");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("ag_step", Ag_Step);
            da.SelectCommand.Parameters.Add("start_date", Start_Date);

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }


        public static List<Pellet_Type_History> GetAll(byte Ag_Step)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE ag_step = :ag_step");


            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("ag_step", Ag_Step);


            DataSet ds = MOO.Data.ExecuteQuery(da);
            List<Pellet_Type_History> elements = new();
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
            sql.AppendLine("ROW_NUMBER() OVER (PARTITION BY AG_STEP ORDER BY START_DATE DESC) rownbr,");  //this will be used by the GetLatest function
            sql.AppendLine("pt.code pt_code, pt.material pt_material, pt.ptype pt_ptype, pt.marketing_name pt_marketing_name,");
            sql.AppendLine("pth.ag_step, pth.pel_type, pth.start_date");
            sql.AppendLine("FROM tolive.pellet_type_history pth");
            sql.AppendLine("JOIN tolive.pellet_type pt");
            sql.AppendLine("ON pth.pel_type = pt.ptype");
            return sql.ToString();
        }


        public static int Insert(Pellet_Type_History obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Pellet_Type_History obj, OracleConnection conn)
        {
            

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO Tolive.Pellet_Type_History(");
            sql.AppendLine("ag_step, pel_type, start_date)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":ag_step, :pel_type, :start_date)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("ag_step", obj.Ag_Step );
            ins.Parameters.Add("pel_type", obj.Pel_Type.Ptype);
            ins.Parameters.Add("start_date", obj.Start_Date);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        //we won't have an update because there is no good primary key on this table.  We would have to choose which should be updated
        //the date or the pellet type


        public static int Delete(Pellet_Type_History obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Pellet_Type_History obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM Tolive.Pellet_Type_History");
            sql.AppendLine("WHERE ag_step = :ag_step AND start_date = :start_date");
            OracleCommand del = new(sql.ToString(), conn);
            del.Parameters.Add("ag_step", obj.Ag_Step);
            del.Parameters.Add("start_date", obj.Start_Date);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        private static Pellet_Type_History DataRowToObject(DataRow row)
        {
            Pellet_Type_History RetVal = new();
            RetVal.Ag_Step = (byte)row.Field<short>("ag_step");
            RetVal.Pel_Type = Pellet_TypeSvc.DataRowToObject(row,"pt_");
            RetVal.Start_Date = row.Field<DateTime>("start_date");
            return RetVal;
        }

    }
}
